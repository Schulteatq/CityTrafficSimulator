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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Crownwood.Magic.Common;
using Crownwood.Magic.Docking;

using CityTrafficSimulator.Timeline;
using CityTrafficSimulator.Vehicle;
using CityTrafficSimulator.Tools.ObserverPattern;


namespace CityTrafficSimulator
    {
	/// <summary>
	/// Hauptformular von CityTrafficSimulator
	/// </summary>
	public partial class MainForm : Form
		{
		#region Docking stuff

		/// <summary>
		/// The one and only holy DockingManager
		/// </summary>
		private Crownwood.Magic.Docking.DockingManager _dockingManager;

		private void SetContentDefaultSettings(Content c, Size s)
			{
			c.DisplaySize = s;
			c.FloatingSize = s;
			c.AutoHideSize = s;
			c.CloseButton = false;
			}

		private void SetupDockingStuff()
			{
			// Setup main canvas
			_dockingManager.InnerControl = pnlMainGrid;
			_dockingManager.ContentHiding += new DockingManager.ContentHidingHandler(_dockingManager_ContentHiding);
			statusleiste.Visible = false;

			// Setup right docks: Most setup panels
			Content connectionContent = _dockingManager.Contents.Add(pnlConnectionSetup, "Connection Setup");
			SetContentDefaultSettings(connectionContent, pnlConnectionSetup.Size);
			Content networkContent = _dockingManager.Contents.Add(pnlNetworkInfo, "Network Info");
			SetContentDefaultSettings(networkContent, pnlNetworkInfo.Size);
			Content signalContent = _dockingManager.Contents.Add(pnlSignalAssignment, "Signal Assignment");
			SetContentDefaultSettings(signalContent, pnlSignalAssignment.Size);
			Content viewContent = _dockingManager.Contents.Add(pnlRenderSetup, "Render Setup");
			SetContentDefaultSettings(viewContent, pnlRenderSetup.Size);
			Content canvasContent = _dockingManager.Contents.Add(pnlCanvasSetup, "Canvas Setup");
			SetContentDefaultSettings(canvasContent, pnlCanvasSetup.Size);
			Content simContent = _dockingManager.Contents.Add(pnlSimulationSetup, "Simulation Setup");
			SetContentDefaultSettings(simContent, pnlSimulationSetup.Size);
			Content thumbContent = _dockingManager.Contents.Add(thumbGrid, "Thumbnail View");
			SetContentDefaultSettings(thumbContent, new Size(150, 150));
			
			WindowContent dock0 = _dockingManager.AddContentWithState(connectionContent, State.DockRight);
			_dockingManager.AddContentToWindowContent(signalContent, dock0);

			WindowContent dock1 = _dockingManager.AddContentToZone(networkContent, dock0.ParentZone, 1) as WindowContent;
			_dockingManager.AddContentToWindowContent(simContent, dock1);

			WindowContent dock2 = _dockingManager.AddContentToZone(thumbContent, dock0.ParentZone, 2) as WindowContent;
			_dockingManager.AddContentToWindowContent(viewContent, dock2); 
			_dockingManager.AddContentToWindowContent(canvasContent, dock2);


			// Setup bottom docks: TrafficLightForm, TrafficVolumeForm, pnlTimeline
			trafficLightForm = new TrafficLightForm(timelineSteuerung);
			trafficVolumeForm = new Verkehr.TrafficVolumeForm(trafficVolumeSteuerung, this, nodeSteuerung);

			Content tlfContent = _dockingManager.Contents.Add(trafficLightForm, "Signal Editor");
			SetContentDefaultSettings(tlfContent, trafficLightForm.Size);
			Content tvfContent = _dockingManager.Contents.Add(trafficVolumeForm, "Traffic Volume Editor");
			SetContentDefaultSettings(tvfContent, trafficVolumeForm.Size);

			WindowContent bottomDock = _dockingManager.AddContentWithState(tlfContent, State.DockBottom);
			_dockingManager.AddContentToWindowContent(tvfContent, bottomDock);

			}

		void _dockingManager_ContentHiding(Content c, CancelEventArgs cea)
			{
			cea.Cancel = true;
			}

		#endregion

		#region Hilfsklassen
		private enum DragNDrop
			{
			NONE, 
			MOVE_NODES, 
			CREATE_NODE, 
			MOVE_IN_SLOPE, MOVE_OUT_SLOPE, 
			MOVE_TIMELINE_BAR, MOVE_EVENT, MOVE_EVENT_START, MOVE_EVENT_END, 
			MOVE_THUMB_RECT,
			DRAG_RUBBERBAND
			}

		/// <summary>
		/// MainForm invalidation level
		/// </summary>
		public enum InvalidationLevel
			{
			/// <summary>
			/// invalidate everything
			/// </summary>
			ALL,
			/// <summary>
			/// invalidate only main canvas
			/// </summary>
			ONLY_MAIN_CANVAS,
			/// <summary>
			/// invalidate main canvas and timeline
			/// </summary>
			MAIN_CANVAS_AND_TIMLINE
			}

		#endregion

		#region Variablen / Properties
		private Random rnd = new Random();

		private bool dockToGrid = false;

		private NodeSteuerung.RenderOptions renderOptionsDaGrid = new NodeSteuerung.RenderOptions();
		private NodeSteuerung.RenderOptions renderOptionsThumbnail = new NodeSteuerung.RenderOptions();
		
		private Bitmap backgroundImage;
		private Bitmap resampledBackgroundImage;

		private DragNDrop howToDrag = DragNDrop.NONE;

		private Rectangle daGridRubberband;

		/// <summary>
		/// AutoscrollPosition vom daGrid umschließenden Panel. (Wird für Thumbnailanzeige benötigt)
		/// </summary>
		private Point daGridAutoscrollPosition = new Point();

		/// <summary>
		/// Mittelpunkt der angezeigten Fläche in Weltkoordinaten. (Wird für Zoom benötigt)
		/// </summary>
		private Point daGridZoomPosition = new Point();

		private List<GraphicsPath> additionalGraphics = new List<GraphicsPath>();

		private float[,] zoomMultipliers = new float[,] {
			{ 0.1f, 10},
			{ 0.15f, 1f/0.15f},
			{ 0.2f, 5},
			{ 0.25f, 4},
			{ 1f/3f, 3},
			{ 0.5f, 2},
			{ 2f/3f, 1.5f},
			{ 1, 1},
			{ 1.5f, 2f/3f},
			{ 2, 0.5f}};

		private Point mouseDownPosition;


		/// <summary>
		/// Flag, ob OnAfterSelect() seine Arbeit tun darf
		/// </summary>
		private bool doHandleTrafficLightTreeViewSelect = true;


		/// <summary>
		/// currently selected start nodes for traffic volume
		/// </summary>
		private List<LineNode> m_fromLineNodes = new List<LineNode>();
		/// <summary>
		/// currently selected start nodes for traffic volume
		/// </summary>
		public List<LineNode> fromLineNodes
			{
			get { return m_fromLineNodes; }
			set { m_fromLineNodes = value; Invalidate(InvalidationLevel.ALL); }
			}

		/// <summary>
		/// currently selected destination nodes for traffic volume
		/// </summary>
		private List<LineNode> m_toLineNodes = new List<LineNode>();
		/// <summary>
		/// currently selected destination nodes for traffic volume
		/// </summary>
		public List<LineNode> toLineNodes
			{
			get { return m_toLineNodes; }
			set { m_toLineNodes = value; Invalidate(InvalidationLevel.ALL); }
			}


		/// <summary>
		/// Thumbnail Rectangle World-Koordinaten
		/// </summary>
		private Rectangle thumbGridWorldRect;
		/// <summary>
		/// Thumbnail Rectangle Client-Koordinaten
		/// </summary>
		private Rectangle thumbGridClientRect;

		/// <summary>
		/// vorläufige Standardgruppe für LSA
		/// </summary>
		private TimelineGroup unsortedGroup = new TimelineGroup("unsortierte LSA", false);


		/// <summary>
		/// NodeSteuerung
		/// </summary>
		public NodeSteuerung nodeSteuerung = new NodeSteuerung();

		/// <summary>
		/// TimelineSteuerung
		/// </summary>
		private TimelineSteuerung timelineSteuerung = new TimelineSteuerung();

		/// <summary>
		/// TrafficVolumeSteuerung
		/// </summary>
		private Verkehr.VerkehrSteuerung trafficVolumeSteuerung = new Verkehr.VerkehrSteuerung();

		/// <summary>
		/// Formular zur LSA-Steuerung
		/// </summary>
		private TrafficLightForm trafficLightForm;

		/// <summary>
		/// TrafficVolumeForm
		/// </summary>
		private Verkehr.TrafficVolumeForm trafficVolumeForm;


		/// <summary>
		/// speichert den ursprünglichen Slope beim Bearbeiten den In-/OutSlopes von LineNodes
		/// </summary>
		private List<Vector2> originalSlopes = new List<Vector2>();


		/// <summary>
		/// speichert die Positionen der vor dem Erstellen von neuen LineNodes selektierten LineNodes
		/// </summary>
		private List<Vector2> previousSelectedNodePositions = new List<Vector2>();

		/// <summary>
		/// speichert die relativen Positionsangaben der selektierten LineNodes zur Clickposition beim Drag'n'Drop bzw. beim erstellen von LineNodes
		/// </summary>
		private List<Vector2> selectedLineNodesMovingOffset = new List<Vector2>();

		/// <summary>
		/// aktuell ausgewählter LineNode
		/// </summary>
		private List<LineNode> m_selectedLineNodes = new List<LineNode>();
		/// <summary>
		/// aktuell ausgewählter LineNode
		/// </summary>
		public List<LineNode> selectedLineNodes
			{
			get { return m_selectedLineNodes; }
			set
				{
				m_selectedLineNodes = value;
				if (m_selectedLineNodes != null && m_selectedLineNodes.Count == 1)
					{
					// TrafficLight Einstellungen in die Form setzen
					if (m_selectedLineNodes[0].tLight != null)
						{
						doHandleTrafficLightTreeViewSelect = false;
						trafficLightTreeView.SelectNodeByTimelineEntry(m_selectedLineNodes[0].tLight);
						trafficLightTreeView.Select();
						doHandleTrafficLightTreeViewSelect = true;
						trafficLightForm.selectedEntry = m_selectedLineNodes[0].tLight;
						}
					else
						{
						trafficLightTreeView.SelectedNode = null;
						trafficLightForm.selectedEntry = null;
						}

					selectedNodeConnection = null;
					}
				else
					{
					trafficLightTreeView.SelectedNode = null;
					trafficLightForm.selectedEntry = null;
					}
				}
			}

		/// <summary>
		/// aktuell ausgewählte NodeConnection
		/// </summary>
		private NodeConnection m_selectedNodeConnection;
		/// <summary>
		/// aktuell ausgewählte NodeConnection
		/// </summary>
		public NodeConnection selectedNodeConnection
			{
			get { return m_selectedNodeConnection; }
			set
				{
				m_selectedNodeConnection = value;
				if (m_selectedNodeConnection != null)
					{
					// NodeConnection Einstellungen setzen
					nodeConnectionPrioritySpinEdit.Value = m_selectedNodeConnection.priority;
					carsAllowedCheckBox.Checked = m_selectedNodeConnection.carsAllowed;
					busAllowedCheckBox.Checked = m_selectedNodeConnection.busAllowed;
					tramAllowedCheckBox.Checked = m_selectedNodeConnection.tramAllowed;
					enableOutgoingLineChangeCheckBox.Checked = m_selectedNodeConnection.enableOutgoingLineChange;
					enableIncomingLineChangeCheckBox.Checked = m_selectedNodeConnection.enableIncomingLineChange;
					spinTargetVelocity.Value = (decimal)m_selectedNodeConnection.targetVelocity;

					selectedLineNodes.Clear();
					}
				}
			}

		/// <summary>
		/// aktuell ausgewähltes IVehicle
		/// </summary>
		private IVehicle m_selectedVehicle;
		/// <summary>
		/// aktuell ausgewähltes IVehicle
		/// </summary>
		public IVehicle selectedVehicle
			{
			get { return m_selectedVehicle; }
			set { m_selectedVehicle = value; }
			}


		/// <summary>
		/// Stopwatch zur Zeitmessung des renderings
		/// </summary>
		private System.Diagnostics.Stopwatch renderStopwatch = new System.Diagnostics.Stopwatch();

		/// <summary>
		/// Stopwatch zur Zeitmessung der Verkehrslogik
		/// </summary>
		private System.Diagnostics.Stopwatch thinkStopwatch = new System.Diagnostics.Stopwatch();

		#endregion


		/// <summary>
		/// Standardkonstruktor des Hauptformulars
		/// </summary>
		public MainForm()
			{
			timelineSteuerung.maxTime = 50;

			InitializeComponent();

			_dockingManager = new Crownwood.Magic.Docking.DockingManager(this, VisualStyle.IDE);
			SetupDockingStuff();

			trafficLightTreeView.steuerung = timelineSteuerung;
			timelineSteuerung.CurrentTimeChanged += new TimelineSteuerung.CurrentTimeChangedEventHandler(timelineSteuerung_CurrentTimeChanged);
			trafficLightForm.SelectedEntryChanged += new TrafficLightForm.SelectedEntryChangedEventHandler(trafficLightForm_SelectedEntryChanged);

			zoomComboBox.SelectedIndex = 7;
			DaGrid.Size = new System.Drawing.Size((int)canvasWidthSpinEdit.Value, (int)canvasHeigthSpinEdit.Value);

			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

			renderOptionsDaGrid.renderLineNodes = true;
			renderOptionsDaGrid.renderNodeConnections = true;
			renderOptionsDaGrid.renderVehicles = true;
			renderOptionsDaGrid.performClipping = true;
			renderOptionsDaGrid.clippingRect = new Rectangle(0, 0, 10000, 10000);
			renderOptionsDaGrid.renderIntersections = false;
			renderOptionsDaGrid.renderLineChangePoints = false;
			renderOptionsDaGrid.renderLineNodeDebugData = false;
			renderOptionsDaGrid.renderNodeConnectionDebugData = false;
			renderOptionsDaGrid.renderVehicleDebugData = false;

			renderOptionsThumbnail.renderLineNodes = false;
			renderOptionsThumbnail.renderNodeConnections = true;
			renderOptionsThumbnail.renderVehicles = false;
			renderOptionsThumbnail.performClipping = false;
			renderOptionsThumbnail.renderIntersections = false;
			renderOptionsThumbnail.renderLineChangePoints = false;
			renderOptionsThumbnail.renderLineNodeDebugData = false;
			renderOptionsThumbnail.renderNodeConnectionDebugData = false;
			renderOptionsThumbnail.renderVehicleDebugData = false;

			}

		private void trafficLightForm_SelectedEntryChanged(object sender, TrafficLightForm.SelectedEntryChangedEventArgs e)
			{
			if (trafficLightForm.selectedEntry != null)
				{
				TrafficLight tl = trafficLightForm.selectedEntry as TrafficLight;
				m_selectedLineNodes.Clear();
				m_selectedLineNodes.AddRange(tl.assignedNodes);
				DaGrid.Invalidate();
				}
			}

		private void timelineSteuerung_CurrentTimeChanged(object sender, TimelineSteuerung.CurrentTimeChangedEventArgs e)
			{
			DaGrid.Invalidate();
			}

		#region Drag'n'Drop Gedöns

		#region thumbGrid
		private void thumbGrid_MouseMove(object sender, MouseEventArgs e)
			{
			// Mauszeiger ändern, falls über TimelineBar
			if (thumbGridClientRect.Contains(e.Location))
				{
				this.Cursor = Cursors.SizeAll;
				}
			else
				{
				this.Cursor = Cursors.Default;
				}

			if (howToDrag == DragNDrop.MOVE_THUMB_RECT)
				{
				thumbGridClientRect.X = e.Location.X + mouseDownPosition.X;
				thumbGridClientRect.Y = e.Location.Y + mouseDownPosition.Y;
				thumbGrid.Invalidate();
				}
			}

		private void thumbGrid_MouseLeave(object sender, EventArgs e)
			{
			this.Cursor = Cursors.Default;
			}

		private void thumbGrid_MouseDown(object sender, MouseEventArgs e)
			{
			if (!thumbGridClientRect.Contains(e.Location))
				{
				float zoom = (float)DaGrid.ClientSize.Width / thumbGrid.ClientSize.Width;
				thumbGridClientRect.X = e.Location.X - thumbGridClientRect.Width/2;
				thumbGridClientRect.Y = e.Location.Y - thumbGridClientRect.Height/2;

				pnlMainGrid.AutoScrollPosition = new Point(
					(int)Math.Round(zoom * thumbGridClientRect.X * zoomMultipliers[zoomComboBox.SelectedIndex, 0]),
					(int)Math.Round(zoom * thumbGridClientRect.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 0]));


				UpdateDaGridClippingRect();
				DaGrid.Invalidate(true);
				}

			mouseDownPosition = new Point(thumbGridClientRect.X - e.Location.X, thumbGridClientRect.Y - e.Location.Y);
			howToDrag = DragNDrop.MOVE_THUMB_RECT;

			}

		private void thumbGrid_MouseUp(object sender, MouseEventArgs e)
			{
			if (howToDrag == DragNDrop.MOVE_THUMB_RECT)
				{
				float zoom = (float)DaGrid.ClientSize.Width / thumbGrid.ClientSize.Width;

				/*thumbGridWorldRect = new Rectangle(
					(int)Math.Round(-daGridAutoscrollPosition.X * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(-daGridAutoscrollPosition.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(pnlMainGrid.ClientSize.Width * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(pnlMainGrid.ClientSize.Height * zoomMultipliers[zoomComboBox.SelectedIndex, 1]));
				*/


				pnlMainGrid.AutoScrollPosition = new Point(
					(int)Math.Round(zoom * thumbGridClientRect.X * zoomMultipliers[zoomComboBox.SelectedIndex, 0]),
					(int)Math.Round(zoom * thumbGridClientRect.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 0]));

				UpdateDaGridClippingRect();
				DaGrid.Invalidate(true);
				}
			howToDrag = DragNDrop.NONE;
			}

		#endregion

		#region DaGrid
		void DaGrid_MouseDown(object sender, MouseEventArgs e)
			{
			Vector2 clickedPosition = new Vector2(e.X, e.Y);
			clickedPosition *= zoomMultipliers[zoomComboBox.SelectedIndex, 1];


			// Node Gedöns
			switch (e.Button)
				{
				case MouseButtons.Left:
					#region Nodes hinzufügen
					if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
						{
						// LineNode hinzufügen
						List<LineNode> nodesToAdd = new List<LineNode>(m_selectedLineNodes.Count);
						selectedLineNodesMovingOffset.Clear();

						// testen ob ein Node schon markiert ist
						if (selectedLineNodes.Count > 0)
							{
							// Mittelpunkt des selektierten LineNodes ermitteln
							Vector2 midpoint = new Vector2(0, 0);
							foreach (LineNode ln in selectedLineNodes)
								{
								midpoint += ln.position;
								}
							midpoint *= (double)1 / selectedLineNodes.Count;

							// Line Node nach SelectedLineNode einfügen
							if (!((Control.ModifierKeys & Keys.Shift) == Keys.Shift))
								{
								// ersten Line Node erstellen
								nodesToAdd.Add(new LineNode(DaGrid.DockToGrid(clickedPosition, dockToGrid)));
								selectedLineNodesMovingOffset.Add(m_selectedLineNodes[0].outSlope);

								// in/outSlope berechnen
								nodesToAdd[0].outSlope = 30 * Vector2.Normalize(nodesToAdd[0].position - midpoint);
								nodesToAdd[0].inSlope = -1 * nodesToAdd[0].outSlope;

								// Connecten
								nodeSteuerung.Connect(
									m_selectedLineNodes[0],
									nodesToAdd[0],
									(int)nodeConnectionPrioritySpinEdit.Value,
									carsAllowedCheckBox.Checked,
									busAllowedCheckBox.Checked,
									tramAllowedCheckBox.Checked,
									enableIncomingLineChangeCheckBox.Checked,
									enableOutgoingLineChangeCheckBox.Checked);


								// nun die restlichen LineNodes parallel erstellen
								for (int i = 1; i < m_selectedLineNodes.Count; i++)
									{
									Vector2 offset = m_selectedLineNodes[0].position - m_selectedLineNodes[i].position;
									selectedLineNodesMovingOffset.Add(offset);

									// Line Node erstellen
									nodesToAdd.Add(new LineNode(DaGrid.DockToGrid(clickedPosition - offset, dockToGrid)));

									// in/outSlope berechnen
									nodesToAdd[i].outSlope = 30 * Vector2.Normalize(nodesToAdd[i].position - midpoint);
									nodesToAdd[i].inSlope = -1 * nodesToAdd[i].outSlope;

									// Connecten
									nodeSteuerung.Connect(
										m_selectedLineNodes[i],
										nodesToAdd[i],
										(int)nodeConnectionPrioritySpinEdit.Value,
										carsAllowedCheckBox.Checked,
										busAllowedCheckBox.Checked,
										tramAllowedCheckBox.Checked,
										enableIncomingLineChangeCheckBox.Checked,
										enableOutgoingLineChangeCheckBox.Checked);

									}
								}
							// Line Node vor SelectedLineNode einfügen
							else
								{
								// ersten Line Node erstellen
								nodesToAdd.Add(new LineNode(DaGrid.DockToGrid(clickedPosition, dockToGrid)));
								selectedLineNodesMovingOffset.Add(m_selectedLineNodes[0].outSlope);


								// in/outSlope berechnen
								nodesToAdd[0].outSlope = 30 * Vector2.Normalize(nodesToAdd[0].position - midpoint);
								nodesToAdd[0].inSlope = -1 * nodesToAdd[0].outSlope;

								// Connecten
								nodeSteuerung.Connect(
									nodesToAdd[0],
									m_selectedLineNodes[0],
									(int)nodeConnectionPrioritySpinEdit.Value,
									carsAllowedCheckBox.Checked,
									busAllowedCheckBox.Checked,
									tramAllowedCheckBox.Checked,
									enableIncomingLineChangeCheckBox.Checked,
									enableOutgoingLineChangeCheckBox.Checked);


								// nun die restlichen LineNodes parallel erstellen
								for (int i = 1; i < m_selectedLineNodes.Count; i++)
									{
									Vector2 offset = m_selectedLineNodes[0].position - m_selectedLineNodes[i].position;
									selectedLineNodesMovingOffset.Add(offset);

									// Line Node erstellen
									nodesToAdd.Add(new LineNode(DaGrid.DockToGrid(clickedPosition - offset, dockToGrid)));

									// in/outSlope berechnen
									nodesToAdd[i].outSlope = 30 * Vector2.Normalize(nodesToAdd[i].position - midpoint);
									nodesToAdd[i].inSlope = -1 * nodesToAdd[i].outSlope;

									// Connecten
									nodeSteuerung.Connect(
										nodesToAdd[i],
										m_selectedLineNodes[i],
										(int)nodeConnectionPrioritySpinEdit.Value,
										carsAllowedCheckBox.Checked,
										busAllowedCheckBox.Checked,
										tramAllowedCheckBox.Checked,
										enableIncomingLineChangeCheckBox.Checked,
										enableOutgoingLineChangeCheckBox.Checked);

									}
								}
							}
						else
							{
							// ersten Line Node erstellen
							nodesToAdd.Add(new LineNode(DaGrid.DockToGrid(clickedPosition, dockToGrid)));
							selectedLineNodesMovingOffset.Add(new Vector2(0, 0));
							}

						previousSelectedNodePositions.Clear();
						foreach (LineNode ln in m_selectedLineNodes)
							{
							previousSelectedNodePositions.Add(ln.position);
							}

						selectedLineNodes.Clear();
						foreach (LineNode ln in nodesToAdd)
							{
							nodeSteuerung.AddLineNode(ln);
							selectedLineNodes.Add(ln);
							}
						howToDrag = DragNDrop.CREATE_NODE;
						}
					#endregion

					#region Nodes Verbinden
					else if (((Control.ModifierKeys & Keys.Alt) == Keys.Alt) && (selectedLineNodes != null))
						{
						LineNode nodeToConnectTo = nodeSteuerung.GetLineNodeAt(clickedPosition);

						if (nodeToConnectTo != null)
							{
							foreach (LineNode ln in selectedLineNodes)
								{
								nodeSteuerung.Connect(
									ln, 
									nodeToConnectTo, 
									(int)nodeConnectionPrioritySpinEdit.Value, 
									carsAllowedCheckBox.Checked, 
									busAllowedCheckBox.Checked, 
									tramAllowedCheckBox.Checked, 
									enableIncomingLineChangeCheckBox.Checked,
									enableOutgoingLineChangeCheckBox.Checked);
								}						
							}
						}
					#endregion

					#region Nodes selektieren bzw. verschieben
					else
						{
						bool found = false;

						if (!lockNodesCheckBox.Checked)
							{
							// erst gucken, ob evtl. In/OutSlopes angeklickt wurden
							if (selectedLineNodes != null && selectedLineNodes.Count >= 1)
								{
									if (m_selectedLineNodes[0].inSlopeRect.Contains(clickedPosition))
										{
										originalSlopes.Clear();
										foreach (LineNode ln in m_selectedLineNodes)
											{
											originalSlopes.Add(ln.inSlope);
											}
										
										howToDrag = DragNDrop.MOVE_IN_SLOPE;
										found = true;
										}
									if (m_selectedLineNodes[0].outSlopeRect.Contains(clickedPosition))
										{
										originalSlopes.Clear();
										foreach (LineNode ln in m_selectedLineNodes)
											{
											originalSlopes.Add(ln.outSlope);
											}

										howToDrag = DragNDrop.MOVE_OUT_SLOPE;
										found = true;
										}
								}
							}

						if (! found)
							{
							LineNode ln = nodeSteuerung.GetLineNodeAt(clickedPosition);
							if (ln != null && !lockNodesCheckBox.Checked)
								{
								if (selectedLineNodes.Contains(ln))
									{
									// MovingOffsets berechnen:
									selectedLineNodesMovingOffset.Clear();
									foreach (LineNode lln in m_selectedLineNodes)
										{
										selectedLineNodesMovingOffset.Add(lln.position - clickedPosition);
										}

									howToDrag = DragNDrop.MOVE_NODES;
									}
								else
									{
									// Häßlicher Workaround, um Settermethode für selectedLineNodes aufzurufen
									List<LineNode> foo = new List<LineNode>();
 									foo.Add(ln);
									selectedLineNodes = foo;

									// bei nur einem Punkt brauchen wir keine MovingOffsets
									selectedLineNodesMovingOffset.Clear();
									selectedLineNodesMovingOffset.Add(new Vector2(0, 0));

									howToDrag = DragNDrop.MOVE_NODES;
									}
								}
							else
								{
								selectedLineNodes = new List<LineNode>();
								howToDrag = DragNDrop.DRAG_RUBBERBAND;

								daGridRubberband.Location = clickedPosition;
								daGridRubberband.Width = 1;
								daGridRubberband.Height = 1;
								}
							}
						}
					#endregion
					break;
				case MouseButtons.Right:
					#region Nodes löschen
					if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
						{
						// LineNode entfernen
						LineNode nodeToDelete = nodeSteuerung.GetLineNodeAt(clickedPosition);
						// checken ob gefunden
						if (nodeToDelete != null)
							{
							if (selectedLineNodes.Contains(nodeToDelete))
								{
								selectedLineNodes.Remove(nodeToDelete);
								}
							nodeSteuerung.DeleteLineNode(nodeToDelete);
							}
						}
					#endregion

					break;
				}

			Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
			}

		void DaGrid_MouseMove(object sender, MouseEventArgs e)
			{
			Vector2 clickedPosition = new Vector2(e.X, e.Y);
			clickedPosition *= zoomMultipliers[zoomComboBox.SelectedIndex, 1];

			if (selectedLineNodes != null)
				{
				switch (howToDrag)
					{
					case DragNDrop.MOVE_NODES:
						for (int i = 0; i < m_selectedLineNodes.Count; i++)
							{
							m_selectedLineNodes[i].position = DaGrid.DockToGrid(clickedPosition + selectedLineNodesMovingOffset[i], dockToGrid);
							}

						Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
						break;
					case DragNDrop.CREATE_NODE:
						selectedLineNodes[0].outSlopeAbs = DaGrid.DockToGrid(clickedPosition, dockToGrid);
						selectedLineNodes[0].inSlope = -1 * selectedLineNodes[0].outSlope;

						for (int i = 1; i < m_selectedLineNodes.Count; i++)
							{
							// Rotation des offsets im Vergleich zum outSlope berechnen
							double rotation = Math.Atan2(selectedLineNodesMovingOffset[i].Y, selectedLineNodesMovingOffset[i].X) - Math.Atan2(selectedLineNodesMovingOffset[0].Y, selectedLineNodesMovingOffset[0].X);

							m_selectedLineNodes[i].position = m_selectedLineNodes[0].position - (m_selectedLineNodes[0].outSlope.RotateCounterClockwise(rotation).Normalized * selectedLineNodesMovingOffset[i].Abs);

							double streckungsfaktor = Math.Pow((m_selectedLineNodes[i].position - previousSelectedNodePositions[i]).Abs / (m_selectedLineNodes[0].position - previousSelectedNodePositions[0]).Abs, 2);

							m_selectedLineNodes[i].outSlope = m_selectedLineNodes[0].outSlope * streckungsfaktor;
							m_selectedLineNodes[i].inSlope = m_selectedLineNodes[0].inSlope * streckungsfaktor;
							}

						Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
						break;
					case DragNDrop.MOVE_IN_SLOPE:
						selectedLineNodes[0].inSlopeAbs = DaGrid.DockToGrid(clickedPosition, dockToGrid);

						// sind mehr als ein LineNode markiert, so sollen die inSlopes aller anderen LineNodes angepasst werden.
						if (m_selectedLineNodes.Count > 1)
							{
							// zur relativen Anpassung eignen sich Polarkoordinaten besonders gut, wir berechnen zunächst die Änderungen
							double streckungsfaktor = m_selectedLineNodes[0].inSlope.Abs / originalSlopes[0].Abs;
							double rotation = Math.Atan2(m_selectedLineNodes[0].inSlope.Y, m_selectedLineNodes[0].inSlope.X) - Math.Atan2(originalSlopes[0].Y, originalSlopes[0].X);

							for (int i = 0; i < m_selectedLineNodes.Count; i++)
								{
								if (i > 0)
									{
									m_selectedLineNodes[i].inSlope = originalSlopes[i].RotateCounterClockwise(rotation);
									m_selectedLineNodes[i].inSlope *= streckungsfaktor;
									}
								}
							}


						if (!((Control.ModifierKeys & Keys.Alt) == Keys.Alt))
							{
							foreach (LineNode ln in m_selectedLineNodes)
								{
								ln.outSlope = -1 * ln.inSlope;
								}
							}
						Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
						break;
					case DragNDrop.MOVE_OUT_SLOPE:
						selectedLineNodes[0].outSlopeAbs = DaGrid.DockToGrid(clickedPosition, dockToGrid);

						// sind mehr als ein LineNode markiert, so sollen die outSlopes aller anderen LineNodes angepasst werden.
						if (m_selectedLineNodes.Count > 1)
							{
							// zur relativen Anpassung eignen sich Polarkoordinaten besonders gut, wir berechnen zunächst die Änderungen
							double streckungsfaktor = m_selectedLineNodes[0].outSlope.Abs / originalSlopes[0].Abs;
							double rotation = Math.Atan2(m_selectedLineNodes[0].outSlope.Y, m_selectedLineNodes[0].outSlope.X) - Math.Atan2(originalSlopes[0].Y, originalSlopes[0].X);

							for (int i = 0; i < m_selectedLineNodes.Count; i++)
								{
								if (i > 0)
									{
									m_selectedLineNodes[i].outSlope = originalSlopes[i].RotateCounterClockwise(rotation);
									m_selectedLineNodes[i].outSlope *= streckungsfaktor;
									}
								}
							}

						if (!((Control.ModifierKeys & Keys.Alt) == Keys.Alt))
							{
							foreach (LineNode ln in m_selectedLineNodes)
								{
								ln.inSlope = -1 * ln.outSlope;
								}							
							}
						Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
						break;
					case DragNDrop.DRAG_RUBBERBAND:
						daGridRubberband.Width = (int) Math.Round(clickedPosition.X - daGridRubberband.X);
						daGridRubberband.Height = (int) Math.Round(clickedPosition.Y - daGridRubberband.Y);
						Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
						break;
					default:
						break;
					}
				}
			else
				{
				}
			}

		void DaGrid_MouseUp(object sender, MouseEventArgs e)
			{
			Vector2 clickedPosition = new Vector2(e.X, e.Y);
			clickedPosition *= zoomMultipliers[zoomComboBox.SelectedIndex, 1];

			switch (howToDrag)
				{
				case DragNDrop.DRAG_RUBBERBAND:
					// nun nochLineNode markieren
					if (Math.Abs(daGridRubberband.Width) > 2 && Math.Abs(daGridRubberband.Height) > 2)
						{
						// Rubberband normalisieren:
						if (daGridRubberband.Width < 0)
							{
							daGridRubberband.X += daGridRubberband.Width;
							daGridRubberband.Width *= -1;
							}
						if (daGridRubberband.Height < 0)
							{
							daGridRubberband.Y += daGridRubberband.Height;
							daGridRubberband.Height *= -1;
							}

						selectedLineNodes = nodeSteuerung.GetLineNodesAt(daGridRubberband);
						}
					else 
						{
						selectedNodeConnection = nodeSteuerung.GetNodeConnectionAt(clickedPosition);
						selectedVehicle = nodeSteuerung.GetVehicleAt(clickedPosition);
						} 
					break;
				default:
					break;
				}

			if ((howToDrag == DragNDrop.CREATE_NODE || howToDrag == DragNDrop.MOVE_NODES || howToDrag == DragNDrop.MOVE_IN_SLOPE || howToDrag == DragNDrop.MOVE_OUT_SLOPE) && m_selectedLineNodes != null)
				{
				nodeSteuerung.UpdateNodeConnections(m_selectedLineNodes);
				}

			// Drag'n'Drop Bereich wieder löschen
			howToDrag = DragNDrop.NONE;
			Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
			}

		void DaGrid_KeyDown(object sender, KeyEventArgs e)
			{
			// Tastenbehandlung
			switch (e.KeyCode)
				{
			#region Nodes verschieben
			// Node verschieben
			case Keys.Left:
				foreach (LineNode ln in selectedLineNodes)
					{
					ln.position.X -= 1;
					e.Handled = true;
					Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
					}
				break;
			case Keys.Right:
				foreach (LineNode ln in selectedLineNodes)
					{
					ln.position.X += 1;
					e.Handled = true;
					Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
					}
				break;
			case Keys.Up:
				foreach (LineNode ln in selectedLineNodes)
					{
					ln.position.Y -= 1;
					e.Handled = true;
					Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
					}
				break;
			case Keys.Down:
				foreach (LineNode ln in selectedLineNodes)
					{
					ln.position.Y += 1;
					e.Handled = true;
					Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
					}
				break;
			#endregion

			#region Nodes durchwandern
			// TODO: nächster Node
			case Keys.PageDown:
				break;
			// TODO: vorheriger Node
			case Keys.PageUp:
				break;
			#endregion

			#region Nodes bearbeiten
			// Node löschen
			case Keys.Delete:
				if (selectedVehicle != null)
					{
					selectedVehicle.currentNodeConnection.RemoveVehicle(selectedVehicle);
					}
				else // do not delete nodes and connections when vehicle selected!
					{
					foreach (LineNode ln in selectedLineNodes)
						{
						nodeSteuerung.DeleteLineNode(ln);
						}
					selectedLineNodes.Clear();
					e.Handled = true;
					Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);

					if (selectedNodeConnection != null)
						{
						nodeSteuerung.Disconnect(selectedNodeConnection.startNode, selectedNodeConnection.endNode);
						selectedNodeConnection = null;
						e.Handled = true;
						Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
						}
					}
				break;

			// LineSegment teilen
			case Keys.S:
				if (selectedNodeConnection != null)
					{
					nodeSteuerung.SplitNodeConnection(selectedNodeConnection);
					selectedNodeConnection = null;
					Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
					}
				break;
			case Keys.Return:
				if (selectedNodeConnection != null)
					{
					nodeSteuerung.FindLineChangePoints(selectedNodeConnection, 50, 32);
					Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
					}
				break;

			case Keys.C:
				if (selectedNodeConnection != null)
					{
					carsAllowedCheckBox.Checked = !carsAllowedCheckBox.Checked;
					}
				break;
			case Keys.B:
				if (selectedNodeConnection != null)
					{
					busAllowedCheckBox.Checked = !busAllowedCheckBox.Checked;
					}
				break;
			case Keys.T:
				if (selectedNodeConnection != null)
					{
					tramAllowedCheckBox.Checked = !tramAllowedCheckBox.Checked;
					}
				break;

			case Keys.O:
				if (selectedNodeConnection != null)
					{
					enableOutgoingLineChangeCheckBox.Checked = !enableOutgoingLineChangeCheckBox.Checked;
					enableOutgoingLineChangeCheckBox_Click(this, new EventArgs());
					}
				break;
			case Keys.I:
				if (selectedNodeConnection != null)
					{
					enableIncomingLineChangeCheckBox.Checked = !enableIncomingLineChangeCheckBox.Checked;
					enableIncomingLineChangeCheckBox_Click(this, new EventArgs());
					}
				break;

			// Ampel
			case Keys.A:
				if (selectedLineNodes != null)
					{
					//IsTrafficLightCheckBox.Checked = !IsTrafficLightCheckBox.Checked;
					}
				break;
			#endregion

			#region from-/toLineNodes setzen

			case Keys.V:
				fromLineNodes.Clear();
				foreach (LineNode ln in selectedLineNodes)
					{
					fromLineNodes.Add(ln);
					}
				Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
				break;

			case Keys.N:
				toLineNodes.Clear();
				foreach (LineNode ln in selectedLineNodes)
					{
					toLineNodes.Add(ln);
					}
				Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
				break;
			#endregion

			#region Zoomfaktor ändern
			case Keys.P:
				if (selectedNodeConnection != null)
					{
					nodeConnectionPrioritySpinEdit.Value++;
					break;
					}
				if (((Control.ModifierKeys & Keys.Control) == Keys.Control) && zoomComboBox.SelectedIndex < zoomComboBox.Items.Count - 1)
					{
					zoomComboBox.SelectedIndex += 1;
					}
				break;

			case Keys.M:
				if (selectedNodeConnection != null)
					{
					nodeConnectionPrioritySpinEdit.Value--;
					break;
					}
				if (((Control.ModifierKeys & Keys.Control) == Keys.Control) && zoomComboBox.SelectedIndex > 0)
					{
					zoomComboBox.SelectedIndex -= 1;
					}
				break;

			case Keys.Add:
				if (selectedNodeConnection != null)
					{
					nodeConnectionPrioritySpinEdit.Value++;
					break;
					}
				if (((Control.ModifierKeys & Keys.Control) == Keys.Control) && zoomComboBox.SelectedIndex < zoomComboBox.Items.Count - 1)
					{
					zoomComboBox.SelectedIndex += 1;
					}
				break;

			case Keys.Subtract:
				if (selectedNodeConnection != null)
					{
					nodeConnectionPrioritySpinEdit.Value--;
					break;
					}
				if (((Control.ModifierKeys & Keys.Control) == Keys.Control) && zoomComboBox.SelectedIndex > 0)
					{
					zoomComboBox.SelectedIndex -= 1;
					}
				break;

			#endregion


			#region Connections bearbeiten

			#endregion
			case Keys.D:
				
				break;
				}
			}
		#endregion
		#endregion

		#region Zeichnen
		void DaGrid_Paint(object sender, PaintEventArgs e)
			{
			// Da pnlMainGrid.OnScroll nicht alles mitbekommt, muss das eben die Paintmethode übernehmen
			if (daGridAutoscrollPosition != pnlMainGrid.AutoScrollPosition)
				{
				daGridZoomPosition = new Point(
					(int)Math.Round((-pnlMainGrid.AutoScrollPosition.X + pnlMainGrid.Width / 2) * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round((-pnlMainGrid.AutoScrollPosition.Y + pnlMainGrid.Height / 2) * zoomMultipliers[zoomComboBox.SelectedIndex, 1]));

				daGridAutoscrollPosition = pnlMainGrid.AutoScrollPosition;

				UpdateDaGridClippingRect();
				UpdateThumbGridRect();
				}

			// TODO: Paint Methode entschlacken und outsourcen?
			switch (renderQualityComboBox.SelectedIndex)
				{
				case 0:
					e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
					e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					break;
				case 1:
					e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
					e.Graphics.InterpolationMode = InterpolationMode.Low;
					break;
				}

			renderStopwatch.Reset();
			renderStopwatch.Start();

			if (resampledBackgroundImage != null)
				{
				resampledBackgroundImage.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
				e.Graphics.DrawImage(resampledBackgroundImage, new Point(0, 0));
				}

			e.Graphics.Transform = new Matrix(zoomMultipliers[zoomComboBox.SelectedIndex, 0], 0, 0, zoomMultipliers[zoomComboBox.SelectedIndex, 0], 0, 0);


			using (Pen BlackPen = new Pen(Color.Black, 1.0F))
				{
				// Zusätzliche Grafiken zeichnen
				foreach (GraphicsPath gp in additionalGraphics)
					{
					e.Graphics.DrawPath(BlackPen, gp);
					}

				nodeSteuerung.RenderNetwork(e.Graphics, renderOptionsDaGrid);

				//to-/fromLineNode malen
				foreach (LineNode ln in toLineNodes)
					{
					RectangleF foo = ln.positionRect;
					foo.Inflate(4, 4);
					e.Graphics.FillEllipse(new SolidBrush(Color.Red), foo);
					}
				foreach (LineNode ln in fromLineNodes)
					{
					RectangleF foo = ln.positionRect;
					foo.Inflate(4, 4);
					e.Graphics.FillEllipse(new SolidBrush(Color.Green), foo);
					}
	
				if (selectedLineNodes.Count >= 1)
					{
					foreach (LineNode ln in selectedLineNodes)
						{
						foreach (GraphicsPath gp in ln.nodeGraphics)
							{
							e.Graphics.DrawPath(BlackPen, gp);
							}

						RectangleF foo = ln.positionRect;
						foo.Inflate(2, 2);
						e.Graphics.FillEllipse(new SolidBrush(Color.Black), foo);
						}

					RectangleF foo2 = m_selectedLineNodes[0].positionRect;
					foo2.Inflate(4, 4);
					e.Graphics.DrawEllipse(BlackPen, foo2);
					}

				// selektierte NodeConnection malen
				if (selectedNodeConnection != null)
					{
					selectedNodeConnection.lineSegment.Draw(e.Graphics, BlackPen);
					}



				// Gummiband zeichnen
				if (howToDrag == DragNDrop.DRAG_RUBBERBAND)
					{
					Point[] points = 
						{
							new Point(daGridRubberband.X, daGridRubberband.Y),
							new Point(daGridRubberband.X + daGridRubberband.Width, daGridRubberband.Y),
							new Point(daGridRubberband.X + daGridRubberband.Width, daGridRubberband.Y + daGridRubberband.Height),
							new Point(daGridRubberband.X, daGridRubberband.Y + daGridRubberband.Height)
						};
					e.Graphics.DrawPolygon(BlackPen, points);
					}

				additionalGraphics.Clear();


				// Statusinfo zeichnen:
				if (selectedVehicle != null && cbRenderVehiclesDebug.Checked)
					{
					selectedVehicle.DrawDebugData(e.Graphics);
					}

				renderStopwatch.Stop();

				if (cbRenderFps.Checked)
					{
					e.Graphics.DrawString(
						"thinking time: " + thinkStopwatch.ElapsedMilliseconds + "ms, possible thoughts per second: " + ((thinkStopwatch.ElapsedMilliseconds != 0) ? (1000 / thinkStopwatch.ElapsedMilliseconds).ToString() : "-"),
						new Font("Arial", 10),
						new SolidBrush(Color.Black),
						-pnlMainGrid.AutoScrollPosition.X * zoomMultipliers[zoomComboBox.SelectedIndex, 1] + 8,
						-pnlMainGrid.AutoScrollPosition.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 1] + 40);

					e.Graphics.DrawString(
						"rendering time: " + renderStopwatch.ElapsedMilliseconds + "ms, possible fps: " + ((renderStopwatch.ElapsedMilliseconds != 0) ? (1000 / renderStopwatch.ElapsedMilliseconds).ToString() : "-"),
						new Font("Arial", 10),
						new SolidBrush(Color.Black),
						-pnlMainGrid.AutoScrollPosition.X * zoomMultipliers[zoomComboBox.SelectedIndex, 1] + 8,
						-pnlMainGrid.AutoScrollPosition.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 1] + 56);
					}
				}
			}

		private void thumbGrid_Paint(object sender, PaintEventArgs e)
			{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.InterpolationMode = InterpolationMode.Bilinear;

			// Zoomfaktor berechnen
			float zoom = (float)thumbGrid.ClientSize.Width / DaGrid.ClientSize.Width;

			e.Graphics.Transform = new Matrix(zoom, 0, 0, zoom, 0, 0);


			using (Pen BlackPen = new Pen(Color.Black, 1.0F))
				{
				nodeSteuerung.RenderNetwork(e.Graphics, renderOptionsThumbnail);

				if (fromLineNodes.Count > 0 && toLineNodes.Count > 0)
					{
					Routing route = Routing.CalculateShortestConenction(fromLineNodes[0], toLineNodes, Vehicle.IVehicle.VehicleTypes.CAR);

					using (Pen orangePen = new Pen(Color.Orange, 4 / zoom))
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

				//to-/fromLineNode malen
				foreach (LineNode ln in toLineNodes)
					{
					RectangleF foo = ln.positionRect;
					foo.Inflate(4 / zoom, 4 / zoom);
					e.Graphics.FillEllipse(new SolidBrush(Color.Red), foo);
					}
				foreach (LineNode ln in fromLineNodes)
					{
					RectangleF foo = ln.positionRect;
					foo.Inflate(4 / zoom, 4 / zoom);
					e.Graphics.FillEllipse(new SolidBrush(Color.Green), foo);
					}

				e.Graphics.Transform = new Matrix(1, 0, 0, 1, 0, 0);
				e.Graphics.DrawRectangle(BlackPen, thumbGridClientRect);
				}

			}

		/// <summary>
		/// aktualisiert das Clipping-Rectangle von DaGrid
		/// </summary>
		private void UpdateDaGridClippingRect()
			{
			if (zoomComboBox.SelectedIndex >= 0)
				{
				// daGridClippingRect aktualisieren
				renderOptionsDaGrid.clippingRect.X = (int)Math.Floor(-daGridAutoscrollPosition.X * zoomMultipliers[zoomComboBox.SelectedIndex, 1]);
				renderOptionsDaGrid.clippingRect.Y = (int)Math.Floor(-daGridAutoscrollPosition.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 1]);
				renderOptionsDaGrid.clippingRect.Width = (int)Math.Ceiling(pnlMainGrid.ClientSize.Width * zoomMultipliers[zoomComboBox.SelectedIndex, 1]);
				renderOptionsDaGrid.clippingRect.Height = (int)Math.Ceiling(pnlMainGrid.ClientSize.Height * zoomMultipliers[zoomComboBox.SelectedIndex, 1]);
				}
			}

		/// <summary>
		/// aktualisiert die Thumbnailansicht
		/// </summary>
		private void UpdateThumbGridRect()
			{
			if (zoomComboBox.SelectedIndex >= 0)
				{
				// Zoomfaktor berechnen
				float zoom = (float)thumbGrid.ClientSize.Width / DaGrid.ClientSize.Width;

				thumbGridWorldRect = new Rectangle(
					(int)Math.Round(-daGridAutoscrollPosition.X * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(-daGridAutoscrollPosition.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(pnlMainGrid.ClientSize.Width * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(pnlMainGrid.ClientSize.Height * zoomMultipliers[zoomComboBox.SelectedIndex, 1]));

				thumbGridClientRect = new Rectangle(
					(int)Math.Round(-daGridAutoscrollPosition.X * zoom * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(-daGridAutoscrollPosition.Y * zoom * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(pnlMainGrid.ClientSize.Width * zoom * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(pnlMainGrid.ClientSize.Height * zoom * zoomMultipliers[zoomComboBox.SelectedIndex, 1]));


				thumbGrid.Invalidate();
				}
			}

		#endregion


		#region Eventhandler

		#region Speichern/Laden
		private void SpeichernButton_Click(object sender, EventArgs e)
			{
			using (SaveFileDialog sfd = new SaveFileDialog())
				{
				sfd.InitialDirectory = Application.ExecutablePath;
				sfd.AddExtension = true;
				sfd.DefaultExt = @".xml";
				sfd.Filter = @"XML Dateien|*.xml";

				if (sfd.ShowDialog() == DialogResult.OK)
					{
					XmlSaver.SaveToFile(sfd.FileName, nodeSteuerung, timelineSteuerung, trafficVolumeSteuerung);
					}
				}
			}

		private void LadenButton_Click(object sender, EventArgs e)
			{
			using (OpenFileDialog ofd = new OpenFileDialog())
				{
				ofd.InitialDirectory = Application.ExecutablePath;
				ofd.AddExtension = true;
				ofd.DefaultExt = @".xml";
				ofd.Filter = @"XML Dateien|*.xml";

				if (ofd.ShowDialog() == DialogResult.OK)
					{
					// erstma alles vorhandene löschen
					selectedLineNodes.Clear();
					timelineSteuerung.Clear();

					// Laden
					List<Auftrag> fahrauftraege = XmlSaver.LoadFromFile(ofd.FileName, nodeSteuerung, timelineSteuerung, trafficVolumeSteuerung);

					titleEdit.Text = nodeSteuerung.title;
					infoEdit.Text = nodeSteuerung.infoText;

					// neuzeichnen
					Invalidate(InvalidationLevel.ALL);

					}
				}
			}
		#endregion


		private void timer1_Tick(object sender, EventArgs e)
			{
			thinkStopwatch.Reset();
			thinkStopwatch.Start();

			double tickLength = 1.0d / (double)stepsPerSecondSpinEdit.Value;
			timelineSteuerung.Advance(tickLength);

			GlobalTime.Instance.Advance(tickLength);

			//tickCount++;

			nodeSteuerung.Tick(tickLength);
			trafficVolumeSteuerung.Tick(tickLength);
				
			nodeSteuerung.Reset();

			thinkStopwatch.Stop();

			Invalidate(InvalidationLevel.MAIN_CANVAS_AND_TIMLINE);
			}

		#endregion

		private void timerOnCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			timer1.Enabled = timerOnCheckBox.Checked;
			}

		private void BildLadenButton_Click(object sender, EventArgs e)
			{
			using (OpenFileDialog ofd = new OpenFileDialog())
				{
				ofd.InitialDirectory = Application.ExecutablePath;
				ofd.AddExtension = true;
				ofd.DefaultExt = @".*";
				ofd.Filter = @"Bilder|*.*";

				if (ofd.ShowDialog() == DialogResult.OK)
					{
					backgroundImageEdit.Text = ofd.FileName;
					backgroundImage = new Bitmap(backgroundImageEdit.Text);
					UpdateBackgroundImage();
					Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
					}
				}

			}

		private void UpdateBackgroundImage()
			{
			if (backgroundImage != null)
				{
				resampledBackgroundImage = ResizeBitmap(
					backgroundImage,
					(int)Math.Round(backgroundImage.Width * zoomMultipliers[zoomComboBox.SelectedIndex, 0] * ((float)backgroundImageScalingSpinEdit.Value / 100)),
					(int)Math.Round(backgroundImage.Height * zoomMultipliers[zoomComboBox.SelectedIndex, 0] * ((float)backgroundImageScalingSpinEdit.Value / 100)));
				}
			}

		private Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
			{
			Bitmap result = new Bitmap(nWidth, nHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
			using (Graphics g = Graphics.FromImage((Image)result))
				{
				g.DrawImage(b, 0, 0, nWidth, nHeight);
				}
			return result;
			}

		private void Form1_Load(object sender, EventArgs e)
			{
			timelineSteuerung.AddGroup(unsortedGroup);
			}

		/// <summary>
		/// Erweiterung der Invalidate() Methode, die alles neu zeichnet
		/// </summary>
		public void Invalidate(InvalidationLevel il)
			{
			base.Invalidate();
			switch (il)
				{
				case InvalidationLevel.ALL:
					thumbGrid.Invalidate();
					DaGrid.Invalidate();
					break;
				case InvalidationLevel.MAIN_CANVAS_AND_TIMLINE:
					DaGrid.Invalidate();
					break;
				case InvalidationLevel.ONLY_MAIN_CANVAS:
					DaGrid.Invalidate();
					break;
				default:
					break;
				}
			}

		private void nodeConnectionPrioritySpinEdit_ValueChanged(object sender, EventArgs e)
			{
			if (selectedNodeConnection != null)
				{
				selectedNodeConnection.priority = (int)nodeConnectionPrioritySpinEdit.Value;
				Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
				}
			}

		private void stepButton_Click(object sender, EventArgs e)
			{
			timer1_Tick(sender, e);
			}

		private void killAllVehiclesButton_Click(object sender, EventArgs e)
			{
			foreach (NodeConnection nc in nodeSteuerung.connections)
				{
				foreach (IVehicle v in nc.vehicles)
					{
					nc.RemoveVehicle(v);
					}

				nc.RemoveAllVehiclesInRemoveList();
				}
			foreach (Intersection i in nodeSteuerung.intersections)
				{
				i.UnregisterAllVehicles();
				}
			}

		private void zoomComboBox_SelectedIndexChanged(object sender, EventArgs e)
			{
			// neue Autoscrollposition berechnen und setzen
			pnlMainGrid.ScrollControlIntoView(DaGrid);
			pnlMainGrid.AutoScrollPosition = new Point(
/*				(int)Math.Round((daGridAutoscrollPosition.X - pnlMainGrid.Width/2) * zoomMultipliers[zoomComboBox.SelectedIndex, 0]), 
				(int)Math.Round((daGridAutoscrollPosition.Y - pnlMainGrid.Height/2) * zoomMultipliers[zoomComboBox.SelectedIndex, 0]));*/
				(int)Math.Round((daGridZoomPosition.X * zoomMultipliers[zoomComboBox.SelectedIndex, 0]) - (pnlMainGrid.Width / 2)),
				(int)Math.Round((daGridZoomPosition.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 0]) - (pnlMainGrid.Height / 2)));
			
			// Bitmap umrechnen:
			UpdateBackgroundImage();

			UpdateDaGridClippingRect();
			DaGrid.Invalidate();
			UpdateThumbGridRect();
			}


		private void clearBackgroudnImageButton_Click(object sender, EventArgs e)
			{
			backgroundImage = null;
			resampledBackgroundImage = null;
			backgroundImageEdit.Text = "";
			DaGrid.Invalidate();
			}

		private void textBox1_Leave(object sender, EventArgs e)
			{
			nodeSteuerung.infoText = infoEdit.Text;
			}

		private void aboutBoxButton_Click(object sender, EventArgs e)
			{
			AboutBox a = new AboutBox();
			a.Show();
			}

		private void DaGrid_Leave(object sender, EventArgs e)
			{
			daGridAutoscrollPosition = pnlMainGrid.AutoScrollPosition;
			}

		#region AutoScroll Bug von DaGrid und timeline umgehen

		/// <summary>
		/// Delegate für Funktion die AutoScrollPosition wiederherstellt
		/// </summary>
		/// <param name="sender">Objekt auf das die Funktion angewendet werden soll</param>
		/// <param name="p">AutoScrollPosition, die gesetzt weden soll</param>
		delegate void AutoScrollPositionDelegate(ScrollableControl sender, Point p);

		private void SetAutoScrollPosition(ScrollableControl sender, Point p)
			{
			p.X = Math.Abs(p.X);
			p.Y = Math.Abs(p.Y);
			sender.AutoScrollPosition = p;
			}

		private void DaGrid_Enter(object sender, EventArgs e)
			{
			if (howToDrag != DragNDrop.MOVE_THUMB_RECT)
				{
				Point p = pnlMainGrid.AutoScrollPosition;
				AutoScrollPositionDelegate del = new AutoScrollPositionDelegate(SetAutoScrollPosition);
				Object[] args = { pnlMainGrid, p };
				BeginInvoke(del, args);
				}
			}

		#endregion

		private void thumbGrid_Resize(object sender, EventArgs e)
			{
			UpdateDaGridClippingRect();
			UpdateThumbGridRect();
			}

		private void carsAllowedCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			if (selectedNodeConnection != null)
				{
				selectedNodeConnection.carsAllowed = carsAllowedCheckBox.Checked;
				Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
				}
			}

		private void busAllowedCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			if (selectedNodeConnection != null)
				{
				selectedNodeConnection.busAllowed = busAllowedCheckBox.Checked;
				Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
				}
			}

		private void tramAllowedCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			if (selectedNodeConnection != null)
				{
				selectedNodeConnection.tramAllowed = tramAllowedCheckBox.Checked;
				Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
				}
			}

		private void findLineChangePointsButton_Click(object sender, EventArgs e)
			{
			foreach (NodeConnection nc in nodeSteuerung.connections)
				{
				if (nc.enableOutgoingLineChange)
					{
					nodeSteuerung.RemoveLineChangePoints(nc, true, false);
					nodeSteuerung.FindLineChangePoints(nc, Constants.maxDistanceToLineChangePoint, Constants.maxDistanceToParallelConnection);
					}
				}
			DaGrid.Invalidate();
			}

		private void visualizationCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			nodeSteuerung.setVisualizationInNodeConnections(cbRenderStatistics.Checked);
			DaGrid.Invalidate();
			}

		private void Form1_ResizeEnd(object sender, EventArgs e)
			{
			UpdateDaGridClippingRect();
			DaGrid.Invalidate();
			}

		private void removeLineChangePoints_Click(object sender, EventArgs e)
			{
			if (selectedNodeConnection != null)
				{
				nodeSteuerung.RemoveLineChangePoints(selectedNodeConnection, true, true);
				}
			else
				{
				foreach (NodeConnection nc in nodeSteuerung.connections)
					{
					nodeSteuerung.RemoveLineChangePoints(nc, true, false);
					}
				}
			DaGrid.Invalidate();
			}

		private void titleEdit_Leave(object sender, EventArgs e)
			{
			nodeSteuerung.title = titleEdit.Text;
			}

		private void titleEdit_TextChanged(object sender, EventArgs e)
			{
			if (titleEdit.Text.Equals(""))
				this.Text = "CityTrafficSimulator";
			else
				this.Text = "CityTrafficSimulator - " + titleEdit.Text;
			}

		private void enableIncomingLineChangeCheckBox_Click(object sender, EventArgs e)
			{
			if (m_selectedNodeConnection != null)
				{
				m_selectedNodeConnection.enableIncomingLineChange = enableIncomingLineChangeCheckBox.Checked;
				if (enableIncomingLineChangeCheckBox.Checked)
					{
					// TODO: zu unperformant - andere Lösung muss her
					/*
					foreach (NodeConnection nc in nodeSteuerung.connections)
						{
						if (nc.enableOutgoingLineChange)
							{
							nodeSteuerung.RemoveLineChangePoints(nc, true, false);
							nodeSteuerung.FindLineChangePoints(nc, Constants.maxDistanceToLineChangePoint, Constants.maxDistanceToParallelConnection);
							}
						}
					 * */
					}
				else
					{
					nodeSteuerung.RemoveLineChangePoints(m_selectedNodeConnection, false, true);
					}
				Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
				}

			}

		private void enableOutgoingLineChangeCheckBox_Click(object sender, EventArgs e)
			{
			if (m_selectedNodeConnection != null)
				{
				m_selectedNodeConnection.enableOutgoingLineChange = enableOutgoingLineChangeCheckBox.Checked;
				if (enableOutgoingLineChangeCheckBox.Checked)
					{
					nodeSteuerung.FindLineChangePoints(m_selectedNodeConnection, Constants.maxDistanceToLineChangePoint, Constants.maxDistanceToParallelConnection);
					}
				else
					{
					nodeSteuerung.RemoveLineChangePoints(m_selectedNodeConnection, true, false);
					}
				Invalidate(InvalidationLevel.ONLY_MAIN_CANVAS);
				}
			}

		private void trafficLightTreeView_AfterSelect(object sender, TreeViewEventArgs e)
			{
			// unendliche Rekursion vermeiden
			if (! doHandleTrafficLightTreeViewSelect)
				return;

			// ausgewähltes TrafficLight bestimmen
			TrafficLight tl = e.Node.Tag as TrafficLight;

			// es wurde ein TrafficLight ausgewählt
			if (tl != null)
				{
				// entweder den ausgewählten Nodes die LSA zuordnen
				if (m_selectedLineNodes.Count > 0)
					{
					foreach (LineNode ln in m_selectedLineNodes)
						{
						if (ln.tLight != null)
							{
							ln.tLight.RemoveAssignedLineNode(ln);
							}

						tl.AddAssignedLineNode(ln);
						}

					trafficLightForm.selectedEntry = tl;
					}
				// oder die der LSA zugeordneten Nodes auswählen
				else
					{
					m_selectedLineNodes.Clear();
					m_selectedLineNodes.AddRange(tl.assignedNodes);
					}
				}
			// es wurde kein TrafficLight ausgewählt
			else
				{
				if (m_selectedLineNodes.Count > 0)
					{
					// dann den ausgewählten LineNodes eine evtl. zugeordnete Ampel wegnehmen
					foreach (LineNode ln in m_selectedLineNodes)
						{
						if (ln.tLight != null)
							{
							ln.tLight.RemoveAssignedLineNode(ln);
							}
						}

					trafficLightForm.selectedEntry = null;
					}
				}

			// neu zeichnen lohnt sich immer
			Invalidate(InvalidationLevel.MAIN_CANVAS_AND_TIMLINE);
			}

		private void backgroundImageScalingSpinEdit_ValueChanged(object sender, EventArgs e)
			{
			UpdateBackgroundImage();
			DaGrid.Invalidate();
			}

		private void canvasWidthSpinEdit_ValueChanged(object sender, EventArgs e)
			{
			DaGrid.Width = (int)canvasWidthSpinEdit.Value;
			DaGrid.Invalidate();
			}

		private void canvasHeigthSpinEdit_ValueChanged(object sender, EventArgs e)
			{
			DaGrid.Height = (int)canvasHeigthSpinEdit.Value;
			DaGrid.Invalidate();
			}

		private void simulationSpeedSpinEdit_ValueChanged(object sender, EventArgs e)
			{
			timer1.Interval = (int)(1000 / stepsPerSecondSpinEdit.Value / simulationSpeedSpinEdit.Value);
			}

		private void stepsPerSecondSpinEdit_ValueChanged(object sender, EventArgs e)
			{
			timer1.Interval = (int)(1000 / stepsPerSecondSpinEdit.Value / simulationSpeedSpinEdit.Value);
			}

		private void freeNodeButton_Click(object sender, EventArgs e)
			{
			if (m_selectedLineNodes.Count > 0)
				{
				// dann den ausgewählten LineNodes eine evtl. zugeordnete Ampel wegnehmen
				foreach (LineNode ln in m_selectedLineNodes)
					{
					if (ln.tLight != null)
						{
						ln.tLight.RemoveAssignedLineNode(ln);
						}
					}

				trafficLightForm.selectedEntry = null;
				}

			Invalidate(InvalidationLevel.MAIN_CANVAS_AND_TIMLINE);
			}

		private void cbRenderLineNodes_CheckedChanged(object sender, EventArgs e)
			{
			renderOptionsDaGrid.renderLineNodes = cbRenderLineNodes.Checked;
			DaGrid.Invalidate();
			}

		private void cbRenderConnections_CheckedChanged(object sender, EventArgs e)
			{
			renderOptionsDaGrid.renderNodeConnections = cbRenderConnections.Checked;
			DaGrid.Invalidate();
			}

		private void cbRenderVehicles_CheckedChanged(object sender, EventArgs e)
			{
			renderOptionsDaGrid.renderVehicles = cbRenderVehicles.Checked;
			DaGrid.Invalidate();
			}

		private void cbRenderLineNodesDebug_CheckedChanged(object sender, EventArgs e)
			{
			renderOptionsDaGrid.renderLineNodeDebugData = cbRenderLineNodesDebug.Checked;
			DaGrid.Invalidate();
			}

		private void cbRenderConnectionsDebug_CheckedChanged(object sender, EventArgs e)
			{
			renderOptionsDaGrid.renderNodeConnectionDebugData = cbRenderConnectionsDebug.Checked;
			DaGrid.Invalidate();
			}

		private void cbRenderVehiclesDebug_CheckedChanged(object sender, EventArgs e)
			{
			renderOptionsDaGrid.renderVehicleDebugData = cbRenderVehiclesDebug.Checked;
			DaGrid.Invalidate();
			}

		private void cbRenderIntersections_CheckedChanged(object sender, EventArgs e)
			{
			renderOptionsDaGrid.renderIntersections = cbRenderIntersections.Checked;
			DaGrid.Invalidate();
			}

		private void cbRenderLineChangePoints_CheckedChanged(object sender, EventArgs e)
			{
			renderOptionsDaGrid.renderLineChangePoints = cbRenderLineChangePoints.Checked;
			DaGrid.Invalidate();
			}

		private void pnlNetworkInfo_Resize(object sender, EventArgs e)
			{
			int h = Math.Max(pnlNetworkInfo.ClientSize.Height, 150);
			int w = pnlNetworkInfo.ClientSize.Width;

			titleEdit.Size = new System.Drawing.Size(w - titleEdit.Location.X - 3, titleEdit.Height);
			infoEdit.Size = new System.Drawing.Size(w - 6, h - infoEdit.Location.Y - 61);

			LadenButton.Location = new System.Drawing.Point(w - 85 - 6 - 85 - 3, h - 55);
			SpeichernButton.Location = new System.Drawing.Point(w - 85 - 3, h - 55);
			aboutBoxButton.Location = new System.Drawing.Point(w - 85 - 6 - 85 - 3, h - 26);
			}

		private void spinTargetVelocity_ValueChanged(object sender, EventArgs e)
			{
			if (selectedNodeConnection != null)
				{
				selectedNodeConnection.targetVelocity = (double)spinTargetVelocity.Value;
				}
			}

		private void pnlSignalAssignment_Resize(object sender, EventArgs e)
			{
			int h = Math.Max(pnlSignalAssignment.ClientSize.Height, 100);
			int w = pnlSignalAssignment.ClientSize.Width;

			trafficLightTreeView.Size = new System.Drawing.Size(w - 6, h - 3 - 23 - 6 - 3);
			freeNodeButton.Location = new System.Drawing.Point(w - 85 - 3, h - 26);
			}

		}
    }