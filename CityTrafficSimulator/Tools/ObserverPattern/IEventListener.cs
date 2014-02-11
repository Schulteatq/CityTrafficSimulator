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

namespace CityTrafficSimulator.Tools.ObserverPattern
	{
	/// <summary>
	/// EventListener Interface zur Realisierung des Observer Verhaltensmusters. 
	/// Kann bei einem EventManager angemeldet werden und wird von diesem beizeiten aufgerufen.
	/// </summary>
	public interface IEventListener
		{

		/// <summary>
		/// Methode, die vom EventManager.NotifyListeners() aufgerufen werden soll.
		/// </summary>
		void Notify();

		}
	}
