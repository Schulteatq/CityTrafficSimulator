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

using CityTrafficSimulator.Vehicle;

namespace CityTrafficSimulator
	{
	/// <summary>
	/// kapselt zwei sich an einer Intersection gegenseitig behindernde Fahrzeuge
	/// </summary>
	public struct InterferingVehicle
		{

		#region Felder

		/// <summary>
		/// ausgebremstes Fahrzeug
		/// </summary>
		public IVehicle jammedVehicle;

		/// <summary>
		/// ausbremsendes Fahrzeug
		/// </summary>
		public IVehicle jammingVehicle;

		/// <summary>
		/// Intersection an der was passieren kann
		/// </summary>
		public Intersection intersection;
		
		/// <summary>
		/// Bremsverzögerung des ausgebremsten Fahrzeuges nach MOBIL
		/// </summary>
		public double forcedAccelerationOfJammedVehicle;

		/// <summary>
		/// Zeit die das behindernde Fahrzeug bis zum Erreichen der Kreuzung benötigt
		/// </summary>
		public double timeOfJammingVehicleToReachIntersection;

		/// <summary>
		/// Zeit die das behindernde Fahrzeug bis zum Verlassen der Kreuzung benötigt
		/// </summary>
		public double timeOfJammingVehicleToLeaveIntersection;

		/// <summary>
		/// Zeit die das behinderte Fahrzeug bis zum Erreichen der Kreuzung benötigt
		/// </summary>
		public double timeOfJammedVehicleToReachIntersection;

		/// <summary>
		/// Zeit die das behinderte Fahrzeug urspünglich zum Erreichen der Intersection benötigt hätte
		/// </summary>
		public double originalTimeOfJammedVehicleToReachIntersecion;

		/// <summary>
		/// Zeit die das behindernde Fahrzeug urspünglich zum Erreichen der Intersection benötigt hätte
		/// </summary>
		public double originalTimeOfJammingVehicleToReachIntersecion;

		#endregion

		/// <summary>
		/// Standardkonstruktor
		/// </summary>
		/// <param name="jammedVehicle">behindertes Fahrzeug</param>
		/// <param name="jammingVehicle">behinderndes Fahrzeug</param>
		/// <param name="intersection">betrachtete Intersection</param>
		/// <param name="forcedAccelerationOfJammedVehicle">Bremsverzögerung des ausgebremsten Fahrzeuges nach MOBIL</param>
		/// <param name="timeOfJammedVehicleToReachIntersection">Zeit die das behindernde Fahrzeug bis zum Erreichen der Kreuzung benötigt</param>
		/// <param name="timeOfJammingVehicleToLeaveIntersection">Zeit die das behindernde Fahrzeug bis zum Verlassen der Kreuzung benötigt</param>
		/// <param name="timeOfJammingVehicleToReachIntersection">Zeit die das behinderte Fahrzeug bis zum Erreichen der Kreuzung benötigt</param>
		/// <param name="originalTimeOfJammingVehicleToReachIntersecion">Zeit die das behindernde Fahrzeug urspünglich zum Erreichen der Intersection benötigt hätte </param>
		/// <param name="originalTimeOfJammedVehicleToReachIntersecion">Zeit die das behinderte Fahrzeug urspünglich zum Erreichen der Intersection benötigt hätte</param>
		public InterferingVehicle(IVehicle jammedVehicle, IVehicle jammingVehicle, Intersection intersection, double forcedAccelerationOfJammedVehicle,
			double timeOfJammingVehicleToReachIntersection, double timeOfJammingVehicleToLeaveIntersection, double timeOfJammedVehicleToReachIntersection,
			double originalTimeOfJammingVehicleToReachIntersecion, double originalTimeOfJammedVehicleToReachIntersecion
			)
			{
			this.jammedVehicle = jammedVehicle;
			this.jammingVehicle = jammingVehicle;
			this.intersection = intersection;
			this.forcedAccelerationOfJammedVehicle = forcedAccelerationOfJammedVehicle;
			this.timeOfJammingVehicleToReachIntersection = timeOfJammingVehicleToReachIntersection;
			this.timeOfJammingVehicleToLeaveIntersection = timeOfJammingVehicleToLeaveIntersection;
			this.timeOfJammedVehicleToReachIntersection = timeOfJammedVehicleToReachIntersection;
			this.originalTimeOfJammingVehicleToReachIntersecion = originalTimeOfJammingVehicleToReachIntersecion;
			this.originalTimeOfJammedVehicleToReachIntersecion = originalTimeOfJammedVehicleToReachIntersecion;
			}
		}
	}


