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

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CityTrafficSimulator.Vehicle
	{
	class Tram : IVehicle
		{
		private GraphicsPath[] wagons = new GraphicsPath[5];

		public Tram(IVehicle.Physics p)
			{
			length = (GlobalRandom.Instance.Next(2) == 0) ? 250 : 400;

			_physics = p;
			color = Color.FromArgb(GlobalRandom.Instance.Next(256), GlobalRandom.Instance.Next(256), GlobalRandom.Instance.Next(256));

			// maximale Beschleunigung
			a = 1.0;

			// komfortable Bremsverzögerung
			b = 1.0;

			_vehicleType = VehicleTypes.TRAM;
			}

		/// <summary>
		/// Berechnet die Absolute Position zur Bogenlängenposition pos.
		/// Kann auch mit negativen Positionsangaben umgehen
		/// </summary>
		/// <param name="pos">Bogenlängenposition</param>
		/// <returns>Weltkoordinaten zu Bogenlängenposition pos</returns>
		private Vector2 GetPositionAbsAtArcPos(double pos)
			{
			if (pos >= 0)
				return currentNodeConnection.lineSegment.AtPosition(pos);
			else
				{
				double remainingDistance = -pos;
				foreach (NodeConnection nc in visitedNodeConnections)
					{
					if (nc.lineSegment.length < remainingDistance)
						{
						remainingDistance -= nc.lineSegment.length;
						continue;
						}
					else
						{
						return nc.lineSegment.AtPosition(nc.lineSegment.length - remainingDistance);
						}
					}
				}
			return state.positionAbs;
			}

		/// <summary>
		/// Generates the GraphicsPath for rendering the vehicle. Should be overloaded by subclasses with distinct rendering.
		/// </summary>
		/// <returns>A GraphicsPath in world coordinates for rendering the vehicle at the current position.</returns>
		protected override GraphicsPath BuildGraphicsPath()
			{
			GraphicsPath toReturn = new GraphicsPath();

			// 25-Meter-Zug
			if (_length == 250)
				{
				Vector2[] positions = {
					state.positionAbs,
					GetPositionAbsAtArcPos(currentPosition - 95),
					GetPositionAbsAtArcPos(currentPosition - 100),
					GetPositionAbsAtArcPos(currentPosition - 150),
					GetPositionAbsAtArcPos(currentPosition - 155),
					GetPositionAbsAtArcPos(currentPosition - 250)
									  };
				Vector2[] normals = {
					(positions[0] - positions[1]).RotatedClockwise.Normalized,
					(positions[2] - positions[3]).RotatedClockwise.Normalized,
					(positions[4] - positions[5]).RotatedClockwise.Normalized
									  };

				PointF[] frontPoints = 
					{ 
					positions[0]  -  11*normals[0],
					positions[0]  +  11*normals[0],
					positions[1]  +  11*normals[0],
					positions[1]  -  11*normals[0],
					};
				PointF[] midPoints = 
					{ 
					positions[2]  -  11*normals[1],
					positions[2]  +  11*normals[1],
					positions[3]  +  11*normals[1],
					positions[3]  -  11*normals[1],
					};

				PointF[] backPoints = 
					{ 
					positions[4]  -  11*normals[2],
					positions[4]  +  11*normals[2],
					positions[5]  +  11*normals[2],
					positions[5]  -  11*normals[2],
					};

				if (positions[0] != positions[1])
					toReturn.AddPolygon(frontPoints);
				if (positions[2] != positions[3])
					toReturn.AddPolygon(midPoints);
				if (positions[4] != positions[5])
					toReturn.AddPolygon(backPoints);
				}
			// 40-Meter-Zug
			else
				{
				Vector2[] positions = {
					state.positionAbs,
					GetPositionAbsAtArcPos(currentPosition - 95),
					GetPositionAbsAtArcPos(currentPosition - 100),
					GetPositionAbsAtArcPos(currentPosition - 150),
					GetPositionAbsAtArcPos(currentPosition - 155),
					GetPositionAbsAtArcPos(currentPosition - 250),
					GetPositionAbsAtArcPos(currentPosition - 255),
					GetPositionAbsAtArcPos(currentPosition - 300),
					GetPositionAbsAtArcPos(currentPosition - 305),
					GetPositionAbsAtArcPos(currentPosition - 400)
									  };
				Vector2[] normals = {
					(positions[0] - positions[1]).RotatedClockwise.Normalized,
					(positions[2] - positions[3]).RotatedClockwise.Normalized,
					(positions[4] - positions[5]).RotatedClockwise.Normalized,
					(positions[6] - positions[7]).RotatedClockwise.Normalized,
					(positions[7] - positions[9]).RotatedClockwise.Normalized
									  };

				PointF[] firstPoints = 
					{ 
					positions[0]  -  11*normals[0],
					positions[0]  +  11*normals[0],
					positions[1]  +  11*normals[0],
					positions[1]  -  11*normals[0],
					};
				PointF[] secondPoints = 
					{ 
					positions[2]  -  11*normals[1],
					positions[2]  +  11*normals[1],
					positions[3]  +  11*normals[1],
					positions[3]  -  11*normals[1],
					};

				PointF[] thirdPoints = 
					{ 
					positions[4]  -  11*normals[2],
					positions[4]  +  11*normals[2],
					positions[5]  +  11*normals[2],
					positions[5]  -  11*normals[2],
					};

				PointF[] forthPoints = 
					{ 
					positions[6]  -  11*normals[3],
					positions[6]  +  11*normals[3],
					positions[7]  +  11*normals[3],
					positions[7]  -  11*normals[3],
					};

				PointF[] fifthPoints = 
					{ 
					positions[8]  -  11*normals[4],
					positions[8]  +  11*normals[4],
					positions[9]  +  11*normals[4],
					positions[9]  -  11*normals[4],
					};

				if ((positions[0] - positions[1]).IsNotZeroVector())
					toReturn.AddPolygon(firstPoints);
				if ((positions[2] - positions[3]).IsNotZeroVector())
					toReturn.AddPolygon(secondPoints);
				if ((positions[4] - positions[5]).IsNotZeroVector())
					toReturn.AddPolygon(thirdPoints);
				if ((positions[6] - positions[7]).IsNotZeroVector())
					toReturn.AddPolygon(forthPoints);
				if ((positions[8] - positions[9]).IsNotZeroVector())
					toReturn.AddPolygon(fifthPoints);
				}

			return toReturn;
			}

		/// <summary>
		/// Prüfe, ob ich auf der NodeConnection nc fahren darf
		/// </summary>
		/// <param name="nc">zu prüfende NodeConnection</param>
		/// <returns>nc.tramAllowed</returns>
		public override bool CheckNodeConnectionForSuitability(NodeConnection nc)
			{
			return nc.tramAllowed;
			}
		}
	}
