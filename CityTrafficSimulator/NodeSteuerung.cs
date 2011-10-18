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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using CityTrafficSimulator.Vehicle;
using CityTrafficSimulator.Tools.ObserverPattern;

namespace CityTrafficSimulator
	{
	/// <summary>
	/// Klasse zur Steuerung von LineNodes, NodeConnections etc.
	/// </summary>
	public class NodeSteuerung : ISavable, ITickable
		{
		#region Variablen und Felder

		/// <summary>
		/// alle verwendenten LineNodes
		/// </summary>
		private List<LineNode> m_nodes = new List<LineNode>();
		/// <summary>
		/// alle verwendenten LineNodes
		/// </summary>
		public List<LineNode> nodes
			{
			get { return m_nodes; }
			set { m_nodes = value; }
			}

		/// <summary>
		/// alle verwendeten NodeConnections
		/// </summary>
		private List<NodeConnection> m_connections = new List<NodeConnection>();
		/// <summary>
		/// alle verwendeten NodeConnections
		/// </summary>
		public List<NodeConnection> connections
			{
			get { return m_connections; }
			set { m_connections = value; }
			}

		/// <summary>
		/// Liste aller bekannten Intersections
		/// </summary>
		private List<Intersection> m_intersections = new List<Intersection>();
		/// <summary>
		/// Liste aller bekannten Intersections
		/// </summary>
		public List<Intersection> intersections
			{
			get { return m_intersections; }
			set { m_intersections = value; }
			}


		/// <summary>
		/// aktuell ausgewählter LineNode
		/// </summary>
		private LineNode m_selectedLineNode;
		/// <summary>
		/// aktuell ausgewählter LineNode
		/// </summary>
		public LineNode selectedLineNode
			{
			get { return m_selectedLineNode; }
			set { m_selectedLineNode = value; }
			}

		/// <summary>
		/// aktuell ausgewählte NodeConnection
		/// </summary>
		private NodeConnection m_selectedNodeConnection;
		/// <summary>
		/// aktue ausgewählte NodeConnection
		/// </summary>
		public NodeConnection selectedNodeConnection
			{
			get { return m_selectedNodeConnection; }
			set { m_selectedNodeConnection = value; }
			}


		/// <summary>
		/// Titel dieses Layouts
		/// </summary>
		private string m_title;
		/// <summary>
		/// Titel dieses Layouts
		/// </summary>
		public string title
			{
			get { return m_title; }
			set { m_title = value; }
			}


		/// <summary>
		/// Informationstext zum Layout
		/// </summary>
		private string m_infoText;
		/// <summary>
		/// Informationstext zum Layout
		/// </summary>
		public string infoText
			{
			get { return m_infoText; }
			set { m_infoText = value; }
			}


		#endregion

		#region Delegates und Beobachter-Muster

		/// <summary>
		/// Delegate für Zuhörer/Beobachter-Strukturmuster
		/// (Statt Interfaces nutzen wir Delegates, welche übergeben und aufgerufen werden)
		/// </summary>
		public delegate void ActionMethod();


		/// <summary>
		/// Liste aller Methoden die aufgerufen werden, wenn sich nodes ändert
		/// </summary>
		private List<ActionMethod> nodesChangedActions = new List<ActionMethod>();

		/// <summary>
		/// Liste aller Methoden die aufgerufen werden, wenn sich connections ändert
		/// </summary>
		private List<ActionMethod> connectionsChangedActions = new List<ActionMethod>();


		/// <summary>
		/// Meldet eine Funktion an, die beim Ändern von nodes aufgerufen werden soll
		/// </summary>
		/// <param name="am">Funktion ohne Argumente</param>
		public void AddNodesChangedAction(ActionMethod am)
			{
			nodesChangedActions.Add(am);
			}
		
		/// <summary>
		/// Meldet eine Funktion an, die beim Ändern von connections aufgerufen werden soll
		/// </summary>
		/// <param name="am">Funktion ohne Argumente</param>
		public void AddConnectionsChangedAction(ActionMethod am)
			{
			connectionsChangedActions.Add(am);
			}


		/// <summary>
		/// sage allen Beobachtern Bescheid, dass nodes sich geändert hat
		/// </summary>
		private void NodesChanged()
			{
			foreach (ActionMethod am in nodesChangedActions)
				{
				am();
				}
			}

		/// <summary>
		/// sage allen Beobachtern Bescheid, dass connections sich geändert hat
		/// </summary>
		private void connectionsChanged()
			{
			foreach (ActionMethod am in connectionsChangedActions)
				{
				am();
				}
			}
		
		#endregion

		#region Konstruktoren

		/// <summary>
		/// leerer Standardkonstruktor
		/// </summary>
		public NodeSteuerung()
			{
			// TODO: muss hier irgendwas passieren?
			}

		#endregion

		#region Methoden für IVehicles

		/// <summary>
		/// Erstellt ein neues IVehicle, welches von startNode nach targetNode fahren möchte
		/// </summary>
		/// <param name="v">zu erstellendes IVehicle</param>
		/// <param name="startNode">Startknoten</param>
		/// <param name="targetNodes">Liste von Zielknoten</param>
		public void AddVehicle(IVehicle v, LineNode startNode, List<LineNode> targetNodes)
			{
			if (startNode.nextConnections.Count > 0)
				{
				// Neue Linie zum Weiterfahren bestimmen
				Random rnd = new Random();
				int foo = rnd.Next(startNode.nextConnections.Count);

				IVehicle.State state = new IVehicle.State(startNode.nextConnections[foo], 0);
				v.state = state;
				
				startNode.nextConnections[foo].AddVehicle(v, targetNodes);

				rnd = null;
				}
			}

		#endregion

		#region Methoden für LineNodes

		/// <summary>
		/// fügt einen LineNode hinzu
		/// </summary>
		/// <param name="ln">der hinzuzufügende LineNode</param>
		public void AddLineNode(LineNode ln)
			{
			nodes.Add(ln);
			NodesChanged();
			}


		/// <summary>
		/// Löscht einen LineNode und alle damit verbundenen NodeConnections
		/// </summary>
		/// <param name="ln">zu löschender LineNode</param>
		public void DeleteLineNode(LineNode ln)
			{
			// ausgehende NodeConnections löschen
			while (ln.nextConnections.Count > 0)
				{
				Disconnect(ln, ln.nextConnections[0].endNode);
				}

			// eingehende NodeConnections löschen
			while (ln.prevConnections.Count > 0)
				{
				Disconnect(ln.prevConnections[0].startNode, ln);
				}

			nodes.Remove(ln);
			}


		/// <summary>
		/// stellt eine NodeConnection von from nach to her
		/// </summary>
		/// <param name="from">LineNode von dem die NodeConnection ausgehen soll</param>
		/// <param name="to">LineNode zu der die NodeConnection hingehen soll</param>
		/// <param name="priority">Priorität der Linie</param>
		/// <param name="carsAllowed">Flag, ob Autos auf dieser NodeConnection erlaubt sind</param>
		/// <param name="busAllowed">Flag, ob Busse auf dieser NodeConnection erlaubt sind</param>
		/// <param name="tramAllowed">Flag, ob Straßenbahnen auf dieser NodeConnection erlaubt sind</param>
		/// <param name="enableIncomingLineChange">Flag, ob eingehende Spurwechsel erlaubt sind</param>
		/// <param name="enableOutgoingLineChange">Flag, ob ausgehende Spurchwechsel erlaubt sind</param>
		public void Connect(LineNode from, LineNode to, int priority, bool carsAllowed, bool busAllowed, bool tramAllowed, bool enableIncomingLineChange, bool enableOutgoingLineChange)
			{
			NodeConnection nc = new NodeConnection(from, to, null, priority, carsAllowed, busAllowed, tramAllowed, enableIncomingLineChange, enableOutgoingLineChange);

			TellNodesTheirConnection(nc);
			UpdateNodeConnection(nc);

			connections.Add(nc);
			connectionsChanged();
			}

		/// <summary>
		/// setzt nextNodes und prevNodes bei den an nc teilnehmenden LineNodes
		/// </summary>
		/// <param name="nc">NodeConnections die angemeldet werden soll</param>
		private void TellNodesTheirConnection(NodeConnection nc)
			{
			nc.startNode.nextConnections.Add(nc);
			nc.endNode.prevConnections.Add(nc);
			}

		/// <summary>
		/// kappt die sämtliche bestehende NodeConnection zwischen from und to und sagt den Nodes auch Bescheid, dass diese nicht mehr bestehen
		/// </summary>
		/// <param name="from">LineNode von dem die NodeConnection ausgeht</param>
		/// <param name="to">LineNode zu der die NodeConnection hingehet</param>
		public void Disconnect(LineNode from, LineNode to)
			{
			NodeConnection nc;
			while ((nc = GetNodeConnection(from, to)) != null)
				{
				// Intersections löschen 
				while (nc.intersections.Count > 0)
					{
					DestroyIntersection(nc.intersections.First.Value);
					}

				// LineChangePoints löschen, so sie denn exisitieren
				RemoveLineChangePoints(nc, true, true);

				// Connections lösen und löschen
				from.nextConnections.Remove(nc);
				to.prevConnections.Remove(nc);
				connections.Remove(nc);
				}

			connectionsChanged();
			}

		/// <summary>
		/// Aktualisiert sämtliche NodeConnections, welche von nodeToUpdate ausgehen
		/// </summary>
		/// <param name="nodeToUpdate">LineNode dessen ausgehende NodeConencitons aktualisiert werden sollen</param>
		public void UpdateOutgoingNodeConnections(LineNode nodeToUpdate)
			{
			foreach (NodeConnection nc in nodeToUpdate.nextConnections)
				{
				UpdateNodeConnection(nc);
				}
			}

		/// <summary>
		/// Aktualisiert sämtliche NodeConnections, welche in nodeToUpdate eingehen
		/// </summary>
		/// <param name="nodeToUpdate">LineNode dessen eingehende NodeConencitons aktualisiert werden sollen</param>
		public void UpdateIncomingNodeConnections(LineNode nodeToUpdate)
			{
			foreach (NodeConnection nc in nodeToUpdate.prevConnections)
				{
				UpdateNodeConnection(nc);
				}
			}

		/// <summary>
		/// Aktualisiert sämtliche NodeConnections, welche mit nodeToUpdate verbunden sind
		/// </summary>
		/// <param name="nodeToUpdate">LineNode dessen NodeConencitons aktualisiert werden sollen</param>
		public void UpdateNodeConnections(LineNode nodeToUpdate)
			{
			UpdateIncomingNodeConnections(nodeToUpdate);
			UpdateOutgoingNodeConnections(nodeToUpdate);
			}

		/// <summary>
		/// Aktualisiert sämtliche NodeConnections, welche mit LineNodes aus nodesToUpdate verbunden sind
		/// </summary>
		/// <param name="nodesToUpdate">Liste von LineNoden dessen NodeConencitons aktualisiert werden sollen</param>
		public void UpdateNodeConnections(List<LineNode> nodesToUpdate)
			{
			foreach (LineNode ln in nodesToUpdate)
				{
				UpdateIncomingNodeConnections(ln);
				UpdateOutgoingNodeConnections(ln);
				}
			}

		/// <summary>
		/// Gibt den LineNode zurück, der sich an der Position pos befindet
		/// </summary>
		/// <param name="pos">Position an dem der LineNode liegen soll</param>
		/// <returns>LineNode mit positionRect.Contains(pos) oder null falls kein solcher existiert</returns>
		public LineNode GetLineNodeAt(Vector2 pos)
			{
			foreach (LineNode ln in nodes)
				{
				if (ln.positionRect.Contains(pos))
					{
					return ln;
					}
				}
			return null;
			}

		/// <summary>
		/// Gibt die LineNodes zurück, die sich innerhalb des Rechteckes r befinden
		/// </summary>
		/// <param name="r">Suchrechteck</param>
		/// <returns>Eine Liste von LineNodes, die sich innerhalb von r befinden</returns>
		public List<LineNode> GetLineNodesAt(Rectangle r)
			{
			List<LineNode> toReturn = new List<LineNode>();
			foreach (LineNode ln in nodes)
				{
				if (r.Contains(ln.position))
					{
					toReturn.Add(ln);
					}
				}
			return toReturn;
			}


		#endregion

		#region Methoden für Intersections

		/// <summary>
		/// Zerstört die Intersection und meldet sie überall ab
		/// </summary>
		/// <param name="i">zu zerstörende Intersection</param>
		private void DestroyIntersection(Intersection i)
			{
			if (i != null)
				{
				i.Dispose();
				intersections.Remove(i);
				}
			}

		/// <summary>
		/// prüft, ob sich aRect und bRect echt überschneiden
		/// </summary>
		/// <param name="aRect">erstes Rechteck</param>
		/// <param name="bRect">zweites Rechteck</param>
		/// <returns></returns>
		private bool IntersectsTrue(RectangleF aRect, RectangleF bRect)
			{
			Interval<float> aHorizontal = new Interval<float>(aRect.Left, aRect.Right);
			Interval<float> aVertical = new Interval<float>(aRect.Top, aRect.Bottom);

			Interval<float> bHorizontal = new Interval<float>(bRect.Left, bRect.Right);
			Interval<float> bVertical = new Interval<float>(bRect.Top, bRect.Bottom);


			return (aHorizontal.IntersectsTrue(bHorizontal) && aVertical.IntersectsTrue(bVertical));
			}


		/// <summary>
		/// Führt zwei Listen mit Schnittpunktpaaren zusammen und eliminiert dabei doppelt gefundenen Schnittpunkte.
		/// </summary>
		/// <param name="correctList">Liste mit bereits gefundenen und überprüften Schnittpunkte (darf keine doppelten Schnittpunkte enthalten)</param>
		/// <param name="newList">Liste mit Schmittpunkten, die geprüft werden und evtl. eingefügt werden sollen</param>
		/// <param name="aSegment">LineSegment des linken Teils der Paare</param>
		/// <param name="bSegment">LineSegment des rechten Teils der Paare</param>
		/// <param name="tolerance">Wie weit entfernt dürfen Paare von Schnittpunkten maximal sein, damit sie als doppelt erkannt werden sollen</param>
		/// <returns>currectList, wo alle Schnittpunkte aus newList eingefügt worden sind, die mindestens tolerance von jedem anderen Schnittpunkt entfernt sind.</returns>
		private List<Pair<double>> MergeIntersectionPairs(List<Pair<double>> correctList, List<Pair<double>> newList, LineSegment aSegment, LineSegment bSegment, double tolerance)
			{
			List<Pair<double>> toReturn = correctList;

			// jedes Paar in newList anschauen
			foreach (Pair<double> p in newList)
				{
				// Position der Intersection feststellen...
				Vector2 positionOfP = aSegment.AtTime(p.Left);
				bool doInsert = true;

				// ...mit Position jeder Intersection in correctList vergleichen...
				for (int i = 0; doInsert && i < correctList.Count; i++)
					{
					Vector2 foo = aSegment.AtTime(toReturn[i].Left);
					if ((foo - positionOfP).Abs <= 8*tolerance)
						{
						// Wir haben einen doppelten gefunden, dann lass uns den Schnittpunkt in die Mitte verschieben
						doInsert = false;

						toReturn[i] = new Pair<double>(toReturn[i].Left + (p.Left - toReturn[i].Left) / 2, toReturn[i].Right + (p.Right - toReturn[i].Right) / 2);
						}
					}

				// ...und evtl. einfügen :)
				if (doInsert) 
					toReturn.Add(p);
				}

			return toReturn;
			}


		/// <summary>
		/// Findet alle Schnittpunkte zwischen aSegment und bSegment und gibt diese als Liste von Zeitpaaren zurück bei einer Genauigkeit von tolerance
		/// </summary>
		/// <param name="aSegment">erstes LineSegment</param>
		/// <param name="bSegment">zweites LineSegment</param>
		/// <param name="aTimeStart"></param>
		/// <param name="aTimeEnd"></param>
		/// <param name="bTimeStart"></param>
		/// <param name="bTimeEnd"></param>
		/// <param name="tolerance">Genauigkeit der Überprüfung: minimale Kantenlänge der überprüften BoundingBox</param>
		/// <param name="aOriginalSegment">ursprüngliches LineSegment A, bevor es aufgeteilt wurde</param>
		/// <param name="bOriginalSegment">ursprüngliches LineSegment B, bevor es aufgeteilt wurde</param>
		/// <returns>Eine Liste von Paaren wo sich ein Schnittpunkt befindet: Linker Teil Zeitparameter der ersten Kurve, rechter Teil Zeitparameter der zweiten Kurve</returns>
		private List<Pair<double>> CalculateIntersections(
			LineSegment aSegment, LineSegment bSegment,
			double aTimeStart, double aTimeEnd,
			double bTimeStart, double bTimeEnd,
			double tolerance,
			LineSegment aOriginalSegment, LineSegment bOriginalSegment)
			{
			List<Pair<double>> foundIntersections = new List<Pair<double>>();

			// überprüfe rekursiv auf Schnittpunkte der BoundingBoxen:
			// TODO: gehts vielleicht effizienter als rekursiv?

			RectangleF aBounds = aSegment.boundingRectangle;// .GetBounds(0);
			RectangleF bBounds = bSegment.boundingRectangle;// .GetBounds(0);

			// schneiden sich die BoundingBoxen? dann lohnt sich eine nähere Untersuchung
			if (IntersectsTrue(aBounds, bBounds)) // aBounds.IntersectsWith(bBounds)) //IntersectsTrue(aBounds, bBounds)) // 
				{
				// sind beide BoundingBoxen schon kleiner als tolerance, dann haben wir einen Schnittpunkt gefunden
				if ((aBounds.Width <= tolerance) && (aBounds.Height <= tolerance)
						&& (bBounds.Width <= tolerance) && (bBounds.Height <= tolerance))
					{
					foundIntersections.Add(new Pair<double>(aTimeStart + ((aTimeEnd - aTimeStart) / 2), bTimeStart + ((bTimeEnd - bTimeStart) / 2)));
					}

				// BoundingBox A ist schon klein genug, aber BoundingBox B sollte nochmal näher untersucht werden:
				else if ((aBounds.Width <= tolerance) && (aBounds.Height <= tolerance))
					{
					Pair<LineSegment> bDivided = bSegment.Subdivide();
					double bTimeMiddle = bTimeStart + ((bTimeEnd - bTimeStart) / 2);

					foundIntersections = MergeIntersectionPairs(foundIntersections, CalculateIntersections(
							aSegment, bDivided.Left,
							aTimeStart, aTimeEnd,
							bTimeStart, bTimeMiddle,
							tolerance,
							aOriginalSegment, bOriginalSegment), aOriginalSegment, bOriginalSegment, 2*tolerance);

					foundIntersections = MergeIntersectionPairs(foundIntersections, CalculateIntersections(
							aSegment, bDivided.Right,
							aTimeStart, aTimeEnd,
							bTimeMiddle, bTimeEnd,
							tolerance,
							aOriginalSegment, bOriginalSegment), aOriginalSegment, bOriginalSegment, 2*tolerance);
					}

				// BoundingBox B ist schon klein genug, aber BoundingBox A sollte nochmal näher untersucht werden:
				else if ((bBounds.Width <= tolerance) && (bBounds.Height <= tolerance))
					{
					Pair<LineSegment> aDivided = aSegment.Subdivide();
					double aTimeMiddle = aTimeStart + ((aTimeEnd - aTimeStart) / 2);

					foundIntersections = MergeIntersectionPairs(foundIntersections, CalculateIntersections(
							aDivided.Left, bSegment,
							aTimeStart, aTimeMiddle,
							bTimeStart, bTimeEnd,
							tolerance,
							aOriginalSegment, bOriginalSegment), aOriginalSegment, bOriginalSegment, 2 * tolerance);
					foundIntersections = MergeIntersectionPairs(foundIntersections, CalculateIntersections(
							aDivided.Right, bSegment,
							aTimeMiddle, aTimeEnd,
							bTimeStart, bTimeEnd,
							tolerance,
							aOriginalSegment, bOriginalSegment), aOriginalSegment, bOriginalSegment, 2 * tolerance);
					}

				// die BoundingBoxen sind noch zu groß - Linie aufteilen und die 2x2 Teile auf Schnittpunkte untersuchen
				else
					{
					Pair<LineSegment> aDivided = aSegment.Subdivide();
					Pair<LineSegment> bDivided = bSegment.Subdivide();

					double aTimeMiddle = aTimeStart + ((aTimeEnd - aTimeStart) / 2);
					double bTimeMiddle = bTimeStart + ((bTimeEnd - bTimeStart) / 2);

					foundIntersections = MergeIntersectionPairs(foundIntersections, CalculateIntersections(
							aDivided.Left, bDivided.Left,
							aTimeStart, aTimeMiddle,
							bTimeStart, bTimeMiddle,
							tolerance,
							aOriginalSegment, bOriginalSegment), aOriginalSegment, bOriginalSegment, 2 * tolerance);
					foundIntersections = MergeIntersectionPairs(foundIntersections, CalculateIntersections(
							aDivided.Right, bDivided.Left,
							aTimeMiddle, aTimeEnd,
							bTimeStart, bTimeMiddle,
							tolerance,
							aOriginalSegment, bOriginalSegment), aOriginalSegment, bOriginalSegment, 2 * tolerance);

					foundIntersections = MergeIntersectionPairs(foundIntersections, CalculateIntersections(
							aDivided.Left, bDivided.Right,
							aTimeStart, aTimeMiddle,
							bTimeMiddle, bTimeEnd,
							tolerance,
							aOriginalSegment, bOriginalSegment), aOriginalSegment, bOriginalSegment, 2 * tolerance);
					foundIntersections = MergeIntersectionPairs(foundIntersections, CalculateIntersections(
							aDivided.Right, bDivided.Right,
							aTimeMiddle, aTimeEnd,
							bTimeMiddle, bTimeEnd,
							tolerance,
							aOriginalSegment, bOriginalSegment), aOriginalSegment, bOriginalSegment, 2 * tolerance);

					}
				}

			// TODO: doppelte Schnittpunkte rausfiltern
			// Nun filtern wir noich alle Schnittpunkte raus, die doppelt erkannt wurden:
			/*
			for (int i = 0; i < foundIntersections.Count-1; i++)
				{
				for (int j = i+1; j < foundIntersections.Count; j++)
					{
					}
				}
			*/
			return foundIntersections;
			}

		#endregion

		#region Methoden für NodeConnections

		/// <summary>
		/// Berechnet alle Spurwechselstellen der NodeConnection nc mit anderen NodeConnections
		/// </summary>
		/// <param name="nc">zu untersuchende NodeConnection</param>
		/// <param name="distanceBetweenChangePoints">Distanz zwischen einzelnen LineChangePoints (quasi Genauigkeit)</param>
		/// <param name="maxDistanceToOtherNodeConnection">maximale Entfernung zwischen zwei NodeConnections zwischen denen ein Spurwechsel stattfinden darf</param>
		public void FindLineChangePoints(NodeConnection nc, double distanceBetweenChangePoints, double maxDistanceToOtherNodeConnection)
			{
			nc.ClearLineChangePoints();

			double currentArcPosition = distanceBetweenChangePoints/2;
			double delta = distanceBetweenChangePoints / 4;

			/*
			 * TODO: Spurwechsel funktioniert soweit so gut. Einziges Manko ist der Spurwechsel über LineNodes hinweg:
			 * 
			 * Zum einen, können so rote Ampeln überfahren werden und zum anderen, kommt es z.T. zu sehr komischen Situationen, wo
			 * das spurwechselnde Auto irgendwie denkt es sei viel weiter vorne und so mittendrin wartet und erst weiterfährt, wenn
			 * das Auto 20m weiter weg ist.
			 * 
			 * Als workaround werden jetzt doch erstmal Spurwechsel kurz vor LineNodes verboten, auch wenn das eigentlich ja gerade
			 * auch ein Ziel der hübschen Spurwechsel war.
			 * 
			 */

			// nur so lange suchen, wie die NodeConnection lang ist
			while (currentArcPosition < nc.lineSegment.length - distanceBetweenChangePoints/2)
				{
				Vector2 startl = nc.lineSegment.AtPosition(currentArcPosition - delta);
				Vector2 startr = nc.lineSegment.AtPosition(currentArcPosition + delta);

				Vector2 leftVector = nc.lineSegment.DerivateAtTime(nc.lineSegment.PosToTime(currentArcPosition - delta)).RotatedClockwise.Normalized;
				Vector2 rightVector = nc.lineSegment.DerivateAtTime(nc.lineSegment.PosToTime(currentArcPosition + delta)).RotatedCounterClockwise.Normalized;

				// Faule Implementierung:	statt Schnittpunkt Gerade/Bezierkurve zu berechnen nutzen wir vorhandenen
				//							Code und Berechnen den Schnittpunkt zwischen zwei Bezierkurven.
				// TODO:	Sollte das hier zu langsam sein, muss eben neuer optimierter Code her für die Berechnung
				//			von Schnittpunkten Gerade/Bezierkurve
				LineSegment leftLS = new LineSegment(0, startl, startl + 0.25 * maxDistanceToOtherNodeConnection * leftVector, startl + 0.75 * maxDistanceToOtherNodeConnection * leftVector, startl + maxDistanceToOtherNodeConnection * leftVector);
				LineSegment rightLS = new LineSegment(0, startr, startr + 0.25 * maxDistanceToOtherNodeConnection * rightVector, startr + 0.75 * maxDistanceToOtherNodeConnection * rightVector, startr + maxDistanceToOtherNodeConnection * rightVector);

				foreach (NodeConnection nc2 in m_connections)
					{
					if (nc2.enableIncomingLineChange && (nc2.carsAllowed || nc2.busAllowed) && nc != nc2)
						{
						// LINKS: Zeitparameterpaare ermitteln 
						List<Pair<double>> intersectionTimes = CalculateIntersections(leftLS, nc2.lineSegment, 0d, 1d, 0d, 1d, 8, leftLS, nc2.lineSegment);
						if (intersectionTimes != null)
							{
							// Startposition
							NodeConnection.SpecificPosition start = new NodeConnection.SpecificPosition(currentArcPosition - delta, nc);

							// LineChangePoints erstellen
							foreach (Pair<double> p in intersectionTimes)
								{
								// Winkel überprüfen
								if (Vector2.AngleBetween(nc.lineSegment.DerivateAtTime(nc.lineSegment.PosToTime(currentArcPosition - delta)), nc2.lineSegment.DerivateAtTime(p.Right)) < Constants.maximumAngleBetweenConnectionsForLineChangePoint)
									{
									NodeConnection.SpecificPosition otherStart = new NodeConnection.SpecificPosition(nc2, p.Right);

									// Einfädelpunkt des Fahrzeugs bestimmen und evtl. auf nächste NodeConnection weiterverfolgen:
									double distance = (nc.lineSegment.AtPosition(currentArcPosition - delta) - nc2.lineSegment.AtTime(p.Right)).Abs;

									// Einfädelpunkt:
									double arcPositionTarget = nc2.lineSegment.TimeToArcPosition(p.Right) + 3 * distance;

									if (arcPositionTarget <= nc2.lineSegment.length)
										{
										NodeConnection.SpecificPosition target = new NodeConnection.SpecificPosition(arcPositionTarget, nc2);
										nc.AddLineChangePoint(new NodeConnection.LineChangePoint(start, target, otherStart));
										}
									else
										{
										double diff = arcPositionTarget - nc2.lineSegment.length;
										foreach (NodeConnection nextNc in nc2.endNode.nextConnections)
											{
											if (nextNc.enableIncomingLineChange && (nextNc.carsAllowed || nextNc.busAllowed) && nc != nextNc)
												{
												NodeConnection.SpecificPosition target = new NodeConnection.SpecificPosition(diff, nextNc);
												nc.AddLineChangePoint(new NodeConnection.LineChangePoint(start, target, otherStart));
												}
											}
										}

									break;
									}
								}
							}

						// RECHTS: Zeitparameterpaare ermitteln
						intersectionTimes = CalculateIntersections(rightLS, nc2.lineSegment, 0d, 1d, 0d, 1d, 8, leftLS, nc2.lineSegment);
						if (intersectionTimes != null)
							{
							// Startposition
							NodeConnection.SpecificPosition start = new NodeConnection.SpecificPosition(currentArcPosition + delta, nc);

							// LineChangePoints erstellen
							foreach (Pair<double> p in intersectionTimes)
								{
								// Winkel überprüfen
								if (Vector2.AngleBetween(nc.lineSegment.DerivateAtTime(nc.lineSegment.PosToTime(currentArcPosition + delta)), nc2.lineSegment.DerivateAtTime(p.Right)) < Constants.maximumAngleBetweenConnectionsForLineChangePoint)
									{
									NodeConnection.SpecificPosition otherStart = new NodeConnection.SpecificPosition(nc2, p.Right);

									// Einfädelpunkt des Fahrzeugs bestimmen und evtl. auf nächste NodeConnection weiterverfolgen:
									double distance = (nc.lineSegment.AtPosition(currentArcPosition + delta) - nc2.lineSegment.AtTime(p.Right)).Abs;

									// Einfädelpunkt:
									double arcPositionTarget = nc2.lineSegment.TimeToArcPosition(p.Right) + 3 * distance;

									if (arcPositionTarget <= nc2.lineSegment.length)
										{
										NodeConnection.SpecificPosition target = new NodeConnection.SpecificPosition(arcPositionTarget, nc2);
										nc.AddLineChangePoint(new NodeConnection.LineChangePoint(start, target, otherStart));
										}
									else
										{
										double diff = arcPositionTarget - nc2.lineSegment.length;
										foreach (NodeConnection nextNc in nc2.endNode.nextConnections)
											{
											NodeConnection.SpecificPosition target = new NodeConnection.SpecificPosition(diff, nextNc);
											nc.AddLineChangePoint(new NodeConnection.LineChangePoint(start, target, otherStart));
											}
										}

									break;
									}
								}
							}
						}
					}

				currentArcPosition += distanceBetweenChangePoints;
				}
			}


		/// <summary>
		/// Entfernt alle LineChangePoints, die von nc ausgehen und evtl. eingehen
		/// </summary>
		/// <param name="nc">NodeConnection dessen ausgehende LineChangePoints gelöscht werden</param>
		/// <param name="removeOutgoingLineChangePoints">ausgehende LineChangePoints löschen</param>
		/// <param name="removeIncomingLineChangePoints">eingehende LineChangePoints löschen</param>
		public void RemoveLineChangePoints(NodeConnection nc, bool removeOutgoingLineChangePoints, bool removeIncomingLineChangePoints)
			{
			if (removeOutgoingLineChangePoints)
				{
				nc.ClearLineChangePoints();
				}
			
			if (removeIncomingLineChangePoints)
				{
				foreach (NodeConnection otherNc in m_connections)
					{
					otherNc.RemoveAllLineChangePointsTo(nc);
					}
				}
			}

		/// <summary>
		/// Gibt eine Liste mit allen Schnittpunkten von nc mit anderen existierenden NodeConnections
		/// </summary>
		/// <param name="nc">zu untersuchende NodeConnection</param>
		/// <param name="tolerance">Toleranz (maximale Kantenlänge der Boundingboxen)</param>
		/// <returns>Eine Liste mit Intersection Objekten die leer ist, falls keine Schnittpunkte existieren</returns>
		private List<Intersection> CalculateIntersections(NodeConnection nc, double tolerance)
			{
			List<Intersection> toReturn = new List<Intersection>();

			foreach (NodeConnection nc2 in connections)
				{
				if (nc != nc2)
					{
					// Zeitparameterpaare ermitteln
					List<Pair<double>> intersectionTimes = CalculateIntersections(nc.lineSegment, nc2.lineSegment, 0d, 1d, 0d, 1d, tolerance, nc.lineSegment, nc2.lineSegment);
					if (intersectionTimes != null)
						{
						// Intersections erstellen
						foreach (Pair<double> p in intersectionTimes)
							{
							toReturn.Add(new Intersection(nc, nc2, p.Left, p.Right));
							}
						}
					}
				}

			return toReturn;
			}

		/// <summary>
		/// Berechnet alle Schnittpunkte von nc mit anderen existierenden NodeConnections und meldet sie an
		/// </summary>
		/// <param name="nc">zu untersuchende NodeConnection</param>
		public void FindIntersections(NodeConnection nc)
			{
			// erstmal bestehende Intersections dieser NodeConnections zerstören
			for (int i = 0; i < intersections.Count; i++)
				{
				if ((intersections[i].aConnection == nc) || (intersections[i].bConnection == nc))
					{
					intersections[i].Dispose();
					intersections.RemoveAt(i);
					i--;
					}
				}

			// jetzt können wir nach neuen Intersections suchen und diese anmelden
			List<Intersection> foundIntersections = CalculateIntersections(nc, 0.25d);
			foreach (Intersection i in foundIntersections)
				{
				i.aConnection.AddIntersection(i);
				i.bConnection.AddIntersection(i);
				intersections.Add(i);
				}
			}

		/// <summary>
		/// Gibt die erstbeste NodeConnection von from nach to zurück
		/// (es sollte eigentlich immer nur eine existieren, das wird aber nicht weiter geprüft)
		/// </summary>
		/// <param name="from">LineNode von dem die NodeConnection ausgeht</param>
		/// <param name="to">LineNode zu der die NodeConnection hingehet</param>
		/// <returns>NodeConnection, welche von from nach to läuft oder null, falls keine solche existiert</returns>
		public NodeConnection GetNodeConnection(LineNode from, LineNode to)
			{
			foreach (NodeConnection nc in this.connections)
				{
				if ((nc.startNode == from) && (nc.endNode == to))
					{
					return nc;
					}
				}
			return null;
			}

		/// <summary>
		/// Aktualisiert die NodeConnection ncToUpdate
		/// (Bezierkurve neu berechnen, etc.)
		/// </summary>
		/// <param name="ncToUpdate">zu aktualisierende NodeConnection</param>
		public void UpdateNodeConnection(NodeConnection ncToUpdate)
			{
			ncToUpdate.lineSegment = null;
			ncToUpdate.lineSegment = new LineSegment(0, ncToUpdate.startNode.position, ncToUpdate.startNode.outSlopeAbs, ncToUpdate.endNode.inSlopeAbs, ncToUpdate.endNode.position);
			FindIntersections(ncToUpdate);


			if (ncToUpdate.enableIncomingLineChange)
				{
				// TODO:	diese Lösung ist viel zu unperformant! Da muss was anderes her.

				/*
				RemoveLineChangePoints(ncToUpdate, false, true);
				foreach (NodeConnection nc in m_connections)
					{
					if (nc.enableOutgoingLineChange)
						{
						FindLineChangePoints(nc, Constants.maxDistanceToLineChangePoint, Constants.maxDistanceToParallelConnection);
						}
					}*/
				}
			else
				{
				// TODO: überlegen, ob hier wirklich nichts gemacht werden muss
				}

			if (ncToUpdate.enableOutgoingLineChange)
				{
				RemoveLineChangePoints(ncToUpdate,true, false);
				FindLineChangePoints(ncToUpdate, Constants.maxDistanceToLineChangePoint, Constants.maxDistanceToParallelConnection);
				}
			else
				{
				// TODO: überlegen, ob hier wirklich nichts gemacht werden muss
				}
			}

		/// <summary>
		/// Teilt die NodeConnection nc in zwei einzelne NodeConnections auf.
		/// Dabei wird in der Mitte natürlich auch ein neuer LineNode erstellt
		/// </summary>
		/// <param name="nc">aufzuteilende NodeConnection</param>
		public void SplitNodeConnection(NodeConnection nc)
			{
			LineNode startNode = nc.startNode;
			LineNode endNode = nc.endNode;

			Pair<LineSegment> divided = nc.lineSegment.Subdivide();

			// Mittelknoten erstellen
			LineNode middleNode = new LineNode(divided.Left.p3);
			middleNode.inSlopeAbs = divided.Left.p2;
			middleNode.outSlopeAbs = divided.Right.p1;
			nodes.Add(middleNode);

			// Anfangs- und Endknoten bearbeiten
			startNode.outSlopeAbs = divided.Left.p1;
			endNode.inSlopeAbs = divided.Right.p2;

			// Alte Connections lösen
			Disconnect(startNode, endNode);

			// Neue Connections bauen
			Connect(startNode, middleNode, nc.priority, nc.carsAllowed, nc.busAllowed, nc.tramAllowed, nc.enableIncomingLineChange, nc.enableOutgoingLineChange);
			Connect(middleNode, endNode, nc.priority, nc.carsAllowed, nc.busAllowed, nc.tramAllowed, nc.enableIncomingLineChange, nc.enableOutgoingLineChange);
			}


		/// <summary>
		/// gibt die NodeConnection zuück, welche sich bei position befindet
		/// </summary>
		/// <param name="position">Position, wo gesucht werden soll</param>
		/// <returns>erstbeste NodeConnection, welche durch den Punkt position läuft oder null, falls keine solche existiert</returns>
		public NodeConnection GetNodeConnectionAt(Vector2 position)
			{
			foreach (NodeConnection nc in connections)
				{
				if (nc.lineSegment.Contains(position))
					{
					return nc;
					}
				}
			return null;
			}


		/// <summary>
		/// liefert das IVehicle an den Weltkoordinaten position zurück.
		/// Momentan wird dabei nur ein Bereich von 30x30Pixeln um die Front des Fahrzeuges herum überprüft)
		/// </summary>
		/// <param name="position">Weltkoordinatenposition, wo nach einem Fahrzeug gesucht werden soll</param>
		/// <returns>Ein IVehicle dessen Front im Umkreis von 15 Pixeln um position herum ist.</returns>
		public IVehicle GetVehicleAt(Vector2 position)
			{
			foreach (NodeConnection nc in connections)
				{
				if (nc.lineSegment.Contains(position, 5, 15))
					{
					foreach (IVehicle v in nc.vehicles)
						{
						if (v.state.boundingRectangle.Contains(position))
							{
							return v;
							}
						}
					}
				}
			return null;
			}


		#endregion

		#region Statistiken

		/// <summary>
		/// aktiviert die Visualisierung von Statistiken in NodeConnections
		/// </summary>
		/// <param name="state">Status der Visualisierung der gesetzt werden soll</param>
		public void setVisualizationInNodeConnections(bool state)
			{
			foreach (NodeConnection nc in m_connections)
				{
				nc.visualizeAverageSpeed = state;
				}
			}


		#endregion

		#region Sonstige Methoden

		/// <summary>
		/// Sorgt dafür dass alle verwalteten Objekte gezeichnet werden
		/// </summary>
		/// <param name="g">Zeichenfläche auf der gezeichnet werden soll</param>
		/// <param name="clippingRange">Clippingbereich in Weltkoordinaten</param>
		/// <param name="doClipping">führe Clipping durch</param>
		/// <param name="drawVehicles">Zeichne Autos</param>
		/// <param name="drawNodesAndConnections">zeichne LineNodes und NodeConnections</param>
		/// <param name="debug">Zeichne Debuginformationen</param>
		public void Draw(Graphics g, Rectangle clippingRange, bool doClipping, bool drawVehicles, bool drawNodesAndConnections, bool debug)
			{
			using (Pen BlackPen = new Pen(Color.Black, 1.0F))
				{
				int clippedNcs = 0;

				// NodeConnections malen:
				if (drawNodesAndConnections)
					foreach (NodeConnection nc in connections)
					{
					if (! doClipping || nc.lineSegment.boundingRectangle.IntersectsWith(clippingRange))
						{
						nc.Draw(g);
						}
					else
						clippedNcs++;
					}

				// LineNodes malen
				if (nodes.Count != 0)
					{
					// Node Verbindungen + Autos zeichnen
					foreach (LineNode aNode in nodes)
						{
						if (drawNodesAndConnections)
							{
							aNode.Draw(g);
							}

						if (drawVehicles)
							{
							foreach (NodeConnection nc in aNode.nextConnections)
								{
								foreach (IVehicle v in nc.vehicles)
									{
									v.Draw(g);
									}
								}
							}
						}
					}

				if (debug)
					{
					// Node Verbindungen + Autos zeichnen
					foreach (LineNode aNode in nodes)
						{
						aNode.DrawDebugData(g);
						}
					foreach (NodeConnection nc in connections)
						{
						nc.DrawDebugData(g);
						}
					}

				// Intersections malen
				if (debug)
					{
					using (Pen iPen = new Pen(Color.Red, 1.0f))
						{
						g.DrawString("Clipped NodeConnections: " + clippedNcs, new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(clippingRange.X + 10, clippingRange.Y + 24));

						foreach (Intersection i in intersections)
							{
							g.DrawLine(iPen, i.aPosition, i.bPosition);

							//g.DrawString(i.GetOriginalArrivingTimesCount().ToString(), new Font("Arial", 7), new SolidBrush(Color.Black), i.aPosition + new Vector2(10.0, 0.0));
							double streckungsfaktor = i.GetWaitingDistance();

							PointF[] surroundingPoints = new PointF[4]
								{
									i.aPosition + i.aConnection.lineSegment.DerivateAtTime(i.aTime).Normalized * streckungsfaktor,
									i.bPosition + i.bConnection.lineSegment.DerivateAtTime(i.bTime).Normalized * streckungsfaktor,
									i.aPosition + i.aConnection.lineSegment.DerivateAtTime(i.aTime).Normalized * streckungsfaktor * -1,
									i.bPosition + i.bConnection.lineSegment.DerivateAtTime(i.bTime).Normalized * streckungsfaktor * -1
								};

							g.DrawPolygon(iPen, surroundingPoints);

							}
						}
					}
				}
			}

		#endregion

		#region Speichern/Laden

		/// <summary>
		/// Speichert alle verwalteten Daten in eine XML Datei
		/// </summary>
		/// <param name="xw">XMLWriter, in denen die verwalteten Daten gespeichert werden soll</param>
		/// <param name="xsn">zugehöriger XML-Namespace</param>
		/// <param name="fahrauftraege">Liste von zu speichernden Fahraufträgen</param>
		public void SaveToFile(XmlWriter xw, XmlSerializerNamespaces xsn, List<Auftrag> fahrauftraege)
			{
			try
				{
				// Alles fürs Speichern vorbereiten
				foreach (LineNode ln in nodes)
					{
					ln.PrepareForSave();
					}
				foreach (NodeConnection nc in connections)
					{
					nc.PrepareForSave();
					}
				foreach (Auftrag a in fahrauftraege)
					{
					a.PrepareForSave();
					}


					
				// zunächst das Layout speichern
				xw.WriteStartElement("Layout");

					xw.WriteStartElement("title");
					xw.WriteString(m_title);
					xw.WriteEndElement();

					xw.WriteStartElement("infoText");
					xw.WriteString(m_infoText);
					xw.WriteEndElement();

					// LineNodes serialisieren
					XmlSerializer xs = new XmlSerializer(typeof(LineNode));
					foreach (LineNode ln in nodes)
						{
						xs.Serialize(xw, ln, xsn);
						}

					// NodeConnections serialisieren
					XmlSerializer xs2 = new XmlSerializer(typeof(NodeConnection));
					foreach (NodeConnection nc in connections)
						{
						xs2.Serialize(xw, nc, xsn);
						}

					xw.WriteEndElement();


				// nun die Fahraufträge speichern
				xw.WriteStartElement("Traffic");

					// Fahraufträge serialisieren
					XmlSerializer xs3 = new XmlSerializer(typeof(Auftrag));
					foreach (Auftrag a in fahrauftraege)
						{
						xs3.Serialize(xw, a, xsn);
						}

				xw.WriteEndElement();

				}
			catch (IOException ex)
				{
				MessageBox.Show(ex.Message);
				throw;
				}
			}


		/// <summary>
		/// Läd eine XML Datei und versucht daraus den gespeicherten Zustand wiederherzustellen
		/// </summary>
		/// <param name="xd">XmlDocument mit den zu ladenden Daten</param>
		/// <param name="lf">LoadingForm für Statusinformationen</param>
		public List<Auftrag> LoadFromFile(XmlDocument xd, LoadingForm.LoadingForm lf)
			{
			lf.SetupLowerProgess("Lese XML-Struktur...", 2);

			List<Auftrag> toReturn = new List<Auftrag>();
			int saveVersion = 0;

			// erstma alles vorhandene löschen
			nodes.Clear();
			connections.Clear();
			intersections.Clear();

			XmlNode mainNode = xd.SelectSingleNode("//CityTrafficSimulator");
			XmlNode saveVersionNode = mainNode.Attributes.GetNamedItem("saveVersion");
			if (saveVersionNode != null)
				saveVersion = Int32.Parse(saveVersionNode.Value);
			else
				saveVersion = 0;

			XmlNode titleNode = xd.SelectSingleNode("//CityTrafficSimulator/Layout/title");
			if (titleNode != null)
				{
				m_title = titleNode.InnerText;
				}

			XmlNode infoNode = xd.SelectSingleNode("//CityTrafficSimulator/Layout/infoText");
			if (infoNode != null)
				{
				m_infoText = infoNode.InnerText;
				}

			lf.StepLowerProgress();

			// entsprechenden Node auswählen
			XmlNodeList xnlLineNode = xd.SelectNodes("//CityTrafficSimulator/Layout/LineNode");
			foreach (XmlNode aXmlNode in xnlLineNode)
				{
				// Node in einen TextReader packen
				TextReader tr = new StringReader(aXmlNode.OuterXml);
				// und Deserializen
				XmlSerializer xs = new XmlSerializer(typeof(LineNode));
				LineNode ln = (LineNode)xs.Deserialize(tr);

				/*ln.position *= 1.9d;
				ln.position += new Vector2(8, 8);

				ln.inSlope *= 1.9d;
				ln.outSlope *= 1.9d;

				ln.position.Round();
				ln.inSlope.Round();
				ln.outSlope.Round();
				*/
				// ab in die Liste
				nodes.Add(ln);
				}

			lf.StepLowerProgress();

			// entsprechenden NodeConnections auswählen
			XmlNodeList xnlNodeConnection = xd.SelectNodes("//CityTrafficSimulator/Layout/NodeConnection");
			foreach (XmlNode aXmlNode in xnlNodeConnection)
				{
				// Node in einen TextReader packen
				TextReader tr = new StringReader(aXmlNode.OuterXml);
				// und Deserializen
				XmlSerializer xs = new XmlSerializer(typeof(NodeConnection));
				NodeConnection ln = (NodeConnection)xs.Deserialize(tr);
				// ab in die Liste
				connections.Add(ln);
				}

			lf.SetupLowerProgess("Stelle LineNodes wieder her...", m_nodes.Count);

			// Nodes wiederherstellen
			foreach (LineNode ln in m_nodes)
				{
				ln.RecoverFromLoad(saveVersion, m_nodes);
				lf.StepLowerProgress();
				}

			lf.SetupLowerProgess("Stelle NodeConnections wieder her...", m_connections.Count);

			// LineNodes wiederherstellen
			foreach (NodeConnection nc in m_connections)
				{
				nc.RecoverFromLoad(saveVersion, nodes);
				TellNodesTheirConnection(nc);
				nc.lineSegment = new LineSegment(0, nc.startNode.position, nc.startNode.outSlopeAbs, nc.endNode.inSlopeAbs, nc.endNode.position);

				lf.StepLowerProgress();
				}

			lf.SetupLowerProgess("Berechne Intersections und Spurwechsel...", m_connections.Count);

			// Intersections wiederherstellen
			foreach (NodeConnection nc in m_connections)
				{
				UpdateNodeConnection(nc);

				lf.StepLowerProgress();
				}


			// Fahraufträge laden
			// entsprechenden Node auswählen
			XmlNodeList xnlAuftrag = xd.SelectNodes("//CityTrafficSimulator/Traffic/Auftrag");

			lf.SetupLowerProgess("Lade Fahraufträge...", 2 * xnlAuftrag.Count);
			
			foreach (XmlNode aXmlNode in xnlAuftrag)
				{
				// Node in einen TextReader packen
				TextReader tr = new StringReader(aXmlNode.OuterXml);
				// und Deserializen
				XmlSerializer xs = new XmlSerializer(typeof(Auftrag));
				Auftrag ln = (Auftrag)xs.Deserialize(tr);

				// in alten Dateien wurde das Feld häufigkeit statt trafficDensity gespeichert. Da es dieses Feld heute nicht mehr gibt, müssen wir konvertieren:
				if (saveVersion < 1)
					{
					// eigentlich wollte ich hier direkt mit aXmlNode arbeiten, das hat jedoch komische Fehler verursacht (SelectSingleNode) wählt immer den gleichen aus)
					// daher der Umweg über das neue XmlDocument.
					XmlDocument doc = new XmlDocument();
					XmlElement elem = doc.CreateElement("Auftrag");
					elem.InnerXml = aXmlNode.InnerXml;
					doc.AppendChild(elem);

					XmlNode haeufigkeitNode = doc.SelectSingleNode("//Auftrag/häufigkeit");
					if (haeufigkeitNode != null)
						{
						ln.trafficDensity = 72000 / Int32.Parse(haeufigkeitNode.InnerXml);
						}
					haeufigkeitNode = null;
					}

				// ab in die Liste
				toReturn.Add(ln);

				lf.StepLowerProgress();
				}

			// Nodes wiederherstellen
			foreach (Auftrag a in toReturn)
				{
				a.RecoverFromLoad(saveVersion, nodes);

				lf.StepLowerProgress();
				}


			return toReturn;
			}

		#endregion
		
		#region ISavable Member

		/// <summary>
		/// Bereitet alles fürs Speichern vor
		/// (Hashes generieren etc.)
		/// </summary>
		public void PrepareForSave()
			{
			foreach (LineNode ln in nodes)
				{
				ln.PrepareForSave();
				}
			}

		/// <summary>
		/// Stellt nach erfolgreicher Deserialisierung alle internen Links wieder her
		/// </summary>
		/// <param name="saveVersion">Version der gespeicherten Datei</param>
		/// <param name="nodesList">Liste der bereits wiederhergestellten LineNodes</param>
		public void RecoverFromLoad(int saveVersion, List<LineNode> nodesList)
			{
			throw new NotImplementedException();
			}

		#endregion

		#region ITickable Member

		/// <summary>
		/// sagt allen verwalteten Objekten Bescheid, dass sie ticken dürfen *g*
		/// </summary>
		public void Tick(double tickLength, double currentTime)
			{
			// TODO: ist das hier nötig? Oder nur son Relikt aus vergangenen Zeiten
			foreach (LineNode ln in nodes)
				{
				ln.Tick(tickLength, currentTime);
				}
			}

		/// <summary>
		/// setzt den Tick-Zustand aller LineNodes zurück
		/// </summary>
		public void Reset()
			{
			foreach (LineNode ln in nodes)
				{
				ln.Reset();
				}
			}
		
		#endregion

		}
	}
