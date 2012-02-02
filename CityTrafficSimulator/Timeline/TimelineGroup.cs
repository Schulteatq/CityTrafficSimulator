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
using System.Drawing;
using System.Xml.Serialization;

using CityTrafficSimulator.Tools;

namespace CityTrafficSimulator.Timeline
	{
	/// <summary>
	/// fasst mehrere TimelineEntries in einer Gruppe zusammen
	/// </summary>
	[Serializable]
	public class TimelineGroup : ISavable
		{
		#region Variablen und Eigenschaften

		/// <summary>
		/// Titel dieser Gruppe
		/// </summary>
		private string _title;
		/// <summary>
		/// Titel dieser Gruppe
		/// </summary>
		public string title
			{
			get { return _title; }
			set { _title = value; OnGroupChanged(); }
			}


		/// <summary>
		/// Flag, ob diese TimelineGroup zusammengefaltet ist (Anzeige)
		/// </summary>
		private bool _collapsed;
		/// <summary>
		/// Flag, ob diese TimelineGroup zusammengefaltet ist (Anzeige)
		/// </summary>
		public bool collapsed
			{
			get { return _collapsed; }
			set { _collapsed = value; OnGroupChanged(); }
			}


		/// <summary>
		/// Liste von den zu dieser TimelineGroup gehörenden TimelineEntries
		/// </summary>
		private List<TimelineEntry> _entries = new List<TimelineEntry>();
		/// <summary>
		/// Liste von den zu dieser TimelineGroup gehörenden TimelineEntries
		/// </summary>
		public List<TimelineEntry> entries
			{
			get { return _entries; }
			set { _entries = value; }
			}


		#endregion


		#region Prozeduren

		/// <summary>
		/// leerer Standardkonstruktor - wird nur für XML-Serialisierung benötigt.
		/// </summary>
		public TimelineGroup()
			{

			}

		/// <summary>
		/// Standardkonstruktor, erstellt eine neue leere TimelineGroup mit dem Namen title
		/// </summary>
		/// <param name="title">Titel dieser Gruppe</param>
		/// <param name="collapsed">Flag, ob diese TimelineGroup zusammengefaltet ist (Anzeige)</param>
		public TimelineGroup(string title, bool collapsed)
			{
			this._title = title;
			this._collapsed = collapsed;
			}


		/// <summary>
		/// fügt der Timeline ein fertiges TimelineEvent an der richtigen Stelle hinzu
		/// </summary>
		/// <param name="entryToAdd">TimelineEntry welcher eingefügt werden soll</param>
		public void AddEntry(TimelineEntry entryToAdd)
			{
			_entries.Add(entryToAdd);
			}


		/// <summary>
		/// Stringausgabe
		/// </summary>
		/// <returns>this.title</returns>
		public override string ToString()
			{
			return _title;
			}

		#endregion

		#region GroupsChanged-Ereignis

		/// <summary>
		/// Delegate für einen EventHandler wenn etwas an TimelineSteuerung.groups geändert wurde
		/// </summary>
		/// <param name="sender">Absender des Events</param>
		/// <param name="e">Eventparameter</param>
		public delegate void GroupChangedEventHandler(object sender, EventArgs e);

		/// <summary>
		/// GroupsChanged Ereignis tritt auf, wenn etwas an TimelineSteuerung.groups geändert wurde
		/// </summary>
		public event GroupChangedEventHandler GroupChanged;

		/// <summary>
		/// Hilfsfunktion zum Absetzten des GroupsChanged Events
		/// </summary>
		protected void OnGroupChanged()
			{
			if (GroupChanged != null)
				{
				GroupChanged(this, new EventArgs());
				}
			}



		#endregion

		#region ISavable Member

		/// <summary>
		/// bereitet das TimelineEntry für die XML-Serialisierung vor
		/// </summary>
		public virtual void PrepareForSave()
			{
			foreach (TimelineEntry te in _entries)
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
			foreach (TimelineEntry te in _entries)
				{
				te.parentGroup = this;
				te.RecoverFromLoad(saveVersion, nodesList);
				}
			}


		#endregion
		}
	}
