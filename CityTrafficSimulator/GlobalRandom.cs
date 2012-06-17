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

namespace CityTrafficSimulator
	{
	/// <summary>
	/// Singleton class for global random number generation
	/// </summary>
	public class GlobalRandom
		{
		#region Singleton stuff

		/// <summary>
		/// Singleton instance
		/// </summary>
		private static readonly GlobalRandom _instance = new GlobalRandom();

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static GlobalRandom Instance
			{
			get { return _instance; }
			}

		/// <summary>
		/// Private Constructor - only to be used by singleton itsself.
		/// </summary>
		private GlobalRandom()
			{
			_random = new Random(42);
			}

		#endregion

		#region Members

		/// <summary>
		/// random number generator
		/// </summary>
		private Random _random;
				
		#endregion

		#region Methods

		/// <summary>
		/// Resets the random number generator by the given seed
		/// </summary>
		/// <param name="seed">seed for the random number generator</param>
		public void Reset(int seed)
			{
			_random = new Random(seed);
			}

		/// <summary>
		/// Returns the next random int
		/// </summary>
		/// <returns></returns>
		public int Next()
			{
			return _random.Next();
			}

		/// <summary>
		/// Returns the next random non-negative int which is smaller than maxValue.
		/// </summary>
		/// <param name="maxValue">maximum value</param>
		/// <returns>a random non-negative int which is smaller than maxValue.</returns>
		public int Next(int maxValue)
			{
			return _random.Next(maxValue);
			}

		/// <summary>
		/// Returns the next random non-negative int which is smaller than maxValue and greater than or equals minValue.
		/// </summary>
		/// <param name="minValue">minimum value</param>
		/// <param name="maxValue">maximum value</param>
		/// <returns>a random non-negative int which is smaller than maxValue and greater than or equals minValue</returns>
		public int Next(int minValue, int maxValue)
			{
			return _random.Next(minValue, maxValue);
			}

		/// <summary>
		/// Returns the next random double value in [0, 1).
		/// </summary>
		/// <returns>a random double value in [0, 1).</returns>
		public double NextDouble()
			{
			return _random.NextDouble();
			}

		#endregion

		}
	}
