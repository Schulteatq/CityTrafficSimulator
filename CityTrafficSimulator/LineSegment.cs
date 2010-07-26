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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;


namespace CityTrafficSimulator
    {
	/// <summary>
	/// Kapselung einer Bezierkurve
	/// Bietet grundlegende Methoden wie Contains(), Intersects(), Subdivide()
	/// </summary>
    public class LineSegment
        {
		/// <summary>
		/// wie fein/genau soll die Länge approximiert werden
		/// </summary>
        private static int feinheit = 31;

        /// <summary>
		/// Speichert zu welcher Zeit die Kurve welche Länge hat
        /// </summary>
        private double[] lengthList = new double[feinheit + 1]; 

        /// <summary>
		/// Intervall in dem sich die Bogenlänge befindet (nur lesen)
        /// </summary>
        private Interval<double> m_posInterval;
		/// <summary>
		/// Intervall in dem sich die Bogenlänge befindet (nur lesen)
		/// </summary>
        public Interval<double> posInterval
            {
            get { return m_posInterval; }
            }
	

        #region Stützpunkte
		/// <summary>
		/// erster Stützpunkt
		/// </summary>
        private Vector2 m_p0;
		/// <summary>
		/// zweiter Stützpunkt
		/// </summary>
        private Vector2 m_p1;
		/// <summary>
		/// dritter Stützpunkt
		/// </summary>
        private Vector2 m_p2;
		/// <summary>
		/// vierter Stützpunkt
		/// </summary>
        private Vector2 m_p3;


		/// <summary>
		/// erster Stützpunkt
		/// </summary>
        public Vector2 p0
            {
            get { return m_p0; }
            set { m_p0 = value; }
            }
		/// <summary>
		/// zweiter Stützpunkt
		/// </summary>
        public Vector2 p1
            {
            get { return m_p1; }
            set { m_p1 = value; }
            }
		/// <summary>
		/// dritter Stützpunkt
		/// </summary>
        public Vector2 p2
            {
            get { return m_p2; }
            set { m_p2 = value; }
            }
		/// <summary>
		/// vierter Stützpunkt
		/// </summary>
        public Vector2 p3
            {
            get { return m_p3; }
            set { m_p3 = value; }
            }
        #endregion

        #region Längenapproximation
        /// <summary>
        /// Euklidische Länge des Segmentes
        /// </summary>
        private double m_Length = 0;
		/// <summary>
		/// Euklidische Länge des Segmentes
		/// </summary>
        public double length
            {
            get { return m_Length; }
            set { m_Length = value; }
            }
        #endregion
        
        #region Konstruktoren

        /// <summary>
        /// Erstellt ein neues Bezierkurven Segment aus 4 gegebenen Stützpunkten
        /// </summary>
        /// <param name="arcPosStart">Initiale Kurvenlänge</param>
        /// <param name="p0">Anfangspunkt</param>
        /// <param name="p1">Auslaufrichtung</param>
        /// <param name="p2">Einlaufrichtung</param>
        /// <param name="p3">Endpunkt</param>
        public LineSegment(double arcPosStart, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
            {
            // Stützpunkte speichern
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;

            // Länge approximieren
            Vector2 LastPoint = p0;
            double sum = arcPosStart;
            for (int i = 0; i <= feinheit; i++)
                {
                float t = (float)i / feinheit;
                Vector2 CurrentPoint = this.AtTime(t);

                sum += Vector2.GetDistance(CurrentPoint, LastPoint);
                LastPoint = CurrentPoint;
                lengthList[i] = sum;
                }
            this.length = lengthList[lengthList.Length - 1];
            this.m_posInterval = new Interval<double>(arcPosStart, length);

			m_BoundingRectangle = new RectangleF(
				(float)Math.Min(p0.X, (Math.Min(p1.X, Math.Min(p2.X, p3.X)))),
				(float)Math.Min(p0.Y, (Math.Min(p1.Y, Math.Min(p2.Y, p3.Y)))),
				0,
				0);
			m_BoundingRectangle.Width = (float)Math.Max(p0.X, (Math.Max(p1.X, Math.Max(p2.X, p3.X)))) - m_BoundingRectangle.X;
			m_BoundingRectangle.Height = (float)Math.Max(p0.Y, (Math.Max(p1.Y, Math.Max(p2.Y, p3.Y)))) - m_BoundingRectangle.Y;
			}

        #endregion

        /// <summary>
        /// Gibt die Koordinaten der Kurve zum Zeitpunkt t zurück
        /// </summary>
        /// <param name="t">Zeitpunkt</param>
        public Vector2 AtTime(double t)
            {
			// ein paar Multiplikationen durch Zwischenspeicherung einsparen
			double tt = t - 1;
			double tt2 = tt * tt;
			double tt3 = tt2 * tt;

            double x = -(p0.X * tt3) + t * (3 * p1.X * tt2 + t * (3 * p2.X - 3 * p2.X * t + p3.X * t));
            double y = -(p0.Y * tt3) + t * (3 * p1.Y * tt2 + t * (3 * p2.Y - 3 * p2.Y * t + p3.Y * t));

            return new Vector2(x, y);
            }

		/// <summary>
		/// Berechnet die Koordinaten der Kurve zu Zeitpunkt t mit dem DeCasteljau-Algorithmus.
		/// (mir wurde mal erzählt, der sei schneller als die direkte Auswertung, praktisch macht es jedoch bisher keinen Unterschied)
		/// </summary>
		/// <param name="t">Zeitpunkt</param>
		/// <returns>Koordinaten der Bezierkurve zum Zeitpunkt t</returns>
		public Vector2 AtTimeDeCasteljau(double t)
			{
			// Erste Iteration:
			Vector2 p01 = p0 + ((p1 - p0) * t);
			Vector2 p11 = p1 + ((p2 - p1) * t);
			Vector2 p21 = p2 + ((p3 - p2) * t);

			// Zweite Iteration:
			Vector2 p02 = p01 + ((p11 - p01) * t);
			Vector2 p12 = p11 + ((p21 - p11) * t);

			// Dritte Iteration:
			return p02 + ((p12 - p02) * t);
			}


		/// <summary>
		/// Gibt die erste Ableitung (Richtung) der Bezierfunktion zum Zeitpunkt t zurück
		/// </summary>
		/// <param name="t">Zeitpunkt</param>
		/// <returns>Richtungsvektor</returns>
		public Vector2 DerivateAtTime(double t)
			{
			//3 (-p0 + p1 + 2 (p0 - 2 p1 + p2) t + (-p0 + 3 p1 - 3 p2 + p3) t^2)
			double x = 3 * (-p0.X + p1.X + 2 * (p0.X - 2 * p1.X + p2.X) * t + (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * t * t);
			double y = 3 * (-p0.Y + p1.Y + 2 * (p0.Y - 2 * p1.Y + p2.Y) * t + (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t * t);

			return new Vector2(x, y);
			}


        /// <summary>
        /// berechnet/approximiert den Zeitpunkt zu einer bestimmten Position auf der Kurve
		/// dank binärer Suche logarithmische Laufzeit
        /// </summary>
        /// <param name="Position">gewünschte Entfernung zum Start der Kurve</param>
		/// <returns>Zeitparameter der Bezierkurve in [0,1] </returns>
        public double PosToTime(double Position)
            {
            if (Position < length)
                {
                // Approximiere die gesuchte Position
                int lborder = 0;
                int rborder = lengthList.Length - 1;

                // Finde zunächst die gespeicherten Einträge, die die gesuchte Position umranden
                while (rborder - lborder > 1)
                    {
                    int i = (lborder + rborder) / 2;

                    if (Position == lengthList[i])
                        {
                        lborder = i;
                        break;
                        }
                    else if (Position < lengthList[i])
                        rborder = i;
                    else
                        lborder = i;
                    }

                // Nun berechne, wo ungefähr dazwischen die gesuchte Position ist
                double diff = lengthList[rborder] - lengthList[lborder];
                return (double)(lborder + ((double)1 / diff) * (Position - lengthList[lborder])) / feinheit;
                }
            else
                return 1;
            }

		/// <summary>
		/// approximiert die ArcPosition auf der Linie zum Zeitparameter time
		/// </summary>
		/// <param name="time"></param>
		/// <returns>Bogenlängenposition durch lengthList approximiert</returns>
		public double TimeToArcPosition(double time)
			{
			if (time >= 1)
				return m_Length;
			else
				{
				int index = (int)Math.Floor(time * feinheit);
				return lengthList[index] + ((time * feinheit) - index) * (lengthList[index + 1] - lengthList[index]);
				}
			}

        /// <summary>
        /// gibt die Koordinaten zur Position pos auf der Kurve zurück
        /// </summary>
        /// <param name="pos">Entfernung zum Startpunkt</param>
        public Vector2 AtPosition(double pos)
            {
            return AtTime(PosToTime(pos));
            }

		/// <summary>
		/// Zeichnet das LineSegment mit dem Stift pen auf die Zeichenfläche g
		/// </summary>
		/// <param name="g">Zeichenfläche auf die gemalt werden soll</param>
		/// <param name="pen">zu benutzender Stift</param>
        public void Draw(Graphics g, Pen pen)
            {
			g.DrawBezier(pen, m_p0, m_p1, m_p2, m_p3);
            }


        #region De-Casteljau Algorithmus

		/// <summary>
		/// Teilt das LineSegment in zwei LineSegments auf
		/// Verwendet wird der De-Casteljau Algorithmus
		/// </summary>
		/// <returns>Ein Paar LineSegmente, die hintereinander das Ursprüngliche LineSegment ergeben</returns>
        public Pair<LineSegment> Subdivide()
			{
			// Erste Iteration:
			Vector2 p01 = p0 + ((p1 - p0) * 0.5d);
			Vector2 p11 = p1 + ((p2 - p1) * 0.5d);
			Vector2 p21 = p2 + ((p3 - p2) * 0.5d);

			// Zweite Iteration:
			Vector2 p02 = p01 + ((p11 - p01) * 0.5d);
			Vector2 p12 = p11 + ((p21 - p11) * 0.5d);

			// Dritte Iteration:
			Vector2 p03 = p02 + ((p12 - p02) * 0.5d);

			return new Pair<LineSegment>(new LineSegment(0, p0, p01, p02, p03), new LineSegment(0, p03, p12, p21, p3));
			}

		/// <summary>
		/// BoundingBox der Bézierkurve
		/// </summary>
		private RectangleF m_BoundingRectangle;
		/// <summary>
		/// BoundingBox der Bézierkurve
		/// </summary>
		public RectangleF boundingRectangle
			{
			get { return m_BoundingRectangle; }
			}

		/// <summary>
		/// Gibt die rechteckige BoundingBox der vier Stützpunkte zurück.
		/// </summary>
		/// <param name="pixelsToAdd">Anzahl der Pixel die zu jeder Seite hinzugerechnet werden sollen</param>
		/// <returns>Ein Rechteck, welches die vier Stützpunkte umschließt.</returns>
		public RectangleF GetBounds(float pixelsToAdd)
			{
			return new RectangleF(
				m_BoundingRectangle.X - pixelsToAdd, 
				m_BoundingRectangle.Y - pixelsToAdd, 
				m_BoundingRectangle.Width + 2*pixelsToAdd, 
				m_BoundingRectangle.Height + 2*pixelsToAdd);
			}

		/// <summary>
		/// Überprüft, ob der Punkt position auf der Bezierline liegt.
		/// </summary>
		/// <param name="position">Position welche überprüft werden soll</param>
		/// <param name="lineWidth">Breite der Linie die berücksichtigt werden soll</param>
		/// <param name="tolerance">Genauigkeit der Überprüfung: minimale Kantenlänge der überprüften Boundingbox.</param>
		/// <returns>true, falls position maximal tolerance Pixel von der Linie entfernt ist, sonst false</returns>
		public bool Contains(Vector2 position, float lineWidth, float tolerance)
			{
			if (tolerance < 2 * lineWidth)
				{
				tolerance = 2 * lineWidth + 1;
				}

			RectangleF boundingBox = GetBounds(lineWidth);

			LineSegment ls = this;

			if (boundingBox.Contains(position))
				{
				while ((boundingBox.Height > tolerance) || (boundingBox.Width > tolerance))
					{
					// Linie aufsplitten und Einzelteile prüfen
					Pair<LineSegment> divided = ls.Subdivide();

					if (divided.Left.GetBounds(lineWidth).Contains(position))
						{
						ls = divided.Left;
						boundingBox = ls.GetBounds(lineWidth);
						}
					else if (divided.Right.GetBounds(lineWidth).Contains(position))
						{
						ls = divided.Right;
						boundingBox = ls.GetBounds(lineWidth);
						}
					else
						{
						return false;
						}
					}
				return true;
				}

			return false;
			}

		/// <summary>
		/// Überprüft, ob der Punkt position auf der Bezierline liegt.
		/// Hier wird per se eine Standardgenauigkeit von 3 Pixeln angenommen
		/// </summary>
		/// <param name="position">Position welche überprüft werden soll</param>
		/// <returns>true, falls position maximal 3 Pixel von der Linie entfernt ist, sonst false</returns>
		public bool Contains(Vector2 position)
			{
			return Contains(position, 4, 11);
			}

        #endregion
        }
    }
