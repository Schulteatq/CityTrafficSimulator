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
using System.Drawing.Drawing2D;

namespace CityTrafficSimulator.Vehicle
	{
	/// <summary>
	/// Implementierung eines IVehicleBehaviour, welches modelliert, dass das Fahrzeug sich einer Intersection annähert
	/// </summary>
	class ApproachingIntersectionVehicleBehaviour : IVehicleBehaviour
		{
		#region Felder

		/// <summary>
		/// Fahrzeug
		/// </summary>
		private IVehicle me;

		/// <summary>
		/// Intersection, an welche sich das Fahrzeug annähert
		/// </summary>
		private Intersection m_intersectionToApproach;
		/// <summary>
		/// Intersection, an welche sich das Fahrzeug annähert
		/// </summary>
		public Intersection intersectionToApproach
			{
			get { return m_intersectionToApproach; }
			private set { m_intersectionToApproach = value; }
			}

		/// <summary>
		/// chronologisch sortierte Liste von VehicleDistances, die zeitlich vor mir die Intersection erreichen.
		/// die Distance gibt an, wie weit das Vehicle (parallel gesehen) vor mir fährt.
		/// 
		/// previousVehicles.First ist das Fahrzeug, welches als erstes die Intersection erreicht, 
		/// previousVehicles.Last das Fahrzeug, welches als letztes vor mir die Intersection erreicht.
		/// </summary>
		private LinkedList<VehicleDistanceTime> m_previousVehicles = new LinkedList<VehicleDistanceTime>();
		/// <summary>
		/// chronologisch sortierte Liste von VehicleDistances, die zeitlich vor mir die Intersection erreichen.
		/// die Distance gibt an, wie weit das Vehicle (parallel gesehen) vor mir fährt.
		/// 
		/// previousVehicles.First ist das Fahrzeug, welches als erstes die Intersection erreicht, 
		/// previousVehicles.Last das Fahrzeug, welches als letztes vor mir die Intersection erreicht.
		/// </summary>
		public LinkedList<VehicleDistanceTime> previousVehicles
			{
			get { return m_previousVehicles; }
			private set { m_previousVehicles = value; }
			}

		/// <summary>
		/// chronologisch sortierte Liste von VehicleDistances, die zeitlich nach mir die Intersection erreichen.
		/// die Distance gibt an, wie weit das Vehicle (parallel gesehen) vor mir fährt.
		/// 
		/// nextVehicles.First ist das Fahrzeug, welches als erstes nach mir die Intersection erreicht, 
		/// nextVehicles.Last das Fahrzeug, welches als letztes die Intersection erreicht.
		/// </summary>
		private LinkedList<VehicleDistanceTime> m_nextVehicles = new LinkedList<VehicleDistanceTime>();
		/// <summary>
		/// chronologisch sortierte Liste von VehicleDistances, die zeitlich nach mir die Intersection erreichen.
		/// die Distance gibt an, wie weit das Vehicle (parallel gesehen) vor mir fährt.
		/// 
		/// nextVehicles.First ist das Fahrzeug, welches als erstes nach mir die Intersection erreicht, 
		/// nextVehicles.Last das Fahrzeug, welches als letztes die Intersection erreicht.
		/// </summary>
		public LinkedList<VehicleDistanceTime> nextVehicles
			{
			get { return m_nextVehicles; }
			private set { m_nextVehicles = value; }
			}


		/// <summary>
		/// Entfernung bis Erreichen der Intersection
		/// </summary>
		private double m_distanceToIntersection;
		/// <summary>
		/// Entfernung bis Erreichen der Intersection
		/// </summary>
		public double distanceToIntersection
			{
			get { return m_distanceToIntersection; }
			set { m_distanceToIntersection = value; }
			}

		/// <summary>
		/// geschätzte Zeit um die Intersection zu erreichen
		/// </summary>
		private double m_timeToReachIntersection;
		/// <summary>
		/// geschätzte Zeit um die Intersection zu erreichen
		/// </summary>
		public double timeToReachIntersection
			{
			get { return m_timeToReachIntersection; }
			set { m_timeToReachIntersection = value; }
			}

		/// <summary>
		/// geschätzte Zeit, bis die Intersection wieder verlassen ist
		/// </summary>
		private double m_timeToLeaveIntersection;
		/// <summary>
		/// geschätzte Zeit, bis die Intersection wieder verlassen ist
		/// </summary>
		public double timeToLeaveIntersection
			{
			get { return m_timeToLeaveIntersection; }
			set { m_timeToLeaveIntersection = value; }
			}

		/// <summary>
		/// geschätztes Zeitintervall, wann das Fahrzeug die Kreuzung blockiert
		/// </summary>
		public Interval<double> timespanAtIntersection 
			{
			get { return new Interval<double>(m_timeToReachIntersection, m_timeToLeaveIntersection); }
			}


		#endregion


		#region Konstruktor

		/// <summary>
		/// Standardkonstruktor.
		/// Berechnet alle parallel fahrenden Fahrzeuge und füllt die dafür vorgesehenden Felder
		/// </summary>
		/// <param name="me">IVehicle für welches die Berechnungen durchgeführt werden sollen</param>
		/// <param name="intersectionToApproach">Intersection, welche angefahren wird</param>
		public ApproachingIntersectionVehicleBehaviour(IVehicle me, Intersection intersectionToApproach)
			{
			m_intersectionToApproach = intersectionToApproach;
			this.me = me;

			// distanceToIntersection berechnen:
			NodeConnection currentNodeConnection = me.currentNodeConnection;
			double m_distanceToIntersection = me.GetDistanceToEndOfLineSegment(me.currentNodeConnection, me.currentPosition);

			int i = 0;
			while (true)
				{
				if (currentNodeConnection == intersectionToApproach.aConnection || currentNodeConnection == intersectionToApproach.bConnection)
					{
					m_distanceToIntersection -= (currentNodeConnection == intersectionToApproach.aConnection 
										? intersectionToApproach.aConnection.lineSegment.length - intersectionToApproach.aArcPosition
										: intersectionToApproach.bConnection.lineSegment.length - intersectionToApproach.bArcPosition);
					break;
					}

				// REMARK: 
				// REMARK:	aufgrund von Änderungen in der RouteToTarget-Sturktur wurde der folgende Code ungültig. Da er 
				//			momentan eh nicht benutzt wird, habe ich ihn einfach durch null ersetzt...
				// REMARK: 
				currentNodeConnection = null; //me.WayToGo.route.ToArray()[i].startConnection;
				
				m_distanceToIntersection += currentNodeConnection.lineSegment.length;
				i++;
				}
			
			m_timeToReachIntersection = me.GetTimeToCoverDistance(m_distanceToIntersection, false);
			m_timeToLeaveIntersection = me.GetTimeToCoverDistance(m_distanceToIntersection + me.length, false);


			// nun wollen wir die parallel fahrenden Fahrzeuge berechnen:
			// das könnte aufwändiger werden... o_O
			NodeConnection theOtherNodeConnection = (intersectionToApproach.aConnection == me.currentNodeConnection ? intersectionToApproach.bConnection : intersectionToApproach.aConnection);
			double theOtherArcPosition = (intersectionToApproach.aConnection == me.currentNodeConnection ? intersectionToApproach.bArcPosition : intersectionToApproach.aArcPosition);

			LinkedList<VehicleDistanceTime> vehicles = GetVehicleDistancesWithinTime(theOtherNodeConnection, theOtherArcPosition, Constants.lookaheadDistance * 2, Constants.lookaheadTime);

			// nun in previous/next einsortieren
			foreach (VehicleDistanceTime vdt in vehicles)
				{
				if (vdt.time < m_timeToReachIntersection)
					{
					m_previousVehicles.AddLast(new VehicleDistanceTime(vdt.vehicle, m_distanceToIntersection - vdt.distance, vdt.time));
					}
				else if (vdt.time > m_timeToReachIntersection)
					{
					m_nextVehicles.AddLast(new VehicleDistanceTime(vdt.vehicle, m_distanceToIntersection - vdt.distance, vdt.time));
					}
				}

			// fertig?
			}

		#endregion


		#region Hilfsmethoden

		/// <summary>
		/// berechnet alle VehicleDistances vor startArcPosition auf nc von Fahrzeugen die maximal
		/// timeWithin Zeit zur Ankunft brauchen oder maximal distanceWithin vom Ziel entfernt sind
		/// </summary>
		/// <param name="nc">NodeConnection des Ziels</param>
		/// <param name="distanceToTarget">Entfernung zum Ziel von Startknoten von nc aus</param>
		/// <param name="distanceWithin">Suchreichweite (maximale Suchentfernung)</param>
		/// <param name="timeWithin">Suchreichweite (maximal benötigte Zeit)</param>
		/// <returns>eine sortierte doppelt verkettete Liste von VehicleDistanceTimes, die aufsteigend nach Zeit sortiert ist</returns>
		private LinkedList<VehicleDistanceTime> GetVehicleDistancesWithinTime(NodeConnection nc, double distanceToTarget, double distanceWithin, double timeWithin)
			{
			LinkedList<VehicleDistanceTime> toReturn = new LinkedList<VehicleDistanceTime>();

			// Anfang finden
			LinkedListNode<IVehicle> lln = nc.vehicles.Last;

			while (true)
				{
				// kein LinkedListNode, dann mit dein vorherigen NodeConnections weitermachen
				if (lln == null)
					{
					// erstmal rekursiv alle Fahrzeuge bestimmen
					List<LinkedList<VehicleDistanceTime>> lol = new List<LinkedList<VehicleDistanceTime>>();
					foreach (NodeConnection prevNc in nc.startNode.prevConnections)
						{
						lol.Add(GetVehicleDistancesWithinTime(prevNc, distanceToTarget+prevNc.lineSegment.length, distanceWithin, timeWithin));
						}

					// Nun à la Natural-Mergesort alles in toReturn einsortieren:
					foreach (LinkedList<VehicleDistanceTime> ll in lol)
						{
						LinkedListNode<VehicleDistanceTime> vlln = toReturn.First;

						foreach (VehicleDistanceTime vdt in ll)
							{
							if (vlln == null)
								{
								toReturn.AddLast(vdt);
								}
							else if (vdt.time > vlln.Value.time)
								{
								vlln = vlln.Next;
								}
							else
								{
								toReturn.AddAfter(vlln, vdt);
								}
							}
						}

					// und Schleife abbrechen 
					break;
					}

				// Entfernung und Zeit berechnen
				double llnDistance = distanceToTarget - lln.Value.currentPosition;
				double llnReachTime = lln.Value.GetTimeToCoverDistance(llnDistance, true);

				// Das Fahrzeug ist schon hinter dem Ziel, also vorherigen Node betrachten
				if (llnDistance <= 0)
					{
					lln = lln.Previous;
					continue;
					}
				
				// Abbruchkriterium
				if (llnReachTime > timeWithin || llnDistance > distanceWithin)
					{
					break;
					}

				toReturn.AddLast(new VehicleDistanceTime(lln.Value, llnDistance, llnReachTime));
				lln = lln.Previous;
				}


			return toReturn;
			}

		#endregion


		#region IVehicleBehaviour-Member

		/// <summary>
		/// Gibt die Art des VehicleBehaviour zurück
		/// </summary>
		/// <returns>Wert aus IVehicleBehaviour.BehaviourType</returns>
		public override IVehicleBehaviour.BehaviourType GetBehaviourType()
			{
			return BehaviourType.APPROACHING_INTERSECTION;
			}

		/// <summary>
		/// Gibt einen String mit Debuginformationen zurück
		/// </summary>
		/// <returns>Following Vehicle #... @...</returns>
		public override string GetDebugString()
			{
			return "Approaching Intersection @" + m_intersectionToApproach.aPosition + ", reaching in " + m_timeToReachIntersection;
			}

		/// <summary>
		/// zeichnet Debuginformationen auf die Zeichenfläche g
		/// </summary>
		/// <param name="g">Zeichenfläche auf die gezeichnet werden soll</param>
		public override void DrawDebugGraphics(Graphics g)
			{
			using(Pen linePen = new Pen(Color.Green))
				{		
				// previousVehicles zeichnen
				foreach (VehicleDistanceTime vdt in m_previousVehicles)
					{
					g.DrawLine(linePen, me.currentNodeConnection.lineSegment.AtPosition(me.currentPosition), vdt.vehicle.currentNodeConnection.lineSegment.AtPosition(vdt.vehicle.currentPosition));
					g.DrawEllipse(linePen, new Rectangle(me.currentNodeConnection.lineSegment.AtPosition(me.currentPosition) - new Vector2(3, 3), new Size(6, 6)));
					}

				// nextVehicles zeichnen
				linePen.Color = Color.Red;
				foreach (VehicleDistanceTime vdt in m_nextVehicles)
					{
					g.DrawLine(linePen, me.currentNodeConnection.lineSegment.AtPosition(me.currentPosition), vdt.vehicle.currentNodeConnection.lineSegment.AtPosition(vdt.vehicle.currentPosition));
					g.DrawEllipse(linePen, new Rectangle(me.currentNodeConnection.lineSegment.AtPosition(me.currentPosition) - new Vector2(3, 3), new Size(6, 6)));
					}
				}
			}

		#endregion
		}
	}
