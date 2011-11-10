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

namespace CityTrafficSimulator
	{
	/// <summary>
	/// Singleton class for global simulation time
	/// </summary>
	public class GlobalTime
		{
		#region Singleton stuff

		/// <summary>
		/// Singleton instance
		/// </summary>
		private static readonly GlobalTime m_instance = new GlobalTime();

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static GlobalTime Instance
			{
			get { return m_instance; }
			}

		/// <summary>
		/// Private Constructor - only to be used by singleton itsself.
		/// </summary>
		private GlobalTime()
			{
			m_currentTime = 0;
			}

		#endregion

		#region Fields and Variables

		/// <summary>
		/// current simulation time
		/// </summary>
		private double m_currentTime;
		/// <summary>
		/// current simulation time
		/// </summary>
		public double currentTime
			{
			get { return m_currentTime; }
			}

		/// <summary>
		/// current simulation time casted to float
		/// </summary>
		public float currentTimeAsFloat
			{
			get { return (float)m_currentTime; }
			}


		#endregion

		#region Methods

		/// <summary>
		/// Advances current time by time.
		/// </summary>
		/// <param name="time">Time to add to current time</param>
		public void Advance(double time)
			{
			m_currentTime += time;
			}

		/// <summary>
		/// Resets current time to 0.
		/// </summary>
		public void Reset()
			{
			m_currentTime = 0;
			}

		#endregion

		}
	}
