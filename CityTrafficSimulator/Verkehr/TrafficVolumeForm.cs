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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace CityTrafficSimulator.Verkehr
	{
	/// <summary>
	/// Window for controlling the traffic volume
	/// </summary>
	public partial class TrafficVolumeForm : Form
		{
		/// <summary>
		/// traffic volume controller class
		/// </summary>
		private VerkehrSteuerung m_steuerung;

		/// <summary>
		/// Refernce to MainForm
		/// </summary>
		private MainForm m_mainForm;

		/// <summary>
		/// Reference to NodeSteuerung
		/// </summary>
		private NodeSteuerung m_nodeController;

		/// <summary>
		/// Traffic Volume of the currently selected route
		/// </summary>
		private TrafficVolume m_currentVolume = null;

		/// <summary>
		/// Flag whether to ignore update events for traffic volume
		/// </summary>
		private bool ignoreUpdateEvent = false;

		/// <summary>
		/// Render options for thumbnail view
		/// </summary>
		private NodeSteuerung.RenderOptions renderOptions = new NodeSteuerung.RenderOptions();

		/// <summary>
		/// Constructor
		/// </summary>
		public TrafficVolumeForm(VerkehrSteuerung steuerung, MainForm mainForm, NodeSteuerung nodeController)
			{
			this.m_steuerung = steuerung;
			this.m_mainForm = mainForm;
			this.m_nodeController = nodeController;

			InitializeComponent();
			this.splitContainer1.Panel2MinSize = 260;

			UpdateListboxLayout();
			GetTrafficVolume();

			this.m_steuerung.StartPointsChanged += new VerkehrSteuerung.StartPointsChangedEventHandler(m_steuerung_StartPointsChanged);
			this.m_steuerung.DestinationPointsChanged += new VerkehrSteuerung.DestinationPointsChangedEventHandler(m_steuerung_DestinationPointsChanged);
			this.m_steuerung.GlobalTrafficMultiplierChanged += new VerkehrSteuerung.GlobalTrafficMultiplierChangedEventHandler(m_steuerung_GlobalTrafficMultiplierChanged);
			this.m_steuerung.CarTargetVelocityChanged += new VerkehrSteuerung.CarTargetVelocityChangedEventHandler(m_steuerung_CarTargetVelocityChanged);
			this.m_steuerung.TruckTargetVelocityChanged += new VerkehrSteuerung.TruckTargetVelocityChangedEventHandler(m_steuerung_TruckTargetVelocityChanged);
			this.m_steuerung.BusTargetVelocityChanged += new VerkehrSteuerung.BusTargetVelocityChangedEventHandler(m_steuerung_BusTargetVelocityChanged);
			this.m_steuerung.TramTargetVelocityChanged += new VerkehrSteuerung.TramTargetVelocityChangedEventHandler(m_steuerung_TramTargetVelocityChanged);

			renderOptions.renderLineNodes = false;
			renderOptions.renderNodeConnections = true;
			renderOptions.renderVehicles = false;
			renderOptions.performClipping = false;
			renderOptions.clippingRect = new Rectangle(0, 0, 10000, 10000);
			renderOptions.renderIntersections = false;
			renderOptions.renderLineChangePoints = false;
			renderOptions.renderLineNodeDebugData = false;
			renderOptions.renderNodeConnectionDebugData = false;
			renderOptions.renderVehicleDebugData = false;
			}

		private void m_steuerung_TramTargetVelocityChanged(object sender, VerkehrSteuerung.TramTargetVelocityChangedEventArgs e)
			{
			if (!ignoreUpdateEvent)
				spinTramsTargetVelocity.Value = (decimal)m_steuerung.tramTargetVelocity;
			}

		private void m_steuerung_BusTargetVelocityChanged(object sender, VerkehrSteuerung.BusTargetVelocityChangedEventArgs e)
			{
			if (!ignoreUpdateEvent)
				spinBussesTargetVelocity.Value = (decimal)m_steuerung.busTargetVelocity;
			}

		private void m_steuerung_TruckTargetVelocityChanged(object sender, VerkehrSteuerung.TruckTargetVelocityChangedEventArgs e)
			{
			if (!ignoreUpdateEvent)
				spinTrucksTargetVelocity.Value = (decimal)m_steuerung.truckTargetVelocity;
			}

		private void m_steuerung_CarTargetVelocityChanged(object sender, VerkehrSteuerung.CarTargetVelocityChangedEventArgs e)
			{
			if (!ignoreUpdateEvent)
				spinCarsTargetVelocity.Value = (decimal)m_steuerung.carTargetVelocity;
			}

		private void m_steuerung_GlobalTrafficMultiplierChanged(object sender, VerkehrSteuerung.GlobalTrafficMultiplierChangedEventArgs e)
			{
			if (!ignoreUpdateEvent)
				spinGlobalTrafficVolumeMultiplier.Value = (decimal)m_steuerung.globalTrafficMultiplier;
			}

		private void m_steuerung_StartPointsChanged(object sender, VerkehrSteuerung.StartPointsChangedEventArgs e)
			{
			lbStartNodes.Items.Clear();
			lbStartNodes.Items.AddRange(m_steuerung.startPoints.ToArray());
			GetTrafficVolume();
			}

		private void m_steuerung_DestinationPointsChanged(object sender, VerkehrSteuerung.DestinationPointsChangedEventArgs e)
			{
			lbDestinationNodes.Items.Clear();
			lbDestinationNodes.Items.AddRange(m_steuerung.destinationPoints.ToArray());
			GetTrafficVolume();
			}

		/// <summary>
		/// Updates the layout of the start- and destination point list boxes
		/// </summary>
		private void UpdateListboxLayout()
			{
			int totalWidth = splitContainer1.Panel1.ClientSize.Width;
			int totalHeight = splitContainer1.Panel1.ClientSize.Height;
			int spacer = 4;

			lbStartNodes.Location = new System.Drawing.Point(spacer, 26);
			lbStartNodes.Size = new System.Drawing.Size(totalWidth / 2 - spacer - spacer, totalHeight - 96);

			lbDestinationNodes.Size = new System.Drawing.Size(totalWidth / 2 - spacer - spacer, totalHeight - 96);
			lbDestinationNodes.Location = new System.Drawing.Point(totalWidth - spacer - lbDestinationNodes.Width, 26);

			label2.Location = new System.Drawing.Point(lbDestinationNodes.Location.X, 9);

			lblStartTitle.Location = new System.Drawing.Point(lbStartNodes.Size.Width + lbStartNodes.Location.X - 200 , lbStartNodes.Size.Height + 32 + 3);
			editStartNodeTitle.Location = new System.Drawing.Point(lbStartNodes.Size.Width + lbStartNodes.Location.X - 166, lbStartNodes.Size.Height + 32);
			btnSetStartTitle.Location = new System.Drawing.Point(lbStartNodes.Size.Width + lbStartNodes.Location.X - 40, lbStartNodes.Size.Height + 32 - 1);
			btnAddStartNode.Location = new System.Drawing.Point(lbStartNodes.Size.Width + lbStartNodes.Location.X - 65 - 65 - 60, lbStartNodes.Size.Height + 56);
			btnUpdateStartNodes.Location = new System.Drawing.Point(lbStartNodes.Size.Width + lbStartNodes.Location.X - 65 - 60, lbStartNodes.Size.Height + 56);
			btnRemoveStartNode.Location = new System.Drawing.Point(lbStartNodes.Size.Width + lbStartNodes.Location.X - 60, lbStartNodes.Size.Height + 56);

			lblDestinationTitle.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 200, lbDestinationNodes.Size.Height + 32 + 3);
			editDestinationNodeTitle.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 166, lbDestinationNodes.Size.Height + 32);
			btnSetDestinationTitle.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 40, lbDestinationNodes.Size.Height + 32 - 1);
			btnAddDestinationNode.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 65 - 65 - 60, lbDestinationNodes.Size.Height + 56);
			btnUpdateDestinationNodes.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 65 - 60, lbDestinationNodes.Size.Height + 56);
			btnRemoveDestinationNode.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 60, lbDestinationNodes.Size.Height + 56);
			}

		/// <summary>
		/// Gets the TrafficVolume record to the corresponding start and destination nodes and updates the SpinEdit values.
		/// </summary>
		private void GetTrafficVolume()
			{
			ignoreUpdateEvent = true;
			BunchOfNodes start = lbStartNodes.SelectedItem as BunchOfNodes;
			BunchOfNodes destination = lbDestinationNodes.SelectedItem as BunchOfNodes;
			if (start != null && destination != null)
				{
				m_currentVolume = m_steuerung.GetOrCreateTrafficVolume(start, destination);

				spinCarsVolume.Value = m_currentVolume.trafficVolumeCars;
				spinTruckVolume.Value = m_currentVolume.trafficVolumeTrucks;
				spinBusVolume.Value = m_currentVolume.trafficVolumeBusses;
				spinTramVolume.Value = m_currentVolume.trafficVolumeTrams;
				spinCarsVolume.Enabled = true;
				spinTruckVolume.Enabled = true;
				spinBusVolume.Enabled = true;
				spinTramVolume.Enabled = true;
				ignoreUpdateEvent = false;

				double milage = (m_currentVolume.statistics.sumMilage / m_currentVolume.statistics.numVehiclesReachedDestination) / 10;
				double tt = m_currentVolume.statistics.sumTravelTime / m_currentVolume.statistics.numVehiclesReachedDestination;
				if (m_currentVolume.statistics.numVehicles == 0)
					{
					milage = 0;
					tt = 1;
					}
				lblNumVehicles.Text = "Total Vehicles: " + m_currentVolume.statistics.numVehicles + " (" + m_currentVolume.statistics.numVehiclesReachedDestination + " reached Destination)";
				lblMilage.Text = "Average Milage: " + milage + "m";
				lblTravelTime.Text = "Average Travel Time: " + tt + "s";
				lblVelocity.Text = "Average Milage: " + (milage / tt) + "m/s";
				lblNumStops.Text = "Average Number of Stops: " + ((float)m_currentVolume.statistics.numStops / m_currentVolume.statistics.numVehicles);
				}
			else
				{
				m_currentVolume = null;
				spinCarsVolume.Enabled = false;
				spinTruckVolume.Enabled = false;
				spinBusVolume.Enabled = false;
				spinTramVolume.Enabled = false;
				lblNumVehicles.Text = "Total Vehicles: 0";
				lblMilage.Text = "Average Milage: 0m";
				lblTravelTime.Text = "Average Travel Time: 0s";
				lblVelocity.Text = "Average Milage: 0m/s";
				lblNumStops.Text = "Average Number of Stops: 0";
				}
			ignoreUpdateEvent = false;
			}

		/// <summary>
		/// Stores the traffic volume into the current TrafficVolume record.
		/// </summary>
		private void UpdateTrafficVolume()
			{
			if (m_currentVolume != null && !ignoreUpdateEvent)
				{
				m_currentVolume.SetTrafficVolume((int)spinCarsVolume.Value, (int)spinTruckVolume.Value, (int)spinBusVolume.Value, (int)spinTramVolume.Value);
				}
			}

		private void splitContainer1_Panel1_Resize(object sender, EventArgs e)
			{
			UpdateListboxLayout();
			}

		private void btnAddStartNode_Click(object sender, EventArgs e)
			{
			if (m_mainForm.selectedLineNodes.Count > 0)
				{
				m_steuerung.AddStartPoint(new BunchOfNodes(m_mainForm.selectedLineNodes, editStartNodeTitle.Text));
				}			
			}

		private void btnAddDestinationNode_Click(object sender, EventArgs e)
			{
			if (m_mainForm.selectedLineNodes.Count > 0)
				{
				m_steuerung.AddDestinationPoint(new BunchOfNodes(m_mainForm.selectedLineNodes, editDestinationNodeTitle.Text));
				}		
			}

		private void btnRemoveStartNode_Click(object sender, EventArgs e)
			{
			BunchOfNodes bon = lbStartNodes.SelectedItem as BunchOfNodes;
			if (bon != null)
				{
				m_steuerung.RemoveStartPoint(bon);
				}
			}

		private void btnRemoveDestinationNode_Click(object sender, EventArgs e)
			{
			BunchOfNodes bon = lbDestinationNodes.SelectedItem as BunchOfNodes;
			if (bon != null)
				{
				m_steuerung.RemoveStartPoint(bon);
				}
			}

		private void btnSetStartTitle_Click(object sender, EventArgs e)
			{
			if (lbStartNodes.SelectedIndex >= 0)
				{
				m_steuerung.UpdateStartPointTitle(lbStartNodes.SelectedIndex, editStartNodeTitle.Text);
				}
			}

		private void btnSetDestinationTitle_Click(object sender, EventArgs e)
			{
			if (lbDestinationNodes.SelectedIndex >= 0)
				{
				m_steuerung.UpdateDestinationPointTitle(lbDestinationNodes.SelectedIndex, editDestinationNodeTitle.Text);
				}

			}

		private void spinTruckVolume_ValueChanged(object sender, EventArgs e)
			{
			UpdateTrafficVolume();
			}

		private void spinCarsVolume_ValueChanged(object sender, EventArgs e)
			{
			UpdateTrafficVolume();
			}

		private void spinBusVolume_ValueChanged(object sender, EventArgs e)
			{
			UpdateTrafficVolume();
			}

		private void spinTramVolume_ValueChanged(object sender, EventArgs e)
			{
			UpdateTrafficVolume();
			}

		private void lbStartNodes_SelectedIndexChanged(object sender, EventArgs e)
			{
			BunchOfNodes bon = lbStartNodes.SelectedItem as BunchOfNodes;
			if (bon != null)
				{
				editStartNodeTitle.Text = bon.title;
				m_mainForm.fromLineNodes = bon.nodes;
				}
			GetTrafficVolume();
			}

		private void lbDestinationNodes_SelectedIndexChanged(object sender, EventArgs e)
			{
			BunchOfNodes bon = lbDestinationNodes.SelectedItem as BunchOfNodes;
			if (bon != null)
				{
				editDestinationNodeTitle.Text = bon.title;
				m_mainForm.toLineNodes = bon.nodes;
				} 
			GetTrafficVolume();
			}

		private void spinGlobalTrafficVolumeMultiplier_ValueChanged(object sender, EventArgs e)
			{
			ignoreUpdateEvent = true;
			m_steuerung.globalTrafficMultiplier = (double)spinGlobalTrafficVolumeMultiplier.Value;
			ignoreUpdateEvent = false;
			}

		private void btnUpdateDestinationNodes_Click(object sender, EventArgs e)
			{
			if (m_mainForm.selectedLineNodes.Count > 0 && lbDestinationNodes.SelectedIndex > -1)
				{
				int tmp = lbDestinationNodes.SelectedIndex;
				m_steuerung.UpdateDestinationPointNodes(tmp, m_mainForm.selectedLineNodes);
				lbDestinationNodes.SelectedIndex = tmp;
				}
			}

		private void btnUpdateStartNodes_Click(object sender, EventArgs e)
			{
			if (m_mainForm.selectedLineNodes.Count > 0 && lbStartNodes.SelectedIndex > -1)
				{
				int tmp = lbStartNodes.SelectedIndex;
				m_steuerung.UpdateStartPointNodes(tmp, m_mainForm.selectedLineNodes);
				lbStartNodes.SelectedIndex = tmp;
				}
			}

		private void spinCarsTargetVelocity_ValueChanged(object sender, EventArgs e)
			{
			ignoreUpdateEvent = true;
			m_steuerung.carTargetVelocity = (double)spinCarsTargetVelocity.Value;
			ignoreUpdateEvent = false;
			}

		private void spinTrucksTargetVelocity_ValueChanged(object sender, EventArgs e)
			{
			ignoreUpdateEvent = true;
			m_steuerung.truckTargetVelocity = (double)spinTrucksTargetVelocity.Value;
			ignoreUpdateEvent = false;
			}

		private void spinBussesTargetVelocity_ValueChanged(object sender, EventArgs e)
			{
			ignoreUpdateEvent = true;
			m_steuerung.busTargetVelocity = (double)spinBussesTargetVelocity.Value;
			ignoreUpdateEvent = false;
			}

		private void spinTramsTargetVelocity_ValueChanged(object sender, EventArgs e)
			{
			ignoreUpdateEvent = true;
			m_steuerung.tramTargetVelocity = (double)spinTrucksTargetVelocity.Value;
			ignoreUpdateEvent = false;
			}
		}
	}
