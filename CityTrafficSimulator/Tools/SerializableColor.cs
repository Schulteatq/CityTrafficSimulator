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
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace CityTrafficSimulator.Tools
	{
	/// <summary>
	/// Serializable wrapper struct for System.Drawing.Color
	/// </summary>
	[Serializable]
	public struct SerializableColor
		{
		/// <summary>
		/// The wrapped Color
		/// </summary>
		private Color _color;

		/// <summary>
		/// ARGB version of the wrapped color (this will be serialized)
		/// </summary>
		public int argbColor
			{
			get { return _color.ToArgb(); }
			set { _color = Color.FromArgb(value); }
			}

		/// <summary>
		/// Implicit conversion SerializableColor -> Color
		/// </summary>
		/// <param name="sc">SerializableColor to convert</param>
		/// <returns>sc._color</returns>
		public static implicit operator Color(SerializableColor sc)
			{
			return sc._color;
			}

		/// <summary>
		/// Implicit conversion Color -> SerializableColor
		/// </summary>
		/// <param name="c">Color to convert</param>
		/// <returns>new SerializableColor(c)</returns>
		public static implicit operator SerializableColor(Color c)
			{
			return new SerializableColor(c);
			}

		/// <summary>
		/// Constructor, _color will be initialized by <paramref name="c"/>
		/// </summary>
		/// <param name="c">Color of this object</param>
		public SerializableColor(Color c)
			{
			_color = c;
			}
		}

	}
