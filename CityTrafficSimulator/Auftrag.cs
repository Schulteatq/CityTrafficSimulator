/*
 *  CityTrafficSimulator - a tool to simulate traffic in urban areas and on intersections
 *  Copyright (C) 2005-2011, Christian Schulte zu Berge
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
using System.Xml.Serialization;

using CityTrafficSimulator.Vehicle;

namespace CityTrafficSimulator
    {
    /// <summary>
    /// Diese Klasse kapselt einen "Fahrtauftrag"
    /// Damit ist festgelegter Fartauftrag für Autos von LineNode nach LineNode in einer bestimmten Häufigkeit
    /// </summary>
	[Serializable]
    public class Auftrag : ITickable, ISavable
        {
		/// <summary>
		/// Zufallsgenerator
		/// </summary>
		private static Random rnd = new Random();


		/// <summary>
		/// Multiplikator für Fahrzeuge/Stunde
		/// </summary>
		public static decimal trafficDensityMultiplier = 1;

		
		/// <summary>
		/// DEPRECATED: Startknoten
		/// </summary>
		[XmlIgnore]
        public LineNode startNode;
		/// <summary>
		/// DEPRECATED: Endknoten
		/// </summary>
		[XmlIgnore]
        public LineNode endNode;

		/// <summary>
		/// Sammlung von Startknoten
		/// </summary>
		[XmlIgnore]
		public List<LineNode> startNodes = new List<LineNode>();

		/// <summary>
		/// Sammlung von Endknoten
		/// </summary>
		[XmlIgnore]
		public List<LineNode> endNodes = new List<LineNode>();

		/// <summary>
		/// Verkehrsdichten in Fahrzeuge/Stunde
		/// </summary>
		private int m_trafficDensity;
		/// <summary>
		/// Verkehrsdichten in Fahrzeuge/Stunde
		/// </summary>
		public int trafficDensity
			{
			get { return m_trafficDensity; }
			set 
				{ 
				m_trafficDensity = value;
				m_häufigkeit = 72000 / m_trafficDensity;
				}
			}

		/// <summary>
		/// Anzahl der Ticks zwischen zwei Fahrzeugspawns
		/// </summary>
		[XmlIgnore]
		private int m_häufigkeit;


		/// <summary>
		/// Wunschgeschwindigkeit der Fahrzeuge
		/// </summary>
		private int m_wunschgeschwindigkeit = 14;
		/// <summary>
		/// Wunschgeschwindigkeit der Fahrzeuge
		/// </summary>
		public int wunschgeschwindigkeit
			{
			get { return m_wunschgeschwindigkeit; }
			set { m_wunschgeschwindigkeit = value; }
			}

		/// <summary>
		/// Fahrzeugtyp
		/// </summary>
		private IVehicle.VehicleTypes m_vehicleType = IVehicle.VehicleTypes.CAR;
		/// <summary>
		/// Fahrzeugtyp
		/// </summary>
		public IVehicle.VehicleTypes vehicleType
			{
			get { return m_vehicleType; }
			set { m_vehicleType = value; }
			}

		/// <summary>
		/// Anteil LKW in %
		/// </summary>
		[XmlIgnore]
		public static int truckRatio = 8;

		/// <summary>
		/// Number of vehicles that were not created successfully
		/// </summary>
        private int failedCount;

        #region Konstruktoren
		/// <summary>
		/// Leerer Konstruktor (NICHT VERWENDEN! - NUR FÜR SERIALISIERUNG)
		/// </summary>
        public Auftrag()
            {
            }

		/// <summary>
		/// Standardkonstruktor
		/// </summary>
		/// <param name="vehicleType">Fahrzeugtyp</param>
		/// <param name="startNodes">Liste von Startknoten</param>
		/// <param name="endNodes">Liste von Zielknoten</param>
		/// <param name="trafficDensity">Häufigkeit in Ticks</param>
		public Auftrag(IVehicle.VehicleTypes vehicleType, List<LineNode> startNodes, List<LineNode> endNodes, int trafficDensity)
            {
			this.m_vehicleType = vehicleType;

			foreach (LineNode ln in startNodes)
				this.startNodes.Add(ln);
			foreach (LineNode ln in endNodes)
				this.endNodes.Add(ln);

			this.trafficDensity = trafficDensity;
            }
        #endregion


        #region ITickable Member

		/// <summary>
		/// lässt die Zeit um einen Tick voranschreiten
		/// </summary>
		/// <param name="tickLength">Länge eines Ticks in Sekunden (berechnet sich mit 1/#Ticks pro Sekunde)</param>
		public void Tick(double tickLength)
            {
			if (failedCount > 0)
				{
				bool success = CreateVehicle();
				if (success)
					--failedCount;
				}
			else
				{
				int zufallsvariable = 0;
				if (trafficDensityMultiplier != 0)
					zufallsvariable = rnd.Next(failedCount, Decimal.ToInt32((decimal)m_häufigkeit / trafficDensityMultiplier));
				else
					zufallsvariable = 1;

				if (zufallsvariable == 0)
					{
					bool success = CreateVehicle();
					if (!success)
						{
						++failedCount;
						}
					}
				}
            }


		/// <summary>
		/// sagt dem Objekt Bescheid, dass der Tick vorbei ist.
		/// (keinerlei wirkliche Funktion hier)
		/// </summary>
		public void Reset()
			{
			}

		#endregion

        /// <summary>
        /// Lässt ein Auto am startNode losfahren
        /// </summary>
		/// <returns>true, if Vehicle was successfully created - otherwise false</returns>
		private bool CreateVehicle()
            {
			IVehicle.Physics p = new IVehicle.Physics(m_wunschgeschwindigkeit, m_wunschgeschwindigkeit, 0);

			IVehicle v = null;
			switch (m_vehicleType)
				{
				case IVehicle.VehicleTypes.CAR:
					if (rnd.Next(100) < truckRatio)
						{
						v = new Truck(p);
						}
					else
						{
						v = new Car(p);
						}					
					break;
				case IVehicle.VehicleTypes.TRAM:
					v = new Tram(p);
					break;
				case IVehicle.VehicleTypes.BUS:
					v = new Bus(p);
					break;
				}

			// Neue Linie zum Weiterfahren bestimmen
			LineNode start = startNodes[rnd.Next(startNodes.Count)];

			if (start.nextConnections.Count > 0)
				{	
				int foo = rnd.Next(start.nextConnections.Count);

				IVehicle.State state = new IVehicle.State(start.nextConnections[foo], 0);
				v.state = state;
				v.targetNodes = endNodes;
				return start.nextConnections[foo].AddVehicle(v);
				}

			return false;
            }

		/// <summary>
		/// Gibt Informationen über den Fahrauftrag als String zurück
		/// </summary>
		/// <returns></returns>
        public override string ToString()
            {
			if (startNodes.Count > 0 && endNodes.Count > 0)
				{
				switch (m_vehicleType)
					{
					case IVehicle.VehicleTypes.CAR:
						return trafficDensity.ToString() + " Autos/h: " + startNodes[0].ToString() + " -> " + endNodes[0].ToString(); 
					case IVehicle.VehicleTypes.TRAM:
						return trafficDensity.ToString() + " Trams/h: " + startNodes[0].ToString() + " -> " + endNodes[0].ToString();
					case IVehicle.VehicleTypes.BUS:
						return trafficDensity.ToString() + " Busse/h: " + startNodes[0].ToString() + " -> " + endNodes[0].ToString();
					default:
						return "kein Fahrzeugtyp definiert?!";
					}
				}
			else
				return "Start-/Endknoten nicht dereferenziert!";
            }

		#region ISavable Member

		/// <summary>
		/// DEPRECATED!
		/// </summary>
		public int startNodeHash, endNodeHash;

		/// <summary>
		/// Hashwerte der Startknoten (nur für XML-Serialisierung benötigt)
		/// </summary>
		public List<int> startNodeHashes = new List<int>();
		/// <summary>
		/// Hashwerte der Endknoten (nur für XML-Serialisierung benötigt)
		/// </summary>
		public List<int> endNodeHashes = new List<int>();

		/// <summary>
		/// Bereitet den Fahrauftrag für die XML-Serialisierung vor
		/// </summary>
		public void PrepareForSave()
			{
			startNodeHash = -1;
			endNodeHash = -1;

			startNodeHashes.Clear();
			endNodeHashes.Clear();
			foreach (LineNode ln in startNodes)
				startNodeHashes.Add(ln.GetHashCode());
			foreach (LineNode ln in endNodes)
				endNodeHashes.Add(ln.GetHashCode());
			}

		/// <summary>
		/// Stellt die Referenzen auf LineNodes wieder her
		/// (auszuführen nach XML-Deserialisierung)
		/// </summary>
		/// <param name="saveVersion">Version der gespeicherten Datei</param>
		/// <param name="nodesList">Liste der bereits wiederhergestellten LineNodes</param>
		public void RecoverFromLoad(int saveVersion, List<LineNode> nodesList)
			{
			// Workaround um falsche Wunschgeschwindigkeiten aus alten Dateien zu korrigieren
			if (saveVersion < 1)
				{
				m_wunschgeschwindigkeit *= 2;
				}

			foreach (LineNode ln in nodesList)
				{
				if (startNodeHashes.Contains(ln.GetHashCode()))
					startNodes.Add(ln);
				else if (endNodeHashes.Contains(ln.GetHashCode()))
					endNodes.Add(ln);

				// dies hier ist nur zur Abwärtskompatibilität von alten Speicherständen:
				if (ln.GetHashCode() == startNodeHash)
					startNode = ln;
				else if (ln.GetHashCode() == endNodeHash)
					endNode = ln;
				}

			// dies hier ist nur zur Abwärtskompatibilität von alten Speicherständen:
			if (startNode != null && !startNodes.Contains(startNode))
				startNodes.Add(startNode);
			if (endNode != null && !endNodes.Contains(endNode))
				endNodes.Add(endNode);
			}

		#endregion
		}
    }
