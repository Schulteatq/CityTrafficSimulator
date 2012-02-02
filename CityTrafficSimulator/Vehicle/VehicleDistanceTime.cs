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

namespace CityTrafficSimulator.Vehicle
	{
	/// <summary>
	/// kapselt ein Fahrzeug mit zugehörigen Distanz- und Zeitparametern
	/// </summary>
	public struct VehicleDistanceTime
		{
		/// <summary>
		/// Fahrzeug 
		/// </summary>
		public IVehicle vehicle;
		/// <summary>
		/// Distanz 
		/// </summary>
		public double distance;
		/// <summary>
		/// Zeit
		/// </summary>
		public double time;


		/// <summary>
		/// Standardkonstruktor
		/// </summary>
		/// <param name="vehicle">Fahrzeug, welches gekapselt wird</param>
		/// <param name="distance">Distanz, die gekapselt wird</param>
		/// <param name="time">Zeit, die gekapselt wird</param>
		public VehicleDistanceTime(IVehicle vehicle, double distance, double time)
			{
			this.distance = distance;
			this.vehicle = vehicle;
			this.time = time;
			}
		}

	}
