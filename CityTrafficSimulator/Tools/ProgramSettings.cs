using System;
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
