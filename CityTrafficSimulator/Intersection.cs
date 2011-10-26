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
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using CityTrafficSimulator.Vehicle;

namespace CityTrafficSimulator
	{
	/// <summary>
	/// Klasse zur Kapselung eines Schnittpunktes zweier NodeConnections A und B
	/// </summary>
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

		/// <summary>
		/// Returns whether vehicles shall avoid blocking this intersection or not.
		/// Intersections, where both NodeConnections originate or terminate in the same LineNode may be blocked.
		/// </summary>
		public bool avoidBlocking
			{
			get { return aConnection.startNode != bConnection.startNode && aConnection.endNode != bConnection.endNode; }
			}

		#endregion

		#region Konstruktoren

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

		#region Interferenzen zwischen Fahrzeugen

		/// <summary>
		/// Original arriving- and blocking times of vehicles approaching on NodeConnection A
		/// </summary>
		private Dictionary<IVehicle, CrossingVehicleTimes> aCrossingVehicles = new Dictionary<IVehicle, CrossingVehicleTimes>(32);

		/// <summary>
		/// Original arriving- and blocking times of vehicles approaching on NodeConnection B
		/// </summary>
		private Dictionary<IVehicle, CrossingVehicleTimes> bCrossingVehicles = new Dictionary<IVehicle, CrossingVehicleTimes>(32);


		/// <summary>
		/// Registers that the given vehicle is going to cross this intersection via the given NodeConnection.
		/// </summary>
		/// <param name="v">Vehicle to cross intersection (must not be registered yet!).</param>
		/// <param name="nc">NodeConnection the vehicle is going to use (must participate on this Intersection!).</param>
		/// <param name="distance">Current distance of the vehicle to the Intersection</param>
		/// <param name="currentTime">current world time.</param>
		public void RegisterVehicle(IVehicle v, NodeConnection nc, double distance, double currentTime)
			{
			Debug.Assert(nc == aConnection || nc == bConnection);
			// TODO: add some safety space before and behind
			double blockingStartTime = currentTime + CalculateArrivingTime(v, distance - GetWaitingDistance()) - v.SafetyTime/2;
			double blockingEndTime = currentTime + CalculateArrivingTime(v, distance + v.length + GetWaitingDistance() + v.SafetyTime/2);

			if (nc == aConnection)
				{
				//Debug.Assert(!aCrossingVehicles.ContainsKey(v));
				aCrossingVehicles.Add(v, new CrossingVehicleTimes(blockingStartTime, distance, new Interval<double>(blockingStartTime, blockingEndTime), false));
				}
			else
				{
				//Debug.Assert(!bCrossingVehicles.ContainsKey(v));
				bCrossingVehicles.Add(v, new CrossingVehicleTimes(blockingStartTime, distance, new Interval<double>(blockingStartTime, blockingEndTime), false));
				}
			}

		/// <summary>
		/// Updates the already registered vehicle v crossing on nc. 
		/// </summary>
		/// <param name="v">Vehicle to update (must already be registered!).</param>
		/// <param name="nc">NodeConnection the vehicle is going to use (must participate on this Intersection!).</param>
		/// <param name="distance">Current distance of the vehicle to the Intersection</param>
		/// <param name="currentTime">current world time.</param>
		public void UpdateVehicle(IVehicle v, NodeConnection nc, double distance, double currentTime)
			{
			Debug.Assert(nc == aConnection || nc == bConnection);
			// TODO: add some safety space before and behind
			double blockingStartTime = currentTime + CalculateArrivingTime(v, distance - GetWaitingDistance()) - v.SafetyTime/2;
			double blockingEndTime = currentTime + CalculateArrivingTime(v, distance + v.length + GetWaitingDistance()) + v.SafetyTime/2;

			if (nc == aConnection)
				{
				Debug.Assert(aCrossingVehicles.ContainsKey(v));
				CrossingVehicleTimes cvt = aCrossingVehicles[v]; // CrossingVehicleTimes is a Value-Type!
				cvt.remainingDistance = distance;
				cvt.blockingTime.left = blockingStartTime;
				cvt.blockingTime.right = blockingEndTime;
				aCrossingVehicles[v] = cvt;
				}
			else
				{
				Debug.Assert(bCrossingVehicles.ContainsKey(v));
				CrossingVehicleTimes cvt = bCrossingVehicles[v]; // CrossingVehicleTimes is a Value-Type!
				cvt.remainingDistance = distance;
				cvt.blockingTime.left = blockingStartTime;
				cvt.blockingTime.right = blockingEndTime;
				bCrossingVehicles[v] = cvt;
				}
			}

		/// <summary>
		/// Updates the already registered vehicle v crossing on nc. 
		/// </summary>
		/// <param name="v">Vehicle to update (must already be registered!).</param>
		/// <param name="nc">NodeConnection the vehicle is going to use (must participate on this Intersection!).</param>
		/// <param name="willWaitInFrontOfIntersection">Set true if vehicle will wait before intersection (and thus not cross it in the meantime).</param>
		public void UpdateVehicle(IVehicle v, NodeConnection nc, bool willWaitInFrontOfIntersection)
			{
			Debug.Assert(nc == aConnection || nc == bConnection);

			if (nc == aConnection)
				{
				Debug.Assert(aCrossingVehicles.ContainsKey(v));
				CrossingVehicleTimes cvt = aCrossingVehicles[v]; // CrossingVehicleTimes is a Value-Type!
				cvt.willWaitInFrontOfIntersection = willWaitInFrontOfIntersection;
				aCrossingVehicles[v] = cvt;
				}
			else
				{
				Debug.Assert(bCrossingVehicles.ContainsKey(v));
				CrossingVehicleTimes cvt = bCrossingVehicles[v]; // CrossingVehicleTimes is a Value-Type!
				cvt.willWaitInFrontOfIntersection = willWaitInFrontOfIntersection;
				bCrossingVehicles[v] = cvt;
				}
			}

		/// <summary>
		/// Unregisters the given vehicle from this intersection.
		/// </summary>
		/// <param name="v">Vehicle to unregister (must already be registered!).</param>
		/// <param name="nc">NodeConnection the vehicle is going to use (must participate on this Intersection!).</param>
		public void UnregisterVehicle(IVehicle v, NodeConnection nc)
			{
			Debug.Assert(nc == aConnection || nc == bConnection);

			if (nc == aConnection)
				{
				Debug.Assert(aCrossingVehicles.ContainsKey(v));
				aCrossingVehicles.Remove(v);
				}
			else
				{
				Debug.Assert(bCrossingVehicles.ContainsKey(v));
				bCrossingVehicles.Remove(v);
				}
			}


		/// <summary>
		/// Calculates all interfering vehicles from registered vehicles.
		/// </summary>
		public List<CrossingVehicleTimes> CalculateInterferingVehicles(IVehicle v, NodeConnection nc)
			{
			Debug.Assert(nc == aConnection || nc == bConnection);

			// hopefully these are references... ;)
			List<CrossingVehicleTimes> toReturn = new List<CrossingVehicleTimes>();
			Dictionary<IVehicle, CrossingVehicleTimes> myCrossingVehicles = (nc == aConnection ? aCrossingVehicles : bCrossingVehicles);
			Dictionary<IVehicle, CrossingVehicleTimes> otherCrossingVehicles = (nc == aConnection ? bCrossingVehicles : aCrossingVehicles);
			CrossingVehicleTimes myCvt = myCrossingVehicles[v];

			// check each vehicle in aCrossingVehicles with each in bCrossingVehicles for interference
			foreach (KeyValuePair<IVehicle, CrossingVehicleTimes> ocv in otherCrossingVehicles)
				{
				if (!ocv.Value.willWaitInFrontOfIntersection && myCvt.blockingTime.IntersectsTrue(ocv.Value.blockingTime))
					{
					toReturn.Add(ocv.Value);
					}
				}

			return toReturn;
			}

		/// <summary>
		/// Returns the CrossingVehicleTimes data of the given vehicle.
		/// </summary>
		/// <param name="v">Vehicle to search for (must already be registered!).</param>
		/// <param name="nc">NodeConnection the vehicle is going to use (must participate on this Intersection!).</param>
		/// <returns>The CrossingVehicleTimes data of the given vehicle.</returns>
		public CrossingVehicleTimes GetCrossingVehicleTimes(IVehicle v, NodeConnection nc)
			{
			Debug.Assert(nc == aConnection || nc == bConnection);

			if (nc == aConnection)
				{
				Debug.Assert(aCrossingVehicles.ContainsKey(v));
				return aCrossingVehicles[v];
				}
			else
				{
				Debug.Assert(bCrossingVehicles.ContainsKey(v));
				return bCrossingVehicles[v];
				}
			}

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

	/// <summary>
	/// Struct encapsulating the original arriving times and blocking times of a vehicle that is going to cross an intersection.
	/// Vehicle, Intersection and NodeConnection must be determined by context.
	/// </summary>
	public struct CrossingVehicleTimes
		{
		/// <summary>
		/// Time when the vehicle originally planned to arrive at the intersection.
		/// </summary>
		public double originalArrivingTime;

		/// <summary>
		/// Remaining distance of the vehicle to the intersection.
		/// </summary>
		public double remainingDistance;

		/// <summary>
		/// Time interval when the vehicle is going to block the intersection.
		/// </summary>
		public Interval<double> blockingTime;

		/// <summary>
		/// Flag whether vehicle will wait befor intersection or proceed crossing it.
		/// </summary>
		public bool willWaitInFrontOfIntersection;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="originalArrivingTime">Time when the vehicle originally planned to arrive at the intersection.</param>
		/// <param name="remainingDistance">Remaining distance of the vehicle to the intersection.</param>
		/// <param name="blockingTime">Time interval when the vehicle is going to block the intersection.</param>
		/// <param name="willWaitInFrontOfIntersection">Flag whether vehicle will wait befor intersection or proceed crossing it.</param>
		public CrossingVehicleTimes(double originalArrivingTime, double remainingDistance, Interval<double> blockingTime, bool willWaitInFrontOfIntersection)
			{
			this.originalArrivingTime = originalArrivingTime;
			this.remainingDistance = remainingDistance;
			this.blockingTime = blockingTime;
			this.willWaitInFrontOfIntersection = willWaitInFrontOfIntersection;
			}
		}

	}
