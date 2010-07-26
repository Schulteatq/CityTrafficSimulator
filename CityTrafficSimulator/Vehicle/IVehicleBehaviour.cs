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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CityTrafficSimulator.Vehicle
	{
	/// <summary>
	/// abstrakte Klasse zur Kapselung des aktuellen Zustandes und Verhaltens eines IVehicles
	/// </summary>
	public abstract class IVehicleBehaviour
		{
		/// <summary>
		/// Art des VehicleBehaviour
		/// </summary>
		public enum BehaviourType
			{
			/// <summary>
			/// freies Fahren, durch nichts beeinflusst
			/// </summary>
			FREE_DRIVE,

			/// <summary>
			/// fährt einem anderen Fahrzeug hinterher
			/// </summary>
			FOLLOWING,

			/// <summary>
			/// nähert sich einer Intersection an
			/// </summary>
			APPROACHING_INTERSECTION,

			/// <summary>
			/// überquert eine Intersection
			/// </summary>
			CROSSING_INTERSECTION
			}

		/// <summary>
		/// Gibt die Art des VehicleBehaviour zurück
		/// </summary>
		/// <returns>Wert aus IVehicleBehaviour.BehaviourType</returns>
		public abstract BehaviourType GetBehaviourType();

		/// <summary>
		/// Gibt einen String mit Debuginformationen aus
		/// </summary>
		public abstract String GetDebugString();

		/// <summary>
		/// zeichnet Debuginformationen auf die Zeichenfläche g
		/// </summary>
		/// <param name="g">Zeichenfläche auf die gezeichnet werden soll</param>
		public virtual void DrawDebugGraphics(Graphics g)
			{
			return;
			}

		}
	}
