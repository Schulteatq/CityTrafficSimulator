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

﻿namespace CityTrafficSimulator.Verkehr
	{
	partial class TrafficVolumeForm
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.btnUpdateDestinationNodes = new System.Windows.Forms.Button();
			this.btnUpdateStartNodes = new System.Windows.Forms.Button();
			this.btnSetDestinationTitle = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSetStartTitle = new System.Windows.Forms.Button();
			this.lbStartNodes = new System.Windows.Forms.ListBox();
			this.lblDestinationTitle = new System.Windows.Forms.Label();
			this.lbDestinationNodes = new System.Windows.Forms.ListBox();
			this.lblStartTitle = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.editDestinationNodeTitle = new System.Windows.Forms.TextBox();
			this.btnAddStartNode = new System.Windows.Forms.Button();
			this.editStartNodeTitle = new System.Windows.Forms.TextBox();
			this.btnRemoveStartNode = new System.Windows.Forms.Button();
			this.btnRemoveDestinationNode = new System.Windows.Forms.Button();
			this.btnAddDestinationNode = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lblNumStops = new System.Windows.Forms.Label();
			this.lblNumVehicles = new System.Windows.Forms.Label();
			this.lblTravelTime = new System.Windows.Forms.Label();
			this.lblVelocity = new System.Windows.Forms.Label();
			this.lblMilage = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.spinTramsTargetVelocity = new System.Windows.Forms.NumericUpDown();
			this.spinBussesTargetVelocity = new System.Windows.Forms.NumericUpDown();
			this.spinTrucksTargetVelocity = new System.Windows.Forms.NumericUpDown();
			this.spinCarsTargetVelocity = new System.Windows.Forms.NumericUpDown();
			this.spinGlobalTrafficVolumeMultiplier = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.spinTruckVolume = new System.Windows.Forms.NumericUpDown();
			this.spinTramVolume = new System.Windows.Forms.NumericUpDown();
			this.spinBusVolume = new System.Windows.Forms.NumericUpDown();
			this.spinCarsVolume = new System.Windows.Forms.NumericUpDown();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spinTramsTargetVelocity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinBussesTargetVelocity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinTrucksTargetVelocity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinCarsTargetVelocity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinGlobalTrafficVolumeMultiplier)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spinTruckVolume)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinTramVolume)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinBusVolume)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinCarsVolume)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.btnUpdateDestinationNodes);
			this.splitContainer1.Panel1.Controls.Add(this.btnUpdateStartNodes);
			this.splitContainer1.Panel1.Controls.Add(this.btnSetDestinationTitle);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.btnSetStartTitle);
			this.splitContainer1.Panel1.Controls.Add(this.lbStartNodes);
			this.splitContainer1.Panel1.Controls.Add(this.lblDestinationTitle);
			this.splitContainer1.Panel1.Controls.Add(this.lbDestinationNodes);
			this.splitContainer1.Panel1.Controls.Add(this.lblStartTitle);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.editDestinationNodeTitle);
			this.splitContainer1.Panel1.Controls.Add(this.btnAddStartNode);
			this.splitContainer1.Panel1.Controls.Add(this.editStartNodeTitle);
			this.splitContainer1.Panel1.Controls.Add(this.btnRemoveStartNode);
			this.splitContainer1.Panel1.Controls.Add(this.btnRemoveDestinationNode);
			this.splitContainer1.Panel1.Controls.Add(this.btnAddDestinationNode);
			this.splitContainer1.Panel1.Resize += new System.EventHandler(this.splitContainer1_Panel1_Resize);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer1.Size = new System.Drawing.Size(890, 459);
			this.splitContainer1.SplitterDistance = 600;
			this.splitContainer1.TabIndex = 0;
			// 
			// btnUpdateDestinationNodes
			// 
			this.btnUpdateDestinationNodes.Location = new System.Drawing.Point(292, 292);
			this.btnUpdateDestinationNodes.Name = "btnUpdateDestinationNodes";
			this.btnUpdateDestinationNodes.Size = new System.Drawing.Size(60, 23);
			this.btnUpdateDestinationNodes.TabIndex = 10;
			this.btnUpdateDestinationNodes.Text = "Update";
			this.btnUpdateDestinationNodes.UseVisualStyleBackColor = true;
			this.btnUpdateDestinationNodes.Click += new System.EventHandler(this.btnUpdateDestinationNodes_Click);
			// 
			// btnUpdateStartNodes
			// 
			this.btnUpdateStartNodes.Location = new System.Drawing.Point(92, 366);
			this.btnUpdateStartNodes.Name = "btnUpdateStartNodes";
			this.btnUpdateStartNodes.Size = new System.Drawing.Size(60, 23);
			this.btnUpdateStartNodes.TabIndex = 4;
			this.btnUpdateStartNodes.Text = "Update";
			this.btnUpdateStartNodes.UseVisualStyleBackColor = true;
			this.btnUpdateStartNodes.Click += new System.EventHandler(this.btnUpdateStartNodes_Click);
			// 
			// btnSetDestinationTitle
			// 
			this.btnSetDestinationTitle.Location = new System.Drawing.Point(378, 264);
			this.btnSetDestinationTitle.Name = "btnSetDestinationTitle";
			this.btnSetDestinationTitle.Size = new System.Drawing.Size(40, 23);
			this.btnSetDestinationTitle.TabIndex = 8;
			this.btnSetDestinationTitle.Text = "Set";
			this.btnSetDestinationTitle.UseVisualStyleBackColor = true;
			this.btnSetDestinationTitle.Click += new System.EventHandler(this.btnSetDestinationTitle_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Start Nodes:";
			// 
			// btnSetStartTitle
			// 
			this.btnSetStartTitle.Location = new System.Drawing.Point(178, 338);
			this.btnSetStartTitle.Name = "btnSetStartTitle";
			this.btnSetStartTitle.Size = new System.Drawing.Size(40, 23);
			this.btnSetStartTitle.TabIndex = 2;
			this.btnSetStartTitle.Text = "Set";
			this.btnSetStartTitle.UseVisualStyleBackColor = true;
			this.btnSetStartTitle.Click += new System.EventHandler(this.btnSetStartTitle_Click);
			// 
			// lbStartNodes
			// 
			this.lbStartNodes.FormattingEnabled = true;
			this.lbStartNodes.Location = new System.Drawing.Point(12, 44);
			this.lbStartNodes.Name = "lbStartNodes";
			this.lbStartNodes.Size = new System.Drawing.Size(206, 290);
			this.lbStartNodes.TabIndex = 0;
			this.lbStartNodes.SelectedIndexChanged += new System.EventHandler(this.lbStartNodes_SelectedIndexChanged);
			// 
			// lblDestinationTitle
			// 
			this.lblDestinationTitle.AutoSize = true;
			this.lblDestinationTitle.Location = new System.Drawing.Point(216, 269);
			this.lblDestinationTitle.Name = "lblDestinationTitle";
			this.lblDestinationTitle.Size = new System.Drawing.Size(30, 13);
			this.lblDestinationTitle.TabIndex = 11;
			this.lblDestinationTitle.Text = "Title:";
			// 
			// lbDestinationNodes
			// 
			this.lbDestinationNodes.FormattingEnabled = true;
			this.lbDestinationNodes.Location = new System.Drawing.Point(270, 74);
			this.lbDestinationNodes.Name = "lbDestinationNodes";
			this.lbDestinationNodes.Size = new System.Drawing.Size(148, 186);
			this.lbDestinationNodes.TabIndex = 6;
			this.lbDestinationNodes.SelectedIndexChanged += new System.EventHandler(this.lbDestinationNodes_SelectedIndexChanged);
			// 
			// lblStartTitle
			// 
			this.lblStartTitle.AutoSize = true;
			this.lblStartTitle.Location = new System.Drawing.Point(16, 343);
			this.lblStartTitle.Name = "lblStartTitle";
			this.lblStartTitle.Size = new System.Drawing.Size(30, 13);
			this.lblStartTitle.TabIndex = 10;
			this.lblStartTitle.Text = "Title:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(267, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(97, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Destination Nodes:";
			// 
			// editDestinationNodeTitle
			// 
			this.editDestinationNodeTitle.Location = new System.Drawing.Point(252, 266);
			this.editDestinationNodeTitle.Name = "editDestinationNodeTitle";
			this.editDestinationNodeTitle.Size = new System.Drawing.Size(120, 20);
			this.editDestinationNodeTitle.TabIndex = 7;
			// 
			// btnAddStartNode
			// 
			this.btnAddStartNode.Location = new System.Drawing.Point(26, 366);
			this.btnAddStartNode.Name = "btnAddStartNode";
			this.btnAddStartNode.Size = new System.Drawing.Size(60, 23);
			this.btnAddStartNode.TabIndex = 3;
			this.btnAddStartNode.Text = "Add";
			this.btnAddStartNode.UseVisualStyleBackColor = true;
			this.btnAddStartNode.Click += new System.EventHandler(this.btnAddStartNode_Click);
			// 
			// editStartNodeTitle
			// 
			this.editStartNodeTitle.Location = new System.Drawing.Point(52, 340);
			this.editStartNodeTitle.Name = "editStartNodeTitle";
			this.editStartNodeTitle.Size = new System.Drawing.Size(120, 20);
			this.editStartNodeTitle.TabIndex = 1;
			// 
			// btnRemoveStartNode
			// 
			this.btnRemoveStartNode.Location = new System.Drawing.Point(158, 366);
			this.btnRemoveStartNode.Name = "btnRemoveStartNode";
			this.btnRemoveStartNode.Size = new System.Drawing.Size(60, 23);
			this.btnRemoveStartNode.TabIndex = 5;
			this.btnRemoveStartNode.Text = "Remove";
			this.btnRemoveStartNode.UseVisualStyleBackColor = true;
			this.btnRemoveStartNode.Click += new System.EventHandler(this.btnRemoveStartNode_Click);
			// 
			// btnRemoveDestinationNode
			// 
			this.btnRemoveDestinationNode.Location = new System.Drawing.Point(358, 292);
			this.btnRemoveDestinationNode.Name = "btnRemoveDestinationNode";
			this.btnRemoveDestinationNode.Size = new System.Drawing.Size(60, 23);
			this.btnRemoveDestinationNode.TabIndex = 11;
			this.btnRemoveDestinationNode.Text = "Remove";
			this.btnRemoveDestinationNode.UseVisualStyleBackColor = true;
			this.btnRemoveDestinationNode.Click += new System.EventHandler(this.btnRemoveDestinationNode_Click);
			this.btnRemoveDestinationNode.SizeChanged += new System.EventHandler(this.btnRemoveDestinationNode_SizeChanged);
			// 
			// btnAddDestinationNode
			// 
			this.btnAddDestinationNode.Location = new System.Drawing.Point(226, 292);
			this.btnAddDestinationNode.Name = "btnAddDestinationNode";
			this.btnAddDestinationNode.Size = new System.Drawing.Size(60, 23);
			this.btnAddDestinationNode.TabIndex = 9;
			this.btnAddDestinationNode.Text = "Add";
			this.btnAddDestinationNode.UseVisualStyleBackColor = true;
			this.btnAddDestinationNode.Click += new System.EventHandler(this.btnAddDestinationNode_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.lblNumStops);
			this.groupBox3.Controls.Add(this.lblNumVehicles);
			this.groupBox3.Controls.Add(this.lblTravelTime);
			this.groupBox3.Controls.Add(this.lblVelocity);
			this.groupBox3.Controls.Add(this.lblMilage);
			this.groupBox3.Location = new System.Drawing.Point(3, 293);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(250, 153);
			this.groupBox3.TabIndex = 23;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Statistics";
			// 
			// lblNumStops
			// 
			this.lblNumStops.AutoSize = true;
			this.lblNumStops.Location = new System.Drawing.Point(6, 125);
			this.lblNumStops.Name = "lblNumStops";
			this.lblNumStops.Size = new System.Drawing.Size(141, 13);
			this.lblNumStops.TabIndex = 4;
			this.lblNumStops.Text = "Average Number of Stops: 0";
			// 
			// lblNumVehicles
			// 
			this.lblNumVehicles.AutoSize = true;
			this.lblNumVehicles.Location = new System.Drawing.Point(6, 21);
			this.lblNumVehicles.Name = "lblNumVehicles";
			this.lblNumVehicles.Size = new System.Drawing.Size(86, 13);
			this.lblNumVehicles.TabIndex = 3;
			this.lblNumVehicles.Text = "Total Vehicles: 0";
			// 
			// lblTravelTime
			// 
			this.lblTravelTime.AutoSize = true;
			this.lblTravelTime.Location = new System.Drawing.Point(6, 73);
			this.lblTravelTime.Name = "lblTravelTime";
			this.lblTravelTime.Size = new System.Drawing.Size(123, 13);
			this.lblTravelTime.TabIndex = 2;
			this.lblTravelTime.Text = "Average Travel Time: 0s";
			// 
			// lblVelocity
			// 
			this.lblVelocity.AutoSize = true;
			this.lblVelocity.Location = new System.Drawing.Point(6, 99);
			this.lblVelocity.Name = "lblVelocity";
			this.lblVelocity.Size = new System.Drawing.Size(117, 13);
			this.lblVelocity.TabIndex = 1;
			this.lblVelocity.Text = "Average Velocity: 0m/s";
			// 
			// lblMilage
			// 
			this.lblMilage.AutoSize = true;
			this.lblMilage.Location = new System.Drawing.Point(6, 47);
			this.lblMilage.Name = "lblMilage";
			this.lblMilage.Size = new System.Drawing.Size(101, 13);
			this.lblMilage.TabIndex = 0;
			this.lblMilage.Text = "Average Milage: 0m";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.spinTramsTargetVelocity);
			this.groupBox2.Controls.Add(this.spinBussesTargetVelocity);
			this.groupBox2.Controls.Add(this.spinTrucksTargetVelocity);
			this.groupBox2.Controls.Add(this.spinCarsTargetVelocity);
			this.groupBox2.Controls.Add(this.spinGlobalTrafficVolumeMultiplier);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Location = new System.Drawing.Point(3, 135);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(250, 152);
			this.groupBox2.TabIndex = 22;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Global Settings:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(6, 125);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(135, 13);
			this.label11.TabIndex = 29;
			this.label11.Text = "Tram Target Velocity (m/s):";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(6, 99);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(129, 13);
			this.label10.TabIndex = 28;
			this.label10.Text = "Bus Target Velocity (m/s):";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(6, 73);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(139, 13);
			this.label9.TabIndex = 27;
			this.label9.Text = "Truck Target Velocity (m/s):";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(6, 47);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(127, 13);
			this.label8.TabIndex = 26;
			this.label8.Text = "Car Target Velocity (m/s):";
			// 
			// spinTramsTargetVelocity
			// 
			this.spinTramsTargetVelocity.Location = new System.Drawing.Point(161, 123);
			this.spinTramsTargetVelocity.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.spinTramsTargetVelocity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.spinTramsTargetVelocity.Name = "spinTramsTargetVelocity";
			this.spinTramsTargetVelocity.Size = new System.Drawing.Size(83, 20);
			this.spinTramsTargetVelocity.TabIndex = 25;
			this.spinTramsTargetVelocity.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
			this.spinTramsTargetVelocity.ValueChanged += new System.EventHandler(this.spinTramsTargetVelocity_ValueChanged);
			// 
			// spinBussesTargetVelocity
			// 
			this.spinBussesTargetVelocity.Location = new System.Drawing.Point(161, 97);
			this.spinBussesTargetVelocity.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.spinBussesTargetVelocity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.spinBussesTargetVelocity.Name = "spinBussesTargetVelocity";
			this.spinBussesTargetVelocity.Size = new System.Drawing.Size(83, 20);
			this.spinBussesTargetVelocity.TabIndex = 24;
			this.spinBussesTargetVelocity.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
			this.spinBussesTargetVelocity.ValueChanged += new System.EventHandler(this.spinBussesTargetVelocity_ValueChanged);
			// 
			// spinTrucksTargetVelocity
			// 
			this.spinTrucksTargetVelocity.Location = new System.Drawing.Point(161, 71);
			this.spinTrucksTargetVelocity.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.spinTrucksTargetVelocity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.spinTrucksTargetVelocity.Name = "spinTrucksTargetVelocity";
			this.spinTrucksTargetVelocity.Size = new System.Drawing.Size(83, 20);
			this.spinTrucksTargetVelocity.TabIndex = 23;
			this.spinTrucksTargetVelocity.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
			this.spinTrucksTargetVelocity.ValueChanged += new System.EventHandler(this.spinTrucksTargetVelocity_ValueChanged);
			// 
			// spinCarsTargetVelocity
			// 
			this.spinCarsTargetVelocity.Location = new System.Drawing.Point(161, 45);
			this.spinCarsTargetVelocity.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.spinCarsTargetVelocity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.spinCarsTargetVelocity.Name = "spinCarsTargetVelocity";
			this.spinCarsTargetVelocity.Size = new System.Drawing.Size(83, 20);
			this.spinCarsTargetVelocity.TabIndex = 22;
			this.spinCarsTargetVelocity.Value = new decimal(new int[] {
            36,
            0,
            0,
            0});
			this.spinCarsTargetVelocity.ValueChanged += new System.EventHandler(this.spinCarsTargetVelocity_ValueChanged);
			// 
			// spinGlobalTrafficVolumeMultiplier
			// 
			this.spinGlobalTrafficVolumeMultiplier.DecimalPlaces = 1;
			this.spinGlobalTrafficVolumeMultiplier.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.spinGlobalTrafficVolumeMultiplier.Location = new System.Drawing.Point(161, 19);
			this.spinGlobalTrafficVolumeMultiplier.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.spinGlobalTrafficVolumeMultiplier.Name = "spinGlobalTrafficVolumeMultiplier";
			this.spinGlobalTrafficVolumeMultiplier.Size = new System.Drawing.Size(83, 20);
			this.spinGlobalTrafficVolumeMultiplier.TabIndex = 20;
			this.spinGlobalTrafficVolumeMultiplier.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.spinGlobalTrafficVolumeMultiplier.ValueChanged += new System.EventHandler(this.spinGlobalTrafficVolumeMultiplier_ValueChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 21);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(155, 13);
			this.label7.TabIndex = 21;
			this.label7.Text = "Global Traffic Volume Multiplier:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.spinTruckVolume);
			this.groupBox1.Controls.Add(this.spinTramVolume);
			this.groupBox1.Controls.Add(this.spinBusVolume);
			this.groupBox1.Controls.Add(this.spinCarsVolume);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(250, 126);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Per-Route Traffic Volume:";
			this.groupBox1.SizeChanged += new System.EventHandler(this.groupBox1_SizeChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 99);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(67, 13);
			this.label6.TabIndex = 7;
			this.label6.Text = "Trams/Hour:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 73);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "Busses/Hour:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 47);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(71, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Trucks/Hour:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Cars/Hour:";
			// 
			// spinTruckVolume
			// 
			this.spinTruckVolume.Location = new System.Drawing.Point(161, 45);
			this.spinTruckVolume.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.spinTruckVolume.Name = "spinTruckVolume";
			this.spinTruckVolume.Size = new System.Drawing.Size(83, 20);
			this.spinTruckVolume.TabIndex = 22;
			this.spinTruckVolume.ValueChanged += new System.EventHandler(this.spinTruckVolume_ValueChanged);
			// 
			// spinTramVolume
			// 
			this.spinTramVolume.Location = new System.Drawing.Point(161, 97);
			this.spinTramVolume.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.spinTramVolume.Name = "spinTramVolume";
			this.spinTramVolume.Size = new System.Drawing.Size(83, 20);
			this.spinTramVolume.TabIndex = 24;
			this.spinTramVolume.ValueChanged += new System.EventHandler(this.spinTramVolume_ValueChanged);
			// 
			// spinBusVolume
			// 
			this.spinBusVolume.Location = new System.Drawing.Point(161, 71);
			this.spinBusVolume.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.spinBusVolume.Name = "spinBusVolume";
			this.spinBusVolume.Size = new System.Drawing.Size(83, 20);
			this.spinBusVolume.TabIndex = 23;
			this.spinBusVolume.ValueChanged += new System.EventHandler(this.spinBusVolume_ValueChanged);
			// 
			// spinCarsVolume
			// 
			this.spinCarsVolume.Location = new System.Drawing.Point(161, 19);
			this.spinCarsVolume.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.spinCarsVolume.Name = "spinCarsVolume";
			this.spinCarsVolume.Size = new System.Drawing.Size(83, 20);
			this.spinCarsVolume.TabIndex = 21;
			this.spinCarsVolume.ValueChanged += new System.EventHandler(this.spinCarsVolume_ValueChanged);
			// 
			// TrafficVolumeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(890, 459);
			this.ControlBox = false;
			this.Controls.Add(this.splitContainer1);
			this.Name = "TrafficVolumeForm";
			this.Text = "Traffic Volume Editor";
			this.Load += new System.EventHandler(this.TrafficVolumeForm_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spinTramsTargetVelocity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinBussesTargetVelocity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinTrucksTargetVelocity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinCarsTargetVelocity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinGlobalTrafficVolumeMultiplier)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spinTruckVolume)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinTramVolume)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinBusVolume)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinCarsVolume)).EndInit();
			this.ResumeLayout(false);

			}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox lbStartNodes;
		private System.Windows.Forms.ListBox lbDestinationNodes;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnRemoveDestinationNode;
		private System.Windows.Forms.Button btnAddDestinationNode;
		private System.Windows.Forms.Button btnRemoveStartNode;
		private System.Windows.Forms.Button btnAddStartNode;
		private System.Windows.Forms.TextBox editDestinationNodeTitle;
		private System.Windows.Forms.TextBox editStartNodeTitle;
		private System.Windows.Forms.Label lblDestinationTitle;
		private System.Windows.Forms.Label lblStartTitle;
		private System.Windows.Forms.Button btnSetDestinationTitle;
		private System.Windows.Forms.Button btnSetStartTitle;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown spinBusVolume;
		private System.Windows.Forms.NumericUpDown spinCarsVolume;
		private System.Windows.Forms.NumericUpDown spinTramVolume;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown spinTruckVolume;
		private System.Windows.Forms.NumericUpDown spinGlobalTrafficVolumeMultiplier;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button btnUpdateDestinationNodes;
		private System.Windows.Forms.Button btnUpdateStartNodes;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDown spinTramsTargetVelocity;
		private System.Windows.Forms.NumericUpDown spinBussesTargetVelocity;
		private System.Windows.Forms.NumericUpDown spinTrucksTargetVelocity;
		private System.Windows.Forms.NumericUpDown spinCarsTargetVelocity;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label lblTravelTime;
		private System.Windows.Forms.Label lblVelocity;
		private System.Windows.Forms.Label lblMilage;
		private System.Windows.Forms.Label lblNumVehicles;
		private System.Windows.Forms.Label lblNumStops;
		}
	}