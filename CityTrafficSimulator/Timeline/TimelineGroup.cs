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


		private List<Pair<SpecificIntersection, double>>[] _conflictPoints;
		private List<Pair<SpecificIntersection, double>>[,] _conflictPointMatrix;

		private double[,] _interimTimes;

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

		#region Conflict Points / Interim Times

		/// <summary>
		/// Updates _conflictPoints array
		/// </summary>
		public void UpdateConflictPoints()
			{
			_conflictPointMatrix = new List<Pair<SpecificIntersection, double>>[_entries.Count, _entries.Count];
			_interimTimes = new double[_entries.Count, _entries.Count];

			// calculate bounds of intersection
			double minX, maxX, minY, maxY;
			minX = minY = Double.PositiveInfinity;
			maxX = maxY = Double.NegativeInfinity;
			foreach (TimelineEntry te in _entries)
				{
				TrafficLight tl = te as TrafficLight;
				if (tl != null)
					{
					foreach (LineNode ln in tl.assignedNodes)
						{
						minX = Math.Min(minX, Math.Min(ln.position.X, Math.Min(ln.inSlopeAbs.X, ln.outSlopeAbs.X)));
						maxX = Math.Max(maxX, Math.Max(ln.position.X, Math.Max(ln.inSlopeAbs.X, ln.outSlopeAbs.X)));
						minY = Math.Min(minY, Math.Min(ln.position.Y, Math.Min(ln.inSlopeAbs.Y, ln.outSlopeAbs.Y)));
						maxY = Math.Max(maxY, Math.Max(ln.position.Y, Math.Max(ln.inSlopeAbs.Y, ln.outSlopeAbs.Y)));
						}
					}
				}
			RectangleF bounds = new RectangleF((float)minX, (float)minY, (float)(maxX - minX), (float)(maxY - minY));

			// Gather all intersections from each entry to each entry within bounds
			_conflictPoints = new List<Pair<SpecificIntersection, double>>[_entries.Count];
			for (int i = 0; i < _entries.Count; i++)
				{
				_conflictPoints[i] = new List<Pair<SpecificIntersection, double>>();

				// initialize working stack
				Stack<Pair<NodeConnection, double>> connectionsToLook = new Stack<Pair<NodeConnection, double>>();
				TrafficLight tl = _entries[i] as TrafficLight;
				if (tl != null)
					{
					foreach (LineNode ln in tl.assignedNodes)
						{
						foreach (NodeConnection nc in ln.nextConnections)
							{
							connectionsToLook.Push(new Pair<NodeConnection, double>(nc, 0));
							}
						}
					}

				// work down stack
				double maxDistance = bounds.Width + bounds.Height;
				while (connectionsToLook.Count > 0)
					{
					// get NodeConnection-Distance pair
					Pair<NodeConnection, double> p = connectionsToLook.Pop();

					// add intersections
					foreach (Intersection inter in p.Left.intersections)
						{
						_conflictPoints[i].Add(new Pair<SpecificIntersection, double>(new SpecificIntersection(p.Left, inter), p.Right + inter.GetMyArcPosition(p.Left)));
						}

					// add next connections if within bounds
					double distance = p.Right + p.Left.lineSegment.length;
					if (distance < maxDistance && bounds.Contains(p.Left.endNode.position))
						{
						if (p.Left.endNode.tLight == null || !_entries.Contains(p.Left.endNode.tLight))
							{
							foreach (NodeConnection nc in p.Left.endNode.nextConnections)
								{
								connectionsToLook.Push(new Pair<NodeConnection, double>(nc, distance));
								}
							}
						}
					}
				}

			// now build conflictPoints array
			for (int from = 0; from < _entries.Count; from++)
				{
				for (int to = 0; to < _entries.Count; to++)
					{
					_conflictPointMatrix[from, to] = new List<Pair<SpecificIntersection, double>>();
					_interimTimes[from, to] = -1;

					if (from == to)
						continue;

					foreach (Pair<SpecificIntersection, double> pFrom in _conflictPoints[from])
						{
						foreach (Pair<SpecificIntersection, double> pTo in _conflictPoints[to])
							{
							if (pFrom.Left.intersection == pTo.Left.intersection)
								{
								_conflictPointMatrix[from, to].Add(pFrom);
								_interimTimes[from, to] = pFrom.Right / (10 * pFrom.Left.nodeConnection.targetVelocity);
								}
							}
						}
					}
				}
			}

		/// <summary>
		/// Returns a list with all computed conflict points between <paramref name="te"/> and all other entries in this timeline group.
		/// Each pair in the list describes a conflict point and its distance to <paramref name="te"/>.
		/// </summary>
		/// <param name="te">The TimelineEntry</param>
		/// <returns>A list with all computed conflict points between <paramref name="te"/> and all other entries in this timeline group.</returns>
		public List<Pair<SpecificIntersection, double>> GetConflictPoints(TimelineEntry te)
			{
			if (_conflictPoints == null)
				return null;

			for (int i = 0; i < _entries.Count; i++)
				{
				if (_entries[i] == te)
					return _conflictPoints[i];
				}
			return null;
			}


		public List<Pair<TimelineEntry, double>> GetInterimTimes(TimelineEntry te)
			{
			if (_interimTimes == null)
				return null;

			for (int from = 0; from < _entries.Count; from++)
				{
				if (_entries[from] == te)
					{
					List<Pair<TimelineEntry, double>> toReturn = new List<Pair<TimelineEntry, double>>();

					for (int to = 0; to < _entries.Count; to++)
						{
						if (_interimTimes[from, to] != -1)
							{
							toReturn.Add(new Pair<TimelineEntry, double>(_entries[to], _interimTimes[from, to]));
							}
						}

					return toReturn;
					}
				}

			return null;
			}

		#endregion

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
