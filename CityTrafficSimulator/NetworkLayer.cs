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
using System.Xml.Serialization;
using System.Text;

namespace CityTrafficSimulator
	{
	/// <summary>
	/// Traffic network layer
	/// </summary>
	[Serializable]
	public class NetworkLayer
		{
		/// <summary>
		/// Title of this layer
		/// </summary>
		private string _title;
		/// <summary>
		/// Title of this layer
		/// </summary>
		public string title
			{
			get { return _title; }
			set { _title = value; InvokeTitleChanged(new TitleChangedEventArgs()); }
			}

		/// <summary>
		/// Flag whether this layer shall be rendered
		/// </summary>
		private bool _visible;
		/// <summary>
		/// Flag whether this layer shall be rendered
		/// </summary>
		public bool visible
			{
			get { return _visible; }
			set { _visible = value; InvokeVisibleChanged(new VisibleChangedEventArgs()); }
			}

		/// <summary>
		/// Hashcodes of all LineNodes on this NetworkLayer (only used for XML serialization)
		/// </summary>
		public List<int> _nodeHashes = new List<int>();


		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="title">Title of this layer</param>
		/// <param name="visible">Flag whether this layer shall be rendered</param>
		public NetworkLayer(string title, bool visible)
			{
			_title = title;
			_visible = visible;
			}

		/// <summary>
		/// Empty default constructor ONLY for (de)serialization. DO NOT USE unless you know what you're doing!
		/// </summary>
		public NetworkLayer()
			{
			}

		/// <summary>
		/// Returns a string representation of this NetworkLayer.
		/// </summary>
		/// <returns>_title</returns>
		public override string ToString()
			{
			return _title;
			}

		#region TitleChanged event

		/// <summary>
		/// EventArgs for a TitleChanged event
		/// </summary>
		public class TitleChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new TitleChangedEventArgs
			/// </summary>
			public TitleChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the TitleChanged-EventHandler, which is called when the title has changed
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void TitleChangedEventHandler(object sender, TitleChangedEventArgs e);

		/// <summary>
		/// The TitleChanged event occurs when the title has changed
		/// </summary>
		public event TitleChangedEventHandler TitleChanged;

		/// <summary>
		/// Helper method to initiate the TitleChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void InvokeTitleChanged(TitleChangedEventArgs e)
			{
			if (TitleChanged != null)
				{
				TitleChanged(this, e);
				}
			}

		#endregion

		#region VisibleChanged event

		/// <summary>
		/// EventArgs for a VisibleChanged event
		/// </summary>
		public class VisibleChangedEventArgs : EventArgs
			{
			/// <summary>
			/// Creates new VisibleChangedEventArgs
			/// </summary>
			public VisibleChangedEventArgs()
				{
				}
			}

		/// <summary>
		/// Delegate for the VisibleChanged-EventHandler, which is called when the visibility flag has changed
		/// </summary>
		/// <param name="sender">Sneder of the event</param>
		/// <param name="e">Event parameter</param>
		public delegate void VisibleChangedEventHandler(object sender, VisibleChangedEventArgs e);

		/// <summary>
		/// The VisibleChanged event occurs when the visibility flag has changed
		/// </summary>
		public event VisibleChangedEventHandler VisibleChanged;

		/// <summary>
		/// Helper method to initiate the VisibleChanged event
		/// </summary>
		/// <param name="e">Event parameters</param>
		protected void InvokeVisibleChanged(VisibleChangedEventArgs e)
			{
			if (VisibleChanged != null)
				{
				VisibleChanged(this, e);
				}
			}

		#endregion
		}
	}
