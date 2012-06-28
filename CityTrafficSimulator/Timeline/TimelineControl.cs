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
using System.Text;
using System.Windows.Forms;

using CityTrafficSimulator.Timeline;
using CityTrafficSimulator.Tools.ObserverPattern;

namespace CityTrafficSimulator
	{
	/// <summary>
	/// Control, welches eine einfache Zeitleiste darstellt
	/// </summary>
	public partial class TimelineControl : Control, IEventListener
		{
		#region Hilfsklassen
		/// <summary>
		/// Art des Drag and Drop-Vorganges
		/// </summary>
		public enum DragNDrop
			{
			/// <summary>
			/// es wird nichts gedragged
			/// </summary>
			NONE, 
			/// <summary>
			/// die vertikale Zeitleiste wird verschoben (die, die die aktuelle Zeit anzeigt)
			/// </summary>
			MOVE_TIMELINE_BAR, 
			/// <summary>
			/// ein ganzes TimelineEvent wird verschoben
			/// </summary>
			MOVE_EVENT, 
			/// <summary>
			/// der Anfang eines TimelineEvents wird verschoben
			/// </summary>
			MOVE_EVENT_START, 
			/// <summary>
			/// das Ende eines TimelineEvents wird verschoben
			/// </summary>
			MOVE_EVENT_END, 
			}
		#endregion
	
		
		#region Variablen


		#region Hilfsvariablen für Eventhandler
		
		/// <summary>
		/// Drag 'n Drop Modus
		/// </summary>
		private DragNDrop howToDrag = DragNDrop.NONE;

		/// <summary>
		/// bei Drag 'n Drop zu bewegendes TimelineEvent
		/// </summary>
		private TimelineEvent eventToDrag = null;
		/// <summary>
		/// Offset des zu bewegenden TimelineEvents
		/// </summary>
		private int eventToDragOffset = 0;

		/// <summary>
		/// Clientposition, wo die Maus gedrückt wurde
		/// </summary>
		private Point mouseDownPosition;


		private double timelineBarTime = 0;

		#endregion



		/// <summary>
		/// zugeordnete TimelineSteuerung
		/// </summary>
		private TimelineSteuerung m_steuerung;
		/// <summary>
		/// zugeordnete TimelineSteuerung, beim Setzen wird sich automatisch an/abgemeldet.
		/// </summary>
		public TimelineSteuerung steuerung
			{
			get { return m_steuerung; }
			set
				{
				// Ereignisse ummelden
				if (m_steuerung != null)
					{
					m_steuerung.CurrentTimeChanged -= m_steuerung_CurrentTimeChanged;
					m_steuerung.EntryChanged -=m_steuerung_EntryChanged;
					m_steuerung.GroupsChanged -= m_steuerung_GroupsChanged;
					m_steuerung.MaxTimeChanged -= m_steuerung_MaxTimeChanged;
					}
				m_steuerung = value;
				if (m_steuerung != null)
					{
					m_steuerung.CurrentTimeChanged += new TimelineSteuerung.CurrentTimeChangedEventHandler(m_steuerung_CurrentTimeChanged);
					m_steuerung.EntryChanged += new TimelineSteuerung.EntryChangedEventHandler(m_steuerung_EntryChanged);
					m_steuerung.GroupsChanged += new TimelineSteuerung.GroupsChangedEventHandler(m_steuerung_GroupsChanged);
					m_steuerung.MaxTimeChanged += new TimelineSteuerung.MaxTimeChangedEventHandler(m_steuerung_MaxTimeChanged);
					}
				}
			}

		void m_steuerung_MaxTimeChanged(object sender, EventArgs e)
			{
			UpdateControlSize();
			Invalidate();
			}

		void m_steuerung_GroupsChanged(object sender, EventArgs e)
			{
			UpdateControlSize();
			Invalidate();
			}

		void m_steuerung_EntryChanged(object sender, TimelineSteuerung.EntryChangedEventArgs e)
			{
			Invalidate();
			}

		void m_steuerung_CurrentTimeChanged(object sender, TimelineSteuerung.CurrentTimeChangedEventArgs e)
			{
			timelineBarTime = e.currentTime;
			Invalidate();
			}

		/// <summary>
		/// aktuell ausgewähler TimelineEntry
		/// </summary>
		private TimelineEntry m_selectedEntry;
		/// <summary>
		/// aktuell ausgewähler TimelineEntry
		/// </summary>
		public TimelineEntry selectedEntry
			{
			get { return m_selectedEntry; }
			set { m_selectedEntry = value; Invalidate(); OnSelectionChanged(); }
			}


		/// <summary>
		/// aktuell ausgewählte TimelineGroup
		/// </summary>
		private TimelineGroup m_selectedGroup;
		/// <summary>
		/// aktuell ausgewählte TimelineGroup
		/// </summary>
		public TimelineGroup selectedGroup
			{
			get { return m_selectedGroup; }
			set { m_selectedGroup = value; Invalidate(); OnSelectionChanged(); }
			}


		/// <summary>
		/// Zoom des Timelinecontrol (= wieviele Pixel sind 1 Zeiteinheit)
		/// </summary>
		private int m_zoom;
		/// <summary>
		/// Zoom des Timelinecontrol (= wieviele Pixel sind 1 Zeiteinheit)
		/// </summary>
		public int zoom
			{
			get { return m_zoom; }
			set { m_zoom = value; UpdateControlSize(); Invalidate(); }
			}


		/// <summary>
		/// Anzahl der Sekunden, auf die die Zeiten der Events gerundet werden
		/// </summary>
		private double m_snapSize = 0.5;
		/// <summary>
		/// Anzahl der Sekunden, auf die die Zeiten der Events gerundet werden
		/// </summary>
		public double snapSize
			{
			get { return m_snapSize; }
			set { m_snapSize = value; }
			}



		/// <summary>
		/// Anzahl der aktuell sichtbaren Zeilen
		/// </summary>
		private int currentVisibleRowCount;

		#endregion

		#region Prozeduren

		/// <summary>
		/// Standardkonstruktor, aktiviert DoubleBuffering und setzt Standardwerte
		/// </summary>
		public TimelineControl()
			{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			this.DoubleBuffered = true;

			if (DesignMode)
				{
				this.zoom = 5;

				}
			/*
			TimelineGroup tg1 = new TimelineGroup("Gruppe 1", false);
			TimelineGroup tg2 = new TimelineGroup("Gruppe 2", true);

			addGroup(tg1);
			addGroup(tg2);

			steuerung.maxTime = 50;
			
			TimelineEntry te1 = new TrafficLight(new LineNode(new Vector2(0, 0)));
			te1.parentGroup = tg1;
			te1.addEvent(new TimelineEvent(10, 20, Color.Green, delegate() { }, delegate() { }));
			addEntry(te1);

			TimelineEntry te2 = new TrafficLight(new LineNode(new Vector2(0, 0)));
			te2.parentGroup = tg1;
			te2.addEvent(new TimelineEvent(5, 10, Color.Green, delegate() { }, delegate() { }));
			te2.addEvent(new TimelineEvent(30, 10, Color.Green, delegate() { }, delegate() { }));
			addEntry(te2);

			TimelineEntry te3 = new TrafficLight(new LineNode(new Vector2(0, 0)));
			te3.parentGroup = tg2;
			te3.addEvent(new TimelineEvent(15, 25, Color.Green, delegate() { }, delegate() { }));
			addEntry(te3);
			*/
			}


		/// <summary>
		/// gibt die Zeit zur Position position auf dem Control zurück
		/// </summary>
		/// <param name="position">Position auf dem Control</param>
		/// <param name="doSnap">runde auf snapSize Sekunden</param>
		/// <returns>position.X/zoom</returns>
		public double GetTimeAtControlPosition(Point position, bool doSnap)
			{
			double abs = (double)(position.X) - totalRowHeight;
			if (doSnap)
				{
				abs /= (double)m_zoom;
				abs /= m_snapSize;
				abs = Math.Round(abs);
				return abs * m_snapSize;
				}
			else
				return abs / (double)zoom;
			}

		/// <summary>
		/// gibt den TimelineEntry zurück, der sich an der Position position befindet
		/// </summary>
		/// <param name="position">Position auf dem Control</param>
		/// <returns>das an position befindliche TimelineEntry oder null, falls kein solches exisitiert</returns>
		public TimelineEntry GetTimelineEntryAtControlPosition(Point position)
			{
			if ((position.Y >= 0)														// testen, ob position überhaupt zulässig	
					&& (position.Y < currentVisibleRowCount * totalRowHeight)			// testen, ob nicht zu weit unten geklickt wurde
					&& (position.Y % totalRowHeight <= rowHeight) && (position.Y > 0))	// testen, ob in die Zwischenräume der Entries geklickt wurde
				{

				int yChecked = 0;

				// alle Gruppen durchgehen
				foreach (TimelineGroup tg in steuerung.groups)
					{
					yChecked += totalRowHeight;

					if (! tg.collapsed)
						{
						foreach (TimelineEntry te in tg.entries)
							{
							if (position.Y > yChecked && position.Y <= yChecked + rowHeight)
								{
								return te;
								}
							yChecked += totalRowHeight;
							}
						}

					if (position.Y < yChecked)
						{
						return null;
						}
					}
				return null;
				}
			else
				{
				return null;
				}
			}

		/// <summary>
		/// Liefert das TimelineEvent zurück dass sich an der Position position befindet
		/// </summary>
		/// <param name="position">Position auf dem Control</param>
		/// <param name="onlySelectedEntry">nur auf dem selektierten TimelineEntry suchen, oder auch auf den anderen</param>
		/// <returns>das TimelineEvent auf welches geklickt wurde, oder null falls es kein solches gibt</returns>
		public TimelineEvent GetTimelineEventAtControlPosition(Point position, Boolean onlySelectedEntry)
			{
			TimelineEntry entry = GetTimelineEntryAtControlPosition(position);
			if (entry != null && (! onlySelectedEntry || entry == selectedEntry))
				{
				double time = GetTimeAtControlPosition(position, false);
				return entry.GetEventAtTime(time);
				}
			return null;
			}

		/// <summary>
		/// gibt die Koordinaten der linken oberen Ecke des TimelineEntries te zurück
		/// </summary>
		/// <param name="te">TimelineEntry, wessen Position berechnet werden soll</param>
		/// <returns>Koordinaten der linken oberen Ecke des Balkens, der te repräsentiert oder null, falls te nicht in m_entries</returns>
		public Point GetClientPositionForTimelineEntry(TimelineEntry te)
			{
			int y = 0;

			foreach (TimelineGroup tg in steuerung.groups)
				{
				y += totalRowHeight;
				if (!tg.collapsed)
					{
					foreach (TimelineEntry foo in tg.entries)
						{
						if (foo == te)
							{
							return new Point(totalRowHeight, y);
							}
						y += totalRowHeight;
						}
					}
				}

			return new Point(0,0);
			}


		/// <summary>
		/// gibt die Koordinaten der linken oberen Ecke des TimelineEvents te zurück
		/// </summary>
		/// <param name="te">TimelineEvent, dessen Position berechnet werden soll</param>
		/// <returns>Koordinaten der linken oberen Ecke des Balkens, der te repräsentiert oder, (0,0), falls te nicht in m_entries.events</returns>
		public Point GetClientPositionForTimelineEvent(TimelineEvent te)
			{
			int y = 0;

			foreach (TimelineGroup tg in steuerung.groups)
				{
				y += totalRowHeight;

				if (! tg.collapsed)
					{
					foreach (TimelineEntry foo in tg.entries)
						{
						if (foo.events.Contains(te))
							{
							return new Point(totalRowHeight + (int)Math.Round(m_zoom * te.eventTime), y);
							}
						y += totalRowHeight;
						}
					}
				}

			return new Point(0, 0);
			}

		/// <summary>
		/// Gibt die horizontale Clientposition zur aktuellen Zeit zurück
		/// </summary>
		/// <returns>CurrentTime*zoom</returns>
		public int GetHorizontalClientPositionAtCurrentTime()
			{
			return totalRowHeight + (int)Math.Round(steuerung.CurrentTime * zoom);
			}


		/// <summary>
		/// sucht die TimelineGroup zur übergebenen Clientposition heraus
		/// </summary>
		/// <param name="clientPosition">Position, wo nach einer TimelineGroup gesucht werden soll</param>
		/// <param name="headerOnly">prüfe, ob clientPosition sich im Header der Gruppe befindet oder auch dort, wo die zugehörigen Entries angezeigt werden</param>
		/// <returns>eine TimelineGroup, die an clientPosition angezeigt wird oder null, falls keine solche existiert</returns>
		public TimelineGroup GetGroupAtClientPosition(Point clientPosition, bool headerOnly)
			{
			int y = 0;

			foreach (TimelineGroup tg in steuerung.groups)
				{
				if ((clientPosition.Y >= y)
					&& (headerOnly && clientPosition.Y <= y + rowHeight) || (!headerOnly && clientPosition.Y <= y + ((1 + tg.entries.Count) * totalRowHeight) - rowSpacing))
					{
					return tg;
					}
				if (y > clientPosition.Y)
					{
					return null;
					}

				y += (tg.collapsed ? totalRowHeight : (tg.entries.Count + 1) * totalRowHeight);
				}
			return null;
			}


		/// <summary>
		/// berechnet und setzt die benötigte Größe des Steuerelementes neu
		/// </summary>
		private void UpdateControlSize()
			{
			currentVisibleRowCount = 0;
			if (steuerung != null)
				{
				foreach (TimelineGroup tg in steuerung.groups)
					{
					currentVisibleRowCount++;
					if (!tg.collapsed)
						currentVisibleRowCount += tg.entries.Count;
					}
				this.ClientSize = new Size((int)Math.Round(m_zoom * steuerung.maxTime) + 5 + totalRowHeight, Math.Max(currentVisibleRowCount * totalRowHeight + 20, 50));
				}
			}

		/// <summary>
		/// Methode, die vom EventManager.NotifyListeners() aufgerufen werden soll.
		/// </summary>
		public void Notify()
			{
			timelineBarTime = m_steuerung.CurrentTime;
			UpdateControlSize();
			Invalidate();
			}

		#endregion

		#region Paint Variablen und Eventhandler

		/// <summary>
		/// weißer Pen
		/// </summary>
		private static Pen whitePen = new Pen(Color.White);

		/// <summary>
		/// weißer Brush
		/// </summary>
		private static SolidBrush whiteBrush = new SolidBrush(Color.White);

		/// <summary>
		/// schwarzer Brush
		/// </summary>
		private static SolidBrush blackBrush = new SolidBrush(Color.Black);

		/// <summary>
		/// schwarzer Pen
		/// </summary>
		private static Pen blackPen = new Pen(Color.Black, 1.0f);

		/// <summary>
		/// gestrichelter Pen
		/// </summary>
		private static Pen dottedPen = new Pen(Color.Black);

		/// <summary>
		/// Calibri Font
		/// </summary>
		private static Font calibriFont = new Font("Calibri", 8.0f);

		/// <summary>
		/// Brush für Hintergründe von selektierten Einheiten
		/// </summary>
		private static Brush backgroundBrush = new SolidBrush(Color.LightGray);

		/// <summary>
		/// Zeilenhöhe
		/// </summary>
		private const int rowHeight = 10;

		/// <summary>
		/// Leerraum zwischen Zeilen
		/// </summary>
		private const int rowSpacing = 2;

		/// <summary>
		/// komplette Höhe einer Zeile inklusive Zwischenleerraum
		/// </summary>
		private const int totalRowHeight = rowHeight + rowSpacing;

		/// <summary>
		/// zeichnet das TimelineControl
		/// </summary>
		/// <param name="sender">Aufrufer</param>
		/// <param name="e">Argumente</param>
		private void TimelineControl_Paint(object sender, PaintEventArgs e)
			{
			// zunächst die ganze Geschichte weiß streichen
			e.Graphics.Clear(Color.White);

			if (steuerung != null)
				{
				int maxWidth = (int)Math.Round(m_zoom * steuerung.maxTime);

				int row = 0;
				foreach (TimelineGroup tg in steuerung.groups)
					{
					// zusammengefaltete Gruppe zeichnen
					if (tg.collapsed)
						{
						if (tg == m_selectedGroup)
							{
							e.Graphics.FillRectangle(backgroundBrush, 0, 1 + (row * totalRowHeight), ClientSize.Width, totalRowHeight);
							}

						// Gruppenname zeichnen
						e.Graphics.DrawString(tg.title, calibriFont, blackBrush, new PointF(totalRowHeight + 3, row * totalRowHeight));

						// Pluszeichen malen
						e.Graphics.DrawRectangle(blackPen, 0, 1 + (row * totalRowHeight), rowHeight, rowHeight);
						e.Graphics.DrawLine(blackPen, 2, 1 + (row * totalRowHeight) + rowHeight / 2, rowHeight - 2, 1 + (row * totalRowHeight) + rowHeight / 2);
						e.Graphics.DrawLine(blackPen, rowHeight / 2, 1 + (row * totalRowHeight) + 2, rowHeight / 2, 1 + (row * totalRowHeight) + rowHeight - 2);
						}
					// aufgefaltete Gruppe zeichnen
					else
						{
						if (tg == m_selectedGroup)
							{
							e.Graphics.FillRectangle(backgroundBrush, 0, 1 + (row * totalRowHeight), ClientSize.Width, (1 + tg.entries.Count) * totalRowHeight);
							}

						// Gruppenname zeichnen
						e.Graphics.DrawString(tg.title, calibriFont, blackBrush, new PointF(totalRowHeight + 3, row * totalRowHeight));

						// Minuszeichen malen
						e.Graphics.DrawRectangle(blackPen, 0, 1 + (row * totalRowHeight), rowHeight, rowHeight);
						e.Graphics.DrawLine(blackPen, 2, 1 + (row * totalRowHeight) + rowHeight / 2, rowHeight - 2, 1 + (row * totalRowHeight) + rowHeight / 2);

						// vertikale Linie unter Minuszeichen malen
						if (tg.entries.Count > 0)
							e.Graphics.DrawLine(blackPen, rowHeight / 2, row * totalRowHeight + totalRowHeight, rowHeight / 2, (row + tg.entries.Count) * totalRowHeight + totalRowHeight / 2);


						// Entries zeichnen
						foreach (TimelineEntry te in tg.entries)
							{
							row++;

							// horizontale Linie links malen
							e.Graphics.DrawLine(blackPen, totalRowHeight / 2, row * totalRowHeight + totalRowHeight / 2, totalRowHeight / 2 + totalRowHeight / 4, row * totalRowHeight + totalRowHeight / 2);

							// Balken malen
							if (te == selectedEntry)
								{
								e.Graphics.DrawRectangle(dottedPen, totalRowHeight, 1 + (row * totalRowHeight), maxWidth, rowHeight);
								}
							else
								{
								e.Graphics.DrawRectangle(blackPen, totalRowHeight, 1 + (row * totalRowHeight), maxWidth, rowHeight);
								}
							e.Graphics.FillRectangle(new SolidBrush(te.color), totalRowHeight + 1, 2 + (row * totalRowHeight), maxWidth - 1, rowHeight - 1);

							// einzelne Events zeichnen
							foreach (TimelineEvent ev in te.events)
								{
								int startPosition = totalRowHeight + (int)Math.Round(ev.eventTime * zoom);
								int length = (int)Math.Round(ev.eventLength * zoom);
								e.Graphics.FillRectangle(new SolidBrush(ev.color), startPosition, 2 + (row * totalRowHeight), length, rowHeight - 1);
								e.Graphics.DrawLine(blackPen, startPosition, 2 + (row * totalRowHeight), startPosition, totalRowHeight - 2 + (row * totalRowHeight));
								e.Graphics.DrawLine(blackPen, startPosition + length, 2 + (row * totalRowHeight), startPosition + length, totalRowHeight - 2 + (row * totalRowHeight));
								}

							e.Graphics.DrawString(te.name, calibriFont, blackBrush, new PointF(totalRowHeight + 3, row * totalRowHeight));
							}
						}

					row++;
					}



				// Lineal zeichnen
				dottedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

				e.Graphics.DrawLine(blackPen, totalRowHeight, this.ClientSize.Height - 1, this.Width, this.Height - 1);

				for (int i = 0; i <= steuerung.maxTime / 10; i++)
					{
					e.Graphics.DrawLine(blackPen, totalRowHeight + i * 10 * zoom, this.Height - 1, totalRowHeight + i * 10 * zoom, this.Height - 7);
					e.Graphics.DrawString((i * 10).ToString(), calibriFont, blackBrush, new PointF(totalRowHeight + (i * 10 * zoom) - 8, this.Height - 18));
					}

				// aktuelle Zeit zeichnen
				e.Graphics.DrawLine(dottedPen, totalRowHeight + (int)Math.Round(timelineBarTime * zoom), 0, totalRowHeight + (int)Math.Round(timelineBarTime * zoom), Height);
				}
			}

		#endregion


		#region Mouse-Eventhandler

		/// <summary>
		/// Eventhandler, bei Mausbewegung über diesem Control
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Argumente</param>
		private void TimelineControl_MouseMove(object sender, MouseEventArgs e)
			{
			if (howToDrag == DragNDrop.MOVE_TIMELINE_BAR)
				{
				timelineBarTime = GetTimeAtControlPosition(e.Location, false);
				Invalidate();
				OnTimelineMoved();
				}
			else if (howToDrag == DragNDrop.MOVE_EVENT)
				{
				double time = GetTimeAtControlPosition(new Point(e.Location.X + eventToDragOffset, e.Location.Y), true);
				selectedEntry.MoveEvent(eventToDrag, time);
				OnEventChanged(new EventChangedEventArgs(m_selectedEntry, eventToDrag, howToDrag));
				Invalidate();
				}
			else if (howToDrag == DragNDrop.MOVE_EVENT_START)
				{
				selectedEntry.MoveEventStart(eventToDrag, GetTimeAtControlPosition(e.Location, true));
				OnEventChanged(new EventChangedEventArgs(m_selectedEntry, eventToDrag, howToDrag));
				Invalidate();
				}
			else if (howToDrag == DragNDrop.MOVE_EVENT_END)
				{
				selectedEntry.MoveEventEnd(eventToDrag, GetTimeAtControlPosition(e.Location, true));
				OnEventChanged(new EventChangedEventArgs(m_selectedEntry, eventToDrag, howToDrag));
				Invalidate();
				}
			else if (howToDrag == DragNDrop.NONE)
				{
				TimelineEvent te = GetTimelineEventAtControlPosition(e.Location, false);

				// Mauszeiger ändern, falls über TimelineBar
				if (Math.Abs(e.X - GetHorizontalClientPositionAtCurrentTime()) < 2)
					{
					this.Cursor = Cursors.SizeWE;
					}

				// Mauszeiger ändern, falls über Plus/Minus einer Gruppe
				else if (e.X < rowHeight && GetGroupAtClientPosition(e.Location, true) != null)
					{
					this.Cursor = Cursors.Hand;
					}

				// Mauszeiger ändern, falls über den Ecken eines Events des selektierten TimelineEntries
				else if (te != null)
					{
					if (Math.Abs(GetClientPositionForTimelineEvent(te).X - e.Location.X) < 3)
						{
						this.Cursor = Cursors.SizeWE;
						}
					else if (Math.Abs(GetClientPositionForTimelineEvent(te).X + te.eventLength * zoom - e.Location.X) < 3)
						{
						this.Cursor = Cursors.SizeWE;
						}
					else
						{
						this.Cursor = Cursors.SizeAll;
						}

					}

				// Mauszeiger normal
				else
					{
					this.Cursor = Cursors.Default;
					}
				}
			}

		/// <summary>
		/// Eventhandler beim Verlassen des Controls
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimelineControl_MouseLeave(object sender, EventArgs e)
			{
			this.Cursor = Cursors.Default;
			}

		/// <summary>
		/// Eventhandler beim Drücken der Maustaste
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimelineControl_MouseDown(object sender, MouseEventArgs e)
			{
			// wurde der Zeitschieber angeklickt?
			if ((e.Button == MouseButtons.Left) && (Math.Abs(e.X - GetHorizontalClientPositionAtCurrentTime()) < 2))
				{
				// Drag'n'Drop Modus setzen
				howToDrag = DragNDrop.MOVE_TIMELINE_BAR;
				}

			// wurde ein Plus-/Minussysmbol einer Gruppe angeklickt?
			else if (e.X < rowHeight && GetGroupAtClientPosition(e.Location, true) != null)
				{
				TimelineGroup tg = GetGroupAtClientPosition(e.Location, true);
				tg.collapsed = !tg.collapsed;
				}

			// wurde vielleicht was anderes wichtiges angeklickt?
			else
				{
				TimelineEntry newEntry = GetTimelineEntryAtControlPosition(e.Location);
				TimelineGroup newGroup = GetGroupAtClientPosition(e.Location, false);

				if (newEntry != m_selectedEntry || newGroup != m_selectedGroup)
					{
					m_selectedGroup = newGroup;
					m_selectedEntry = newEntry;
					OnSelectionChanged();
					}

				// wurde ein TimelineEntry angeklickt?
				if (m_selectedEntry != null)
					{
					// gucken, ob es sich bei te um ein TrafficLight handelt
					TrafficLight tl = m_selectedEntry as TrafficLight;
					if (tl != null)
						{
						switch (e.Button)
							{
						case MouseButtons.Left:
							#region TimelineEvent hinzufügen
							if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
								{
								double time = GetTimeAtControlPosition(e.Location, true);

								// Event hinzufügen, welches die Ampel umschaltet
								m_selectedEntry.AddEvent(new TimelineEvent(time, (m_selectedEntry.GetTimeOfNextEvent(time) - time) / 2, Color.Green, tl.SwitchToGreen, tl.SwitchToRed), true);

								Invalidate();
								}
							#endregion

							#region TimelineEntry selektieren
							else
								{
								// Drag'n'Drop für Events initialisieren
								TimelineEvent theEvent = GetTimelineEventAtControlPosition(e.Location, true);
								if (theEvent != null)
									{
									if (Math.Abs(GetClientPositionForTimelineEvent(theEvent).X - e.Location.X) < 3)
										{
										howToDrag = DragNDrop.MOVE_EVENT_START;
										eventToDrag = theEvent;
										}
									else if (Math.Abs(GetClientPositionForTimelineEvent(theEvent).X + theEvent.eventLength * zoom - e.Location.X) < 3)
										{
										howToDrag = DragNDrop.MOVE_EVENT_END;
										eventToDrag = theEvent;
										}
									else
										{
										mouseDownPosition = e.Location;
										eventToDragOffset = GetClientPositionForTimelineEvent(theEvent).X - e.Location.X;
										howToDrag = DragNDrop.MOVE_EVENT;
										eventToDrag = theEvent;
										}
									}

								}
							#endregion
							break;

						case MouseButtons.Right:
							#region TimelineEvent entfernen
							if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
								{
								double time = GetTimeAtControlPosition(e.Location, false);

								TimelineEvent eventToRemove = tl.GetEventAtTime(time);
								tl.RemoveEvent(eventToRemove);
								}
							#endregion
							break;
							}
						}
					}

				Invalidate();
				}
			}

		private void TimelineControl_MouseUp(object sender, MouseEventArgs e)
			{
			if (howToDrag == DragNDrop.MOVE_TIMELINE_BAR)
				{
				steuerung.AdvanceTo(GetTimeAtControlPosition(e.Location, false));
				}
			howToDrag = DragNDrop.NONE;
			}


		#endregion

		#region neue Events

		/// <summary>
		/// Delegate für einen EventHandler wenn die TimelineBar bewegt wurde
		/// </summary>
		/// <param name="sender">Absender des Events</param>
		/// <param name="e">Eventparameter</param>
		public delegate void TimelineMovedEventHandler(object sender, EventArgs e);

		/// <summary>
		/// TimelineMoved Ereignis tritt auf, wenn die Timeline mit Hilfe dieses Controls bewegt wurde
		/// </summary>
		public event TimelineMovedEventHandler TimelineMoved;

		/// <summary>
		/// Hilfsfunktion zum Absetzten des TimelineMoved Events
		/// </summary>
		protected void OnTimelineMoved()
			{
			if (TimelineMoved != null)
				{
				TimelineMoved(this, new EventArgs());
				}
			}


		/// <summary>
		/// EventArgs die einem SelectionChanged-Ereignis mitgegeben wird
		/// </summary>
		public class SelectionChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Erstellt ein neues SelectionChangedEventArgs
			/// </summary>
			/// <param name="tg">ausgewählte TimelineGroup</param>
			/// <param name="te">ausgewählte TimelineGroup</param>
			public SelectionChangedEventArgs(TimelineGroup tg, TimelineEntry te)
				{
				this.m_selectedGroup = tg;
				this.m_selectedEntry = te;
				}

			/// <summary>
			/// ausgewählte TimelineGroup
			/// </summary>
			private TimelineGroup m_selectedGroup;
			/// <summary>
			/// ausgewählte TimelineGroup
			/// </summary>
			public TimelineGroup selectedGroup
				{
				get { return m_selectedGroup; }
				}

			/// <summary>
			/// ausgewähltes TimelineEntry
			/// </summary>
			private TimelineEntry m_selectedEntry;
			/// <summary>
			/// ausgewähltes TimelineEntry
			/// </summary>
			public TimelineEntry selectedEntry
				{
				get { return m_selectedEntry; }
				}
			}

		/// <summary>
		/// Delegate für einen EventHandler wenn die selektierte Gruppe/Entry geändert wurde
		/// </summary>
		/// <param name="sender">Absender des Events</param>
		/// <param name="e">Eventparameter</param>
		public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

		/// <summary>
		/// SelectionChanged Ereignis tritt auf, wenn selectedGroup oder selectedEntry geändert wurde
		/// </summary>
		public event SelectionChangedEventHandler SelectionChanged;

		/// <summary>
		/// Hilfsfunktion zum Absetzten des SelectionChanged Events
		/// </summary>
		protected void OnSelectionChanged()
			{
			if (SelectionChanged != null)
				{
				SelectionChanged(this, new SelectionChangedEventArgs(m_selectedGroup, m_selectedEntry));
				}
			}




		/// <summary>
		/// EventArgs die einem SelectionChanged-Ereignis mitgegeben wird
		/// </summary>
		public class EventChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Erstellt ein neues SelectionChangedEventArgs
			/// </summary>
			/// <param name="te">ausgewähltes TimelineEntry</param>
			/// <param name="ev">ausgewähltes TimelineEvent</param>
			/// <param name="dragAction">Art des Drag and Drop-Vorganges</param>
			public EventChangedEventArgs(TimelineEntry te, TimelineEvent ev, DragNDrop dragAction)
				{
				this.m_selectedEntry = te;
				this.m_handeledEvent = ev;
				this.m_dragAction = dragAction;
				}

			/// <summary>
			/// TimelineEntry dessen Event verändert wurde
			/// </summary>
			private TimelineEntry m_selectedEntry;
			/// <summary>
			/// TimelineEntry dessen Event verändert wurde
			/// </summary>
			public TimelineEntry selectedEntry
				{
				get { return m_selectedEntry; }
				}

			/// <summary>
			/// verändertes TimelineEvent
			/// </summary>
			private TimelineEvent m_handeledEvent;
			/// <summary>
			/// verändertes TimelineEvent
			/// </summary>
			public TimelineEvent handeledEvent
				{
				get { return m_handeledEvent; }
				set { m_handeledEvent = value; }
				}

			/// <summary>
			/// Art, wie das TimelineEvent verändert wurde
			/// </summary>
			private DragNDrop m_dragAction;
			/// <summary>
			/// Art, wie das TimelineEvent verändert wurde
			/// </summary>
			public DragNDrop dragAction
				{
				get { return m_dragAction; }
				set { m_dragAction = value; }
				}
			}

		/// <summary>
		/// Delegate für einen EventHandler wenn die selektierte Gruppe/Entry geändert wurde
		/// </summary>
		/// <param name="sender">Absender des Events</param>
		/// <param name="e">Eventparameter</param>
		public delegate void EventChangedEventHandler(object sender, EventChangedEventArgs e);

		/// <summary>
		/// SelectionChanged Ereignis tritt auf, wenn selectedGroup oder selectedEntry geändert wurde
		/// </summary>
		public event EventChangedEventHandler EventChanged;

		/// <summary>
		/// Hilfsfunktion zum Absetzten des SelectionChanged Events
		/// </summary>
		/// <param name="e">Eventargumente</param>
		protected void OnEventChanged(EventChangedEventArgs e)
			{
			if (EventChanged != null)
				{
				EventChanged(this, e);
				}
			}




		#endregion

		}
	}
