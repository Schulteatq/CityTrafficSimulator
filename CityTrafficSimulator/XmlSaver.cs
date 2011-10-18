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
		/// <param name="fahrauftraege">Liste von Fahraufträgen</param>
		public static void SaveToFile(string filename, NodeSteuerung nodeSteuerung, TimelineSteuerung timelineSteuerung, List<Auftrag> fahrauftraege)
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
					xw.WriteString("4");
					xw.WriteEndAttribute();

					nodeSteuerung.SaveToFile(xw, xsn, fahrauftraege);
					timelineSteuerung.SaveToFile(xw, xsn);

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
		public static List<Auftrag> LoadFromFile(String filename, NodeSteuerung nodeSteuerung, TimelineSteuerung timelineSteuerung)
			{

			LoadingForm.LoadingForm lf = new LoadingForm.LoadingForm();
			lf.Text = "Laden von Datei '" + filename + "'...";
			lf.Show();

			lf.SetupUpperProgress("Lade Dokument...", 7);

			// Dokument laden
			XmlDocument xd = new XmlDocument();
			xd.Load(filename);

			lf.StepUpperProgress("Lade Layout...");

			List<Auftrag> toReturn = nodeSteuerung.LoadFromFile(xd, lf);

			lf.StepUpperProgress("Lade LSA...");

			timelineSteuerung.LoadFromFile(xd, nodeSteuerung.nodes, lf);

			lf.StepUpperProgress("fertig");

			lf.Close();
			lf = null;

			return toReturn;
			}
		}
	}
