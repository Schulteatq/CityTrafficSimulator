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
		#endregion

		#region Variablen / Properties
		private Random rnd = new Random();

		/// <summary>
		/// Flag, ob gerade OnEventChanged() gefeuert wurde
		/// </summary>
		private bool changedEvent = false;

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
		/// Startknoten für Verkehr
		/// </summary>
		public List<LineNode> fromLineNodes = new List<LineNode>();
		/// <summary>
		/// Zielknoten für Verkehr
		/// </summary>
		public List<LineNode> toLineNodes = new List<LineNode>();

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
						timeline.selectedEntry = m_selectedLineNodes[0].tLight;
						}
					else
						{
						trafficLightTreeView.SelectedNode = null;
						timeline.selectedEntry = null;
						}

					selectedNodeConnection = null;
					}
				else
					{
					trafficLightTreeView.SelectedNode = null;
					timeline.selectedEntry = null;
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
			timeline.steuerung = timelineSteuerung;
			trafficLightTreeView.steuerung = timelineSteuerung;

			trafficLightForm = new TrafficLightForm(timelineSteuerung);
			trafficLightForm.Show();

			trafficVolumeForm = new Verkehr.TrafficVolumeForm(trafficVolumeSteuerung, this, nodeSteuerung);
			trafficVolumeForm.Show();

			timelineSteuerung.CurrentTimeChanged += new TimelineSteuerung.CurrentTimeChangedEventHandler(timelineSteuerung_CurrentTimeChanged);

			zoomComboBox.SelectedIndex = 7;

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

		void timelineSteuerung_CurrentTimeChanged(object sender, TimelineSteuerung.CurrentTimeChangedEventArgs e)
			{
			DaGrid.Invalidate();
			}

		#region Drag'n'Drop Gedöns

		#region Timeline
		private void timeline_MouseMove(object sender, MouseEventArgs e)
			{
			if (changedEvent)
				{
				changedEvent = false;
				}
			else
				{
				statusLabel.Text = "Zeitleiste Mausposition: " + timeline.GetTimeAtControlPosition(e.Location, false).ToString() + "s";
				}
			}

		private void timeline_MouseDown(object sender, MouseEventArgs e)
			{

			}

		private void timeline_MouseUp(object sender, MouseEventArgs e)
			{
			howToDrag = DragNDrop.NONE;
			}
		#endregion

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

				splitContainer1.Panel1.AutoScrollPosition = new Point(
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
					(int)Math.Round(splitContainer1.Panel1.ClientSize.Width * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(splitContainer1.Panel1.ClientSize.Height * zoomMultipliers[zoomComboBox.SelectedIndex, 1]));
				*/


				splitContainer1.Panel1.AutoScrollPosition = new Point(
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

			Invalidate();
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
						
						Invalidate();
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

						Invalidate();
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
						Invalidate();
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
						Invalidate();
						break;
					case DragNDrop.DRAG_RUBBERBAND:
						daGridRubberband.Width = (int) Math.Round(clickedPosition.X - daGridRubberband.X);
						daGridRubberband.Height = (int) Math.Round(clickedPosition.Y - daGridRubberband.Y);
						Invalidate();
						break;
					default:
						if (drawDebugCheckBox.Checked)
							{
							//Invalidate();
							}
						break;
					}
				}
			else
				if (drawDebugCheckBox.Checked)
					{
					//Invalidate();
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
			Invalidate();
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
					Invalidate();
					}
				break;
			case Keys.Right:
				foreach (LineNode ln in selectedLineNodes)
					{
					ln.position.X += 1;
					e.Handled = true;
					Invalidate();
					}
				break;
			case Keys.Up:
				foreach (LineNode ln in selectedLineNodes)
					{
					ln.position.Y -= 1;
					e.Handled = true;
					Invalidate();
					}
				break;
			case Keys.Down:
				foreach (LineNode ln in selectedLineNodes)
					{
					ln.position.Y += 1;
					e.Handled = true;
					Invalidate();
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
					Invalidate();

					if (selectedNodeConnection != null)
						{
						nodeSteuerung.Disconnect(selectedNodeConnection.startNode, selectedNodeConnection.endNode);
						selectedNodeConnection = null;
						e.Handled = true;
						Invalidate();
						}
					}
				break;

			// LineSegment teilen
			case Keys.S:
				if (selectedNodeConnection != null)
					{
					nodeSteuerung.SplitNodeConnection(selectedNodeConnection);
					selectedNodeConnection = null;
					Invalidate();
					}
				break;
			case Keys.Return:
				if (selectedNodeConnection != null)
					{
					nodeSteuerung.FindLineChangePoints(selectedNodeConnection, 50, 32);
					Invalidate();
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
				Invalidate();
				break;

			case Keys.N:
				toLineNodes.Clear();
				foreach (LineNode ln in selectedLineNodes)
					{
					toLineNodes.Add(ln);
					}
				Invalidate();
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
				drawDebugCheckBox.Checked = !drawDebugCheckBox.Checked;
				break;
				}
			}
		#endregion
		#endregion

		#region Zeichnen
		void DaGrid_Paint(object sender, PaintEventArgs e)
			{
			// Da splitContainer1.Panel1.OnScroll nicht alles mitbekommt, muss das eben die Paintmethode übernehmen
			if (daGridAutoscrollPosition != splitContainer1.Panel1.AutoScrollPosition)
				{
				daGridZoomPosition = new Point(
					(int)Math.Round((-splitContainer1.Panel1.AutoScrollPosition.X + splitContainer1.Panel1.Width / 2) * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round((-splitContainer1.Panel1.AutoScrollPosition.Y + splitContainer1.Panel1.Height / 2) * zoomMultipliers[zoomComboBox.SelectedIndex, 1]));

				daGridAutoscrollPosition = splitContainer1.Panel1.AutoScrollPosition;

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
				if (drawDebugCheckBox.Checked)
					{
					e.Graphics.DrawString(
						"#Intersections: " + nodeSteuerung.intersections.Count, new Font("Arial", 10),
						new SolidBrush(Color.Black),
						-splitContainer1.Panel1.AutoScrollPosition.X * zoomMultipliers[zoomComboBox.SelectedIndex, 1] + 8,
						-splitContainer1.Panel1.AutoScrollPosition.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 1] + 8);

					if (selectedVehicle != null)
						{
						selectedVehicle.DrawDebugData(e.Graphics);
						}

					// Auto unter Mauszeiger finden
					//IVehicle v = nodeSteuerung.GetVehicleAt(currentMousePosition);

					/*if (v != null)
						{
						v.DrawDebugData(e.Graphics);
						}*/

					}
				renderStopwatch.Stop();
				e.Graphics.DrawString(
					"thinking time: " + thinkStopwatch.ElapsedMilliseconds + "ms, possible thoughts per second: " + ((thinkStopwatch.ElapsedMilliseconds != 0) ? (1000 / thinkStopwatch.ElapsedMilliseconds).ToString() : "-"),
					new Font("Arial", 10),
					new SolidBrush(Color.Black),
					-splitContainer1.Panel1.AutoScrollPosition.X * zoomMultipliers[zoomComboBox.SelectedIndex, 1] + 8,
					-splitContainer1.Panel1.AutoScrollPosition.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 1] + 40);

				e.Graphics.DrawString(
					"rendering time: " + renderStopwatch.ElapsedMilliseconds + "ms, possible fps: " + ((renderStopwatch.ElapsedMilliseconds != 0) ? (1000 / renderStopwatch.ElapsedMilliseconds).ToString() : "-"),
					new Font("Arial", 10),
					new SolidBrush(Color.Black),
					-splitContainer1.Panel1.AutoScrollPosition.X * zoomMultipliers[zoomComboBox.SelectedIndex, 1] + 8,
					-splitContainer1.Panel1.AutoScrollPosition.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 1] + 56);

				}
			}

		private void thumbGrid_Paint(object sender, PaintEventArgs e)
			{
			// TODO: Paint Methode entschlacken und outsourcen?
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.InterpolationMode = InterpolationMode.Bilinear;

			// Zoomfaktor berechnen
			float zoom = (float)thumbGrid.ClientSize.Width / DaGrid.ClientSize.Width;

			e.Graphics.Transform = new Matrix(zoom, 0, 0, zoom, 0, 0);


			using (Pen BlackPen = new Pen(Color.Black, 1.0F))
				{
				nodeSteuerung.RenderNetwork(e.Graphics, renderOptionsThumbnail);

				//to-/fromLineNode malen
				foreach (LineNode ln in toLineNodes)
					{
					RectangleF foo = ln.positionRect;
					foo.Inflate(24, 24);
					e.Graphics.FillEllipse(new SolidBrush(Color.Red), foo);
					}
				foreach (LineNode ln in fromLineNodes)
					{
					RectangleF foo = ln.positionRect;
					foo.Inflate(24, 24);
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
				renderOptionsDaGrid.clippingRect.Width = (int)Math.Ceiling(splitContainer1.Panel1.ClientSize.Width * zoomMultipliers[zoomComboBox.SelectedIndex, 1]);
				renderOptionsDaGrid.clippingRect.Height = (int)Math.Ceiling(splitContainer1.Panel1.ClientSize.Height * zoomMultipliers[zoomComboBox.SelectedIndex, 1]);
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
					(int)Math.Round(splitContainer1.Panel1.ClientSize.Width * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(splitContainer1.Panel1.ClientSize.Height * zoomMultipliers[zoomComboBox.SelectedIndex, 1]));

				thumbGridClientRect = new Rectangle(
					(int)Math.Round(-daGridAutoscrollPosition.X * zoom * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(-daGridAutoscrollPosition.Y * zoom * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(splitContainer1.Panel1.ClientSize.Width * zoom * zoomMultipliers[zoomComboBox.SelectedIndex, 1]),
					(int)Math.Round(splitContainer1.Panel1.ClientSize.Height * zoom * zoomMultipliers[zoomComboBox.SelectedIndex, 1]));

				if (tabControl1.SelectedTab == thumbTabPage)
					{
					thumbGrid.Invalidate();
					}
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
					List<Auftrag> fahraufträge = new List<Auftrag>();
					foreach (Auftrag a in AufträgeCheckBox.Items)
						{
						fahraufträge.Add(a);
						}

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
					AufträgeCheckBox.Items.Clear();
					timelineSteuerung.Clear();

					// Laden
					List<Auftrag> fahrauftraege = XmlSaver.LoadFromFile(ofd.FileName, nodeSteuerung, timelineSteuerung, trafficVolumeSteuerung);

					/*// Timeline mit den Ampeln füllen
					// TODO: ist das nicht eigentlich Aufgabe der NodeSteuerung oder so?
					foreach (LineNode ln in nodeSteuerung.nodes)
						{
						if (ln.tLight != null)
							{
							ln.tLight.parentGroup = unsortedGroup;
							timelineSteuerung.AddEntry(ln.tLight);
							}
						}
					*/
					// Fahraufträge in Liste eintragen 
					// TODO: sollte auch eleganter gelöst werden...
					foreach (Auftrag a in fahrauftraege)
						{
						AufträgeCheckBox.Items.Add(a);
						}

					titleEdit.Text = nodeSteuerung.title;
					infoEdit.Text = nodeSteuerung.infoText;

					// neuzeichnen
					Invalidate();

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

			Invalidate();
			}

		private void dockToGridcheckBox_CheckedChanged(object sender, EventArgs e)
			{
			dockToGrid = dockToGridcheckBox.Checked;
			}

		private void addCarButton_Click(object sender, EventArgs e)
			{
			// TODO / BUG:
			//		Fahrzeuge auf Routen von Verkehrsaufträgen bauen beim Berechnen der Route
			//		Müll (nicht genügend Connections oder Nullpointer auf dem route-Stack) wenn
			//		sie über den "neue(s) ..." Button erstellt werden
			if (fromLineNodes.Count > 0 && toLineNodes.Count > 0)
				{
				IVehicle.Physics p = new IVehicle.Physics((double)v0Edit.Value, 0, 0);
				Car t = new Car(p);

				nodeSteuerung.AddVehicle(t, fromLineNodes[rnd.Next(fromLineNodes.Count)], toLineNodes);
				Invalidate();
				}
			}

		private void carRemoveButton_Click(object sender, EventArgs e)
			{
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
					Invalidate();
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

		private void NeuerAuftragButton_Click(object sender, EventArgs e)
			{
			if (fromLineNodes.Count > 0 && toLineNodes.Count > 0)
				{
				IVehicle.VehicleTypes type;
				switch (vehicleTypeComboBox.SelectedIndex)
					{
					case 1:
						type = IVehicle.VehicleTypes.BUS;
						break;
					case 2:
						type = IVehicle.VehicleTypes.TRAM;
						break;
					default:
						type = IVehicle.VehicleTypes.CAR;
						break;
					}

				Auftrag auftragToAdd = new Auftrag(
					type,
					fromLineNodes, 
					toLineNodes, 
					(int)HäufigkeitSpinEdit.Value);
				AufträgeCheckBox.Items.Add(auftragToAdd);
				}
			}

		private void SetStartNodeButton_Click(object sender, EventArgs e)
			{
			fromLineNodes.Clear();
			foreach (LineNode ln in selectedLineNodes)
				{
				fromLineNodes.Add(ln);
				}
			Invalidate();
			}

		private void SetEndNodeButton_Click(object sender, EventArgs e)
			{
			toLineNodes.Clear();
			foreach (LineNode ln in selectedLineNodes)
				{
				toLineNodes.Add(ln);
				}
			Invalidate();
			}

		private void HäufigkeitSpinEdit_ValueChanged(object sender, EventArgs e)
			{
			if (AufträgeCheckBox.SelectedItem != null)
				{
				Auftrag auftragToModify = AufträgeCheckBox.SelectedItem as Auftrag;
				if (auftragToModify != null)
					{
					if (true)
						{
						auftragToModify.trafficDensity = (int)HäufigkeitSpinEdit.Value;
						AufträgeCheckBox.Invalidate();
						}
					}
				}

			}

		private void AuftragLöschenButton_Click(object sender, EventArgs e)
			{
			if (AufträgeCheckBox.SelectedItem != null)
				{
				AufträgeCheckBox.Items.RemoveAt(AufträgeCheckBox.SelectedIndex);
				}
			}

		private void Form1_Load(object sender, EventArgs e)
			{
			vehicleTypeComboBox.SelectedIndex = 0;
			timelineSteuerung.AddGroup(unsortedGroup);
			}

		/// <summary>
		/// Override der Invalidate() Methode, die alles neu zeichnet
		/// </summary>
		new public void Invalidate()
			{
			base.Invalidate();
			DaGrid.Invalidate();
			timeline.Invalidate();
			thumbGrid.Invalidate();
			}

		private void timeline_MouseClick(object sender, MouseEventArgs e)
			{

			}

		private void nodeConnectionPrioritySpinEdit_ValueChanged(object sender, EventArgs e)
			{
			if (selectedNodeConnection != null)
				{
				selectedNodeConnection.priority = (int)nodeConnectionPrioritySpinEdit.Value;
				Invalidate();
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
			}

		private void zoomComboBox_SelectedIndexChanged(object sender, EventArgs e)
			{
			// neue Autoscrollposition berechnen und setzen
			splitContainer1.Panel1.ScrollControlIntoView(DaGrid);
			splitContainer1.Panel1.AutoScrollPosition = new Point(
/*				(int)Math.Round((daGridAutoscrollPosition.X - splitContainer1.Panel1.Width/2) * zoomMultipliers[zoomComboBox.SelectedIndex, 0]), 
				(int)Math.Round((daGridAutoscrollPosition.Y - splitContainer1.Panel1.Height/2) * zoomMultipliers[zoomComboBox.SelectedIndex, 0]));*/
				(int)Math.Round((daGridZoomPosition.X * zoomMultipliers[zoomComboBox.SelectedIndex, 0]) - (splitContainer1.Panel1.Width / 2)),
				(int)Math.Round((daGridZoomPosition.Y * zoomMultipliers[zoomComboBox.SelectedIndex, 0]) - (splitContainer1.Panel1.Height / 2)));
			
			// Bitmap umrechnen:
			UpdateBackgroundImage();

			UpdateDaGridClippingRect();
			DaGrid.Invalidate();
			UpdateThumbGridRect();
			}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
			{

			}

		private void AufträgeCheckBox_SelectedIndexChanged(object sender, EventArgs e)
			{
			if (AufträgeCheckBox.SelectedItem != null)
				{
				Auftrag auftragToModify = AufträgeCheckBox.SelectedItem as Auftrag;
				if (auftragToModify != null)
					{
					fromLineNodes.Clear();
					toLineNodes.Clear();

					foreach (LineNode ln in auftragToModify.startNodes)
						fromLineNodes.Add(ln);
					foreach (LineNode ln in auftragToModify.endNodes)
						toLineNodes.Add(ln);

					HäufigkeitSpinEdit.Value = auftragToModify.trafficDensity;
					vehicleTypeComboBox.SelectedIndex = 
						(auftragToModify.vehicleType == IVehicle.VehicleTypes.CAR ? 0 
						: (auftragToModify.vehicleType == IVehicle.VehicleTypes.BUS ? 1 : 2));

					DaGrid.Invalidate();
					}
				}
			}

		private void drawDebugCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			DaGrid.Invalidate();
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
			daGridAutoscrollPosition = splitContainer1.Panel1.AutoScrollPosition;
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
				Point p = splitContainer1.Panel1.AutoScrollPosition;
				AutoScrollPositionDelegate del = new AutoScrollPositionDelegate(SetAutoScrollPosition);
				Object[] args = { splitContainer1.Panel1, p };
				BeginInvoke(del, args);
				}
			}


		private void timeline_Enter(object sender, EventArgs e)
			{
			Point p = timelinePanel.AutoScrollPosition;
			AutoScrollPositionDelegate del = new AutoScrollPositionDelegate(SetAutoScrollPosition);
			Object[] args = { timelinePanel, p };
			BeginInvoke(del, args);
			}

		#endregion

		private void timeline_MouseLeave(object sender, EventArgs e)
			{
			this.Cursor = Cursors.Default;
			statusLabel.Text = "";
			}

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
				Invalidate();
				}
			}

		private void busAllowedCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			if (selectedNodeConnection != null)
				{
				selectedNodeConnection.busAllowed = busAllowedCheckBox.Checked;
				Invalidate();
				}
			}

		private void tramAllowedCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			if (selectedNodeConnection != null)
				{
				selectedNodeConnection.tramAllowed = tramAllowedCheckBox.Checked;
				Invalidate();
				}
			}

		private void addTramButton_Click(object sender, EventArgs e)
			{
			// TODO / BUG:
			//		Fahrzeuge auf Routen von Verkehrsaufträgen bauen beim Berechnen der Route
			//		Müll (nicht genügend Connections oder Nullpointer auf dem route-Stack) wenn
			//		sie über den "neue(s) ..." Button erstellt werden
			if (fromLineNodes.Count > 0 && toLineNodes.Count > 0)
				{
				IVehicle.Physics p = new IVehicle.Physics((double)v0Edit.Value, 0, 0);
				Tram t = new Tram(p);

				nodeSteuerung.AddVehicle(t, fromLineNodes[rnd.Next(fromLineNodes.Count)], toLineNodes);
				Invalidate();
				}
			}

		private void vehicleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
			{
			if (AufträgeCheckBox.SelectedItem != null)
				{
				Auftrag a = AufträgeCheckBox.SelectedItem as Auftrag;
				if (a != null)
					{
					a.vehicleType = (vehicleTypeComboBox.SelectedIndex == 0 
						? IVehicle.VehicleTypes.CAR 
						: (vehicleTypeComboBox.SelectedIndex == 1 ? IVehicle.VehicleTypes.BUS : IVehicle.VehicleTypes.TRAM));
					Invalidate();
					}
				}
			}

		private void addBusButton_Click(object sender, EventArgs e)
			{
			// TODO / BUG:
			//		Fahrzeuge auf Routen von Verkehrsaufträgen bauen beim Berechnen der Route
			//		Müll (nicht genügend Connections oder Nullpointer auf dem route-Stack) wenn
			//		sie über den "neue(s) ..." Button erstellt werden
			if (fromLineNodes.Count > 0 && toLineNodes.Count > 0)
				{
				IVehicle.Physics p = new IVehicle.Physics((double)v0Edit.Value, 0, 0);
				Bus t = new Bus(p);

				nodeSteuerung.AddVehicle(t, fromLineNodes[rnd.Next(fromLineNodes.Count)], toLineNodes);
				Invalidate();
				}
			}

		private void button3_Click(object sender, EventArgs e)
			{
			UpdateFromToNodeInSelectedAuftrag();
			}

		private void UpdateFromToNodeInSelectedAuftrag() 
			{
			if (AufträgeCheckBox.SelectedItem != null)
				{
				Auftrag auftragToModify = AufträgeCheckBox.SelectedItem as Auftrag;
				if (auftragToModify != null)
					{
					auftragToModify.startNodes.Clear();
					auftragToModify.endNodes.Clear();

					foreach (LineNode ln in fromLineNodes)
						{
						auftragToModify.startNodes.Add(ln);
						}
					foreach (LineNode ln in toLineNodes)
						{
						auftragToModify.endNodes.Add(ln);
						}

					AufträgeCheckBox.Invalidate();
					}
				}
			}

		private void drawNodeConnectionsCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			Invalidate();
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

		private void truckRatioSpinEdit_ValueChanged(object sender, EventArgs e)
			{
			Auftrag.truckRatio = (int)truckRatioSpinEdit.Value;
			}

		private void visualizationCheckBox_CheckedChanged(object sender, EventArgs e)
			{
			nodeSteuerung.setVisualizationInNodeConnections(visualizationCheckBox.Checked);
			DaGrid.Invalidate();
			}

		private void Form1_ResizeEnd(object sender, EventArgs e)
			{
			UpdateDaGridClippingRect();
			DaGrid.Invalidate();
			}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
			{
			Auftrag.trafficDensityMultiplier = trafficDensityMultiplierSpinEdit.Value;
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
				Invalidate();
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
				Invalidate();
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

					timeline.selectedEntry = tl;
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

					timeline.selectedEntry = null;
					}
				}

			// neu zeichnen lohnt sich immer
			Invalidate();
			}

		private void timeline_TimelineMoved(object sender, EventArgs e)
			{
			}

		private void timeline_SelectionChanged(object sender, TimelineControl.SelectionChangedEventArgs e)
			{
			if (e.selectedEntry != null)
				{
				TrafficLight tl = e.selectedEntry as TrafficLight;
				m_selectedLineNodes.Clear();
				m_selectedLineNodes.AddRange(tl.assignedNodes);

				DaGrid.Invalidate();
				}
			}

		private void AufträgeCheckBox_KeyDown(object sender, KeyEventArgs e)
			{
			if (e.KeyCode == Keys.Delete && AufträgeCheckBox.SelectedItem != null)
				{
				AuftragLöschenButton_Click(this, new EventArgs());
				}
			}

		private void timeline_EventChanged(object sender, TimelineControl.EventChangedEventArgs e)
			{
			switch (e.dragAction)
				{
				case TimelineControl.DragNDrop.MOVE_EVENT:
					statusLabel.Text = "verschiebe Event, Start: " + e.handeledEvent.eventTime + "s, Ende: " + (e.handeledEvent.eventTime + e.handeledEvent.eventLength) + "s";
					changedEvent = true;
					break;
				case TimelineControl.DragNDrop.MOVE_EVENT_START:
					statusLabel.Text = "verschiebe Event-Start: " + e.handeledEvent.eventTime + "s";
					changedEvent = true;
					break;
				case TimelineControl.DragNDrop.MOVE_EVENT_END:
					statusLabel.Text = "verschiebe Event-Ende: " + (e.handeledEvent.eventTime + e.handeledEvent.eventLength) + "s";
					changedEvent = true;
					break;
				default:
					break;
				}

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

				timeline.selectedEntry = null;
				}

			Invalidate();
			}

		private void showEditorButton_Click(object sender, EventArgs e)
			{
			if (trafficLightForm.IsDisposed)
				{
				trafficLightForm = new TrafficLightForm(timelineSteuerung);
				}
			trafficLightForm.Show();
			trafficLightForm.BringToFront();
			}


		}
    }