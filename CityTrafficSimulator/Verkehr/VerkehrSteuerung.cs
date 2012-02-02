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
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using CityTrafficSimulator.Vehicle;

namespace CityTrafficSimulator.Verkehr
	{
	/// <summary>
	/// Steuert das Verkehrsaufkommen
	/// </summary>
	public class VerkehrSteuerung : ITickable
		{
		#region Klassenmember inklusive Modifikationsmethoden

		/// <summary>
		/// Random number generator
		/// </summary>
		private static Random rnd = new Random();

		#region Startpunkte

		/// <summary>
		/// Liste der Startpunkte
		/// </summary>
		private List<BunchOfNodes> _startPoints = new List<BunchOfNodes>();
		/// <summary>
		/// Liste der Startpunkte
		/// </summary>
		public List<BunchOfNodes> startPoints
			{
			get { return _startPoints; }
			}

		/// <summary>
		/// fügt eine BunchOfNodes den startPoints hinzu
		/// </summary>
		/// <param name="sp">hinzuzufügende BunchOfNodes</param>
		public void AddStartPoint(BunchOfNodes sp)
			{
			_startPoints.Add(sp);
			OnStartPointsChanged(new StartPointsChangedEventArgs());
			}

		/// <summary>
		/// entfernt die BunchOfNodes sp aus startPoints
		/// </summary>
		/// <param name="sp">zu löschende BunchOfNodes</param>
		public void RemoveStartPoint(BunchOfNodes sp)
			{
			// remove all corresponding traffic volumes
			trafficVolumes.RemoveAll(delegate(TrafficVolume tv) { return tv.startNodes == sp; });

			_startPoints.Remove(sp);
			OnStartPointsChanged(new StartPointsChangedEventArgs());
			}

		/// <summary>
		/// Leert die Liste der Startpunkte
		/// </summary>
		public void ClearStartPoints()
			{
			_startPoints.Clear();
			_trafficVolumes.Clear();
			OnStartPointsChanged(new StartPointsChangedEventArgs());
			}

		/// <summary>
		/// Updates the title of the start point bunch with the given index
		/// </summary>
		/// <param name="index">Index in start point list</param>
		/// <param name="title">New title</param>
		public void UpdateStartPointTitle(int index, string title)
			{
			if (index < _startPoints.Count)
				{
				_startPoints[index].title = title;
				OnStartPointsChanged(new StartPointsChangedEventArgs());
				}
			}

		/// <summary>
		/// Updates the LineNodes of the start point bunch with the given index
		/// </summary>
		/// <param name="index">Index in the start point list</param>
		/// <param name="nodes">New list of nodes</param>
		public void UpdateStartPointNodes(int index, List<LineNode> nodes)
			{
			if (index < _startPoints.Count)
				{
				_startPoints[index].nodes = nodes;
				OnDestinationPointsChanged(new DestinationPointsChangedEventArgs());
				}

			}

		#endregion

		#region Zielpunkte

		/// <summary>
		/// Liste der Zielpunkte
		/// </summary>
		private List<BunchOfNodes> _destinationPoints = new List<BunchOfNodes>();
		/// <summary>
		/// Liste der Zielpunkte
		/// </summary>
		public List<BunchOfNodes> destinationPoints
			{
			get { return _destinationPoints; }
			}

		/// <summary>
		/// fügt eine BunchOfNodes den targetPoints hinzu
		/// </summary>
		/// <param name="sp">hinzuzufügende BunchOfNodes</param>
		public void AddDestinationPoint(BunchOfNodes sp)
			{
			_destinationPoints.Add(sp);
			OnDestinationPointsChanged(new DestinationPointsChangedEventArgs());
			}

		/// <summary>
		/// entfernt die BunchOfNodes sp aus targetPoints
		/// </summary>
		/// <param name="sp">zu löschende BunchOfNodes</param>
		public void RemoveDestinationPoint(BunchOfNodes sp)
			{
			// remove all corresponding traffic volumes
			trafficVolumes.RemoveAll(delegate(TrafficVolume tv) { return tv.destinationNodes == sp; });

			_destinationPoints.Remove(sp);
			OnDestinationPointsChanged(new DestinationPointsChangedEventArgs());
			}

		/// <summary>
		/// Leert die Liste der Zielpunkte
		/// </summary>
		public void ClearDestinationPoints()
			{
			_destinationPoints.Clear();
			_trafficVolumes.Clear();
			OnDestinationPointsChanged(new DestinationPointsChangedEventArgs());
			}

		/// <summary>
		/// Updates the title of the destination point bunch with the given index
		/// </summary>
		/// <param name="index">Index in destination point list</param>
		/// <param name="title">New title</param>
		public void UpdateDestinationPointTitle(int index, string title)
			{
			if (index < _destinationPoints.Count)
				{
				_destinationPoints[index].title = title;
				OnDestinationPointsChanged(new DestinationPointsChangedEventArgs());
				}
			}

		/// <summary>
		/// Updates the LineNodes of the destination point bunch with the given index
		/// </summary>
		/// <param name="index">Index in the destination point list</param>
		/// <param name="nodes">New list of nodes</param>
		public void UpdateDestinationPointNodes(int index, List<LineNode> nodes)
			{
			if (index < _destinationPoints.Count)
				{
				_destinationPoints[index].nodes = nodes;
				OnDestinationPointsChanged(new DestinationPointsChangedEventArgs());
				}

			}

		#endregion

		/// <summary>
		/// List of traffic volumes
		/// </summary>
		private List<TrafficVolume> _trafficVolumes = new List<TrafficVolume>();
		/// <summary>
		/// List of traffic volumes
		/// </summary>
		public List<TrafficVolume> trafficVolumes
			{
			get { return _trafficVolumes; }
			}

		/// <summary>
		/// Global multiplier for traffic volume
		/// </summary>
		private double _globalTrafficMultiplier = 1;
		/// <summary>
		/// Global multiplier for traffic volume
		/// </summary>
		public double globalTrafficMultiplier
			{
			get { return _globalTrafficMultiplier; }
			set { _globalTrafficMultiplier = value; OnGlobalTrafficMultiplierChanged(new GlobalTrafficMultiplierChangedEventArgs()); }
			}

		/// <summary>
		/// Target velocity for cars
		/// </summary>
		private double _carTargetVelocity = 36;
		/// <summary>
		/// Target velocity for cars
		/// </summary>
		public double carTargetVelocity
			{
			get { return _carTargetVelocity; }
			set { _carTargetVelocity = value; OnCarTargetVelocityChanged(new CarTargetVelocityChangedEventArgs()); }
			}

		/// <summary>
		/// Target velocity for trucks
		/// </summary>
		private double _truckTargetVelocity = 23;
		/// <summary>
		/// Target velocity for trucks
		/// </summary>
		public double truckTargetVelocity
			{
			get { return _truckTargetVelocity; }
			set { _truckTargetVelocity = value; OnTruckTargetVelocityChanged(new TruckTargetVelocityChangedEventArgs()); }
			}

		/// <summary>
		/// Target velocity for busses
		/// </summary>
		private double _busTargetVelocity = 23;
		/// <summary>
		/// Target velocity for busses
		/// </summary>
		public double busTargetVelocity
			{
			get { return _busTargetVelocity; }
			set { _busTargetVelocity = value; OnBusTargetVelocityChanged(new BusTargetVelocityChangedEventArgs()); }
			}

		/// <summary>
		/// Target velocity for trams
		/// </summary>
		private double _tramTargetVelocity = 23;
		/// <summary>
		/// Target velocity for trams
		/// </summary>
		public double tramTargetVelocity
			{
			get { return _tramTargetVelocity; }
			set { _tramTargetVelocity = value; OnTramTargetVelocityChanged(new TramTargetVelocityChangedEventArgs()); }
			}


		/// <summary>
		/// List of vehicles to spawn
		/// </summary>
		private List<TrafficVolume.VehicleSpawnedEventArgs> _vehiclesToSpawn = new List<TrafficVolume.VehicleSpawnedEventArgs>();
		
		#endregion

		#region Methods

		/// <summary>
		/// Returns the Traffic Volume for the route from start to destination.
		/// If no such TrafficVolume exists, a new one will be created.
		/// start and destination MUST be != null!
		/// </summary>
		/// <param name="start">Start nodes</param>
		/// <param name="destination">Destination nodes</param>
		/// <returns></returns>
		public TrafficVolume GetOrCreateTrafficVolume(BunchOfNodes start, BunchOfNodes destination)
			{
			Debug.Assert(start != null && destination != null);

			// There certainly are data structures offering better algorithms to search for these specific entities.
			// But m_trafficVolumes usually contains < 100 items, so performance can be seen as unimportant here.
			foreach (TrafficVolume tv in _trafficVolumes)
				{
				if (tv.startNodes == start && tv.destinationNodes == destination)
					return tv;
				}

			TrafficVolume newTV = new TrafficVolume(start, destination);
			newTV.VehicleSpawned += new TrafficVolume.VehicleSpawnedEventHandler(newTV_VehicleSpawned);
			_trafficVolumes.Add(newTV);
			return newTV;
			}


		/// <summary>
		/// Finds a BunchOfNode in bofList being equal to nodeList. If no such BoF exists, a new one will be created and added.
		/// </summary>
		/// <param name="nodeList">List of LineNodes</param>
		/// <param name="bofList">List of BunchOfNodes</param>
		/// <returns>A BunchOfNodes containing the same LineNodes as nodeList</returns>
		private BunchOfNodes GetOrCreateEqualBoF(List<LineNode> nodeList, List<BunchOfNodes> bofList)
			{
			// search for an fitting BunchOfNode
			foreach (BunchOfNodes bof in bofList)
				{
				bool equal = true;
				foreach (LineNode ln in nodeList)
					{
					if (!bof.nodes.Contains(ln))
						{
						equal = false;
						break;
						}
					}

				if (equal)
					{
					return bof;
					}
				}

			// No equal BoF was found => create a new one
			BunchOfNodes newBof = new BunchOfNodes(nodeList, "Bunch " + (bofList.Count + 1).ToString());
			bofList.Add(newBof);
			return newBof;
			}

		/// <summary>
		/// Imports traffic volume from older file versions still containing "Aufträge"
		/// </summary>
		/// <param name="fahrauftraege">List of all "Aufträge" to import</param>
		public void ImportOldTrafficVolumeData(List<Auftrag> fahrauftraege)
			{
			foreach (Auftrag a in fahrauftraege)
				{
				BunchOfNodes startBof = GetOrCreateEqualBoF(a.startNodes, startPoints);
				BunchOfNodes destinationBof = GetOrCreateEqualBoF(a.endNodes, destinationPoints);

				TrafficVolume tv = GetOrCreateTrafficVolume(startBof, destinationBof);
				switch (a.vehicleType)
					{
					case CityTrafficSimulator.Vehicle.IVehicle.VehicleTypes.CAR:
						tv.SetTrafficVolume((int)(tv.trafficVolumeCars + a.trafficDensity * 0.92), (int)(tv.trafficVolumeTrucks + a.trafficDensity * 0.08), tv.trafficVolumeBusses, tv.trafficVolumeTrams);
						break;
					case CityTrafficSimulator.Vehicle.IVehicle.VehicleTypes.BUS:
						tv.SetTrafficVolume(tv.trafficVolumeCars, tv.trafficVolumeTrucks, tv.trafficVolumeBusses + a.trafficDensity, tv.trafficVolumeTrams);
						break;
					case CityTrafficSimulator.Vehicle.IVehicle.VehicleTypes.TRAM:
						tv.SetTrafficVolume(tv.trafficVolumeCars, tv.trafficVolumeTrucks, tv.trafficVolumeBusses, tv.trafficVolumeTrams + a.trafficDensity);
						break;
					}
				}

			OnStartPointsChanged(new StartPointsChangedEventArgs());
			OnDestinationPointsChanged(new DestinationPointsChangedEventArgs());
			}

		#endregion

		#region Save/Load

		/// <summary>
		/// Writes all handeled data to a XML document
		/// </summary>
		/// <param name="xw">XMLWriter to use</param>
		/// <param name="xsn">corresponding XML namespace</param>
		public void SaveToFile(XmlWriter xw, XmlSerializerNamespaces xsn)
			{
			try
				{
				XmlSerializer bofSerializer = new XmlSerializer(typeof(BunchOfNodes));
				XmlSerializer tvSerializer = new XmlSerializer(typeof(TrafficVolume));

				// Prepare data for serialization
				foreach (BunchOfNodes bof in startPoints)
					{
					bof.PrepareForSave();
					}
				foreach (BunchOfNodes bof in destinationPoints)
					{
					bof.PrepareForSave();
					}
				foreach (TrafficVolume tv in trafficVolumes)
					{
					tv.PrepareForSave();
					}

				// Write Data
				xw.WriteStartElement("TrafficVolumes");

				xw.WriteStartElement("StartPoints");
				foreach (BunchOfNodes bof in startPoints)
					{
					bofSerializer.Serialize(xw, bof, xsn);
					}
				xw.WriteEndElement();

				xw.WriteStartElement("DestinationPoints");
				foreach (BunchOfNodes bof in destinationPoints)
					{
					bofSerializer.Serialize(xw, bof, xsn);
					}
				xw.WriteEndElement();

				foreach (TrafficVolume tv in trafficVolumes)
					{
					tvSerializer.Serialize(xw, tv, xsn);
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
		/// Loads the traffic volume setup from the given XML file
		/// </summary>
		/// <param name="xd">XmlDocument to parse</param>
		/// <param name="nodesList">List of all existing LineNodes</param>
		/// <param name="lf">LoadingForm for status updates</param>
		public void LoadFromFile(XmlDocument xd, List<LineNode> nodesList, LoadingForm.LoadingForm lf)
			{
			lf.SetupLowerProgess("Parsing XML...", 3);

			// clear everything first
			_trafficVolumes.Clear();
			_startPoints.Clear();
			_destinationPoints.Clear();

			// parse save file version (currently not needed, but probably in future)
			int saveVersion = 0;
			XmlNode mainNode = xd.SelectSingleNode("//CityTrafficSimulator");
			XmlNode saveVersionNode = mainNode.Attributes.GetNamedItem("saveVersion");
			if (saveVersionNode != null)
				saveVersion = Int32.Parse(saveVersionNode.Value);
			else
				saveVersion = 0;

			// Load start points:
			lf.StepLowerProgress();
			// get corresponding XML nodes
			XmlNodeList xnlStartNodes = xd.SelectNodes("//CityTrafficSimulator/TrafficVolumes/StartPoints/BunchOfNodes");
			foreach (XmlNode aXmlNode in xnlStartNodes)
				{
				// Deserialize each node
				TextReader tr = new StringReader(aXmlNode.OuterXml);
				XmlSerializer xs = new XmlSerializer(typeof(BunchOfNodes));
				BunchOfNodes bof = (BunchOfNodes)xs.Deserialize(tr);
				bof.RecoverFromLoad(saveVersion, nodesList);
				_startPoints.Add(bof);
				}

			// Load destination points:
			lf.StepLowerProgress();
			// get corresponding XML nodes
			XmlNodeList xnlDestinationNodes = xd.SelectNodes("//CityTrafficSimulator/TrafficVolumes/DestinationPoints/BunchOfNodes");
			foreach (XmlNode aXmlNode in xnlDestinationNodes)
				{
				// Deserialize each node
				TextReader tr = new StringReader(aXmlNode.OuterXml);
				XmlSerializer xs = new XmlSerializer(typeof(BunchOfNodes));
				BunchOfNodes bof = (BunchOfNodes)xs.Deserialize(tr);
				bof.RecoverFromLoad(saveVersion, nodesList);
				_destinationPoints.Add(bof);
				}

			// Load traffic volumes:
			lf.StepLowerProgress();
			// get corresponding XML nodes
			XmlNodeList xnlTrafficVolumes = xd.SelectNodes("//CityTrafficSimulator/TrafficVolumes/TrafficVolume");
			foreach (XmlNode aXmlNode in xnlTrafficVolumes)
				{
				// Deserialize each node
				TextReader tr = new StringReader(aXmlNode.OuterXml);
				XmlSerializer xs = new XmlSerializer(typeof(TrafficVolume));
				TrafficVolume tv = (TrafficVolume)xs.Deserialize(tr);

				tv.RecoverFromLoad(saveVersion, startPoints, destinationPoints);
				if (tv.startNodes != null && tv.destinationNodes != null)
					{
					_trafficVolumes.Add(tv);
					tv.VehicleSpawned += new TrafficVolume.VehicleSpawnedEventHandler(newTV_VehicleSpawned);
					}
				else
					{
					lf.Log("Error during traffic volume deserialization: Could not dereference start-/end nodes. Traffic volume was dismissed.");
					}
				}

			OnStartPointsChanged(new StartPointsChangedEventArgs());
			OnDestinationPointsChanged(new DestinationPointsChangedEventArgs());
			}


		#endregion

		#region Event-Gedöns

		private void newTV_VehicleSpawned(object sender, TrafficVolume.VehicleSpawnedEventArgs e)
			{
			_vehiclesToSpawn.Add(e);
			}

		#region StartPointsChanged event

		/// <summary>
		/// EventArgs for a StartPointsChanged event
		/// </summary>
		public class StartPointsChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new StartPointsChangedEventArgs
			/// </summary>
			public StartPointsChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the StartPointsChanged-EventHandler, which is called when the collection of start points has changed
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void StartPointsChangedEventHandler(object sender, StartPointsChangedEventArgs e);

		/// <summary>
		/// The StartPointsChanged event occurs when the collection of start points has changed
		/// </summary>
		public event StartPointsChangedEventHandler StartPointsChanged;

		/// <summary>
		/// Helper method to initiate the StartPointsChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void OnStartPointsChanged(StartPointsChangedEventArgs e)
			{
			if (StartPointsChanged != null)
				{
				StartPointsChanged(this, e);
				}
			}

		#endregion

		#region DestinationPointsChanged event

		/// <summary>
		/// EventArgs for a DestinationPointsChanged event
		/// </summary>
		public class DestinationPointsChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new DestinationPointsChangedEventArgs
			/// </summary>
			public DestinationPointsChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the DestinationPointsChanged-EventHandler, which is called when the collection of destination points has changed
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void DestinationPointsChangedEventHandler(object sender, DestinationPointsChangedEventArgs e);

		/// <summary>
		/// The DestinationPointsChanged event occurs when the collection of destination points has changed
		/// </summary>
		public event DestinationPointsChangedEventHandler DestinationPointsChanged;

		/// <summary>
		/// Helper method to initiate the DestinationPointsChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void OnDestinationPointsChanged(DestinationPointsChangedEventArgs e)
			{
			if (DestinationPointsChanged != null)
				{
				DestinationPointsChanged(this, e);
				}
			}

		#endregion

		#region GlobalTrafficMultiplierChanged event

		/// <summary>
		/// EventArgs for a GlobalTrafficMultiplierChanged event
		/// </summary>
		public class GlobalTrafficMultiplierChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new GlobalTrafficMultiplierChangedEventArgs
			/// </summary>
			public GlobalTrafficMultiplierChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the GlobalTrafficMultiplierChanged-EventHandler, which is called when the global traffic volume multiplier has changed
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void GlobalTrafficMultiplierChangedEventHandler(object sender, GlobalTrafficMultiplierChangedEventArgs e);

		/// <summary>
		/// The GlobalTrafficMultiplierChanged event occurs when the global traffic volume multiplier has changed
		/// </summary>
		public event GlobalTrafficMultiplierChangedEventHandler GlobalTrafficMultiplierChanged;

		/// <summary>
		/// Helper method to initiate the GlobalTrafficMultiplierChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void OnGlobalTrafficMultiplierChanged(GlobalTrafficMultiplierChangedEventArgs e)
			{
			if (GlobalTrafficMultiplierChanged != null)
				{
				GlobalTrafficMultiplierChanged(this, e);
				}
			}

		#endregion

		#region CarTargetVelocityChanged event

		/// <summary>
		/// EventArgs for a CarTargetVelocityChanged event
		/// </summary>
		public class CarTargetVelocityChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new CarTargetVelocityChangedEventArgs
			/// </summary>
			public CarTargetVelocityChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the CarTargetVelocityChanged-EventHandler, which is called when the target velocity for cars has changed
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void CarTargetVelocityChangedEventHandler(object sender, CarTargetVelocityChangedEventArgs e);

		/// <summary>
		/// The CarTargetVelocityChanged event occurs when the target velocity for cars has changed
		/// </summary>
		public event CarTargetVelocityChangedEventHandler CarTargetVelocityChanged;

		/// <summary>
		/// Helper method to initiate the CarTargetVelocityChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void OnCarTargetVelocityChanged(CarTargetVelocityChangedEventArgs e)
			{
			if (CarTargetVelocityChanged != null)
				{
				CarTargetVelocityChanged(this, e);
				}
			}

		#endregion

		#region TruckTargetVelocityChanged event

		/// <summary>
		/// EventArgs for a TruckTargetVelocityChanged event
		/// </summary>
		public class TruckTargetVelocityChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new TruckTargetVelocityChangedEventArgs
			/// </summary>
			public TruckTargetVelocityChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the TruckTargetVelocityChanged-EventHandler, which is called when the target velocity for trucks has changed
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void TruckTargetVelocityChangedEventHandler(object sender, TruckTargetVelocityChangedEventArgs e);

		/// <summary>
		/// The TruckTargetVelocityChanged event occurs when the target velocity for trucks has changed
		/// </summary>
		public event TruckTargetVelocityChangedEventHandler TruckTargetVelocityChanged;

		/// <summary>
		/// Helper method to initiate the TruckTargetVelocityChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void OnTruckTargetVelocityChanged(TruckTargetVelocityChangedEventArgs e)
			{
			if (TruckTargetVelocityChanged != null)
				{
				TruckTargetVelocityChanged(this, e);
				}
			}

		#endregion

		#region BusTargetVelocityChanged event

		/// <summary>
		/// EventArgs for a BusTargetVelocityChanged event
		/// </summary>
		public class BusTargetVelocityChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new BusTargetVelocityChangedEventArgs
			/// </summary>
			public BusTargetVelocityChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the BusTargetVelocityChanged-EventHandler, which is called when the target velocity for busses has changed
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void BusTargetVelocityChangedEventHandler(object sender, BusTargetVelocityChangedEventArgs e);

		/// <summary>
		/// The BusTargetVelocityChanged event occurs when the target velocity for busses has changed
		/// </summary>
		public event BusTargetVelocityChangedEventHandler BusTargetVelocityChanged;

		/// <summary>
		/// Helper method to initiate the BusTargetVelocityChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void OnBusTargetVelocityChanged(BusTargetVelocityChangedEventArgs e)
			{
			if (BusTargetVelocityChanged != null)
				{
				BusTargetVelocityChanged(this, e);
				}
			}

		#endregion

		#region TramTargetVelocityChanged event

		/// <summary>
		/// EventArgs for a TramTargetVelocityChanged event
		/// </summary>
		public class TramTargetVelocityChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new TramTargetVelocityChangedEventArgs
			/// </summary>
			public TramTargetVelocityChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the TramTargetVelocityChanged-EventHandler, which is called when the target velocity for trams has changed
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void TramTargetVelocityChangedEventHandler(object sender, TramTargetVelocityChangedEventArgs e);

		/// <summary>
		/// The TramTargetVelocityChanged event occurs when the target velocity for trams has changed
		/// </summary>
		public event TramTargetVelocityChangedEventHandler TramTargetVelocityChanged;

		/// <summary>
		/// Helper method to initiate the TramTargetVelocityChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void OnTramTargetVelocityChanged(TramTargetVelocityChangedEventArgs e)
			{
			if (TramTargetVelocityChanged != null)
				{
				TramTargetVelocityChanged(this, e);
				}
			}

		#endregion

		#endregion


		#region ITickable Member

		private bool SpawnVehicle(TrafficVolume.VehicleSpawnedEventArgs e)
			{
			LineNode start = e.tv.startNodes.nodes[rnd.Next(e.tv.startNodes.nodes.Count)];
			if (start.nextConnections.Count > 0)
				{
				int foo = rnd.Next(start.nextConnections.Count);
				NodeConnection nc = start.nextConnections[foo];

				e.vehicleToSpawn.state = new IVehicle.State(nc, 0);
				e.vehicleToSpawn.physics = new IVehicle.Physics(nc.targetVelocity, nc.targetVelocity, 0);
				if (start.nextConnections[foo].AddVehicle(e.vehicleToSpawn))
					{
					e.vehicleToSpawn.targetNodes = e.tv.destinationNodes.nodes;
					e.vehicleToSpawn.VehicleDied += new IVehicle.VehicleDiedEventHandler(e.tv.SpawnedVehicleDied);
					return true;
					}
				}

			return false;
			}

		/// <summary>
		/// Notifies all handled entities that the world time has advanced by tickLength.
		/// </summary>
		/// <param name="tickLength">Amount the time has advanced</param>
		public void Tick(double tickLength)
			{
			double tmp = tickLength * globalTrafficMultiplier;
			foreach (TrafficVolume tv in trafficVolumes)
				{
				tv.Tick(tmp);
				}

			List<TrafficVolume.VehicleSpawnedEventArgs> failedList = new List<TrafficVolume.VehicleSpawnedEventArgs>(_vehiclesToSpawn.Count);
			foreach (TrafficVolume.VehicleSpawnedEventArgs e in _vehiclesToSpawn)
				{
				if (! SpawnVehicle(e))
					{
					failedList.Add(e);
					}
				}
			_vehiclesToSpawn = failedList;
			}

		/// <summary>
		/// Is called after the tick.
		/// </summary>
		public void Reset()
			{
			// Nothing to do here
			}

		#endregion
		}
	}
