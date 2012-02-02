/*
 *  CityTrafficSimulator - a tool to simulate traffic in urban areas and on intersections
 *  Copyright (C) 2005-2012, Christian Schulte zu Berge
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

namespace CityTrafficSimulator
	{
	/// <summary>
	/// Wegroute zu einem Zielknoten. Rückgabetyp des A*-Algorithmus
	/// </summary>
	public class Routing : IEnumerable<Routing.RouteSegment>
		{
		/// <summary>
		/// Wegroute
		/// </summary>
		private LinkedList<RouteSegment> route;

		/// <summary>
		/// Kosten der gesamten Route
		/// </summary>
		public double costs;

		/// <summary>
		/// Anzahl der benötigten Spurwechsel
		/// </summary>
		public int countOfLineChanges;


		/// <summary>
		/// Standardkonstruktor, erstellt eine neue leere Wegroute zu einem Zielknoten
		/// </summary>
		public Routing()
			{
			route = new LinkedList<RouteSegment>();
			costs = 0;
			countOfLineChanges = 0;
			}


		/// <summary>
		/// Pusht das übergebene RouteSegment auf den route-Stack und aktualisiert die Kosten und Anzahl benötigter Spurwechsel
		/// </summary>
		/// <param name="rs">einzufügendes RouteSegment</param>
		public void Push(RouteSegment rs)
			{
			route.AddFirst(rs);
			costs += rs.costs;
			if (rs.lineChangeNeeded)
				++countOfLineChanges;
			}

		/// <summary>
		/// Poppt das oberste Element vom route-Stack und aktualisiert das length-Feld
		/// </summary>
		/// <returns>route.First.Value</returns>
		public RouteSegment Pop()
			{
			if (route.Count > 0)
				{
				RouteSegment rs = route.First.Value;
				costs -= rs.costs;
				if (rs.lineChangeNeeded)
					{
					--countOfLineChanges;
					}

				route.RemoveFirst();
				return rs;
				}
			else
				{
				return null;
				}
			}

		/// <summary>
		/// Liefert das oberste Element vom route-Stack zurück
		/// </summary>
		/// <returns>route.First.Value</returns>
		public RouteSegment Top()
			{
			return route.First.Value;
			}

		/// <summary>
		/// gibt die Anzahl der Elemente des route-Stacks zurück
		/// </summary>
		/// <returns>route.Count</returns>
		public int SegmentCount()
			{
			return route.Count;
			}

		#region IEnumerable<RouteSegment> Member

		/// <summary>
		/// Gibt den Enumerator für foreach-Schleifen zurück
		/// </summary>
		/// <returns>route.GetEnumerator()</returns>
		public IEnumerator<RouteSegment> GetEnumerator()
			{
			return route.GetEnumerator();
			}

		/// <summary>
		/// Gibt den Enumerator für foreach-Schleifen zurück
		/// </summary>
		/// <returns>route.GetEnumerator()</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
			return route.GetEnumerator();
			}

		#endregion

		/// <summary>
		/// berechnet die minimale euklidische Entfernung von startNode zu einem der Knoten aus targetNodes
		/// </summary>
		/// <param name="startNode">Startknoten von dem aus die Entfernung berechnet werden soll</param>
		/// <param name="targetNodes">Liste von LineNodes zu denen die Entfernung berechnet werden soll</param>
		/// <returns>minimale euklidische Distanz</returns>
		private static double GetMinimumEuklidDistance(LineNode startNode, List<LineNode> targetNodes)
			{
			if (targetNodes.Count > 0)
				{
				double minValue = Vector2.GetDistance(startNode.position, targetNodes[0].position);

				for (int i = 1; i < targetNodes.Count; i++)
					{
					double newDist = Vector2.GetDistance(startNode.position, targetNodes[i].position);
					if (newDist < minValue)
						{
						minValue = newDist;
						}
					}

				return minValue;
				}
			return 0;
			}

		/// <summary>
		/// Checks, whether the given vehicle type is allowed on ALL NodeConnections incoming in ln.
		/// </summary>
		/// <param name="ln">LineNode to check</param>
		/// <param name="type">Vehicle type to check</param>
		/// <returns>true, if the given vehicle type is allowed on ALL NodeConnections running to ln.</returns>
		public static bool CheckLineNodeForIncomingSuitability(LineNode ln, Vehicle.IVehicle.VehicleTypes type)
			{
			foreach (NodeConnection nc in ln.prevConnections)
				{
				if (!nc.CheckForSuitability(type))
					return false;
				}
			return true;
			}

		/// <summary>
		/// Berechnet den kürzesten Weg zum targetNode und speichert diesen als Stack in WayToGo
		/// Implementierung des A*-Algorithmus' frei nach Wikipedia :)
		/// </summary>
		/// <param name="startNode">Startknoten von dem aus der kürzeste Weg berechnet werden soll</param>
		/// <param name="targetNodes">Liste von Zielknoten zu einem von denen der kürzeste Weg berechnet werden soll</param>
		/// <param name="vehicleType">Vehicle type</param>
		public static Routing CalculateShortestConenction(LineNode startNode, List<LineNode> targetNodes, Vehicle.IVehicle.VehicleTypes vehicleType)
			{
			PriorityQueue<LineNode.LinkedLineNode, double> openlist = new PriorityQueue<LineNode.LinkedLineNode, double>();
			Stack<LineNode.LinkedLineNode> closedlist = new Stack<LineNode.LinkedLineNode>();
			Routing toReturn = new Routing();
			
			// Initialisierung der Open List, die Closed List ist noch leer
			// (die Priorität bzw. der f Wert des Startknotens ist unerheblich)
			openlist.Enqueue(new LineNode.LinkedLineNode(startNode, null, false), 0);

			// diese Schleife wird durchlaufen bis entweder
			// - die optimale Lösung gefunden wurde oder
			// - feststeht, dass keine Lösung existiert
			do
				{
				// Knoten mit dem geringsten (in diesem Fall größten) f Wert aus der Open List entfernen
				PriorityQueueItem<LineNode.LinkedLineNode, double> currentNode = openlist.Dequeue();

				// wurde das Ziel gefunden?
				if (targetNodes.Contains(currentNode.Value.node))
					{
					// nun noch die closedList in eine Routing umwandeln
					closedlist.Push(currentNode.Value);
					LineNode.LinkedLineNode endnode = closedlist.Pop();
					LineNode.LinkedLineNode startnode = endnode.parent;
					while (startnode != null)
						{
						// einfacher/direkter Weg über eine NodeConnection
						if (!endnode.lineChangeNeeded)
							{
							toReturn.Push(new RouteSegment(startnode.node.GetNodeConnectionTo(endnode.node), endnode.node, false, startnode.node.GetNodeConnectionTo(endnode.node).lineSegment.length));
							}
						// Spurwechsel nötig
						else
							{
							NodeConnection formerConnection = startnode.parent.node.GetNodeConnectionTo(startnode.node);

							double length = formerConnection.GetLengthToLineNodeViaLineChange(endnode.node) + Constants.lineChangePenalty;
							// Anfangs-/ oder Endknoten des Spurwechsels ist eine Ampel => Kosten-Penalty, da hier verstärktes Verkehrsaufkommen zu erwarten ist
							if ((endnode.node.tLight != null) || (startnode.node.tLight != null))
								length += Constants.lineChangeBeforeTrafficLightPenalty;

							toReturn.Push(new RouteSegment(formerConnection, endnode.node, true, length));

							// TODO:	Erklären: hier wird irgendwas doppelt gemacht - ich meine mich zu Erinnern,
							//			das das so soll, aber nicht warum. Bitte beizeiten analysieren und erklären
							endnode = startnode;
							startnode = startnode.parent;
							}

						endnode = startnode;
						startnode = startnode.parent;
						}
					return toReturn;
					}

				#region Nachfolgeknoten auf die Open List setzen
				// Nachfolgeknoten auf die Open List setzen
				// überprüft alle Nachfolgeknoten und fügt sie der Open List hinzu, wenn entweder
				// - der Nachfolgeknoten zum ersten Mal gefunden wird oder
				// - ein besserer Weg zu diesem Knoten gefunden wird

				#region nächste LineNodes ohne Spurwechsel untersuchen
				foreach (NodeConnection nc in currentNode.Value.node.nextConnections)
					{
					// prüfen, ob ich auf diesem NodeConnection überhaupt fahren darf
					if (!nc.CheckForSuitability(vehicleType))
						continue;

					LineNode.LinkedLineNode successor = new LineNode.LinkedLineNode(nc.endNode, null, false);
					bool nodeInClosedList = false;
					foreach (LineNode.LinkedLineNode lln in closedlist)
						if (lln.node == successor.node)
							{
							nodeInClosedList = true;
							continue;
							}

					// wenn der Nachfolgeknoten bereits auf der Closed List ist - tue nichts
					if (!nodeInClosedList)
						{
						NodeConnection theConnection = currentNode.Value.node.GetNodeConnectionTo(successor.node);
						// f Wert für den neuen Weg berechnen: g Wert des Vorgängers plus die Kosten der
						// gerade benutzten Kante plus die geschätzten Kosten von Nachfolger bis Ziel
						double f = currentNode.Value.length												// exakte Länge des bisher zurückgelegten Weges
							+ theConnection.lineSegment.length;											// exakte Länge des gerade untersuchten Segmentes

						if (currentNode.Value.countOfParents < 3)										// Stau kostet extra, aber nur, wenn innerhalb
							{																			// der nächsten 2 Connections
							f += theConnection.vehicles.Count * Constants.vehicleOnRoutePenalty;
							}
						f += GetMinimumEuklidDistance(successor.node, targetNodes);						// Minimumweg zum Ziel (Luftlinie)
						f *= 14 / theConnection.targetVelocity;
						f *= -1;


						// gucke, ob der Node schon in der Liste drin ist und wenn ja, dann evtl. rausschmeißen
						bool nodeInOpenlist = false;
						foreach (PriorityQueueItem<LineNode.LinkedLineNode, double> pqi in openlist)
							{
							if (pqi.Value.node == successor.node)
								{
								if (f <= pqi.Priority)
									nodeInOpenlist = true;
								else
									openlist.Remove(pqi.Value); // erst entfernen
								break;
								}
							}

						if (!nodeInOpenlist)
							{
							// Vorgängerzeiger setzen
							successor.parent = currentNode.Value;
							openlist.Enqueue(successor, f); // dann neu einfügen
							}
						}
					}
				#endregion

				#region nächste LineNodes mit Spurwechsel untersuchen

				if (currentNode.Value.parent != null)
					{
					NodeConnection currentConnection = currentNode.Value.parent.node.GetNodeConnectionTo(currentNode.Value.node);
					if (currentConnection != null)
						{
						foreach (LineNode ln in currentConnection.viaLineChangeReachableNodes)
							{
							// prüfen, ob ich diesen LineNode überhaupt anfahren darf
							if (!CheckLineNodeForIncomingSuitability(ln, vehicleType))
								continue;

							// neuen LinkedLineNode erstellen
							LineNode.LinkedLineNode successor = new LineNode.LinkedLineNode(ln, null, true);
							bool nodeInClosedList = false;
							foreach (LineNode.LinkedLineNode lln in closedlist)
								if (lln.node == successor.node)
									{
									nodeInClosedList = true;
									break;
									}

							// wenn der Nachfolgeknoten bereits auf der Closed List ist - tue nichts
							if (!nodeInClosedList)
								{
								// passendes LineChangeInterval finden
								NodeConnection.LineChangeInterval lci;
								currentConnection.lineChangeIntervals.TryGetValue(ln.hashcode, out lci);

								if (lci.length < Constants.minimumLineChangeLength)
									break;

								// f-Wert für den neuen Weg berechnen: g Wert des Vorgängers plus die Kosten der
								// gerade benutzten Kante plus die geschätzten Kosten von Nachfolger bis Ziel
								double f = currentNode.Value.parent.length;										// exakte Länge des bisher zurückgelegten Weges
								f += currentConnection.GetLengthToLineNodeViaLineChange(successor.node);

								// Kostenanteil, für den Spurwechsel dazuaddieren
								f += (lci.length < 2 * Constants.minimumLineChangeLength) ? 2 * Constants.lineChangePenalty : Constants.lineChangePenalty;

								// Anfangs-/ oder Endknoten des Spurwechsels ist eine Ampel => Kosten-Penalty, da hier verstärktes Verkehrsaufkommen zu erwarten ist
								if ((lci.targetNode.tLight != null) || (currentConnection.startNode.tLight != null))
									f += Constants.lineChangeBeforeTrafficLightPenalty;

								f += GetMinimumEuklidDistance(successor.node, targetNodes);						// Minimumweg zum Ziel (Luftlinie)
								f *= -1;


								// gucke, ob der Node schon in der Liste drin ist und wenn ja, dann evtl. rausschmeißen
								bool nodeInOpenlist = false;
								foreach (PriorityQueueItem<LineNode.LinkedLineNode, double> pqi in openlist)
									{
									if (pqi.Value.node == successor.node)
										{
										if (f <= pqi.Priority)
											nodeInOpenlist = true;
										else
											openlist.Remove(pqi.Value); // erst entfernen
										break;
										}
									}

								if (!nodeInOpenlist)
									{
									// Vorgängerzeiger setzen
									successor.parent = currentNode.Value;
									openlist.Enqueue(successor, f); // dann neu einfügen
									}
								}
							}
						}
					}


				#endregion

				#endregion

				// der aktuelle Knoten ist nun abschließend untersucht
				closedlist.Push(currentNode.Value);
				}
			while (openlist.Count != 0);

			// Es wurde kein Weg gefunden - dann lassen wir das Auto sich selbst zerstören:
			return toReturn;
			}

		/// <summary>
		/// Teilstück einer Wegroute. Entweder eine der endNode der aktuellen NodeConnection, der endNode einer parallelen NodeConnection, zu der ein Spurwechsel nötig ist
		/// </summary>
		public class RouteSegment
			{
			/// <summary>
			/// NodeConnection auf der dieses RouteSegment beginnt (enden kann es auf einer andere, wenn ein Spurwechsel nötig ist)
			/// </summary>
			public NodeConnection startConnection;

			/// <summary>
			/// LineNode, der als nächstes angefahren werden soll
			/// </summary>
			public LineNode nextNode;

			/// <summary>
			/// Flag, ob dazu ein Spurwechsel nötig ist
			/// </summary>
			public bool lineChangeNeeded;

			/// <summary>
			/// Kosten dieses Teilstücks (mindestens Länge der NodeConnection, plus evtl. Strafkosten für teure Spurwechsel
			/// </summary>
			public double costs;


			/// <summary>
			/// Standardkonstruktor, erstellt eine neues Routen-Teilstück
			/// </summary>
			/// <param name="startConnection">NodeConnection auf der dieses RouteSegment beginnt (enden kann es auf einer andere, wenn ein Spurwechsel nötig ist)</param>
			/// <param name="nextNode">LineNode, der als nächstes angefahren werden soll</param>
			/// <param name="lineChangeNeeded">Flag, ob dazu ein Spurwechsel nötig ist</param>
			/// <param name="costs">Kosten dieses Teilstücks (mindestens Länge der NodeConnection, plus evtl. Strafkosten für teure Spurwechsel</param>
			public RouteSegment(NodeConnection startConnection, LineNode nextNode, bool lineChangeNeeded, double costs)
				{
				this.startConnection = startConnection;
				this.nextNode = nextNode;
				this.lineChangeNeeded = lineChangeNeeded;
				this.costs = costs;
				}
			}
		}
	}
