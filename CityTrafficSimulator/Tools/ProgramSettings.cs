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

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CityTrafficSimulator.Tools
	{
	/// <summary>
	/// Record of all program settings which should be saved together with the network
	/// </summary>
	[Serializable]
	public struct ProgramSettings
		{
		/// <summary>
		/// Simulation Speed
		/// </summary>
		public decimal _simSpeed;

		/// <summary>
		/// Simulation Steps/s
		/// </summary>
		public decimal _simSteps;

		/// <summary>
		/// Simulation Duration
		/// </summary>
		public decimal _simDuration;

		/// <summary>
		/// Random Seed for Simulation
		/// </summary>
		public decimal _simRandomSeed;

		/// <summary>
		/// Selected Item im Zoom ComboBox
		/// </summary>
		public int _zoomLevel;

		/// <summary>
		/// Selected Item in Render Quality ComboBox
		/// </summary>
		public int _renderQuality;

		/// <summary>
		/// Flag whether to render NodeConnection statistics
		/// </summary>
		public bool _renderStatistics;

		/// <summary>
		/// Flag whether to render vehicle velocity mapping
		/// </summary>
		public bool _renderVelocityMapping;

		/// <summary>
		/// Flag whether to show FPS
		/// </summary>
		public bool _showFPS;

		/// <summary>
		/// Render Options for Main Canvas
		/// </summary>
		public NodeSteuerung.RenderOptions _renderOptions;

		/// <summary>
		/// ColorMap for Velocity Mapping
		/// </summary>
		public Tools.ColorMap _velocityMappingColorMap;

		}
	}
