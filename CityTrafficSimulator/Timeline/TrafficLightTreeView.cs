/*
 *  CityTrafficSimulator - a tool to simulate traffic in urban areas and on intersections
 *  Copyright (C) 2005-2014, Christian Schulte zu Berge
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
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

using CityTrafficSimulator.Tools.ObserverPattern;

namespace CityTrafficSimulator.Timeline
	{
	/// <summary>
	/// TreeView-Control zur Anzeige von geordneten TrafficLights, welches als Observer fungiert und sich so bei einer TimelineSteuerung anmelden kann
	/// </summary>
	public partial class TrafficLightTreeView : System.Windows.Forms.TreeView, IEventListener
		{
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
				if (m_steuerung != null)
					{
					m_steuerung.EntryChanged -= m_steuerung_EntryChanged;
					m_steuerung.GroupsChanged -= m_steuerung_GroupsChanged;
					}
				m_steuerung = value;
				if (m_steuerung != null)
					{
					m_steuerung.EntryChanged += new TimelineSteuerung.EntryChangedEventHandler(m_steuerung_EntryChanged);
					m_steuerung.GroupsChanged += new TimelineSteuerung.GroupsChangedEventHandler(m_steuerung_GroupsChanged);
					}
					
				}
			}

		void m_steuerung_GroupsChanged(object sender, EventArgs e)
			{
			UpdateTreeViewData();
			}

		void m_steuerung_EntryChanged(object sender, TimelineSteuerung.EntryChangedEventArgs e)
			{
			// Gruppen und darin enthaltenen Entries mit e.affectedEntry vergleichen
			foreach (TreeNode groupNode in Nodes)
				{
				foreach (TreeNode entryNode in groupNode.Nodes)
					{
					if (entryNode.Tag == e.affectedEntry)
						{
						entryNode.Text = e.affectedEntry.name;
						}
					}
				}
			}

		/// <summary>
		/// Standardkonstruktor
		/// </summary>
		public TrafficLightTreeView()
			{
			InitializeComponent();
			}

		/// <summary>
		/// Konstruktor, falls Control in Container
		/// </summary>
		/// <param name="container">übergeordneter Container</param>
		public TrafficLightTreeView(IContainer container)
			{
			container.Add(this);

			InitializeComponent();
			}


		/// <summary>
		/// baut den Inhalt der TreeView neu auf
		/// </summary>
		private void UpdateTreeViewData()
			{
			if (steuerung != null)
				{
				// SuspendLayout um Rechenzeit zu sparen
				this.SuspendLayout();

				// erstmal alles löschen
				Nodes.Clear();

				// Gruppen und darin enthaltenen Entries hinzufügen
				foreach (TimelineGroup tg in steuerung.groups)
					{
					TreeNode groupNode = new TreeNode(tg.title);
					groupNode.Tag = tg;
					if (tg.collapsed)
						groupNode.Collapse();
					else
						groupNode.Expand();

					

					foreach (TimelineEntry te in tg.entries)
						{
						TreeNode entryNode = new TreeNode(te.name);
						entryNode.Tag = te;
						groupNode.Nodes.Add(entryNode);
						}

					Nodes.Add(groupNode);
					}

				// Layout wieder einschalten
				this.ResumeLayout();
				}
			}

		/// <summary>
		/// Selektiert den Knoten des TreeViews, der das TimelineEntry te enthält.
		/// </summary>
		/// <param name="te">zu suchendes TimelineEntry</param>
		/// <returns>true, falls te gefunden und selektiert wurde, sonst false</returns>
		public bool SelectNodeByTimelineEntry(TimelineEntry te)
			{
			foreach (TreeNode groupNode in Nodes)
				{
				foreach (TreeNode entryNode in groupNode.Nodes)
					{
					TimelineEntry entryOfNode = entryNode.Tag as TimelineEntry;
					if (te == entryOfNode)
						{
						SelectedNode = entryNode;
						Invalidate();
						return true;
						}
					}
				}

			return false;
			}


		#region IEventListener Member

		/// <summary>
		/// Wird von der TimelineSteuerung aufgerufen und sorgt dafür, dass sich das Control neu aufbaut.
		/// </summary>
		public void Notify()
			{
			UpdateTreeViewData();
			Invalidate();
			}

		#endregion
		}
	}
