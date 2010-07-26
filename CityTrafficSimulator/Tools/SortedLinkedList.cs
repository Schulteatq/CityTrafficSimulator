/*
 *  CityTrafficSimulator - a tool to simulate traffic in urban areas and on intersections
 *  Copyright (C) 2005-2010, Christian Schulte zu Berge
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

namespace CityTrafficSimulator.Tools
	{
	/// <summary>
	/// Wrapperklasse einer LinkedList, welche eine Vergleichsfunktion besitzt und so die beherbergten Elemente automatisch sortiert hält.
	/// </summary>
	/// <typeparam name="T">Typ der Elemente, die in der LinkedListe gespeichert werden sollen</typeparam>
	public class SortedLinkedList<T> : LinkedList<T>
		{
		/// <summary>
		/// Delegatedefinition für Vergleichsfunktion
		/// </summary>
		/// <param name="a">erster zu vergleichender Wert</param>
		/// <param name="b">zweiter zu vergleichender Wert</param>
		/// <returns>-1 falls a echt kleiner b, 0 falls a=b, 1 falls a echt größer b</returns>
		public delegate int CompareDelegate(T a, T b);

		/// <summary>
		/// Vergleichsfunktion
		/// </summary>
		private CompareDelegate m_comparer;
		/// <summary>
		/// Vergleichsfunktion
		/// </summary>
		public CompareDelegate comparer
			{
			get { return m_comparer; }
			set { m_comparer = value; }
			}

		/// <summary>
		/// Standardaccessor der LinkedList
		/// Gibt oder Setzt das Element Nr. j von vorn
		/// </summary>
		/// <param name="j"></param>
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
		/// Standardkonstruktor, der einen Comparer entgegennimmt
		/// </summary>
		/// <param name="comparer">Vergleichsfunktion</param>
		public SortedLinkedList(CompareDelegate comparer)
			{
			this.comparer = comparer;
			}

		/// <summary>
		/// leerer Standardkonstruktor - nicht implementiert!
		/// </summary>
		public SortedLinkedList()
			{
			throw new NotImplementedException();
			}

		/// <summary>
		/// fügt toadd der SortedLinkedList hinzu und sorgt dafür, dass die Liste sortiert bleibt
		/// </summary>
		/// <param name="toadd">hinzuzufügendes Element</param>
		/// <returns>LinkedListNode, der das hinzugefügte Element enthält</returns>
		public LinkedListNode<T> Add(T toadd)
			{
			LinkedListNode<T> lln = this.First;

			while (lln != null && comparer(toadd, lln.Value) == 1)
				{
				lln = lln.Next;
				}

			if (lln != null)
				{
				if (comparer(toadd, lln.Value) == 0)
					return lln;
				else
					return this.AddBefore(lln, toadd);
				}
			else
				return this.AddLast(toadd);
			}
		}
	}
