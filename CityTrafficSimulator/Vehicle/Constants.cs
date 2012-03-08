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
	/// statische Klasse, die lediglich einige wichtige Konstanten rund um Vehicles global speichert
	/// </summary>
	internal static class Constants
		{
		/// <summary>
		/// Zeitspanne in Sekunden, die in vorausgerechnet werden soll
		/// </summary>
		public const double lookaheadTime = 16;

		/// <summary>
		/// Entfernung, die vorausgerechnet werden soll
		/// </summary>
		public const double lookaheadDistance = 768;

		/// <summary>
		/// maximale Entfernung, die ein LineChangePoint entfernt sein darf, damit er benutzt wird
		/// </summary>
		public const double maxDistanceToLineChangePoint = 28;


		/// <summary>
		/// maximale Entfernung zwischen zwei parallelen NodeConnections, für einen LineChangePoint
		/// </summary>
		public const double maxDistanceToParallelConnection = 48;

		/// <summary>
		/// maximaler Winkel zwischen zwei NodeConnections, damit sie für LineChangePoints als parallel angesehen werden
		/// </summary>
		public const double maximumAngleBetweenConnectionsForLineChangePoint = Math.PI / 8;

		/// <summary>
		/// Standardkapazität der LineChangeInterval-Dictionaries
		/// </summary>
		public const int defaultLineChangeIntervalDictionaryCapacity = 16;


		/// <summary>
		/// pauschale Kosten bei der Wegberechnung für einen Spurwechsel
		/// </summary>
		public const double lineChangePenalty = 1536;

		/// <summary>
		/// pauschale Kosten bei der Wegberechnung für einen Spurwechsel direkt vor einer Ampel
		/// </summary>
		public const double lineChangeBeforeTrafficLightPenalty = lineChangePenalty * 4;


		/// <summary>
		/// maximales Verhältnis Wegkosten/Wegkosten nach Spurwechsel, bis zu dem noch ein freiwilliger Spurwechsel durchgeführt werden soll
		/// </summary>
		public const double maxRatioForVoluntaryLineChange = 1.25;

		/// <summary>
		/// maximales Verhältnis Wegkoste nnach Spurwechsel/Wegkosten ohne Spurwechsel, bis zu dem auf ein einen eigentlich zwangsweisen Spurwechsel verzichtet werden soll
		/// </summary>
		public const double maxRatioForEnforcedLineChange = 2;


		/// <summary>
		/// pauschale Kosten bei der Wegberechnung für Fahrzeug was auf dem geplanten Weg fährt
		/// </summary>
		public const double vehicleOnRoutePenalty = 48;

		/// <summary>
		/// minimale Länge eines LineChangeIntervals, damit es für die Wegberechnung berücksichtigt wird
		/// </summary>
		public const double minimumLineChangeLength = 192;

		/// <summary>
		/// distance to end of LineChangeInterval where the vehicle shal stop in case of a forced line change
		/// </summary>
		public const double breakPointBeforeForcedLineChange = 48;

		/// <summary>
		/// lookahead distance for intersections
		/// </summary>
		public const double intersectionLookaheadDistance = 384;
		}
	}
