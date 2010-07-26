using System;
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

		}
	}
