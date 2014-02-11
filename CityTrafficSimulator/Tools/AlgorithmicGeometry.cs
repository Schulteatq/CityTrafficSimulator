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

﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace CityTrafficSimulator.Tools
	{
	/// <summary>
	/// Implements various algorithms from algorithmic geometry
	/// </summary>
	public class AlgorithmicGeometry
		{
		/// <summary>
		/// Computes a GraphicsPath for the convex hull of the given points.
		/// Convex hull will be inflated by <paramref name="radius"/> and contain rounded corners.
		/// </summary>
		/// <param name="input">Input points</param>
		/// <param name="radius">Radius for rounded corners</param>
		/// <returns>A GraphicsPath of the convex hull with rounded corners</returns>
		public static GraphicsPath roundedConvexHullPath(List<Vector2> input, double radius)
			{
			GraphicsPath toReturn = new GraphicsPath();

			// build convex hull
			LinkedList<Vector2> convexHull = AlgorithmicGeometry.convexHull(input);
			if (convexHull.Count == 0)
				{
				return toReturn;
				}				
			else if (convexHull.Count == 1)
				{
				toReturn.AddEllipse((float)(convexHull.First.Value.X - radius), (float)(convexHull.First.Value.Y - radius), (float)(2 * radius), (float)(2 * radius));
				return toReturn;
				}

			// add the first two points again for easier building of the GraphicsPath
			convexHull.AddLast(convexHull.First.Value);
			convexHull.AddLast(convexHull.First.Next.Value);

			// build GraphicsPath
			LinkedListNode<Vector2> curNode = convexHull.First;
			while (curNode.Next.Next != null)
				{
				// compute orthogonal vectors to point-to-point directions
				Vector2 ortho = (curNode.Next.Value - curNode.Value).RotatedClockwise.Normalized * radius;
				Vector2 nextOrtho = (curNode.Next.Next.Value - curNode.Next.Value).RotatedClockwise.Normalized * radius;

				// compute angles for rounded corners
				float start = (float)(Math.Atan2(ortho.Y, ortho.X) * 180 / Math.PI);
				float end = (float)(Math.Atan2(nextOrtho.Y, nextOrtho.X) * 180 / Math.PI);
				float sweep = end - start;
				if (sweep < 0)
					sweep += 360;

				toReturn.AddLine(curNode.Value + ortho, curNode.Next.Value + ortho);
				toReturn.AddArc((float)(curNode.Next.Value.X - radius), (float)(curNode.Next.Value.Y - radius), (float)(2 * radius), (float)(2 * radius), start, sweep);

				curNode = curNode.Next;
				}

			return toReturn;
			}

		/// <summary>
		/// Computes the convex hull of the given points in <paramref name="input"/>.
		/// </summary>
		/// <param name="input">List of points</param>
		/// <returns>List of the vertices of the convex hull of <paramref name="input"/>.</returns>
		public static LinkedList<Vector2> convexHull(List<Vector2> input)
			{
			// perform copy of input list to work on
			List<Vector2> points = new List<Vector2>(input);

			// make sure point list is well defined
			LinkedList<Vector2> toReturn = new LinkedList<Vector2>();
			if (points.Count < 3)
				{
				foreach (Vector2 p in points)
					toReturn.AddLast(p);
				return toReturn;
				}

			// find point with minimum Y coordinate
			Vector2 yMin = points[0];
			foreach (Vector2 p in points)
				{
				if (p.Y < yMin.Y)
					yMin = p;
				}

			// sort points in toReturn by polar angle around yMin
			points.Sort(delegate(Vector2 left, Vector2 right) 
				{
				if (left == right)
					return 0;
				if (left == yMin)
					return -1;
				if (right == yMin)
					return 1;

				double o = orientation(yMin, left, right);
				if (o > 0)
					return -1;
				else if (o == 0)
					return (left.Y >= yMin.Y && left.X <= right.Y) ? -1 : 1;						
				return 1;
				});

			// add first three points and start Graham's scan
			toReturn.AddLast(points[0]);
			toReturn.AddLast(points[1]);
			toReturn.AddLast(points[2]);

			// do Graham's scan
			for (int i = 3; i < points.Count; ++i)
				{
				while (!leftTurn(toReturn.Last.Previous.Value, toReturn.Last.Value, points[i]))
					toReturn.RemoveLast();
				toReturn.AddLast(points[i]);
				}

			return toReturn;
			}

		/// <summary>
		/// Computes the orientation of the three points a, b, c.
		/// </summary>
		/// <param name="a">First point</param>
		/// <param name="b">Second point</param>
		/// <param name="c">Third point</param>
		/// <returns>Left turn: &lt; 0, collinear: = 0, right turn: &gt; 0</returns>
		public static double orientation(Vector2 a, Vector2 b, Vector2 c)
			{
			return a.X * b.Y + a.Y * c.X + b.X * c.Y - c.X * b.Y - c.Y * a.X - b.X * a.Y;
			}

		/// <summary>
		/// Checks whether the three points are collinear.
		/// </summary>
		/// <param name="a">First point</param>
		/// <param name="b">Second point</param>
		/// <param name="c">Third point</param>
		/// <returns>True, when a, b, c are collinear.</returns>
		public static bool collinear(Vector2 a, Vector2 b, Vector2 c)
			{
			return (orientation(a, b, c) == 0);
			}

		/// <summary>
		/// Checks whether the three points build a left turn.
		/// </summary>
		/// <param name="a">First point</param>
		/// <param name="b">Second point</param>
		/// <param name="c">Third point</param>
		/// <returns>True, when a, b, c build a left turn.</returns>
		public static bool leftTurn(Vector2 a, Vector2 b, Vector2 c)
			{
			return (orientation(a, b, c) > 0);
			}

		/// <summary>
		/// Checks whether the three points build a right turn.
		/// </summary>
		/// <param name="a">First point</param>
		/// <param name="b">Second point</param>
		/// <param name="c">Third point</param>
		/// <returns>True, when a, b, c build a right turn.</returns>
		public static bool rightTurn(Vector2 a, Vector2 b, Vector2 c)
			{
			return (orientation(a, b, c) < 0);
			}

		/// <summary>
		/// Checks whether point <paramref name="d"/> is within the circle defined by a, b, c.
		/// </summary>
		/// <param name="a">First point of circle</param>
		/// <param name="b">Second point of circle</param>
		/// <param name="c">Third point of circle</param>
		/// <param name="d">Point to check</param>
		/// <returns>True, when d is within the circle of a, b, c.</returns>
		public static bool inCircle(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
			{
			double det = 0;
			det += (a.X - d.X) * (b.Y - d.Y) * ((c.X - d.X) * (c.X - d.X) + (c.Y - d.Y) * (c.Y - d.Y));
			det += (a.Y - d.Y) * ((b.X - d.X) * (b.X - d.X) + (b.Y - d.Y) * (b.Y - d.Y)) * (c.X - d.X);
			det += ((a.X - d.X) * (a.X - d.X) + (a.Y - d.Y) * (a.Y - d.Y)) * (b.X - d.X) * (c.Y - d.Y);

			det -= (c.X - d.X) * (b.Y - d.Y) * ((a.X - d.X) * (a.X - d.X) + (a.Y - d.Y) * (a.Y - d.Y));
			det -= (c.Y - d.Y) * ((b.X - d.X) * (b.X - d.X) + (b.Y - d.Y) * (b.Y - d.Y)) * (a.X - d.X);
			det -= ((c.X - d.X) * (c.X - d.X) + (c.Y - d.Y) * (c.Y - d.Y)) * (b.X - d.X) * (a.Y - d.Y);

			return (det < 0);
			}
		}
	}
