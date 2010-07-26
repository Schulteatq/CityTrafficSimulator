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

using CityTrafficSimulator.Vehicle;

namespace CityTrafficSimulator
	{
	/// <summary>
	/// Klasse zur Kapselung eines Schnittpunktes zweier NodeConnections A und B
	/// </summary>
	[Serializable]
	public class Intersection
		{


		#region Variablen und Eigenschaften

		/// <summary>
		/// Winkel der beiden NodeConnections zueinander an der Intersection 
		/// </summary>
		private double m_angle;
		/// <summary>
		/// Winkel der beiden NodeConnections zueinander an der Intersection 
		/// </summary>
		public double angle
			{
			get { return m_angle; }
			set { m_angle = value; }
			}

		/// <summary>
		/// NodeConnection A
		/// </summary>
		private NodeConnection m_aConnection;
		/// <summary>
		/// NodeConnection A
		/// </summary>
		public NodeConnection aConnection
			{
			get { return m_aConnection; }
			set { m_aConnection = value; }
			}

		/// <summary>
		/// LinkedListNode der von NodeConnection A auf die Intersection verweist
		/// </summary>
		private LinkedListNode<Intersection> m_aListNode;
		/// <summary>
		/// LinkedListNode der von NodeConnection A auf die Intersection verweist
		/// </summary>
		public LinkedListNode<Intersection> aListNode
			{
			get { return m_aListNode; }
			set { m_aListNode = value; }
			}

		/// <summary>
		/// Zeitpunkt der NodeConnection A des Schnittpunktes
		/// </summary>
		private double m_aTime;
		/// <summary>
		/// Zeitpunkt der NodeConnection A des Schnittpunktes
		/// </summary>
		public double aTime
			{
			get { return m_aTime; }
			set { m_aTime = value; }
			}
		/// <summary>
		/// absolute Position der Intersection auf NodeConnection A
		/// </summary>
		public Vector2 aPosition
			{
			get { return aConnection.lineSegment.AtTime(aTime); }
			}
		/// <summary>
		/// Bogenlängenposition des Schnittpunktes auf NodeConnection A
		/// </summary>
		public double aArcPosition
			{
			get { return aConnection.lineSegment.TimeToArcPosition(aTime); }
			}

		/// <summary>
		/// NodeConnection B
		/// </summary>
		private NodeConnection m_bConnection;
		/// <summary>
		/// NodeConnection B
		/// </summary>
		public NodeConnection bConnection
			{
			get { return m_bConnection; }
			set { m_bConnection = value; }
			}

		/// <summary>
		/// LinkedListNode der von NodeConnection B auf die Intersection verweist
		/// </summary>
		private LinkedListNode<Intersection> m_bListNode;
		/// <summary>
		/// LinkedListNode der von NodeConnection B auf die Intersection verweist
		/// </summary>
		public LinkedListNode<Intersection> bListNode
			{
			get { return m_bListNode; }
			set { m_bListNode = value; }
			}

		/// <summary>
		/// Zeitpunkt der NodeConnection B des Schnittpunktes
		/// </summary>
		private double m_bTime;
		/// <summary>
		/// Zeitpunkt der NodeConnection B des Schnittpunktes
		/// </summary>
		public double bTime
			{
			get { return m_bTime; }
			set { m_bTime = value; }
			}
		/// <summary>
		/// absolute Position der Intersection auf NodeConnection B
		/// </summary>
		public Vector2 bPosition
			{
			get { return bConnection.lineSegment.AtTime(bTime); }
			}
		/// <summary>
		/// Bogenlängenposition des Schnittpunktes auf NodeConnection B
		/// </summary>
		public double bArcPosition
			{
			get { return bConnection.lineSegment.TimeToArcPosition(bTime); }
			}

		#endregion

		#region Konstruktoren

		/// <summary>
		/// Leerer Standardkonstruktor
		/// (nicht verwenden - wird nur für XML Serialisierung gebraucht)
		/// </summary>
		public Intersection()
			{

			}

		/// <summary>
		/// Standardkonstruktor
		/// Erstellt ein neues Intersection Objekt
		/// Die Parameter sollten so gewählt sein, dass aConnection.lineSegment.AtTime(aTime) ~ bConnection.lineSegment.AtTime(bTime)
		/// </summary>
		/// <param name="aConnection">NodeConnection A</param>
		/// <param name="bConnection">NodeConnection B</param>
		/// <param name="aTime">Zeitpunkt des Schnittpunktes an NodeConnection A</param>
		/// <param name="bTime">Zeitpunkt des Schnittpunktes an NodeConnection B</param>
		public Intersection(NodeConnection aConnection, NodeConnection bConnection, double aTime, double bTime)
			{
			this.aConnection = aConnection;
			this.bConnection = bConnection;
			this.aTime = aTime;
			this.bTime = bTime;

			angle = Vector2.AngleBetween(aConnection.lineSegment.DerivateAtTime(aTime), bConnection.lineSegment.DerivateAtTime(bTime));
			}

		#endregion

		#region Methoden

		/// <summary>
		/// Distanz, die ein wartendes Fahrzeug mindestens zur Intersection halten sollte.
		/// Diese ist abhängig vom Schnittwinkel der beiden NodeConnections.
		/// </summary>
		/// <returns>(angle - Math.PI / 2) * 8</returns>
		public double GetWaitingDistance()
			{
			double toReturn = Math.Pow(1.5d* Math.Abs(angle - Math.PI/2), 2) * 8;
			return toReturn;
			}

		/// <summary>
		/// prüft ob die NodeConnection nc an der Intersection teilnimmt
		/// </summary>
		/// <param name="nc">zu prüfende NodeConnection</param>
		/// <returns>(nc == aConnection) || (nc == bConnection)</returns>
		public bool ContainsNodeConnection(NodeConnection nc)
			{
			return (nc == aConnection) || (nc == bConnection);
			}


		/// <summary>
		/// sorgt dafür, dass sich die Intersection bei den NodeConnections abmeldet
		/// </summary>
		public void Dispose()
			{
			if (m_handler != null)
				{
				m_handler.RemoveHandledIntersection(this);
				}			
			aConnection.RemoveIntersection(this);
			bConnection.RemoveIntersection(this);
			}


		/// <summary>
		/// Gibt die andere an der Intersection teilhabende NodeConnection zurück
		/// </summary>
		/// <param name="nc">NodeConnection die nicht zurückgegeben werden soll</param>
		/// <returns>Die NodeConnection, die sich in dieser Intersection mit nc schneidet oder null, wenn nc nicht an der Intersection teilnimmt</returns>
		public NodeConnection GetOtherNodeConnection(NodeConnection nc)
			{
			if (aConnection == nc)
				{
				return bConnection;
				}
			else if (bConnection == nc)
				{
				return aConnection;
				}
			else
				{
				return null;
				}
			}


		/// <summary>
		/// Gibt den Zeitpunkt des Schnittpunktes an der NodeConnection nc an
		/// </summary>
		/// <param name="nc">NodeConnection dessen Schnittpunkt-Zeitparameter zurückgegeben werden soll</param>
		/// <returns>aTime/bTime falls nc=aConnection/nc=bConnection, sonst Exception</returns>
		public double GetMyTime(NodeConnection nc)
			{
			if (aConnection == nc)
				return m_aTime;
			else if (bConnection == nc)
				return m_bTime;
			else
				throw new Exception();
			}


		/// <summary>
		/// Gibt Bogenlängenposition des Schnittpunktes an der NodeConnection nc an
		/// </summary>
		/// <param name="nc">NodeConnection dessen Bogenlängenposition zurückgegeben werden soll</param>
		/// <returns>aArcPosition/bArcPosition falls nc=aConnection/nc=bConnection, sonst Exception</returns>
		public double GetMyArcPosition(NodeConnection nc)
			{
			if (aConnection == nc)
				return aArcPosition;
			else if (bConnection == nc)
				return bArcPosition;
			else
				throw new Exception();
			}


		/// <summary>
		/// Gibt den ListNode des Schnittpunktes an der NodeConnection nc an
		/// </summary>
		/// <param name="nc">NodeConnection dessen ListNode zurückgegeben werden soll</param>
		/// <returns>aListNode/bListNode falls nc=aConnection/nc=bConnection, sonst Exception</returns>
		public LinkedListNode<Intersection> GetMyListNode(NodeConnection nc)
			{
			if (aConnection == nc)
				return aListNode;
			else if (bConnection == nc)
				return bListNode;
			else
				throw new Exception();
			}


		/// <summary>
		/// ToString() Override
		/// </summary>
		/// <returns>"Intersection @ aPosition.ToString()"</returns>
		public override string ToString()
			{
			return "Intersection@ " + aPosition.ToString();
			}

		#endregion

		#region vierter Versuch

		/// <summary>
		/// IntersectionHandler für diese Intersection
		/// </summary>
		private IntersectionHandler m_handler;
		/// <summary>
		/// IntersectionHandler für diese Intersection
		/// </summary>
		public IntersectionHandler handler
			{
			get { return m_handler; }
			set { m_handler = value; }
			}

		/// <summary>
		/// registriert ein Fahrzeug an dieser Intersection
		/// </summary>
		/// <param name="nc"></param>
		/// <param name="v"></param>
		public IntersectionHandler RegisterVehicle(NodeConnection nc, IVehicle v)
			{
			if (nc != aConnection && nc != bConnection)
				throw new Exception();

			if (m_handler == null)
				{
				// erstelle neuen IntersectionHandler
				m_handler = new IntersectionHandler(nc, this, v);
				}
			else
				{
				m_handler.RegisterVehicle(nc, this, v);
				}

			return m_handler;
			}

		#endregion

		#region Interferenzen zwischen Fahrzeugen

		/// <summary>
		/// wurde für diese Intersection schon alle interferierenden Fahrzeuge berechnet?
		/// </summary>
		private bool m_CalculatedInterferingVehicles = false;
		/// <summary>
		/// wurde für diese Intersection schon alle interferierenden Fahrzeuge berechnet?
		/// </summary>
		public bool calculatedInterferingVehicles
			{
			get { return m_CalculatedInterferingVehicles; }
			}

		/// <summary>
		/// Liste aller an dieser Intersection sich behindernden Fahrzeuge
		/// </summary>
		private List<InterferingVehicle> m_InterferingVehicles = new List<InterferingVehicle>();
		/// <summary>
		/// Liste aller an dieser Intersection sich behindernden Fahrzeuge
		/// </summary>
		public List<InterferingVehicle> interferingVehicles
			{
			get { return m_InterferingVehicles; }
			set { m_InterferingVehicles = value; }
			}

		/// <summary>
		/// Setzt interferingVehicles zurück
		/// </summary>
		public void ResetInterferingVehicles()
			{
			interferingVehicles.Clear();
			m_CalculatedInterferingVehicles = false;
			}


		private double CalculateArrivingTime(IVehicle v, double distance)
			{
			if (v.physics.velocity < 5)
				{
				//double timeToArrive = (v.physics.desiredVelocity * Math2.Acosh (Math.Exp(v.a * distance / Math2.Square(v.physics.desiredVelocity)))) / (10 * v.a);
				return v.GetTimeToCoverDistance(distance, false);//timeToArrive;
				}
			else
				{
				return v.GetTimeToCoverDistance(distance, true);
				}
			}

		/// <summary>
		/// prüft ob die letzten Fahrzeuge vor der Intersection interferieren und speichert das in InterferingVehicle Datensätzen
		/// </summary>
		/// <param name="timeWithin">maximale Zeit die vorausberechnet werden soll</param>
		public void CalculateInterferingVehicles(double timeWithin)
			{
			// prüfe, ob interferingVehicles schon berechnet wurde, um keine Arbeit doppelt zu machen
			if (calculatedInterferingVehicles) 
				return;

			double firstNodeIntersectionArcPosition = aArcPosition;
			double secondNodeIntersectionArcPosition = bArcPosition;

			// prüfen, ob es überhaupt auf beiden Strecken Fahrzeuge gibt, die vor der Kreuzung sind.
			LinkedListNode<IVehicle> firstVehicleNode = aConnection.vehicles.Last;
			while (firstVehicleNode != null && firstVehicleNode.Value.state.position - 2 * firstVehicleNode.Value.length > firstNodeIntersectionArcPosition)
				firstVehicleNode = firstVehicleNode.Previous;

			LinkedListNode<IVehicle> secondVehicleNode = bConnection.vehicles.Last;
			while (secondVehicleNode != null && secondVehicleNode.Value.state.position - 2 * secondVehicleNode.Value.length > secondNodeIntersectionArcPosition)
				secondVehicleNode = secondVehicleNode.Previous;

			// prüfe, ob es überhaupt interessante Autos gibt
			while (firstVehicleNode != null && secondVehicleNode != null)
				{
				// berechne die Zeit, wann die Autos die Intersection erreichen
				// Workaround: velocity + epsilon damit nicht durch 0 geteilt wird.
				double firstTimeToReachIntersection = CalculateArrivingTime(firstVehicleNode.Value, (firstNodeIntersectionArcPosition - firstVehicleNode.Value.state.position));
				double secondTimeToReachIntersection = CalculateArrivingTime(secondVehicleNode.Value, secondNodeIntersectionArcPosition - secondVehicleNode.Value.state.position);

				// Abbruchkriterium: Die Autos erreichen die Intersection später als in timeWithin Zeit
				if (firstTimeToReachIntersection > timeWithin || secondTimeToReachIntersection > timeWithin)
					{
					break;
					}

				// tausche first/secondVehicleNode falls notwendig (sollte wenn dann nur beim ersten Schleifendurchlauf passieren!)
				if (firstTimeToReachIntersection > secondTimeToReachIntersection)
					{
					LinkedListNode<IVehicle> foo = firstVehicleNode;
					firstVehicleNode = secondVehicleNode;
					secondVehicleNode = foo;

					double bar = firstTimeToReachIntersection;
					firstTimeToReachIntersection = secondTimeToReachIntersection;
					secondTimeToReachIntersection = bar;

					bar = firstNodeIntersectionArcPosition;
					firstNodeIntersectionArcPosition = secondNodeIntersectionArcPosition;
					secondNodeIntersectionArcPosition = bar;
					}

				double firstTimeToLeaveIntersection = CalculateArrivingTime(firstVehicleNode.Value, firstNodeIntersectionArcPosition - firstVehicleNode.Value.state.position + (firstVehicleNode.Value.length));

				if (true)//(secondTimeToReachIntersection - firstTimeToLeaveIntersection < 10)
					{
					// Anscheinend haben wir jetzt endlich zwei Fahrzeuge gefunden, die wir vergleichen wollen
					// also berechnen wir mal die notwendige Verzögerung vom zweiten Fahrzeug an der Intersection:
					double forcedAccelerationOfSecondVehicle = secondVehicleNode.Value.CalculateAcceleration(
						secondVehicleNode.Value.physics.velocity,
						secondVehicleNode.Value.physics.effectiveDesiredVelocity,
						(secondNodeIntersectionArcPosition - secondVehicleNode.Value.currentPosition) - 0.5*(secondVehicleNode.Value.physics.velocity * firstTimeToLeaveIntersection),
						secondVehicleNode.Value.physics.velocity);// - firstVehicleNode.Value.physics.velocity);

					// lade bzw. speichere die originalArrivingTimes im Dictionary
					double firstOriginalArrivingTime = 0, secondOriginalArrivingTime = 0;
					firstVehicleNode.Value.originalArrivingTimes.TryGetValue(this, out firstOriginalArrivingTime);
					secondVehicleNode.Value.originalArrivingTimes.TryGetValue(this, out secondOriginalArrivingTime);

					// Nun können wir einen InterferingVehicles Datensatz anlegen
					interferingVehicles.Add(
						new InterferingVehicle(
							secondVehicleNode.Value, 
							firstVehicleNode.Value, 
							this, 
							forcedAccelerationOfSecondVehicle, 
							firstTimeToReachIntersection, 
							firstTimeToLeaveIntersection, 
							secondTimeToReachIntersection,
							firstOriginalArrivingTime,
							secondOriginalArrivingTime
							));
					}

				// Diese beiden Autos wurden abschließend untersucht, machen wir mit den nächsten weiter

				// prüfe ob sich zwischen first- und secondVehicleNode noch ein weiteres Fahrzeug befindet
				if (firstVehicleNode.Previous != null
					&& (CalculateArrivingTime(firstVehicleNode.Previous.Value, (firstNodeIntersectionArcPosition - firstVehicleNode.Previous.Value.state.position)) < secondTimeToReachIntersection))
					{
					// dann setze firstVehicleNode einen weiter und starte die Schleife von vorn
					firstVehicleNode = firstVehicleNode.Previous;
					}
				// ansonsten ist das secondVehicle das neue First, und das neue second das Fahrzeug hinter dem aktuellen firstVehicle
				else
					{
					LinkedListNode<IVehicle> temp = firstVehicleNode;
					firstVehicleNode = secondVehicleNode;
					secondVehicleNode = temp.Previous;
					}
				}

			m_CalculatedInterferingVehicles = true;
			}


		#endregion


		}


	/// <summary>
	/// kapselt eine nodeConnection und eine zugehörige Intersection
	/// </summary>
	public struct SpecificIntersection
		{
		/// <summary>
		/// NodeConnection
		/// </summary>
		public NodeConnection nodeConnection;

		/// <summary>
		/// Intersection, die an nodeConnection liegt
		/// </summary>
		public Intersection intersection;

		/// <summary>
		/// Standardkonstruktor
		/// </summary>
		/// <param name="nc">NodeConnection</param>
		/// <param name="i">Intersection, die an nc liegt</param>
		public SpecificIntersection(NodeConnection nc, Intersection i)
			{
			nodeConnection = nc;
			intersection = i;
			}

		/// <summary>
		/// Überprüft zwei SpecificIntersections auf Gleichheit
		/// </summary>
		/// <param name="a">erste zu untersuchente SpecificIntersection</param>
		/// <param name="b">zweite zu untersuchente SpecificIntersection</param>
		/// <returns>(a.intersection == b.intersection AND a.nodeConnection == b.nodeConnection)</returns>
		public static bool Equals(SpecificIntersection a, SpecificIntersection b)
			{
			return (a.intersection == b.intersection && a.nodeConnection == b.nodeConnection);
			}
		}

	}
