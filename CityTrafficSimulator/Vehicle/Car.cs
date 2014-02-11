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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CityTrafficSimulator.Vehicle
    {
    class Car : IVehicle
        {
        public Car(IVehicle.Physics p)
            {
			length = GlobalRandom.Instance.Next(28, 45);

			_physics = p;

			// etwas Zufall:
			a *= (GlobalRandom.Instance.NextDouble() + 0.5);
			b *= (GlobalRandom.Instance.NextDouble() + 0.5);
			s0 *= (GlobalRandom.Instance.NextDouble() + 0.5);
			T *= (GlobalRandom.Instance.NextDouble() + 0.5);

			_physics.targetVelocity += ((GlobalRandom.Instance.NextDouble() - 0.5) * 4);

			color = Color.FromArgb(GlobalRandom.Instance.Next(256), GlobalRandom.Instance.Next(256), GlobalRandom.Instance.Next(256));
			_vehicleType = VehicleTypes.CAR;
			}
        
		/// <summary>
		/// Prüfe, ob ich auf der NodeConnection nc fahren darf
		/// </summary>
		/// <param name="nc">zu prüfende NodeConnection</param>
		/// <returns>nc.carsAllowed</returns>
		public override bool CheckNodeConnectionForSuitability(NodeConnection nc)
			{
			return nc.carsAllowed;
			}
        }
    }
