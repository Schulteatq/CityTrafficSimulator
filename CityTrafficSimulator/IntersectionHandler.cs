/*
 *  CityTrafficSimulator - a tool to simulate traffic in urban areas and on intersections
 *  Copyright (C) 2005-2010, Christian Schulte zu Berge
 *  
 *  This program is free software; you can redistribute it and/or modify it under the 
 *  terms of the GNU General Public License as published by the Free Software 
 *  Foundation; either version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful, but WITHOUT ANY 
 *  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A 
 *  PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along with this 
 *  program; if not, see <http://www.gnu.org/licenses/>.
 * 
 *  Web:  http://www.cszb.net
 *  Mail: software@cszb.net
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using CityTrafficSimulator.Tools;
using CityTrafficSimulator.Vehicle;


namespace CityTrafficSimulator
	{
	/// <summary>
	/// Ein IntersectionHandler kapselt ein oder mehrere Intersections und kümmert sich um die kreuzenden Fahrzeuge. 
	/// 
	/// Jede Intersection ist genau einem IntersectionHandler zugeordnet. Letzterer wird spätestens genau dann erstellt, sobald ein Fahrzeug
	/// in die Nähe der Intersection kommt und verwaltet werden möchte.
	/// Nähert sich ein Fahrzeug einer Intersection, so meldet diese das Fahrzeug am zugehörigen IntersectionHandler an. Dieser prüft zunächst,
	/// ob der intersectionSpace für die Fahrzeuglänge groß genug ist und fügt ggf. weitere Intersections (zwischen denen das Fahrzeug 
	/// mangels Platz nicht halten kann) hinzu. 
	/// Nun kann das Fahrzeug beim IntersectionHandler anfragen, wie es sich verhalten soll. Dieser berechnent anhand der Priorität der 
	/// teilnehmenden NodeConnections und den Ankunftszeitpunkten der Fahrzeuge, welche NodeConnections (und damit Fahrzeuge) fahren dürfen
	/// und welche Fahrzeuge warten müssen.
	/// </summary>
	public class IntersectionHandler : ITickable
		{

		#region Subklassen

		/// <summary>
		/// kapselt einen Pfad von SpecifigIntersections mit darauf fahrenden Autos
		/// </summary>
		private struct IntersectionPath
			{
			private static Pen pen = new Pen(Color.Purple, 2f);

			/// <summary>
			/// Ein Pfad, modelliert durch eine Abfolge von SpecificIntersections
			/// </summary>
			public SortedLinkedList<SpecificIntersection> path;

			/// <summary>
			/// Alle zu behandelnden Autos auf diesem Pfad
			/// </summary>
			public List<IVehicle> vehicles;

			/// <summary>
			/// Konstruktor, der einen path entgegennimmt und eine leere Liste von Autos erstellt
			/// </summary>
			/// <param name="path">Ein Pfad von SpecificIntersections</param>
			public IntersectionPath(SortedLinkedList<SpecificIntersection> path)
				{
				this.path = path;
				this.vehicles = new List<IVehicle>();
				}

			/// <summary>
			/// Zeichnet den IntersectionPath auf die Zeichenfläche g
			/// </summary>
			/// <param name="g">Zeichenfläche auf der gezeichnet werden soll</param>
			public void Draw(Graphics g)
				{
				LinkedListNode<SpecificIntersection> lln = path.First;
				while (lln != null && lln.Next != null)
					{
					g.DrawLine(pen, lln.Value.intersection.aPosition, lln.Next.Value.intersection.aPosition);
					lln = lln.Next;
					}
				}
			}

		#endregion


		#region Felder

		/// <summary>
		/// maximale entfernung zwischen zwei Intersections, die von diesem IntersectionHandler verwaltet werden
		/// </summary>
		private double m_intersectionSpace;
		/// <summary>
		/// maximale entfernung zwischen zwei Intersections, die von diesem IntersectionHandler verwaltet werden
		/// </summary>
		public double intersectionSpace
			{
			get { return m_intersectionSpace; }
			private set { m_intersectionSpace = value; }
			}


		/// <summary>
		/// Liste von verwalteten Intersections
		/// </summary>
		private List<Intersection> m_handledIntersections = new List<Intersection>();
		/// <summary>
		/// Liste von verwalteten Intersections
		/// </summary>
		public List<Intersection> handledIntersections
			{
			get { return m_handledIntersections; }
			set { m_handledIntersections = value; }
			}


		/// <summary>
		/// Liste von verwalteten Wegen durch die IntersectionBunch
		/// </summary>
		private List<IntersectionPath> m_intersectionPaths = new List<IntersectionPath>();
		/// <summary>
		/// Liste von verwalteten Wegen durch die IntersectionBunch
		/// </summary>
		private List<IntersectionPath> intersectionPaths
			{
			get { return m_intersectionPaths; }
			set { m_intersectionPaths = value; }
			}

		/// <summary>
		/// Ministruct, der einen booleschen Wert mit einer NodeConnections kapselt. (Wird vom IntersectionComparer benötigt)
		/// </summary>
		private struct BoolNC
			{
			public bool prev;
			public NodeConnection nc;

			public BoolNC(bool prev, NodeConnection nc)
				{
				this.prev = prev;
				this.nc = nc;
				}
			}

		/// <summary>
		/// Comparer, der zwei SpecificIntersections miteinander vergleicht.
		/// </summary>
		static SortedLinkedList<SpecificIntersection>.CompareDelegate intersectionComparer = delegate(SpecificIntersection a, SpecificIntersection b) {
			if (a.nodeConnection == b.nodeConnection)
				{
				return a.intersection.GetMyTime(a.nodeConnection).CompareTo(b.intersection.GetMyTime(b.nodeConnection));
				}
			else
				{
				Queue<BoolNC> connectionsToCheck = new Queue<BoolNC>();
				foreach (NodeConnection nextConnection in a.nodeConnection.endNode.nextConnections)
					connectionsToCheck.Enqueue(new BoolNC(false, nextConnection));
				foreach (NodeConnection prevConnection in a.nodeConnection.startNode.prevConnections)
					connectionsToCheck.Enqueue(new BoolNC(true, prevConnection));

				while (connectionsToCheck.Count > 0)
					{
					BoolNC bnc = connectionsToCheck.Dequeue();

					if (bnc.prev)
						{
						if (bnc.nc == b.nodeConnection)
							{
							return 1;
							}
						else
							{
							foreach (NodeConnection prevConnection in bnc.nc.startNode.prevConnections)
								connectionsToCheck.Enqueue(new BoolNC(true, prevConnection));
							}
						}
					else
						{
						if (bnc.nc == b.nodeConnection)
							{
							return -1;
							}
						else
							{
							foreach (NodeConnection nextConnection in bnc.nc.endNode.nextConnections)
								connectionsToCheck.Enqueue(new BoolNC(false, nextConnection));
							}
						}
					}
				}
			return 0;
		};

		#endregion


		#region Konstruktoren

		/// <summary>
		/// erstellt einen neuen IntersectionHandler für das Fvahrzeug v, welches auf nc an i vorbeifährt
		/// </summary>
		/// <param name="nc">NodeConnection zur Intersection</param>
		/// <param name="i">Intersection, die verwaltet werden soll</param>
		/// <param name="v">Fahrzeug, welches sich als erstes anmeldet</param>
		public IntersectionHandler(NodeConnection nc, Intersection i, IVehicle v)
			{
			SpecificIntersection si = new SpecificIntersection(nc, i);
			SortedLinkedList<SpecificIntersection> newPath = new SortedLinkedList<SpecificIntersection>(intersectionComparer);
			newPath.Add(si);

			IntersectionPath ip = new IntersectionPath(newPath);
			ip.vehicles.Add(v);

			m_intersectionPaths.Add(ip);
			UpdateIntersectionPath(ip, v);

			m_intersectionPaths.Add(ip);
			}

		#endregion



		#region Methoden

		/// <summary>
		/// Aktualisiert den IntersectionPath insoweit, als dass für das Fahrzeug v alle die Intersections drangehängt werden, 
		/// die so nah sind, dass v dazwischen nicht halten kann ohne eine Intersection zu blockieren
		/// </summary>
		/// <param name="ip">zu untersuchender intersectionPath</param>
		/// <param name="v">Fahrzeug als Prüfgrundlage</param>
		private void UpdateIntersectionPath(IntersectionPath ip, IVehicle v)
			{
			/*
			// hinten prüfen
			while (true)
				{
				SpecificIntersection si = ip.path.Last.Value;
				// TODO: distance Berechnung etwas ungenau, aber fürs erste reichts hoffentlich
				double distance = 2 * si.intersection.GetWaitingDistance() + v.s0 + v.length;
				double rightIntervalBorder = si.intersection.GetMyArcPosition(si.nodeConnection) + distance;

				// brauche nur auf der aktuellen NodeConnection zu suchen:
				LinkedListNode<Intersection> nextNode = si.intersection.GetMyListNode(si.nodeConnection).Next;
				if (nextNode != null)
					{
					if (nextNode.Value.GetMyArcPosition(si.nodeConnection) <= rightIntervalBorder + nextNode.Value.GetWaitingDistance() - si.intersection.GetWaitingDistance())
						{
						//TODO : AddLast() müsste es hier eigentlich auch und schneller tun, aber sicher ist erstmal sicher
						if (nextNode.Value.handler == null || nextNode.Value.handler == this)
							ip.path.AddLast(new SpecificIntersection(si.nodeConnection, nextNode.Value));
						else
							MergeIntersectionHandler(ip, new SpecificIntersection(si.nodeConnection, nextNode.Value), nextNode.Value.handler);
						continue;
						}
					else
						break;
					}
				// prüfe nun noch die vorherigen NodeConnections
				else if (rightIntervalBorder > si.nodeConnection.lineSegment.length)
					{
					int foo = v.WayToGo.route.Count;

					// Workaround:
					v.WayToGo.route.Push(v.currentNodeConnection);
					// :Workaround


					bool doContinue = false;
					rightIntervalBorder -= si.nodeConnection.lineSegment.length;

					// erstmal suchen wir die entsprechende NodeConnection im WayToGo vom Fahrzeug
					// Da wir dabei nur auf das oberste Element zugreifen können wird alles was runtergepoppt wird in backup-Stack gesichert

					/*
					 * OK, Problem erkannt:
					 * Das Verfahren hier funktioniert nicht für intersectionPaths, die sich verzweigen. 
					 * 
					 * Gefahr ist leider noch nicht gebannt...
					 */
			/*
					Stack<NodeConnection> backup = new Stack<NodeConnection>();
					NodeConnection nc = v.WayToGo.route.Pop();
					backup.Push(nc);
					while (nc != si.nodeConnection)
						{
						nc = v.WayToGo.route.Pop();
						backup.Push(nc);
						}


					while (rightIntervalBorder > 0)
						{
						nc = v.WayToGo.route.Pop();
						backup.Push(nc);
						nextNode = nc.intersections.First;

						if (nextNode != null)
							{
							if (nextNode.Value.GetMyArcPosition(nc) <= rightIntervalBorder)
								{
								if (nextNode.Value.handler == null || nextNode.Value.handler == this)
									ip.path.AddLast(new SpecificIntersection(nc, nextNode.Value));
								else
									MergeIntersectionHandler(ip, new SpecificIntersection(nc, nextNode.Value), nextNode.Value.handler);
								doContinue = true;
								}
							break;
							}
						else
							{
							rightIntervalBorder -= nc.lineSegment.length;
							}
						}
					
					// wiederherstellen
					while (backup.Count > 1)
						{
						v.WayToGo.route.Push(backup.Pop());
						}

					if (foo != v.WayToGo.route.Count)
						{
						throw new Exception();
						}

					if (doContinue) 
						continue;
					else 
						break;
					}
				else
					break;
				}
			 * */
			}


		/// <summary>
		/// Fügt den kompletten IntersectionHandler ih hinzu
		/// </summary>
		/// <param name="ip">IntersectionPath, bei dem ih angehängt werden soll</param>
		/// <param name="si">SpecificIntersection, bei dem this und ih sich überschneiden</param>
		/// <param name="otherIH">IntersectionHandler, der hinzugefügt werden soll</param>
		private void MergeIntersectionHandler(IntersectionPath ip, SpecificIntersection si, IntersectionHandler otherIH)
			{
			//TODO:	momentan ist das mergen noch sehr stümperhaft und simpel.
			//		Man sollte beim Debuggen nochmal genau hinschauen, ob dies genügt, 
			//		oder an dieser Stelle tiefgründigere Untersuchungen stattfinden müssen
			foreach (IntersectionPath iip in otherIH.m_intersectionPaths)
				{
				// erstmal gucken, ob sll eine Weiterführung von path ist
				if (SpecificIntersection.Equals(si, iip.path.First.Value))
					{
					// dann alle Elemente dem Path hinzufügen und die handler umbiegen
					foreach (SpecificIntersection foo in iip.path)
						{
						ip.path.Add(foo);
						foo.intersection.handler = this;
						}
					// handledVehicles hinzufügen
					ip.vehicles.AddRange(iip.vehicles);
					}
				// sonst einfach den ganzen Path als solches hinzufügen die handler umbiegen
				else
					{
					m_intersectionPaths.Add(iip);
					foreach (SpecificIntersection foo in iip.path)
						{
						foo.intersection.handler = this;
						}
					}
				}

			foreach (IntersectionPath iip in m_intersectionPaths)
				{
				foreach (SpecificIntersection ssi in iip.path)
					{
					ssi.intersection.handler = this;
					}
				}
			// wir haben fertig?
			}

		/// <summary>
		/// Meldet ein Fahrzeug für diesen IntersectionHandler an
		/// </summary>
		/// <param name="nc">NodeConnection zur Intersection</param>
		/// <param name="i">Intersection des IntersectionHandlers, die zuerst erreicht wird</param>
		/// <param name="v">Fahrzeug, welches sich anmelden will</param>
		public IntersectionHandler RegisterVehicle(NodeConnection nc, Intersection i, IVehicle v)
			{
			return this;
			/*
			SpecificIntersection si = new SpecificIntersection(nc, i);

			// gucken, ob schon ein IntersectionPath mit si exisitiert
			foreach (IntersectionPath path in m_intersectionPaths)
				{
				LinkedListNode<SpecificIntersection> lln = path.path.First;
				while (lln != null)
					{
					if (SpecificIntersection.Equals(si, lln.Value))
						{
						// hier müssen wir nun noch prüfen, ob auch wirklich der gesamte Pfad durchlaufen wird
						// ob also alle folgenden SpecificIntersections auch auf v.WayToGo liegen.
						// TODO: diese Lösung ist nicht hübsch, aber erstmal effektiv:

						// erstmal suchen wir die entsprechende NodeConnection im WayToGo vom Fahrzeug
						// Da wir dabei nur auf das oberste Element zugreifen können wird alles was runtergepoppt wird in backup-Stack gesichert
						v.WayToGo.route.Push(v.currentNodeConnection);
						Stack<NodeConnection> backup = new Stack<NodeConnection>();
						NodeConnection currentNC = v.WayToGo.route.Pop();
						backup.Push(currentNC);
						while (currentNC != lln.Value.nodeConnection)
							{
							currentNC = v.WayToGo.route.Pop();
							backup.Push(currentNC);
							}

						// Nun prüfen, ob der intersectionPath komplett auf v.WayToGo liegt
						while (lln != null)
							{
							// Wegpunkt liegt auf currentNC von WayToGo
							if (lln.Value.nodeConnection == currentNC)
								{
								lln = lln.Next;
								}
							// Wegpunkt liegt nicht auf currentNC von WayToGo
							else
								{
								// gucken, ob WayToGo noch was enthält 
								// TODO: hier könnte man noch ne schneller Abbruchbedingung finden, um die Laufzeit zu reduzieren
								if (v.WayToGo.route.Count > 0)
									{
									currentNC = v.WayToGo.route.Pop();
									backup.Push(currentNC);
									}
								// anscheinend liegt dieser Wegpunkt NICHT auf v.WayToGo, also lassen wir ab dem Vorgänger einen neuen IntersectionPath beginnen
								else
									{
									// WayToGo wiederherstellen
									while (backup.Count > 1)
										v.WayToGo.route.Push(backup.Pop());

									SortedLinkedList<SpecificIntersection> abzweigung = new SortedLinkedList<SpecificIntersection>(intersectionComparer);
									abzweigung.Add(lln.Previous.Value);

									IntersectionPath ip = new IntersectionPath(abzweigung);
									ip.vehicles.Add(v);

									UpdateIntersectionPath(ip, v);

									m_intersectionPaths.Add(ip);

									return this;
									}
								}
							}

						// wiederherstellen
						while (backup.Count > 1)
							{
							v.WayToGo.route.Push(backup.Pop());
							}


						path.vehicles.Add(v);
						UpdateIntersectionPath(path, v);

						// abbrechen
						return this;
						}
					lln = lln.Next;
					}
				}

			// anscheinend exisitiert noch kein passender IntersectionPath, also legen wir einen neuen an
			SortedLinkedList<SpecificIntersection> newPath = new SortedLinkedList<SpecificIntersection>(intersectionComparer);
			newPath.Add(si);

			IntersectionPath iip = new IntersectionPath(newPath);
			iip.vehicles.Add(v);

			m_intersectionPaths.Add(iip);
			UpdateIntersectionPath(iip, v);

			m_intersectionPaths.Add(iip);
			return this;
			*/
			}


		/// <summary>
		/// Entfernt die Intersection i aus dieser Verwaltungseinheit
		/// </summary>
		/// <param name="i">Intersection die entfernt werden soll</param>
		public void RemoveHandledIntersection(Intersection i)
			{
			List<IntersectionPath> pathsToRemove = new List<IntersectionPath>();

			// zunächst alle IntersectionPaths durchgehen und gucken, ob er i enthält
			foreach (IntersectionPath ip in m_intersectionPaths)
				{
				LinkedListNode<SpecificIntersection> lln = ip.path.First;
				while (lln != null)
					{
					// Intersection aus dem Path löschen und ip zum löschen vormerken
					if (lln.Value.intersection == i)
						{
						pathsToRemove.Add(ip);
						break;
						}
					lln = lln.Next;
					}
				}

			// nun alle entsprechenden IntersectionPaths löschen
			foreach (IntersectionPath ip in pathsToRemove)
				{
				RemoveIntersectionPath(ip);
				}
			}


		/// <summary>
		/// Entfernt den IntersectionPath ip und meldet ihn bei allen teilnehmenden Intersections und Fahrzeugen ab
		/// </summary>
		/// <param name="ip">zu entfernender IntersectionPath</param>
		private void RemoveIntersectionPath(IntersectionPath ip)
			{
			foreach (SpecificIntersection si in ip.path)
				{
				si.intersection.handler = null;
				}
			foreach (IVehicle v in ip.vehicles)
				{
				v.registeredIntersectionHandlers.Remove(this);
				}

			m_intersectionPaths.Remove(ip);
			}



		/// <summary>
		/// Zeichnet Debuginformationen zum IntersectionHandler auf die Zeichenfläche g
		/// </summary>
		/// <param name="g">Zeichenfläche auf die gezeichnet werden soll</param>
		public void Draw(Graphics g)
			{
			
			foreach (IntersectionPath ip in m_intersectionPaths)
				{
				ip.Draw(g);
				}
			
			}


		#endregion



		#region ITickable Member

		/// <summary>
		/// lässt die Zeit um einen Tick voranschreiten
		/// </summary>
		/// <param name="tickLength">Länge eines Ticks in Sekunden (berechnet sich mit 1/#Ticks pro Sekunde)</param>
		/// <param name="currentTime">aktuelle Zeit in Sekunden nach Sekunde 0</param>
		public void Tick(double tickLength, double currentTime)
			{
			throw new NotImplementedException();
			}

		/// <summary>
		/// sagt dem Objekt Bescheid, dass der Tick vorbei ist.
		/// (wir zur Zeit nur von IVehicle benötigt)
		/// </summary>
		public void Reset()
			{
			throw new NotImplementedException();
			}

		#endregion
		}
	}
