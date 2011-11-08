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
			UpdateListboxLayout();
			GetTrafficVolume();

			this.m_steuerung.StartPointsChanged += new VerkehrSteuerung.StartPointsChangedEventHandler(m_steuerung_StartPointsChanged);
			this.m_steuerung.DestinationPointsChanged += new VerkehrSteuerung.DestinationPointsChangedEventHandler(m_steuerung_DestinationPointsChanged);
			this.m_steuerung.GlobalTrafficMultiplierChanged += new VerkehrSteuerung.GlobalTrafficMultiplierChangedEventHandler(m_steuerung_GlobalTrafficMultiplierChanged);

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

		/// <summary>
		/// Invalidates the thumbnail rendering
		/// </summary>
		public void InvalidateThumbGrid()
			{
			thumbGrid.Invalidate();
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
			btnSetStartTitle.Location = new System.Drawing.Point(lbStartNodes.Size.Width + lbStartNodes.Location.X - 40, lbStartNodes.Size.Height + 32 - 1);
			editStartNodeTitle.Location = new System.Drawing.Point(lbStartNodes.Size.Width + lbStartNodes.Location.X - 166, lbStartNodes.Size.Height + 32);
			btnAddStartNode.Location = new System.Drawing.Point(lbStartNodes.Size.Width + lbStartNodes.Location.X - 85 - 80, lbStartNodes.Size.Height + 56);
			btnRemoveStartNode.Location = new System.Drawing.Point(lbStartNodes.Size.Width + lbStartNodes.Location.X - 80, lbStartNodes.Size.Height + 56);

			lblDestinationTitle.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 200, lbDestinationNodes.Size.Height + 32 + 3);
			btnSetDestinationTitle.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 40, lbDestinationNodes.Size.Height + 32 - 1);
			editDestinationNodeTitle.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 166, lbDestinationNodes.Size.Height + 32);
			btnAddDestinationNode.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 85 - 80, lbDestinationNodes.Size.Height + 56);
			btnRemoveDestinationNode.Location = new System.Drawing.Point(lbDestinationNodes.Size.Width + lbDestinationNodes.Location.X - 80, lbDestinationNodes.Size.Height + 56);
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
				}
			else
				{
				m_currentVolume = null;
				spinCarsVolume.Enabled = false;
				spinTruckVolume.Enabled = false;
				spinBusVolume.Enabled = false;
				spinTramVolume.Enabled = false;
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
				}
			GetTrafficVolume();
			InvalidateThumbGrid();
			}

		private void lbDestinationNodes_SelectedIndexChanged(object sender, EventArgs e)
			{
			BunchOfNodes bon = lbDestinationNodes.SelectedItem as BunchOfNodes;
			if (bon != null)
				{
				editDestinationNodeTitle.Text = bon.title;
				} 
			GetTrafficVolume();
			InvalidateThumbGrid();
			}

		private void spinGlobalTrafficVolumeMultiplier_ValueChanged(object sender, EventArgs e)
			{
			ignoreUpdateEvent = true;
			m_steuerung.globalTrafficMultiplier = (double)spinGlobalTrafficVolumeMultiplier.Value;
			ignoreUpdateEvent = false;
			}

		private void thumbGrid_Paint(object sender, PaintEventArgs e)
			{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.InterpolationMode = InterpolationMode.Bilinear;

			// calculate zoom factor
			Vector2 bounds = m_nodeController.GetBounds();
			bounds *= 1.05;
			float zoom = (bounds.X == 0 && bounds.Y == 0) ? 1 : (float)Math.Min(thumbGrid.Size.Width / bounds.X, thumbGrid.Size.Height / bounds.Y);
			e.Graphics.Transform = new Matrix(zoom, 0, 0, zoom, 0, 0);

			// render network
			m_nodeController.RenderNetwork(e.Graphics, renderOptions);

			// render routing details
			BunchOfNodes start = lbStartNodes.SelectedItem as BunchOfNodes;
			BunchOfNodes destination = lbDestinationNodes.SelectedItem as BunchOfNodes;
			if (start != null && destination != null)
				{
				// render shortest connection if one exists
				if (start.nodes.Count > 0)
					{
					Routing route = Routing.CalculateShortestConenction(start.nodes[0], destination.nodes, Vehicle.IVehicle.VehicleTypes.CAR);

					using (Pen orangePen = new Pen(Color.Orange, 4/zoom))
						{
						foreach (Routing.RouteSegment rs in route)
							{
							if (!rs.lineChangeNeeded)
								{
								NodeConnection nextNC = rs.startConnection;
								e.Graphics.DrawBezier(orangePen, nextNC.lineSegment.p0, nextNC.lineSegment.p1, nextNC.lineSegment.p2, nextNC.lineSegment.p3);
								}
							else
								{
								e.Graphics.DrawLine(orangePen, rs.startConnection.startNode.position, rs.nextNode.position);
								}
							}
						}
					}
	
				// render start- / destination nodes
				foreach (LineNode ln in start.nodes)
					{
					RectangleF foo = ln.positionRect;
					foo.Inflate(4 / zoom, 4 / zoom);
					e.Graphics.FillEllipse(new SolidBrush(Color.Green), foo);
					}
				foreach (LineNode ln in destination.nodes)
					{
					RectangleF foo = ln.positionRect;
					foo.Inflate(4 / zoom, 4 / zoom);
					e.Graphics.FillEllipse(new SolidBrush(Color.Red), foo);
					}
				}

			}
		}
	}
