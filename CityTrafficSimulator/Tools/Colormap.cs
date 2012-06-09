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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CityTrafficSimulator.Tools
	{
	/// <summary>
	/// WindowsForms control for modifying a Colormap
	/// </summary>
	public class ColorMapControl : Control
		{
		#region Members

		/// <summary>
		/// Colormap object this control is handling.
		/// </summary>
		private ColorMap _colormap;
		/// <summary>
		/// Gets or sets the Colormap this control is handling.
		/// </summary>
		public ColorMap colormap
			{
			get { return _colormap; }
			set 
				{
				if (_colormap != null)
					_colormap.ColorsChanged -= _colormap_ColorsChanged; 
				_colormap = value;
				if (_colormap != null)
					_colormap.ColorsChanged += new ColorMap.ColorsChangedEventHandler(_colormap_ColorsChanged); 
				Invalidate();
				InvokeColorMapChanged(new ColorMapChangedEventArgs());
				}
			}


		/// <summary>
		/// Flag whether leftMouseButton is currently pressed
		/// </summary>
		private bool _leftMouseDown = false;
		/// <summary>
		/// Position of last MouseDown event.
		/// </summary>
		private Point _mouseDownPosition = new Point();
		/// <summary>
		/// Flag whether there is currently a color being dragged
		/// </summary>
		private bool _draggingColor = false;

		/// <summary>
		/// Temporary Colormap for painting during color moving action. MUST always be the same size as _colormap!
		/// </summary>
		private ColorMap _tempMap;
		#endregion

		#region Methods

		/// <summary>
		/// Default Constructor
		/// </summary>
		public ColorMapControl()
			{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			this.DoubleBuffered = true;

			InitializeComponent();

			List<Color> colors = new List<Color>();
			colors.Add(Color.Black);
			colors.Add(Color.White);
			_colormap = new ColorMap(colors);
			_colormap.ColorsChanged += new ColorMap.ColorsChangedEventHandler(_colormap_ColorsChanged);

			if (DesignMode)
				{
				this.Size = new System.Drawing.Size(96, 24);
				}
			}

		#endregion

		private void InitializeComponent()
			{
			this.SuspendLayout();
			// 
			// ColorMapControl
			// 
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorMapControl_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ColorMapControl_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColorMapControl_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ColorMapControl_MouseUp);
			this.ResumeLayout(false);
			}

		#region Eventhandlers

		/// <summary>
		/// schwarzer Pen
		/// </summary>
		private static Pen blackPen = new Pen(Color.Black, 1.0f);

		private void ColorMapControl_Paint(object sender, PaintEventArgs e)
			{
			e.Graphics.Clear(Color.White);

			if (_colormap != null)
				{
				if (_colormap.Count == 1)
					{
					e.Graphics.FillRectangle(new SolidBrush(_colormap[0]), ClientRectangle);
					}
				else
					{
					float stepsize = (float)ClientSize.Width / (float)(_colormap.Count - 1);
					for (int i = 0; i < _colormap.Count - 1; ++i)
						{
						ColorMap cm = (_draggingColor) ? _tempMap : _colormap;
						using (LinearGradientBrush gradBrush = new LinearGradientBrush(new PointF(0, 0), new PointF(stepsize, 0), cm[i], cm[i + 1]))
							{
							e.Graphics.FillRectangle(gradBrush, new RectangleF(stepsize * i, 0, stepsize * (i + 1), ClientSize.Height));
							}
						}
					}
				}
			else
				{
				using (Brush hatchBrush = new HatchBrush(HatchStyle.DiagonalCross, Color.Black, Color.White))
					{
					e.Graphics.FillRectangle(hatchBrush, ClientRectangle);
					}
				}

			e.Graphics.DrawRectangle(blackPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
			}

		private void ColorMapControl_MouseDown(object sender, MouseEventArgs e)
			{
			if (_colormap == null)
				return;

			_mouseDownPosition = e.Location;
			_leftMouseDown = (e.Button == MouseButtons.Left);
			}

		private void ColorMapControl_MouseUp(object sender, MouseEventArgs e)
			{
			_leftMouseDown = false;

			if (_colormap == null)
				return;

			// check if we have a click event
			if (Math.Abs(_mouseDownPosition.X - e.X) <= 2 && Math.Abs(_mouseDownPosition.Y - e.Y) <= 2)
				{
				int index = WorldToIndex(e.X);

				if (e.Button == MouseButtons.Left)
					{
					if (ClientSize.Width - e.X < 8)
						index = _colormap.Count;
					ColorDialog cd = new ColorDialog();
					cd.FullOpen = true;
					cd.AnyColor = true;
					if (cd.ShowDialog() == DialogResult.OK)
						{
						_colormap.AddColorAt(cd.Color, index);
						}
					}
				else if (e.Button == MouseButtons.Right)
					{
					if (_colormap.Count > 1)
						_colormap.RemoveColorAt(index);
					}
				}
			else if (_draggingColor)
				{
				_draggingColor = false;
				colormap = _tempMap;
				}
			}

		private void ColorMapControl_MouseMove(object sender, MouseEventArgs e)
			{
			if (_colormap == null)
				return;

			if (_leftMouseDown && (Math.Abs(_mouseDownPosition.X - e.X) > 2 || Math.Abs(_mouseDownPosition.Y - e.Y) > 2))
				{
				int origin = WorldToIndex(_mouseDownPosition.X);
				int destination = WorldToIndex(e.X);
				List<Color> l = _colormap.GetListCopy();

				Color c = l[origin];
				l[origin] = l[destination];
				l[destination] = c;

				_tempMap = new ColorMap(l);
				_draggingColor = true;
				Invalidate();
				}
			}

		void _colormap_ColorsChanged(object sender, ColorMap.ColorsChangedEventArgs e)
			{
			Invalidate();
			InvokeColorMapChanged(new ColorMapChangedEventArgs());
			}

		/// <summary>
		/// Converts a world X coordinate to the index based on the number of color in the current color map.
		/// </summary>
		/// <param name="worldX">World X coordinate</param>
		/// <returns>Corresponding color map index</returns>
		private int WorldToIndex(int worldX)
			{
			if (_colormap.Count <= 1)
				return 0;


			return Math2.Clamp((int)Math.Round(((float)(_colormap.Count - 1) * (float)worldX / (float)ClientSize.Width)), 0, _colormap.Count - 1);
			}

		#endregion

		#region Events

		#region ColorMapChanged event

		/// <summary>
		/// EventArgs for a ColorMapChanged event
		/// </summary>
		public class ColorMapChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new ColorMapChangedEventArgs
			/// </summary>
			public ColorMapChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the ColorMapChanged-EventHandler, which is called when the color map has changed.
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void ColorMapChangedEventHandler(object sender, ColorMapChangedEventArgs e);

		/// <summary>
		/// The ColorMapChanged event occurs when the color map has changed.
		/// </summary>
		public event ColorMapChangedEventHandler ColorMapChanged;

		/// <summary>
		/// Helper method to initiate the ColorMapChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void InvokeColorMapChanged(ColorMapChangedEventArgs e)
			{
			if (ColorMapChanged != null)
				{
				ColorMapChanged(this, e);
				}
			}

		#endregion

		#endregion

		}

	/// <summary>
	/// Wrapper for a list of colors offering interpolation methods.
	/// </summary>
	[Serializable]
	public class ColorMap
		{
		#region Members

		/// <summary>
		/// List of all colors in this color map.
		/// </summary>
		public List<SerializableColor> _colors;


		/// <summary>
		/// Returns the number of Colors in this color map
		/// </summary>
		public int Count { get { return _colors.Count; } }

		/// <summary>
		/// Returns the Color at index <paramref name="index"/>.
		/// </summary>
		/// <param name="index">Index of the color to return</param>
		/// <returns>_colors[index]</returns>
		public Color this[int index] { get { Debug.Assert(index >= 0 && index < _colors.Count); return _colors[index]; } }

		#endregion

		#region Methods

		/// <summary>
		/// Default Constructor
		/// ONLY for XML serialization, do not use unless you really know what you're doing.
		/// </summary>
		public ColorMap()
			{
			}

		/// <summary>
		/// Creates a new color map containing the colors in <paramref name="colors"/>.
		/// </summary>
		/// <param name="colors">List of colors for the color map. Must not be empty.</param>
		public ColorMap(List<Color> colors)
			{
			Debug.Assert(colors.Count > 0);
			_colors = new List<SerializableColor>(colors.Count);
			foreach (Color c in colors)
				{
				_colors.Add(c);
				}
			}

		/// <summary>
		/// Creates a new color map containing only the color <paramref name="c"/>
		/// </summary>
		/// <param name="c">The initial color of the color map</param>
		public ColorMap(Color c)
			{
			_colors = new List<SerializableColor>();
			_colors.Add(c);
			}

		/// <summary>
		/// Returns the color at the given index
		/// </summary>
		/// <param name="index">Index of the color. Must be in [0, Count)</param>
		/// <returns>_colors[index]</returns>
		public Color GetColorAt(int index)
			{
			Debug.Assert(index >= 0 && index < _colors.Count);
			return _colors[index];
			}

		/// <summary>
		/// Returns the color at the given position within interval [0,1]. If no such color is defined, linear interpolation between the surrounding colors will be performed.
		/// </summary>
		/// <param name="position">Position for the interpolated color. Must be within [0, 1].</param>
		/// <returns>The interpolated color at the given position.</returns>
		public Color GetInterpolatedColor(double position)
			{
			if (_colors.Count == 0)
				return Color.Black;

			if (position <= 0)
				return _colors[0];
			else if (position >= 1)
				return _colors[_colors.Count - 1];
			else
				{
				double cPos = position * (_colors.Count - 1);
				Color left = _colors[(int)Math.Floor(cPos)];
				Color right = _colors[(int)Math.Ceiling(cPos)];

				double alpha = (cPos - Math.Floor(cPos));
				double iAlpha = 1 - alpha;

				return Color.FromArgb(
					(int)(iAlpha * left.R + alpha * right.R),
					(int)(iAlpha * left.G + alpha * right.G),
					(int)(iAlpha * left.B + alpha * right.B));
				}
			}

		/// <summary>
		/// Returns a copy of the stored color list.
		/// </summary>
		/// <returns>A copy of the stored color list.</returns>
		public List<Color> GetListCopy()
			{
			List<Color> l = new List<Color>(_colors.Count);
			foreach (SerializableColor sc in _colors)
				{
				l.Add(sc);
				}
			return l;
			}

		/// <summary>
		/// Adds the color <paramref name="p"/> to the color map at index <paramref name="index"/>.
		/// </summary>
		/// <param name="c">The color to add</param>
		/// <param name="index">The index where to add the color</param>
		public void AddColorAt(Color c, int index)
			{
			Debug.Assert(index >= 0 && index <= _colors.Count);

			List<SerializableColor> newColors = new List<SerializableColor>(_colors.Count + 1);
			for (int i = 0; i < index; ++i)
				newColors.Add(_colors[i]);
			newColors.Add(c);
			for (int i = index; i < _colors.Count; ++i)
				newColors.Add(_colors[i]);

			_colors = newColors;
			InvokeColorsChanged(new ColorsChangedEventArgs());
			}

		/// <summary>
		/// Adds the color <paramref name="c"/> to the end of the color map.
		/// </summary>
		/// <param name="c">The color to add</param>
		public void AddColor(Color c)
			{
			AddColorAt(c, _colors.Count);
			}

		/// <summary>
		/// Removes the color at index <paramref name="index"/> from the color map.
		/// </summary>
		/// <param name="index">The index of the color to remove. Must be in [0, Count)</param>
		public void RemoveColorAt(int index)
			{
			Debug.Assert(index >= 0 && index < _colors.Count);

			List<SerializableColor> newColors = new List<SerializableColor>(_colors.Count);
			for (int i = 0; i < index; ++i)
				newColors.Add(_colors[i]);
			for (int i = index+1; i < _colors.Count; ++i)
				newColors.Add(_colors[i]);

			_colors = newColors;
			InvokeColorsChanged(new ColorsChangedEventArgs());
			}

		#endregion

		#region Events

		#region ColorsChanged event

		/// <summary>
		/// EventArgs for a ColorsChanged event
		/// </summary>
		public class ColorsChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new ColorsChangedEventArgs
			/// </summary>
			public ColorsChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the ColorsChanged-EventHandler, which is called when the color list of this color map has changed
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void ColorsChangedEventHandler(object sender, ColorsChangedEventArgs e);

		/// <summary>
		/// The ColorsChanged event occurs when the color list of this color map has changed
		/// </summary>
		public event ColorsChangedEventHandler ColorsChanged;

		/// <summary>
		/// Helper method to initiate the ColorsChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void InvokeColorsChanged(ColorsChangedEventArgs e)
			{
			if (ColorsChanged != null)
				{
				ColorsChanged(this, e);
				}
			}

		#endregion

		#endregion

		}
	}
