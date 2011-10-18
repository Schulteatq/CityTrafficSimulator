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

namespace CityTrafficSimulator
    {
	/// <summary>
	/// Hauptformular von CityTrafficSimulator
	/// </summary>
    partial class MainForm
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
            {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.layoutTabPage = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.titleEdit = new System.Windows.Forms.TextBox();
			this.infoEdit = new System.Windows.Forms.TextBox();
			this.NodeConnectionSetupGroupBox = new System.Windows.Forms.GroupBox();
			this.enableIncomingLineChangeCheckBox = new System.Windows.Forms.CheckBox();
			this.enableOutgoingLineChangeCheckBox = new System.Windows.Forms.CheckBox();
			this.tramAllowedCheckBox = new System.Windows.Forms.CheckBox();
			this.busAllowedCheckBox = new System.Windows.Forms.CheckBox();
			this.carsAllowedCheckBox = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.nodeConnectionPrioritySpinEdit = new System.Windows.Forms.NumericUpDown();
			this.LineNodeSetupGroupBox = new System.Windows.Forms.GroupBox();
			this.verkehrTabPage = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.trafficDensityMultiplierSpinEdit = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.button3 = new System.Windows.Forms.Button();
			this.vehicleTypeComboBox = new System.Windows.Forms.ComboBox();
			this.carGroupBox = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.truckRatioSpinEdit = new System.Windows.Forms.NumericUpDown();
			this.addBusButton = new System.Windows.Forms.Button();
			this.addTramButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.v0Edit = new System.Windows.Forms.NumericUpDown();
			this.addCarButton = new System.Windows.Forms.Button();
			this.SetEndNodeButton = new System.Windows.Forms.Button();
			this.SetStartNodeButton = new System.Windows.Forms.Button();
			this.HäufigkeitSpinEdit = new System.Windows.Forms.NumericUpDown();
			this.AuftragLöschenButton = new System.Windows.Forms.Button();
			this.NeuerAuftragButton = new System.Windows.Forms.Button();
			this.AufträgeCheckBox = new System.Windows.Forms.CheckedListBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.canvasHeigthSpinEdit = new System.Windows.Forms.NumericUpDown();
			this.canvasWidthSpinEdit = new System.Windows.Forms.NumericUpDown();
			this.HintergrundbildGroupBox = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.backgroundImageScalingSpinEdit = new System.Windows.Forms.NumericUpDown();
			this.clearBackgroudnImageButton = new System.Windows.Forms.Button();
			this.BildLadenButton = new System.Windows.Forms.Button();
			this.backgroundImageEdit = new System.Windows.Forms.TextBox();
			this.AnsichtGroupBox = new System.Windows.Forms.GroupBox();
			this.label13 = new System.Windows.Forms.Label();
			this.renderQualityComboBox = new System.Windows.Forms.ComboBox();
			this.visualizationCheckBox = new System.Windows.Forms.CheckBox();
			this.drawNodeConnectionsCheckBox = new System.Windows.Forms.CheckBox();
			this.zoomComboBox = new System.Windows.Forms.ComboBox();
			this.drawDebugCheckBox = new System.Windows.Forms.CheckBox();
			this.dockToGridcheckBox = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.thumbTabPage = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.stepsPerSecondSpinEdit = new System.Windows.Forms.NumericUpDown();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.simulationSpeedSpinEdit = new System.Windows.Forms.NumericUpDown();
			this.SpeichernButton = new System.Windows.Forms.Button();
			this.aboutBoxButton = new System.Windows.Forms.Button();
			this.LadenButton = new System.Windows.Forms.Button();
			this.findLineChangePointsButton = new System.Windows.Forms.Button();
			this.timelinePanel = new System.Windows.Forms.Panel();
			this.lockNodesCheckBox = new System.Windows.Forms.CheckBox();
			this.killAllVehiclesButton = new System.Windows.Forms.Button();
			this.stepButton = new System.Windows.Forms.Button();
			this.timerOnCheckBox = new System.Windows.Forms.CheckBox();
			this.statusleiste = new System.Windows.Forms.StatusStrip();
			this.statusMenu = new System.Windows.Forms.ToolStripDropDownButton();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.freeNodeButton = new System.Windows.Forms.Button();
			this.showEditorButton = new System.Windows.Forms.Button();
			this.DaGrid = new CityTrafficSimulator.RechenkaestchenControl();
			this.trafficLightTreeView = new CityTrafficSimulator.Timeline.TrafficLightTreeView(this.components);
			this.thumbGrid = new CityTrafficSimulator.RechenkaestchenControl();
			this.timeline = new CityTrafficSimulator.TimelineControl();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.layoutTabPage.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.NodeConnectionSetupGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nodeConnectionPrioritySpinEdit)).BeginInit();
			this.LineNodeSetupGroupBox.SuspendLayout();
			this.verkehrTabPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trafficDensityMultiplierSpinEdit)).BeginInit();
			this.carGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.truckRatioSpinEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.v0Edit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.HäufigkeitSpinEdit)).BeginInit();
			this.tabPage2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.canvasHeigthSpinEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.canvasWidthSpinEdit)).BeginInit();
			this.HintergrundbildGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.backgroundImageScalingSpinEdit)).BeginInit();
			this.AnsichtGroupBox.SuspendLayout();
			this.thumbTabPage.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.stepsPerSecondSpinEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simulationSpeedSpinEdit)).BeginInit();
			this.timelinePanel.SuspendLayout();
			this.statusleiste.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Interval = 67;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer2.Size = new System.Drawing.Size(964, 652);
			this.splitContainer2.SplitterDistance = 512;
			this.splitContainer2.TabIndex = 2;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.AutoScroll = true;
			this.splitContainer1.Panel1.CausesValidation = false;
			this.splitContainer1.Panel1.Controls.Add(this.DaGrid);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer1.Size = new System.Drawing.Size(964, 512);
			this.splitContainer1.SplitterDistance = 655;
			this.splitContainer1.TabIndex = 2;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.layoutTabPage);
			this.tabControl1.Controls.Add(this.verkehrTabPage);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.thumbTabPage);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(305, 512);
			this.tabControl1.TabIndex = 11;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// layoutTabPage
			// 
			this.layoutTabPage.Controls.Add(this.groupBox3);
			this.layoutTabPage.Controls.Add(this.NodeConnectionSetupGroupBox);
			this.layoutTabPage.Controls.Add(this.LineNodeSetupGroupBox);
			this.layoutTabPage.Location = new System.Drawing.Point(4, 22);
			this.layoutTabPage.Name = "layoutTabPage";
			this.layoutTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.layoutTabPage.Size = new System.Drawing.Size(297, 486);
			this.layoutTabPage.TabIndex = 0;
			this.layoutTabPage.Text = "Network";
			this.layoutTabPage.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.titleEdit);
			this.groupBox3.Controls.Add(this.infoEdit);
			this.groupBox3.Location = new System.Drawing.Point(8, 324);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(280, 156);
			this.groupBox3.TabIndex = 21;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Network Information";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 22);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(30, 13);
			this.label7.TabIndex = 2;
			this.label7.Text = "Title:";
			// 
			// titleEdit
			// 
			this.titleEdit.Location = new System.Drawing.Point(42, 19);
			this.titleEdit.Name = "titleEdit";
			this.titleEdit.Size = new System.Drawing.Size(232, 20);
			this.titleEdit.TabIndex = 1;
			this.titleEdit.TextChanged += new System.EventHandler(this.titleEdit_TextChanged);
			this.titleEdit.Leave += new System.EventHandler(this.titleEdit_Leave);
			// 
			// infoEdit
			// 
			this.infoEdit.Location = new System.Drawing.Point(6, 45);
			this.infoEdit.Multiline = true;
			this.infoEdit.Name = "infoEdit";
			this.infoEdit.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.infoEdit.Size = new System.Drawing.Size(268, 104);
			this.infoEdit.TabIndex = 0;
			this.infoEdit.Leave += new System.EventHandler(this.textBox1_Leave);
			// 
			// NodeConnectionSetupGroupBox
			// 
			this.NodeConnectionSetupGroupBox.Controls.Add(this.enableIncomingLineChangeCheckBox);
			this.NodeConnectionSetupGroupBox.Controls.Add(this.enableOutgoingLineChangeCheckBox);
			this.NodeConnectionSetupGroupBox.Controls.Add(this.tramAllowedCheckBox);
			this.NodeConnectionSetupGroupBox.Controls.Add(this.busAllowedCheckBox);
			this.NodeConnectionSetupGroupBox.Controls.Add(this.carsAllowedCheckBox);
			this.NodeConnectionSetupGroupBox.Controls.Add(this.label4);
			this.NodeConnectionSetupGroupBox.Controls.Add(this.nodeConnectionPrioritySpinEdit);
			this.NodeConnectionSetupGroupBox.Location = new System.Drawing.Point(8, 228);
			this.NodeConnectionSetupGroupBox.Name = "NodeConnectionSetupGroupBox";
			this.NodeConnectionSetupGroupBox.Size = new System.Drawing.Size(280, 90);
			this.NodeConnectionSetupGroupBox.TabIndex = 17;
			this.NodeConnectionSetupGroupBox.TabStop = false;
			this.NodeConnectionSetupGroupBox.Text = "Connection";
			// 
			// enableIncomingLineChangeCheckBox
			// 
			this.enableIncomingLineChangeCheckBox.AutoSize = true;
			this.enableIncomingLineChangeCheckBox.Checked = true;
			this.enableIncomingLineChangeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enableIncomingLineChangeCheckBox.Location = new System.Drawing.Point(9, 66);
			this.enableIncomingLineChangeCheckBox.Name = "enableIncomingLineChangeCheckBox";
			this.enableIncomingLineChangeCheckBox.Size = new System.Drawing.Size(160, 17);
			this.enableIncomingLineChangeCheckBox.TabIndex = 6;
			this.enableIncomingLineChangeCheckBox.Text = "Allow Incoming Line Change";
			this.enableIncomingLineChangeCheckBox.UseVisualStyleBackColor = true;
			this.enableIncomingLineChangeCheckBox.Click += new System.EventHandler(this.enableIncomingLineChangeCheckBox_Click);
			// 
			// enableOutgoingLineChangeCheckBox
			// 
			this.enableOutgoingLineChangeCheckBox.AutoSize = true;
			this.enableOutgoingLineChangeCheckBox.Checked = true;
			this.enableOutgoingLineChangeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enableOutgoingLineChangeCheckBox.Location = new System.Drawing.Point(9, 43);
			this.enableOutgoingLineChangeCheckBox.Name = "enableOutgoingLineChangeCheckBox";
			this.enableOutgoingLineChangeCheckBox.Size = new System.Drawing.Size(160, 17);
			this.enableOutgoingLineChangeCheckBox.TabIndex = 5;
			this.enableOutgoingLineChangeCheckBox.Text = "Allow Outgoing Line Change";
			this.enableOutgoingLineChangeCheckBox.UseVisualStyleBackColor = true;
			this.enableOutgoingLineChangeCheckBox.Click += new System.EventHandler(this.enableOutgoingLineChangeCheckBox_Click);
			// 
			// tramAllowedCheckBox
			// 
			this.tramAllowedCheckBox.AutoSize = true;
			this.tramAllowedCheckBox.Location = new System.Drawing.Point(183, 66);
			this.tramAllowedCheckBox.Name = "tramAllowedCheckBox";
			this.tramAllowedCheckBox.Size = new System.Drawing.Size(83, 17);
			this.tramAllowedCheckBox.TabIndex = 4;
			this.tramAllowedCheckBox.Text = "Allow Trams";
			this.tramAllowedCheckBox.UseVisualStyleBackColor = true;
			this.tramAllowedCheckBox.CheckedChanged += new System.EventHandler(this.tramAllowedCheckBox_CheckedChanged);
			// 
			// busAllowedCheckBox
			// 
			this.busAllowedCheckBox.AutoSize = true;
			this.busAllowedCheckBox.Location = new System.Drawing.Point(183, 43);
			this.busAllowedCheckBox.Name = "busAllowedCheckBox";
			this.busAllowedCheckBox.Size = new System.Drawing.Size(88, 17);
			this.busAllowedCheckBox.TabIndex = 3;
			this.busAllowedCheckBox.Text = "Allow Busses";
			this.busAllowedCheckBox.UseVisualStyleBackColor = true;
			this.busAllowedCheckBox.CheckedChanged += new System.EventHandler(this.busAllowedCheckBox_CheckedChanged);
			// 
			// carsAllowedCheckBox
			// 
			this.carsAllowedCheckBox.AutoSize = true;
			this.carsAllowedCheckBox.Checked = true;
			this.carsAllowedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.carsAllowedCheckBox.Location = new System.Drawing.Point(183, 20);
			this.carsAllowedCheckBox.Name = "carsAllowedCheckBox";
			this.carsAllowedCheckBox.Size = new System.Drawing.Size(75, 17);
			this.carsAllowedCheckBox.TabIndex = 2;
			this.carsAllowedCheckBox.Text = "Allow Cars";
			this.carsAllowedCheckBox.UseVisualStyleBackColor = true;
			this.carsAllowedCheckBox.CheckedChanged += new System.EventHandler(this.carsAllowedCheckBox_CheckedChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 13);
			this.label4.TabIndex = 1;
			this.label4.Text = "Priority:";
			// 
			// nodeConnectionPrioritySpinEdit
			// 
			this.nodeConnectionPrioritySpinEdit.Location = new System.Drawing.Point(58, 19);
			this.nodeConnectionPrioritySpinEdit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nodeConnectionPrioritySpinEdit.Name = "nodeConnectionPrioritySpinEdit";
			this.nodeConnectionPrioritySpinEdit.Size = new System.Drawing.Size(56, 20);
			this.nodeConnectionPrioritySpinEdit.TabIndex = 0;
			this.nodeConnectionPrioritySpinEdit.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nodeConnectionPrioritySpinEdit.ValueChanged += new System.EventHandler(this.nodeConnectionPrioritySpinEdit_ValueChanged);
			// 
			// LineNodeSetupGroupBox
			// 
			this.LineNodeSetupGroupBox.Controls.Add(this.showEditorButton);
			this.LineNodeSetupGroupBox.Controls.Add(this.freeNodeButton);
			this.LineNodeSetupGroupBox.Controls.Add(this.trafficLightTreeView);
			this.LineNodeSetupGroupBox.Location = new System.Drawing.Point(6, 6);
			this.LineNodeSetupGroupBox.Name = "LineNodeSetupGroupBox";
			this.LineNodeSetupGroupBox.Size = new System.Drawing.Size(282, 216);
			this.LineNodeSetupGroupBox.TabIndex = 15;
			this.LineNodeSetupGroupBox.TabStop = false;
			this.LineNodeSetupGroupBox.Text = "Assign Signals to Nodes";
			// 
			// verkehrTabPage
			// 
			this.verkehrTabPage.Controls.Add(this.label6);
			this.verkehrTabPage.Controls.Add(this.trafficDensityMultiplierSpinEdit);
			this.verkehrTabPage.Controls.Add(this.label5);
			this.verkehrTabPage.Controls.Add(this.button3);
			this.verkehrTabPage.Controls.Add(this.vehicleTypeComboBox);
			this.verkehrTabPage.Controls.Add(this.carGroupBox);
			this.verkehrTabPage.Controls.Add(this.SetEndNodeButton);
			this.verkehrTabPage.Controls.Add(this.SetStartNodeButton);
			this.verkehrTabPage.Controls.Add(this.HäufigkeitSpinEdit);
			this.verkehrTabPage.Controls.Add(this.AuftragLöschenButton);
			this.verkehrTabPage.Controls.Add(this.NeuerAuftragButton);
			this.verkehrTabPage.Controls.Add(this.AufträgeCheckBox);
			this.verkehrTabPage.Location = new System.Drawing.Point(4, 22);
			this.verkehrTabPage.Name = "verkehrTabPage";
			this.verkehrTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.verkehrTabPage.Size = new System.Drawing.Size(297, 486);
			this.verkehrTabPage.TabIndex = 2;
			this.verkehrTabPage.Text = "Traffic";
			this.verkehrTabPage.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(8, 342);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(78, 13);
			this.label6.TabIndex = 21;
			this.label6.Text = "Vehicles/Hour:";
			// 
			// trafficDensityMultiplierSpinEdit
			// 
			this.trafficDensityMultiplierSpinEdit.DecimalPlaces = 1;
			this.trafficDensityMultiplierSpinEdit.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.trafficDensityMultiplierSpinEdit.Location = new System.Drawing.Point(164, 256);
			this.trafficDensityMultiplierSpinEdit.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.trafficDensityMultiplierSpinEdit.Name = "trafficDensityMultiplierSpinEdit";
			this.trafficDensityMultiplierSpinEdit.Size = new System.Drawing.Size(124, 20);
			this.trafficDensityMultiplierSpinEdit.TabIndex = 20;
			this.trafficDensityMultiplierSpinEdit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.trafficDensityMultiplierSpinEdit.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 258);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(155, 13);
			this.label5.TabIndex = 19;
			this.label5.Text = "Global Traffic Volume Multiplier:";
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(229, 311);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(59, 23);
			this.button3.TabIndex = 18;
			this.button3.Text = "Set";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// vehicleTypeComboBox
			// 
			this.vehicleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.vehicleTypeComboBox.FormattingEnabled = true;
			this.vehicleTypeComboBox.Items.AddRange(new object[] {
            "Auto",
            "Bus",
            "Tram"});
			this.vehicleTypeComboBox.Location = new System.Drawing.Point(6, 284);
			this.vehicleTypeComboBox.Name = "vehicleTypeComboBox";
			this.vehicleTypeComboBox.Size = new System.Drawing.Size(152, 21);
			this.vehicleTypeComboBox.TabIndex = 17;
			this.vehicleTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.vehicleTypeComboBox_SelectedIndexChanged);
			// 
			// carGroupBox
			// 
			this.carGroupBox.Controls.Add(this.label1);
			this.carGroupBox.Controls.Add(this.truckRatioSpinEdit);
			this.carGroupBox.Controls.Add(this.addBusButton);
			this.carGroupBox.Controls.Add(this.addTramButton);
			this.carGroupBox.Controls.Add(this.label3);
			this.carGroupBox.Controls.Add(this.v0Edit);
			this.carGroupBox.Controls.Add(this.addCarButton);
			this.carGroupBox.Location = new System.Drawing.Point(6, 370);
			this.carGroupBox.Name = "carGroupBox";
			this.carGroupBox.Size = new System.Drawing.Size(282, 110);
			this.carGroupBox.TabIndex = 16;
			this.carGroupBox.TabStop = false;
			this.carGroupBox.Text = "Create Single Vehicle";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Truck Rate in %:";
			// 
			// truckRatioSpinEdit
			// 
			this.truckRatioSpinEdit.Location = new System.Drawing.Point(102, 43);
			this.truckRatioSpinEdit.Name = "truckRatioSpinEdit";
			this.truckRatioSpinEdit.Size = new System.Drawing.Size(57, 20);
			this.truckRatioSpinEdit.TabIndex = 7;
			this.truckRatioSpinEdit.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.truckRatioSpinEdit.ValueChanged += new System.EventHandler(this.truckRatioSpinEdit_ValueChanged);
			// 
			// addBusButton
			// 
			this.addBusButton.Location = new System.Drawing.Point(165, 45);
			this.addBusButton.Name = "addBusButton";
			this.addBusButton.Size = new System.Drawing.Size(111, 23);
			this.addBusButton.TabIndex = 6;
			this.addBusButton.Text = "Create Single Bus";
			this.addBusButton.UseVisualStyleBackColor = true;
			this.addBusButton.Click += new System.EventHandler(this.addBusButton_Click);
			// 
			// addTramButton
			// 
			this.addTramButton.Location = new System.Drawing.Point(165, 74);
			this.addTramButton.Name = "addTramButton";
			this.addTramButton.Size = new System.Drawing.Size(111, 23);
			this.addTramButton.TabIndex = 5;
			this.addTramButton.Text = "Create Single Tram";
			this.addTramButton.UseVisualStyleBackColor = true;
			this.addTramButton.Click += new System.EventHandler(this.addTramButton_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Target Speed:";
			// 
			// v0Edit
			// 
			this.v0Edit.Location = new System.Drawing.Point(102, 17);
			this.v0Edit.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.v0Edit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.v0Edit.Name = "v0Edit";
			this.v0Edit.Size = new System.Drawing.Size(57, 20);
			this.v0Edit.TabIndex = 3;
			this.v0Edit.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
			// 
			// addCarButton
			// 
			this.addCarButton.Location = new System.Drawing.Point(165, 16);
			this.addCarButton.Name = "addCarButton";
			this.addCarButton.Size = new System.Drawing.Size(111, 23);
			this.addCarButton.TabIndex = 2;
			this.addCarButton.Text = "Create Single Car";
			this.addCarButton.UseVisualStyleBackColor = true;
			this.addCarButton.Click += new System.EventHandler(this.addCarButton_Click);
			// 
			// SetEndNodeButton
			// 
			this.SetEndNodeButton.Location = new System.Drawing.Point(164, 311);
			this.SetEndNodeButton.Name = "SetEndNodeButton";
			this.SetEndNodeButton.Size = new System.Drawing.Size(59, 23);
			this.SetEndNodeButton.TabIndex = 5;
			this.SetEndNodeButton.Text = "Destination";
			this.SetEndNodeButton.UseVisualStyleBackColor = true;
			this.SetEndNodeButton.Click += new System.EventHandler(this.SetEndNodeButton_Click);
			// 
			// SetStartNodeButton
			// 
			this.SetStartNodeButton.Location = new System.Drawing.Point(99, 311);
			this.SetStartNodeButton.Name = "SetStartNodeButton";
			this.SetStartNodeButton.Size = new System.Drawing.Size(59, 23);
			this.SetStartNodeButton.TabIndex = 4;
			this.SetStartNodeButton.Text = "Start";
			this.SetStartNodeButton.UseVisualStyleBackColor = true;
			this.SetStartNodeButton.Click += new System.EventHandler(this.SetStartNodeButton_Click);
			// 
			// HäufigkeitSpinEdit
			// 
			this.HäufigkeitSpinEdit.Location = new System.Drawing.Point(164, 340);
			this.HäufigkeitSpinEdit.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.HäufigkeitSpinEdit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.HäufigkeitSpinEdit.Name = "HäufigkeitSpinEdit";
			this.HäufigkeitSpinEdit.Size = new System.Drawing.Size(124, 20);
			this.HäufigkeitSpinEdit.TabIndex = 3;
			this.HäufigkeitSpinEdit.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.HäufigkeitSpinEdit.Click += new System.EventHandler(this.HäufigkeitSpinEdit_ValueChanged);
			// 
			// AuftragLöschenButton
			// 
			this.AuftragLöschenButton.Location = new System.Drawing.Point(229, 282);
			this.AuftragLöschenButton.Name = "AuftragLöschenButton";
			this.AuftragLöschenButton.Size = new System.Drawing.Size(59, 23);
			this.AuftragLöschenButton.TabIndex = 2;
			this.AuftragLöschenButton.Text = "Delete";
			this.AuftragLöschenButton.UseVisualStyleBackColor = true;
			this.AuftragLöschenButton.Click += new System.EventHandler(this.AuftragLöschenButton_Click);
			// 
			// NeuerAuftragButton
			// 
			this.NeuerAuftragButton.Location = new System.Drawing.Point(164, 282);
			this.NeuerAuftragButton.Name = "NeuerAuftragButton";
			this.NeuerAuftragButton.Size = new System.Drawing.Size(59, 23);
			this.NeuerAuftragButton.TabIndex = 1;
			this.NeuerAuftragButton.Text = "New";
			this.NeuerAuftragButton.UseVisualStyleBackColor = true;
			this.NeuerAuftragButton.Click += new System.EventHandler(this.NeuerAuftragButton_Click);
			// 
			// AufträgeCheckBox
			// 
			this.AufträgeCheckBox.FormattingEnabled = true;
			this.AufträgeCheckBox.Location = new System.Drawing.Point(6, 6);
			this.AufträgeCheckBox.Name = "AufträgeCheckBox";
			this.AufträgeCheckBox.Size = new System.Drawing.Size(282, 244);
			this.AufträgeCheckBox.TabIndex = 0;
			this.AufträgeCheckBox.SelectedIndexChanged += new System.EventHandler(this.AufträgeCheckBox_SelectedIndexChanged);
			this.AufträgeCheckBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AufträgeCheckBox_KeyDown);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.groupBox2);
			this.tabPage2.Controls.Add(this.HintergrundbildGroupBox);
			this.tabPage2.Controls.Add(this.AnsichtGroupBox);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(297, 486);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "View";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.canvasHeigthSpinEdit);
			this.groupBox2.Controls.Add(this.canvasWidthSpinEdit);
			this.groupBox2.Location = new System.Drawing.Point(6, 364);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(282, 100);
			this.groupBox2.TabIndex = 23;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Network Canvas";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(111, 47);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(82, 13);
			this.label10.TabIndex = 3;
			this.label10.Text = "Height in Pixels:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(110, 21);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(79, 13);
			this.label9.TabIndex = 2;
			this.label9.Text = "Width in Pixels:";
			// 
			// canvasHeigthSpinEdit
			// 
			this.canvasHeigthSpinEdit.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.canvasHeigthSpinEdit.Location = new System.Drawing.Point(195, 45);
			this.canvasHeigthSpinEdit.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.canvasHeigthSpinEdit.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.canvasHeigthSpinEdit.Name = "canvasHeigthSpinEdit";
			this.canvasHeigthSpinEdit.Size = new System.Drawing.Size(75, 20);
			this.canvasHeigthSpinEdit.TabIndex = 1;
			this.canvasHeigthSpinEdit.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.canvasHeigthSpinEdit.ValueChanged += new System.EventHandler(this.canvasHeigthSpinEdit_ValueChanged);
			// 
			// canvasWidthSpinEdit
			// 
			this.canvasWidthSpinEdit.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.canvasWidthSpinEdit.Location = new System.Drawing.Point(195, 19);
			this.canvasWidthSpinEdit.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.canvasWidthSpinEdit.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.canvasWidthSpinEdit.Name = "canvasWidthSpinEdit";
			this.canvasWidthSpinEdit.Size = new System.Drawing.Size(75, 20);
			this.canvasWidthSpinEdit.TabIndex = 0;
			this.canvasWidthSpinEdit.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.canvasWidthSpinEdit.ValueChanged += new System.EventHandler(this.canvasWidthSpinEdit_ValueChanged);
			// 
			// HintergrundbildGroupBox
			// 
			this.HintergrundbildGroupBox.Controls.Add(this.label8);
			this.HintergrundbildGroupBox.Controls.Add(this.backgroundImageScalingSpinEdit);
			this.HintergrundbildGroupBox.Controls.Add(this.clearBackgroudnImageButton);
			this.HintergrundbildGroupBox.Controls.Add(this.BildLadenButton);
			this.HintergrundbildGroupBox.Controls.Add(this.backgroundImageEdit);
			this.HintergrundbildGroupBox.Location = new System.Drawing.Point(6, 244);
			this.HintergrundbildGroupBox.Name = "HintergrundbildGroupBox";
			this.HintergrundbildGroupBox.Size = new System.Drawing.Size(280, 114);
			this.HintergrundbildGroupBox.TabIndex = 22;
			this.HintergrundbildGroupBox.TabStop = false;
			this.HintergrundbildGroupBox.Text = "Background Image";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(6, 76);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(130, 13);
			this.label8.TabIndex = 4;
			this.label8.Text = "Background Image Zoom:";
			// 
			// backgroundImageScalingSpinEdit
			// 
			this.backgroundImageScalingSpinEdit.Location = new System.Drawing.Point(195, 74);
			this.backgroundImageScalingSpinEdit.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
			this.backgroundImageScalingSpinEdit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.backgroundImageScalingSpinEdit.Name = "backgroundImageScalingSpinEdit";
			this.backgroundImageScalingSpinEdit.Size = new System.Drawing.Size(75, 20);
			this.backgroundImageScalingSpinEdit.TabIndex = 3;
			this.backgroundImageScalingSpinEdit.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.backgroundImageScalingSpinEdit.ValueChanged += new System.EventHandler(this.backgroundImageScalingSpinEdit_ValueChanged);
			// 
			// clearBackgroudnImageButton
			// 
			this.clearBackgroudnImageButton.Location = new System.Drawing.Point(114, 45);
			this.clearBackgroudnImageButton.Name = "clearBackgroudnImageButton";
			this.clearBackgroudnImageButton.Size = new System.Drawing.Size(75, 23);
			this.clearBackgroudnImageButton.TabIndex = 2;
			this.clearBackgroudnImageButton.Text = "Remove";
			this.clearBackgroudnImageButton.UseVisualStyleBackColor = true;
			this.clearBackgroudnImageButton.Click += new System.EventHandler(this.clearBackgroudnImageButton_Click);
			// 
			// BildLadenButton
			// 
			this.BildLadenButton.Location = new System.Drawing.Point(195, 45);
			this.BildLadenButton.Name = "BildLadenButton";
			this.BildLadenButton.Size = new System.Drawing.Size(75, 23);
			this.BildLadenButton.TabIndex = 1;
			this.BildLadenButton.Text = "Load Image";
			this.BildLadenButton.UseVisualStyleBackColor = true;
			this.BildLadenButton.Click += new System.EventHandler(this.BildLadenButton_Click);
			// 
			// backgroundImageEdit
			// 
			this.backgroundImageEdit.Location = new System.Drawing.Point(6, 19);
			this.backgroundImageEdit.Name = "backgroundImageEdit";
			this.backgroundImageEdit.Size = new System.Drawing.Size(264, 20);
			this.backgroundImageEdit.TabIndex = 0;
			// 
			// AnsichtGroupBox
			// 
			this.AnsichtGroupBox.Controls.Add(this.label13);
			this.AnsichtGroupBox.Controls.Add(this.renderQualityComboBox);
			this.AnsichtGroupBox.Controls.Add(this.visualizationCheckBox);
			this.AnsichtGroupBox.Controls.Add(this.drawNodeConnectionsCheckBox);
			this.AnsichtGroupBox.Controls.Add(this.zoomComboBox);
			this.AnsichtGroupBox.Controls.Add(this.drawDebugCheckBox);
			this.AnsichtGroupBox.Controls.Add(this.dockToGridcheckBox);
			this.AnsichtGroupBox.Controls.Add(this.label2);
			this.AnsichtGroupBox.Location = new System.Drawing.Point(6, 6);
			this.AnsichtGroupBox.Name = "AnsichtGroupBox";
			this.AnsichtGroupBox.Size = new System.Drawing.Size(282, 232);
			this.AnsichtGroupBox.TabIndex = 21;
			this.AnsichtGroupBox.TabStop = false;
			this.AnsichtGroupBox.Text = "View";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(6, 48);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(80, 13);
			this.label13.TabIndex = 14;
			this.label13.Text = "Render Quality:";
			// 
			// renderQualityComboBox
			// 
			this.renderQualityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.renderQualityComboBox.FormattingEnabled = true;
			this.renderQualityComboBox.Items.AddRange(new object[] {
            "High",
            "Low"});
			this.renderQualityComboBox.Location = new System.Drawing.Point(114, 45);
			this.renderQualityComboBox.Name = "renderQualityComboBox";
			this.renderQualityComboBox.Size = new System.Drawing.Size(156, 21);
			this.renderQualityComboBox.TabIndex = 13;
			// 
			// visualizationCheckBox
			// 
			this.visualizationCheckBox.AutoSize = true;
			this.visualizationCheckBox.Location = new System.Drawing.Point(9, 141);
			this.visualizationCheckBox.Name = "visualizationCheckBox";
			this.visualizationCheckBox.Size = new System.Drawing.Size(106, 17);
			this.visualizationCheckBox.TabIndex = 12;
			this.visualizationCheckBox.Text = "Render Statistics";
			this.visualizationCheckBox.UseVisualStyleBackColor = true;
			this.visualizationCheckBox.CheckedChanged += new System.EventHandler(this.visualizationCheckBox_CheckedChanged);
			// 
			// drawNodeConnectionsCheckBox
			// 
			this.drawNodeConnectionsCheckBox.AutoSize = true;
			this.drawNodeConnectionsCheckBox.Checked = true;
			this.drawNodeConnectionsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.drawNodeConnectionsCheckBox.Location = new System.Drawing.Point(9, 118);
			this.drawNodeConnectionsCheckBox.Name = "drawNodeConnectionsCheckBox";
			this.drawNodeConnectionsCheckBox.Size = new System.Drawing.Size(178, 17);
			this.drawNodeConnectionsCheckBox.TabIndex = 11;
			this.drawNodeConnectionsCheckBox.Text = "Render Nodes and Connections";
			this.drawNodeConnectionsCheckBox.UseVisualStyleBackColor = true;
			this.drawNodeConnectionsCheckBox.CheckedChanged += new System.EventHandler(this.drawNodeConnectionsCheckBox_CheckedChanged);
			// 
			// zoomComboBox
			// 
			this.zoomComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.zoomComboBox.FormattingEnabled = true;
			this.zoomComboBox.Items.AddRange(new object[] {
            "10%",
            "15%",
            "20%",
            "25%",
            "33%",
            "50%",
            "67%",
            "100%",
            "150%",
            "200%"});
			this.zoomComboBox.Location = new System.Drawing.Point(114, 18);
			this.zoomComboBox.Name = "zoomComboBox";
			this.zoomComboBox.Size = new System.Drawing.Size(156, 21);
			this.zoomComboBox.TabIndex = 8;
			this.zoomComboBox.SelectedIndexChanged += new System.EventHandler(this.zoomComboBox_SelectedIndexChanged);
			// 
			// drawDebugCheckBox
			// 
			this.drawDebugCheckBox.AutoSize = true;
			this.drawDebugCheckBox.Location = new System.Drawing.Point(9, 72);
			this.drawDebugCheckBox.Name = "drawDebugCheckBox";
			this.drawDebugCheckBox.Size = new System.Drawing.Size(143, 17);
			this.drawDebugCheckBox.TabIndex = 10;
			this.drawDebugCheckBox.Text = "Show Debug Information";
			this.drawDebugCheckBox.UseVisualStyleBackColor = true;
			this.drawDebugCheckBox.CheckedChanged += new System.EventHandler(this.drawDebugCheckBox_CheckedChanged);
			// 
			// dockToGridcheckBox
			// 
			this.dockToGridcheckBox.AutoSize = true;
			this.dockToGridcheckBox.Location = new System.Drawing.Point(9, 95);
			this.dockToGridcheckBox.Name = "dockToGridcheckBox";
			this.dockToGridcheckBox.Size = new System.Drawing.Size(86, 17);
			this.dockToGridcheckBox.TabIndex = 7;
			this.dockToGridcheckBox.Text = "Dock to Grid";
			this.dockToGridcheckBox.UseVisualStyleBackColor = true;
			this.dockToGridcheckBox.CheckStateChanged += new System.EventHandler(this.dockToGridcheckBox_CheckedChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Zoom:";
			// 
			// thumbTabPage
			// 
			this.thumbTabPage.Controls.Add(this.thumbGrid);
			this.thumbTabPage.Location = new System.Drawing.Point(4, 22);
			this.thumbTabPage.Name = "thumbTabPage";
			this.thumbTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.thumbTabPage.Size = new System.Drawing.Size(297, 486);
			this.thumbTabPage.TabIndex = 3;
			this.thumbTabPage.Text = "Thumbnail";
			this.thumbTabPage.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.stepsPerSecondSpinEdit);
			this.groupBox1.Controls.Add(this.label12);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.simulationSpeedSpinEdit);
			this.groupBox1.Controls.Add(this.SpeichernButton);
			this.groupBox1.Controls.Add(this.aboutBoxButton);
			this.groupBox1.Controls.Add(this.LadenButton);
			this.groupBox1.Controls.Add(this.findLineChangePointsButton);
			this.groupBox1.Controls.Add(this.timelinePanel);
			this.groupBox1.Controls.Add(this.lockNodesCheckBox);
			this.groupBox1.Controls.Add(this.killAllVehiclesButton);
			this.groupBox1.Controls.Add(this.stepButton);
			this.groupBox1.Controls.Add(this.timerOnCheckBox);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(964, 136);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Signal Timeline";
			// 
			// stepsPerSecondSpinEdit
			// 
			this.stepsPerSecondSpinEdit.Location = new System.Drawing.Point(737, 48);
			this.stepsPerSecondSpinEdit.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.stepsPerSecondSpinEdit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.stepsPerSecondSpinEdit.Name = "stepsPerSecondSpinEdit";
			this.stepsPerSecondSpinEdit.Size = new System.Drawing.Size(46, 20);
			this.stepsPerSecondSpinEdit.TabIndex = 26;
			this.stepsPerSecondSpinEdit.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.stepsPerSecondSpinEdit.ValueChanged += new System.EventHandler(this.stepsPerSecondSpinEdit_ValueChanged);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(645, 50);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(70, 13);
			this.label12.TabIndex = 25;
			this.label12.Text = "Sim. Steps/s:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(645, 21);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(92, 13);
			this.label11.TabIndex = 24;
			this.label11.Text = "Simulation Speed:";
			// 
			// simulationSpeedSpinEdit
			// 
			this.simulationSpeedSpinEdit.Location = new System.Drawing.Point(737, 19);
			this.simulationSpeedSpinEdit.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.simulationSpeedSpinEdit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.simulationSpeedSpinEdit.Name = "simulationSpeedSpinEdit";
			this.simulationSpeedSpinEdit.Size = new System.Drawing.Size(46, 20);
			this.simulationSpeedSpinEdit.TabIndex = 23;
			this.simulationSpeedSpinEdit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.simulationSpeedSpinEdit.ValueChanged += new System.EventHandler(this.simulationSpeedSpinEdit_ValueChanged);
			// 
			// SpeichernButton
			// 
			this.SpeichernButton.Location = new System.Drawing.Point(875, 16);
			this.SpeichernButton.Name = "SpeichernButton";
			this.SpeichernButton.Size = new System.Drawing.Size(83, 23);
			this.SpeichernButton.TabIndex = 21;
			this.SpeichernButton.Text = "Save Network";
			this.SpeichernButton.UseVisualStyleBackColor = true;
			this.SpeichernButton.Click += new System.EventHandler(this.SpeichernButton_Click);
			// 
			// aboutBoxButton
			// 
			this.aboutBoxButton.Location = new System.Drawing.Point(789, 103);
			this.aboutBoxButton.Name = "aboutBoxButton";
			this.aboutBoxButton.Size = new System.Drawing.Size(169, 23);
			this.aboutBoxButton.TabIndex = 12;
			this.aboutBoxButton.Text = "About CityTrafficSimulator";
			this.aboutBoxButton.UseVisualStyleBackColor = true;
			this.aboutBoxButton.Click += new System.EventHandler(this.aboutBoxButton_Click);
			// 
			// LadenButton
			// 
			this.LadenButton.Location = new System.Drawing.Point(789, 16);
			this.LadenButton.Name = "LadenButton";
			this.LadenButton.Size = new System.Drawing.Size(82, 23);
			this.LadenButton.TabIndex = 20;
			this.LadenButton.Text = "Load Network";
			this.LadenButton.UseVisualStyleBackColor = true;
			this.LadenButton.Click += new System.EventHandler(this.LadenButton_Click);
			// 
			// findLineChangePointsButton
			// 
			this.findLineChangePointsButton.Location = new System.Drawing.Point(789, 74);
			this.findLineChangePointsButton.Name = "findLineChangePointsButton";
			this.findLineChangePointsButton.Size = new System.Drawing.Size(169, 23);
			this.findLineChangePointsButton.TabIndex = 22;
			this.findLineChangePointsButton.Text = "Recalculate Line Changes";
			this.findLineChangePointsButton.UseVisualStyleBackColor = true;
			this.findLineChangePointsButton.Click += new System.EventHandler(this.findLineChangePointsButton_Click);
			// 
			// timelinePanel
			// 
			this.timelinePanel.AutoScroll = true;
			this.timelinePanel.Controls.Add(this.timeline);
			this.timelinePanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.timelinePanel.Location = new System.Drawing.Point(3, 16);
			this.timelinePanel.Name = "timelinePanel";
			this.timelinePanel.Size = new System.Drawing.Size(546, 117);
			this.timelinePanel.TabIndex = 14;
			// 
			// lockNodesCheckBox
			// 
			this.lockNodesCheckBox.AutoSize = true;
			this.lockNodesCheckBox.Location = new System.Drawing.Point(555, 20);
			this.lockNodesCheckBox.Name = "lockNodesCheckBox";
			this.lockNodesCheckBox.Size = new System.Drawing.Size(84, 17);
			this.lockNodesCheckBox.TabIndex = 13;
			this.lockNodesCheckBox.Text = "Lock Nodes";
			this.lockNodesCheckBox.UseVisualStyleBackColor = true;
			// 
			// killAllVehiclesButton
			// 
			this.killAllVehiclesButton.Location = new System.Drawing.Point(789, 45);
			this.killAllVehiclesButton.Name = "killAllVehiclesButton";
			this.killAllVehiclesButton.Size = new System.Drawing.Size(169, 23);
			this.killAllVehiclesButton.TabIndex = 11;
			this.killAllVehiclesButton.Text = "Remove all Vehicles";
			this.killAllVehiclesButton.UseVisualStyleBackColor = true;
			this.killAllVehiclesButton.Click += new System.EventHandler(this.killAllVehiclesButton_Click);
			// 
			// stepButton
			// 
			this.stepButton.Location = new System.Drawing.Point(648, 103);
			this.stepButton.Name = "stepButton";
			this.stepButton.Size = new System.Drawing.Size(135, 23);
			this.stepButton.TabIndex = 9;
			this.stepButton.Text = "Single Step";
			this.stepButton.UseVisualStyleBackColor = true;
			this.stepButton.Click += new System.EventHandler(this.stepButton_Click);
			// 
			// timerOnCheckBox
			// 
			this.timerOnCheckBox.AutoSize = true;
			this.timerOnCheckBox.Location = new System.Drawing.Point(648, 78);
			this.timerOnCheckBox.Name = "timerOnCheckBox";
			this.timerOnCheckBox.Size = new System.Drawing.Size(110, 17);
			this.timerOnCheckBox.TabIndex = 6;
			this.timerOnCheckBox.Text = "Enable Simulation";
			this.timerOnCheckBox.UseVisualStyleBackColor = true;
			this.timerOnCheckBox.CheckedChanged += new System.EventHandler(this.timerOnCheckBox_CheckedChanged);
			// 
			// statusleiste
			// 
			this.statusleiste.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusMenu,
            this.statusLabel});
			this.statusleiste.Location = new System.Drawing.Point(0, 652);
			this.statusleiste.Name = "statusleiste";
			this.statusleiste.Size = new System.Drawing.Size(964, 22);
			this.statusleiste.TabIndex = 8;
			this.statusleiste.Text = "statusStrip1";
			// 
			// statusMenu
			// 
			this.statusMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.statusMenu.Image = ((System.Drawing.Image)(resources.GetObject("statusMenu.Image")));
			this.statusMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.statusMenu.Name = "statusMenu";
			this.statusMenu.Size = new System.Drawing.Size(29, 20);
			this.statusMenu.Text = "toolStripDropDownButton1";
			// 
			// statusLabel
			// 
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(195, 41);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 21;
			this.button1.Text = "Speichern";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(114, 41);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 20;
			this.button2.Text = "Laden";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// freeNodeButton
			// 
			this.freeNodeButton.Location = new System.Drawing.Point(198, 187);
			this.freeNodeButton.Name = "freeNodeButton";
			this.freeNodeButton.Size = new System.Drawing.Size(75, 23);
			this.freeNodeButton.TabIndex = 2;
			this.freeNodeButton.Text = "Free Node";
			this.freeNodeButton.UseVisualStyleBackColor = true;
			this.freeNodeButton.Click += new System.EventHandler(this.freeNodeButton_Click);
			// 
			// showEditorButton
			// 
			this.showEditorButton.Location = new System.Drawing.Point(117, 187);
			this.showEditorButton.Name = "showEditorButton";
			this.showEditorButton.Size = new System.Drawing.Size(75, 23);
			this.showEditorButton.TabIndex = 3;
			this.showEditorButton.Text = "Show Editor";
			this.showEditorButton.UseVisualStyleBackColor = true;
			this.showEditorButton.Click += new System.EventHandler(this.showEditorButton_Click);
			// 
			// DaGrid
			// 
			this.DaGrid.AllowDrop = true;
			this.DaGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.DaGrid.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
			this.DaGrid.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.DaGrid.CellHeight = 16;
			this.DaGrid.CellSize = new System.Drawing.Size(16, 16);
			this.DaGrid.CellWidth = 16;
			this.DaGrid.Dimension = new System.Drawing.Point(64, 64);
			this.DaGrid.DrawGrid = false;
			this.DaGrid.Location = new System.Drawing.Point(0, 0);
			this.DaGrid.Max_X = 64;
			this.DaGrid.Max_Y = 64;
			this.DaGrid.Name = "DaGrid";
			this.DaGrid.Size = new System.Drawing.Size(12000, 10000);
			this.DaGrid.TabIndex = 3;
			this.DaGrid.Paint += new System.Windows.Forms.PaintEventHandler(this.DaGrid_Paint);
			this.DaGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DaGrid_MouseMove);
			this.DaGrid.Leave += new System.EventHandler(this.DaGrid_Leave);
			this.DaGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DaGrid_MouseDown);
			this.DaGrid.Enter += new System.EventHandler(this.DaGrid_Enter);
			this.DaGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DaGrid_MouseUp);
			this.DaGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DaGrid_KeyDown);
			// 
			// trafficLightTreeView
			// 
			this.trafficLightTreeView.Location = new System.Drawing.Point(9, 19);
			this.trafficLightTreeView.Name = "trafficLightTreeView";
			this.trafficLightTreeView.Size = new System.Drawing.Size(265, 162);
			this.trafficLightTreeView.steuerung = null;
			this.trafficLightTreeView.TabIndex = 1;
			this.trafficLightTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trafficLightTreeView_AfterSelect);
			// 
			// thumbGrid
			// 
			this.thumbGrid.BackColor = System.Drawing.Color.White;
			this.thumbGrid.CellHeight = 0;
			this.thumbGrid.CellSize = new System.Drawing.Size(0, 0);
			this.thumbGrid.CellWidth = 0;
			this.thumbGrid.Dimension = new System.Drawing.Point(0, 0);
			this.thumbGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.thumbGrid.DrawGrid = false;
			this.thumbGrid.Location = new System.Drawing.Point(3, 3);
			this.thumbGrid.Max_X = 0;
			this.thumbGrid.Max_Y = 0;
			this.thumbGrid.Name = "thumbGrid";
			this.thumbGrid.Size = new System.Drawing.Size(291, 480);
			this.thumbGrid.TabIndex = 0;
			this.thumbGrid.MouseLeave += new System.EventHandler(this.thumbGrid_MouseLeave);
			this.thumbGrid.Paint += new System.Windows.Forms.PaintEventHandler(this.thumbGrid_Paint);
			this.thumbGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.thumbGrid_MouseMove);
			this.thumbGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.thumbGrid_MouseDown);
			this.thumbGrid.Resize += new System.EventHandler(this.thumbGrid_Resize);
			this.thumbGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.thumbGrid_MouseUp);
			// 
			// timeline
			// 
			this.timeline.Location = new System.Drawing.Point(0, 0);
			this.timeline.Name = "timeline";
			this.timeline.selectedEntry = null;
			this.timeline.selectedGroup = null;
			this.timeline.Size = new System.Drawing.Size(505, 50);
			this.timeline.snapSize = 0.5;
			this.timeline.steuerung = null;
			this.timeline.TabIndex = 7;
			this.timeline.zoom = 10;
			this.timeline.MouseLeave += new System.EventHandler(this.timeline_MouseLeave);
			this.timeline.SelectionChanged += new CityTrafficSimulator.TimelineControl.SelectionChangedEventHandler(this.timeline_SelectionChanged);
			this.timeline.MouseMove += new System.Windows.Forms.MouseEventHandler(this.timeline_MouseMove);
			this.timeline.MouseDown += new System.Windows.Forms.MouseEventHandler(this.timeline_MouseDown);
			this.timeline.Enter += new System.EventHandler(this.timeline_Enter);
			this.timeline.MouseUp += new System.Windows.Forms.MouseEventHandler(this.timeline_MouseUp);
			this.timeline.EventChanged += new CityTrafficSimulator.TimelineControl.EventChangedEventHandler(this.timeline_EventChanged);
			this.timeline.TimelineMoved += new CityTrafficSimulator.TimelineControl.TimelineMovedEventHandler(this.timeline_TimelineMoved);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(964, 674);
			this.Controls.Add(this.splitContainer2);
			this.Controls.Add(this.statusleiste);
			this.DoubleBuffered = true;
			this.Name = "MainForm";
			this.Text = "CityTrafficSimulator";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.layoutTabPage.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.NodeConnectionSetupGroupBox.ResumeLayout(false);
			this.NodeConnectionSetupGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nodeConnectionPrioritySpinEdit)).EndInit();
			this.LineNodeSetupGroupBox.ResumeLayout(false);
			this.verkehrTabPage.ResumeLayout(false);
			this.verkehrTabPage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trafficDensityMultiplierSpinEdit)).EndInit();
			this.carGroupBox.ResumeLayout(false);
			this.carGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.truckRatioSpinEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.v0Edit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.HäufigkeitSpinEdit)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.canvasHeigthSpinEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.canvasWidthSpinEdit)).EndInit();
			this.HintergrundbildGroupBox.ResumeLayout(false);
			this.HintergrundbildGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.backgroundImageScalingSpinEdit)).EndInit();
			this.AnsichtGroupBox.ResumeLayout(false);
			this.AnsichtGroupBox.PerformLayout();
			this.thumbTabPage.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.stepsPerSecondSpinEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simulationSpeedSpinEdit)).EndInit();
			this.timelinePanel.ResumeLayout(false);
			this.statusleiste.ResumeLayout(false);
			this.statusleiste.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private RechenkaestchenControl DaGrid;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage verkehrTabPage;
        private System.Windows.Forms.Button SetEndNodeButton;
        private System.Windows.Forms.Button SetStartNodeButton;
        private System.Windows.Forms.NumericUpDown HäufigkeitSpinEdit;
        private System.Windows.Forms.Button AuftragLöschenButton;
        private System.Windows.Forms.Button NeuerAuftragButton;
        private System.Windows.Forms.CheckedListBox AufträgeCheckBox;
		private System.Windows.Forms.TabPage layoutTabPage;
        private System.Windows.Forms.GroupBox carGroupBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown v0Edit;
        private System.Windows.Forms.Button addCarButton;
		private System.Windows.Forms.GroupBox LineNodeSetupGroupBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox HintergrundbildGroupBox;
        private System.Windows.Forms.Button BildLadenButton;
        private System.Windows.Forms.TextBox backgroundImageEdit;
		private System.Windows.Forms.GroupBox AnsichtGroupBox;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SpeichernButton;
        private System.Windows.Forms.Button LadenButton;
		private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox timerOnCheckBox;
		private TimelineControl timeline;
		private System.Windows.Forms.StatusStrip statusleiste;
		private System.Windows.Forms.ToolStripDropDownButton statusMenu;
		private System.Windows.Forms.ToolStripStatusLabel statusLabel;
		private System.Windows.Forms.GroupBox NodeConnectionSetupGroupBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nodeConnectionPrioritySpinEdit;
		private System.Windows.Forms.Button stepButton;
		private System.Windows.Forms.CheckBox drawDebugCheckBox;
		private System.Windows.Forms.Button killAllVehiclesButton;
		private System.Windows.Forms.ComboBox zoomComboBox;
		private System.Windows.Forms.CheckBox dockToGridcheckBox;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button clearBackgroudnImageButton;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox infoEdit;
		private System.Windows.Forms.Button aboutBoxButton;
		private System.Windows.Forms.CheckBox lockNodesCheckBox;
		private System.Windows.Forms.Panel timelinePanel;
		private System.Windows.Forms.TabPage thumbTabPage;
		private RechenkaestchenControl thumbGrid;
		private System.Windows.Forms.CheckBox busAllowedCheckBox;
		private System.Windows.Forms.CheckBox carsAllowedCheckBox;
		private System.Windows.Forms.CheckBox tramAllowedCheckBox;
		private System.Windows.Forms.Button addTramButton;
		private System.Windows.Forms.ComboBox vehicleTypeComboBox;
		private System.Windows.Forms.Button addBusButton;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.CheckBox drawNodeConnectionsCheckBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown truckRatioSpinEdit;
		private System.Windows.Forms.CheckBox visualizationCheckBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown trafficDensityMultiplierSpinEdit;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button findLineChangePointsButton;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox titleEdit;
		private System.Windows.Forms.CheckBox enableOutgoingLineChangeCheckBox;
		private System.Windows.Forms.CheckBox enableIncomingLineChangeCheckBox;
		private CityTrafficSimulator.Timeline.TrafficLightTreeView trafficLightTreeView;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown backgroundImageScalingSpinEdit;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown canvasHeigthSpinEdit;
		private System.Windows.Forms.NumericUpDown canvasWidthSpinEdit;
		private System.Windows.Forms.NumericUpDown simulationSpeedSpinEdit;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.NumericUpDown stepsPerSecondSpinEdit;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.ComboBox renderQualityComboBox;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Button showEditorButton;
		private System.Windows.Forms.Button freeNodeButton;

        }
    }

