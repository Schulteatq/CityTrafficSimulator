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
        private double _x;
		/// <summary>
		/// y-Koordinate
		/// </summary>
        private double _y;

		/// <summary>
		/// x-Koordinate
		/// </summary>
        public double X
            {
            get { return _x; }
            set { _x = value; }
            }

		/// <summary>
		/// y-Koordinate
		/// </summary>
        public double Y
            {
            get { return _y; }
            set { _y = value; }
            }

        #region Konstruktoren
		/// <summary>
		/// Standardkonstruktor - initialisiert den Vector2 mit (0,0)
		/// </summary>
        public Vector2()
            {
            this._x = 0;
            this._y = 0;
            }

		/// <summary>
		/// Konstruktor, initialisiert den Vector2 mit den übergebenen Parametern
		/// </summary>
		/// <param name="x">x-Koordinate</param>
		/// <param name="y">y-Koordinate</param>
        public Vector2(double x, double y)
            {
            this._x = x;
            this._y = y;
            }

		/// <summary>
		/// Konstruktor, initialisiert den Vector2 mit dem übergebenen PointF
		/// </summary>
		/// <param name="point">Punkt dessen Koordinaten den Vector2 initialisieren sollen</param>
        public Vector2(PointF point)
            {
            this._x = point.X;
            this._y = point.Y;
            }

		/// <summary>
		/// Konstruktor, initialisiert den Vector2 mit dem übergebenen Point
		/// </summary>
		/// <param name="point">Punkt dessen Koordinaten den Vector2 initialisieren sollen</param>
		public Vector2(Point point)
            {
            this._x = point.X;
            this._y = point.Y;
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
            return new Vector2(v._x * d, v._y * d);
            }

		/// <summary>
		/// Mal-Operator zwischen Skalar und Vektor 
		/// </summary>
		/// <param name="v">Vektor</param>
		/// <param name="d">Skalar</param>
		/// <returns>v*d</returns>
		public static Vector2 operator *(double d, Vector2 v)
            {
            return new Vector2(v._x * d, v._y * d);
            }

		/// <summary>
		/// Skalarprodukt zweier Vektoren
		/// </summary>
		/// <param name="v1">erster Vektor</param>
		/// <param name="v2">zweiter Vektor</param>
		/// <returns>Skalarprodukt: dot(v1, v2) </returns>
        public static double operator *(Vector2 v1, Vector2 v2)
            {
            return v1._x * v2._x + v1._y * v2._y;
            }

		/// <summary>
		/// Vektoraddition
		/// </summary>
		/// <param name="v1">erster Vektor</param>
		/// <param name="v2">zweiter Vektor</param>
		/// <returns>v1 + v2</returns>
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
            {
            return new Vector2(v1._x + v2._x, v1._y + v2._y);
            }

		/// <summary>
		/// Vektorsubtraktion
		/// </summary>
		/// <param name="v1">erster Vektor</param>
		/// <param name="v2">zweiter Vektor</param>
		/// <returns>v1 - v2</returns>
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
            {
            return new Vector2(v1._x - v2._x, v1._y - v2._y);
            }
        #endregion
        
		/// <summary>
		/// Absolutbetrag/Länge des Vektors
		/// </summary>
        public double Abs
            {
            get { return Math.Sqrt(_x * _x + _y * _y); }
            }

		/// <summary>
		/// gibt den Vektor um 90° im Uhrzeigersinn gedreht zurück
		/// </summary>
        public Vector2 RotatedClockwise
            {
            get { return new Vector2(_y, -1 * _x); }
            }

		/// <summary>
		/// gibt den Vektor um 90° im Uhrzeigersinn gedreht zurück
		/// </summary>
		public Vector2 RotatedCounterClockwise
			{
			get { return new Vector2(-_y, _x); }
			}

		/// <summary>
		/// gibt den Vektor um phi gegen den Urzeigersinn gedreht zurück
		/// </summary>
		/// <param name="phi">Winkel im Bogenmaß um den gedreht werden soll</param>
		/// <returns>Ein neuer Vektor2 der gegenüber this um Phi gegen den Urzeigersinn gedreht ist</returns>
		public Vector2 RotateCounterClockwise(double phi)
			{
			return new Vector2(Math.Cos(phi)*_x - Math.Sin(phi) * _y, Math.Sin(phi)*_x + Math.Cos(phi) * _y);
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
			_x = Math.Max(_x, v._x);
			_y = Math.Max(_y, v._y);
			}

		/// <summary>
		/// formatiert den Vektor als String
		/// </summary>
		/// <returns>(x, y)</returns>
        public override string ToString()
            {
            return String.Format("({0}, {1})", _x, _y);
            }

        #region implizite/explizite Castings
		/// <summary>
		/// impliziter Cast in einen Point
		/// </summary>
		/// <param name="v">zu castender Vector2</param>
		/// <returns>einen Point mit den Koordinaten von v</returns>
        public static implicit operator System.Drawing.Point(Vector2 v)
            {
            return new System.Drawing.Point((int)v._x, (int)v._y);
            }

		/// <summary>
		/// impliziter Cast in einen PointF
		/// </summary>
		/// <param name="v">zu castender Vector2</param>
		/// <returns>einen PointF mit den Koordinaten von v</returns>
		public static implicit operator System.Drawing.PointF(Vector2 v)
            {
			return new System.Drawing.PointF((float)v._x, (float)v._y);
            }

		/// <summary>
		/// impliziter Cast in einen Size
		/// </summary>
		/// <param name="v">zu castender Vector2</param>
		/// <returns>Size mit den Dimensionen der Koordinaten von v</returns>
		public static implicit operator System.Drawing.Size(Vector2 v)
			{
			return new System.Drawing.Size((int)v._x, (int)v._y);
			}

		/// <summary>
		/// impliziter Cast in einen SizeF
		/// </summary>
		/// <param name="v">zu castender Vector2</param>
		/// <returns>SizeF mit den Dimensionen der Koordinaten von v</returns>
		public static implicit operator System.Drawing.SizeF(Vector2 v)
			{
			return new System.Drawing.SizeF((float)v._x, (float)v._y);
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
			_x = Math.Round(_x);
			_y = Math.Round(_y);
			}


		/// <summary>
		/// gibt den kleineren der beiden Winkel zwischen den beiden Vektoren v1 und v2 zurück
		/// </summary>
		/// <param name="v1">Vektor 1</param>
		/// <param name="v2">Vektor 2</param>
		/// <returns>den Winkel kleiner pi zwischen v1 und v2</returns>
		public static double AngleBetween(Vector2 v1, Vector2 v2)
			{
			return Math.Acos((v1._x * v2._x + v1._y * v2._y) / (v1.Abs * v2.Abs));
			}

		/// <summary>
		/// Returns whether this vector is a zero vector
		/// </summary>
		/// <returns>X == 0 AND Y == 0</returns>
		public bool IsZeroVector()
			{
			return (_x == 0 && _y == 0);
			}
        }
    }
