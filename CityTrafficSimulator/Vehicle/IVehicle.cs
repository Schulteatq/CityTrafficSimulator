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
using System.Diagnostics;
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
		/// absolute Startzeit des Fahrzeuges auf der aktuellen NodeConnection
		/// </summary>
		protected float m_startTimeOnNodeConnection;
		/// <summary>
		/// absolute Startzeit des Fahrzeuges auf der aktuellen NodeConnection
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
			if (vehicleWhichSlowsDownForMe != null)
				vehicleWhichSlowsDownForMe.slowDownForChangingVehicle = false;
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

			// Tell other vehicle that waits for me, that I'm finished
			if (vehicleWhichSlowsDownForMe != null)
				{
				vehicleWhichSlowsDownForMe.slowDownForChangingVehicle = false;
				vehicleWhichSlowsDownForMe = null;
				}
			}


		#endregion


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
		/// Flag, ob auf der aktuellen NodeConnection ein Spurwechsel nötig ist
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
			List<NodeConnection> route = new List<NodeConnection>();
			route.Add(currentNodeConnection);
			foreach (RouteSegment rs in WayToGo)
				route.Add(rs.startConnection);

			double acceleration = Think(route, currentPosition, false, tickLength, currentTime);
			Accelerate(acceleration);
			//Think(currentNodeConnection, currentPosition, false, tickLength, currentTime);
			}

		/// <summary>
		/// Calculates driving parameters and new acceleration of the vehicle.
		/// </summary>
		/// <param name="route">Route of the Vehicle.</param>
		/// <param name="arcPos">Current arc position of the vehicle on the first NodeConnection on <paramref name="route"/>.</param>
		/// <param name="onlySimpleCalculations">Perform only simple calculations (e.g. no free line changes).</param>
		/// <param name="tickLength">Length of a tick in seconds.</param>
		/// <param name="currentTime">Current world time.</param>
		/// <returns></returns>
		public double Think(List<NodeConnection> route, double arcPos, bool onlySimpleCalculations, double tickLength, double currentTime)
			{
			if (route.Count == 0)
				return 0;

			double lookaheadDistance = Constants.lookaheadDistance;
			double stopDistance = -1;

			bool thinkAboutLineChange = false;
			double lowestAcceleration = 0;
			m_Physics.multiplierDesiredVelocity = (slowDownForChangingVehicle) ? 2 * p : 1;

			#region Vehicles in front

			// Determine the next vehicle in front.
			VehicleDistance theVehicleInFrontOfMe = GetNextVehicleOnMyTrack(route[0], arcPos, lookaheadDistance);

			// The stored distance is to the front of the vehicle. All following calculations need the distance
			// to its tail. Hence, substract the vehicle length.
			if (theVehicleInFrontOfMe != null)
				{
				lookaheadDistance = theVehicleInFrontOfMe.distance;
				thinkAboutLineChange = true;
				lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, theVehicleInFrontOfMe.distance, physics.velocity - theVehicleInFrontOfMe.vehicle.physics.velocity);

				if (    (theVehicleInFrontOfMe.vehicle.physics.velocity < 2.5)
					 || (theVehicleInFrontOfMe.vehicle.physics.velocity < 5 && theVehicleInFrontOfMe.vehicle.physics.acceleration < 0.1))
					{
					stopDistance = theVehicleInFrontOfMe.distance;
					}
				}
			else
				{
				lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lookaheadDistance, physics.velocity);
				}

			#endregion

			#region Traffic lights

			// Check for red traffic lights on route
			double distanceToTrafficLight = GetDistanceToNextTrafficLightOnRoute(route, arcPos, lookaheadDistance, true);

			// If the next TrafficLight is closer than the next vehicle, no free line change shall be performed
			if (distanceToTrafficLight < lookaheadDistance)
				{
				lookaheadDistance = distanceToTrafficLight;
				thinkAboutLineChange = false;
				lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lookaheadDistance, physics.velocity);
				}
			
			#endregion

			#region Intersections

			// Registration target for intersections.
			// (When doing simple calculations, we do not want to unregister at all intersections. Hence, we use a temporary regsitration):
			LinkedList<SpecificIntersection> registrationTarget = (onlySimpleCalculations ? temporaryRegisteredIntersections : registeredIntersections);

			// gather all upcoming intersections (and update the ones we are already registered at)
			GatherNextIntersectionsOnMyTrack(route, arcPos, registrationTarget, lookaheadDistance, currentTime);
			double distanceToIntersection = HandleIntersections(registrationTarget, stopDistance);

			// If there is an intersection where I should wait, I should do so...
			if (distanceToIntersection > 0 && distanceToIntersection < lookaheadDistance)
				{
				lookaheadDistance = distanceToIntersection;
				lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, distanceToIntersection, physics.velocity);
				}

			// In case of simple calculations we need to unregister the vehicle from all intersections. Otherwise
			// some other vehicle might wait for this vehicle even if it will never pass the intersection.
			// (simple calculations are only hypothetical)
			if (onlySimpleCalculations)
				{
				foreach (SpecificIntersection si in registrationTarget)
					{
					si.intersection.UnregisterVehicle(this, si.nodeConnection);
					}
				registrationTarget.Clear();
				}

			#endregion

			#region Line changes

			// simple calculation do not consider line changes
			if (!onlySimpleCalculations)
				{
				#region Forced line changes

				// our route forces to perform a line change
				if (lineChangeNeeded && !currentlyChangingLine)
					{
					thinkAboutLineChange = false;

					// get current LineChangePoint and check, whether it's leading to our target
					NodeConnection.LineChangePoint lcp = route[0].GetPrevLineChangePoint(arcPos);
					if (lci.targetNode.prevConnections.Contains(lcp.target.nc))
						{
						bool slowDownToBreakPoint = false;
						double myArcPositionOnOtherConnection = lcp.otherStart.arcPosition + (arcPos - lcp.start.arcPosition);

						// check if found LineChangePoint is not too far away to perform the line change
						if ((myArcPositionOnOtherConnection >= 0) && (Math.Abs(arcPos - lcp.start.arcPosition) < Constants.maxDistanceToLineChangePoint * 0.67))
							{
							// Check the relation to my surrounding vehicles on the target NodeConnection
							Pair<VehicleDistance> otherVehicles = lcp.otherStart.nc.GetVehiclesAroundArcPosition(myArcPositionOnOtherConnection, Constants.lookaheadDistance);

							// the new vehicle in front wouldn't be too close
							if (   otherVehicles.Right == null
								|| otherVehicles.Right.distance > otherVehicles.Right.vehicle.length + CalculateWantedDistance(physics.velocity, physics.velocity - otherVehicles.Right.vehicle.physics.velocity)/2)
								{
								// the new vehicle behind wouldn't be too close
								if (   otherVehicles.Left == null
									|| otherVehicles.Left.distance > length + otherVehicles.Left.vehicle.CalculateWantedDistance(otherVehicles.Left.vehicle.physics.velocity, otherVehicles.Left.vehicle.physics.velocity - physics.velocity)/2)
									{
									// calculate my necessary acceleration in case of a line change
									double myAccelerationOnOtherConnection = (otherVehicles.Right != null)
										? CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, otherVehicles.Right.vehicle.currentPosition - myArcPositionOnOtherConnection, physics.velocity - otherVehicles.Right.vehicle.physics.velocity)
										: CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lookaheadDistance, physics.velocity);

									// calculate the necessary acceleration of the vehicle behind in case of a line change
									double forcedAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null)
										? CalculateAcceleration(otherVehicles.Left.vehicle.physics.velocity, otherVehicles.Left.vehicle.physics.effectiveDesiredVelocity, otherVehicles.Left.distance, otherVehicles.Left.vehicle.physics.velocity - physics.velocity)
										: 0;

									double currentAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null) ? otherVehicles.Left.vehicle.physics.acceleration : 0;

									// Final check:
									//  - The new vehicle behind must not break harder than bSave
									//  - My line change must be sufficiently necessary. The closer I come to the end of the LineChangeInterval, the more I may thwart the vehicle behind. 
									// TODO: double check calculations - the politeness factor seems to be canceled out...
									if (   (forcedAccelerationOfVehicleBehindMeOnOtherConnection > bSave)
										&& (p * (arcPos - lci.startArcPos) / lci.length >= p * (currentAccelerationOfVehicleBehindMeOnOtherConnection - forcedAccelerationOfVehicleBehindMeOnOtherConnection)))
										{
										// return to normal velocity
										m_Physics.multiplierDesiredVelocity = 1;

										// dispose on current NodeConnection
										Dispose((float)((arcPos - m_arcPositionOfStartOnNodeConnection) / (10.0f * (currentTime - m_startTimeOnNodeConnection))));

										// initiate the line change
										InitiateLineChange(lcp, arcPos - lcp.start.arcPosition);
										lowestAcceleration = myAccelerationOnOtherConnection;
										}
									}
								// the new vehicle behind would too close but I can accelerate and are at least as fast as him
								else if (   otherVehicles.Left.vehicle.physics.velocity / this.physics.velocity < 2  // I am not significantly slower than him
										 && lookaheadDistance > 2 * length			// no vehicle/traffic light/intersection in front
										 && lci.endArcPos - arcPos > 2 * length		// enough space left in LineChangeInterval
										 && lowestAcceleration >= -0.1)				// currently not braking
									{
									// accelerate to get in front
									m_Physics.multiplierDesiredVelocity = 1.75;
									lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lookaheadDistance, physics.velocity);

									// Tell the other vehicle that I want to change line. Probably, it will brake for me.
									if (vehicleWhichSlowsDownForMe != null)
										vehicleWhichSlowsDownForMe.slowDownForChangingVehicle = false;

									otherVehicles.Left.vehicle.slowDownForChangingVehicle = true;
									vehicleWhichSlowsDownForMe = otherVehicles.Left.vehicle;
									}
								// There is no way to perform a line change now => slow down
								else
									{
									slowDownToBreakPoint = true;
									}
								}
							// The new vehicle in front is too close => slow down
							else
								{
								slowDownToBreakPoint = true;
								}
							}

						if (slowDownToBreakPoint)
							{
							double percentOfLCIPassed = Math.Max(0.2, (lci.endArcPos - currentPosition - Constants.breakPointBeforeForcedLineChange) / (lci.length - Constants.breakPointBeforeForcedLineChange));

							// slow down a bit
							m_Physics.multiplierDesiredVelocity = percentOfLCIPassed;

							// When reaching the end of the LineChangeInterval, check whether there are other possibilities to reach the target:
							if (percentOfLCIPassed < 0.5)
								{
								RouteToTarget newRTT = CalculateShortestConenction(route[0].endNode, m_TargetNodes);
								// The alternative route does not cost too much -> choose it
								if (newRTT.SegmentCount() > 0 && newRTT.costs / m_WayToGo.costs < Constants.maxRatioForEnforcedLineChange)
									{
									m_WayToGo = newRTT;
									m_Physics.multiplierDesiredVelocity = 1;
									lineChangeNeeded = false;
									lci = null;
									}
								// No suitable alternative -> stop at break point
								else
									{
									lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lci.endArcPos - Constants.breakPointBeforeForcedLineChange - arcPos, physics.velocity);
									}
								}
							// Stop at break point
							else
								{
								lowestAcceleration = CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, lci.endArcPos - Constants.breakPointBeforeForcedLineChange - arcPos, physics.velocity);
								}
							}
						}
					}

				#endregion

				#region freiwillig

				if (thinkAboutLineChange && !currentlyChangingLine)
					{
					// get current LineChangePoint and check, whether it's reachable
					NodeConnection.LineChangePoint lcp = route[0].GetPrevLineChangePoint(arcPos);
					if ((lcp.target.nc != null) && (Math.Abs(arcPos - lcp.start.arcPosition) < Constants.maxDistanceToLineChangePoint * 0.67))
						{
						// check whether there is an alternative route that is not too costly
						RouteToTarget alternativeRoute = CalculateShortestConenction(lcp.target.nc.endNode, targetNodes);
						if (alternativeRoute.SegmentCount() > 0 && alternativeRoute.costs / WayToGo.costs < Constants.maxRatioForVoluntaryLineChange && !alternativeRoute.Top().lineChangeNeeded)
							{
							double myArcPositionOnOtherConnection = lcp.otherStart.arcPosition + (arcPos - lcp.start.arcPosition);
							if (myArcPositionOnOtherConnection >= 0)
								{
								// Check the relation to my surrounding vehicles on the target NodeConnection
								Pair<VehicleDistance> otherVehicles = lcp.otherStart.nc.GetVehiclesAroundArcPosition(myArcPositionOnOtherConnection, Constants.lookaheadDistance);

								// the new vehicle in front wouldn't be too close
								if (   otherVehicles.Right == null
									|| otherVehicles.Right.distance > otherVehicles.Right.vehicle.length + 2 * lcp.length)
									{
									// the new vehicle behind wouldn't be too close
									if (   otherVehicles.Left == null
										|| otherVehicles.Left.distance > length + otherVehicles.Left.vehicle.CalculateWantedDistance(otherVehicles.Left.vehicle.physics.velocity, otherVehicles.Left.vehicle.physics.velocity - physics.velocity)/2)
										{
										List<NodeConnection> l = new List<NodeConnection>();
										l.Add(lcp.target.nc);
										foreach (RouteSegment rs in alternativeRoute)
											l.Add(rs.startConnection);

										// calculate my necessary acceleration in case of a line change
										double myAccelerationOnOtherConnection = (otherVehicles.Right != null)
											? CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, otherVehicles.Right.vehicle.currentPosition - myArcPositionOnOtherConnection, physics.velocity - otherVehicles.Right.vehicle.physics.velocity)
											: CalculateAcceleration(physics.velocity, physics.effectiveDesiredVelocity, GetDistanceToNextTrafficLightOnRoute(l, myArcPositionOnOtherConnection, Constants.lookaheadDistance, true), physics.velocity);

										// calculate the necessary acceleration of the vehicle behind in case of a line change
										double forcedAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null)
											? CalculateAcceleration(otherVehicles.Left.vehicle.physics.velocity, otherVehicles.Left.vehicle.physics.effectiveDesiredVelocity, otherVehicles.Left.distance, otherVehicles.Left.vehicle.physics.velocity - physics.velocity)
											: 0;

										double currentAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null) ? otherVehicles.Left.vehicle.physics.acceleration : 0;

										// simplified implementation of MOBIL: http://www.vwi.tu-dresden.de/~treiber/MicroApplet/MOBIL.html
										if (forcedAccelerationOfVehicleBehindMeOnOtherConnection > bSave)
											{
											if (myAccelerationOnOtherConnection - lowestAcceleration > p * (currentAccelerationOfVehicleBehindMeOnOtherConnection - forcedAccelerationOfVehicleBehindMeOnOtherConnection) + lineChangeThreshold)
												{
												// dispose on current NodeConnection
												Dispose((float)((arcPos - m_arcPositionOfStartOnNodeConnection) / (10.0f * (currentTime - m_startTimeOnNodeConnection))));

												// initiate the line change
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

				#endregion

				}

			#endregion

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
						if (lineChangeNeeded)
							{
							m_Physics.multiplierDesiredVelocity = 1;
							}
						if (vehicleWhichSlowsDownForMe != null)
							{
							vehicleWhichSlowsDownForMe.slowDownForChangingVehicle = false;
							vehicleWhichSlowsDownForMe = null;
							}

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
		/// List of all Intersections where this vehicle has registered itsself.
		/// This list is sorted ascending by the remaining distance of the vehicle to the intersection.
		/// </summary>
		private LinkedList<SpecificIntersection> registeredIntersections = new LinkedList<SpecificIntersection>();

		/// <summary>
		/// List of all Intersections where this vehicle has temporarily registered itsself.
		/// This list is sorted ascending by the remaining distance of the vehicle to the intersection.
		/// </summary>
		private LinkedList<SpecificIntersection> temporaryRegisteredIntersections = new LinkedList<SpecificIntersection>();

		/// <summary>
		/// Returns the next vehicle on the given Nodeconnection together with the distance to its rear end.
		/// </summary>
		/// <param name="nc">NodeConnection where to start with search</param>
		/// <param name="arcPos">Arc position on given Nodeconnection.</param>
		/// <param name="distance">Distance to cover during search.</param>
		/// <returns>The closest vehicle and its distance to the rear end, null if no such vehicle exists.</returns>
		private VehicleDistance GetNextVehicleOnMyTrack(NodeConnection nc, double arcPos, double distance)
			{
			VehicleDistance toReturn = null;

			// the beginning of a NodeConnection may be close to other connections - search on each of them
			double arcLengthToLookForParallelVehicles = 1.2 * nc.startNode.outSlope.Abs; // search distance on parallel connections
			foreach (NodeConnection parallelNodeConnection in nc.startNode.nextConnections)
				{
				VehicleDistance vd = parallelNodeConnection.GetVehicleBehindArcPosition(arcPos, distance);
				if (vd != null && vd.distance > 0)
					{
					vd.distance -= vd.vehicle.length;

					if ((toReturn == null || vd.distance < toReturn.distance) && (parallelNodeConnection == nc || arcPos < arcLengthToLookForParallelVehicles))
						{
						toReturn = vd;
						}
					}
				}

			return toReturn;
			}

		/// <summary>
		/// Searches for the next TrafficLight on the vehicle's route within the given distance.
		/// </summary>
		/// <param name="route">Route of the Vehicle.</param>
		/// <param name="arcPos">Current arc position of the vehicle on the first NodeConnection on <paramref name="route"/>.</param>
		/// <param name="distance">Distance to cover during search.</param>
		/// <param name="considerOnlyRed">If true, only red traffic lights will be considered.</param>
		/// <returns>The distance to the next TrafficLight on the vehicle's route that covers the given constraints. <paramref name="distance"/> if no such TrafficLight exists.</returns>
		private double GetDistanceToNextTrafficLightOnRoute(List<NodeConnection> route, double arcPos, double distance, bool considerOnlyRed)
			{
			Debug.Assert(route.Count > 0);

			double doneDistance = -arcPos;
			foreach (NodeConnection nc in route)
				{
				doneDistance += nc.lineSegment.length;
				if (doneDistance >= distance)
					return distance;

				if (nc.endNode.tLight != null && (!considerOnlyRed || nc.endNode.tLight.trafficLightState == TrafficLight.State.RED))
					return doneDistance;
				}

			return distance;
			}

		/// <summary>
		/// Gathers all upcoming intersections on the current route and stores/updates them in registeredIntersections.
		/// </summary>
		/// <param name="route">Route of the Vehicle.</param>
		/// <param name="arcPos">Current arc position of the vehicle on the first NodeConnection on <paramref name="route"/>.</param>
		/// <param name="intersectionRegistration">Target list to store all intersections where the vehicle is registered. CAUTION: Will be modified!</param>
		/// <param name="distance">Distance to cover during search.</param>
		/// <param name="currentTime">Current world time.</param>
		private void GatherNextIntersectionsOnMyTrack(List<NodeConnection> route, double arcPos, LinkedList<SpecificIntersection> intersectionRegistration, double distance, double currentTime)
			{
			Debug.Assert(route.Count > 0);

			// Setup some helper variables:
			double doneDistance = -currentPosition;										// total covered distance
			double remainingDistance = distance;										// total remaining distance to cover
			double startPosition = currentPosition - length;							// start position at current NodeConnection
			LinkedListNode<SpecificIntersection> lln = intersectionRegistration.First;	// current already registered intersection

			// search each NodeConnection
			foreach (NodeConnection nc in route)
				{
				// gather all intersections on current nc
				List<SpecificIntersection> l = nc.GetSortedIntersectionsWithinArcLength(new Interval<double>(startPosition - length, startPosition + remainingDistance));

				// merge found intersections with already registered ones while cleaning them up at the same time
				foreach (SpecificIntersection si in l)
					{
					double myDistance = doneDistance + si.intersection.GetMyArcPosition(si.nodeConnection);

					// unregister at all intersections between last and si
					while (lln != null && !SpecificIntersection.Equals(lln.Value, si))
						{
						LinkedListNode<SpecificIntersection> backupNode = lln;
						lln = lln.Next;
						intersectionRegistration.Remove(backupNode);
						backupNode.Value.intersection.UnregisterVehicle(this, backupNode.Value.nodeConnection);
						}

					// the current intersection is already registered - update needed
					if (lln != null)
						{
						si.intersection.UpdateVehicle(this, si.nodeConnection, myDistance, currentTime);
						lln.Value = new SpecificIntersection(si.nodeConnection, si.intersection);
						lln = lln.Next;
						}
					// the current intersection is not yet registered - do it now
					else
						{
						si.intersection.RegisterVehicle(this, si.nodeConnection, myDistance, currentTime);
						if (lln != null)
							intersectionRegistration.AddBefore(lln, new SpecificIntersection(si.nodeConnection, si.intersection));
						else
							intersectionRegistration.AddLast(new SpecificIntersection(si.nodeConnection, si.intersection));
						}
					}

				// continue with next NodeConnection on route
				remainingDistance -= (nc.lineSegment.length - startPosition);
				if (remainingDistance < 0)
					break;

				doneDistance += nc.lineSegment.length;
				startPosition = 0;
				}

			// Unregister from all following intersections
			while (lln != null)
				{
				LinkedListNode<SpecificIntersection> backupNode = lln;
				lln = lln.Next;
				intersectionRegistration.Remove(backupNode);
				backupNode.Value.intersection.UnregisterVehicle(this, backupNode.Value.nodeConnection);
				}
			}

		/// <summary>
		/// Handles each intersection in <paramref name="intersectionRegistration"/>.
		/// </summary>
		/// <param name="intersectionRegistration">List of all intersections to handle.</param>
		/// <param name="stopPoint">Distance to position where vehicle is going to stop. Setting this value to a sane value may prevent blocking of intersections.</param>
		/// <returns>If there is an intersection where the vehicle should wait, the remaining distance to this intersection will be returned. If no such intersection exists -1 will be returned.</returns>
		private double HandleIntersections(LinkedList<SpecificIntersection> intersectionRegistration, double stopPoint)
			{
			LinkedListNode<SpecificIntersection> lln = intersectionRegistration.First;
			while (lln != null)
				{
				SpecificIntersection si = lln.Value;
				bool waitInFrontOfIntersection = false;

				// Get interfering vehicles for this intersections
				List<CrossingVehicleTimes> cvtList = si.intersection.CalculateInterferingVehicles(this, si.nodeConnection);
				NodeConnection otherNC = si.intersection.GetOtherNodeConnection(si.nodeConnection);
				CrossingVehicleTimes myCvt = si.intersection.GetCrossingVehicleTimes(this, si.nodeConnection);

				// If other NodeConnection is more important than mine
				if (otherNC.priority > si.nodeConnection.priority)
					{
					// If there is any infering vehicle, I should wait in front of the intersection.
					// TODO:	Develop s.th. more convenient (e.g. if possible, try to accelerate a little to get through first).
					if (cvtList.Count > 0)
						waitInFrontOfIntersection = true;
					// Intersection is close to stop point so that vehicle will block this intersection => wait in front
					if ((stopPoint > 0) && (stopPoint - length - s0 < myCvt.remainingDistance))
						{
						waitInFrontOfIntersection = true;
						}
					}
				// Both NodeConnections have the same priority
				else if (otherNC.priority == si.nodeConnection.priority)
					{
					// Intersection is close to stop point so that vehicle will block this intersection => wait in front
					if ((stopPoint > 0) && (stopPoint - length - s0 < myCvt.remainingDistance))
						{
						waitInFrontOfIntersection = true;
						}

					// check at each intersection, which vehicle was there first
					foreach (CrossingVehicleTimes otherCvt in cvtList)
						{						
						// I should wait if:
						//  - The other vehicle originally reached the intersection before me
						//  - TODO: I would block him significantly if I continue.
						if (myCvt.originalArrivingTime >= otherCvt.originalArrivingTime)
							{
							waitInFrontOfIntersection = true;
							break;
							}
						}
					}
				// My Nodeconnection is more important than the other one
				else
					{
					// Nothing to do here yet.
					// Above is ensured, that the other vehicle will wait (hopefully, it does... o_O)
					}

				if (waitInFrontOfIntersection)
					{
					// first go backwards: If I wait before an intersection I might block other intersections before.
					// This is usually not wanted, therefore go backwards and wait in front of the first intersection 
					// where I won't block any other.
					double distanceToLookBack = si.intersection.GetCrossingVehicleTimes(this, si.nodeConnection).remainingDistance - m_Length;
					while (lln.Previous != null)
						{
						double remainingDistanceToPrevIntersection = lln.Previous.Value.intersection.GetCrossingVehicleTimes(this, lln.Previous.Value.nodeConnection).remainingDistance;

						// check whether intersection will be blocked
						if (remainingDistanceToPrevIntersection > 0 && remainingDistanceToPrevIntersection + lln.Previous.Value.intersection.GetWaitingDistance() > distanceToLookBack)
							{
							// will be blocked, wait in front of it and continue looking backwards
							distanceToLookBack = remainingDistanceToPrevIntersection - m_Length;
							lln = lln.Previous;
							}
						else
							{
							// will not be blocked => no further search necessary
							break;
							}
						}

					si = lln.Value; // si is a Value-Type (copy)*/

					// Update this and all following intersections, that I won't cross in the near future.
					while (lln != null)
						{
						lln.Value.intersection.UpdateVehicle(this, lln.Value.nodeConnection, true);
						lln = lln.Next;
						}

					return si.intersection.GetCrossingVehicleTimes(this, si.nodeConnection).remainingDistance - si.intersection.GetWaitingDistance(); // si is a Value-Type (copy)
					}

				lln = lln.Next;
				}

			return -1;
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
			Pen lineToIntersectionPen = new Pen(Color.Maroon, 3);
			Brush blackBrush =  new SolidBrush(Color.Black);
			Font debugFont = new Font("Calibri", 6);

			Pen prevNodeConnectionsPen = new Pen(Color.Red, 3);
			Pen nextNodeConnectionsPen = new Pen(Color.Green, 3);

			foreach (KeyValuePair<Intersection, double> pair in originalArrivingTimes)
				{
				//g.DrawLine(lineToIntersectionPen, pair.Key.aPosition, this.state.positionAbs);
				//g.DrawString(pair.Value.ToString(), debugFont, blackBrush, pair.Key.aPosition + 0.5 * (state.positionAbs - pair.Key.aPosition));
				}
			if (vehicleWhichSlowsDownForMe != null)
				{
				g.DrawLine(lineToIntersectionPen, state.positionAbs, vehicleWhichSlowsDownForMe.state.positionAbs);
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


			g.DrawString(hashcode.ToString() + " @ " + currentNodeConnection.lineSegment.PosToTime(currentPosition).ToString("0.##") + "t ," + currentPosition.ToString("####") + "dm - " + physics.velocity.ToString("##.#") + "m/s\nnoch " + WayToGo.SegmentCount() + " nodes zu befahren\n\n" + debugData.ToString(), debugFont, blackBrush, state.positionAbs + new Vector2(0, -10));
			}

		#endregion
		}
    }
