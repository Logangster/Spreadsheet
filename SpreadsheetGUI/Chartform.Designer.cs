namespace SpreadsheetGUI
{
    partial class ChartForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.barChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.cellBarsLabel = new System.Windows.Forms.Label();
            this.barNameLabel = new System.Windows.Forms.Label();
            this.axisNameLabel = new System.Windows.Forms.Label();
            this.cellNameTextbox = new System.Windows.Forms.TextBox();
            this.barNameTextbox = new System.Windows.Forms.TextBox();
            this.chartNameTextbox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.controlGroup = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.barChart)).BeginInit();
            this.controlGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // barChart
            // 
            chartArea1.Name = "ChartArea1";
            this.barChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.barChart.Legends.Add(legend1);
            this.barChart.Location = new System.Drawing.Point(27, 13);
            this.barChart.Name = "barChart";
            this.barChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Fire;
            this.barChart.Size = new System.Drawing.Size(473, 312);
            this.barChart.TabIndex = 0;
            this.barChart.Text = "chart1";
            // 
            // cellBarsLabel
            // 
            this.cellBarsLabel.AutoSize = true;
            this.cellBarsLabel.Location = new System.Drawing.Point(0, 0);
            this.cellBarsLabel.Name = "cellBarsLabel";
            this.cellBarsLabel.Size = new System.Drawing.Size(210, 13);
            this.cellBarsLabel.TabIndex = 1;
            this.cellBarsLabel.Text = "Cells to use as bars(separated by commas):";
            // 
            // barNameLabel
            // 
            this.barNameLabel.AutoSize = true;
            this.barNameLabel.Location = new System.Drawing.Point(0, 24);
            this.barNameLabel.Name = "barNameLabel";
            this.barNameLabel.Size = new System.Drawing.Size(259, 13);
            this.barNameLabel.TabIndex = 2;
            this.barNameLabel.Text = "Names for bars(separated by commas, may use cells):";
            // 
            // axisNameLabel
            // 
            this.axisNameLabel.AutoSize = true;
            this.axisNameLabel.Location = new System.Drawing.Point(0, 48);
            this.axisNameLabel.Name = "axisNameLabel";
            this.axisNameLabel.Size = new System.Drawing.Size(83, 13);
            this.axisNameLabel.TabIndex = 3;
            this.axisNameLabel.Text = "Name for graph:";
            // 
            // cellNameTextbox
            // 
            this.cellNameTextbox.Location = new System.Drawing.Point(217, 0);
            this.cellNameTextbox.Name = "cellNameTextbox";
            this.cellNameTextbox.Size = new System.Drawing.Size(189, 20);
            this.cellNameTextbox.TabIndex = 4;
            // 
            // barNameTextbox
            // 
            this.barNameTextbox.Location = new System.Drawing.Point(265, 24);
            this.barNameTextbox.Name = "barNameTextbox";
            this.barNameTextbox.Size = new System.Drawing.Size(173, 20);
            this.barNameTextbox.TabIndex = 5;
            // 
            // chartNameTextbox
            // 
            this.chartNameTextbox.Location = new System.Drawing.Point(86, 48);
            this.chartNameTextbox.Name = "chartNameTextbox";
            this.chartNameTextbox.Size = new System.Drawing.Size(189, 20);
            this.chartNameTextbox.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(106, 77);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Load Graph";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.loadGraphButton_Click);
            // 
            // controlGroup
            // 
            this.controlGroup.Controls.Add(this.chartNameTextbox);
            this.controlGroup.Controls.Add(this.button1);
            this.controlGroup.Controls.Add(this.barNameTextbox);
            this.controlGroup.Controls.Add(this.cellNameTextbox);
            this.controlGroup.Controls.Add(this.axisNameLabel);
            this.controlGroup.Controls.Add(this.barNameLabel);
            this.controlGroup.Controls.Add(this.cellBarsLabel);
            this.controlGroup.Location = new System.Drawing.Point(27, 339);
            this.controlGroup.Name = "controlGroup";
            this.controlGroup.Size = new System.Drawing.Size(444, 106);
            this.controlGroup.TabIndex = 8;
            this.controlGroup.TabStop = false;
            // 
            // ChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 457);
            this.Controls.Add(this.controlGroup);
            this.Controls.Add(this.barChart);
            this.Name = "ChartForm";
            this.Text = "Bar Graph";
            ((System.ComponentModel.ISupportInitialize)(this.barChart)).EndInit();
            this.controlGroup.ResumeLayout(false);
            this.controlGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart barChart;
        private System.Windows.Forms.Label cellBarsLabel;
        private System.Windows.Forms.Label barNameLabel;
        private System.Windows.Forms.Label axisNameLabel;
        private System.Windows.Forms.TextBox cellNameTextbox;
        private System.Windows.Forms.TextBox barNameTextbox;
        private System.Windows.Forms.TextBox chartNameTextbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox controlGroup;
    }
}