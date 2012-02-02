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
	/// Sammlung von häufig benötigten Mathefunktionen
	/// </summary>
    public static class Math2
        {
		/// <summary>
		/// Calculates x^3
		/// </summary>
		/// <param name="x">value</param>
		/// <returns>x^3</returns>
		public static double Cubic(double x)
			{
			return x * x * x;
			}

		/// <summary>
		/// Berechnet das Quadrat einer Zahl
		/// </summary>
		/// <param name="x">Zahl dessen Quadrat berechnet werden soll</param>
		/// <returns>x*x</returns>
        public static double Square(double x)
    		{
			return x * x;
	    	}

		/// <summary>
		/// Berechnet das Quadrat einer Zahl
		/// </summary>
		/// <param name="x">Zahl dessen Quadrat berechnet werden soll</param>
		/// <returns>x*x</returns>
		public static float Square(float x)
		    {
			return x * x;
	    	}

		/// <summary>
		/// Berechnet das Quadrat einer Zahl
		/// </summary>
		/// <param name="x">Zahl dessen Quadrat berechnet werden soll</param>
		/// <returns>x*x</returns>
		public static int Square(int x)
	    	{
			return x * x;
	    	}

		/// <summary>
		/// Prüft, ob value im Intervall [min, max] liegt
		/// </summary>
		/// <param name="value">zu prüfender Wert</param>
		/// <param name="min">Minimalwert</param>
		/// <param name="max">Maximalwert</param>
		/// <returns>value in [min, max]</returns>
	   	public static bool InBetween(double value, double min, double max)
	    	{
			return value >= min ? value <= max ? true : false : false;
		    }

		/// <summary>
		/// berechnet den Arkuscosiunus Hyperbolicus
		/// </summary>
		/// <param name="d">Eingabewert</param>
		/// <returns>acosh(d)</returns>
		public static double Acosh(double d)
			{
			return Math.Log(d + Math.Sqrt(d * d - 1));
			}

		/// <summary>
		/// Clamps value to [min, max]
		/// </summary>
		/// <param name="value">Value to clamp</param>
		/// <param name="min">Minimum value to return</param>
		/// <param name="max">Maximum value to return</param>
		/// <returns>min(max(value, min), max)</returns>
		public static double Clamp(double value, double min, double max)
			{
			return Math.Min(Math.Max(value, min), max);
			}
        }
    }
