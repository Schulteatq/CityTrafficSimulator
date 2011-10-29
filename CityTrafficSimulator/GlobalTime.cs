using System;
using System.Collections.Generic;
using System.Text;

namespace CityTrafficSimulator
	{
	/// <summary>
	/// Singleton class for global simulation time
	/// </summary>
	public class GlobalTime
		{
		#region Singleton stuff

		/// <summary>
		/// Singleton instance
		/// </summary>
		private static readonly GlobalTime m_instance = new GlobalTime();

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static GlobalTime Instance
			{
			get { return m_instance; }
			}

		/// <summary>
		/// Private Constructor - only to be used by singleton itsself.
		/// </summary>
		private GlobalTime()
			{
			m_currentTime = 0;
			}

		#endregion

		#region Fields and Variables

		/// <summary>
		/// current simulation time
		/// </summary>
		private double m_currentTime;
		/// <summary>
		/// current simulation time
		/// </summary>
		public double currentTime
			{
			get { return m_currentTime; }
			}

		/// <summary>
		/// current simulation time casted to float
		/// </summary>
		public float currentTimeAsFloat
			{
			get { return (float)m_currentTime; }
			}


		#endregion

		#region Methods

		/// <summary>
		/// Advances current time by time.
		/// </summary>
		/// <param name="time">Time to add to current time</param>
		public void Advance(double time)
			{
			m_currentTime += time;
			}

		/// <summary>
		/// Resets current time to 0.
		/// </summary>
		public void Reset()
			{
			m_currentTime = 0;
			}

		#endregion

		}
	}
