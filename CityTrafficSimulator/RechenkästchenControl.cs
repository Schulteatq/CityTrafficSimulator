/*
 *  CityTrafficSimulator - a tool to simulate traffic in urban areas and on intersections
 *  Copyright (C) 2005-2010, Christian Schulte zu Berge
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CityTrafficSimulator
    {
	/// <summary>
	/// Control welches ein Grid variabler Größe enthalten kann und Unterobjekte daran ausrichten kann
	/// </summary>
    public partial class RechenkaestchenControl : UserControl
        {
        #region Variablen und Properties
		/// <summary>
		/// Dimension des Grids
		/// </summary>
        private Point m_Dimension;
		/// <summary>
		/// Größe einer einzelnen Zelle
		/// </summary>
        private Size m_CellSize;
		/// <summary>
		/// Flag, ob das Grid gezeichnet werden soll
		/// </summary>
        private bool m_DrawGrid;

		/// <summary>
		/// Dimension des Grids
		/// </summary>
        public Point Dimension
            {
            get { return m_Dimension; }
            set 
                { 
                m_Dimension = value;
                Invalidate();
                }
            }

		/// <summary>
		/// Breite des Grids
		/// </summary>
        public int Max_X
            {
            get { return m_Dimension.X; }
            set 
                { 
                m_Dimension.X = value;
                Invalidate();
                }
            }
		/// <summary>
		/// Höhe des Grids
		/// </summary>
        public int Max_Y
            {
            get { return m_Dimension.Y; }
            set 
                {
                m_Dimension.Y = value;
                Invalidate();
                }
            }

		/// <summary>
		/// Dimension einer einzelnen Zelle
		/// </summary>
        public Size CellSize
            {
            get { return m_CellSize; }
            set 
                { 
                m_CellSize = value;
                Invalidate();
                }
            }
		/// <summary>
		/// Breite einer einzelnen Zelle
		/// </summary>
        public int CellWidth
            {
            get { return m_CellSize.Width; }
            set 
                {
                m_CellSize.Width = value;
                Invalidate();
                }
            }
		/// <summary>
		/// Höhe einer einzelnen Zelle
		/// </summary>
        public int CellHeight
            {
            get { return m_CellSize.Height; }
            set 
                {
                m_CellSize.Height = value;
                Invalidate();
                }
            }

		/// <summary>
		/// Flag, ob das Grid gezeichnet werden soll
		/// </summary>
        public bool DrawGrid
            {
            get { return m_DrawGrid; }
            set { m_DrawGrid = value; }
            }
	
        #endregion

        #region Prozeduren
		/// <summary>
		/// Standardkonstruktor, initialisiert das Grid als DoubleBuffered
		/// </summary>
        public RechenkaestchenControl()
            {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            if (DesignMode)
                {
                this.Dimension = new Point(10, 10);
                this.CellSize = new Size(16, 16);
                }

            }

		/// <summary>
		/// richtet den Punkt clientPosition am Raster aus
		/// </summary>
		/// <param name="clientPosition">auszurichtender Punkt</param>
		/// <param name="DockToGrid">Flag, ob überhaupt ausgerichtet werden soll</param>
		/// <returns>Falls DockToGrid==true: der Punkt auf dem Grid der clientPosition am nächsten ist, sonst clientPosition</returns>
        public Vector2 DockToGrid(Vector2 clientPosition, bool DockToGrid)
            {
            if (DockToGrid)
                {
                Vector2 toreturn = new Vector2((float)Math.Floor(clientPosition.X / this.CellWidth) * this.CellWidth, (float)Math.Floor(clientPosition.Y / this.CellHeight) * this.CellHeight);
                return toreturn;
                }
            else
                {
                return clientPosition;
                }
            }

        #endregion

        #region Override Prozeduren
		/// <summary>
		/// Override-Methode für IsInputKey
		/// </summary>
		/// <param name="keyData">keyData</param>
		/// <returns>true</returns>
        protected override bool IsInputKey(Keys keyData)
            {
            return true;
            }
        #endregion

        #region Event-Handler
		/// <summary>
		/// Paint-Methode. Falls DrawGrid==true wird hier ein Rechenkästchenraster gezeichnet
		/// </summary>
		/// <param name="sender">aufrufendes Objekt</param>
		/// <param name="e">PaintEvent-Argumente</param>
        private void RechenkaestchenControl_Paint(object sender, PaintEventArgs e)
            {
            // zunächst die ganze Geschichte weiß streichen
            using (Pen BackPen = new Pen(Color.White))
                {
                using (SolidBrush BackBrush = new SolidBrush(Color.White))
                    {
                    e.Graphics.FillRectangle(BackBrush, this.ClientRectangle);
                    e.Graphics.DrawRectangle(BackPen, this.ClientRectangle);

                    if (DrawGrid)
                        {
                        // nun das Gitternetz anbringen
                        Pen GridPen = new Pen(Color.LightGray);
                        GridPen.Width = 1;
                        // erst die Spalten
                        for (int i = 0; i <= this.Max_X; i++)
                            {
                            e.Graphics.DrawLine(GridPen, i * this.CellWidth, 0, i * this.CellWidth, this.CellHeight * this.Max_Y);
                            }
                        // nun die Zeilen
                        for (int i = 0; i <= this.Max_Y; i++)
                            {
                            e.Graphics.DrawLine(GridPen, 0, i * this.CellHeight, this.Max_X * this.CellWidth, i * this.CellHeight);
                            }

                        // aufräumen
                        GridPen.Dispose();
                        }
                    }
                } 

            }
        #endregion

        #region Subklassen
        #endregion
        }
    }
