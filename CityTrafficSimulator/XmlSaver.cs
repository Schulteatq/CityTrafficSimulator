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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;

using CityTrafficSimulator.LoadingForm;
using CityTrafficSimulator.Timeline;
using CityTrafficSimulator.Tools;

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
		/// <param name="ps">ProgramSettings</param>
		public static void SaveToFile(string filename, NodeSteuerung nodeSteuerung, TimelineSteuerung timelineSteuerung, Verkehr.VerkehrSteuerung trafficVolumeSteuerung, ProgramSettings ps)
			{
			try
				{
				// erstma nen XMLWriter machen
				XmlWriterSettings xws = new XmlWriterSettings();
				xws.Indent = true;
				xws.NewLineHandling = NewLineHandling.Entitize;

				XmlWriter xw = XmlWriter.Create(filename, xws);

				// leeren XmlSerializerNamespaces Namespace erstellen
				XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
				xsn.Add("", "");

				xw.WriteStartElement("CityTrafficSimulator");

					// write saveVersion
					xw.WriteStartAttribute("saveVersion");
					xw.WriteString("8");
					xw.WriteEndAttribute();

					// serialize program settings
					XmlSerializer xsPS = new XmlSerializer(typeof(ProgramSettings));
					xsPS.Serialize(xw, ps, xsn);

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
		public static ProgramSettings LoadFromFile(String filename, NodeSteuerung nodeSteuerung, TimelineSteuerung timelineSteuerung, Verkehr.VerkehrSteuerung trafficVolumeSteuerung)
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

			ProgramSettings ps;
			if (saveVersion >= 8)
				{
				XmlNode xnlLineNode = xd.SelectSingleNode("//CityTrafficSimulator/ProgramSettings");
				TextReader tr = new StringReader(xnlLineNode.OuterXml);
				XmlSerializer xsPS = new XmlSerializer(typeof(ProgramSettings));
				ps = (ProgramSettings)xsPS.Deserialize(tr);
				}
			else
				{
				// set some okay default settings
				ps = new ProgramSettings();

				ps._simSpeed = 1;
				ps._simSteps = 15;
				ps._simDuration = 300;
				ps._simRandomSeed = 42;

				ps._zoomLevel = 7;
				ps._renderQuality = 0;

				ps._renderStatistics = false;
				ps._renderVelocityMapping = false;
				ps._showFPS = false;

				ps._renderOptions = new NodeSteuerung.RenderOptions();
				ps._renderOptions.renderLineNodes = true;
				ps._renderOptions.renderNodeConnections = true;
				ps._renderOptions.renderVehicles = true;
				ps._renderOptions.performClipping = true;
				ps._renderOptions.clippingRect = new Rectangle(0, 0, 10000, 10000);
				ps._renderOptions.renderIntersections = false;
				ps._renderOptions.renderLineChangePoints = false;
				ps._renderOptions.renderLineNodeDebugData = false;
				ps._renderOptions.renderNodeConnectionDebugData = false;
				ps._renderOptions.renderVehicleDebugData = false;

				List<Color> tmp = new List<Color>();
				tmp.Add(Color.DarkRed);
				tmp.Add(Color.Yellow);
				tmp.Add(Color.DarkGreen);
				ps._velocityMappingColorMap = new Tools.ColorMap(tmp);
				}
			
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

			return ps;
			}
		}
	}
