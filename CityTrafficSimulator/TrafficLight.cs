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
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;

using CityTrafficSimulator.Timeline;

namespace CityTrafficSimulator
    {
	/// <summary>
	/// Kapselt eine Ampel und implementiert ein TimelineEntry
	/// </summary>
    [Serializable]
    public class TrafficLight : TimelineEntry, ISavable
        {
		/// <summary>
		/// Status einer Ampel (rot oder grün)
		/// </summary>
        public enum State {
			/// <summary>
			/// grüne Ampel
			/// </summary>
            GREEN, 

			/// <summary>
			/// rote Ampel
			/// </summary>
			RED
            }


		/// <summary>
		/// aktueller Status der Ampel
		/// </summary>
		private State _trafficLightState;
		/// <summary>
		/// aktueller Status der Ampel
		/// </summary>
		[XmlIgnore]
        public State trafficLightState
            {
            get { return _trafficLightState; }
			set { _trafficLightState = value; }
            }

		/// <summary>
		/// Liste von LineNodes denen dieses TrafficLight zugeordnet ist
		/// </summary>
		[XmlIgnore]
		private List<LineNode> _assignedNodes = new List<LineNode>();

		/// <summary>
		/// Liste von LineNodes denen dieses TrafficLight zugeordnet ist
		/// </summary>
		[XmlIgnore]
		public List<LineNode> assignedNodes
			{
			get { return _assignedNodes; }
			}

		#region Hashcodes

		/*
		 * Nachdem der ursprüngliche Ansatz zu Hashen zu argen Kollisionen geführt hat, nun eine verlässliche Methode für Kollisionsfreie Hashes 
		 * mittels eindeutiger IDs für jedes TrafficLight die über statisch Klassenvariablen vergeben werden
		 */

		/// <summary>
		/// Klassenvariable welche den letzten vergebenen hashcode speichert und bei jeder Instanziierung eines Objektes inkrementiert werden muss
		/// </summary>
		[XmlIgnore]
		private static int hashcodeIndex = 0;

		/// <summary>
		/// Hashcode des instanziierten Objektes
		/// </summary>
		public int hashcode = -1;

		/// <summary>
		/// gibt den Hashcode des Fahrzeuges zurück.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
			{
			return hashcode;
			}

		/// <summary>
		/// Setzt die statische Klassenvariable hashcodeIndex zurück. Achtung: darf nur in bestimmten Fällen aufgerufen werden.
		/// </summary>
		public static void ResetHashcodeIndex()
			{
			hashcodeIndex = 0;
			}

		#endregion
		
        #region Konstruktoren

		/// <summary>
		/// Konstruktor für TimelineEntry-Ampeln
		/// </summary>
		public TrafficLight()
			{
			hashcode = hashcodeIndex++;

			// Initial Event anlegen
			this.defaultAction = SwitchToRed;
			trafficLightState = State.RED;
			this.color = Color.Red;
			}
        #endregion


        #region Speichern/Laden
		/// <summary>
		/// DEPRECATED: Hash des Elternknotens (wird für Serialisierung gebraucht)
		/// </summary>
		[XmlIgnore]
		public int parentNodeHash = 0;

		/// <summary>
		/// Hashes der zugeordneten LineNodes
		/// </summary>
		public List<int> assignedNodesHashes = new List<int>();

		/// <summary>
		/// bereitet das TrafficLight auf die XML-Serialisierung vor.
		/// </summary>
        public override void PrepareForSave()
            {
			base.PrepareForSave();

			assignedNodesHashes.Clear();
			foreach (LineNode ln in _assignedNodes)
				{
				assignedNodesHashes.Add(ln.GetHashCode());
				}
            }
		/// <summary>
		/// stellt das TrafficLight nach einer XML-Deserialisierung wieder her
		/// </summary>
		/// <param name="saveVersion">Version der gespeicherten Datei</param>
		/// <param name="nodesList">Liste von allen existierenden LineNodes</param>
        public override void RecoverFromLoad(int saveVersion, List<LineNode> nodesList)
            {
			// Klassenvariable für Hashcode erhöhen um Kollisionen für zukünftige LineNodes zu verhindern
			if (hashcodeIndex <= hashcode)
				{
				hashcodeIndex = hashcode + 1;
				}

			// erstmal EventActions setzen
			this.defaultAction = SwitchToRed;
			foreach (TimelineEvent e in events)
				{
				e.eventStartAction = SwitchToGreen;
				e.eventEndAction = SwitchToRed;
				}

			// nun die assignedNodes aus der nodesList dereferenzieren
			foreach (int hash in assignedNodesHashes)
				{
				foreach (LineNode ln in nodesList)
					{
					if (ln.GetHashCode() == hash)
						{
						_assignedNodes.Add(ln);
						ln.tLight = this;
						break;
						}
					}
				}

			// Alte Versionen konnten nur einen Node pro TrafficLight haben und waren daher anders referenziert, auch darum wollen wir uns kümmern:
			if (saveVersion <= 2)
				{
				foreach (LineNode ln in nodesList)
					{
					if (ln.GetHashCode() == parentNodeHash)
						{
						AddAssignedLineNode(ln);
						break;
						}
					}
				}
			}
        #endregion

		/// <summary>
		/// meldet den LineNode ln bei diesem TrafficLight an, sodass es weiß das es diesem zugeordnet ist
		/// </summary>
		/// <param name="ln">anzumeldender LineNode</param>
		public void AddAssignedLineNode(LineNode ln)
			{
			_assignedNodes.Add(ln);
			ln.tLight = this;
			}

		/// <summary>
		/// meldet den LineNode ln bei diesem TrafficLight wieder ab, sodass es weiß, dass es diesem nicht mehr zugeordnet ist
		/// </summary>
		/// <param name="ln">abzumeldender LineNode</param>
		/// <returns>true, falls der Abmeldevorgang erfolgreich, sonst false</returns>
		public bool RemoveAssignedLineNode(LineNode ln)
			{
			if (ln != null)
				{
				ln.tLight = null;
				return _assignedNodes.Remove(ln);
				}
			return false;
			}

		/// <summary>
		/// stellt die Ampel auf grün
		/// </summary>
		public void SwitchToGreen()
			{
			this.trafficLightState = State.GREEN;
			}
		/// <summary>
		/// stellt die Ampel auf rot
		/// </summary>
		public void SwitchToRed()
			{
			this.trafficLightState = State.RED;
			}


		/// <summary>
		/// meldet das TrafficLight bei den zugeordneten LineNodes ab, sodas das TrafficLight gefahrlos gelöscht werden kann.
		/// </summary>
		public override void Dispose()
			{
			base.Dispose();

			while (_assignedNodes.Count > 0)
				{
				RemoveAssignedLineNode(_assignedNodes[0]);
				}
			}

		}
    }
