namespace CityTrafficSimulator.Verkehr
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
			this.thumbGrid = new CityTrafficSimulator.RechenkaestchenControl();
			this.spinGlobalTrafficVolumeMultiplier = new System.Windows.Forms.NumericUpDown();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.spinTruckVolume = new System.Windows.Forms.NumericUpDown();
			this.spinTramVolume = new System.Windows.Forms.NumericUpDown();
			this.spinBusVolume = new System.Windows.Forms.NumericUpDown();
			this.spinCarsVolume = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
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
			this.splitContainer1.Panel2.Controls.Add(this.thumbGrid);
			this.splitContainer1.Panel2.Controls.Add(this.spinGlobalTrafficVolumeMultiplier);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel2.Controls.Add(this.label7);
			this.splitContainer1.Size = new System.Drawing.Size(890, 484);
			this.splitContainer1.SplitterDistance = 590;
			this.splitContainer1.TabIndex = 0;
			// 
			// btnSetDestinationTitle
			// 
			this.btnSetDestinationTitle.Location = new System.Drawing.Point(378, 264);
			this.btnSetDestinationTitle.Name = "btnSetDestinationTitle";
			this.btnSetDestinationTitle.Size = new System.Drawing.Size(40, 23);
			this.btnSetDestinationTitle.TabIndex = 13;
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
			this.btnSetStartTitle.TabIndex = 12;
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
			this.lbDestinationNodes.TabIndex = 1;
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
			this.editDestinationNodeTitle.TabIndex = 9;
			// 
			// btnAddStartNode
			// 
			this.btnAddStartNode.Location = new System.Drawing.Point(52, 366);
			this.btnAddStartNode.Name = "btnAddStartNode";
			this.btnAddStartNode.Size = new System.Drawing.Size(80, 23);
			this.btnAddStartNode.TabIndex = 4;
			this.btnAddStartNode.Text = "Add";
			this.btnAddStartNode.UseVisualStyleBackColor = true;
			this.btnAddStartNode.Click += new System.EventHandler(this.btnAddStartNode_Click);
			// 
			// editStartNodeTitle
			// 
			this.editStartNodeTitle.Location = new System.Drawing.Point(52, 340);
			this.editStartNodeTitle.Name = "editStartNodeTitle";
			this.editStartNodeTitle.Size = new System.Drawing.Size(120, 20);
			this.editStartNodeTitle.TabIndex = 8;
			// 
			// btnRemoveStartNode
			// 
			this.btnRemoveStartNode.Location = new System.Drawing.Point(138, 366);
			this.btnRemoveStartNode.Name = "btnRemoveStartNode";
			this.btnRemoveStartNode.Size = new System.Drawing.Size(80, 23);
			this.btnRemoveStartNode.TabIndex = 5;
			this.btnRemoveStartNode.Text = "Remove";
			this.btnRemoveStartNode.UseVisualStyleBackColor = true;
			this.btnRemoveStartNode.Click += new System.EventHandler(this.btnRemoveStartNode_Click);
			// 
			// btnRemoveDestinationNode
			// 
			this.btnRemoveDestinationNode.Location = new System.Drawing.Point(338, 292);
			this.btnRemoveDestinationNode.Name = "btnRemoveDestinationNode";
			this.btnRemoveDestinationNode.Size = new System.Drawing.Size(80, 23);
			this.btnRemoveDestinationNode.TabIndex = 7;
			this.btnRemoveDestinationNode.Text = "Remove";
			this.btnRemoveDestinationNode.UseVisualStyleBackColor = true;
			this.btnRemoveDestinationNode.Click += new System.EventHandler(this.btnRemoveDestinationNode_Click);
			// 
			// btnAddDestinationNode
			// 
			this.btnAddDestinationNode.Location = new System.Drawing.Point(252, 292);
			this.btnAddDestinationNode.Name = "btnAddDestinationNode";
			this.btnAddDestinationNode.Size = new System.Drawing.Size(80, 23);
			this.btnAddDestinationNode.TabIndex = 6;
			this.btnAddDestinationNode.Text = "Add";
			this.btnAddDestinationNode.UseVisualStyleBackColor = true;
			this.btnAddDestinationNode.Click += new System.EventHandler(this.btnAddDestinationNode_Click);
			// 
			// thumbGrid
			// 
			this.thumbGrid.BackColor = System.Drawing.Color.White;
			this.thumbGrid.CellHeight = 0;
			this.thumbGrid.CellSize = new System.Drawing.Size(0, 0);
			this.thumbGrid.CellWidth = 0;
			this.thumbGrid.Dimension = new System.Drawing.Point(0, 0);
			this.thumbGrid.DrawGrid = false;
			this.thumbGrid.Location = new System.Drawing.Point(12, 3);
			this.thumbGrid.Max_X = 0;
			this.thumbGrid.Max_Y = 0;
			this.thumbGrid.Name = "thumbGrid";
			this.thumbGrid.Size = new System.Drawing.Size(272, 272);
			this.thumbGrid.TabIndex = 23;
			this.thumbGrid.Paint += new System.Windows.Forms.PaintEventHandler(this.thumbGrid_Paint);
			// 
			// spinGlobalTrafficVolumeMultiplier
			// 
			this.spinGlobalTrafficVolumeMultiplier.DecimalPlaces = 1;
			this.spinGlobalTrafficVolumeMultiplier.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.spinGlobalTrafficVolumeMultiplier.Location = new System.Drawing.Point(179, 281);
			this.spinGlobalTrafficVolumeMultiplier.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.spinGlobalTrafficVolumeMultiplier.Name = "spinGlobalTrafficVolumeMultiplier";
			this.spinGlobalTrafficVolumeMultiplier.Size = new System.Drawing.Size(99, 20);
			this.spinGlobalTrafficVolumeMultiplier.TabIndex = 22;
			this.spinGlobalTrafficVolumeMultiplier.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.spinGlobalTrafficVolumeMultiplier.ValueChanged += new System.EventHandler(this.spinGlobalTrafficVolumeMultiplier_ValueChanged);
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
			this.groupBox1.Location = new System.Drawing.Point(12, 307);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(272, 138);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Traffic Volume:";
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
			this.spinTruckVolume.Location = new System.Drawing.Point(167, 45);
			this.spinTruckVolume.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.spinTruckVolume.Name = "spinTruckVolume";
			this.spinTruckVolume.Size = new System.Drawing.Size(99, 20);
			this.spinTruckVolume.TabIndex = 3;
			this.spinTruckVolume.ValueChanged += new System.EventHandler(this.spinTruckVolume_ValueChanged);
			// 
			// spinTramVolume
			// 
			this.spinTramVolume.Location = new System.Drawing.Point(167, 97);
			this.spinTramVolume.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.spinTramVolume.Name = "spinTramVolume";
			this.spinTramVolume.Size = new System.Drawing.Size(99, 20);
			this.spinTramVolume.TabIndex = 2;
			this.spinTramVolume.ValueChanged += new System.EventHandler(this.spinTramVolume_ValueChanged);
			// 
			// spinBusVolume
			// 
			this.spinBusVolume.Location = new System.Drawing.Point(167, 71);
			this.spinBusVolume.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.spinBusVolume.Name = "spinBusVolume";
			this.spinBusVolume.Size = new System.Drawing.Size(99, 20);
			this.spinBusVolume.TabIndex = 1;
			this.spinBusVolume.ValueChanged += new System.EventHandler(this.spinBusVolume_ValueChanged);
			// 
			// spinCarsVolume
			// 
			this.spinCarsVolume.Location = new System.Drawing.Point(167, 19);
			this.spinCarsVolume.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.spinCarsVolume.Name = "spinCarsVolume";
			this.spinCarsVolume.Size = new System.Drawing.Size(99, 20);
			this.spinCarsVolume.TabIndex = 0;
			this.spinCarsVolume.ValueChanged += new System.EventHandler(this.spinCarsVolume_ValueChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(18, 283);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(155, 13);
			this.label7.TabIndex = 21;
			this.label7.Text = "Global Traffic Volume Multiplier:";
			// 
			// TrafficVolumeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(890, 484);
			this.ControlBox = false;
			this.Controls.Add(this.splitContainer1);
			this.Name = "TrafficVolumeForm";
			this.Text = "Traffic Volume Editor";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
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
		private RechenkaestchenControl thumbGrid;
		}
	}