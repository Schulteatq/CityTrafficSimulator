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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CityTrafficSimulator.LoadingForm
	{
	/// <summary>
	/// Sehr simples Formular zur Fortschrittsanzeige des Ladevorganges
	/// </summary>
	public partial class LoadingForm : Form
		{
		/// <summary>
		/// Log messages
		/// </summary>
		private StringBuilder logMessages = new StringBuilder();

		/// <summary>
		/// Number of logged messages
		/// </summary>
		private int numLogs = 0;

		/// <summary>
		/// Standardkonstruktor
		/// </summary>
		public LoadingForm()
			{
			InitializeComponent();
			}

		/// <summary>
		/// Aktualisiert die Anzeige
		/// </summary>
		public override void Refresh()
			{
			// ja es ist eigentlich Bullshit den Programmablauf aufzuhalten, nur um eine richtige 
			// Fortschrittsanzeige zu haben, aber form follows function
			System.Threading.Thread.Sleep(40);
			base.Refresh();
			}

		/// <summary>
		/// setzt die Anzahl der Schritte für den upperProgress
		/// </summary>
		/// <param name="stepName">Beschreibung des aktuellen Schrittes</param>
		/// <param name="stepCount">Anzahl der insgesamt benötigten Schritte im upperProgress</param>
		public void SetupUpperProgress(string stepName, int stepCount)
			{
			upperLabel.Text = stepName;
			upperProgress.Maximum = stepCount;
			upperProgress.Value = 0;

			Refresh();
			}

		/// <summary>
		/// setzt den upperProgress einen Schritt weiter
		/// </summary>	
		/// <param name="stepName">Beschreibung des aktuellen Schrittes</param>
		public void StepUpperProgress(string stepName)
			{
			upperLabel.Text = stepName;
			upperProgress.PerformStep();

			Refresh();
			}

		/// <summary>
		/// setzt den upperProgress einen Schritt weiter
		/// </summary>	
		public void StepUpperProgress()
			{
			upperProgress.PerformStep();

			Refresh();
			}


		/// <summary>
		/// setzt die Beschreibung und Anzahl Schritte für den lowerProgress
		/// </summary>
		/// <param name="stepName">Beschreibung des aktuellen Vorgangs</param>
		/// <param name="stepCount">Anzahl ingesamt benötigter Schritte im lowerProgress</param>
		public void SetupLowerProgess(string stepName, int stepCount)
			{
			StepUpperProgress();

			lowerLabel.Text = stepName;
			lowerProgress.Maximum = stepCount;
			lowerProgress.Value = 0;
			lowerLabel.Refresh();
			lowerProgress.Refresh();
			}

		/// <summary>
		/// setzt den lowerProgress einen Schritt weiter
		/// </summary>
		public void StepLowerProgress()
			{
			lowerProgress.PerformStep();
			lowerProgress.Refresh();
			}

		/// <summary>
		/// Append the given message to the log.
		/// </summary>
		/// <param name="message">message to log</param>
		public void Log(string message)
			{
			logMessages.AppendLine(message);
			++numLogs;
			}

		/// <summary>
		/// Shows a dialog with all logged messages
		/// </summary>
		public void ShowLog()
			{
			if (numLogs > 0)
				{
				MessageBox.Show(numLogs.ToString() + " error(s) occured:\n" + logMessages.ToString());
				}
			}

		}
	}
