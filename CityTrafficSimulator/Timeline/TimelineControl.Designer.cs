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

namespace CityTrafficSimulator
	{
	partial class TimelineControl
		{
		/// <summary> 
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
			{
			if (disposing && (components != null))
				{
				components.Dispose();
				}
			base.Dispose(disposing);
			}

		#region Vom Komponenten-Designer generierter Code

		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
			{
			this.SuspendLayout();
			// 
			// TimelineControl
			// 
			this.Size = new System.Drawing.Size(435, 147);
			this.MouseLeave += new System.EventHandler(this.TimelineControl_MouseLeave);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.TimelineControl_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TimelineControl_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TimelineControl_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TimelineControl_MouseUp);
			this.ResumeLayout(false);

			}

		#endregion
		}
	}
