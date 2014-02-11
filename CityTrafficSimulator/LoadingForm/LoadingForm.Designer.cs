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

﻿namespace CityTrafficSimulator.LoadingForm
	{
	partial class LoadingForm
		{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
			{
			if (disposing && (components != null))
				{
				components.Dispose();
				}
			base.Dispose(disposing);
			}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
			{
			this.upperProgress = new System.Windows.Forms.ProgressBar();
			this.upperLabel = new System.Windows.Forms.Label();
			this.lowerLabel = new System.Windows.Forms.Label();
			this.lowerProgress = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// upperProgress
			// 
			this.upperProgress.Location = new System.Drawing.Point(12, 42);
			this.upperProgress.Maximum = 8;
			this.upperProgress.Name = "upperProgress";
			this.upperProgress.Size = new System.Drawing.Size(429, 23);
			this.upperProgress.Step = 1;
			this.upperProgress.TabIndex = 0;
			// 
			// upperLabel
			// 
			this.upperLabel.Location = new System.Drawing.Point(12, 9);
			this.upperLabel.Name = "upperLabel";
			this.upperLabel.Size = new System.Drawing.Size(429, 30);
			this.upperLabel.TabIndex = 1;
			this.upperLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lowerLabel
			// 
			this.lowerLabel.Location = new System.Drawing.Point(12, 68);
			this.lowerLabel.Name = "lowerLabel";
			this.lowerLabel.Size = new System.Drawing.Size(429, 30);
			this.lowerLabel.TabIndex = 3;
			this.lowerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lowerProgress
			// 
			this.lowerProgress.Location = new System.Drawing.Point(12, 101);
			this.lowerProgress.Name = "lowerProgress";
			this.lowerProgress.Size = new System.Drawing.Size(429, 23);
			this.lowerProgress.Step = 1;
			this.lowerProgress.TabIndex = 2;
			// 
			// LoadingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(453, 155);
			this.ControlBox = false;
			this.Controls.Add(this.lowerLabel);
			this.Controls.Add(this.lowerProgress);
			this.Controls.Add(this.upperLabel);
			this.Controls.Add(this.upperProgress);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "LoadingForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Laden...";
			this.TopMost = true;
			this.ResumeLayout(false);

			}

		#endregion

		/// <summary>
		/// Beschriftung des oberen Fortschrittsbalkens
		/// </summary>
		public System.Windows.Forms.Label upperLabel;
		/// <summary>
		/// Beschriftung des unteren Fortschrittsbalkens
		/// </summary>
		public System.Windows.Forms.Label lowerLabel;
		/// <summary>
		/// unterer Fortschrittsbalken
		/// </summary>
		public System.Windows.Forms.ProgressBar lowerProgress;
		/// <summary>
		/// oberer Fortschrittsbalken
		/// </summary>
		public System.Windows.Forms.ProgressBar upperProgress;

		}
	}