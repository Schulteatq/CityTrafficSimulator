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
		private int _trafficVolumeCars;
		/// <summary>
		/// Car traffic volume in vehicles/hour
		/// </summary>
		public int trafficVolumeCars
			{
			get { return _trafficVolumeCars; }
			set { _trafficVolumeCars = value; }
			}

		/// <summary>
		/// Truck traffic volume in vehicles/hour
		/// </summary>
		private int _trafficVolumeTrucks;
		/// <summary>
		/// Truck traffic volume in vehicles/hour
		/// </summary>
		public int trafficVolumeTrucks
			{
			get { return _trafficVolumeTrucks; }
			set { _trafficVolumeTrucks = value; }
			}

		/// <summary>
		/// Bus traffic volume in vehicles/hour
		/// </summary>
		private int _trafficVolumeBusses;
		/// <summary>
		/// Bus traffic volume in vehicles/hour
		/// </summary>
		public int trafficVolumeBusses
			{
			get { return _trafficVolumeBusses; }
			set { _trafficVolumeBusses = value; }
			}

		/// <summary>
		/// Tram traffic volume in vehicles/hour
		/// </summary>
		private int _trafficVolumeTrams;
		/// <summary>
		/// Tram traffic volume in vehicles/hour
		/// </summary>
		public int trafficVolumeTrams
			{
			get { return _trafficVolumeTrams; }
			set { _trafficVolumeTrams = value; }
			}

		#region Statistics

		/// <summary>
		/// Statistics Record for TrafficVolumes
		/// </summary>
		public struct Statistics
			{
			/// <summary>
			/// Total spawned and died vehicles of this TrafficVolume
			/// </summary>
			public int numVehicles;

			/// <summary>
			/// Total number of vehicles that reached its destination
			/// </summary>
			public int numVehiclesReachedDestination;

			/// <summary>
			/// Total number of stops of all vehicles;
			/// </summary>
			public int numStops;

			/// <summary>
			/// Sum of total travel time
			/// </summary>
			public double sumTravelTime;

			/// <summary>
			/// Sum of total milage
			/// </summary>
			public double sumMilage;
			}

		/// <summary>
		/// Statistics record of this TrafficVolume
		/// </summary>
		private TrafficVolume.Statistics _statistics;
		/// <summary>
		/// Statistics record of this TrafficVolume
		/// </summary>
		public TrafficVolume.Statistics statistics
			{
			get { return _statistics; }
			}


		#endregion


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
			this._trafficVolumeCars = 0;
			this._trafficVolumeTrucks = 0;
			this._trafficVolumeBusses = 0;
			this._trafficVolumeTrams = 0;
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
			this._trafficVolumeCars = cars;
			this._trafficVolumeTrucks = trucks;
			this._trafficVolumeBusses = busses;
			this._trafficVolumeTrams = trams;
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
				int randomValue = trafficVolumeCars > 0 ? GlobalRandom.Instance.Next((int)Math.Ceiling(3600.0 / (tickLength * trafficVolumeCars))) : -1;
				if (randomValue == 0)
					{
					OnVehicleSpawned(new VehicleSpawnedEventArgs(new Car(new IVehicle.Physics()), this));
					}

				// enqueue trucks
				randomValue = trafficVolumeTrucks > 0 ? GlobalRandom.Instance.Next((int)Math.Ceiling(3600.0 / (tickLength * trafficVolumeTrucks))) : -1;
				if (randomValue == 0)
					{
					OnVehicleSpawned(new VehicleSpawnedEventArgs(new Truck(new IVehicle.Physics()), this));
					}

				// enqueue busses
				randomValue = trafficVolumeBusses > 0 ? GlobalRandom.Instance.Next((int)Math.Ceiling(3600.0 / (tickLength * trafficVolumeBusses))) : -1;
				if (randomValue == 0)
					{
					OnVehicleSpawned(new VehicleSpawnedEventArgs(new Bus(new IVehicle.Physics()), this));
					}

				// enqueue trams
				randomValue = trafficVolumeTrams > 0 ? GlobalRandom.Instance.Next((int)Math.Ceiling(3600.0 / (tickLength * trafficVolumeTrams))) : -1;
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

		/// <summary>
		/// To be called, when a vehicle which was spawned by this TrafficVolume has died
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="e">VehicleDiedEventArgs</param>
		public void SpawnedVehicleDied(object sender, IVehicle.VehicleDiedEventArgs e)
			{
			++_statistics.numVehicles;
			if (e.reachedDestination)
				++_statistics.numVehiclesReachedDestination;
			_statistics.numStops += e.vehicleStatistics.numStops;
			_statistics.sumMilage += e.vehicleStatistics.totalMilage;
			_statistics.sumTravelTime += GlobalTime.Instance.currentTime - e.vehicleStatistics.startTime;
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
