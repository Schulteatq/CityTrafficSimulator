namespace CityTrafficSimulator.Timeline
	{
	/// <summary>
	/// Forumular zur LSA-Steuerung
	/// </summary>
	partial class TrafficLightForm
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
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.zoomSpinEdit = new System.Windows.Forms.NumericUpDown();
			this.trafficLightGroupBox = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupComboBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.trafficLightNameEdit = new System.Windows.Forms.TextBox();
			this.removeTrafficLightButton = new System.Windows.Forms.Button();
			this.addTrafficLightButton = new System.Windows.Forms.Button();
			this.groupsGroupBox = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupTitleEdit = new System.Windows.Forms.TextBox();
			this.removeGroupButton = new System.Windows.Forms.Button();
			this.addGroupButton = new System.Windows.Forms.Button();
			this.timelineControl = new CityTrafficSimulator.TimelineControl();
			this.statusStrip1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomSpinEdit)).BeginInit();
			this.trafficLightGroupBox.SuspendLayout();
			this.groupsGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 306);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(881, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// statusLabel
			// 
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(9, 17);
			this.statusLabel.Text = " ";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel1.Controls.Add(this.trafficLightGroupBox);
			this.splitContainer1.Panel1.Controls.Add(this.groupsGroupBox);
			this.splitContainer1.Panel1MinSize = 340;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.Controls.Add(this.timelineControl);
			this.splitContainer1.Size = new System.Drawing.Size(881, 306);
			this.splitContainer1.SplitterDistance = 340;
			this.splitContainer1.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.zoomSpinEdit);
			this.groupBox1.Location = new System.Drawing.Point(12, 225);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(318, 72);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "View";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(125, 13);
			this.label4.TabIndex = 1;
			this.label4.Text = "Timeline Zoom (Pixels/s):";
			// 
			// zoomSpinEdit
			// 
			this.zoomSpinEdit.Location = new System.Drawing.Point(208, 19);
			this.zoomSpinEdit.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.zoomSpinEdit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.zoomSpinEdit.Name = "zoomSpinEdit";
			this.zoomSpinEdit.Size = new System.Drawing.Size(104, 20);
			this.zoomSpinEdit.TabIndex = 0;
			this.zoomSpinEdit.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.zoomSpinEdit.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// trafficLightGroupBox
			// 
			this.trafficLightGroupBox.Controls.Add(this.label3);
			this.trafficLightGroupBox.Controls.Add(this.groupComboBox);
			this.trafficLightGroupBox.Controls.Add(this.label2);
			this.trafficLightGroupBox.Controls.Add(this.trafficLightNameEdit);
			this.trafficLightGroupBox.Controls.Add(this.removeTrafficLightButton);
			this.trafficLightGroupBox.Controls.Add(this.addTrafficLightButton);
			this.trafficLightGroupBox.Location = new System.Drawing.Point(12, 103);
			this.trafficLightGroupBox.Name = "trafficLightGroupBox";
			this.trafficLightGroupBox.Size = new System.Drawing.Size(318, 116);
			this.trafficLightGroupBox.TabIndex = 1;
			this.trafficLightGroupBox.TabStop = false;
			this.trafficLightGroupBox.Text = "Signal";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Signal Group:";
			// 
			// groupComboBox
			// 
			this.groupComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.groupComboBox.FormattingEnabled = true;
			this.groupComboBox.Location = new System.Drawing.Point(98, 45);
			this.groupComboBox.Name = "groupComboBox";
			this.groupComboBox.Size = new System.Drawing.Size(214, 21);
			this.groupComboBox.TabIndex = 8;
			this.groupComboBox.SelectedIndexChanged += new System.EventHandler(this.groupComboBox_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Signal Name:";
			// 
			// trafficLightNameEdit
			// 
			this.trafficLightNameEdit.Location = new System.Drawing.Point(98, 19);
			this.trafficLightNameEdit.Name = "trafficLightNameEdit";
			this.trafficLightNameEdit.Size = new System.Drawing.Size(214, 20);
			this.trafficLightNameEdit.TabIndex = 6;
			this.trafficLightNameEdit.TextChanged += new System.EventHandler(this.trafficLightNameEdit_TextChanged);
			// 
			// removeTrafficLightButton
			// 
			this.removeTrafficLightButton.Location = new System.Drawing.Point(208, 72);
			this.removeTrafficLightButton.Name = "removeTrafficLightButton";
			this.removeTrafficLightButton.Size = new System.Drawing.Size(104, 23);
			this.removeTrafficLightButton.TabIndex = 5;
			this.removeTrafficLightButton.Text = "Remove Signal";
			this.removeTrafficLightButton.UseVisualStyleBackColor = true;
			this.removeTrafficLightButton.Click += new System.EventHandler(this.removeTrafficLightButton_Click);
			// 
			// addTrafficLightButton
			// 
			this.addTrafficLightButton.Location = new System.Drawing.Point(98, 72);
			this.addTrafficLightButton.Name = "addTrafficLightButton";
			this.addTrafficLightButton.Size = new System.Drawing.Size(104, 23);
			this.addTrafficLightButton.TabIndex = 4;
			this.addTrafficLightButton.Text = "Create Signal";
			this.addTrafficLightButton.UseVisualStyleBackColor = true;
			this.addTrafficLightButton.Click += new System.EventHandler(this.addTrafficLightButton_Click);
			// 
			// groupsGroupBox
			// 
			this.groupsGroupBox.Controls.Add(this.label1);
			this.groupsGroupBox.Controls.Add(this.groupTitleEdit);
			this.groupsGroupBox.Controls.Add(this.removeGroupButton);
			this.groupsGroupBox.Controls.Add(this.addGroupButton);
			this.groupsGroupBox.Location = new System.Drawing.Point(12, 12);
			this.groupsGroupBox.Name = "groupsGroupBox";
			this.groupsGroupBox.Size = new System.Drawing.Size(318, 85);
			this.groupsGroupBox.TabIndex = 0;
			this.groupsGroupBox.TabStop = false;
			this.groupsGroupBox.Text = "Signal Groups";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Group Name:";
			// 
			// groupTitleEdit
			// 
			this.groupTitleEdit.Location = new System.Drawing.Point(98, 19);
			this.groupTitleEdit.Name = "groupTitleEdit";
			this.groupTitleEdit.Size = new System.Drawing.Size(214, 20);
			this.groupTitleEdit.TabIndex = 2;
			this.groupTitleEdit.TextChanged += new System.EventHandler(this.groupTitleEdit_TextChanged);
			// 
			// removeGroupButton
			// 
			this.removeGroupButton.Location = new System.Drawing.Point(208, 45);
			this.removeGroupButton.Name = "removeGroupButton";
			this.removeGroupButton.Size = new System.Drawing.Size(104, 23);
			this.removeGroupButton.TabIndex = 1;
			this.removeGroupButton.Text = "Remove Group";
			this.removeGroupButton.UseVisualStyleBackColor = true;
			this.removeGroupButton.Click += new System.EventHandler(this.removeGroupButton_Click);
			// 
			// addGroupButton
			// 
			this.addGroupButton.Location = new System.Drawing.Point(98, 45);
			this.addGroupButton.Name = "addGroupButton";
			this.addGroupButton.Size = new System.Drawing.Size(104, 23);
			this.addGroupButton.TabIndex = 0;
			this.addGroupButton.Text = "Create Group";
			this.addGroupButton.UseVisualStyleBackColor = true;
			this.addGroupButton.Click += new System.EventHandler(this.addGroupButton_Click);
			// 
			// timelineControl
			// 
			this.timelineControl.Location = new System.Drawing.Point(0, 0);
			this.timelineControl.Name = "timelineControl";
			this.timelineControl.selectedEntry = null;
			this.timelineControl.selectedGroup = null;
			this.timelineControl.Size = new System.Drawing.Size(435, 147);
			this.timelineControl.snapSize = 0.5;
			this.timelineControl.steuerung = null;
			this.timelineControl.TabIndex = 0;
			this.timelineControl.Text = "timelineControl";
			this.timelineControl.zoom = 10;
			this.timelineControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.timelineControl_MouseMove);
			this.timelineControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.timelineControl_KeyUp);
			this.timelineControl.EventChanged += new CityTrafficSimulator.TimelineControl.EventChangedEventHandler(this.timelineControl_EventChanged);
			this.timelineControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.timelineControl_KeyDown);
			// 
			// TrafficLightForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(881, 328);
			this.ControlBox = false;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.statusStrip1);
			this.Name = "TrafficLightForm";
			this.Text = "TrafficLightForm";
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoomSpinEdit)).EndInit();
			this.trafficLightGroupBox.ResumeLayout(false);
			this.trafficLightGroupBox.PerformLayout();
			this.groupsGroupBox.ResumeLayout(false);
			this.groupsGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private TimelineControl timelineControl;
		private System.Windows.Forms.GroupBox trafficLightGroupBox;
		private System.Windows.Forms.GroupBox groupsGroupBox;
		private System.Windows.Forms.Button addGroupButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox groupTitleEdit;
		private System.Windows.Forms.Button removeGroupButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox trafficLightNameEdit;
		private System.Windows.Forms.Button removeTrafficLightButton;
		private System.Windows.Forms.Button addTrafficLightButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox groupComboBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown zoomSpinEdit;
		private System.Windows.Forms.ToolStripStatusLabel statusLabel;

		}
	}