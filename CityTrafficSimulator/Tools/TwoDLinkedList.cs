using System;
using System.Collections.Generic;
using System.Text;

namespace CityTrafficSimulator.Tools
	{
	/// <summary>
	/// Knoten einer vierfach (oder doppelt-doppelt) verketteten Liste
	/// </summary>
	/// <typeparam name="T">Typ der zu speichernden Daten</typeparam>
	public class TwoDLinkedListNode<T>
		{
		/// <summary>
		/// Im Knoten gespeicherter Wert
		/// </summary>
		private T m_value;
		/// <summary>
		/// Im Knoten gespeicherter Wert
		/// </summary>
		public T value
			{
			get { return m_value; }
			set { m_value = value; }
			}

		/// <summary>
		/// zugehörige TwoDLinkedList
		/// </summary>
		private TwoDLinkedList<T> m_list;
		/// <summary>
		/// zugehörige TwoDLinkedList
		/// </summary>
		public TwoDLinkedList<T> list
			{
			get { return m_list; }
			protected set { m_list = value; }
			}

		/// <summary>
		/// Knoten links
		/// </summary>
		private TwoDLinkedListNode<T> m_before;
		/// <summary>
		/// Knoten links
		/// </summary>
		public TwoDLinkedListNode<T> before
			{
			get { return m_before; }
			protected set { m_before = value; }
			}

		/// <summary>
		/// Knoten rechts
		/// </summary>
		private TwoDLinkedListNode<T> m_behind;
		/// <summary>
		/// Knoten rechts
		/// </summary>
		public TwoDLinkedListNode<T> behind
			{
			get { return m_behind; }
			protected set { m_behind = value; }
			}

		/// <summary>
		/// Knoten dadrüber
		/// </summary>
		private TwoDLinkedListNode<T> m_above;
		/// <summary>
		/// Knoten dadrüber
		/// </summary>
		public TwoDLinkedListNode<T> above
			{
			get { return m_above; }
			set { m_above = value; }
			}

		/// <summary>
		/// Knoten dadrunter
		/// </summary>
		private TwoDLinkedListNode<T> m_below;
		/// <summary>
		/// Knoten dadrunter
		/// </summary>
		public TwoDLinkedListNode<T> below
			{
			get { return m_below; }
			set { m_below = value; }
			}

		}

	/// <summary>
	/// Implementierung einer vierfach (oder doppelt-doppelt) verketteten Liste
	/// </summary>
	/// <typeparam name="T">Typ der zu speichernden Daten</typeparam>
	public class TwoDLinkedList<T>
		{


		}
	}

