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
using System.Xml.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;

using CityTrafficSimulator.Tools;
using CityTrafficSimulator.Vehicle;


namespace CityTrafficSimulator
    {
	/// <summary>
	/// Verbindung zwischen zwei LineNodes
	/// </summary>
    [Serializable]
    public class NodeConnection : ISavable, IDrawable
		{

		#region Variablen

		/// <summary>
		/// Startknoten
		/// </summary>
		[XmlIgnore]
        public LineNode startNode;

		/// <summary>
		/// Endknoten
		/// </summary>
        [XmlIgnore]
        public LineNode endNode;

		/// <summary>
		/// zugehöriges LineSegment (Bézierkurve)
		/// </summary>
        [XmlIgnore]
        public LineSegment lineSegment;

		/// <summary>
		/// Priorität der NodeConnection
		/// </summary>
		private int m_priority = 1;
		/// <summary>
		/// Priorität der NodeConnection
		/// </summary>
		public int priority
			{
			get { return m_priority; }
			set { m_priority = value; UpdatePen(); }
			}

		/// <summary>
		/// Flag, ob Autos auf dieser NodeConnection erlaubt sind
		/// </summary>
		private bool m_carsAllowed = true;
		/// <summary>
		/// Flag, ob Autos auf dieser NodeConnection erlaubt sind
		/// </summary>
		public bool carsAllowed
			{
			get { return m_carsAllowed; }
			set { m_carsAllowed = value; UpdatePen(); }
			}

		/// <summary>
		/// Flag, ob Busse auf dieser NodeConnection erlaubt sind
		/// </summary>
		private bool m_busAllowed;
		/// <summary>
		/// Flag, ob Busse auf dieser NodeConnection erlaubt sind
		/// </summary>
		public bool busAllowed
			{
			get { return m_busAllowed; }
			set { m_busAllowed = value; UpdatePen(); }
			}

		/// <summary>
		/// Flag, ob Straßenbahnen auf dieser NodeConnection erlaubt sind
		/// </summary>
		private bool m_tramAllowed;
		/// <summary>
		/// Flag, ob Straßenbahnen auf dieser NodeConnection erlaubt sind
		/// </summary>
		public bool tramAllowed
			{
			get { return m_tramAllowed; }
			set { m_tramAllowed = value; UpdatePen(); }
			}


		/// <summary>
		/// Flag, ob ausgehende Spurwechsel von dieser NodeConnection erlaubt sind
		/// </summary>
		private bool m_enableOutgoingLineChange;
		/// <summary>
		/// Flag, ob ausgehende Spurwechsel von dieser NodeConnection erlaubt sind
		/// </summary>
		public bool enableOutgoingLineChange
			{
			get { return m_enableOutgoingLineChange; }
			set { m_enableOutgoingLineChange = value; }
			}

		/// <summary>
		/// Flag, ob eingehende Spurwechsel auf diese NodeConnection erlaubt ist
		/// </summary>
		private bool m_enableIncomingLineChange;
		/// <summary>
		/// Flag, ob eingehende Spurwechsel auf diese NodeConnection erlaubt ist
		/// </summary>
		public bool enableIncomingLineChange
			{
			get { return m_enableIncomingLineChange; }
			set { m_enableIncomingLineChange = value; }
			}


		/// <summary>
		/// Comparer, der zwei LineChangePoints miteinander vergleicht.
		/// </summary>
		static SortedLinkedList<LineChangePoint>.CompareDelegate lineChangePointComparer = delegate(LineChangePoint a, LineChangePoint b)
			{
				return a.start.arcPosition.CompareTo(b.start.arcPosition);
			};
	
		/// <summary>
		/// Liste von LineChangePoints, nach Bogenlängenposition sortiert
		/// </summary>
		[XmlIgnore]
		private List<LineChangePoint> m_lineChangePoints = new List<LineChangePoint>();//new SortedLinkedList<LineChangePoint>(lineChangePointComparer);
		/// <summary>
		/// Liste von LineChangePoints, nach Bogenlängenposition sortiert
		/// </summary>
		[XmlIgnore]
		public List<LineChangePoint> lineChangePoints
			{
			get { return m_lineChangePoints; }
			set { m_lineChangePoints = value; }
			}


		/// <summary>
		/// Liste von LineChangeAreas, aller von dieser NodeConnection per Spurwechsel zu erreichenden LineNodes
		/// </summary>
		[XmlIgnore]
		private Dictionary<int, LineChangeInterval> m_lineChangeIntervals = new Dictionary<int,LineChangeInterval>(Constants.defaultLineChangeIntervalDictionaryCapacity);
		/// <summary>
		/// Liste von LineChangeAreas, aller von dieser NodeConnection per Spurwechsel zu erreichenden LineNodes
		/// </summary>
		[XmlIgnore]
		public Dictionary<int, LineChangeInterval> lineChangeIntervals
			{
			get { return m_lineChangeIntervals; }
			}


		/// <summary>
		/// Liste aller LineNodes, die über Spurwechsel erreicht werden können
		/// </summary>
		[XmlIgnore]
		private List<LineNode> m_viaLineChangeReachableNodes = new List<LineNode>();
		/// <summary>
		/// Liste aller LineNodes, die über Spurwechsel erreicht werden können
		/// </summary>
		[XmlIgnore]
		public List<LineNode> viaLineChangeReachableNodes
			{
			get { return m_viaLineChangeReachableNodes; }
			}




		#region Statistiken

		// TODO:	eigentlich sollten die Statisktiken extern verwaltet werden
		//			dies hier sollte erstmal nur zum testen quick and dirty sein

		/// <summary>
		/// Summe der Durchschnittsgeschwindigkeiten in m/s der Autos auf dieser NodeConnection
		/// </summary>
		private float m_sumOfAverageSpeeds;
		/// <summary>
		/// Summe der Durchschnittsgeschwindigkeiten in m/s der Autos auf dieser NodeConnection
		/// </summary>
		[XmlIgnore]
		public float sumOfAverageSpeeds
			{
			get { return m_sumOfAverageSpeeds; }
			}

		/// <summary>
		/// Anzahl der Summanden der Durchschnittsgeschwindigkeiten
		/// </summary>
		private int m_countOfVehicles;
		/// <summary>
		/// Anzahl der Summanden der Durchschnittsgeschwindigkeiten
		/// </summary>
		[XmlIgnore]
		public int countOfVehicles
			{
			get { return m_countOfVehicles; }
			}



		#endregion

		#region Zeichnen
		/// <summary>
		/// DEPRECATED: Farbe der Linie
		/// </summary>
		private Color m_color = Color.Black;
		/// <summary>
		/// DEPRECATED: Farbe der Linie
		/// </summary>
		[XmlIgnore]
		public Color color
			{
			get { return m_color; }
			set { m_color = value; UpdatePen(); }
			}
		/// <summary>
		/// DEPRECATED: Farbe der Linie im ARGB-Format (Für Serialisierung benötigt)
		/// </summary>
		[XmlIgnore]
		public int argbColor
			{
			get { return m_color.ToArgb(); }
			set { m_color = Color.FromArgb(value); }
			}


		/// <summary>
		/// Pen mit dem die Linie gezeichnet werden soll
		/// </summary>
		[XmlIgnore]
		private Pen drawingPen;


		/// <summary>
		/// soll die Durchschnittsgeschwindigkeit durch den Zeichenstil visualisiert werden?
		/// </summary>
		private bool m_visualizeAverageSpeed = false;
		/// <summary>
		/// soll die Durchschnittsgeschwindigkeit durch den Zeichenstil visualisiert werden?
		/// </summary>
		[XmlIgnore]
		public bool visualizeAverageSpeed
			{
			get { return m_visualizeAverageSpeed; }
			set { m_visualizeAverageSpeed = value; UpdatePen(); }
			}


		/// <summary>
		/// sorgt dafür, dass der Pen neu gesetzt wird
		/// </summary>
		private void UpdatePen()
			{
			if (m_visualizeAverageSpeed)
				{
				float averageSpeed = getAverageSpeedOfVehicles();
				if (averageSpeed < 2)
					{
					drawingPen = new Pen(Color.DarkViolet, 12);
					}
				else if (averageSpeed < 4)
					{
					drawingPen = new Pen(Color.DarkRed, 12);
					}
				else if (averageSpeed < 6)
					{
					drawingPen = new Pen(Color.Red, 12);
					}
				else if (averageSpeed < 8)
					{
					drawingPen = new Pen(Color.Orange, 12);
					}
				else if (averageSpeed < 10)
					{
					drawingPen = new Pen(Color.Yellow, 12);
					}
				else if (averageSpeed < 12)
					{
					drawingPen = new Pen(Color.YellowGreen, 12);
					}
				else 
					{
					drawingPen = new Pen(Color.Lime, 12);
					}
				}
			else
				{
				if (m_carsAllowed && !m_busAllowed && !m_tramAllowed)
					{
					drawingPen = new Pen(Color.LightGray, priority);
					}
				else if (!m_carsAllowed && m_busAllowed && !m_tramAllowed)
					{
					drawingPen = new Pen(Color.LightBlue, priority);
					}
				else if (!m_carsAllowed && !m_busAllowed && m_tramAllowed)
					{
					drawingPen = new Pen(Color.Black, priority);
					}

				else if (m_carsAllowed && !m_busAllowed && m_tramAllowed)
					{
					drawingPen = new Pen(new HatchBrush(HatchStyle.LargeConfetti, Color.Black, Color.LightGray), m_priority);
					}
				else if (m_carsAllowed && m_busAllowed && !m_tramAllowed)
					{
					drawingPen = new Pen(new HatchBrush(HatchStyle.LargeConfetti, Color.LightBlue, Color.LightGray), m_priority);
					}
				else if (!m_carsAllowed && m_busAllowed && m_tramAllowed)
					{
					drawingPen = new Pen(new HatchBrush(HatchStyle.LargeConfetti, Color.Black, Color.LightBlue), m_priority);
					}

				else
					{
					drawingPen = new Pen(Color.DarkBlue, priority);
					}
				}
			}
		#endregion

		#endregion

		#region Konstruktoren

		/// <summary>
        /// Standardconstruktor (!!! NICHT VERVENDEN !!!) [wird nur für XML Serialisierung gebraucht]
        /// </summary>
        public NodeConnection()
            {
            //** Hier passiert gar nix - die NodeConnection ist nicht funktionsfähig ***\\

			// den intersectionComparer müssen wir trotzdem erstellen...
			intersectionComparer = delegate(Intersection a, Intersection b)
			{
				bool aA = (this == a.aConnection);
				bool bA = (this == b.aConnection);

				if (aA && bA)
					return a.aTime.CompareTo(b.aTime);
				else if (!aA && bA)
					return a.bTime.CompareTo(b.aTime);
				else if (aA && !bA)
					return a.aTime.CompareTo(b.bTime);
				else
					return a.bTime.CompareTo(b.bTime);
			};

			m_intersections = new SortedLinkedList<Intersection>(intersectionComparer);
			}

		/// <summary>
		/// erstellt eine neue NodeConnection
		/// </summary>
		/// <param name="startNode">Anfangsknoten</param>
		/// <param name="endNode">Endknoten</param>
		/// <param name="ls">LineSegment</param>
		/// <param name="priority">Priorität</param>
		/// <param name="carsAllowed">Flag, ob Autos auf dieser NodeConnection erlaubt sind</param>
		/// <param name="busAllowed">Flag, ob Busse auf dieser NodeConnection erlaubt sind</param>
		/// <param name="tramAllowed">Flag, ob Straßenbahnen auf dieser NodeConnection erlaubt sind</param>
		/// <param name="enableIncomingLineChange">Flag, ob Spurwechsel auf diese NodeConnection erlaubt sind</param>
		/// <param name="enableOutgoingLineChange">Flag, ob Spurwechsel von dieser NodeConnection erlaubt sind</param>
		public NodeConnection(
			LineNode startNode, 
			LineNode endNode, 
			LineSegment ls, 
			int priority, 
			bool carsAllowed,
			bool busAllowed,
			bool tramAllowed,
			bool enableIncomingLineChange,
			bool enableOutgoingLineChange)
            {
			// TODO: NodeConnections werden stets mit lineSegment = null initialisiert? Warum?
            this.startNode = startNode;
            this.endNode = endNode;
            lineSegment = ls;

			this.m_priority = priority;
			this.m_carsAllowed = carsAllowed;
			this.m_busAllowed = busAllowed;
			this.m_tramAllowed = tramAllowed;
			this.m_enableIncomingLineChange = enableIncomingLineChange;
			this.m_enableOutgoingLineChange = enableOutgoingLineChange;

			UpdatePen();

			intersectionComparer = delegate(Intersection a, Intersection b)
				{
				bool aA = (this == a.aConnection);
				bool bA = (this == b.aConnection);

				if (aA && bA)
					return a.aTime.CompareTo(b.aTime);
				else if (!aA && bA)
					return a.bTime.CompareTo(b.aTime);
				else if (aA && !bA)
					return a.aTime.CompareTo(b.bTime);
				else
					return a.bTime.CompareTo(b.bTime);
				};

			m_intersections = new SortedLinkedList<Intersection>(intersectionComparer);
			}

		#endregion

		#region Methoden


		/// <summary>
		/// Fügt den LineChangePoint lcp dieser NodeConnection hinzu und aktualisiert die entsprechenden LineChangeIntervals
		/// </summary>
		/// <param name="lcp">hinzuzufügender LineChangePoint</param>
		public void AddLineChangePoint(LineChangePoint lcp)
			{
			LineChangeInterval lci;
			// prüfen, ob der Zielknoten schon im LineChangeInterval-Dictionary enthalten ist
			if (m_lineChangeIntervals.TryGetValue(lcp.target.nc.endNode.hashcode, out lci))
				{
				// er ist schon vorhanden, also passen wir ihn evtl. an
				if (lci.startArcPos > lcp.start.arcPosition)
					{
					lci.startArcPos = lcp.start.arcPosition;
					}
				else if (lci.endArcPos < lcp.start.arcPosition)
					{
					lci.endArcPos = lcp.start.arcPosition;
					}
				}
			else
				{
				// er ist noch nicht vorhanden, also legen wir einen neuen an
				lci = new LineChangeInterval(lcp.target.nc.endNode, lcp.start.arcPosition, lcp.start.arcPosition);
				m_lineChangeIntervals.Add(lcp.target.nc.endNode.hashcode, lci);
				m_viaLineChangeReachableNodes.Add(lcp.target.nc.endNode);
				}

			m_lineChangePoints.Add(lcp);
			}

		/// <summary>
		/// entfernt alle LineChangePoints und LineChangeIntervals dieser NodeConnection
		/// </summary>
		public void ClearLineChangePoints()
			{
			m_lineChangePoints.Clear();
			m_lineChangeIntervals.Clear();
			m_viaLineChangeReachableNodes.Clear();
			}


		/// <summary>
		/// berechnet die Bogenlängenentfernung vom Beginn dieser NodeConnection zum LineNode ln, wobei der erstbeste LineChangePoint zum wechseln benutzt wird.
		/// </summary>
		/// <param name="ln">Zielknoten</param>
		/// <returns></returns>
		public double GetLengthToLineNodeViaLineChange(LineNode ln)
			{
			foreach (LineChangePoint lcp in m_lineChangePoints)
				{
				if (lcp.target.nc.endNode == ln)
					{
					return lcp.start.arcPosition + lcp.length + lcp.target.nc.lineSegment.length - lcp.target.arcPosition;
					}
				}
			return Double.PositiveInfinity;
			}

		/// <summary>
		/// entfernt alle LineChangePoints und LineChangeIntervals zur NodeConnection nc
		/// </summary>
		/// <param name="nc">Ziel-NodeConnection der zu löschenden LineChangePoints</param>
		public void RemoveAllLineChangePointsTo(NodeConnection nc)
			{
			for (int i = 0; i < m_lineChangePoints.Count; i++)
				{
				if ((m_lineChangePoints[i].otherStart.nc == nc) || (m_lineChangePoints[i].target.nc == nc))
					{
					m_lineChangePoints.RemoveAt(i);
					i--;
					}
				}

			m_lineChangeIntervals.Remove(nc.endNode.hashcode);
			m_viaLineChangeReachableNodes.Remove(nc.endNode);
			}



		/// <summary>
		/// setzt das LineSegment auf lc
		/// </summary>
		/// <param name="lc">LineSegment, welches gesetzt werden soll</param>
		public void SetLineSegment(LineSegment lc)
            {
            lineSegment = lc;
            }

        /// <summary>
        /// Gibt rekursiv das nächste TrafficLight zurück
        /// </summary>
        /// <param name="distance">Maximale Suchreichweite</param>
        public TrafficLight GetNextTrafficLightWithin(double distance)
            {
            int currentDistance = 0;
            LineNode currentNode = this.endNode;
            while (currentDistance <= distance)
                {
                if (currentNode.tLight != null)
                    {
                    return currentNode.tLight;
                    }
                else
                    {
                    currentDistance += 0;
                    }
                }
            return null;
			}

		/// <summary>
		/// gibt den letzten LineChangePoint des NodeConnection zurück, der sich vor arcPosition befindet.
		/// (binäre Suche -> logarithmische Laufzeit)
		/// </summary>
		/// <param name="arcPosition">Bogenlängenposition, vor der gesucht werden soll</param>
		/// <returns>den letzte LineChangePoint mit lcp.myArcPosition \leq arcPosition oder null</returns>
		public LineChangePoint GetPrevLineChangePoint(double arcPosition)
			{
			if (m_lineChangePoints.Count > 0 && m_lineChangePoints[0].start.arcPosition < arcPosition)
				{
				int lBorder = 0;
				int rBorder = m_lineChangePoints.Count - 1;

				while (rBorder - lBorder > 1)
					{
					int i = (rBorder + lBorder) / 2;

					if (arcPosition == m_lineChangePoints[i].start.arcPosition)
						{
						return m_lineChangePoints[i];
						}
					else if (m_lineChangePoints[i].start.arcPosition < arcPosition)
						{
						lBorder = i ;
						}
					else
						{
						rBorder = i - 1;
						}
					}

				return m_lineChangePoints[lBorder];
				}
			else
				{
				return new LineChangePoint();
				}
			}

		/// <summary>
		/// gibt den nächsten LineChangePoint des NodeConnection zurück, der sich hinter arcPosition befindet.
		/// (binäre Suche -> logarithmische Laufzeit)
		/// </summary>
		/// <param name="arcPosition">Bogenlängenposition, ab der gesucht werden soll</param>
		/// <returns>den ersten LineChangePoint mit lcp.myArcPosition >= arcPosition oder null</returns>
		public LineChangePoint GetNextLineChangePoint(double arcPosition)
			{
			if (m_lineChangePoints.Count > 0 && m_lineChangePoints[0].start.arcPosition < arcPosition)
				{
				int lBorder = 0;
				int rBorder = m_lineChangePoints.Count - 1;

				while (rBorder - lBorder > 1)
					{
					int i = (rBorder + lBorder) / 2;

					if (arcPosition == m_lineChangePoints[i].start.arcPosition)
						{
						return m_lineChangePoints[i];
						}
					else if (m_lineChangePoints[i].start.arcPosition < arcPosition)
						{
						lBorder = i+1;
						}
					else
						{
						rBorder = i;
						}
					}

				return m_lineChangePoints[rBorder];
				}
			else
				{
				return new LineChangePoint();
				}
			}

		#endregion

		#region Intersections

		private SortedLinkedList<Intersection>.CompareDelegate intersectionComparer;

		/// <summary>
		/// Liste von Intersections
		/// </summary>
		private SortedLinkedList<Intersection> m_intersections;// = new SortedLinkedList<Intersection>(intersectionComparer);
		/// <summary>
		/// Liste von Intersections
		/// </summary>
		[XmlIgnore]
		public SortedLinkedList<Intersection> intersections
			{
			get { return m_intersections; }
			}


		/// <summary>
		/// macht der NodeConnection eine Intersection bekannt
		/// </summary>
		/// <param name="i">Intersection-Objekt mit allen nötigen Informationen</param>
		public void AddIntersection(Intersection i)
			{
			if (this == i.aConnection)
				i.aListNode = intersections.Add(i);
			else
				i.bListNode = intersections.Add(i);
			}

		/// <summary>
		/// Meldet die Intersection i bei der NodeConnection ab
		/// </summary>
		/// <param name="i">abzumeldende Intersection</param>
		public void RemoveIntersection(Intersection i)
			{
			intersections.Remove(i);
			}


		/// <summary>
		/// Liefert alle Intersections im Zeitintervall interval
		/// </summary>
		/// <param name="interval">Intervall in dem nach Intersections gesucht werden soll</param>
		/// <returns>Eine Liste von Intersections die im Zeitintervall interval auf dieser Linie vorkommen</returns>
		public List<Intersection> GetIntersectionsWithinTime(Interval<double> interval)
			{
			List<Intersection> toReturn = new List<Intersection>();

			foreach (Intersection i in this.intersections)
				{
				if (this == i.aConnection)
					{
					if (interval.Contains(i.aTime))
						{
						toReturn.Add(i);
						}
					}
				else
					{
					if (interval.Contains(i.bTime))
						{
						toReturn.Add(i);
						}
					}
				}

			return toReturn;
			}

		/// <summary>
		/// Liefert alle Intersections im Bogenlängenintervall intervall
		/// </summary>
		/// <param name="interval">Bogenlängeintervall in dem die Intersections liegen sollen</param>
		/// <returns>Eine Liste von Intersections, die im Bogenlängenintervall intervall liegen</returns>
		public List<SpecificIntersection> GetIntersectionsWithinArcLength(Interval<double> interval)
			{
			List<SpecificIntersection> toReturn = new List<SpecificIntersection>();

			foreach (Intersection i in this.intersections)
				{
				if (this == i.aConnection)
					{
					if (interval.Contains(i.aArcPosition))
						{
						toReturn.Add(new SpecificIntersection(this, i));
						}
					}
				else
					{
					if (interval.Contains(i.bArcPosition))
						{
						toReturn.Add(new SpecificIntersection(this, i));
						}
					}
				}

			return toReturn;
			}


		#endregion

		#region Autos auf der Linie
		/// <summary>
		/// Auf der NodeConnection fahrende IVehicles
		/// </summary>
		[XmlIgnore]
        public MyLinkedList<IVehicle> vehicles = new MyLinkedList<IVehicle>();

		/// <summary>
		/// von dieser NodeConnection zu entfernende IVehicles. (wird benötigt, da sonst in Iteratoren die Auflistung verändert würde)
		/// </summary>
		[XmlIgnore]
		private List<IVehicle> vehiclesToRemove = new List<IVehicle>();

        /// <summary>
        /// Fügt der Linie ein Auto hinzu und sorgt dafür
        /// </summary>
		/// <param name="veh">hinzuzufügendes Vehicle</param>
		/// <param name="targetNodes">Liste von Zielknoten</param>
        public void AddVehicle(IVehicle veh, List<LineNode> targetNodes)
            {
			// prüfen, ob die Linie nicht schon zu voll mit Autos ist
			if (vehicles.First == null || vehicles.First.Value.currentPosition - vehicles.First.Value.length > veh.physics.velocity + veh.state.position)
				{
				vehicles.AddFirst(veh);
				veh.listNode = vehicles.First;
				veh.targetNodes = targetNodes;
				}
            }

		/// <summary>
		/// Fügt der Linie ein Auto hinzu
		/// </summary>
		/// <param name="veh">hinzuzufügendes Vehicle</param>
		public void AddVehicle(IVehicle veh)
			{
			// prüfen, ob die Linie nicht schon zu voll mit Autos ist
			if (vehicles.First == null || vehicles.First.Value.currentPosition - vehicles.First.Value.length > veh.physics.velocity + veh.state.position)
				{
				vehicles.AddFirst(veh);
				veh.listNode = vehicles.First;
				}
			}


		/// <summary>
		/// fügt das IVehicle v an der Position arcPosition ein und sorgt dabei für eine weiterhin korrekte Verkettung von m_vehicles
		/// </summary>
		/// <param name="v">einzufügendes Auto</param>
		/// <param name="arcPosition">Bogenlängenposition, wo das Auto eingefügt werden soll</param>
		public void AddVehicleAt(IVehicle v, double arcPosition)
			{
			LinkedListNode<IVehicle> lln = GetVehicleListNodeBehindArcPosition(arcPosition);
			if (lln != null)
				{
				if (lln.Value.currentPosition - lln.Value.length < arcPosition)
					{
					throw new Exception("Das neue Fahrzeug überlappt sich mit einem anderen Fahrzeug!");
					}
				v.listNode = vehicles.AddBefore(lln, v);
				}
			else
				{
				v.listNode = vehicles.AddLast(v);
				}
			}


		/// <summary>
		/// Meldet das IVehicle v von dieser NodeConnection ab. (Effektiv wird dieses Auto erst am Ende des Ticks entfernt)
		/// </summary>
		/// <param name="v">zu entfernendes Auto</param>
		/// <param name="averageSpeed">Durchschnittsgeschwindigkeit in m/s, die das Fahrzeug v auf der NodeConnection verbracht hat</param>
		public void RemoveVehicle(IVehicle v, float averageSpeed)
			{
			if (! vehicles.Contains(v))
				{
				throw new Exception("Vehicle " + v + " nicht auf dieser NodeConnection!");
				}

			if (averageSpeed >= 0)
				{
				m_countOfVehicles++;
				m_sumOfAverageSpeeds += averageSpeed;
				if (m_visualizeAverageSpeed)
					UpdatePen();
				}

			vehiclesToRemove.Add(v);
			}

		/// <summary>
		/// entfernt alle IVehicles in vehiclesToRemove aus vehicles
		/// </summary>
		public void RemoveAllVehiclesInRemoveList()
			{
			foreach (IVehicle v in vehiclesToRemove)
				{
				vehicles.Remove(v);
				}
			vehiclesToRemove.Clear();
			}

		/// <summary>
		/// bestimmt des LinkedListNode des ersten IVehicles hinter der Bogenlängenposition arcPosition
		/// </summary>
		/// <param name="arcPosition">Bogenlängenposition, ab dem das erste IVehicle zurückgegeben werden soll</param>
		/// <returns>Der erste LinkedListNode mit Value.currentPosition >= arcPosition oder null, falls kein solches existiert</returns>
		public LinkedListNode<IVehicle> GetVehicleListNodeBehindArcPosition(double arcPosition)
			{
			LinkedListNode<IVehicle> lln = vehicles.First;

			while (lln != null)
				{
				if (lln.Value.currentPosition > arcPosition)
					{
					return lln;
					}

				lln = lln.Next;
				}

			return lln;
			}

		/// <summary>
		/// bestimmt des LinkedListNode des ersten IVehicles vor der Bogenlängenposition arcPosition
		/// </summary>
		/// <param name="arcPosition">Bogenlängenposition, ab dem das erste IVehicle zurückgegeben werden soll</param>
		/// <returns>Der erste LinkedListNode mit Value.currentPosition kleinergleich arcPosition oder null, falls kein solches existiert</returns>
		public LinkedListNode<IVehicle> GetVehicleListNodeBeforeArcPosition(double arcPosition)
			{
			LinkedListNode<IVehicle> lln = vehicles.Last;

			while (lln != null)
				{
				if (lln.Value.currentPosition < arcPosition)
					{
					return lln;
					}

				lln = lln.Previous;
				}

			return lln;
			}


		/// <summary>
		/// bestimmt das IVehicle hinter der Bogenlängenposition arcPosition und die Entfernung dorthin.
		/// Ist distanceWithin größer als die Entfernung zum endNode, so werden alle weiteren ausgehenden NodeConnections auch untersucht
		/// </summary>
		/// <param name="arcPosition">Bogenlängenposition, wo die Suche gestartet wird</param>
		/// <param name="distanceWithin">maximal verbleibende Suchreichweite</param>
		/// <returns>VehicleDistance zum nächsten IVehicle mit maximalem Abstand distanceWithin</returns>
		public VehicleDistance GetVehicleBehindArcPosition(double arcPosition, double distanceWithin)
			{
			// sich selbst auf Autos untersuchen
			LinkedListNode<IVehicle> lln = GetVehicleListNodeBehindArcPosition(arcPosition);
			if (lln != null)
				{
				return new VehicleDistance(lln.Value, lln.Value.currentPosition - arcPosition);
				}
			// auf dieser Connection ist kein Auto mehr in Reichweite
			else
				{
				// verbleibende Suchreichweite bestimmen
				double remainingDistance = distanceWithin - (lineSegment.length - arcPosition);

				if (remainingDistance <= 0)
					{
					return null;
					}
				else
					{
					// ich muss noch die ausgehenden Connections untersuchen
					if (endNode.nextConnections.Count > 0)
						{
						List<VehicleDistance> vdList = new List<VehicleDistance>();
						foreach (NodeConnection nc in endNode.nextConnections)
							{
							VehicleDistance foo = nc.GetVehicleBehindArcPosition(0, remainingDistance);
							if (foo != null)
								{
								vdList.Add(foo);
								}
							}

						if (vdList.Count == 0)
							{
							return null;
							}
						else
							{
							// Minimum bestimmen und zurückgeben
							double minValue = vdList[0].distance;
							VehicleDistance toReturn = vdList[0];

							foreach (VehicleDistance vd in vdList)
								{
								if (vd.distance < minValue)
									{
									toReturn = vd;
									minValue = vd.distance;
									}
								}
							toReturn.distance += (lineSegment.length - arcPosition);
							return toReturn;
							}
						}

					return null;
					}
				}
			}


		/// <summary>
		/// bestimmt das IVehicle hinter der Bogenlängenposition arcPosition und die Entfernung dorthin.
		/// Ist distanceWithin größer als die Entfernung zum endNode, so werden alle weiteren ausgehenden NodeConnections auch untersucht
		/// </summary>
		/// <param name="arcPosition">Bogenlängenposition, wo die Suche gestartet wird</param>
		/// <param name="distanceWithin">maximal verbleibende Suchreichweite</param>
		/// <returns>VehicleDistance zum vorherigen IVehicle mit maximalem Abstand distanceWithin, VehicleDistance.distance ist dabei positiv!</returns>
		public VehicleDistance GetVehicleBeforeArcPosition(double arcPosition, double distanceWithin)
			{
			// sich selbst auf Autos untersuchen
			LinkedListNode<IVehicle> lln = GetVehicleListNodeBeforeArcPosition(arcPosition);
			if (lln != null)
				{
				return new VehicleDistance(lln.Value, arcPosition - lln.Value.currentPosition);
				}
			// auf dieser Connection ist kein Auto mehr in Reichweite
			else
				{
				// verbleibende Suchreichweite bestimmen
				double remainingDistance = distanceWithin - arcPosition;

				if (remainingDistance <= 0)
					{
					return null;
					}
				else
					{
					// ich muss noch die eingehenden Connections untersuchen
					if (startNode.prevConnections.Count > 0)
						{
						List<VehicleDistance> vdList = new List<VehicleDistance>();
						foreach (NodeConnection nc in startNode.prevConnections)
							{
							VehicleDistance foo = nc.GetVehicleBeforeArcPosition(nc.lineSegment.length, remainingDistance);
							if (foo != null)
								{
								vdList.Add(foo);
								}
							}

						if (vdList.Count == 0)
							{
							return null;
							}
						else
							{
							// Minimum bestimmen und zurückgeben
							double minValue = vdList[0].distance;
							VehicleDistance toReturn = vdList[0];

							foreach (VehicleDistance vd in vdList)
								{
								if (vd.distance < minValue)
									{
									toReturn = vd;
									minValue = vd.distance;
									}
								}
							toReturn.distance += arcPosition;
							return toReturn;
							}
						}
					}
				}
			return null;
			}

		/// <summary>
		/// bestimmt die Autos vor und hinter der Bogenlängenposition arcPosition. 
		/// Ist maxDistance größer als die Entfernung zum start-/endNode, so werden alle ein-/ausgehenden Connections auch durchsucht.
		/// </summary>
		/// <param name="arcPosition">Bogenlängenposition zu denen die Autos bestimmt werden sollen</param>
		/// <param name="distanceWithin">maximale Suchreichweite</param>
		/// <returns>Ein Paar von IVehicles: Left ist das davor, Right das dahinter</returns>
		public Pair<VehicleDistance> GetVehiclesAroundArcPosition(double arcPosition, double distanceWithin)
			{
			//return new Pair<VehicleDistance>(null, null);
			return new Pair<VehicleDistance>(GetVehicleBeforeArcPosition(arcPosition, distanceWithin), GetVehicleBehindArcPosition(arcPosition, distanceWithin));
			}


		
		#endregion

		#region Statistiken

		/// <summary>
		/// gibt die Durchschnittsgeschwindigkeit der auf dieser Connection fahrenden Autos in m/s zurück
		/// </summary>
		/// <returns>Durchschnittsgeschwindigkeit in m/s</returns>
		public float getAverageSpeedOfVehicles()
			{
			if (m_countOfVehicles == 0)
				return 0;
			else
				return m_sumOfAverageSpeeds / m_countOfVehicles;
			}


		#endregion


		#region Hashes
		/// <summary>
		/// HashCode des von startNode
		/// </summary>
        public int startNodeHash;
		/// <summary>
		/// Hashcode von endNode
		/// </summary>
        public int endNodeHash;


		/// <summary>
		/// Gibt den LineNode aus nodesList zurück, dessen Hash mit hash übereinstimmt
		/// </summary>
		/// <param name="nodesList">zu durchsuchende Liste von LineNodes</param>
		/// <param name="hash">auf Gleichheit zu überprüfender Hashcode</param>
		/// <returns>den erstbesten LineNode mit GetHashCode() == hash oder null, falls kein solcher existiert</returns>
        private LineNode GetLineNodeByHash(List<LineNode> nodesList, int hash)
            {
            foreach (LineNode ln in nodesList)
                {
                if (ln.GetHashCode() == hash)
                    {
                    return ln;
                    }
                }
            return null;
            }
        #endregion

        #region Speichern/ Laden
		/// <summary>
		/// Bereitet diese NodeConnection fürs Speichern vor
		/// Insbesondere werden hier die Hashes der Start- und EndNodes zur Speicherung generiert
		/// </summary>
        public void PrepareForSave()
            {
			startNodeHash = startNode.GetHashCode();
			endNodeHash = endNode.GetHashCode();
			}

		/// <summary>
		/// Stellt die Start- und EndNode der NodeConnection anhand der Hashes und der übergebenen Line Liste wieder her
		/// </summary>
		/// <param name="saveVersion">Version der gespeicherten Datei</param>
		/// <param name="nodesList">Eine Liste mit sämtlichen existierenden Linien</param>
        public void RecoverFromLoad(int saveVersion, List<LineNode> nodesList)
            {
			startNode = GetLineNodeByHash(nodesList, startNodeHash);
			endNode = GetLineNodeByHash(nodesList, endNodeHash);
			UpdatePen();
            }
        #endregion

		/// <summary>
		/// gibt Basisinformationen über die NodeConnection als String zurück
		/// </summary>
		/// <returns>"NodeConnection von #" + startNode.hashcode + " nach #" + endNode.hashcode</returns>
		public override string ToString()
			{
			return "NodeConnection von #" + startNode.GetHashCode() + " nach #" + endNode.GetHashCode();
			}

		#region IDrawable Member

		/// <summary>
		/// Zeichnet das Vehicle auf der Zeichenfläche g
		/// </summary>
		/// <param name="g">Die Zeichenfläche auf der gezeichnet werden soll</param>
		public void Draw(Graphics g)
			{
			lineSegment.Draw(g, drawingPen);
			}

		/// <summary>
		/// Zeichnet Debuginformationen auf die Zeichenfläche g
		/// </summary>
		/// <param name="g">Die Zeichenfläche auf der gezeichnet werden soll</param>
		public void DrawDebugData(Graphics g)
			{
			using (Pen blackPen = new Pen(Color.DarkGray, 1))
				{
				foreach (LineChangePoint lcp in m_lineChangePoints)
					{
					// LineChangeIntervals malen:
					/*LineChangeInterval lci;
					m_lineChangeIntervals.TryGetValue(lcp.target.nc.endNode.hashcode, out lci);

					g.DrawLine(blackPen, lineSegment.AtPosition(lci.startArcPos), lineSegment.AtPosition(lci.endArcPos));
					g.DrawLine(blackPen, lineSegment.AtPosition(lci.endArcPos), lci.targetNode.position);
					g.DrawLine(blackPen, lci.targetNode.position, lineSegment.AtPosition(lci.startArcPos));
					*/
					// LineChangePoints malen:
					lcp.lineSegment.Draw(g, new Pen(Color.Orange, 1));

					/*				Vector2 leftVector = lineSegment.DerivateAtTime(lcp.start.time).RotatedClockwise.Normalized * lcp.parallelDistance;
									g.DrawLine(new Pen(Color.Gray, 1), lineSegment.AtTime(lcp.start.time), lineSegment.AtTime(lcp.start.time) + lineSegment.DerivateAtTime(lcp.start.time));
									g.DrawLine(new Pen(Color.Gray, 1), lineSegment.AtTime(lcp.start.time), lineSegment.AtTime(lcp.start.time) + leftVector);
					*/
					g.FillEllipse(new SolidBrush(Color.Green), new Rectangle(lineSegment.AtTime(lcp.start.time) - new Vector2(2, 2), new Size(3, 3)));
					g.FillEllipse(new SolidBrush(Color.Orange), new Rectangle(lcp.otherStart.nc.lineSegment.AtTime(lcp.otherStart.time) - new Vector2(2, 2), new Size(3, 3)));
					g.FillEllipse(new SolidBrush(Color.Red), new Rectangle(lcp.target.nc.lineSegment.AtTime(lcp.target.time) - new Vector2(2, 2), new Size(3, 3)));
					}
				}

			g.DrawString(/*"Länge: " + (lineSegment.length/10) + "m\n*/"avg Speed:" + getAverageSpeedOfVehicles() + " m/s", new Font("Arial", 9), new SolidBrush(Color.Black), lineSegment.AtTime(0.5));
			}

		#endregion

		#region Subklassen

		/// <summary>
		/// kapselt eine NodeConnection, Zeitposition und Bogenlängenposition auf ihr zusammen
		/// </summary>
		public struct SpecificPosition
			{
			/// <summary>
			/// erstellt eine SpecificPosition mittels NodeConnection und Zeitparameter
			/// </summary>
			/// <param name="nc">NodeConnection</param>
			/// <param name="time">Zeitparameter auf der NodeConnection</param>
			public SpecificPosition(NodeConnection nc, double time)
				{
				this.nc = nc;
				this.time = time;
				this.arcPosition = nc.lineSegment.TimeToArcPosition(time);
				}

			/// <summary>
			/// erstellt eine SpecificPosition mittels NodeConnection und Bogenlängenposition
			/// </summary>
			/// <param name="arcPosition">Bogenlängenposition</param>
			/// <param name="nc">NodeConnection</param>
			public SpecificPosition(double arcPosition, NodeConnection nc)
				{
				this.nc = nc;
				this.arcPosition = arcPosition;
				this.time = nc.lineSegment.PosToTime(arcPosition);
				}

			/// <summary>
			/// gekapselte NodeConnection
			/// </summary>
			public NodeConnection nc;

			/// <summary>
			/// gekapselte Zeit
			/// </summary>
			public double time;

			/// <summary>
			/// gekapselte Bogenlängenposition
			/// </summary>
			public double arcPosition;
			}

		/// <summary>
		/// Sturktur, die einen möglichen Spurwechselort zusammenfasst
		/// </summary>
		public struct LineChangePoint
			{
			/// <summary>
			/// erstellt einen neuen LineChangePoint
			/// </summary>
			/// <param name="start">Position, wo der LineChangePoint beginnt</param>
			/// <param name="target">Position, wo der LineChangePoint auf die ZielConnection trifft</param>
			/// <param name="otherStart">Position die auf der Zielspur auf Höhe von start liegt (parallel gesehen)</param>
			public LineChangePoint(SpecificPosition start, SpecificPosition target, SpecificPosition otherStart)
				{
				this.start = start;
				this.target = target;
				this.otherStart = otherStart;

				parallelDistance = (start.nc.lineSegment.AtTime(start.time) - otherStart.nc.lineSegment.AtTime(otherStart.time)).Abs;

				lineSegment = new LineSegment(0,
					start.nc.lineSegment.AtTime(start.time),
					start.nc.lineSegment.AtTime(start.time) + start.nc.lineSegment.DerivateAtTime(start.time).Normalized * (parallelDistance),
					target.nc.lineSegment.AtTime(target.time) - target.nc.lineSegment.DerivateAtTime(target.time).Normalized * (parallelDistance),
					target.nc.lineSegment.AtTime(target.time));
				}


			/// <summary>
			/// Position, wo der LineChangePoint beginnt
			/// </summary>
			public SpecificPosition start;

			/// <summary>
			/// Position, wo der LineChangePoint auf die ZielConnection trifft
			/// </summary>
			public SpecificPosition target;

			/// <summary>
			/// Position die auf der Zielspur auf Höhe von start liegt (parallel gesehen)
			/// </summary>
			public SpecificPosition otherStart;

			/// <summary>
			/// Euklidische Entfernung zwischen den beiden NodeConnections
			/// </summary>
			public double parallelDistance;
			
			/// <summary>
			/// Länge der Bézierkurve des Spurwechsels
			/// </summary>
			public double length
				{
				get { return lineSegment.length; }
				}

			/// <summary>
			/// Bézierkurve, die das Fahrzeug bei Benutzung dieses LineChangePoints entlang fährt
			/// </summary>
			public LineSegment lineSegment;
			}

		/// <summary>
		/// kapselt Informationen, wie man den targetNode über Spurwechsel erreicht.
		/// ist eine class, damit Call-By-Reference genutzt wird
		/// </summary>
		public class LineChangeInterval
			{
			/// <summary>
			/// Zielknoten, der über Spurwechsel erreicht werden soll
			/// </summary>
			public LineNode targetNode;

			/// <summary>
			/// Bogenlängenposition, ab dem Spurwechsel zum targetNode möglich sind
			/// </summary>
			public double startArcPos;

			/// <summary>
			/// Bogenlängenposition, bis zu dem Spurwechsel zum targetNode möglich sind
			/// </summary>
			public double endArcPos;

			/// <summary>
			/// Intervall von Bogenlängenposition, in dem der Spurwechsel zum targetNode möglich ist
			/// </summary>
			public Interval<double> arcPosInterval
				{
				get { return new Interval<double>(startArcPos, endArcPos); }
				}

			/// <summary>
			/// Länge des Intervall, in dem der Spurwechsel möglich ist
			/// </summary>
			public double length
				{
				get { return endArcPos - startArcPos; }
				}


			/// <summary>
			/// erstellt ein neues LineChangeInterval
			/// </summary>
			/// <param name="targetNode">Zielknoten, der über Spurwechsel erreicht werden soll</param>
			/// <param name="startArcPos">Bogenlängenposition, ab dem Spurwechsel zum targetNode möglich sind</param>
			/// <param name="endArcPos">Bogenlängenposition, bis zu dem Spurwechsel zum targetNode möglich sind</param>
			public LineChangeInterval(LineNode targetNode, double startArcPos, double endArcPos)
				{
				this.targetNode = targetNode;
				this.startArcPos = startArcPos;
				this.endArcPos = endArcPos;
				}
			}

		#endregion
		}
    }
