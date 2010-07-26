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

namespace CityTrafficSimulator
    {
	/// <summary>
	/// abstrakte Klasse für das Intelligent Driver Model
	/// </summary>
    public abstract class IDM
        {
        #region Konstanten
        /*
         * Berechnungen nach dem IDM Schema der TU Dresden
         * http://www.cs.wm.edu/~coppit/csci435-spring2005/project/MOBIL.pdf
         */
        /*double T = 10; // Zeitlicher Sicherheitsabstand
        double a = 2; // Maxmale Beschleunigung°
        double b = 3; // komfortable Bremsverzögerung
        double s0 = 20; // Mindestabstand im Stau
        */

		/*
		 * Konstanten in s, m/s und m/s²
		 *
        protected double T = 2; // Zeitlicher Sicherheitsabstand
		protected double a = 1.2; // Maxmale Beschleunigung
		protected double b = 1.5; // komfortable Bremsverzögerung
		protected double s0 = 10; // Mindestabstand im Stau
		 //*/
		
		/*
		 * Konstanten in s/20, Pixel/20s^2
		 * 1 Pixel = 1 dm	
		 */
		/// <summary>
		/// Zeitlicher Sicherheitsabstand
		/// </summary>
		protected double T = 1.4;

		/// <summary>
		/// maximale Beschleunigung
		/// </summary>
		protected double a = 1.2;

		/// <summary>
		/// komfortable Bremsverzögerung
		/// </summary>
		protected double b = 1.5;

		/// <summary>
		/// Mindestabstand im Stau
		/// </summary>
		public double s0 = 20;


		/* MOBIL PARAMETER */


		/// <summary>
		/// Politeness-Faktor von MOBIL
		/// </summary>
		protected double p = 0.25;

		/// <summary>
		/// Mindest-Vorteilswert für Spurwechsel
		/// </summary>
		protected double lineChangeThreshold = 0.4;

		/// <summary>
		/// maximale sichere Bremsverzögerung
		/// </summary>
		protected double bSave = -3;
		//*/

		#endregion

		/// <summary>
		/// Berechnet den Wunschabstand nach dem IDM
		/// </summary>
		/// <param name="velocity">eigene Geschwindigkeit</param>
		/// <param name="vDiff">Differenzgeschwindigkeit zum Vorausfahrenden Fahrzeug</param>
		/// <returns></returns>
		public double CalculateWantedDistance(double velocity, double vDiff)
			{
			// berechne s* = Wunschabstand
			double ss = s0 + T * velocity + (velocity * vDiff) / (2 * Math.Sqrt(a * b));

			if (ss < s0)
				{
				ss = s0;
				}

			return ss;
			}

		/// <summary>
		/// Berechnet die Beschleunigung frei nach dem IDM
		/// http://www.cs.wm.edu/~coppit/csci435-spring2005/project/MOBIL.pdf
		/// </summary>
		/// <param name="velocity">aktuelle Geschwindigkei</param>
		/// <param name="desiredVelocity">Wunschgeschwindigkeit</param>
		/// <param name="distance">Distanz zum vorausfahrenden Fahrzeug</param>
		/// <param name="vDiff">Geschwindigkeitsunterschied zum vorausfahrenden Fahrzeug</param>
		/// <returns></returns>
        public double CalculateAcceleration(double velocity, double desiredVelocity, double distance, double vDiff)
            {
            // berechne s* = Wunschabstand
			double ss = CalculateWantedDistance(velocity, vDiff);

			// Neue Geschwindigkeit berechnen
            double vNeu = a * (1 - Math.Pow((velocity / desiredVelocity), 2) - Math2.Square(ss / distance));

            return vNeu;
            }

		/// <summary>
		/// Berechnet die Beschleunigung frei nach dem IDM, wenn kein Fahrzeug voraus fährt
		/// </summary>
		/// <param name="velocity">eigene Geschwindigkeit</param>
		/// <param name="desiredVelocity">Wunschgeschwindigkeit</param>
		/// <returns></returns>
		public double CalculateAcceleration(double velocity, double desiredVelocity)
			{
			// Neue Geschwindigkeit berechnen
			double vNeu = a * (1 - Math.Pow((velocity / desiredVelocity), 2) );

			return vNeu;
			}

		/// <summary>
		/// Berechnet die Beschleunigung frei nach dem IDM mit dem Verfahren von Heun (Konsistenzordnung 2!)
		/// http://www.cs.wm.edu/~coppit/csci435-spring2005/project/MOBIL.pdf
		/// </summary>
		/// <param name="velocity">aktuelle Geschwindigkei</param>
		/// <param name="desiredVelocity">Wunschgeschwindigkeit</param>
		/// <param name="distance">Distanz zum vorausfahrenden Fahrzeug</param>
		/// <param name="vDiff">Geschwindigkeitsunterschied zum vorausfahrenden Fahrzeug</param>
		/// <returns></returns>
		public double CalculateAccelerationHeun(double velocity, double desiredVelocity, double distance, double vDiff)
			{
			// erste Näherung:
				// berechne s* = Wunschabstand
				double ss1 = CalculateWantedDistance(velocity, vDiff);
				// Neue Geschwindigkeit berechnen
				double vNeu = a * (1 - Math.Pow((velocity / desiredVelocity), 2) - Math2.Square(ss1 / distance));

			// zweite Näherung:
				// berechne s* = Wunschabstand
				double ss2 = CalculateWantedDistance(velocity + vNeu, vDiff + vNeu);
				// Neue Geschwindigkeit berechnen
				vNeu += a * (1 - Math.Pow(((velocity + vNeu) / desiredVelocity), 2) - Math2.Square(ss2 / distance));

			return vNeu/2;
			}



		/// <summary>
		/// Berechnet die Beschleunigung, wenn Wunschabstand schon bekannt nach dem IDM
		/// </summary>
		/// <param name="velocity">aktuelle Geschwindigkeit</param>
		/// <param name="desiredVelocity">Wunschgeschwindigkeit</param>
		/// <param name="distance">aktueller Abstand</param>
		/// <param name="wantedDistance">Wunschabstand</param>
		/// <param name="vDiff">Geschwindigkeitsdifferenz</param>
		/// <returns></returns>
		public double CalculateAcceleration(double velocity, double desiredVelocity, double distance, double wantedDistance, double vDiff)
			{
			// Neue Geschwindigkeit berechnen
			double vNeu = a * (1 - Math.Pow((velocity / desiredVelocity), 2) - Math2.Square(wantedDistance / distance));

			return vNeu;
			}


        }
    }
