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
	/// Abstrakte EventManager Klasse zur Realisierung des Observer Verhaltensmusters. 
	/// EventListener können sich hier registrieren und über NotifyListeners() aufgerufen werden.
	/// </summary>
	public abstract class EventManager
		{

		/// <summary>
		/// Liste aller angemeldeten EventListener
		/// </summary>
		private List<IEventListener> eventListeners = new List<IEventListener>();

		/// <summary>
		/// registriert den EventListener ev bei diesem EventManager
		/// </summary>
		/// <param name="ev">zu registrierender EventListener</param>
		public void RegisterEventListener(IEventListener ev)
			{
			eventListeners.Add(ev);
			}

		/// <summary>
		/// macht die Registrierung des EventListeners ev bei diesem EventManager rückgängig
		/// </summary>
		/// <param name="ev">zu unregistrierender EventListener</param>
		/// <returns>true, falls das unregistrierung erfolgreich war</returns>
		public bool UnregisterEventListener(IEventListener ev)
			{
			return eventListeners.Remove(ev);
			}

		/// <summary>
		/// benachrichtigt alle angemeldeten EventListener
		/// </summary>
		public void NotifyListeners()
			{
			foreach (IEventListener el in eventListeners)
				{
				el.Notify();
				}
			}
		}
	}
