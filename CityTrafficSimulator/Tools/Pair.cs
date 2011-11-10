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
	/// kapselt ein Paar von zwei gleichartigen Werten
	/// </summary>
	/// <typeparam name="T">Typ den beide Werte haben</typeparam>
    public struct Pair<T>
        {
		/// <summary>
		/// linker Teil des Paares
		/// </summary>
        public T Left;

		/// <summary>
		/// rechter Teil des paares
		/// </summary>
        public T Right;

		/// <summary>
		/// Standardkonstruktor für ein Paar vom Typ T
		/// </summary>
		/// <param name="Left">linker Teil des Paares</param>
		/// <param name="Right">rechter Teil des Paares</param>
		public Pair(T Left, T Right)
            {
            this.Left = Left;
            this.Right = Right;
            }
        }

	/// <summary>
	/// kapselt ein Paar von zwei unterschiedlichen Werten
	/// </summary>
	/// <typeparam name="S">Typ des ersten zu speichernden Wertes</typeparam>
	/// <typeparam name="T">Typ des zweiten zu speichernden Wertes</typeparam>
	public struct Pair<S, T>
		{
		/// <summary>
		/// linker Teil des Paares
		/// </summary>
		public S left;

		/// <summary>
		/// rechter Teil des Paares
		/// </summary>
		public T right;

		/// <summary>
		/// Standardkonstruktor für ein Paar vom Typ S, T
		/// </summary>
		/// <param name="left">linker Teil des Paares</param>
		/// <param name="right">rechter Teil des Paares</param>
		public Pair(S left, T right)
			{
			this.left = left;
			this.right = right;
			}
		}
    }
