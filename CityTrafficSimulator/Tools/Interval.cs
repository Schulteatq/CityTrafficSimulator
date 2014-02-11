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
using System.Diagnostics;
using System.Text;

namespace CityTrafficSimulator
    {
    /// <summary>
    /// Generisches abgeschlossenes Intervall
    /// </summary>
    public struct Interval<T> where T : IComparable<T>
        {
		/// <summary>
		/// linke Grenze des Intervalls
		/// </summary>
        public T left;
		/// <summary>
		/// rechte Grenze des Intervalls
		/// </summary>
        public T right;

		/// <summary>
		/// Standardkonstruktor
		/// </summary>
		/// <param name="Start">linke Intervallgrenze</param>
		/// <param name="Ende">rechte Intervallgrenze</param>
        public Interval(T Start, T Ende)
            {
	        this.left = Start;
            this.right = Ende;
            }

        /// <summary>
        /// überprüft, ob Value im abgeschlossenen Intervall liegt
        /// </summary>
        public bool Contains(T Value)
            {
            return ((Value.CompareTo(left) >= 0) && (Value.CompareTo(right) <= 0));
            }

		/// <summary>
		/// überprüft, ob sich das Intervall mit otherInterval echt schneidet
		/// </summary>
		/// <param name="otherInterval">zu prüfendes Intervall</param>
		/// <returns></returns>
		public bool IntersectsTrue(Interval<T> otherInterval)
			{
			if ((otherInterval.left.CompareTo(left) < 0 && otherInterval.right.CompareTo(left) > 0)
					|| (otherInterval.left.CompareTo(left) > 0 && otherInterval.right.CompareTo(right) <= 0)
					|| (otherInterval.left.CompareTo(left) >= 0 && otherInterval.right.CompareTo(right) < 0)
					|| (otherInterval.left.CompareTo(right) < 0 && otherInterval.right.CompareTo(right) > 0))
				{
				return true;
				}
			return false;
			}
        }
    }
