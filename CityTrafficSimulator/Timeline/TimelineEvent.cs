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
using System.Drawing;
using System.Xml.Serialization;

namespace CityTrafficSimulator.Timeline
	{
	/// <summary>
	/// Einzelnes Event einer Timeline, gehört immer zu einem TimelineEntry, und hat zwei Aktionen die bei eventStart und eventEnde ausgeführt werden sollen.
	/// </summary>
	[Serializable]
	public class TimelineEvent : ISavable
		{
		#region Delegates

		/// <summary>
		/// delegate für Befehl der ausgeführt werden soll, wenn das TimelineEvent eintritt
		/// </summary>
		public delegate void EventAction();

		#endregion

		#region Variablen und Eigenschaften

		/// <summary>
		/// Zeit wenn das TimelineEvent eintritt
		/// </summary>
		private double _eventTime;
		/// <summary>
		/// Zeit wenn das TimelineEvent eintritt
		/// </summary>
		public double eventTime
			{
			get { return _eventTime; }
			set { _eventTime = value; }
			}

		/// <summary>
		/// Dauer des TimelineEvents
		/// </summary>
		private double _eventLength;
		/// <summary>
		/// Dauer des TimelineEvents
		/// </summary>
		public double eventLength
			{
			get { return _eventLength; }
			set { _eventLength = value; }
			}


		/// <summary>
		/// Befehl der ausgeführt werden soll, wenn das TimelineEvent eintritt
		/// </summary>
		private EventAction _eventStartAction;
		/// <summary>
		/// Befehl der ausgeführt werden soll, wenn das TimelineEvent eintritt
		/// </summary>
		[XmlIgnore]
		public EventAction eventStartAction
			{
			get { return _eventStartAction; }
			set { _eventStartAction = value; }
			}

		/// <summary>
		/// Befehl der ausgeführt werden soll, wenn das TimelineEvent zu Ende ist
		/// </summary>
		private EventAction _eventEndAction;
		/// <summary>
		/// Befehl der ausgeführt werden soll, wenn das TimelineEvent zu Ende ist
		/// </summary>
		[XmlIgnore]
		public EventAction eventEndAction
			{
			get { return _eventEndAction; }
			set { _eventEndAction = value; }
			}


		/// <summary>
		/// Farbe durch die das Event repräsentiert werden soll
		/// </summary>
		private Color _color;
		/// <summary>
		/// Farbe durch die das Event repräsentiert werden soll
		/// </summary>
		[XmlIgnore]
		public Color color
			{
			get { return _color; }
			set { _color = value; }
			}
		/// <summary>
		/// Farbe im ARGB-Format (Für Serialisierung benötigt)
		/// </summary>
		public int argbColor
			{
			get { return _color.ToArgb(); }
			set { _color = Color.FromArgb(value); }
			}
		
		#endregion

		#region Prozeduren
		/// <summary>
		/// Standardkonstruktur nur zur internenen Verwendung!
		/// </summary>
		public TimelineEvent()
			{
			}

		/// <summary>
		/// Standardkonstruktor zur erstellung eines neuen TimelineEvents
		/// </summary>
		/// <param name="time">Zeit, wann das Event eintreten soll</param>
		/// <param name="length">Länge des Events.</param>
		/// <param name="color">Farbe durch die das Event repräsentiert werden soll</param>
		/// <param name="startAction">Funktion die bei Eintreten aufgerufen werden soll</param>
		/// <param name="endAction">Funktion die beim Ende des Events aufgerufen werden soll</param>
		public TimelineEvent(double time, double length, Color color, EventAction startAction, EventAction endAction)
			{
			this.eventTime = time;
			this.eventLength = length;
			this.color = color;
			this.eventStartAction = startAction;
			this.eventEndAction = endAction;
			}

		#endregion


		#region ISavable Member

		/// <summary>
		/// bereitet das TimelineEvent zur Xml-Serialisierung vor
		/// </summary>
		public void PrepareForSave()
			{

			}

		/// <summary>
		/// Stellt das TimelineEvent nach dem Laden wieder her
		/// </summary>
		/// <param name="saveVersion">Version der gespeicherten Datei</param>
		/// <param name="nodesList">Eine Liste mit sämtlichen existierenden Linien</param>
		public void RecoverFromLoad(int saveVersion, List<LineNode> nodesList)
			{
	
			}

		#endregion
		}
	}
