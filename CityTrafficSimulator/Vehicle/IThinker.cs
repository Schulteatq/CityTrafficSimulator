using System;
using System.Collections.Generic;
using System.Text;

namespace CityTrafficSimulator.Vehicle
	{
	abstract class IThinker
		{

		/// <summary>
		/// liefert alle Intersections innerhalb der nächsten distanceWithin auf dem Weg von v zurück.
		/// Untersucht nicht nur die aktuelle NodeConnection sondern falls nötig auch die folgenden NodeConnections.
		/// </summary>
		/// <param name="distanceWithin">Suchentfernung</param>
		/// <param name="v">Fahrzeug, für die die Berechnungen durchgeführt werden sollen</param>
		/// <param name="currentTime">aktuelle Zeit in Sekunden nach Sekunde 0 (wird für originalArrivingTime-Berechnung benötigt)</param>
		/// <returns>Eine Liste aller Intersections zwischen currentPosition und currentPosition+distanceWithin</returns>
		protected static List<SpecificIntersection> GetNextIntersectionsOnMyTrack(IVehicle v, double distanceWithin, double currentTime)
			{
			// TODO: erzwungener Spurwechsel bei Berechnungen berücksichtigen

			List<SpecificIntersection> toReturn = new List<SpecificIntersection>();

			// muss ich nur auf der currentNodeConnection suchen?
			if (distanceWithin <= v.GetDistanceToEndOfLineSegment(v.currentNodeConnection, v.currentPosition))
				{
				List<SpecificIntersection> intersectionsToAdd = v.currentNodeConnection.GetIntersectionsWithinArcLength(new Interval<double>(v.currentPosition, v.currentPosition + distanceWithin));
				foreach (SpecificIntersection si in intersectionsToAdd)
					{
					toReturn.Add(si);

					// originalArrivingTimes berechnen, falls das noch nicht geschehen ist
					if (!v.originalArrivingTimes.ContainsKey(si.intersection))
						{
						if (v.physics.velocity < 4)
							v.originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(v, si.intersection.GetMyArcPosition(v.currentNodeConnection) - v.state.position, false));
						else
							v.originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(v, si.intersection.GetMyArcPosition(v.currentNodeConnection) - v.state.position, true));
						}
					}
				}
			// ich muss auch auf den folgenden NodeConnections suchen
			else
				{
				// erstmal alle Intersections auf dieser NodeConnection speichern
				double alreadyCheckedDistance = v.GetDistanceToEndOfLineSegment(v.currentNodeConnection, v.currentPosition);

				List<SpecificIntersection> intersectionsToAdd = v.currentNodeConnection.GetIntersectionsWithinArcLength(new Interval<double>(v.currentPosition, v.currentNodeConnection.lineSegment.length));
				foreach (SpecificIntersection si in intersectionsToAdd)
					{
					toReturn.Add(si);

					// originalArrivingTimes berechnen, falls das noch nicht geschehen ist
					if (!v.originalArrivingTimes.ContainsKey(si.intersection))
						{
						if (v.physics.velocity < 4)
							v.originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(v, si.intersection.GetMyArcPosition(v.currentNodeConnection) - v.state.position, false));
						else
							v.originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(v, si.intersection.GetMyArcPosition(v.currentNodeConnection) - v.state.position, true));
						}
					}


				// nun die Intersections auf den folgenden NodeConnections
				foreach (IVehicle.RouteSegment rs in v.WayToGo)
					{
					NodeConnection nc = rs.startConnection;

					// Abbruchbedingung: das LineSegment ist länger als die Rest-Suchentfernung
					if (nc.lineSegment.length > distanceWithin - alreadyCheckedDistance)
						{
						intersectionsToAdd = nc.GetIntersectionsWithinArcLength(new Interval<double>(0, distanceWithin - alreadyCheckedDistance));
						foreach (SpecificIntersection si in intersectionsToAdd)
							{
							toReturn.Add(si);

							// originalArrivingTimes berechnen, falls das noch nicht geschehen ist
							if (!v.originalArrivingTimes.ContainsKey(si.intersection))
								{
								if (v.physics.velocity < 4)
									v.originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(v, si.intersection.GetMyArcPosition(nc) + alreadyCheckedDistance, false));
								else
									v.originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(v, si.intersection.GetMyArcPosition(nc) + alreadyCheckedDistance, true));
								}
							}

						break;
						}
					// ich muss noch mehr suchen - also alle Intersections reinballern und die nächste Nodeconnection nehmen
					else
						{
						foreach (Intersection i in nc.intersections)
							{
							SpecificIntersection si = new SpecificIntersection(nc, i);
							toReturn.Add(si);

							// originalArrivingTimes berechnen, falls das noch nicht geschehen ist
							if (!v.originalArrivingTimes.ContainsKey(si.intersection))
								{
								if (v.physics.velocity < 4)
									v.originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(v, si.intersection.GetMyArcPosition(nc) + alreadyCheckedDistance, false));
								else
									v.originalArrivingTimes.Add(si.intersection, currentTime + GetTimeToCoverDistance(v, si.intersection.GetMyArcPosition(nc) + alreadyCheckedDistance, true));
								}
							}

						alreadyCheckedDistance += nc.lineSegment.length;
						}
					}
				}

			return toReturn;
			}

		/// <summary>
		/// Finde alle Fahrzeuge die das Fahrzeug v evtl. ausbremsen könnte
		/// </summary>
		/// <param name="v">Fahrzeug, für die die Berechnungen durchgeführt werden sollen</param>
		/// <param name="distanceWithin">Suchentfernung</param>
		/// <returns>Liste von JammedVehicles</returns>
		protected static List<InterferingVehicle> GetJammedVehicles(IVehicle v, double distanceWithin)
			{
			List<InterferingVehicle> toReturn = new List<InterferingVehicle>();

			#region erstmal alle Intersections bestimmen
			List<Intersection> intersections = new List<Intersection>();

			// kann ich die Suche auf die currentNodeConnection beschränken?
			if (v.GetDistanceToEndOfLineSegment(v.currentNodeConnection, v.currentPosition) > distanceWithin)
				{
				intersections.AddRange(v.currentNodeConnection.GetIntersectionsWithinTime(
					new Interval<double>(
						v.currentNodeConnection.lineSegment.PosToTime(v.currentPosition),
						v.currentNodeConnection.lineSegment.PosToTime(v.currentPosition + distanceWithin)
						)));
				}
			// ich muss noch die nächsten NodeConnections auf meinem Weg durchsuchen
			else
				{
				intersections.AddRange(v.currentNodeConnection.GetIntersectionsWithinTime(
					new Interval<double>(v.currentNodeConnection.lineSegment.PosToTime(v.currentPosition), 1)));

				double alreadyCheckedDistance = v.GetDistanceToEndOfLineSegment(v.currentNodeConnection, v.currentPosition);
				foreach (IVehicle.RouteSegment rs in v.WayToGo)
					{
					NodeConnection nc = rs.startConnection;

					// Abbruchkriterium
					if (alreadyCheckedDistance + nc.lineSegment.length > distanceWithin)
						{
						intersections.AddRange(v.currentNodeConnection.GetIntersectionsWithinTime(
							new Interval<double>(
								v.currentNodeConnection.lineSegment.PosToTime(v.currentPosition),
								v.currentNodeConnection.lineSegment.PosToTime(v.currentPosition + distanceWithin)
								)));
						break;
						}
					else
						{
						intersections.AddRange(nc.GetIntersectionsWithinTime(new Interval<double>(0, 1)));
						alreadyCheckedDistance += nc.lineSegment.length;
						}
					}
				}
			#endregion

			return toReturn;
			}


		/// <summary>
		/// Berechnet die benötigte Zeit die das Fahrzeug v braucht, um sich distance weit zu bewegen
		/// </summary>
		/// <param name="v">Fahrzeug, für die die Berechnungen durchgeführt werden sollen</param>
		/// <param name="distance">Entfernung, die zurückgelegt werden soll</param>
		/// <param name="keepVelocity">soll die aktuelle Geschwindigkeit beibehalten werden, oder auf Wunschgeschwindigkeit beschleunigt werden?</param>
		/// <returns>approximierte Anzahl der Ticks, die das Fahrzeug braucht um distance Entfernung zurückzulegen</returns>
		public static double GetTimeToCoverDistance(IVehicle v, double distance, bool keepVelocity)
			{
			if (distance < 0)
				{
				return 0;
				}

			if (keepVelocity)
				{
				return distance / v.physics.velocity;
				}
			else
				{
				/*
				 * Da sich die IDM Differentialgleichung zwar lösen ließ, aber das Integral darüber selbst für Mathematica 
				 * unlösbar erscheint, lässt sich die benötigte Zeit leider nicht allgemein analytisch lösen.
				 * Genausowenig vermochte Mathematica die Rekurrenzgleichung der durch das Eulerverfahren gegebenen Iteration lösen.
				 * Daher verfolgen wir hier nun einen iterativen Ansatz, der einfach die Zukunft so lange simuliert, bis distance
				 * 
				 * erreicht wurde. Vielleicht könnte man hier in Zukunft eine Tabelle mit Richtwerten anlegen, in der man nur noch 
				 * den passenden Eintrag suchen muss und so Rechenzeit sparen. Momentan scheinen aber noch genug Leistungsreserven 
				 * vorhanden zu sein.
				 */
				double alreadyCoveredDistance = 0;
				int alreadySpentTime = 0;
				double currentVelocity = v.physics.velocity;

				while (alreadyCoveredDistance <= distance)
					{
					currentVelocity += v.CalculateAcceleration(currentVelocity, v.physics.effectiveDesiredVelocity);
					alreadyCoveredDistance += currentVelocity;
					alreadySpentTime++;
					}

				return alreadySpentTime - (1 - ((alreadyCoveredDistance - distance) / currentVelocity));
				}
			}
		}
	}
