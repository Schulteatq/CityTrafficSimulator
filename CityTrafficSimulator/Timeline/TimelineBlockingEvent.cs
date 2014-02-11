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
using System.Drawing;

namespace CityTrafficSimulator.Timeline
	{
	/// <summary>
	/// TimelineEvent for blocking the interim times of the parent TimelineEvent on this TimelineEntry
	/// </summary>
	public class TimelineBlockingEvent : TimelineEvent
		{

		#region Members and Properties

		/// <summary>
		/// The parent TimelineEvent this event is blocking for
		/// </summary>
		private TimelineEvent _parentEvent;
		/// <summary>
		/// The parent TimelineEvent this event is blocking for
		/// </summary>
		public TimelineEvent parentEvent
			{
			get { return _parentEvent; }
			set { _parentEvent = value; }
			}

		/// <summary>
		/// Interim time (offset) between parent event and this blocking event
		/// </summary>
		private double _interimTime;
		/// <summary>
		/// Interim time (offset) between parent event and this blocking event
		/// </summary>
		public double interimTime
			{
			get { return _interimTime; }
			set { _interimTime = value; }
			}

		/// <summary>
		/// Additional buffer time before event
		/// </summary>
		private double _bufferTimeBefore;
		/// <summary>
		/// Additional buffer time before event
		/// </summary>
		public double bufferTimeBefore
			{
			get { return _bufferTimeBefore; }
			set { _bufferTimeBefore = value; }
			}

		/// <summary>
		/// Additional buffer time after event
		/// </summary>
		private double _bufferTimeAfter;
		/// <summary>
		/// Additional buffer time after event
		/// </summary>
		public double bufferTimeAfter
			{
			get { return _bufferTimeAfter; }
			set { _bufferTimeAfter = value; }
			}


		#endregion

		#region Methods

		/// <summary>
		/// Creates a new TimelineBlockingEvent.
		/// </summary>
		/// <param name="parentEvent">The parent TimelineEvent this event is blocking for</param>
		/// <param name="interimTime">Interim time (offset) between parent event and this blocking event</param>
		/// <param name="bufferTimeBefore">Additional buffer time before event</param>
		/// <param name="bufferTimeAfter">Additional buffer time after event</param>
		public TimelineBlockingEvent(TimelineEvent parentEvent, double interimTime, double bufferTimeBefore, double bufferTimeAfter)
			: base(
				parentEvent.eventTime + interimTime - bufferTimeBefore, 
				parentEvent.eventLength + bufferTimeBefore + bufferTimeAfter, 
				Color.DarkGray,
				delegate() {},
				delegate() {})
			{
			this.parentEvent = parentEvent;
			this.interimTime = interimTime;
			this.bufferTimeBefore = bufferTimeBefore;
			this.bufferTimeAfter = bufferTimeAfter;

			parentEvent.EventTimesChanged += new EventTimesChangedEventHandler(parentEvent_EventTimesChanged);
			}

		/// <summary>
		/// Adjusts the time of this event to the timing of the parent event
		/// </summary>
		protected void adjustEventTimesToParentEvent()
			{
			eventTime = parentEvent.eventTime + interimTime - bufferTimeBefore;
			eventLength = parentEvent.eventLength + bufferTimeBefore + bufferTimeAfter;
			}

		#endregion

		#region EventHandlers

		void parentEvent_EventTimesChanged(object sender, TimelineEvent.EventTimesChangedEventArgs e)
			{
			adjustEventTimesToParentEvent();
			}

		#endregion
		}
	}
