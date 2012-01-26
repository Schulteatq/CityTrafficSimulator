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
		private static readonly GlobalTime _instance = new GlobalTime();

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static GlobalTime Instance
			{
			get { return _instance; }
			}

		/// <summary>
		/// Private Constructor - only to be used by singleton itsself.
		/// </summary>
		private GlobalTime()
			{
			currentTime = 0;
			cycleTime = 50;
			ticksPerSecond = 15;
			}

		#endregion

		#region Fields and Variables

		/// <summary>
		/// cycle time
		/// </summary>
		public double cycleTime { get; private set; }

		/// <summary>
		/// Number of ticks per second
		/// </summary>
		public double ticksPerSecond { get; private set; }

		/// <summary>
		/// current simulation time
		/// </summary>
		public double currentTime { get; private set; }
		
		/// <summary>
		/// current simulation time casted to float
		/// </summary>
		public float currentTimeAsFloat
			{
			get { return (float)currentTime; }
			}

		/// <summary>
		/// current tick number modulo cycle time
		/// </summary>
		public int currentCycleTick
			{
			get { return (int)((currentTime % cycleTime) * ticksPerSecond); }
			}


		#endregion

		#region Methods

		/// <summary>
		/// Advances current time by time.
		/// </summary>
		/// <param name="time">Time to add to current time</param>
		public void Advance(double time)
			{
			currentTime += time;
			}

		/// <summary>
		/// Resets current time to 0.
		/// </summary>
		public void Reset()
			{
			currentTime = 0;
			}

		/// <summary>
		/// Updates cylce time and ticks per second parameters.
		/// </summary>
		/// <param name="cycleTime">Cycle time</param>
		/// <param name="ticksPerSecond">Number of ticks/second</param>
		public void UpdateSimulationParameters(double cycleTime, double ticksPerSecond)
			{
			this.cycleTime = cycleTime;
			this.ticksPerSecond = ticksPerSecond;
			}

		#endregion

		}
	}
