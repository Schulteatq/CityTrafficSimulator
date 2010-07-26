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

namespace CityTrafficSimulator.Vehicle
	{
	/// <summary>
	/// kapselt ein Vehicle und Distanz.
	/// TODO: sollte eigentlich ein struct sein, wird aber z.T. mit null initialisert, daher noch eine class... das ist unschön
	/// </summary>
	public class VehicleDistance
		{
		/// <summary>
		/// Fahrzeug mit Distanz distance
		/// </summary>
		public IVehicle vehicle;
		/// <summary>
		/// Distanz zum Fahrzeug vehicle
		/// </summary>
		public double distance;


		/// <summary>
		/// Standardkonstruktor
		/// </summary>
		/// <param name="vehicle">Fahrzeug, welches gekapselt wird</param>
		/// <param name="distance">Distanz, die gekapselt wird</param>
		public VehicleDistance(IVehicle vehicle, double distance)
			{
			this.distance = distance;
			this.vehicle = vehicle;
			}
		}
	}
