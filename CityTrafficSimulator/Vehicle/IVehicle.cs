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
    public abstract class IVehicle : IDM
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
		/// Vehicle statistics record
		/// </summary>
		public struct Statistics
			{
			/// <summary>
			/// world time when vehicle was created
			/// </summary>
			public double startTime;

			/// <summary>
			/// Total milage in network (arc length as unit)
			/// </summary>
			public double totalMilage;

			/// <summary>
			/// Number of used NodeConnections
			/// </summary>
			public int numNodeConnections;

			/// <summary>
			/// Number of performed line changes
			/// </summary>
			public int numLineChanges;

			/// <summary>
			/// absolute Startzeit des Fahrzeuges auf der aktuellen NodeConnection
			/// </summary>
			public double startTimeOnNodeConnection;

			/// <summary>
			/// Bogenlängenposition, bei der dieses Auto auf dieser Linie gestartet ist
			/// </summary>
			public double arcPositionOfStartOnNodeConnection;

			/// <summary>
			/// number of times this vehicle stopped
			/// </summary>
			public int numStops;
			}

		/// <summary>
		/// Statistics record of this vehicle
		/// </summary>
		protected IVehicle.Statistics _statistics = new IVehicle.Statistics();
		/// <summary>
		/// Statistics record of this vehicle
		/// </summary>
		public IVehicle.Statistics statistics
			{
			get { return _statistics; }
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
			_statistics.startTime = GlobalTime.Instance.currentTime;
			_statistics.startTimeOnNodeConnection = GlobalTime.Instance.currentTime;
			}

		#endregion

		#region Selbstzerstörung

		/// <summary>
		/// To be called when this vehicle shall be removed from the current NodeConnection
		/// </summary>
		/// <param name="logConnectionStatistics">Flag, whether NodeConnection statistics shall be logged</param>
		/// <param name="nextConnection">NodeConnection where to respawn vehicle - set to null if vehicle shall not respawn</param>
		/// <param name="nextArcPosition">arc position on nextConnection where vehicle shall respawn</param>
		private void RemoveFromCurrentNodeConnection(bool logConnectionStatistics, NodeConnection nextConnection, double nextArcPosition)
			{
			_state.UnsetLineChangeVehicleInteraction();

			if (logConnectionStatistics)
				{
				OnVehicleLeftNodeConnection(new VehicleLeftNodeConnectionEventArgs(new Interval<double>(statistics.arcPositionOfStartOnNodeConnection, currentPosition), new Interval<double>(statistics.startTimeOnNodeConnection, GlobalTime.Instance.currentTime)));
				}
			currentNodeConnection.RemoveVehicle(this);

			
			if (nextConnection != null)
				{
				double pos = Math.Min(nextConnection.lineSegment.length, nextArcPosition);
				if (pos < nextArcPosition)
					{
					// FIXME: this sometimes happens, even if it should not!
					}

				lastLineChangeCheck.Right -= currentNodeConnection.lineSegment.length;

				// update statistics
				_statistics.totalMilage += currentPosition - statistics.arcPositionOfStartOnNodeConnection;
				_statistics.numNodeConnections++;
				_statistics.arcPositionOfStartOnNodeConnection = pos;
				_statistics.startTimeOnNodeConnection = GlobalTime.Instance.currentTime;

				// set new state
				_state = new State(nextConnection, pos);

				// add vehicle to new node connection
				nextConnection.AddVehicleAt(this, pos);
				}
			else
				{
				// evoke VehicleDied event
				OnVehicleDied(new VehicleDiedEventArgs(_statistics, targetNodes.Contains(currentNodeConnection.endNode)));
				}
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
		/// Last check for voluntary line change (Left = world time, Right = arc pos)
		/// </summary>
		private Pair<double> lastLineChangeCheck = new Pair<double>();

		/// <summary>
		/// Initiiert einen Spurwechsel
		/// </summary>
		/// <param name="lcp">LCP auf dem der Spurwechsel durchgeführt werden soll</param>
		/// <param name="arcPositionOffset">bereits auf dem LCP zurückgelegte Strecke</param>
		private void InitiateLineChange(NodeConnection.LineChangePoint lcp, double arcPositionOffset)
			{
			// einem evtl. ausgebremsten Fahrzeug sagen, dass es nicht mehr extra für mich abbremsen braucht
			_state.UnsetLineChangeVehicleInteraction();

			// unregister at all intersections
			foreach (SpecificIntersection si in registeredIntersections)
				{
				si.intersection.UnregisterVehicle(this, si.nodeConnection);
				}
			registeredIntersections.Clear();
	
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
			RemoveFromCurrentNodeConnection(true, lcp.otherStart.nc, lcp.otherStart.arcPosition + arcPositionOffset * ratioProjectionOnTargetConnectionvsLCPLength);
			_wayToGo = Routing.CalculateShortestConenction(currentNodeConnection.endNode, targetNodes, _vehicleType);

			_statistics.numLineChanges++;
			lineChangeNeeded = false;
			lci = null;
			}

		/// <summary>
		/// schließt den Spurwechsel ab
		/// </summary>
		/// <param name="arcPositionOffset">Bogenlänge über die das Fahrzeug bereits über die Länge des LCP hinaus ist</param>
		private void FinishLineChange(double arcPositionOffset)
			{
			_state.position = currentLineChangePoint.target.arcPosition + arcPositionOffset;
			lastLineChangeCheck.Left = GlobalTime.Instance.currentTime;
			lastLineChangeCheck.Right = currentPosition;

			//m_WayToGo = CalculateShortestConenction(currentNodeConnection.endNode, m_TargetNodes);

			currentlyChangingLine = false;

			lineChangeNeeded = false;
			lci = null;
			_physics.multiplierTargetVelocity = 1;

			// Tell other vehicle that waits for me, that I'm finished. Kinda redundant, but safe is safe.
			_state.UnsetLineChangeVehicleInteraction();
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
		protected double _length = 40;
		/// <summary>
		/// Länge des Fahrzeuges
		/// </summary>
		public double length
			{
			get { return _length; }
			set { _length = value; }
			}

		/// <summary>
		/// Farbe des Autos
		/// </summary>
		protected Color _color = Color.Black;
		/// <summary>
		/// Farbe des Autos
		/// </summary>
		public Color color
			{
			get { return _color; }
			set { _color = value; _fillBrush = new SolidBrush(_color); }
			}

		/// <summary>
		/// Color map used for velocity color mapping
		/// </summary>
		public static Tools.Colormap _colormap;

		/// <summary>
		/// Solid brush for filling the vehicle during rendering. Appropriate color has to be set during rendering process.
		/// </summary>
		private SolidBrush _fillBrush = new SolidBrush(Color.Black);

		/// <summary>
		/// Pen for drawing the vehicle outline during rendering. Appropriate color has to be set during rendering process.
		/// </summary>
		private Pen _outlinePen = new Pen(Color.Red, 2.0f);

		/// <summary>
		/// Physik des Fahrzeuges
		/// </summary>
		protected IVehicle.Physics _physics;
		/// <summary>
		/// Physik des Fahrzeuges
		/// </summary>
        public IVehicle.Physics physics
            {
            get { return _physics; }
			set { _physics = value; }
            }

		/// <summary>
		/// Target Velocity of this vehicle.
		/// Currently only a shortcut to the target velocity of the current NodeConnection of this vehicle.
		/// </summary>
		public double targetVelocity
			{
			get 
				{
				return (currentNodeConnection != null) ? Math.Min(_physics.targetVelocity, currentNodeConnection.targetVelocity) : _physics.targetVelocity; 
				}
			}

		/// <summary>
		/// effektive gewünschte Gecshwindigkeit des Autos (mit Multiplikator multipliziert)
		/// </summary>
		public double effectiveDesiredVelocity
			{
			get { return targetVelocity * _physics.multiplierTargetVelocity; }
			}

		/// <summary>
		/// aktueller State des Fahrezeuges
		/// </summary>
        protected IVehicle.State _state;
		/// <summary>
		/// aktueller State des Fahrezeuges
		/// </summary>
        public IVehicle.State state
            {
            get { return _state; }
			set { _state = value; }
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
		/// checks whether this vehicle is currently stopped
		/// </summary>
		public bool isStopped = false;

		/// <summary>
		/// The stop sign a vehicle shall ignore because, it did already stop in front of it.
		/// This is a kinda lazy implementation for stop sign handling. Won't work in the (rare) case that a vehicle passes the same stop sign directly twice.
		/// </summary>
		private LineNode _stopSignToIgnore = null;

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
		public LinkedList<NodeConnection> visitedNodeConnections = new LinkedList<NodeConnection>();


		/// <summary>
		/// Fahrzeug berechnet neue Beschleunigungswerte für die aktuelle Position auf der auktuellen NodeConnection 
		/// </summary>
		/// <param name="tickLength">Länge eines Ticks in Sekunden</param>
		public void Think(double tickLength)
			{
			List<NodeConnection> route = new List<NodeConnection>();
			route.Add(currentNodeConnection);
			foreach (Routing.RouteSegment rs in wayToGo)
				route.Add(rs.startConnection);

			double acceleration = Think(route, currentPosition, false, tickLength);
			Accelerate(acceleration);
			//Think(currentNodeConnection, currentPosition, false, tickLength);
			}

		/// <summary>
		/// Calculates driving parameters and new acceleration of the vehicle.
		/// </summary>
		/// <param name="route">Route of the Vehicle.</param>
		/// <param name="arcPos">Current arc position of the vehicle on the first NodeConnection on <paramref name="route"/>.</param>
		/// <param name="onlySimpleCalculations">Perform only simple calculations (e.g. no free line changes).</param>
		/// <param name="tickLength">Length of a tick in seconds.</param>
		/// <returns></returns>
		public double Think(List<NodeConnection> route, double arcPos, bool onlySimpleCalculations, double tickLength)
			{
			if (route.Count == 0)
				return 0;

			double lookaheadDistance = Constants.lookaheadDistance;
			double intersectionLookaheadDistance = Constants.intersectionLookaheadDistance;
			double stopDistance = -1;
			_state._freeDrive = true;

			bool thinkAboutLineChange = false;
			double lowestAcceleration = 0;

			#region LineChangeVehicleInteraction

			// if necessary, wait for other vehicle to change line
			if (_state.letVehicleChangeLine)
				{
				double percentOfLCILeft = (lci == null) ? 0.2 : Math.Max(0.2, (lci.endArcPos - currentPosition - Constants.breakPointBeforeForcedLineChange) / (lci.length - Constants.breakPointBeforeForcedLineChange));
				lookaheadDistance = Math.Max(3 * percentOfLCILeft * s0, _state.tailPositionOfOtherVehicle - currentPosition);
				thinkAboutLineChange = false;
				lowestAcceleration = CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, lookaheadDistance, physics.velocity);
				_state._freeDrive = false;
				}

			#endregion

			#region Vehicles in front

			// Determine the next vehicle in front.
			VehicleDistance theVehicleInFrontOfMe = GetNextVehicleOnMyTrack(route[0], arcPos, lookaheadDistance);

			// The stored distance is to the front of the vehicle. All following calculations need the distance
			// to its tail. Hence, substract the vehicle length.
			if (theVehicleInFrontOfMe != null && theVehicleInFrontOfMe.distance < lookaheadDistance)
				{
				lookaheadDistance = theVehicleInFrontOfMe.distance;
				thinkAboutLineChange = true;
				lowestAcceleration = CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, theVehicleInFrontOfMe.distance, physics.velocity - theVehicleInFrontOfMe.vehicle.physics.velocity);
				if (lowestAcceleration < 0.1)
					_state._freeDrive = false;

				if (    (theVehicleInFrontOfMe.vehicle.physics.velocity < 2.5)
					 || (theVehicleInFrontOfMe.vehicle.physics.velocity < 5 && theVehicleInFrontOfMe.vehicle.physics.acceleration < 0.1))
					{
					stopDistance = theVehicleInFrontOfMe.distance;
					}
				}
			else
				{
				lowestAcceleration = CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, lookaheadDistance, physics.velocity);
				}


			#endregion

			#region Stop Signs

			Pair<LineNode, double> nextStopSign = GetDistanceToNextStopSignOnRoute(route, arcPos,lookaheadDistance);
			if (nextStopSign.Left != null && nextStopSign.Left != _stopSignToIgnore)
				{
				if (isStopped)
					{
					_stopSignToIgnore = nextStopSign.Left;
					}
				else
					{
					lookaheadDistance = nextStopSign.Right;
					thinkAboutLineChange = false;
					lowestAcceleration = CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, nextStopSign.Right, physics.velocity);
					_state._freeDrive = false;
					}
				}

			#endregion

			#region Traffic lights

			// Check for red traffic lights on route
			double distanceToTrafficLight = GetDistanceToNextTrafficLightOnRoute(route, arcPos, Constants.lookaheadDistance, true);
			intersectionLookaheadDistance = distanceToTrafficLight;

			// If the next TrafficLight is closer than the next vehicle, no free line change shall be performed
			if (distanceToTrafficLight < lookaheadDistance)
				{
				lookaheadDistance = distanceToTrafficLight;
				thinkAboutLineChange = false;
				lowestAcceleration = CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, lookaheadDistance, physics.velocity);
				_state._freeDrive = false;
				}
			
			#endregion

			#region Intersections

			// Registration target for intersections.
			// (When doing simple calculations, we do not want to unregister at all intersections. Hence, we use a temporary regsitration):
			LinkedList<SpecificIntersection> registrationTarget = (onlySimpleCalculations ? temporaryRegisteredIntersections : registeredIntersections);

			// gather all upcoming intersections (and update the ones we are already registered at)
			GatherNextIntersectionsOnMyTrack(route, arcPos, registrationTarget, intersectionLookaheadDistance);
			double distanceToIntersection = HandleIntersections(registrationTarget, stopDistance);

			// If there is an intersection where I should wait, I should do so...
			if (!Double.IsPositiveInfinity(distanceToIntersection))// && distanceToIntersection < lookaheadDistance)
				{
				lookaheadDistance = distanceToIntersection;
				double newAcceleration = CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, distanceToIntersection, physics.velocity);
				lowestAcceleration = Math.Min(lowestAcceleration, newAcceleration);
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
				if (lineChangeNeeded && !currentlyChangingLine && lci != null)
					{
					thinkAboutLineChange = false;
					lastLineChangeCheck.Left = GlobalTime.Instance.currentTime;
					lastLineChangeCheck.Right = currentPosition;

					// get current LineChangePoint and check, whether it's leading to our target
					NodeConnection.LineChangePoint lcp = route[0].GetPrevLineChangePoint(arcPos);
					if (lci.targetNode.prevConnections.Contains(lcp.target.nc))
						{
						bool slowDownToBreakPoint = false;
						double myArcPositionOnOtherConnection = lcp.otherStart.arcPosition + (arcPos - lcp.start.arcPosition);

						// check if found LineChangePoint is not too far away to perform the line change
						if ((myArcPositionOnOtherConnection >= 0) && (Math.Abs(arcPos - lcp.start.arcPosition) < Constants.maxDistanceToLineChangePoint * 1.25))
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
										? CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, otherVehicles.Right.vehicle.currentPosition - myArcPositionOnOtherConnection, physics.velocity - otherVehicles.Right.vehicle.physics.velocity)
										: CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, lookaheadDistance, physics.velocity);

									// calculate the necessary acceleration of the vehicle behind in case of a line change
									double forcedAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null)
										? CalculateAcceleration(otherVehicles.Left.vehicle.physics.velocity, otherVehicles.Left.vehicle.effectiveDesiredVelocity, otherVehicles.Left.distance, otherVehicles.Left.vehicle.physics.velocity - physics.velocity)
										: 0;

									double currentAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null) ? otherVehicles.Left.vehicle.physics.acceleration : 0;

									// Final check:
									//  - The new vehicle behind must not break harder than bSave
									//  - My line change must be sufficiently necessary. The closer I come to the end of the LineChangeInterval, the more I may thwart the vehicle behind. 
									if (   (forcedAccelerationOfVehicleBehindMeOnOtherConnection > bSave)
										&& ((arcPos - lci.startArcPos) / lci.length >= (currentAccelerationOfVehicleBehindMeOnOtherConnection - forcedAccelerationOfVehicleBehindMeOnOtherConnection)))
										{
										// return to normal velocity
										_physics.multiplierTargetVelocity = 1;

										// initiate the line change
										InitiateLineChange(lcp, arcPos - lcp.start.arcPosition);
										lowestAcceleration = myAccelerationOnOtherConnection;
										}
									// I do not want to change line yet, but I could position myself better between the two other vehicles on the parallel line.
									else if (true)
										{
										// TODO: implement
										slowDownToBreakPoint = true;
										}
									}
								// the new vehicle behind would too close but I can accelerate and are at least as fast as him
								else if (   otherVehicles.Left.vehicle.physics.velocity / this.physics.velocity < 1.2  // I am not significantly slower than him
										 && (otherVehicles.Right == null || otherVehicles.Right.distance > 1.5 * length) // the new vehicle in front is far enough away
										 && lookaheadDistance > 2 * length			// no vehicle/traffic light/intersection in front
										 && lci.endArcPos - arcPos > 2 * length		// enough space left in LineChangeInterval
										 && lowestAcceleration >= -0.1)				// currently not braking
									{
									// accelerate to get in front
									_physics.multiplierTargetVelocity = 1.75;
									lowestAcceleration = Math.Min(lowestAcceleration, CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, lookaheadDistance, physics.velocity));

									_state.SetLineChangeVehicleInteraction(this, otherVehicles.Left.vehicle, lcp.otherStart.nc, myArcPositionOnOtherConnection - _length);
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
							double percentOfLCILeft = Math.Max(0.2, (lci.endArcPos - currentPosition - Constants.breakPointBeforeForcedLineChange) / (lci.length - Constants.breakPointBeforeForcedLineChange));

							// slow down a bit
							_physics.multiplierTargetVelocity = Math.Min(0.9, 1.5 * percentOfLCILeft);

							// When reaching the end of the LineChangeInterval, check whether there are other possibilities to reach the target:
							if (percentOfLCILeft < 0.5)
								{
								Routing newRTT = Routing.CalculateShortestConenction(route[0].endNode, _targetNodes, _vehicleType);
								// The alternative route does not cost too much -> choose it
								if (newRTT.SegmentCount() > 0 && newRTT.costs / _wayToGo.costs < Constants.maxRatioForEnforcedLineChange)
									{
									_wayToGo = newRTT;
									_physics.multiplierTargetVelocity = 1;
									lineChangeNeeded = false;
									lci = null;
									}
								}
							// Line change still necessacy => stop at break point
							if (lineChangeNeeded)
								{
								if (! _state.letVehicleChangeLine && percentOfLCILeft < 0.8)
									{
									VehicleDistance otherBehind = lcp.otherStart.nc.GetVehicleBeforeArcPosition(myArcPositionOnOtherConnection - ((_length + s0)), Constants.lookaheadDistance);

									// In rare cases deadlocks may appear when a very long and a short vehicle are driving parallel to each other (and both want to change line):
									// Then, none of the two finds a vehicle behind on the parallel connection. Hence, none of the two will wait for the other one
									// and both might drive to the end of the line change interval and there block each other.
									// To avoid this case, if there is no otherBehind, we also look for a parallel vehicle in front of our back (there should be one, otherwise
									// something went terribly wrong above). The longer one of the two will wait for the shorter one to make sure, no deadlock will occur.
									if (otherBehind == null)
										{
										VehicleDistance otherFront = lcp.otherStart.nc.GetVehicleBehindArcPosition(myArcPositionOnOtherConnection - ((_length + s0)), Constants.lookaheadDistance);
										if (otherFront.vehicle.lineChangeNeeded && otherFront.vehicle._length > _length)
											{
											otherBehind = otherFront;
											otherBehind.distance *= -1;
											}
										}

									//Pair<VehicleDistance> vd = lcp.otherStart.nc.GetVehiclesAroundArcPosition(myArcPositionOnOtherConnection - ( (_length + s0)), Constants.lookaheadDistance);
									if (otherBehind != null)// && otherBehind.vehicle.p >= p)
										{
										// tell the vehicle behind my back to wait for me
										_state.SetLineChangeVehicleInteraction(this, otherBehind.vehicle, lcp.otherStart.nc, myArcPositionOnOtherConnection - _length);

										// In addition, I need to get behind the vehicle in front of the vehicle which waits for me. Therefore I adapt the desired velocity
										if (_state.vehicleThatLetsMeChangeLine != null)
											{
											VehicleDistance otherBehindForman = _state.vehicleThatLetsMeChangeLine.currentNodeConnection.GetVehicleBehindArcPosition(_state.vehicleThatLetsMeChangeLine.currentPosition, 2 * (length + s0));
											if (otherBehindForman != null)
												{
												//_physics.multiplierTargetVelocity = Math2.Clamp(Math2.Cubic((otherBehindForman.distance - otherBehind.distance - s0) / (_length + 4 * s0)), 0.3, 1);
												double multPerDistance = 1 - Math2.Clamp((otherBehind.distance + _length + s0 - otherBehindForman.distance + otherBehindForman.vehicle._length + s0) / (otherBehindForman.vehicle._length), 0.2, 0.75);
												double multPerSpeedDiff = Math2.Clamp((otherBehindForman.vehicle._physics.velocity - _physics.velocity) / 2, 0.25, 0.8);
												_physics.multiplierTargetVelocity = Math.Min(multPerDistance, multPerSpeedDiff);
												}
											}
										}
									}

								lowestAcceleration = Math.Min(lowestAcceleration, CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, lci.endArcPos - Constants.breakPointBeforeForcedLineChange - arcPos, physics.velocity));
								}
							}
						}
					}
				else if (_state.vehicleThatLetsMeChangeLine != null)
					{
					_state.UnsetLineChangeVehicleInteraction();
					}

				#endregion

				#region freiwillig

				thinkAboutLineChange &= ((GlobalTime.Instance.currentTime - lastLineChangeCheck.Left > 1) || (currentPosition - lastLineChangeCheck.Right > 50));

				if (thinkAboutLineChange && !currentlyChangingLine)
					{
					lastLineChangeCheck.Left = GlobalTime.Instance.currentTime;
					lastLineChangeCheck.Right = currentPosition;

					// get current LineChangePoint and check, whether it's reachable
					NodeConnection.LineChangePoint lcp = route[0].GetPrevLineChangePoint(arcPos);
					if ((lcp.target.nc != null) && (Math.Abs(arcPos - lcp.start.arcPosition) < Constants.maxDistanceToLineChangePoint * 0.67))
						{
						// check whether there is an alternative route that is not too costly
						Routing alternativeRoute = Routing.CalculateShortestConenction(lcp.target.nc.endNode, targetNodes, _vehicleType);
						if (alternativeRoute.SegmentCount() > 0 && alternativeRoute.costs / wayToGo.costs < Constants.maxRatioForVoluntaryLineChange && !alternativeRoute.Top().lineChangeNeeded)
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
										foreach (Routing.RouteSegment rs in alternativeRoute)
											l.Add(rs.startConnection);

										// calculate my necessary acceleration in case of a line change
										double myAccelerationOnOtherConnection = (otherVehicles.Right != null)
											? CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, otherVehicles.Right.vehicle.currentPosition - myArcPositionOnOtherConnection, physics.velocity - otherVehicles.Right.vehicle.physics.velocity)
											: CalculateAcceleration(physics.velocity, effectiveDesiredVelocity, GetDistanceToNextTrafficLightOnRoute(l, myArcPositionOnOtherConnection, Constants.lookaheadDistance, true), physics.velocity);

										// calculate the necessary acceleration of the vehicle behind in case of a line change
										double forcedAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null)
											? CalculateAcceleration(otherVehicles.Left.vehicle.physics.velocity, otherVehicles.Left.vehicle.effectiveDesiredVelocity, otherVehicles.Left.distance, otherVehicles.Left.vehicle.physics.velocity - physics.velocity)
											: 0;

										double currentAccelerationOfVehicleBehindMeOnOtherConnection = (otherVehicles.Left != null) ? otherVehicles.Left.vehicle.physics.acceleration : 0;

										// simplified implementation of MOBIL: http://www.vwi.tu-dresden.de/~treiber/MicroApplet/MOBIL.html
										if (forcedAccelerationOfVehicleBehindMeOnOtherConnection > bSave)
											{
											if (myAccelerationOnOtherConnection - lowestAcceleration > p * (currentAccelerationOfVehicleBehindMeOnOtherConnection - forcedAccelerationOfVehicleBehindMeOnOtherConnection) + lineChangeThreshold)
												{
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
		public void Move(double tickLength)
            {
			if (!alreadyMoved)
				{
				_physics.velocity += physics.acceleration;

				// Rückwärts fahren geht nicht
				if (_physics.velocity < 0)
					_physics.velocity = 0;

				double arcLengthToMove = (physics.velocity * tickLength * 10);

				if (_physics.velocity < 0.1)
					{
					if (!isStopped)
						{
						++_statistics.numStops;
						}
					isStopped = true;
					}
				else
					{
					isStopped = false;
					}

				// wenn ich gerade am Spurwechseln bin, sollte ich das erstmal behandeln
				if (currentlyChangingLine)
					{
					currentPositionOnLineChangePoint += arcLengthToMove; // ich bewege mich echt auf dem LCP
					_state.position += arcLengthToMove * ratioProjectionOnTargetConnectionvsLCPLength; // ich muss meine Position auf der Ziel-NodeConnection entsprechend anpassen
					}
				else
					{
					_state.position += arcLengthToMove;
					}

				// wenn meine aktuelle NodeConnection zu Ende ist, sollte ich das auch behandeln
				if (currentPosition > currentNodeConnection.lineSegment.length)
					{
					// gucken, ob es mit ner Connection weitergeht
					if ((currentNodeConnection.endNode.nextConnections.Count != 0) && (wayToGo.SegmentCount() > 0))
						{
						_physics.multiplierTargetVelocity = 1;
						_state.UnsetLineChangeVehicleInteraction();

						double startDistance = (currentPosition - currentNodeConnection.lineSegment.length);

						// falls ich mehrere Connections zur Auswahl habe, berechne die mit dem kürzesten Weg
						// (dieser könnte sich geändert haben, weil dort plötzlich mehr Autos fahren)
						if (currentNodeConnection.endNode.nextConnections.Count > 1)
							{
							_wayToGo = Routing.CalculateShortestConenction(currentNodeConnection.endNode, targetNodes, _vehicleType);
							if (_wayToGo.SegmentCount() == 0 || _wayToGo.Top() == null)
								{
								RemoveFromCurrentNodeConnection(true, null, 0);
								return;
								}
							}

						visitedNodeConnections.AddFirst(currentNodeConnection);

						// nächsten Wegpunkt extrahieren
						Routing.RouteSegment rs = wayToGo.Pop();

						if (rs == null)
							{
							RemoveFromCurrentNodeConnection(true, null, 0);
							return;
							}
						else
							{
							// ist ein Spurwechsel nötig, so die entsprechenden Felder füllen
							if (rs.lineChangeNeeded)
								rs.startConnection.lineChangeIntervals.TryGetValue(rs.nextNode.hashcode, out lci);
							else
								lci = null;
							lineChangeNeeded = (lci != null);

							LinkedListNode<IVehicle> lln = rs.startConnection.GetVehicleListNodeBehindArcPosition(startDistance);
							if (lln == null || lln.Value.currentPosition - lln.Value.length >= startDistance)
								{
								RemoveFromCurrentNodeConnection(true, rs.startConnection, startDistance);
								}
							else
								{
								RemoveFromCurrentNodeConnection(true, null, 0);
								}
							}
						}
					else
						{
						// Ende der Fahnenstange, also selbstzerstören
						RemoveFromCurrentNodeConnection(true, null, 0);
						}
					}
				else if (Double.IsNaN(currentPosition))
					{
					RemoveFromCurrentNodeConnection(false, null, 0);
					}

				// Der Spurwechsel ist fertig, dann sollte ich diesen auch abschließen:
				if (currentlyChangingLine && currentPositionOnLineChangePoint >= currentLineChangePoint.lineSegment.length)
					{
					FinishLineChange(currentPositionOnLineChangePoint - currentLineChangePoint.lineSegment.length);

					_statistics.startTimeOnNodeConnection = GlobalTime.Instance.currentTime;
					_statistics.arcPositionOfStartOnNodeConnection = _state.position;
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
            _physics.acceleration = newAcceleration;
            }
        #endregion


		#region Bewegungsberechnungen

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
			double arcLengthToLookForParallelVehicles = 1.0 * nc.startNode.outSlope.Abs; // search distance on parallel connections
			foreach (NodeConnection parallelNodeConnection in nc.startNode.nextConnections)
				{
				VehicleDistance vd = parallelNodeConnection.GetVehicleBehindArcPosition(arcPos, distance);
				if (vd != null && vd.distance > 0)
					{
					vd.distance -= vd.vehicle.length;

					if (   (toReturn == null || vd.distance < toReturn.distance) 
						&& (parallelNodeConnection == nc || (arcPos < arcLengthToLookForParallelVehicles && vd.distance < arcLengthToLookForParallelVehicles)))
						{
						toReturn = vd;
						}
					}
				}

			return toReturn;
			}

		/// <summary>
		/// Searches for the next LineNode with a stop sign on the vehicle's route within the given distance.
		/// </summary>
		/// <param name="route">Route of the Vehicle.</param>
		/// <param name="arcPos">Current arc position of the vehicle on the first NodeConnection on <paramref name="route"/>.</param>
		/// <param name="distance">Distance to cover during search.</param>
		/// <returns>Pair of the next LineNode with a stop sign on the vehicle's route that covers the given constraints and the distance to there. Pair of null and <paramref name="distance"/> if no such LineNode exists.</returns>
		private Pair<LineNode, double> GetDistanceToNextStopSignOnRoute(List<NodeConnection> route, double arcPos, double distance)
			{
			Debug.Assert(route.Count > 0);

			double doneDistance = -arcPos;
			foreach (NodeConnection nc in route)
				{
				doneDistance += nc.lineSegment.length;
				if (doneDistance >= distance)
					return new Pair<LineNode, double>(null, distance);

				if (nc.endNode.stopSign)
					return new Pair<LineNode, double>(nc.endNode, doneDistance);
				}

			return new Pair<LineNode, double>(null, distance);
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
		private void GatherNextIntersectionsOnMyTrack(List<NodeConnection> route, double arcPos, LinkedList<SpecificIntersection> intersectionRegistration, double distance)
			{
			Debug.Assert(route.Count > 0);
			List<NodeConnection> workingRoute;
			LinkedListNode<SpecificIntersection> lln = intersectionRegistration.First;				// current already registered intersection
			double doneDistance, remainingDistance, startPosition;

			if (arcPos < _length && visitedNodeConnections.Count > 0)
				{
				workingRoute = new List<NodeConnection>(route.Count + 1);
				workingRoute.Add(visitedNodeConnections.First.Value);
				workingRoute.AddRange(route);

				// Setup some helper variables:
				doneDistance = -workingRoute[0].lineSegment.length - currentPosition;				// total covered distance
				remainingDistance = distance + length;												// total remaining distance to cover
				startPosition = workingRoute[0].lineSegment.length - (length - currentPosition);	// start position at current NodeConnection
				}
			else
				{
				workingRoute = route;

				// Setup some helper variables:
				doneDistance = -currentPosition;									// total covered distance
				remainingDistance = distance + length;								// total remaining distance to cover
				startPosition = currentPosition - length;							// start position at current NodeConnection
				}
			
			// search each NodeConnection
			foreach (NodeConnection nc in workingRoute)
				{
				// gather all intersections on current nc
				List<SpecificIntersection> l = nc.GetSortedIntersectionsWithinArcLength(new Interval<double>(startPosition, startPosition + remainingDistance));

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
						si.intersection.UpdateVehicle(this, si.nodeConnection, myDistance, GlobalTime.Instance.currentTime);
						lln.Value = new SpecificIntersection(si.nodeConnection, si.intersection);
						lln = lln.Next;
						}
					// the current intersection is not yet registered - do it now
					else
						{
						si.intersection.RegisterVehicle(this, si.nodeConnection, myDistance, GlobalTime.Instance.currentTime);
						if (lln != null)
							intersectionRegistration.AddBefore(lln, new SpecificIntersection(si.nodeConnection, si.intersection));
						else
							intersectionRegistration.AddLast(new SpecificIntersection(si.nodeConnection, si.intersection));
						}
					}

				// continue with next NodeConnection on workingRoute
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
				bool avoidBlocking = true;

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
					if ((stopPoint > 0) && (stopPoint - length - s0 < myCvt.remainingDistance) && (si.intersection.avoidBlocking))
						{
						waitInFrontOfIntersection = true;
						}
					}
				// Both NodeConnections have the same priority
				else if (otherNC.priority == si.nodeConnection.priority)
					{
					// Intersection is close to stop point so that vehicle will block this intersection => wait in front
					if ((stopPoint > 0) && (stopPoint - length - s0 < myCvt.remainingDistance) && (si.intersection.avoidBlocking))
						{
						waitInFrontOfIntersection = true;
						}

					// check at each intersection, which vehicle was there first
					foreach (CrossingVehicleTimes otherCvt in cvtList)
						{						
						// I should wait if:
						//  - The other vehicle originally reached the intersection before me
						//  - TODO: I would block him significantly if I continue.
						if (myCvt.originalArrivingTime > otherCvt.originalArrivingTime || otherCvt.remainingDistance < 0)
							{
							waitInFrontOfIntersection = true;
							break;
							}
						}
					}
				// My Nodeconnection is more important than the other one
				else
					{
					// If the other vehicle hasn't reached the intersection yet, nothing is to do here.
					// Above is ensured, that the other vehicle will wait (hopefully, it does... o_O)

					// If otherwise the other vehicle is already blocking the intersection, I'm doing good in waiting in front of it.
					foreach (CrossingVehicleTimes otherCvt in cvtList)
						{
						if (otherCvt.remainingDistance < 0)
							{
							waitInFrontOfIntersection = true;
							avoidBlocking = false;
							break;
							}
						}
					}

				if (waitInFrontOfIntersection)
					{
					if (avoidBlocking)
						{
						// first go backwards: If I wait before an intersection I might block other intersections before.
						// This is usually not wanted, therefore go backwards and wait in front of the first intersection 
						// where I won't block any other.
						double distanceToLookBack = si.intersection.GetCrossingVehicleTimes(this, si.nodeConnection).remainingDistance - _length - s0;
						while (lln.Previous != null)
							{
							double remainingDistanceToPrevIntersection = lln.Previous.Value.intersection.GetCrossingVehicleTimes(this, lln.Previous.Value.nodeConnection).remainingDistance;

							// check whether intersection will be blocked
							if (remainingDistanceToPrevIntersection > 0 && remainingDistanceToPrevIntersection + lln.Previous.Value.intersection._rearWaitingDistance > distanceToLookBack)
								{
								// intersection will be blocked and both NodeConnections from the intersection do not originate from the same node 
								// => wait in front of it and continue looking backwards
								if (lln.Previous.Value.intersection.avoidBlocking)
									{
									distanceToLookBack = remainingDistanceToPrevIntersection - _length - s0;
									lln = lln.Previous;
									}
								// intersection will be blocked but both NodeConnections from the intersectino originate from the same LineNode
								else
									{
									// this intersection may be blocked => nothing to do here
									lln = lln.Previous;
									}
								}
							else
								{
								// will not be blocked => no further search necessary
								break;
								}
							}

						si = lln.Value; // si is a Value-Type (copy)*/
						}

					// Update this and all following intersections, that I won't cross in the near future.
					while (lln != null)
						{
						lln.Value.intersection.UpdateVehicle(this, lln.Value.nodeConnection, true);
						lln = lln.Next;
						}

					return si.intersection.GetCrossingVehicleTimes(this, si.nodeConnection).remainingDistance - si.intersection._frontWaitingDistance; // si is a Value-Type (copy)
					}
				else
					{
					lln.Value.intersection.UpdateVehicle(this, lln.Value.nodeConnection, false);
					}

				lln = lln.Next;
				}

			return Double.PositiveInfinity;
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

			// distance is in dm, velocity in m/s. For easier calculations, we transform the distance unit to meters.
			distance /= 10;

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
				double effDesVel = effectiveDesiredVelocity;

				while (alreadyCoveredDistance <= distance)
					{
					currentVelocity += CalculateAcceleration(currentVelocity, effDesVel);
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
		private List<LineNode> _targetNodes;
		/// <summary>
		/// Der Ort, zu dem das Fahrzeug hin will
		/// </summary>
		public List<LineNode> targetNodes
			{
			get { return _targetNodes; }
			set 
				{ 
				_targetNodes = value;
				_wayToGo = Routing.CalculateShortestConenction(currentNodeConnection.endNode, _targetNodes, _vehicleType);
				if (_wayToGo.SegmentCount() == 0)
					{
					RemoveFromCurrentNodeConnection(false, null, 0);
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
                targetVelocity = d;
                velocity = v;
                acceleration = a;
				multiplierTargetVelocity = 1;
                }

			/// <summary>
			/// gewünschte Gecshwindigkeit des Autos 
			/// </summary>
			public double targetVelocity;

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
			public double multiplierTargetVelocity;
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
				m_letVehicleChangeLine = false;
				m_tailPositionOfOtherVehicle = 0;
				m_vehicleThatLetsMeChangeLine = null;
				m_vehicleToChangeLine = null;
				_freeDrive = true;
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
				m_letVehicleChangeLine = false;
				m_tailPositionOfOtherVehicle = 0;
				m_vehicleThatLetsMeChangeLine = null;
				m_vehicleToChangeLine = null;
				_freeDrive = true;
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

			/// <summary>
			/// Flag whether this vehicle can drive freely or is obstructed (traffic light, slower vehicle in front)
			/// </summary>
			public bool _freeDrive;

			#region LineChangeVehicleInteraction

			/// <summary>
			/// Flag whether I shall wait behind the other vehicle to let him change line
			/// </summary>
			private bool m_letVehicleChangeLine;
			/// <summary>
			/// Flag whether I shall wait behind the other vehicle to let him change line
			/// </summary>
			public bool letVehicleChangeLine
				{
				get { return m_letVehicleChangeLine; }
				}

			/// <summary>
			/// Arc position of other vehicle projected on my NodeConnection (I should not go beyond this point if I want to let him in).
			/// </summary>
			private double m_tailPositionOfOtherVehicle;
			/// <summary>
			/// Arc position of other vehicle projected on my NodeConnection (I should not go beyond this point if I want to let him in).
			/// </summary>
			public double tailPositionOfOtherVehicle
				{
				get { return m_tailPositionOfOtherVehicle; }
				set { m_tailPositionOfOtherVehicle = value; }
				}

			/// <summary>
			/// Used by the line changing vehicle: Reference to vehicle, that waits for me to change line.
			/// </summary>
			private IVehicle m_vehicleThatLetsMeChangeLine;
			/// <summary>
			/// Used by the line changing vehicle: Reference to vehicle, that waits for me to change line.
			/// </summary>
			public IVehicle vehicleThatLetsMeChangeLine
				{
				get { return m_vehicleThatLetsMeChangeLine; }
				}

			/// <summary>
			/// Used by the waiting vehicle: Reference to the vehicle, that wants to change line
			/// </summary>
			private IVehicle m_vehicleToChangeLine;
			/// <summary>
			/// Used by the waiting vehicle: Reference to the vehicle, that wants to change line
			/// </summary>
			public IVehicle vehicleToChangeLine
				{
				get { return m_vehicleToChangeLine; }
				}


			/// <summary>
			/// Resets the LineChangeVehicleInteraction: If another vehicle is waiting for me to change line, it will not continue to do so.
			/// </summary>
			public void UnsetLineChangeVehicleInteraction()
				{
				if (m_vehicleThatLetsMeChangeLine != null)
					{
					m_vehicleThatLetsMeChangeLine._state.m_letVehicleChangeLine = false;
					m_vehicleThatLetsMeChangeLine._state.m_vehicleToChangeLine = null;
					}
					
				m_vehicleThatLetsMeChangeLine = null;
				}

			/// <summary>
			/// Sets the LineChangeVehicleInteraction: otherVehicle will be told to wait for me to change line. Therefore, it will try to keep behind myTailPositionOnTargetConnection.
			/// </summary>
			/// <param name="lineChangingVehicle">vehicle, that wants to change line</param>
			/// <param name="otherVehicle">vehicle on the target connection, that will wait for me to change line</param>
			/// <param name="targetConnection">Target NodeConnection</param>
			/// <param name="myTailPositionOnTargetConnection">My projected tail position on targetConnection. otherVehicle will try to keep behind this arc position.</param>
			public void SetLineChangeVehicleInteraction(IVehicle lineChangingVehicle, IVehicle otherVehicle, NodeConnection targetConnection, double myTailPositionOnTargetConnection)
				{
				UnsetLineChangeVehicleInteraction();

				if (otherVehicle.currentNodeConnection == targetConnection && myTailPositionOnTargetConnection >= 0)
					{
					// Make sure that no two LineChangeVehicleInteractions cross each other:
					// Therefore, we here check die vehicles on the two NodeConnections of the LineChangePoint. This simplifies the algorithm and should be enough here.
					LinkedListNode<IVehicle> myNode = lineChangingVehicle.listNode.Previous;
					while (myNode != null)
						{
						// we have found a vehicle (call it vehicle B) behind me waiting for another vehicle (call it vehicle C) changing line
						if (myNode.Value._state.letVehicleChangeLine)
							{
							// check whether vehicle C is in front of the vehicle, that will wait for me
							if (myNode.Value._state.vehicleToChangeLine.currentPosition > otherVehicle.currentPosition)
								{
								// We have found two LineChangeVehicleInteraction crossing each other. To solve the problem, simply let vehicle C wait for me.
								otherVehicle = myNode.Value._state.vehicleToChangeLine;
								break;
								}
							}
						myNode = myNode.Previous;
						}

					m_vehicleThatLetsMeChangeLine = otherVehicle;
					otherVehicle._state.m_letVehicleChangeLine = true;
					otherVehicle._state.m_tailPositionOfOtherVehicle = myTailPositionOnTargetConnection;
					otherVehicle._state.m_vehicleToChangeLine = lineChangingVehicle;
					}
				else
					{
					// TODO: think about s.th. clever to do in this case. :)
					}
				}


			#endregion
			}
        #endregion

		/// <summary>
		/// Stack von den noch zu besuchenden NodeConnections
		/// </summary>
		private Routing _wayToGo;
		/// <summary>
		/// Stack von den noch zu besuchenden NodeConnections
		/// </summary>
		public Routing wayToGo
			{
			get { return _wayToGo; }
			}

		/// <summary>
		/// Type of this very vehicle
		/// </summary>
		protected VehicleTypes _vehicleType;

		/// <summary>
		/// Generates the GraphicsPath for rendering the vehicle. Should be overloaded by subclasses with distinct rendering.
		/// </summary>
		/// <returns>A GraphicsPath in world coordinates for rendering the vehicle at the current position.</returns>
		protected virtual GraphicsPath BuildGraphicsPath()
			{
			GraphicsPath toReturn = new GraphicsPath();
			if (!currentlyChangingLine)
				{
				Vector2 direction = state.orientation;
				if (!direction.IsZeroVector())
					{
					Vector2 orientation = direction.Normalized;
					Vector2 normal = direction.RotatedClockwise.Normalized;

					PointF[] ppoints = 
						{ 
						state.positionAbs  -  8 * normal,
						state.positionAbs  +  8 * normal,
						state.positionAbs  -  length * orientation  +  8 * normal,
						state.positionAbs  -  length * orientation  -  8 * normal,
						};
					toReturn.AddPolygon(ppoints);
					}
				}
			else
				{
				Vector2 positionOnLcp = currentLineChangePoint.lineSegment.AtPosition(currentPositionOnLineChangePoint);
				Vector2 derivate = currentLineChangePoint.lineSegment.DerivateAtTime(currentLineChangePoint.lineSegment.PosToTime(currentPositionOnLineChangePoint));
				if (!derivate.IsZeroVector())
					{
					Vector2 orientation = derivate.Normalized;
					Vector2 normal = derivate.RotatedClockwise.Normalized;
					PointF[] ppoints = 
						{ 
						positionOnLcp  -  8 * normal,
						positionOnLcp  +  8 * normal,
						positionOnLcp  -  length * orientation  +  8 * normal,
						positionOnLcp  -  length * orientation  -  8 * normal,
						};
					toReturn.AddPolygon(ppoints);
					}
				}
			return toReturn;
			}

		/// <summary>
		/// Zeichnet das Vehicle auf der Zeichenfläche g
		/// </summary>
		/// <param name="g">Die Zeichenfläche auf der gezeichnet werden soll</param>
		/// <param name="velocityMapping">Flag whether current velocity and acceleration shall be mapped to color.</param>
		public virtual void Draw(Graphics g, bool velocityMapping)
			{
			GraphicsPath gp = BuildGraphicsPath();
			if (velocityMapping)
				_fillBrush.Color = _colormap.GetInterpolatedColor(_physics.velocity / effectiveDesiredVelocity);
			else
				_fillBrush.Color = _color;
			g.FillPath(_fillBrush, gp);

			if (velocityMapping)
				{
				if (isStopped)
					_outlinePen.Color = Color.Red;
				else
					_outlinePen.Color = (_physics.acceleration >= 0) ? Color.Green : Color.Orange;
				g.DrawPath(_outlinePen, gp);
				}
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

			foreach (SpecificIntersection si in registeredIntersections)
				{
				g.DrawLine(lineToIntersectionPen, state.positionAbs, si.intersection.aPosition);
				CrossingVehicleTimes myCvt = si.intersection.GetCrossingVehicleTimes(this, si.nodeConnection);
				g.DrawString("arr.: " + myCvt.originalArrivingTime.ToString("####.##") + ", wait: " + myCvt.willWaitInFrontOfIntersection, debugFont, blackBrush, (state.positionAbs + si.intersection.aPosition) * 0.5);
				}


			g.DrawString(hashcode.ToString() + " @ " + currentPosition.ToString("####") + "dm - " + physics.velocity.ToString("##.#") + "m/s - Mult.: " + physics.multiplierTargetVelocity.ToString("#.##") + debugData.ToString(), debugFont, blackBrush, state.positionAbs + new Vector2(0, -10));
			}

		#region Events

		#region VehicleLeftNodeConnection event

		/// <summary>
		/// EventArgs for a VehicleLeftNodeConnection event
		/// </summary>
		public class VehicleLeftNodeConnectionEventArgs : EventArgs
			{
			/// <summary>
			/// used parts of the NodeConnection (arc length)
			/// </summary>
			public Interval<double> partsUsed;

			/// <summary>
			/// start and end time on the NodeConnection
			/// </summary>
			public Interval<double> timeInterval;

			/// <summary>
			/// Creates new VehicleLeftNodeConnectionEventArgs
			/// </summary>
			/// <param name="partsUsed">used parts of the NodeConnection (arc length)</param>
			/// <param name="timeInterval">start and end time on the NodeConnection</param>
			public VehicleLeftNodeConnectionEventArgs(Interval<double> partsUsed, Interval<double> timeInterval)
				{
				this.partsUsed = partsUsed;
				this.timeInterval = timeInterval;
				}
			}

		/// <summary>
		/// Delegate for the VehicleLeftNodeConnection-EventHandler, which is called when vehicle has left a NodeConnection
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void VehicleLeftNodeConnectionEventHandler(object sender, VehicleLeftNodeConnectionEventArgs e);

		/// <summary>
		/// The VehicleLeftNodeConnection event occurs when vehicle has left a NodeConnection
		/// </summary>
		public event VehicleLeftNodeConnectionEventHandler VehicleLeftNodeConnection;

		/// <summary>
		/// Helper method to initiate the VehicleLeftNodeConnection event
		/// </summary>
		/// <param name="e">Eventparameter</param>
		protected void OnVehicleLeftNodeConnection(VehicleLeftNodeConnectionEventArgs e)
			{
			if (VehicleLeftNodeConnection != null)
				{
				VehicleLeftNodeConnection(this, e);
				}
			}

		#endregion

		#region VehicleDied event

		/// <summary>
		/// EventArgs for a VehicleDied event
		/// </summary>
		public class VehicleDiedEventArgs : EventArgs
			{
			/// <summary>
			/// Statistics record of the died vehicle
			/// </summary>
			public IVehicle.Statistics vehicleStatistics;

			/// <summary>
			/// Flag, whether the vehicle reached its destination or not
			/// </summary>
			public bool reachedDestination;

			/// <summary>
			/// Creates new VehicleDiedEventArgs
			/// </summary>
			/// <param name="vehicleStatistics">Statistics record of the died vehicle</param>
			/// <param name="reachedDestination">Flag, whether the vehicle reached its destination or not</param>
			public VehicleDiedEventArgs(IVehicle.Statistics vehicleStatistics, bool reachedDestination)
				{
				this.vehicleStatistics = vehicleStatistics;
				this.reachedDestination = reachedDestination;
				}

			}

		/// <summary>
		/// Delegate for the VehicleDied-EventHandler, which is called when a vehicle dies and won't be spawed somewhere else
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void VehicleDiedEventHandler(object sender, VehicleDiedEventArgs e);

		/// <summary>
		/// The VehicleDied event occurs when a vehicle dies and won't be spawed somewhere else
		/// </summary>
		public event VehicleDiedEventHandler VehicleDied;

		/// <summary>
		/// Helper method to initialte the VehicleDied event
		/// </summary>
		/// <param name="e">Eventparameter</param>
		protected void OnVehicleDied(VehicleDiedEventArgs e)
			{
			if (VehicleDied != null)
				{
				VehicleDied(this, e);
				}
			}

		#endregion

		#endregion

		}
    }
