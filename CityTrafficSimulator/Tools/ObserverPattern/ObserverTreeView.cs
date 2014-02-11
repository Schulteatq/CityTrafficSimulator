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
using System.Windows.Forms;

using CityTrafficSimulator.Tools.ObserverPattern;


namespace CityTrafficSimulator.Tools.ObserverPattern
	{
	/// <summary>
	/// TreeView-Control, welches als Observer fungiert und sich so bei einem EventManager anmelden kann.
	/// </summary>
	class ObserverTreeView : TreeView, IEventListener
		{
		/// <summary>
		/// Notify-Funktion, die vom EventManager aufgerufen wird.
		/// </summary>
		public virtual void Notify()
			{
			
			}
		}
	}
