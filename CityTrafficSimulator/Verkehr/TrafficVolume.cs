/*
 *  CityTrafficSimulator - a tool to simulate traffic in urban areas and on intersections
 *  Copyright (C) 2005-2011, Christian Schulte zu Berge
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

using CityTrafficSimulator.Vehicle;

namespace CityTrafficSimulator.Verkehr
	{
	/// <summary>
	/// This class encapsulates the traffic volume between two given BunchOfNode entities.
	/// </summary>
	[Serializable]
	public class TrafficVolume : ITickable
		{
		/// <summary>
		/// Random number generator
		/// </summary>
		private static Random rnd = new Random();


		/// <summary>
		/// Multiplikator für Fahrzeuge/Stunde
		/// </summary>
		public static decimal trafficDensityMultiplier = 1;

		/// <summary>
		/// Start nodes of the traffic volume
		/// </summary>
		[XmlIgnore]
		public BunchOfNodes startNodes;

		/// <summary>
		/// Destination nodes of the traffic volume
		/// </summary>
		[XmlIgnore]
		public BunchOfNodes destinationNodes;

		/// <summary>
		/// Car traffic volume in vehicles/hour
		/// </summary>
		private int m_trafficVolumeCars;
		/// <summary>
		/// Car traffic volume in vehicles/hour
		/// </summary>
		public int trafficVolumeCars
			{
			get { return m_trafficVolumeCars; }
			set { m_trafficVolumeCars = value; }
			}

		/// <summary>
		/// Truck traffic volume in vehicles/hour
		/// </summary>
		private int m_trafficVolumeTrucks;
		/// <summary>
		/// Truck traffic volume in vehicles/hour
		/// </summary>
		public int trafficVolumeTrucks
			{
			get { return m_trafficVolumeTrucks; }
			set { m_trafficVolumeTrucks = value; }
			}

		/// <summary>
		/// Bus traffic volume in vehicles/hour
		/// </summary>
		private int m_trafficVolumeBusses;
		/// <summary>
		/// Bus traffic volume in vehicles/hour
		/// </summary>
		public int trafficVolumeBusses
			{
			get { return m_trafficVolumeBusses; }
			set { m_trafficVolumeBusses = value; }
			}

		/// <summary>
		/// Tram traffic volume in vehicles/hour
		/// </summary>
		private int m_trafficVolumeTrams;
		/// <summary>
		/// Tram traffic volume in vehicles/hour
		/// </summary>
		public int trafficVolumeTrams
			{
			get { return m_trafficVolumeTrams; }
			set { m_trafficVolumeTrams = value; }
			}

		/// <summary>
		/// Queue of vehicles which have been created but could not be put on network because network was full
		/// </summary>
		private Queue<IVehicle> queuedVehicles = new Queue<IVehicle>();

		#region Konstruktoren

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="start">start nodes</param>
		/// <param name="destination">Destination nodes</param>
		public TrafficVolume(BunchOfNodes start, BunchOfNodes destination)
			{
			this.startNodes = start;
			this.destinationNodes = destination;
			this.m_trafficVolumeCars = 0;
			this.m_trafficVolumeTrucks = 0;
			this.m_trafficVolumeBusses = 0;
			this.m_trafficVolumeTrams = 0;
			}

		/// <summary>
		/// DO NOT USE: Empty Constructor - only needed for XML Serialization
		/// </summary>
		public TrafficVolume()
			{
			}

		/// <summary>
		/// Sets the traffic volume for each vehicle type
		/// </summary>
		/// <param name="cars">Car traffic volume in vehicles/hour</param>
		/// <param name="trucks">Truck traffic volume in vehicles/hour</param>
		/// <param name="busses">Bus traffic volume in vehicles/hour</param>
		/// <param name="trams">Tram traffic volume in vehicles/hour</param>
		public void SetTrafficVolume(int cars, int trucks, int busses, int trams)
			{
			this.m_trafficVolumeCars = cars;
			this.m_trafficVolumeTrucks = trucks;
			this.m_trafficVolumeBusses = busses;
			this.m_trafficVolumeTrams = trams;
			}

		#endregion


		#region ITickable Member

		/// <summary>
		/// Notification that the world time has advanced by tickLength.
		/// </summary>
		/// <param name="tickLength">Amount the time has advanced</param>
		public void Tick(double tickLength)
			{
			if (tickLength > 0)
				{
				// enqueue cars
				int randomValue = trafficVolumeCars > 0 ? rnd.Next((int)Math.Ceiling(3600.0 / (tickLength * trafficVolumeCars))) : -1;
				if (randomValue == 0)
					{
					OnVehicleSpawned(new VehicleSpawnedEventArgs(new Car(new IVehicle.Physics()), this));
					}

				// enqueue trucks
				randomValue = trafficVolumeTrucks > 0 ? rnd.Next((int)Math.Ceiling(3600.0 / (tickLength * trafficVolumeTrucks))) : -1;
				if (randomValue == 0)
					{
					OnVehicleSpawned(new VehicleSpawnedEventArgs(new Truck(new IVehicle.Physics()), this));
					}

				// enqueue busses
				randomValue = trafficVolumeBusses > 0 ? rnd.Next((int)Math.Ceiling(3600.0 / (tickLength * trafficVolumeBusses))) : -1;
				if (randomValue == 0)
					{
					OnVehicleSpawned(new VehicleSpawnedEventArgs(new Bus(new IVehicle.Physics()), this));
					}

				// enqueue trams
				randomValue = trafficVolumeTrams > 0 ? rnd.Next((int)Math.Ceiling(3600.0 / (tickLength * trafficVolumeTrams))) : -1;
				if (randomValue == 0)
					{
					OnVehicleSpawned(new VehicleSpawnedEventArgs(new Tram(new IVehicle.Physics()), this));
					}
				}
			}


		/// <summary>
		/// Is called after the tick.
		/// </summary>
		public void Reset()
			{
			// Nothing to do here
			}

		#endregion

		#region Save/Load stuff

		/// <summary>
		/// Hash code of start point
		/// </summary>
		public int startHash = -1;
		/// <summary>
		/// Hash code of destination point
		/// </summary>
		public int destinationHash = -1;

		/// <summary>
		/// Prepares the object for XML serialization.
		/// </summary>
		public void PrepareForSave()
			{
			startHash = startNodes.hashcode;
			destinationHash = destinationNodes.hashcode;
			}

		/// <summary>
		/// Recovers the references after XML deserialization.
		/// </summary>
		/// <param name="saveVersion">Version of the read file</param>
		/// <param name="startList">List of all start BunchOfNodes</param>
		/// <param name="destinationList">List of all destination BunchOfNodes</param>
		public void RecoverFromLoad(int saveVersion, List<BunchOfNodes> startList, List<BunchOfNodes> destinationList)
			{
			foreach (BunchOfNodes bof in startList)
				{
				if (bof.hashcode == startHash)
					startNodes = bof;
				}
			foreach (BunchOfNodes bof in destinationList)
				{
				if (bof.hashcode == destinationHash)
					destinationNodes = bof;
				}
			}

		#endregion

		#region VehicleSpawned event

		/// <summary>
		/// EventArgs for a VehicleSpawned event
		/// </summary>
		public class VehicleSpawnedEventArgs : EventArgs
			{
			/// <summary>
			/// The vehicle to spawn
			/// </summary>
			public IVehicle vehicleToSpawn;

			/// <summary>
			/// Corresponding TrafficVolume
			/// </summary>
			public TrafficVolume tv;

			/// <summary>
			/// Creates new VehicleSpawnedEventArgs
			/// </summary>
			/// <param name="v">The vehicle to spawn</param>
			/// <param name="tv">Corresponding TrafficVolume</param>
			public VehicleSpawnedEventArgs(IVehicle v, TrafficVolume tv)
				{
				this.vehicleToSpawn = v;
				this.tv = tv;
				}
			}

		/// <summary>
		/// Delegate for the VehicleSpawned-EventHandler, which is called when a vehicle is to be spawned
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void VehicleSpawnedEventHandler(object sender, VehicleSpawnedEventArgs e);

		/// <summary>
		/// The VehicleSpawned event occurs when a vehicle is to be spawned
		/// </summary>
		public event VehicleSpawnedEventHandler VehicleSpawned;

		/// <summary>
		/// Helper method to initiate the VehicleSpawned event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void OnVehicleSpawned(VehicleSpawnedEventArgs e)
			{
			if (VehicleSpawned != null)
				{
				VehicleSpawned(this, e);
				}
			}

		#endregion
		}
	}
