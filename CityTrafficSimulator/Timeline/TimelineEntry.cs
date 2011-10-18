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
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

using CityTrafficSimulator.Tools.ObserverPattern;

namespace CityTrafficSimulator.Timeline
	{
	/// <summary>
	/// Repräsentiert einen Eintrag in einer Timeline. Überspannt das komplette Zeitintervall der timeline und kann mehrere TimelineEvents enthalten
	/// </summary>
	[Serializable]
	public abstract class TimelineEntry : ISavable, IDisposable
		{
		#region Variablen und Eigenschaften

		/// <summary>
		/// Elterngruppe dieses TimelineEntries
		/// </summary>
		[XmlIgnore]
		public TimelineGroup parentGroup;


		/// <summary>
		/// Liste von Events die ausgeführt werden sollen
		/// </summary>
		protected MyLinkedList<TimelineEvent> m_events = new MyLinkedList<TimelineEvent>();
		/// <summary>
		/// Liste von Events die ausgeführt werden sollen
		/// </summary>
		public MyLinkedList<TimelineEvent> events
			{
			get { return m_events; }
			}

		/// <summary>
		/// Standardevent der ausgeführt werden soll, wenn sonst nix is
		/// </summary>
		private TimelineEvent.EventAction m_defaultAction;
		/// <summary>
		/// Standardevent der ausgeführt werden soll, wenn sonst nix is
		/// </summary>
		[XmlIgnore]
		public TimelineEvent.EventAction defaultAction
			{
			get { return m_defaultAction; }
			set { m_defaultAction = value; }
			}


		/// <summary>
		/// Name des TimelineEntries
		/// </summary>
		protected String m_name;
		/// <summary>
		/// Name des TimelineEntries
		/// </summary>
		public virtual String name
			{
			get { return m_name; }
			set { m_name = value; OnEntryChanged(new EntryChangedEventArgs(this)); }
			}

		/// <summary>
		/// Länge des TimelineEntries
		/// </summary>
		protected double m_maxTime;
		/// <summary>
		/// Länge des TimelineEntries
		/// </summary>
		public double maxTime
			{
			get { return m_maxTime; }
			set { m_maxTime = value; AdaptEventsToMaxTime(); }
			}

		/// <summary>
		/// Farbe durch die das Event repräsentiert werden soll
		/// </summary>
		private Color m_color;
		/// <summary>
		/// Farbe durch die das Event repräsentiert werden soll
		/// </summary>
		[XmlIgnore]
		public Color color
			{
			get { return m_color; }
			set { m_color = value; OnEntryChanged(new EntryChangedEventArgs(this)); }
			}
		/// <summary>
		/// Farbe im ARGB-Format (Für Serialisierung benötigt)
		/// </summary>
		public int argbColor
			{
			get { return m_color.ToArgb(); }
			set { m_color = Color.FromArgb(value); OnEntryChanged(new EntryChangedEventArgs(this)); }
			}

		#endregion

		#region Prozeduren
		/// <summary>
		/// leerer Standardkonstruktor
		/// </summary>
		public TimelineEntry()
			{
			}

		/// <summary>
		/// Standardkonstruktor zur Verwendung
		/// </summary>
		/// <param name="name">Name des TimelineEntry</param>
		/// <param name="color">Farbe durch die das TimelineEntry dargestellt werden soll</param>
		public TimelineEntry(String name, Color color)
			{
			this.name = name;
			this.color = color;
			}

		/// <summary>
		/// fügt dem TimelineEntry ein fertiges TimelineEvent an der richtigen Stelle hinzu
		/// </summary>
		/// <param name="eventToAdd">TimelineEvent welches eingefügt werden soll</param>
		public void AddEvent(TimelineEvent eventToAdd)
			{
			if (events.Count != 0)
				{
				LinkedListNode<TimelineEvent> ln = events.First;
				while (ln != null && ln.Value.eventTime + ln.Value.eventLength < eventToAdd.eventTime)
					{
					ln = ln.Next;
					}

				if (ln == null)
					{
					events.AddLast(eventToAdd);
					}
				else
					{
					if (eventToAdd.eventTime + eventToAdd.eventLength > ln.Value.eventTime)
						{
						Exception e = new Exception("TimelineEvents überlappen sich");
						//e.Data.Add("eventToAdd", eventToAdd);
						//e.Data.Add("overlappingEvent", ln);
						throw e;
						}
					events.AddBefore(ln, eventToAdd);
					}				
				}
			else
				{
				events.AddFirst(eventToAdd);
				}
			OnEntryChanged(new EntryChangedEventArgs(this));
			}

		/// <summary>
		/// entfernt das TimelineEvent von diesem TimelineEntry
		/// </summary>
		/// <param name="eventToRemove">zu entfernendes TimelineEvent</param>
		public void RemoveEvent(TimelineEvent eventToRemove)
			{
			m_events.Remove(eventToRemove);
			OnEntryChanged(new EntryChangedEventArgs(this));
			}



		/// <summary>
		/// Gibt den Startzeitpunkt des nächsten Events nach time zurück oder maxTime, falls kein solches mehr existiert
		/// </summary>
		/// <param name="time">Zeitpunkt ab dem gesucht werden soll</param>
		/// <returns></returns>
		public double GetTimeOfNextEvent(double time)
			{
			LinkedListNode<TimelineEvent> lln = events.First;
			while (lln != null && lln.Value.eventTime < time)
				{
				lln = lln.Next;
				}

			return (lln != null) ? lln.Value.eventTime : maxTime;
			}


		/// <summary>
		/// gibt das TimelineEvent zurück, welches bei time aktiv ist oder null, falls es kein solches gibt
		/// </summary>
		/// <param name="time">Zeitpunkt zu dem das TimelineEvent aktiv sein soll</param>
		/// <returns>Ein TimelineEvent, welches bei time aktiv ist oder null, falls es kein solches gibt</returns>
		public TimelineEvent GetEventAtTime(double time)
			{
			foreach (TimelineEvent te in events)
				{
				if (te.eventTime <= time && te.eventTime + te.eventLength >= time)
					{
					return te;
					}
				}
			return null;
			}

		/// <summary>
		/// bewegt TimelineEvent eventToMove als ganzes aber nur so weit, dass es sich mit keinen anderen Events überschneidet oder aus dem Entry herausragt
		/// </summary>
		/// <param name="eventToMove">Event welches bewegt werden soll</param>
		/// <param name="time">Zeit an die das Event bewegt werden soll</param>
		public void MoveEvent(TimelineEvent eventToMove, double time)
			{
			LinkedListNode<TimelineEvent> lln = GetListNode(eventToMove);
			if (lln != null)
				{
				double min = (lln.Previous != null) ? lln.Previous.Value.eventTime + lln.Previous.Value.eventLength : 0;
				double max = (lln.Next != null) ? lln.Next.Value.eventTime - eventToMove.eventLength : this.maxTime - eventToMove.eventLength;

				if (time < min)
					eventToMove.eventTime = min;
				else if (time > max)
					eventToMove.eventTime = max;
				else
					eventToMove.eventTime = time;
				}
			OnEntryChanged(new EntryChangedEventArgs(this));
			}

		/// <summary>
		/// bewegt den Beginn des TimelineEvents eventToMove aber nur so weit, dass es sich mit keinen anderen Events überschneidet, aus dem Entry herausragt oder kleiner als 1 wird
		/// </summary>
		/// <param name="eventToMove">Event welches bewegt werden soll</param>
		/// <param name="time">Zeit an die das Event bewegt werden soll</param>
		public void MoveEventStart(TimelineEvent eventToMove, double time)
			{
			LinkedListNode<TimelineEvent> lln = GetListNode(eventToMove);
			if (lln != null)
				{
				double min = (lln.Previous != null) ? lln.Previous.Value.eventTime + lln.Previous.Value.eventLength : 0;
				double max = eventToMove.eventTime + eventToMove.eventLength - 1;

				if (time < min)
					{
					eventToMove.eventTime = min;
					eventToMove.eventLength = max + 1 - eventToMove.eventTime;
					}
				else if (time > max)
					{
					eventToMove.eventTime = max;
					eventToMove.eventLength = 1;
					}
				else
					{
					eventToMove.eventTime = time;
					eventToMove.eventLength = max + 1 - eventToMove.eventTime;
					}
				}
			OnEntryChanged(new EntryChangedEventArgs(this));
			}

		/// <summary>
		/// bewegt das Ende des TimelineEvents eventToMove aber nur so weit, dass es sich mit keinen anderen Events überschneidet, aus dem Entry herausragt oder kleiner als 1 wird
		/// </summary>
		/// <param name="eventToMove">Event welches bewegt werden soll</param>
		/// <param name="time">Zeit an die das Event bewegt werden soll</param>
		public void MoveEventEnd(TimelineEvent eventToMove, double time)
			{
			LinkedListNode<TimelineEvent> lln = GetListNode(eventToMove);
			if (lln != null)
				{
				double min = eventToMove.eventTime + 1;
				double max = (lln.Next != null) ? lln.Next.Value.eventTime : this.maxTime;

				if (time < min)
					{
					eventToMove.eventLength = min - eventToMove.eventTime;
					}
				else if (time > max)
					{
					eventToMove.eventLength = max - eventToMove.eventTime;
					}
				else
					{
					eventToMove.eventLength = time - eventToMove.eventTime;
					}
				}
			OnEntryChanged(new EntryChangedEventArgs(this));
			}


		/// <summary>
		/// Gibt den LinkedListNode zurück, der e enthält
		/// </summary>
		/// <param name="e">Event nach dem zu suchen ist</param>
		/// <returns>den erstbesten LinkedListNode mit Value == e oder null, falls kein solcher existiert</returns>
		private LinkedListNode<TimelineEvent> GetListNode(TimelineEvent e)
			{
			LinkedListNode<TimelineEvent> lln = events.First;
			while (lln != null)
				{
				if (lln.Value == e)
					{
					return lln;
					}
				lln = lln.Next;
				}
			return null;
			}


		/// <summary>
		/// sorgt dafür, dass das TimelineEntry den Status wie zum Zeitpunkt time hat
		/// </summary>
		/// <param name="time">Zeit zu der sich die Timeline bewegen soll</param>
		public void AdvanceTo(double time)
			{
			// schauen wir mal welches Event bei time aktiv ist:
			LinkedListNode<TimelineEvent> lln = events.First;
			while (lln != null)
				{
				// lln.Value ist aktiv
				if (lln.Value.eventTime <= time && lln.Value.eventTime + lln.Value.eventLength >= time)
					{
					lln.Value.eventStartAction();
					return;
					}
				// lln.Value ist schon vorbei, aber lln.Next.Value startet erst nach time
				else if (lln.Value.eventTime + lln.Value.eventLength < time && (lln.Next == null || lln.Next.Value.eventTime > time))
					{
					lln.Value.eventEndAction();
					return;
					}

				
				lln = lln.Next;
				}
			defaultAction();
			}


		/// <summary>
		/// Updates all contained events so that they fit into the new maxTime.
		/// </summary>
		public void AdaptEventsToMaxTime()
			{
			List<TimelineEvent> eventsToDelete = new List<TimelineEvent>();

			foreach (TimelineEvent te in m_events)
				{
				if (te.eventTime < m_maxTime && te.eventTime + te.eventLength > m_maxTime)
					{
					te.eventLength = m_maxTime - te.eventTime;
					}
				else if (te.eventTime >= m_maxTime)
					{
					eventsToDelete.Add(te);
					}
				}

			foreach (TimelineEvent te in eventsToDelete)
				{
				m_events.Remove(te);
				}

			OnEntryChanged(new EntryChangedEventArgs(this));
			}
		
		#endregion

		#region EntryChanged-Ereignis

		/// <summary>
		/// EventArgs die einem EntryChanged-Ereignis mitgegeben wird
		/// </summary>
		public class EntryChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Erstellt ein neues SelectionChangedEventArgs
			/// </summary>
			/// <param name="te"></param>
			public EntryChangedEventArgs(TimelineEntry te)
				{
				this.m_affectedEntry = te;
				}

			/// <summary>
			/// beteiligtes TimelineEntry
			/// </summary>
			private TimelineEntry m_affectedEntry;
			/// <summary>
			/// beteiligtes TimelineEntry
			/// </summary>
			public TimelineEntry affectedEntry
				{
				get { return m_affectedEntry; }
				}
			}

		/// <summary>
		/// Delegate für einen EventHandler wenn ein TimelineEntry geändert wurde
		/// </summary>
		/// <param name="sender">Absender des Events</param>
		/// <param name="e">Eventparameter</param>
		public delegate void EntryChangedEventHandler(object sender, EntryChangedEventArgs e);

		/// <summary>
		/// EntryChanged Ereignis tritt auf, wenn ein TimelineEntry geändert wurde
		/// </summary>
		public event EntryChangedEventHandler EntryChanged;

		/// <summary>
		/// Hilfsfunktion zum Absetzten des EntryChanged Events
		/// </summary>
		/// <param name="e">Eventparameter</param>
		protected void OnEntryChanged(EntryChangedEventArgs e)
			{
			if (EntryChanged != null)
				{
				EntryChanged(this, e);
				}
			}


		#endregion

		#region ISavable Member

		/// <summary>
		/// bereitet das TimelineEntry für die XML-Serialisierung vor
		/// </summary>
		public virtual void PrepareForSave()
			{
			foreach (TimelineEvent te in m_events)
				{
				te.PrepareForSave();
				}
			}

		/// <summary>
		/// stellt das TimelineEntry nach XML-Deserialisierung wieder her
		/// </summary>
		/// <param name="saveVersion">Version der gespeicherten Datei</param>
		/// <param name="nodesList">Liste aller LineNodes</param>
		public virtual void RecoverFromLoad(int saveVersion, List<LineNode> nodesList)
			{
			foreach (TimelineEvent te in m_events)
				{
				te.RecoverFromLoad(saveVersion, nodesList);
				}
			}

		#endregion

		#region IDisposable Member

		/// <summary>
		/// räumt auf, sodass das TimelineEntry gelöscht werden kann
		/// </summary>
		public virtual void Dispose()
			{
			
			}

		#endregion
		}
	}
