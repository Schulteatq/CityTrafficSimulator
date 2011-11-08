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
using System.Xml;
using System.Xml.Serialization;


namespace CityTrafficSimulator
    {
	/// <summary>
	/// zweidimensionaler double-Vektor
	/// </summary>
    [Serializable]
    public class Vector2
        {
		/// <summary>
		/// x-Koordinate
		/// </summary>
        private double x;
		/// <summary>
		/// y-Koordinate
		/// </summary>
        private double y;

		/// <summary>
		/// x-Koordinate
		/// </summary>
        public double X
            {
            get { return x; }
            set { x = value; }
            }

		/// <summary>
		/// y-Koordinate
		/// </summary>
        public double Y
            {
            get { return y; }
            set { y = value; }
            }

        #region Konstruktoren
		/// <summary>
		/// Standardkonstruktor - initialisiert den Vector2 mit (0,0)
		/// </summary>
        public Vector2()
            {
            this.x = 0;
            this.y = 0;
            }

		/// <summary>
		/// Konstruktor, initialisiert den Vector2 mit den übergebenen Parametern
		/// </summary>
		/// <param name="x">x-Koordinate</param>
		/// <param name="y">y-Koordinate</param>
        public Vector2(double x, double y)
            {
            this.x = x;
            this.y = y;
            }

		/// <summary>
		/// Konstruktor, initialisiert den Vector2 mit dem übergebenen PointF
		/// </summary>
		/// <param name="point">Punkt dessen Koordinaten den Vector2 initialisieren sollen</param>
        public Vector2(PointF point)
            {
            this.x = point.X;
            this.y = point.Y;
            }

		/// <summary>
		/// Konstruktor, initialisiert den Vector2 mit dem übergebenen Point
		/// </summary>
		/// <param name="point">Punkt dessen Koordinaten den Vector2 initialisieren sollen</param>
		public Vector2(Point point)
            {
            this.x = point.X;
            this.y = point.Y;
            }
        #endregion

        #region Operatoren
		/// <summary>
		/// Mal-Operator zwischen Vektor und Skalar
		/// </summary>
		/// <param name="v">Vektor</param>
		/// <param name="d">Skalar</param>
		/// <returns>v*d</returns>
        public static Vector2 operator *(Vector2 v, double d)
            {
            return new Vector2(v.X * d, v.Y * d);
            }

		/// <summary>
		/// Mal-Operator zwischen Skalar und Vektor 
		/// </summary>
		/// <param name="v">Vektor</param>
		/// <param name="d">Skalar</param>
		/// <returns>v*d</returns>
		public static Vector2 operator *(double d, Vector2 v)
            {
            return new Vector2(v.X * d, v.Y * d);
            }

		/// <summary>
		/// Skalarprodukt zweier Vektoren
		/// </summary>
		/// <param name="v1">erster Vektor</param>
		/// <param name="v2">zweiter Vektor</param>
		/// <returns>Skalarprodukt: dot(v1, v2) </returns>
        public static double operator *(Vector2 v1, Vector2 v2)
            {
            return v1.X * v2.X + v1.Y * v2.Y;
            }

		/// <summary>
		/// Vektoraddition
		/// </summary>
		/// <param name="v1">erster Vektor</param>
		/// <param name="v2">zweiter Vektor</param>
		/// <returns>v1 + v2</returns>
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
            {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
            }

		/// <summary>
		/// Vektorsubtraktion
		/// </summary>
		/// <param name="v1">erster Vektor</param>
		/// <param name="v2">zweiter Vektor</param>
		/// <returns>v1 - v2</returns>
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
            {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
            }
        #endregion
        
		/// <summary>
		/// Absolutbetrag/Länge des Vektors
		/// </summary>
        public double Abs
            {
            get { return Math.Sqrt(X * X + Y * Y); }
            }

		/// <summary>
		/// gibt den Vektor um 90° im Uhrzeigersinn gedreht zurück
		/// </summary>
        public Vector2 RotatedClockwise
            {
            get { return new Vector2(Y, -1 * X); }
            }

		/// <summary>
		/// gibt den Vektor um 90° im Uhrzeigersinn gedreht zurück
		/// </summary>
		public Vector2 RotatedCounterClockwise
			{
			get { return new Vector2(-Y, X); }
			}

		/// <summary>
		/// gibt den Vektor um phi gegen den Urzeigersinn gedreht zurück
		/// </summary>
		/// <param name="phi">Winkel im Bogenmaß um den gedreht werden soll</param>
		/// <returns>Ein neuer Vektor2 der gegenüber this um Phi gegen den Urzeigersinn gedreht ist</returns>
		public Vector2 RotateCounterClockwise(double phi)
			{
			return new Vector2(Math.Cos(phi)*x - Math.Sin(phi) * y, Math.Sin(phi)*x + Math.Cos(phi) * y);
			}

		/// <summary>
		/// normalisiert den Vektor auf die Länge 1
		/// </summary>
		public Vector2 Normalized
            {
            get { return this * (1 / this.Abs); }
            }

		/// <summary>
		/// normalisiert den Vektor v auf die Länge 1
		/// </summary>
		/// <param name="v">zu normalisierender Vektor</param>
		/// <returns>v / ||v||</returns>
        public static Vector2 Normalize(Vector2 v)
            {
            return v * (1 / v.Abs);
            }

		/// <summary>
		/// Sets this vector's coordinates to Max(this, v).
		/// </summary>
		/// <param name="v">other vector</param>
		public void Nibble(Vector2 v)
			{
			x = Math.Max(x, v.x);
			y = Math.Max(y, v.y);
			}

		/// <summary>
		/// formatiert den Vektor als String
		/// </summary>
		/// <returns>(x, y)</returns>
        public override string ToString()
            {
            return String.Format("({0}, {1})", x, y);
            }

        #region implizite/explizite Castings
		/// <summary>
		/// impliziter Cast in einen Point
		/// </summary>
		/// <param name="v">zu castender Vector2</param>
		/// <returns>einen Point mit den Koordinaten von v</returns>
        public static implicit operator System.Drawing.Point(Vector2 v)
            {
            return new System.Drawing.Point((int)v.X, (int)v.Y);
            }

		/// <summary>
		/// impliziter Cast in einen PointF
		/// </summary>
		/// <param name="v">zu castender Vector2</param>
		/// <returns>einen PointF mit den Koordinaten von v</returns>
		public static implicit operator System.Drawing.PointF(Vector2 v)
            {
			return new System.Drawing.PointF((float)v.X, (float)v.Y);
            }

		/// <summary>
		/// impliziter Cast in einen Size
		/// </summary>
		/// <param name="v">zu castender Vector2</param>
		/// <returns>Size mit den Dimensionen der Koordinaten von v</returns>
		public static implicit operator System.Drawing.Size(Vector2 v)
			{
			return new System.Drawing.Size((int)v.X, (int)v.Y);
			}

		/// <summary>
		/// impliziter Cast in einen SizeF
		/// </summary>
		/// <param name="v">zu castender Vector2</param>
		/// <returns>SizeF mit den Dimensionen der Koordinaten von v</returns>
		public static implicit operator System.Drawing.SizeF(Vector2 v)
			{
			return new System.Drawing.SizeF((float)v.X, (float)v.Y);
			}

		/// <summary>
		/// impliziter Cast aus einem Point
		/// </summary>
		/// <param name="p">Point aus dem gecastet werden soll</param>
		/// <returns>einen Vector2 mit den Koordinaten von p</returns>
		public static implicit operator Vector2(Point p)
            {
            return new Vector2(p);
            }

		/// <summary>
		/// impliziter Cast aus einem PointF
		/// </summary>
		/// <param name="p">PointF aus dem gecastet werden soll</param>
		/// <returns>einen Vector2 mit den Koordinaten von p</returns>
		public static implicit operator Vector2(PointF p)
			{
			return new Vector2(p);
			}
		#endregion

        /// <summary>
        /// Länge der Projektion von source auf dest
        /// </summary>
        public static double Project(Vector2 source, Vector2 dest)
            {
            return source * dest / dest.Abs;
            }

		/// <summary>
		/// berechnet die absolute euklidische Entfernung zwischen From und To
		/// </summary>
		/// <param name="From">erster Vektor</param>
		/// <param name="To">zweiter Vektor</param>
		/// <returns>(To-From).Abs</returns>
        public static double GetDistance(Vector2 From, Vector2 To)
            {
            Vector2 diff = To - From;
            return diff.Abs;
            }

		/// <summary>
		/// rundet den Vector2
		/// </summary>
		public void Round()
			{
			x = Math.Round(x);
			y = Math.Round(y);
			}


		/// <summary>
		/// gibt den kleineren der beiden Winkel zwischen den beiden Vektoren v1 und v2 zurück
		/// </summary>
		/// <param name="v1">Vektor 1</param>
		/// <param name="v2">Vektor 2</param>
		/// <returns>den Winkel kleiner pi zwischen v1 und v2</returns>
		public static double AngleBetween(Vector2 v1, Vector2 v2)
			{
			return Math.Acos((v1.x * v2.x + v1.y * v2.y) / (v1.Abs * v2.Abs));
			}
        }
    }
