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
using System.Xml;
using System.Xml.Serialization;

namespace CityTrafficSimulator.Vehicle
    {
    /// <summary>
    /// Interface für Objekte, welche sich auf den Straßen bewegen können 
    /// (es werden wohl zunächst nur LKWs und Autos davon abgelitten
    /// </summary>
    [Serializable]
    public abstract class IVehicle : IDM, IDrawable
		{
		/// <summary>
		/// Enum, aller implementierten IVehicles
		/// </summary>
		[Serializable]
		public enum VehicleTypes
			{
			/// <summary>
			/// Auto
			/// </summary>
			CAR, 
			/// <summary>
			/// Bus
			/// </summary>
			BUS, 
			/// <summary>
			/// Straßenbahn
			/// </summary>
			TRAM
			};


		#region Statistiken

		/// <summary>
		/// absolute Startzeit des Fahrzeuges
		/// </summary>
		protected float m_startTime = 0;
		/// <summary>
		/// absolute Startzeit des Fahrzeuges
		/// </summary>
		public float startTime
			{
			get { return m_startTime; }
			}


		/// <summary>
		/// absolute Startzeit des Fahrzeuges auf der aktuellen NodeConnectino
		/// </summary>
		protected float m_startTimeOnNodeConnection;
		/// <summary>
		/// absolute Startzeit des Fahrzeuges auf der aktuellen NodeConnectino
		/// </summary>
		public float startTimeOnNodeConnection
			{
			get { return m_startTimeOnNodeConnection; }
			}


		/// <summary>
		/// Bogenlängenposition, bei der dieses Auto auf dieser Linie gestartet ist
		/// </summary>
		protected double m_arcPositionOfStartOnNodeConnection;
		/// <summary>
		/// Bogenlängenposition, bei der dieses Auto auf dieser Linie gestartet ist
		/// </summary>
		public double arcPositionOfStartOnNodeConnection
			{
			get { return m_arcPositionOfStartOnNodeConnection; }
			}



		#endregion

		#region Hashcodes von Vehicles
		/*
		 * Wir benötigen für Fahrzeuge Hashcodes zur schnellen quasi-eindeutigen Identifizierung. Da sich der Zustand eines Fahrzeuges quasi
		 * ständig ändert, gibt es leider keine zuverlässigen Felder die zur Hashwertberechnung dienen können.
		 * 
		 * Also bedienen wir uns einen alten, nicht umbedingt hübschen, aber bewährten Tricks:
		 * IVehicle verwaltet eine statische Klassenvariable hashcodeIndex, die mit jeder Instanziierung eines Vehicles inkrementiert wird und 
		 * als eindeutiger Hashcode für das Fahrzeug dient. Es muss insbesondere sichergestellt werden, dass bei jeder abgeleiteten Klasse entweder
		 * der Elternkonstruktor aufgerufen wird, oder sich die abgeleitete Klasse selbst um einen gültigen Hashcode kümmert.
		 */
		
		/// <summary>
		/// Klassenvariable welche den letzten vergebenen hashcode speichert und bei jeder Instanziierung eines Objektes inkrementiert werden muss
		/// </summary>
		private static int hashcodeIndex = 0;

		/// <summary>
		/// Hashcode des instanziierten Objektes
		/// </summary>
		private int hashcode = -1;

		/// <summary>
		/// gibt den Hashcode des Fahrzeuges zurück.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
			{
			return hashcode;
			}

		/// <summary>
		/// Standardkonstruktor, der nichts tut als den Hashcode zu setzen.
		/// </summary>
		public IVehicle()
			{
			hashcode = hashcodeIndex++;
			}

		#endregion

		#region Selbstzerstörung

		/// <summary>
		/// Bereitet das IVehicle auf die Selbstzerstörung vor und meldet sich an allen beteiligten NodeConnections ab
		/// </summary>
		/// <param name="averageSpeed">Durchschnittsgeschwindigkeit in m/s, die dieses Fahrzeug auf der NodeConnection hatte</param>
		private void Dispose(float averageSpeed)
			{
			currentNodeConnection.RemoveVehicle(this, averageSpeed);
			}

        #endregion


        #region Zustand

		#region Spurwechsel

		/// <summary>
		/// IVehicle ist gerade am Spurwechseln
		/// </summary>
		private	bool currentlyChangingLine = false;

		/// <summary>
		/// gibt an, welchen Spurwechsel das IVehicle gerade durchführt
		/// </summary>
		private NodeConnection.LineChangePoint currentLineChangePoint;

		/// <summary>
		/// aktuelle Position (und damit Fortschritt) des Spurwechsels
		/// </summary>
		private double currentPositionOnLineChangePoint = 0;

		/// <summary>
		/// Verhältnis von der Projektion der LCP auf die Ziel-NodeConnection zu der Länge des LCP
		/// </summary>
		private double ratioProjectionOnTargetConnectionvsLCPLength = 0;


		/// <summary>
		/// Initiiert einen Spurwechsel
		/// </summary>
		/// <param name="lcp">LCP auf dem der Spurwechsel durchgeführt werden soll</param>
		/// <param name="arcPositionOffset">bereits auf dem LCP zurückgelegte Strecke</param>
		private void InitiateLineChange(NodeConnection.LineChangePoint lcp, double arcPositionOffset)
			{
			// einem evtl. ausgebremsten Fahrzeug sagen, dass es nicht mehr extra für mich abbremsen braucht
			if (vehicleWhichSlowsDownForMe != null)
				{
				vehicleWhichSlowsDownForMe.slowDownForChangingVehicle = false;
				vehicleWhichSlowsDownForMe = null;
				}

			// sich merken, dass das IVehicle gerade am Spurwechseln ist
			currentlyChangingLine = true;
			currentLineChangePoint = lcp;
			currentPositionOnLineChangePoint = arcPositionOffset;
			if (lcp.target.nc == lcp.otherStart.nc)
				{
				ratioProjectionOnTargetConnectionvsLCPLength = (lcp.target.arcPosition - lcp.otherStart.arcPosition) / lcp.lineSegment.length;
				}
			else
				{
				ratioProjectionOnTargetConnectionvsLCPLength = (lcp.target.arcPosition + lcp.otherStart.nc.lineSegment.length - lcp.otherStart.arcPosition) / lcp.lineSegment.length;
				}

			// Neuen State schonmal auf die ZielNodeConnection setzen (currentNodeConnection, currentPosition)
			m_State = new State(lcp.otherStart.nc, lcp.otherStart.arcPosition + arcPositionOffset*ratioProjectionOnTargetConnectionvsLCPLength);
			lcp.otherStart.nc.AddVehicleAt(this, lcp.otherStart.arcPosition);
			m_WayToGo = CalculateShortestConenction(currentNodeConnection.endNode, targetNodes);

			lineChangeNeeded = false;
			lci = null;
			}

		/// <summary>
		/// schließt den Spurwechsel ab
		/// </summary>
		/// <param name="arcPositionOffset">Bogenlänge über die das Fahrzeug bereits über die Länge des LCP hinaus ist</param>
		private void FinishLineChange(double arcPositionOffset)
			{
			m_State.position = currentLineChangePoint.target.arcPosition + arcPositionOffset;
			m_WayToGo = CalculateShortestConenction(currentNodeConnection.endNode, m_TargetNodes);

			currentlyChangingLine = false;

			lineChangeNeeded = false;
			lci = null;
			m_Physics.multiplierDesiredVelocity = 1;
			}


		#endregion


		/// <summary>
		/// DEPRECATED: Verhalten dieses Fahrzeuges - war mal so ne fixe Idee zur Implementierung der Verkehrslogik, wurde dann aber irgendwann nicht weiterverfolgt.
		/// </summary>
		public IVehicleBehaviour behaviour = new FreeDriveVehicleBehaviour();


		/// <summary>
		/// Alle IntersectionHandlers, bei dem dieses Fahrzeug sich registriert hat
		/// </summary>
		private List<IntersectionHandler> m_registeredIntersectionHandlers = new List<IntersectionHandler>();
		/// <summary>
		/// Alle IntersectionHandlers, bei dem dieses Fahrzeug sich registriert hat
		/// </summary>
		public List<IntersectionHandler> registeredIntersectionHandlers
			{
			get { return m_registeredIntersectionHandlers; }
			set { m_registeredIntersectionHandlers = value; }
			}


		/// <summary>
		/// wurde das Fahrzeug bei diesem Tick schon bewegt?
		/// </summary>
		private bool alreadyMoved = false;

		private StringBuilder debugData = new StringBuilder();

		/// <summary>
		/// Länge des Fahrzeuges
		/// </summary>
		protected double m_Length = 40;
		/// <summary>
		/// Länge des Fahrzeuges
		/// </summary>
		public double length
			{
			get { return m_Length; }
			set { m_Length = value; }
			}

		/// <summary>
		/// Farbe des Autos
		/// </summary>
		protected Color m_Color = Color.Black;
		/// <summary>
		/// Farbe des Autos
		/// </summary>
		public Color color
			{
			get { return m_Color; }
			}


		/// <summary>
		/// Physik des Fahrzeuges
		/// </summary>
		protected IVehicle.Physics m_Physics;
		/// <summary>
		/// Physik des Fahrzeuges
		/// </summary>
        public IVehicle.Physics physics
            {
            get { return m_Physics; }
            }

		/// <summary>
		/// aktueller State des Fahrezeuges
		/// </summary>
        protected IVehicle.State m_State;
		/// <summary>
		/// aktueller State des Fahrezeuges
		/// </summary>
        public IVehicle.State state
            {
            get { return m_State; }
			set { m_State = value; }
            }

		/// <summary>
		/// aktuelle Bogenlängenposition auf der aktuellen NodeConnection
		/// </summary>
        public double currentPosition
            {
            get { return state.position; }
            }

		/// <summary>
		/// aktuelle NodeConnection
		/// </summary>
        public NodeConnection currentNodeConnection
            {
            get { return state.currentNodeConnection; }
            }

		/// <summary>
		/// Flag, ob auf der aktuellen NodeConnectino ein Spurwechsel nötig ist
		/// </summary>
		private bool lineChangeNeeded = false;

		/// <summary>
		/// zu benutzendes LineChangeInterval
		/// </summary>
		private NodeConnection.LineChangeInterval lci;

		/// <summary>
		/// verbleibende bogenlängenentfernung bis zum Ende der NodeConnection
		/// </summary>
		/// <param name="currentNC">NodeConnection für die die Beschleunigungswerte berechnet werden sollen</param>
		/// <param name="arcPos">Position auf currentNC für die die Beschleunigungswerte berechnet werden sollen</param>
		/// <returns>currentNC.lineSegment.length - arcPos</returns>
		public double GetDistanceToEndOfLineSegment(NodeConnection currentNC, double arcPos)
            {
            return currentNC.lineSegment.length - arcPos;
            }

		/// <summary>
		/// alle bisher besuchten NodeConnections
		/// </summary>
		protected LinkedList<NodeConnection> visitedNodeConnections = new LinkedList<NodeConnection>();


		/// <summary>
		/// Fahrzeug berechnet neue Beschleunigungswerte für die aktuelle Position auf der auktuellen NodeConnection 
		/// </summary>
		/// <param name="tickLength">Länge eines Ticks in Sekunden</param>
		/// <param name="currentTime">aktuelle Zeit in Sekunden nach Sekunde 0</param>
		public void Think(double tickLength, double currentTime)
			{
			Think(currentNodeConnection, currentPosition, false, tickLength, currentTime);
			}


		/// <summary>
		/// Fahrzeug berechnet neue Beschleunigungswerte
		/// </summary>
		/// <param name="currentNC">NodeConnection für die die Beschleunigungswerte berechnet werden sollen</param>
		/// <param name="arcPos">Position auf currentNC für die die Beschleunigungswerte berechnet werden sollen</param>
		/// <param name="onlySimpleCalculations">Flag, ob nur einfache Berechnungen (Vordermann, LSA, Intersections) oder komplette Berechnungen inkl. Spurwechsel etc. gemacht werden sollen</param>
		/// <param name="tickLength">Länge eines Ticks in Sekunden</param>
		/// <param name="currentTime">aktuelle Zeit in Sekunden nach Sekunde 0</param>
		/// <return>berechnete Beschleunigung</return>
		public double Think(NodeConnection currentNC, double arcPos, bool onlySimpleCalculations, double tickLength, double currentTime)
			{
			behaviour = new FreeDriveVehicleBehaviour();

			debugData.Remove(0, debugData.Length);

			double lookaheadDistance = Constants.lookaheadDistance;
			double lookaheadTime = Constants.lookaheadTime;
			double distanceToLookForTrafficLights;

			// bestimme das Auto welches vor mir fährt
			VehicleDistance theVehicleInFrontOfMe = GetNextVehicleOnMyTrack(currentNC, arcPos, lookaheadDistance);

			// bestimme alle Intersections zwischen mir und dem nächsten Auto
			List<SpecificIntersection> interestingIntersections;
			if (theVehicleInFrontOfMe != null)
				{
				//behaviour = new FollowingVehicleBehaviour(theVehicleInFrontOfMe);
				interestingIntersections = GetNextIntersectionsOnMyTrack(currentNC, arcPos, lookaheadDistance, currentTime);
				distanceToLookForTrafficLights = theVehicleInFrontOfMe.distance;
				debugData.Append("#NextVehicle: " + theVehicleInFrontOfMe.vehicle.hashcode + "@" + theVehicleInFrontOfMe.distance.ToString("###.#") + "dm\n");
				//debugData.Append("#lookwithin: " + theVehicleInFrontOfMe.distance + "\n");
				}
			else
				{
				interestingIntersections = GetNextIntersectionsOnMyTrack(currentNC, arcPos, lookaheadDistance, currentTime);
				distanceToLookForTrafficLights = lookaheadDistance;
				//debugData.Append("#lookwithin: " + lookaheadDistance + "\n");
				}


			List<InterferingVehicle> jammedVehicles = new List<InterferingVehicle>(); // alle durch mich evtl. ausgebremsten Fahrzeuge
			List<InterferingVehicle> jammingVehicles = new List<InterferingVehicle>(); // alle mich evtl. ausbremsenden Fahrzeuge

			// bestimme alle Autos die ich ausbremsen könnte oder die mich ausbremsen könnten
			foreach (SpecificIntersection si in interestingIntersections)
				{
				si.intersection.CalculateInterferingVehicles(lookaheadTime);
				foreach (InterferingVehicle iv in si.intersection.interferingVehicles)
					{
					if (iv.jammedVehicle == this)
						{
						jammingVehicles.Add(iv);
						}
					else if (iv.jammingVehicle == this)
						{
						jammedVehicles.Add(iv);
						}
					}
				}

			// Alle wichtigen Daten gesammelt, dann berechnen wir mal die Beschleunigung:

			// Idee 1: berechne alle möglichen Beschleunigungswerte und wähle Minimum (dann sollte zumindest wenig schief gehen können)
			bool thinkAboutLineChange = false;
			wunschabstand = 100;
			double lowestAcceleration = 0;

			m_Physics.multiplierDesiredVelocity = (slowDownForChangingVehicle) ? (1 - p)*(1 - p) : 1;

			if (theVehicleInFrontOfMe != null)
				{
				lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, theVehicleInFrontOfMe.distance, physics.velocity - theVehicleInFrontOfMe.vehicle.physics.velocity);
				wunschabstand = CalculateWantedDistance(physics.velocity, physics.velocity - theVehicleInFrontOfMe.vehicle.physics.velocity);
				thinkAboutLineChange = true;
				}
			else
				lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lookaheadDistance, physics.velocity);


			#region Intersectionhandling

			// TODO:
			// Algorithmus läuft gut, einzig die Berücksichtigung der An-/Abkunftszeit der Fahrzeuge an der Intersection liefert jedoch keine Ergebnisse
			// die die Realität wirklich widerspiegeln: Es werden eindeutig die Autos bevorzugt, die schneller fahren, statt die die früher an der Kreuzung
			// standen. Genauer: Ein Fahrzeug was einmal zum stehen gekommen ist, wartet bis das ganze Pulk der kreuzenden Fahrzeuge vorbei ist.
			//
			// Idee: speichere in InterferingVehicle die ursprüngliche Ankuftszeit des Fahrzeuges an der Intersection und berücksichtige diese bei der 
			// Vorfahrtsberechnung
			//
			foreach (InterferingVehicle iv in jammedVehicles)
				{
				debugData.Append("jamming veh.: " + iv.jammedVehicle.GetHashCode().ToString() + "\n");
				//debugData.Append(iv.timeOfJammingVehicleToReachIntersection);
				//debugData.Append(", ");
				//debugData.Append(iv.timeOfJammingVehicleToLeaveIntersection);
				debugData.Append(iv.originalTimeOfJammingVehicleToReachIntersecion.ToString("###.#"));
				debugData.Append("\n");
				

				// beide NodeConnections haben die gleiche Priorität
				if (currentNC.priority == iv.jammedVehicle.currentNodeConnection.priority)
					{
					// gucken, ob ich den anderen behindere
					// das andere Auto wäre eigentlich >10s vor mir dagewesen
					if ((iv.originalTimeOfJammingVehicleToReachIntersecion - iv.originalTimeOfJammedVehicleToReachIntersecion >= 2)) 
						{
						double newWunschabstand = CalculateWantedDistance(physics.velocity, physics.velocity);

						double newAcceleration = CalculateAcceleration(
							physics.velocity,
							physics.effectiveDesiredVelocity,
							((currentNC == iv.intersection.aConnection) ? iv.intersection.aArcPosition : iv.intersection.bArcPosition) - arcPos - iv.jammingVehicle.length - iv.intersection.GetWaitingDistance(),
							newWunschabstand,
							physics.velocity);

						// Minimum speichern
						if (newAcceleration < lowestAcceleration)
							{
							lowestAcceleration = newAcceleration;
							thinkAboutLineChange = false;
							}

						// Minimum speichern
						if (newWunschabstand < wunschabstand)
							wunschabstand = newWunschabstand;
						}
					}
				// meine NodeConnection hat die höhere Priorität
				else if (currentNC.priority > iv.jammedVehicle.currentNodeConnection.priority)
					{
					// ich bin vorne und zudem auch noch wichtiger - also nichts zu tun?!
					}
				// meine NodeConnection hat die geringere Priorität
				else
					{
					// gucken, wie stark ich behindere 
					// 10 Zeiteinheiten Sicherheitsabstand zwischen dem der mich behindert und mir
					if (iv.timeOfJammedVehicleToReachIntersection - iv.timeOfJammingVehicleToLeaveIntersection < 8)
						{
						double newWunschabstand = CalculateWantedDistance(physics.velocity, physics.velocity);

						double newAcceleration = CalculateAcceleration(
							physics.velocity,
							physics.effectiveDesiredVelocity,
							((currentNC == iv.intersection.aConnection) ? iv.intersection.aArcPosition : iv.intersection.bArcPosition) - arcPos - iv.jammingVehicle.length - iv.intersection.GetWaitingDistance(),
							newWunschabstand,
							physics.velocity);

						// Minimum speichern
						if (newAcceleration < lowestAcceleration)
							{
							lowestAcceleration = newAcceleration;
							thinkAboutLineChange = false;
							}

						// Minimum speichern
						if (newWunschabstand < wunschabstand)
							wunschabstand = newWunschabstand;
						}
					}
				}

			foreach (InterferingVehicle iv in jammingVehicles)
				{
				debugData.Append("jammed by: " + iv.jammingVehicle.GetHashCode().ToString() + "\n");
				//debugData.Append(iv.timeOfJammedVehicleToReachIntersection);
				//debugData.Append(", ");
				//debugData.Append(iv.timeOfJammingVehicleToLeaveIntersection);
				debugData.Append(iv.originalTimeOfJammedVehicleToReachIntersecion.ToString("###.#"));
				debugData.Append("\n");

				// beide NodeConnections haben die gleiche Priorität
				if (currentNC.priority == iv.jammingVehicle.currentNodeConnection.priority)
					{
					// gucken, wie stark ich behindert werde
					if ((iv.timeOfJammedVehicleToReachIntersection - iv.timeOfJammingVehicleToLeaveIntersection < 7) // entweder weniger als 10 Zeiteinheiten Sicherheitsabstand 
						&& (iv.originalTimeOfJammingVehicleToReachIntersecion - iv.originalTimeOfJammedVehicleToReachIntersecion <= 2)) // oder das andere Auto wäre eigentlich >10s vor mir dagewesen
						{
						double newWunschabstand = CalculateWantedDistance(physics.velocity, physics.velocity);

						double myDistanceToIntersection = ((currentNC == iv.intersection.aConnection) ? iv.intersection.aArcPosition : iv.intersection.bArcPosition) - arcPos - iv.jammingVehicle.length - iv.intersection.GetWaitingDistance();

						double newAcceleration = CalculateAcceleration(
							physics.velocity, 
							physics.effectiveDesiredVelocity,
							myDistanceToIntersection,
							newWunschabstand, 
							physics.velocity);

						// Minimum speichern
						if (newAcceleration < lowestAcceleration)
							{
							lowestAcceleration = newAcceleration;
							thinkAboutLineChange = false;
							}

						// Minimum speichern
						if (newWunschabstand < wunschabstand)
							wunschabstand = newWunschabstand;
						}
					}
				// meine NodeConnection hat die höhere Priorität
				else if (currentNC.priority > iv.jammingVehicle.currentNodeConnection.priority)
					{
					// TODO: Problem: ich bin zwar eigentlich wichtiger als der Arsch der mich ausbremst, aber ich will ja auch keinen Unfall bauen...
					//       also was ist zu tun?

					// Also, wenn mir tatsächlich jemand vor die Nase fährt werd ich wohl ne Notbremsung machen:
					// (vielleicht erkenn ich das dass ich stärker als b_save bremsen muss)
					
					if ((iv.timeOfJammingVehicleToReachIntersection < 3) 
						&& (iv.timeOfJammedVehicleToReachIntersection - iv.timeOfJammingVehicleToLeaveIntersection < 3))
						{
						/*double newWunschabstand = CalculateWantedDistance(physics.velocity, physics.velocity);

						double newAcceleration = CalculateAcceleration(
							physics.velocity,
							physics.desiredVelocity,
							((currentNC == iv.intersection.aConnection) ? iv.intersection.aArcPosition : iv.intersection.bArcPosition) - arcPos - iv.jammingVehicle.length,
							newWunschabstand,
							physics.velocity);

						// Minimum speichern
						if (newAcceleration < lowestAcceleration)
							lowestAcceleration = newAcceleration;

						// Minimum speichern
						if (newWunschabstand < wunschabstand)
							wunschabstand = newWunschabstand;*/
						}
					}
				// meine NodeConnection hat die geringere Priorität
				else
					{
					// gucken, wie stark ich behindert werde
					// 10 Zeiteinheiten Sicherheitsabstand zwischen dem der mich behindert und mir
					if (iv.timeOfJammedVehicleToReachIntersection - iv.timeOfJammingVehicleToLeaveIntersection < 8)
						{
						double newWunschabstand = CalculateWantedDistance(physics.velocity, physics.velocity);

						double newAcceleration = CalculateAcceleration(
							physics.velocity,
							physics.effectiveDesiredVelocity,
							((currentNC == iv.intersection.aConnection) ? iv.intersection.aArcPosition : iv.intersection.bArcPosition) - arcPos - iv.jammingVehicle.length - iv.intersection.GetWaitingDistance(),
							newWunschabstand,
							physics.velocity);

						// Minimum speichern
						if (newAcceleration < lowestAcceleration)
							{
							lowestAcceleration = newAcceleration;
							thinkAboutLineChange = false;
							}

						// Minimum speichern
						if (newWunschabstand < wunschabstand)
							wunschabstand = newWunschabstand;
						}
					}
				}

			#endregion

			#region LSA

			// Nun noch gucken, ob es evtl. Ampeln gibt, die ich beachten muss:
			// TODO:	auch folgende NCs nach Ampeln durchsuchen!
			if (distanceToLookForTrafficLights > GetDistanceToEndOfLineSegment(currentNC, arcPos)
				&& currentNC.endNode.tLight != null
				&& currentNC.endNode.tLight.trafficLightState == TrafficLight.State.RED)
				{
				double newWunschabstand = CalculateWantedDistance(physics.velocity, physics.velocity);

				double newAcceleration = CalculateAcceleration(
					physics.velocity,
					physics.effectiveDesiredVelocity,
					GetDistanceToEndOfLineSegment(currentNC, arcPos),
					newWunschabstand,
					physics.velocity);

				// Minimum speichern
				if (newAcceleration < lowestAcceleration)
					{
					lowestAcceleration = newAcceleration;
					thinkAboutLineChange = false;
					}

				// Minimum speichern
				if (newWunschabstand < wunschabstand)
					wunschabstand = newWunschabstand;

				// abmelden von allen LSAs
				originalArrivingTimes.Clear();				
				}

			#endregion

			#region Spurwechsel

			if (!onlySimpleCalculations)
				{

				#region zwangsweise

				// unsere Route gibt uns vor hier einen Spurwechsel zu machen
				if (lineChangeNeeded)
					{
					thinkAboutLineChange = false;
					if (!currentlyChangingLine)
						{
						// nächsten LineChangePoint finden, der uns zum nächsten Zielknoten führt
						NodeConnection.LineChangePoint lcp = currentNC.GetPrevLineChangePoint(arcPos);
						if (lci.targetNode.prevConnections.Contains(lcp.target.nc))
							{
							if (Math.Abs(arcPos - lcp.start.arcPosition) < Constants.maxDistanceToLineChangePoint / 2)
								{
								double myArcPositionOnOtherConnection = lcp.otherStart.arcPosition + (arcPos - lcp.start.arcPosition);// -(lcp.myArcPosition - arcPos);
								if (myArcPositionOnOtherConnection >= 0)
									{
									Pair<VehicleDistance> otherVehicles = lcp.otherStart.nc.GetVehiclesAroundArcPosition(myArcPositionOnOtherConnection, Constants.lookaheadDistance);

									// neuer Vordermann wäre nicht zu nah dran
									if (otherVehicles.Right == null || otherVehicles.Right.distance > otherVehicles.Right.vehicle.length * 1.5)// + lcp.length)
										{
										// neuer Hintermann wäre nicht zu nah dran
										if (otherVehicles.Left == null || otherVehicles.Left.distance > m_Length * 1.5)
											{
											// meine theoretische Beschleunigung auf der anderen Spur berechnen:
											double myAccelerationOnOtherConnection = (otherVehicles.Right != null)
												? CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, otherVehicles.Right.vehicle.currentPosition - myArcPositionOnOtherConnection, physics.velocity - otherVehicles.Right.vehicle.physics.velocity)
												: CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lookaheadDistance, physics.velocity);

											// die Beschleunigung meines neuen Hintermannes berechnen:
											double forcedAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null)
												? CalculateAcceleration(otherVehicles.Left.vehicle.physics.velocity, otherVehicles.Left.vehicle.physics.effectiveDesiredVelocity, otherVehicles.Left.distance, otherVehicles.Left.vehicle.physics.velocity - physics.velocity)
												: 0;

											double currentAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null) ? otherVehicles.Left.vehicle.physics.acceleration : 0;

											// eigenes Spurwechselmodell:
											// der neue Hintermann darf nicht mehr als bSave ausgebremst werden
											// und und mein Spurwechsel muss hinreichend dringend werden. Je näher ich ans Ende des LineChangeIntervalls komme, desto mehr darf ich den Hintermann ausbremsen
											if ((forcedAccelerationOfVehicleBehindMeOnOtherConnection > bSave)
												&& (p * (arcPos - lci.startArcPos) / lci.length >= p * (currentAccelerationOfVehicleBehindMeOnOtherConnection - forcedAccelerationOfVehicleBehindMeOnOtherConnection)))
												{
												// wieder auf normale Wunschegschwindigkeit gehen
												m_Physics.multiplierDesiredVelocity = 1;

												// erstmal mich selbst zerstören
												Dispose((float)((arcPos - m_arcPositionOfStartOnNodeConnection) / (10.0f * (currentTime - m_startTimeOnNodeConnection))));

												// ich darf einen Linienwechsel durchführen, jetzt wirds spannend:
												InitiateLineChange(lcp, arcPos - lcp.start.arcPosition);

												lowestAcceleration = myAccelerationOnOtherConnection;
												}
											}
										// wenn ich jetzt nen Spurwechsel machen würde, dann würde ich meinen neuen Hintermann ausbremsen, aber ich kann beschleunigen und fahre schon mindestens genauso schnell wie er
										else if ((otherVehicles.Left.vehicle.physics.velocity / this.physics.velocity < 1.1) && (lowestAcceleration >= -0.1))
											{
											// also beschleunige ich etwas, damit ich mich vor ihn setze demnächst den Spurwechsel machen kann
											m_Physics.multiplierDesiredVelocity = 1.5;
											lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lookaheadDistance, physics.velocity);

											// ich sage dem anderen Auto, dass ich die Spur wechseln möchte, vielleicht bremst es für mich,
											// vorher sollte ich mich aber noch bei dem, den ich evtl. vorher ausgebremst habe abmelden:
											if (vehicleWhichSlowsDownForMe != null)
												vehicleWhichSlowsDownForMe.slowDownForChangingVehicle = false;

											otherVehicles.Left.vehicle.slowDownForChangingVehicle = true;
											vehicleWhichSlowsDownForMe = otherVehicles.Left.vehicle;
											}
										// auch das würde nichts helfen, also bremse ich etwas ab
										else
											{
											// ich bremse vorsichtshalber schonmal etwas ab
											m_Physics.multiplierDesiredVelocity = 0.75;

											// ich sage dem anderen Auto, dass ich die Spur wechseln möchte, vielleicht bremst es für mich,
											// vorher sollte ich mich aber noch bei dem, den ich evtl. vorher ausgebremst habe abmelden:
											if (vehicleWhichSlowsDownForMe != null)
												vehicleWhichSlowsDownForMe.slowDownForChangingVehicle = false;

											otherVehicles.Left.vehicle.slowDownForChangingVehicle = true;
											vehicleWhichSlowsDownForMe = otherVehicles.Left.vehicle;

											// prüfen, wieviel Strecke es noch gibt, auf der ich die Spur wechseln kann
											if (lci.endArcPos - arcPos < Constants.minimumLineChangeLength)
												{
												// die aktuelle Spurwechselmöglichkeit ist fast zu Ende, prüfen, ob es schlimm ist, wenn ich nicht jetzt sofort wechsle
												RouteToTarget newRTT = CalculateShortestConenction(currentNC.endNode, m_TargetNodes);
												// ich kann auch später die Spur wechseln und komme trotzdem ans Ziel (ohne allzu große Umwege zu fahren)
												if (newRTT.SegmentCount() > 0 && newRTT.costs / m_WayToGo.costs < Constants.maxRatioForEnforcedLineChange)
													{													
													m_WayToGo = newRTT;
													m_Physics.multiplierDesiredVelocity = 1;
													lineChangeNeeded = false;
													lci = null;
													}
												// nein, ich muss jetzt gleich hier die Spur wechseln, also halte zur Not an bis mich jemand reinlässt
												else
													{
													lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lci.endArcPos - arcPos, physics.velocity);
													}
												}
											// die aktuelle Spruchwechslmöglichkeit ist noch lang genug
											else
												{
												lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lci.endArcPos - arcPos, physics.velocity);
												}
											}
										}
									// auch das würde nichts helfen, also bremse ich etwas ab
									else
										{
										m_Physics.multiplierDesiredVelocity = 0.75;
										if (lci.endArcPos - arcPos < Constants.minimumLineChangeLength)
											{
											// die aktuelle Spurwechselmöglichkeit ist fast zu Ende, prüfen, ob es schlimm ist, wenn ich nicht jetzt sofort wechsle
											RouteToTarget newRTT = CalculateShortestConenction(currentNC.endNode, m_TargetNodes);
											// ich kann auch später die Spur wechseln und komme trotzdem ans Ziel (ohne allzu große Umwege zu fahren)
											if (newRTT.SegmentCount() > 0 && newRTT.costs/ m_WayToGo.costs < Constants.maxRatioForEnforcedLineChange)
												{
												m_WayToGo = newRTT;
												m_Physics.multiplierDesiredVelocity = 1;
												lineChangeNeeded = false;
												lci = null;
												}
											else
												{
												lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lci.endArcPos - arcPos, physics.velocity);
												}
											}
										else
											{
											lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lci.endArcPos - arcPos, physics.velocity);
											}
										}
									}
								}
							}
						}
					}

				#endregion

				#region freiwillig
				// Über Spurwechsel nachdenken
				if (thinkAboutLineChange && !currentlyChangingLine)
					{
					// nächsten LineChangePoint finden
					NodeConnection.LineChangePoint lcp = currentNC.GetPrevLineChangePoint(arcPos);
					if (lcp.target.nc != null)
						{
						if (Math.Abs(arcPos - lcp.start.arcPosition) < Constants.maxDistanceToLineChangePoint / 2)
							{
							// prüfen, ob ich auf der anderen NodeConnection überhaupt ans Ziel komme und das in einem angemessenen Zeitraum:
							RouteToTarget alternativeRoute = CalculateShortestConenction(lcp.target.nc.endNode, targetNodes);

							if (alternativeRoute.SegmentCount() > 0 && alternativeRoute.costs / WayToGo.costs < Constants.maxRatioForVoluntaryLineChange && !alternativeRoute.Top().lineChangeNeeded)
								{
								double myArcPositionOnOtherConnection = lcp.otherStart.arcPosition + (arcPos - lcp.start.arcPosition);// -(lcp.myArcPosition - arcPos);
								if (myArcPositionOnOtherConnection >= 0)
									{
									Pair<VehicleDistance> otherVehicles = lcp.otherStart.nc.GetVehiclesAroundArcPosition(myArcPositionOnOtherConnection, Constants.lookaheadDistance);

									// neuer Vordermann wäre nicht zu nah dran
									if (otherVehicles.Right == null || otherVehicles.Right.distance > otherVehicles.Right.vehicle.length * 2 + lcp.length)
										{
										// neuer Hintermann wäre nicht zu nah dran
										if (otherVehicles.Left == null || otherVehicles.Left.distance > m_Length * 2)
											{
											//double myAccelerationOnOtherConnection = Think(lcp.otherStart.nc, myArcPositionOnOtherConnection, true, tickLength, currentTime);
											// meine theoretische Beschleunigung auf der anderen Spur berechnen:
											double myAccelerationOnOtherConnection = (otherVehicles.Right != null)
												? CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, otherVehicles.Right.vehicle.currentPosition - myArcPositionOnOtherConnection, physics.velocity - otherVehicles.Right.vehicle.physics.velocity)
												: CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lookaheadDistance, physics.velocity);

											// die Beschleunigung meines neuen Hintermannes berechnen:
											double forcedAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null)
												? CalculateAcceleration(otherVehicles.Left.vehicle.physics.velocity, otherVehicles.Left.vehicle.physics.effectiveDesiredVelocity, otherVehicles.Left.distance, otherVehicles.Left.vehicle.physics.velocity - physics.velocity)
												: 0;

											double currentAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null) ? otherVehicles.Left.vehicle.physics.acceleration : 0;

											// vereinfachte Implementierung von MOBIL: http://www.vwi.tu-dresden.de/~treiber/MicroApplet/MOBIL.html
											if (forcedAccelerationOfVehicleBehindMeOnOtherConnection > bSave)
												{
												if (myAccelerationOnOtherConnection - lowestAcceleration > p * (currentAccelerationOfVehicleBehindMeOnOtherConnection - forcedAccelerationOfVehicleBehindMeOnOtherConnection) + lineChangeThreshold)
													{
													// erstmal mich selbst zerstören
													Dispose((float)((arcPos - m_arcPositionOfStartOnNodeConnection) / (10.0f * (currentTime - m_startTimeOnNodeConnection))));

													// ich darf einen Linienwechsel durchführen, jetzt wirds spannend:
													InitiateLineChange(lcp, arcPos - lcp.start.arcPosition);

													lowestAcceleration = myAccelerationOnOtherConnection;
													}
												}
											}
										}
									}
								}
							}
						}
					}

				#endregion

				}

			#endregion

			if (!onlySimpleCalculations)
				{
				Accelerate(lowestAcceleration);
				}

			return lowestAcceleration;
			}



		/// <summary>
		/// gibt die Entfernung zum nächsten LineNode zurück
		/// </summary>
		/// <returns>currentNodeConnection.lineSegment.length - currentPosition</returns>
		private double GetDistanceToNextNode()
			{
			return currentNodeConnection.lineSegment.length - currentPosition;
			}

        /// <summary>
        /// Bewegt das Auto
        /// </summary>
		/// <param name="tickLength">Länge eines Ticks in Sekunden (berechnet sich mit 1/#Ticks pro Sekunde)</param>
		/// <param name="currentTime">aktuelle Zeit in Sekunden nach Sekunde 0</param>
		public void Move(double tickLength, float currentTime)
            {
			if (!alreadyMoved)
				{
				m_Physics.velocity += physics.acceleration;

				// Rückwärts fahren geht nicht
				if (m_Physics.velocity < 0)
					m_Physics.velocity = 0;

				double arcLengthToMove = (physics.velocity * tickLength * 10);

				// wenn ich gerade am Spurwechseln bin, sollte ich das erstmal behandeln
				if (currentlyChangingLine)
					{
					currentPositionOnLineChangePoint += arcLengthToMove; // ich bewege mich echt auf dem LCP
					m_State.position += arcLengthToMove * ratioProjectionOnTargetConnectionvsLCPLength; // ich muss meine Position auf der Ziel-NodeConnection entsprechend anpassen
					}
				else
					{
					m_State.position += arcLengthToMove;
					}

				// wenn meine aktuelle NodeConnection zu Ende ist, sollte ich das auch behandeln
				if (currentPosition > currentNodeConnection.lineSegment.length)
					{
					// gucken, ob es mit ner Connection weitergeht
					if ((currentNodeConnection.endNode.nextConnections.Count != 0) && (WayToGo.SegmentCount() > 0))
						{
						double startDistance = (currentPosition - currentNodeConnection.lineSegment.length);

						// wenn ich an einer Intersection vorbeigefahren bin, dann sollte ich sie aus originalArrivingTimes wieder rausschmeißen
						List<SpecificIntersection> intersectionsToRemoveFromList = currentNodeConnection.GetIntersectionsWithinArcLength(new Interval<double>(currentPosition - arcLengthToMove, currentNodeConnection.lineSegment.length));

						// falls ich mehrere Connections zur Auswahl habe, berechne die mit dem kürzesten Weg
						// (dieser könnte dich geändert haben, weil dort plötzlich mehr Autos fahren)
						if (currentNodeConnection.endNode.nextConnections.Count > 1)
							{
							m_WayToGo = CalculateShortestConenction(currentNodeConnection.endNode, targetNodes);
							if (m_WayToGo.SegmentCount() == 0 || m_WayToGo.Top() == null)
								{
								Dispose(-1);
								return;
								}
							}

						visitedNodeConnections.AddFirst(currentNodeConnection);

						// Auto auf der alten Linie zerstören
						Dispose((float) (currentPosition - m_arcPositionOfStartOnNodeConnection) / (10.0f * (currentTime - m_startTimeOnNodeConnection)));

						// nächsten Wegpunkt extrahieren
						RouteSegment rs = WayToGo.Pop();

						if (rs == null)
							{
							Dispose(-1);
							return;
							}

						// ist ein Spurwechsel nötig, so die entsprechenden Felder füllen
						if (rs.lineChangeNeeded)
							{
							lineChangeNeeded = true;
							rs.startConnection.lineChangeIntervals.TryGetValue(rs.nextNode.hashcode, out lci);
							}
						else
							{
							lineChangeNeeded = false;
							lci = null;
							}

						// neue NodeConnection setzen
						NodeConnection newNodeConnection = rs.startConnection; //currentNodeConnection.endNode.GetNodeConnectionTo(nextLineNode);
						
						// Neuen State setzen (currentNodeConnection, currentPosition)
						m_State = new State(newNodeConnection, startDistance);

						// Statistiken neu setzen
						m_arcPositionOfStartOnNodeConnection = startDistance;
						m_startTimeOnNodeConnection = currentTime;

						// Fahrzeug dort einfügen
						newNodeConnection.AddVehicle(this);

						intersectionsToRemoveFromList.AddRange(currentNodeConnection.GetIntersectionsWithinArcLength(new Interval<double>(0, currentPosition)));

						// jetzt die Intersections aus originalArrivingTimes wieder rausschmeißen
						foreach (SpecificIntersection si in intersectionsToRemoveFromList)
							{
							originalArrivingTimes.Remove(si.intersection);
							}
						}
					else
						{
						// Ende der Fahnenstange, also selbstzerstören
						Dispose((float)(currentPosition - m_arcPositionOfStartOnNodeConnection) / (10.0f * (currentTime - m_startTimeOnNodeConnection)));
						}
					}
				else
					{
					// wenn ich an einer Intersection vorbeigefahren bin, dann sollte ich sie aus originalArrivingTimes wieder rausschmeißen
					List<SpecificIntersection> intersectionsToRemoveFromList = currentNodeConnection.GetIntersectionsWithinArcLength(new Interval<double>(currentPosition - arcLengthToMove, currentPosition));
					foreach (SpecificIntersection si in intersectionsToRemoveFromList)
						{
						originalArrivingTimes.Remove(si.intersection);
						}
					}
				if (Double.IsNaN(currentPosition))
					{
					Dispose(-1);
					}

				// Der Spurwechsel ist fertig, dann sollte ich diesen auch abschließen:
				if (currentlyChangingLine && currentPositionOnLineChangePoint >= currentLineChangePoint.lineSegment.length)
					{
					FinishLineChange(currentPositionOnLineChangePoint - currentLineChangePoint.lineSegment.length);

					m_startTimeOnNodeConnection = (float)currentTime;
					m_arcPositionOfStartOnNodeConnection = m_State.position;
					}


				alreadyMoved = true;
				}
            }

		/// <summary>
		/// setzt alreadyMoved wieder auf false
		/// </summary>
		public void Reset()
			{
			alreadyMoved = false;
			}


        /// <summary>
        /// Ändert die Beschleunigung des Autos auf den neuen Wert
        /// </summary>
		/// <param name="newAcceleration">der neue Beschleunigungswert</param>
        public void Accelerate(double newAcceleration)
            {
            m_Physics.acceleration = newAcceleration;
            }
        #endregion


		#region Bewegungsberechnungen

		/// <summary>
		/// Flag, ob dieses Auto so nett sein soll, für ein vor ihm einscherendes Fahrzeug abzubremsen
		/// </summary>
		private bool slowDownForChangingVehicle = false;

		/// <summary>
		/// Fahrzeug, dem ich gesagt habe für mich bitte abzubremsen (damit ich mich später wieder abmelden kann)
		/// </summary>
		private IVehicle vehicleWhichSlowsDownForMe = null;

		/// <summary>
		/// Dictionary mit den ursprünglichen Ankunftszeiten für Intersections
		/// </summary>
		public Dictionary<Intersection, double> originalArrivingTimes = new Dictionary<Intersection, double>();

		/// <summary>
		/// Merkvariable für Wunschabstand
		/// </summary>
		private double wunschabstand = 0;




		/// <summary>
		/// gibt das nächste IVehicle und den zugehörigen Abstand zurück. null, falls kein IVehicle in den nächsten distanceWithin
		/// </summary>
		/// <param name="currentNC">NodeConnection für die die Beschleunigungswerte berechnet werden sollen</param>
		/// <param name="arcPosition">Position auf currentNC für die die Beschleunigungswerte berechnet werden sollen</param>
		/// <param name="distanceWithin">Entfernung in der nach Autos gesucht werden soll</param>
		/// <returns>das Vehicle und die zugehörige Entfernung - null, falls kein solches IVehicle existiert</returns>
		private VehicleDistance GetNextVehicleOnMyTrack(NodeConnection currentNC, double arcPosition, double distanceWithin)
			{
			double arcLengthToLookForVehicles = 1.2 * currentNC.startNode.outSlope.Abs; // Suchreichweite nach Fahrzeugen auf parallelen Strecken
			VehicleDistance toReturn = null;

			// bin ich noch auf den ersten Metern der Connection wo ich auf parallele Connections achten muss?
			if (arcPosition < arcLengthToLookForVehicles)
				{
				// alle zu currentNC parallelen NodeConnections anschauen
				foreach (NodeConnection parallelNodeConnection in currentNC.startNode.nextConnections)
					{
					// fahren dort überhaupt Autos?
					if (parallelNodeConnection.vehicles.Count > 0)
						{
						// ich schau mir gerade currentNC an (ich will auf currentNC selbst fahren - also schau ich mir in Zweifelsfall die ganze Connection an)
						if (parallelNodeConnection == currentNC)
							{
							// gucken ob das erste Auto überhaupt in Suchreichweite und näher dran als ein vielleicht schon gefundenes Auto
							if ((listNode.Next != null)
									&& (listNode.Next.Value.currentPosition - arcPosition - listNode.Next.Value.length <= distanceWithin)
									&& (toReturn == null || listNode.Next.Value.currentPosition - arcPosition - listNode.Next.Value.length < toReturn.distance))
								{
								toReturn = new VehicleDistance(listNode.Next.Value, listNode.Next.Value.currentPosition - arcPosition - listNode.Next.Value.length);
								}
							}

						// ich schau mir eine parallele Connection an - da brauch ich nur die ersten Meter zu checken
						else
							{
							IVehicle parallelVehicle = null;
							foreach (IVehicle v in parallelNodeConnection.vehicles)
								{
								if (v.currentPosition > arcPosition)
									{
									parallelVehicle = v;
									break;
									}
								if (v.currentPosition > arcLengthToLookForVehicles)
									{
									break;
									}
								}
							// gucken ob das erste Auto überhaupt in Suchreichweite und näher dran als ein vielleicht schon gefundenes Auto
							if ((parallelVehicle != null)
									&& (toReturn == null || parallelVehicle.currentPosition - arcPosition - parallelVehicle.length < toReturn.distance))
								{
								toReturn = new VehicleDistance(parallelVehicle, parallelVehicle.currentPosition - arcPosition - parallelVehicle.length);
								}
							}
						}
					}

				if (toReturn != null)
					return toReturn;
				}
			else
				{
				// gucken ob auf dem aktuellen LineSegment noch ein Auto vorweg fährt
				if (listNode.Next != null)
					{
					if (listNode.Next.Value.state.position - arcPosition > distanceWithin)
						{
						return null;
						}
					else
						{
						return new VehicleDistance(listNode.Next.Value, listNode.Next.Value.state.position - arcPosition - listNode.Next.Value.length);
						}
					}
				}


			// gucken, ob auf den nächsten LineSegmenten Autos in Reichweite sind
			double alreadyCheckedDistance = GetDistanceToEndOfLineSegment(currentNC, arcPosition);
			foreach (RouteSegment rs in WayToGo)
				{
				NodeConnection nc = rs.startConnection;

				// Abbruchbedingung: wenn schon genug Strecke untersucht wurde, brauchen wir nicht weiterzusuchen
				if (alreadyCheckedDistance > distanceWithin)
					return null;

				// einige Variablen zur Verwendung
				arcLengthToLookForVehicles = nc.startNode.outSlope.Abs; // Suchreichweite nach Fahrzeugen auf parallelen Strecken
				toReturn = null;

				// alle zu nc parallelen NodeConnections anschauen
				foreach (NodeConnection parallelNodeConnection in nc.startNode.nextConnections)
					{
					// fahren dort überhaupt Autos?
					if (parallelNodeConnection.vehicles.Count > 0)
						{
						// ich schau mir gerade nc an (ich will auf nc selbst fahren - also schau ich mir in Zweifelsfall die ganze Connection an)
						if (parallelNodeConnection == nc)
							{
							// gucken ob das erste Auto überhaupt in Suchreichweite und näher dran als ein vielleicht schon gefundenes Auto
							if ((alreadyCheckedDistance + parallelNodeConnection.vehicles.First.Value.currentPosition - parallelNodeConnection.vehicles.First.Value.length <= distanceWithin)
									&& (toReturn == null || alreadyCheckedDistance + parallelNodeConnection.vehicles.First.Value.currentPosition - parallelNodeConnection.vehicles.First.Value.length < toReturn.distance))
								{
								toReturn = new VehicleDistance(parallelNodeConnection.vehicles.First.Value, alreadyCheckedDistance + parallelNodeConnection.vehicles.First.Value.currentPosition - parallelNodeConnection.vehicles.First.Value.length);
								}
							}

						// ich schau mir eine parallele Connection an - da brauch ich nur die ersten Meter zu checken
						else
							{
							// gucken ob das erste Auto überhaupt in Suchreichweite und näher dran als ein vielleicht schon gefundenes Auto
							if ((parallelNodeConnection.vehicles.First.Value.currentPosition - parallelNodeConnection.vehicles.First.Value.length < arcLengthToLookForVehicles)
									&& (toReturn == null || alreadyCheckedDistance + parallelNodeConnection.vehicles.First.Value.currentPosition - parallelNodeConnection.vehicles.First.Value.length < toReturn.distance))
								{
								toReturn = new VehicleDistance(parallelNodeConnection.vehicles.First.Value, alreadyCheckedDistance + parallelNodeConnection.vehicles.First.Value.currentPosition - parallelNodeConnection.vehicles.First.Value.length);
								}
							}
						}
					}

				if (toReturn != null)
					return toReturn;

				alreadyCheckedDistance += nc.lineSegment.length;
				}
			return null;
			}

		/// <summary>
		/// liefert alle Intersections innerhalb der nächsten distanceWithin auf meinem Weg zurück.
		/// Untersucht nicht nur die aktuelle NodeConnection sondern falls nötig auch die folgenden
		/// </summary>
		/// <param name="currentNC">NodeConnection für die die Beschleunigungswerte berechnet werden sollen</param>
		/// <param name="arcPos">Position auf nc für die die Beschleunigungswerte berechnet werden sollen</param>
		/// <param name="distanceWithin">Suchentfernung</param>
		/// <param name="currentTime">aktuelle Zeit in Sekunden nach Sekunde 0 (wird für originalArrivingTime-Berechnung benötigt)</param>
		/// <returns>Eine Liste aller Intersections zwischen arcPos und arcPos+distanceWithin</returns>
		private List<SpecificIntersection> GetNextIntersectionsOnMyTrack(NodeConnection currentNC, double arcPos, double distanceWithin, double currentTime)
			{
			List<SpecificIntersection> toReturn = new List<SpecificIntersection>();

			// muss ich nur auf der currentNC suchen?
			if (distanceWithin <= GetDistanceToEndOfLineSegment(currentNC, arcPos))
				{
				List<SpecificIntersection> intersectionsToAdd = currentNC.GetIntersectionsWithinArcLength(new Interval<double>(arcPos, arcPos + distanceWithin));
				foreach (SpecificIntersection si in intersectionsToAdd)
					{
					toReturn.Add(si);

					// originalArrivingTimes berechnen, falls das noch nicht geschehen ist
					if (! originalArrivingTimes.ContainsKey(si.intersection))
						{
						if (physics.velocity < 4)
							originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(si.intersection.GetMyArcPosition(currentNC) - arcPos, false));
						else
							originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(si.intersection.GetMyArcPosition(currentNC) - arcPos, true));
						}
					}
				}
			// ich muss auch auf den folgenden NodeConnections suchen
			else
				{
				// erstmal alle Intersections auf dieser NodeConnection speichern
				double alreadyCheckedDistance = GetDistanceToEndOfLineSegment(currentNC, arcPos);

				List<SpecificIntersection> intersectionsToAdd = currentNC.GetIntersectionsWithinArcLength(new Interval<double>(arcPos, currentNC.lineSegment.length));
				foreach (SpecificIntersection si in intersectionsToAdd)
					{
					toReturn.Add(si);

					// originalArrivingTimes berechnen, falls das noch nicht geschehen ist
					if (!originalArrivingTimes.ContainsKey(si.intersection))
						{
						if (physics.velocity < 4)
							originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(si.intersection.GetMyArcPosition(currentNC) - arcPos, false));
						else
							originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(si.intersection.GetMyArcPosition(currentNC) - arcPos, true));
						}
					}
				

				// nun die Intersections auf den folgenden NodeConnections
				foreach (RouteSegment rs in WayToGo)
					{
					NodeConnection nc = rs.startConnection;

					// Abbruchbedingung: das LineSegment ist länger als die Rest-Suchentfernung
					if (nc.lineSegment.length > distanceWithin-alreadyCheckedDistance)
						{
						intersectionsToAdd = nc.GetIntersectionsWithinArcLength(new Interval<double>(0, distanceWithin - alreadyCheckedDistance));
						foreach (SpecificIntersection si in intersectionsToAdd)
							{
							toReturn.Add(si);

							// originalArrivingTimes berechnen, falls das noch nicht geschehen ist
							if (!originalArrivingTimes.ContainsKey(si.intersection))
								{
								if (physics.velocity < 4)
									originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(si.intersection.GetMyArcPosition(nc) + alreadyCheckedDistance, false));
								else
									originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(si.intersection.GetMyArcPosition(nc) + alreadyCheckedDistance, true));
								}
							}
				
						break;
						}
					// ich muss noch mehr suchen - also alle Intersections reinballern und die nächste Nodeconnection nehmen
					else
						{
						foreach (Intersection i in nc.intersections)
							{
							SpecificIntersection si = new SpecificIntersection(nc, i);
							toReturn.Add(si);

							// originalArrivingTimes berechnen, falls das noch nicht geschehen ist
							if (!originalArrivingTimes.ContainsKey(si.intersection))
								{
								if (physics.velocity < 4)
									originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(si.intersection.GetMyArcPosition(nc) + alreadyCheckedDistance, false));
								else
									originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(si.intersection.GetMyArcPosition(nc) + alreadyCheckedDistance, true));
								}
							}

						alreadyCheckedDistance += nc.lineSegment.length;
						}
					}
				}

			return toReturn;
			}

		/// <summary>
		/// Finde alle Fahrzeuge die ich evtl. ausbremsen könnte
		/// </summary>
		/// <param name="currentNC">NodeConnection für die die Beschleunigungswerte berechnet werden sollen</param>
		/// <param name="arcPos">Position auf nc für die die Beschleunigungswerte berechnet werden sollen</param>
		/// <param name="distanceWithin">Suchentfernung</param>
		/// <returns>Liste von JammedVehicles</returns>
		private List<InterferingVehicle> GetJammedVehicles(NodeConnection currentNC, double arcPos, double distanceWithin)
			{
			List<InterferingVehicle> toReturn = new List<InterferingVehicle>();

			#region erstmal alle Intersections bestimmen
			List<Intersection> intersections = new List<Intersection>();

			// kann ich die Suche auf die currentNC beschränken?
			if (GetDistanceToEndOfLineSegment(currentNC, arcPos) > distanceWithin)
				{
				intersections.AddRange(currentNC.GetIntersectionsWithinTime(
					new Interval<double>(
						currentNC.lineSegment.PosToTime(arcPos),
						currentNC.lineSegment.PosToTime(arcPos + distanceWithin)
						)));
				}
			// ich muss noch die nächsten NodeConnections auf meinem Weg durchsuchen
			else
				{
				intersections.AddRange(currentNC.GetIntersectionsWithinTime(
					new Interval<double>(currentNC.lineSegment.PosToTime(arcPos), 1)));

				double alreadyCheckedDistance = GetDistanceToEndOfLineSegment(currentNC, arcPos);
				foreach (RouteSegment rs in WayToGo)
					{
					NodeConnection nc = rs.startConnection;

					// Abbruchkriterium
					if (alreadyCheckedDistance + nc.lineSegment.length > distanceWithin)
						{
						intersections.AddRange(currentNC.GetIntersectionsWithinTime(
							new Interval<double>(
								currentNC.lineSegment.PosToTime(arcPos),
								currentNC.lineSegment.PosToTime(arcPos + distanceWithin)
								)));
						break;
						}
					else
						{
						intersections.AddRange(nc.GetIntersectionsWithinTime(new Interval<double>(0, 1)));
						alreadyCheckedDistance += nc.lineSegment.length;
						}
					}
				}
			#endregion

			return toReturn;
			}


		/// <summary>
		/// Berechnet die benötigte Zeit um sich um distance Entfernung zu bewegen
		/// </summary>
		/// <param name="distance">Entfernung, die zurückgelegt werden soll</param>
		/// <param name="keepVelocity">soll die aktuelle Geschwindigkeit beibehalten werden, oder auf Wunschgeschwindigkeit beschleunigt werden?</param>
		/// <returns>approximierte Anzahl der Ticks, die das Fahrzeug braucht um distance Entfernung zurückzulegen</returns>
		public double GetTimeToCoverDistance(double distance, bool keepVelocity)
			{
			if (distance < 0)
				{
				return 0;
				}

			if (keepVelocity)
				{
				return distance / physics.velocity;
				}
			else
				{
				/*
				 * Da sich die IDM Differentialgleichung zwar lösen ließ, aber das Integral darüber selbst für Mathematica 
				 * unlösbar erscheint, lässt sich die benötigte Zeit leider nicht allgemein analytisch lösen.
				 * Genausowenig vermochte Mathematica die Rekurrenzgleichung der durch das Eulerverfahren gegebenen Iteration lösen.
				 * Daher verfolgen wir hier nun einen iterativen Ansatz, der einfach die Zukunft so lange simuliert, bis distance
				 * 
				 * erreicht wurde. Vielleicht könnte man hier in Zukunft eine Tabelle mit Richtwerten anlegen, in der man nur noch 
				 * den passenden Eintrag suchen muss und so Rechenzeit sparen. Momentan scheinen aber noch genug Leistungsreserven 
				 * vorhanden zu sein.
				 */
				double alreadyCoveredDistance = 0;
				int alreadySpentTime = 0;
				double currentVelocity = physics.velocity;

				while (alreadyCoveredDistance <= distance)
					{
					currentVelocity += CalculateAcceleration(currentVelocity, physics.effectiveDesiredVelocity);
					alreadyCoveredDistance += currentVelocity;
					alreadySpentTime++;
					}

				return alreadySpentTime - (1 - ((alreadyCoveredDistance - distance) / currentVelocity));
				}
			}

		#endregion


		#region Ziel
		/// <summary>
        /// Der Ort, zu dem das Fahrzeug hin will
        /// </summary>
		private List<LineNode> m_TargetNodes;
		/// <summary>
		/// Der Ort, zu dem das Fahrzeug hin will
		/// </summary>
		public List<LineNode> targetNodes
			{
			get { return m_TargetNodes; }
			set 
				{ 
				m_TargetNodes = value;
				m_WayToGo = CalculateShortestConenction(currentNodeConnection.endNode, m_TargetNodes);
				if (m_WayToGo.SegmentCount() == 0)
					{
					Dispose(-1);
					}
				}
			}


        #endregion


        #region Umwelt (sinnvoll?)
		/// <summary>
		/// LinkedListNode des Autos
		/// </summary>
        [XmlIgnore]
        public LinkedListNode<IVehicle> listNode;
            
        #endregion

        #region sonstige Funktionen

		/// <summary>
		/// gibt Informationen über dieses Fahrzeug als String zurück
		/// </summary>
		/// <returns>Vehicle #hashcode arcPos=...</returns>
		public override string ToString()
			{
			return "Vehicle #" + hashcode.ToString() + " arcPos=" + currentPosition.ToString();
			}

		/// <summary>
		/// Prüfe, ob ich auf der NodeConnection nc fahren darf
		/// </summary>
		/// <param name="nc">zu prüfende NodeConnection</param>
		/// <returns>true, wenn ich auf nc fahren darf, sonst false</returns>
		public abstract bool CheckNodeConnectionForSuitability(NodeConnection nc);

		/// <summary>
		/// Prüfe, ob ich den LineNode ln von allen Richtungen anfahren darf
		/// </summary>
		/// <param name="ln">zu überprüfender LineNode</param>
		/// <returns>true, falls für alle NodeConnection sin ln.prevConnections CheckNodeConnectionForSuitability() = true gilt</returns>
		public bool CheckLineNodeForIncomingSuitability(LineNode ln)
			{
			foreach (NodeConnection nc in ln.prevConnections)
				{
				if (!CheckNodeConnectionForSuitability(nc))
					return false;
				}
			return true;
			}

        #endregion

        #region Physik
		/// <summary>
		/// Struktur, die Wunschgeschwindigkeit, Geschwindigkeit und Beschleunigung kapselt
		/// </summary>
        public struct Physics
            {
            /// <summary>
            /// Erstellt ein neues Physics Record
            /// </summary>
            /// <param name="d">Wunschgeschwindigkeit</param>
            /// <param name="v">Geschwindigkeit</param>
            /// <param name="a">Beschleunigung</param>
            public Physics(double d, double v, double a)
                {
                desiredVelocity = d;
                velocity = v;
                acceleration = a;
				multiplierDesiredVelocity = 1;
                }




			/// <summary>
			/// gewünschte Gecshwindigkeit des Autos 
			/// </summary>
			public double desiredVelocity;


            /// <summary>
            /// effektive gewünschte Gecshwindigkeit des Autos (mit Multiplikator multipliziert)
            /// </summary>
			public double effectiveDesiredVelocity
				{
				get { return desiredVelocity * multiplierDesiredVelocity; }
				}
			

            /// <summary>
            /// Geschwindigkeit des Fahrzeuges
            /// (sehr wahrscheinlich gemessen in Änderung der Position/Tick)
            /// </summary>
            public double velocity;

            /// <summary>
            /// Beschleunigung des Fahrzeugens
            /// (gemessen in Änderung der Geschwindigkeit/Tick)
            /// </summary>
            public double acceleration;


			/// <summary>
			/// Multiplikator für die Wunschgeschwindigkeit.
			/// Kann benutzt werden, um kurzzeitig schneller zu fahren (etwa um einen Spurwechsel zu machen)
			/// </summary>
			public double multiplierDesiredVelocity;
            }


        #endregion

        #region Status
		/// <summary>
		/// Statusrecord, kapselt aktuelle NodeConnection+Position
		/// </summary>
        public struct State
            {
			/// <summary>
			/// Konstruktor, der nur die Position initialisiert. Der Rest ist uninitialisiert!
			/// </summary>
			/// <param name="p">Zeitposition auf der Linie</param>
            public State(double p)
                {
                currentNodeConnection = null;
                m_Position = p;
                m_PositionAbs = null;
                m_Orientation = null;
                }

			/// <summary>
			/// Standardkonstruktor, benötigt eine Nodeconnection und Zeitposition. Der Rest wird intern berechnet
			/// </summary>
			/// <param name="nc">aktuelle NodeConnection</param>
			/// <param name="p">Zeitposition auf nc</param>
            public State(NodeConnection nc, double p)
                {
                currentNodeConnection = nc;
                m_Position = p;

				double t = currentNodeConnection.lineSegment.PosToTime(p);
				m_PositionAbs = currentNodeConnection.lineSegment.AtTime(t);
				m_Orientation = currentNodeConnection.lineSegment.DerivateAtTime(t);
                }

            /// <summary>
            /// die Line, auf der sich das Fahrzeug gerade befindet
            /// </summary>
            public NodeConnection currentNodeConnection;

            /// <summary>
            /// relative Position auf der Line
            /// </summary>
            private double m_Position;
			/// <summary>
			/// Zeitposition auf der Linie
			/// </summary>
            public double position
                {
                get { return m_Position; }
                set 
                    { 
                    m_Position = value;
                    if (currentNodeConnection != null)
                        {
						double t = currentNodeConnection.lineSegment.PosToTime(m_Position);
						m_PositionAbs = currentNodeConnection.lineSegment.AtTime(t);
						m_Orientation = currentNodeConnection.lineSegment.DerivateAtTime(t);
						}
                    }
                }

            /// <summary>
            /// absolute Position auf der Linie
            /// </summary>
            private Vector2 m_PositionAbs;
			/// <summary>
			/// absolute Position auf der Linie in Weltkoordinaten
			/// </summary>
            public Vector2 positionAbs
                {
                get { return m_PositionAbs; }
                }

            /// <summary>
            /// Richtung in die das Fahrzeug fährt
            /// </summary>
            private Vector2 m_Orientation;
			/// <summary>
			/// Richtung (Ableitung) an der aktuellen Position
			/// </summary>
            public Vector2 orientation
                {
                get { return m_Orientation; }
                }

			/// <summary>
			/// gibt ein das Auto umschließendes RectangleF zurück
			/// </summary>
			public RectangleF boundingRectangle
				{
				get
					{
					return new RectangleF(
						m_PositionAbs - new Vector2(15, 15),
						new Vector2(30, 30)
						);
					}
				}
            }
        #endregion

		// TODO: 
		// Irgendwie scheint der Algorithmus noch nicht so zu laufen wie er soll
		// Der Haken scheint bei der Miteinberechnung der auf den Linien vorhandenen Autos zu sein. 
		// Kann es sein, dass, die entsprechenden Felder nicht richtig aktualisiert werden? Evtl. beim NodeConnection-Wechsel?
		//
		// UPDATE: liegt einfach daran, dass die Beste Route beim Start berechnet wird und dann nicht mehr aktualisiert wird.

		#region A* Algorithmus


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


		/// <summary>
		/// Wegroute zu einem Zielknoten. Rückgabetyp des A*-Algorithmus
		/// </summary>
		public class RouteToTarget : IEnumerable<RouteSegment>
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
			public RouteToTarget()
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
			}

        private PriorityQueue<LineNode.LinkedLineNode, double> openlist = new PriorityQueue<LineNode.LinkedLineNode, double>();
        private Stack<LineNode.LinkedLineNode> closedlist = new Stack<LineNode.LinkedLineNode>();

		/// <summary>
		/// Stack von den noch zu besuchenden NodeConnections
		/// </summary>
		private RouteToTarget m_WayToGo;
		/// <summary>
		/// Stack von den noch zu besuchenden NodeConnections
		/// </summary>
		public RouteToTarget WayToGo
            {
            get { return m_WayToGo; }
            }


		/// <summary>
		/// berechnet die minimale euklidische Entfernung von startNode zu einem der Knoten aus targetNodes
		/// </summary>
		/// <param name="startNode">Startknoten von dem aus die Entfernung berechnet werden soll</param>
		/// <param name="targetNodes">Liste von LineNodes zu denen die Entfernung berechnet werden soll</param>
		/// <returns>minimale euklidische Distanz</returns>
		private double GetMinimumEuklidDistance(LineNode startNode, List<LineNode> targetNodes)
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
		/// Berechnet den kürzesten Weg zum targetNode und speichert diesen als Stack in WayToGo
		/// Implementierung des A*-Algorithmus' frei nach Wikipedia :)
		/// </summary>
		/// <param name="startNode">Startknoten von dem aus der kürzeste Weg berechnet werden soll</param>
		/// <param name="targetNodes">Liste von Zielknoten zu einem von denen der kürzeste Weg berechnet werden soll</param>
        public RouteToTarget CalculateShortestConenction(LineNode startNode, List<LineNode> targetNodes)
            {
            openlist.Clear();
            closedlist.Clear();
			RouteToTarget toReturn = new RouteToTarget();
			//Stack<RouteSegment> toReturn = new Stack<RouteSegment>();
			

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
					// nun noch die closedList in eine RouteToTarget umwandeln
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
					if (! CheckNodeConnectionForSuitability(nc))
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
						double f = currentNode.Value.GetLength()										// exakte Länge des bisher zurückgelegten Weges
							+ theConnection.lineSegment.length;											// exakte Länge des gerade untersuchten Segmentes

						if (currentNode.Value.countOfParents < 3)										// Stau kostet extra, aber nur, wenn innerhalb
							{																			// der nächsten 2 Connections
							f += theConnection.vehicles.Count * Constants.vehicleOnRoutePenalty; 
							}
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

                        if (! nodeInOpenlist)
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
							if (!CheckLineNodeForIncomingSuitability(ln))
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
								double f = currentNode.Value.parent.GetLength();										// exakte Länge des bisher zurückgelegten Weges
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

        #endregion

		#region IDrawable Member

		/// <summary>
		/// Zeichnet das Vehicle auf der Zeichenfläche g
		/// </summary>
		/// <param name="g">Die Zeichenfläche auf der gezeichnet werden soll</param>
		public virtual void Draw(Graphics g)
			{
			GraphicsPath gp = new GraphicsPath();
			if (! currentlyChangingLine)
				{
				PointF[] ppoints = 
					{ 
					state.positionAbs  -  8*state.orientation.RotatedClockwise.Normalized,
					state.positionAbs  +  8*state.orientation.RotatedClockwise.Normalized,
					state.positionAbs  -  length*state.orientation.Normalized  +  8*state.orientation.RotatedClockwise.Normalized,
					state.positionAbs  -  length*state.orientation.Normalized  -  8*state.orientation.RotatedClockwise.Normalized,
	                };
				gp.AddPolygon(ppoints);
				}
			else
				{
				Vector2 positionOnLcp = currentLineChangePoint.lineSegment.AtPosition(currentPositionOnLineChangePoint);
				Vector2 derivate = currentLineChangePoint.lineSegment.DerivateAtTime(currentLineChangePoint.lineSegment.PosToTime(currentPositionOnLineChangePoint));
				PointF[] ppoints = 
					{ 
					positionOnLcp  -  8*derivate.RotatedClockwise.Normalized,
					positionOnLcp  +  8*derivate.RotatedClockwise.Normalized,
					positionOnLcp  -  length*derivate.Normalized  +  8*derivate.RotatedClockwise.Normalized,
					positionOnLcp  -  length*derivate.Normalized  -  8*derivate.RotatedClockwise.Normalized,
					};
				gp.AddPolygon(ppoints);
				}
			g.FillPath(new SolidBrush(color), gp);
			}

		/// <summary>
		/// Zeichnet Debuginformationen auf die Zeichenfläche g
		/// </summary>
		/// <param name="g">Die Zeichenfläche auf der gezeichnet werden soll</param>
		public void DrawDebugData(Graphics g)
			{
			Pen lineToIntersectionPen = new Pen(Color.Maroon);
			Brush blackBrush =  new SolidBrush(Color.Black);
			Font debugFont = new Font("Calibri", 6);

			Pen prevNodeConnectionsPen = new Pen(Color.Red, 3);
			Pen nextNodeConnectionsPen = new Pen(Color.Green, 3);

			foreach (KeyValuePair<Intersection, double> pair in originalArrivingTimes)
				{
				//g.DrawLine(lineToIntersectionPen, pair.Key.aPosition, this.state.positionAbs);
				//g.DrawString(pair.Value.ToString(), debugFont, blackBrush, pair.Key.aPosition + 0.5 * (state.positionAbs - pair.Key.aPosition));
				}


			foreach (NodeConnection prevNC in visitedNodeConnections)
				{
				g.DrawBezier(prevNodeConnectionsPen, prevNC.lineSegment.p0, prevNC.lineSegment.p1, prevNC.lineSegment.p2, prevNC.lineSegment.p3);
				}
			foreach (RouteSegment rs in WayToGo)
				{
				if (!rs.lineChangeNeeded)
					{
					NodeConnection nextNC = rs.startConnection;
					g.DrawBezier(nextNodeConnectionsPen, nextNC.lineSegment.p0, nextNC.lineSegment.p1, nextNC.lineSegment.p2, nextNC.lineSegment.p3);
					}
				else
					{
					g.DrawLine(nextNodeConnectionsPen, rs.startConnection.startNode.position, rs.nextNode.position);
					}
				}


			g.DrawString(hashcode.ToString() + " @ " + currentNodeConnection.lineSegment.PosToTime(currentPosition).ToString("0.##") + "t ," + currentPosition.ToString("####") + "dm - " + physics.velocity.ToString("##.#") + "m/s\nnoch " + WayToGo.SegmentCount() + " nodes zu befahren\n" + behaviour.GetDebugString() + "\n\n" + debugData.ToString(), debugFont, blackBrush, state.positionAbs + new Vector2(0, -10));

			behaviour.DrawDebugGraphics(g);
			}

		#endregion
		}
    }
