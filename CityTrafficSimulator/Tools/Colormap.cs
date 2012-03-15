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
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace CityTrafficSimulator.Tools
	{
	/// <summary>
	/// Wrapper for a list of colors offering interpolation methods.
	/// </summary>
	public class Colormap
		{
		#region Members

		/// <summary>
		/// List of all colors in this color map.
		/// </summary>
		private List<Color> _colors;

		#endregion

		#region Methods

		/// <summary>
		/// Creates a new color map containing the colors in <paramref name="colors"/>.
		/// </summary>
		/// <param name="colors">List of colors for the color map. Must not be empty.</param>
		public Colormap(List<Color> colors)
			{
			Debug.Assert(colors.Count > 0);
			_colors = colors;
			}

		/// <summary>
		/// Returns the color at the given position within interval [0,1]. If no such color is defined, linear interpolation between the surrounding colors will be performed.
		/// </summary>
		/// <param name="position">Position for the interpolated color. Must be within [0, 1].</param>
		/// <returns>The interpolated color at the given position.</returns>
		public Color GetInterpolatedColor(double position)
			{
			if (_colors.Count == 0)
				return Color.Black;

			if (position <= 0)
				return _colors[0];
			else if (position >= 1)
				return _colors[_colors.Count - 1];
			else
				{
				double cPos = position * (_colors.Count - 1);
				Color left = _colors[(int)Math.Floor(cPos)];
				Color right = _colors[(int)Math.Ceiling(cPos)];

				double alpha = (cPos - Math.Floor(cPos));
				double iAlpha = 1 - alpha;

				return Color.FromArgb(
					(int)(iAlpha * left.R + alpha * right.R),
					(int)(iAlpha * left.G + alpha * right.G),
					(int)(iAlpha * left.B + alpha * right.B));
				}
			}

		#endregion



		}
	}
