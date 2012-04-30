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
using System.Xml;
using System.Xml.Serialization;


namespace CityTrafficSimulator.Verkehr
	{
	/// <summary>
	/// Ansammlung von LineNodes, die unter einem Namen zusammengefasst wird
	/// </summary>
	[Serializable]
	public class BunchOfNodes : ISavable
		{
		#region Hashcodes

		/// <summary>
		/// Class variable which stores the last used hash code - needs to be incremented on every class instantiation.
		/// </summary>
		public static int hashcodeIndex = 0;

		/// <summary>
		/// Hash code of this very object.
		/// </summary>
		public int hashcode = -1;

		#endregion

		#region Klassenmember

		/// <summary>
		/// Name dieser Ansammlung von LineNodes
		/// </summary>
		private string _title;
		/// <summary>
		/// Name dieser Ansammlung von LineNodes
		/// </summary> 
		public string title
			{
			get { return _title; }
			set { _title = value; }
			}

		/// <summary>
		/// Liste der teilnehmenden LineNodes
		/// </summary>
		[XmlIgnore]
		private List<LineNode> _nodes = new List<LineNode>();
		/// <summary>
		/// Liste der teilnehmenden LineNodes
		/// </summary>
		[XmlIgnore]
		public List<LineNode> nodes
			{
			get { return _nodes; }
			set { _nodes = value; }
			}

		#endregion

		#region Konstruktoren & Methoden

		/// <summary>
		/// Erstellt eine neue leere BunchOfNodes mit dem Titel title
		/// </summary>
		/// <param name="nodes">List of nodes for this BunchOfNodes</param>
		/// <param name="title">Titel dieser Ansammlung von Nodes</param>
		public BunchOfNodes(List<LineNode> nodes, string title)
			{
			hashcode = hashcodeIndex++;

			this._nodes = nodes;
			this._title = title;
			}

		/// <summary>
		/// DO NOT USE: Empty Constructor - only needed for XML Serialization
		/// </summary>
		public BunchOfNodes()
			{
			}

		/// <summary>
		/// Returns the title of this bunch of nodes
		/// </summary>
		/// <returns>this.title</returns>
		public override string ToString()
			{
			return _title;
			}

		#endregion


		#region ISavable Member

		/// <summary>
		/// Hashwerte der Startknoten (nur für XML-Serialisierung benötigt)
		/// </summary>
		public List<int> nodeHashes = new List<int>();

		/// <summary>
		/// Bereitet den Fahrauftrag für die XML-Serialisierung vor
		/// </summary>
		public void PrepareForSave()
			{
			nodeHashes.Clear();
			foreach (LineNode ln in _nodes)
				nodeHashes.Add(ln.GetHashCode());
			}

		/// <summary>
		/// Stellt die Referenzen auf LineNodes wieder her
		/// (auszuführen nach XML-Deserialisierung)
		/// </summary>
		/// <param name="saveVersion">Version der gespeicherten Datei</param>
		/// <param name="nodesList">Liste der bereits wiederhergestellten LineNodes</param>
		public void RecoverFromLoad(int saveVersion, List<LineNode> nodesList)
			{
			foreach (LineNode ln in nodesList)
				{
				if (nodeHashes.Contains(ln.GetHashCode()))
					_nodes.Add(ln);
				}
			}

		#endregion
		}
	}
