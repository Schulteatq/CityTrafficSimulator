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

namespace CityTrafficSimulator.Verkehr
	{
	/// <summary>
	/// Steuert das Verkehrsaufkommen
	/// </summary>
	public class VerkehrSteuerung
		{
		#region Klassenmember inklusive Modifikationsmethoden

		#region Startpunkte

		/// <summary>
		/// Liste der Startpunkte
		/// </summary>
		private List<BunchOfNodes> m_startPoints = new List<BunchOfNodes>();
		/// <summary>
		/// Liste der Startpunkte
		/// </summary>
		public List<BunchOfNodes> startPoints
			{
			get { return m_startPoints; }
			}

		/// <summary>
		/// fügt eine BunchOfNodes den startPoints hinzu
		/// </summary>
		/// <param name="sp">hinzuzufügende BunchOfNodes</param>
		public void AddStartPoint(BunchOfNodes sp)
			{
			m_startPoints.Add(sp);
			}

		/// <summary>
		/// entfernt die BunchOfNodes sp aus startPoints
		/// </summary>
		/// <param name="sp">zu löschende BunchOfNodes</param>
		public void RemoveStartPoint(BunchOfNodes sp)
			{
			m_startPoints.Remove(sp);
			}

		/// <summary>
		/// Leert die Liste der Startpunkte
		/// </summary>
		public void ClearStartPoints()
			{
			m_startPoints.Clear();
			}

		#endregion

		#region Zielpunkte

		/// <summary>
		/// Liste der Zielpunkte
		/// </summary>
		private List<BunchOfNodes> m_targetPoints = new List<BunchOfNodes>();
		/// <summary>
		/// Liste der Zielpunkte
		/// </summary>
		public List<BunchOfNodes> targetPoints
			{
			get { return m_targetPoints; }
			}

		/// <summary>
		/// fügt eine BunchOfNodes den targetPoints hinzu
		/// </summary>
		/// <param name="sp">hinzuzufügende BunchOfNodes</param>
		public void AddTargetPoint(BunchOfNodes sp)
			{
			m_targetPoints.Add(sp);
			}

		/// <summary>
		/// entfernt die BunchOfNodes sp aus targetPoints
		/// </summary>
		/// <param name="sp">zu löschende BunchOfNodes</param>
		public void RemoveTargetPoint(BunchOfNodes sp)
			{
			m_targetPoints.Remove(sp);
			}

		/// <summary>
		/// Leert die Liste der Zielpunkte
		/// </summary>
		public void ClearTargetPoints()
			{
			m_targetPoints.Clear();
			}

		#endregion

		/// <summary>
		/// Liste von Fahraufträgen
		/// </summary>
		private List<Auftrag> m_auftraege  = new List<Auftrag>();
		/// <summary>
		/// Liste von Fahraufträgen
		/// </summary>
		public List<Auftrag> auftraege
			{
			get { return m_auftraege; }
			}
		
		#endregion


		#region Event-Gedöns

		

		#endregion

		}
	}
