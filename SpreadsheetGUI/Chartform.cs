//Author: Logan Gore
using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Form for creating bar graphs
    /// </summary>
    public partial class ChartForm : Form
    {
        /// <summary>
        /// Spreadsheet passed to graph for use of graphing
        /// </summary>
        private Spreadsheet ss;

        public ChartForm(Spreadsheet ss)
        {
            InitializeComponent();
            this.ss = ss;
        }

        /// <summary>
        /// Event handler for the load graph button
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void loadGraphButton_Click(object sender, EventArgs e)
        {
            try
            {     
                // Example idea taken from http://www.dotnetperls.com/chart
                //Edited for the use of this program

                string[] barNames = barNameTextbox.Text.Split(',');

                List<double> points = new List<double>();

                string[] cellNames = cellNameTextbox.Text.Split(',');

                //Get each value out of the cell for the points
                foreach (String cell in cellNames)
                {
                    points.Add((Double)ss.GetCellValue(cell.Trim()));
                }

                // Set title.
                barChart.Titles.Add(chartNameTextbox.Text);


                // Add series.
                for (int i = 0; i < barNames.Length; i++)
                {
                    Series series;
  
                    if (Regex.IsMatch(barNames[i].Trim(), @"^[a-zA-Z]+[0-9]+$"))
                        //Retrive cell value to use as label for bar, must trim to get rid of white space
                        series = barChart.Series.Add(ss.GetCellValue(barNames[i].Trim()).ToString());
                    else
                        series = barChart.Series.Add(barNames[i]);


                    // Add point.
                    series.Points.Add(points[i]);
                }

            }
            catch
            {
                MessageBox.Show("Invalid information given.", "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
            }

            //Graph is made hide controls and grow the size
            barChart.Height = 400;
            controlGroup.Hide();
        }
    }
}
