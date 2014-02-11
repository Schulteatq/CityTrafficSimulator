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

/**
 *  CityTrafficSimulator - a tool to simulate traffic in urban areas and on intersections
 *  Copyright (C) 2009, Christian Schulte zu Berge
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
//using System.Collections;

namespace CityTrafficSimulator
{
	/// <summary>
	/// Interface für eine Prioritätswarteschlange mit Elementen vom Typ T
	/// </summary>
	/// <typeparam name="T">Typ der Elemente in der Prioritätswarteschlange</typeparam>
	public interface IPriorityQueue<T> : ICollection<T>, ICloneable, IList<T>
	{
		/// <summary>
		/// fügt ein Element hinzu
		/// </summary>
		/// <param name="O">hinzuzufügendes Element</param>
		/// <returns></returns>
		int Push(T O);
		T Pop();
		T Peek();
		void Update(int i);
	}

	public class BinaryPriorityQueue<T> : IPriorityQueue<T>, ICollection<T>, ICloneable, IList<T>
	{
		protected List<T> InnerList = new List<T>();
		protected IComparer<T> Comparer;

		#region contructors
		public BinaryPriorityQueue() : this(System.Collections.Generic.Comparer<T>.Default)
		{}
		public BinaryPriorityQueue(IComparer<T> c)
		{
			Comparer = c;
		}
        public BinaryPriorityQueue(int C)
            : this(System.Collections.Generic.Comparer<T>.Default, C)
		{}
		public BinaryPriorityQueue(IComparer<T> c, int Capacity)
		{
			Comparer = c;
			InnerList.Capacity = Capacity;
		}

		protected BinaryPriorityQueue(List<T> Core, IComparer<T> Comp, bool Copy)
		{
        if (Copy)
            InnerList = Core;
        else
            InnerList = Core;
			Comparer = Comp;
		}

		#endregion
		protected void SwitchElements(int i, int j)
		{
			T h = InnerList[i];
			InnerList[i] = InnerList[j];
			InnerList[j] = h;
		}

		protected virtual int OnCompare(int i, int j)
		{
			return Comparer.Compare(InnerList[i],InnerList[j]);
		}

		#region public methods
		/// <summary>
		/// Push an object onto the PQ
		/// </summary>
		/// <param name="O">The new object</param>
		/// <returns>The index in the list where the object is _now_. This will change when objects are taken from or put onto the PQ.</returns>
		public int Push(T O)
		{
			int p = InnerList.Count,p2;
			InnerList.Add(O); // E[p] = O
			do
			{
				if(p==0)
					break;
				p2 = (p-1)/2;
				if(OnCompare(p,p2)<0)
				{
					SwitchElements(p,p2);
					p = p2;
				}
				else
					break;
			}while(true);
			return p;
		}

		/// <summary>
		/// Get the smallest object and remove it.
		/// </summary>
		/// <returns>The smallest object</returns>
		public T Pop()
		{
			T result = InnerList[0];
			int p = 0,p1,p2,pn;
			InnerList[0] = InnerList[InnerList.Count-1];
			InnerList.RemoveAt(InnerList.Count-1);
			do
			{
				pn = p;
				p1 = 2*p+1;
				p2 = 2*p+2;
				if(InnerList.Count>p1 && OnCompare(p,p1)>0) // links kleiner
					p = p1;
				if(InnerList.Count>p2 && OnCompare(p,p2)>0) // rechts noch kleiner
					p = p2;
				
				if(p==pn)
					break;
				SwitchElements(p,pn);
			}while(true);
			return result;
		}

		/// <summary>
		/// Notify the PQ that the object at position i has changed
		/// and the PQ needs to restore order.
		/// Since you dont have access to any indexes (except by using the
		/// explicit IList.this) you should not call this function without knowing exactly
		/// what you do.
		/// </summary>
		/// <param name="i">The index of the changed object.</param>
		public void Update(int i)
		{
			int p = i,pn;
			int p1,p2;
			do	// aufsteigen
			{
				if(p==0)
					break;
				p2 = (p-1)/2;
				if(OnCompare(p,p2)<0)
				{
					SwitchElements(p,p2);
					p = p2;
				}
				else
					break;
			}while(true);
			if(p<i)
				return;
			do	   // absteigen
			{
				pn = p;
				p1 = 2*p+1;
				p2 = 2*p+2;
				if(InnerList.Count>p1 && OnCompare(p,p1)>0) // links kleiner
					p = p1;
				if(InnerList.Count>p2 && OnCompare(p,p2)>0) // rechts noch kleiner
					p = p2;
				
				if(p==pn)
					break;
				SwitchElements(p,pn);
			}while(true);
		}

		/// <summary>
		/// Get the smallest object without removing it.
		/// </summary>
		/// <returns>The smallest object</returns>
		public T Peek()
		{
			if(InnerList.Count>0)
				return InnerList[0];
            return default(T);
		}

		public bool Contains(T value)
		{
			return InnerList.Contains(value);
		}

		public void Clear()
		{
			InnerList.Clear();
		}

		public int Count
		{
			get
			{
				return InnerList.Count;
			}
		}

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}

        public void CopyTo(T[] array, int index)
		    {
			    InnerList.CopyTo(array,index);
		    }

            public object Clone()
		    {
			    return new BinaryPriorityQueue<T>(InnerList,Comparer,true);	
		    }

		public BinaryPriorityQueue<T> SyncRoot
		{
			get
			{
				return this;
			}
		}
		#endregion
		#region explicit implementation
        bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

        T IList<T>.this[int index]
		{
			get
			{
				return InnerList[index];
			}
			set
			{
				InnerList[index] = value;
				Update(index);
			}
		}

        
        void ICollection<T>.Add(T o)
		{
			Push(o);
		}

        void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

        void IList<T>.Insert(int index, T value)
		{
			throw new NotSupportedException();
		}

        bool ICollection<T>.Remove(T value)
		{
			throw new NotSupportedException();
		}

        int IList<T>.IndexOf(T value)
		{
			throw new NotSupportedException();
		}
        /*
        bool IList<T>.IsFixedSize
		{
			get
			{
				return false;
			}
		}*/
        bool ICollection<T>.Contains(T item)
		{
			throw new NotSupportedException();
		}

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		#endregion



    #region IEnumerable Member

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
        throw new Exception("The method or operation is not implemented.");
        }

    #endregion

    }
}
