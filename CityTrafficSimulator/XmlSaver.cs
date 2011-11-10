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
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;

using CityTrafficSimulator.Timeline;
using CityTrafficSimulator.LoadingForm;

namespace CityTrafficSimulator
	{
	/// <summary>
	/// Speichert/Lädt den aktuellen Zustand der nodeSteuerung und timelineSteuerung in/aus eine(r) XML-Datei
	/// </summary>
	public static class XmlSaver
		{
		/// <summary>
		/// Speichert den aktuellen Zustand der nodeSteuerung und timelineSteuerung in die XML-Datei filename
		/// </summary>
		/// <param name="filename">Dateiname der zu speichernden Datei</param>
		/// <param name="nodeSteuerung">NodeSteuerung</param>
		/// <param name="timelineSteuerung">TimelineSteuerung</param>
		/// <param name="trafficVolumeSteuerung">VerkehrSteuerung</param>
		public static void SaveToFile(string filename, NodeSteuerung nodeSteuerung, TimelineSteuerung timelineSteuerung, Verkehr.VerkehrSteuerung trafficVolumeSteuerung)
			{
			try
				{
				// erstma nen XMLWriter machen
				XmlWriter xw = XmlWriter.Create(filename);

				// leeren XmlSerializerNamespaces Namespace erstellen
				XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
				xsn.Add("", "");

				xw.WriteStartElement("CityTrafficSimulator");

					// saveVersion schreiben
					xw.WriteStartAttribute("saveVersion");
					xw.WriteString("5");
					xw.WriteEndAttribute();

					nodeSteuerung.SaveToFile(xw, xsn);
					timelineSteuerung.SaveToFile(xw, xsn);
					trafficVolumeSteuerung.SaveToFile(xw, xsn);

				xw.WriteEndElement();

				xw.Close();

				}
			catch (IOException e)
				{
				MessageBox.Show(e.Message);
				throw;
				}
			}


		/// <summary>
		/// Läd eine XML Datei und versucht daraus den gespeicherten Zustand wiederherzustellen
		/// </summary>
		/// <param name="filename">Dateiname der zu ladenden Datei</param>
		/// <param name="nodeSteuerung">NodeSteuerung in das Layout eingelesen werden soll</param>
		/// <param name="timelineSteuerung">TimelineSteuerung in die die LSA eingelesen werden soll</param>
		/// <param name="trafficVolumeSteuerung">VerkehrSteurung to load into</param>
		public static List<Auftrag> LoadFromFile(String filename, NodeSteuerung nodeSteuerung, TimelineSteuerung timelineSteuerung, Verkehr.VerkehrSteuerung trafficVolumeSteuerung)
			{
			LoadingForm.LoadingForm lf = new LoadingForm.LoadingForm();
			lf.Text = "Loading file '" + filename + "'...";
			lf.Show();

			lf.SetupUpperProgress("Loading Document...", 8);

			// Dokument laden
			XmlDocument xd = new XmlDocument();
			xd.Load(filename);

			// parse save file version
			int saveVersion = 0;
			XmlNode mainNode = xd.SelectSingleNode("//CityTrafficSimulator");
			XmlNode saveVersionNode = mainNode.Attributes.GetNamedItem("saveVersion");
			if (saveVersionNode != null)
				saveVersion = Int32.Parse(saveVersionNode.Value);
			else
				saveVersion = 0; 
			
			lf.StepUpperProgress("Parsing Network Layout...");
			List<Auftrag> toReturn = nodeSteuerung.LoadFromFile(xd, lf);

			lf.StepUpperProgress("Parsing Singnals...");
			timelineSteuerung.LoadFromFile(xd, nodeSteuerung.nodes, lf);

			lf.StepUpperProgress("Parsing Traffic Volume...");
			trafficVolumeSteuerung.LoadFromFile(xd, nodeSteuerung.nodes, lf);
			if (saveVersion < 5)
				{
				trafficVolumeSteuerung.ImportOldTrafficVolumeData(toReturn);
				}

			lf.StepUpperProgress("Done");
			lf.ShowLog();

			lf.Close();
			lf = null;

			return toReturn;
			}
		}
	}
