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
using System.Drawing;

namespace CityTrafficSimulator
    {
	/// <summary>
	/// Wrapperklasse einer LinkedList, welche einen Standardaccessor hat, damit die Liste serialisiert werden kann
	/// </summary>
	/// <typeparam name="T">Typ der Elemente, die in der LinkedListe gespeichert werden sollen</typeparam>
    [Serializable]
    public class MyLinkedList<T> : LinkedList<T>
        {
		/// <summary>
		/// Standardaccessor der LinkedList
		/// Gibt oder Setzt das Element Nr. j von vorn
		/// </summary>
		/// <param name="j">Elementnummer, auf das zugegriffen werden soll</param>
		/// <returns></returns>
        public T this[int j]
            {
            get 
                {
                LinkedListNode<T> toreturn = this.First;
                for (int i = 1; i <= j; i++)
                    {
					if (toreturn == null)
						{
						throw new IndexOutOfRangeException();
						}
                    toreturn = toreturn.Next;
                    }
                return toreturn.Value;
                }
            set 
                {
                LinkedListNode<T> toreturn = this.First;
                for (int i = 1; i <= j; i++)
                    {
                    toreturn = toreturn.Next;
					if (toreturn == null)
						{
						throw new IndexOutOfRangeException();
						}
					}
                toreturn.Value = value;
                }
            }

		/// <summary>
		/// fügt das Element toadd am Ende der verketteten Liste an
		/// </summary>
		/// <param name="toadd">hinzuzufügendes Element</param>
        public void Add(T toadd)
            {
            this.AddLast(toadd);
            }
        }
    }
