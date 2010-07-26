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
	/// Implementierung eines IVehicleBehaviour, welches freie Fahrt, die durch nichts behindert wird modelliert
	/// </summary>
	class FreeDriveVehicleBehaviour : IVehicleBehaviour
		{

		/// <summary>
		/// Standardkonstruktor
		/// </summary>
		public FreeDriveVehicleBehaviour()
			{

			}

		/// <summary>
		/// Gibt die Art des VehicleBehaviour zurück
		/// </summary>
		/// <returns>Wert aus IVehicleBehaviour.BehaviourType</returns>
		public override IVehicleBehaviour.BehaviourType GetBehaviourType()
			{
			return BehaviourType.FREE_DRIVE;
			}

		/// <summary>
		/// Gibt einen String mit Debuginformationen zurück
		/// </summary>
		/// <returns>Following Vehicle #... @...</returns>
		public override string GetDebugString()
			{
			return String.Empty;
			}

		}
	}
