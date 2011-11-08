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

			lf.Close();
			lf = null;

			return toReturn;
			}
		}
	}
