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
	/// Implementierung eines IVehicleBehaviour, welches modelliert, dass das Fahrzeug einem anderen Fahrzeug folgt
	/// </summary>
	class FollowingVehicleBehaviour : IVehicleBehaviour
		{
		#region Felder

		/// <summary>
		/// VehicleDistance-Instanz, welches auf das vorausfahrende Fahrzeug verweist
		/// </summary>
		private VehicleDistance m_previousVehicleDistance;
		/// <summary>
		/// VehicleDistance-Instanz, welches auf das vorausfahrende Fahrzeug verweist
		/// </summary>
		public VehicleDistance previousVehicleDistance
			{
			get { return m_previousVehicleDistance; }
			set { m_previousVehicleDistance = value; }
			}

		#endregion

		#region Konstruktoren

		/// <summary>
		/// Standardkonstruktor
		/// </summary>
		/// <param name="previousVehicleDistance">VehicleDistance-Instanz, welches auf das vorausfahrende Fahrzeug verweist</param>
		public FollowingVehicleBehaviour(VehicleDistance previousVehicleDistance)
			{
			this.m_previousVehicleDistance = previousVehicleDistance;
			}

		#endregion


		#region IVehicleBehaviour-Member

		/// <summary>
		/// Gibt die Art des VehicleBehaviour zurück
		/// </summary>
		/// <returns>Wert aus IVehicleBehaviour.BehaviourType</returns>
		public override IVehicleBehaviour.BehaviourType GetBehaviourType()
			{
			return BehaviourType.FOLLOWING;
			}

		/// <summary>
		/// Gibt einen String mit Debuginformationen zurück
		/// </summary>
		/// <returns>Following Vehicle #... @...</returns>
		public override string GetDebugString()
			{
			return "Following Vehicle #" + previousVehicleDistance.vehicle.GetHashCode() + " @" + previousVehicleDistance.distance;
			}

		#endregion


		}
	}
