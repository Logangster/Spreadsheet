//Author: Logan Gore

using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Main Form for the spreadsheet program
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Keeps track of spreadsheet
        /// </summary>
        private Spreadsheet spreadSheet;

        /// <summary>
        /// Keeps track of the last edited cell
        /// 
        /// Invariant: Make sure the value is cleared everytime a cell value is updated 
        /// or new spreadsheet is opened
        /// </summary>
        private string editedCell;

        /// <summary>
        /// Initialize new spreadsheet 
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            //Add event handler on click in spreadsheet panel
            spreadsheetPanel1.SelectionChanged += displayFormula;

            //Make sure user enters a valid cell i.e A1, cannot allow A0
            spreadSheet = new Spreadsheet(s => Regex.IsMatch(s, @"^[a-zA-Z]{1}[1-9]+[0-9]?$"), s => s.ToUpper(), "default");
        }

        /// <summary>
        /// Retrieves the name of the cell currently selected
        /// </summary>
        /// <returns>Name of cell currently selected</returns>
        private string GetCurrentCellName()
        {
            int row, col;
            spreadsheetPanel1.GetSelection(out col, out row);

            //Using ascii retrieve a letter from the column number
            Char columnName = Convert.ToChar(col + 65);
            row += 1;

            return "" + columnName + row;
        }

        /// <summary>
        /// Given a cell name, gives the row and column of the cell
        /// </summary>
        /// <param name="row">Row of the cell</param>
        /// <param name="col">Columnn of the cell</param>
        /// <param name="cellName">Name of the cell</param>
        private void GetRowAndCol(out int row, out int col, string cellName)
        {
            //Reverse the operation we used in GetCurrentCellName
            col = (int)cellName[0] - 65;
            row = Int32.Parse(cellName.Substring(1)) - 1;
        }

        /// <summary>
        /// Sets cell and all it's dependents
        /// </summary>
        /// <param name="cellName">Name of cell to set.</param>
        private void SetCells(string cellName)
        {
            //Get the row and column for the cell name sent to the method
            int row, col;
            GetRowAndCol(out row, out col, cellName);

            try
            {
                //Set the contents of cell to formula text box and retrieve a hashset of the cells to update
                HashSet<string> cellsToUpdate = new HashSet<string>(spreadSheet.SetContentsOfCell(cellName, formulaTextbox.Text));

                //Set value of the new updated cell in the SpreadsheetPanel
                spreadsheetPanel1.SetValue(col, row, "" + spreadSheet.GetCellValue(cellName));

                //Update each of the cells
                foreach (String cell in cellsToUpdate)
                {
                    GetRowAndCol(out row, out col, cell);
                    Char column = Convert.ToChar(col + 65);
                    spreadsheetPanel1.SetValue(col, row, "" + spreadSheet.GetCellValue("" + column + (row + 1)));
                }
            }
            //Display dialogs for each of the errors
            catch (InvalidNameException ex)
            {
                MessageBox.Show("Invalid cell name(s) given.", "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
            }
            catch (CircularException ex)
            {
                spreadsheetPanel1.SetValue(col, row, "");
                MessageBox.Show("Invalid formula, cannot contain circular dependencies.", "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
            }
            catch (Exception ex)
            {
                spreadsheetPanel1.SetValue(col, row, "");
                MessageBox.Show("Invalid formula.", "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Call back method when user selects a cell
        /// Updates last edited cell if necessary and displays the cells contents in the formula textbox
        /// </summary>
        /// <param name="sender"></param>
        private void displayFormula(SpreadsheetPanel sender)
        {
            if (formulaTextbox.Text != "" && editedCell != "")
            {
                SetCells(editedCell);
                //No longer an edited cell
                editedCell = "";
            }

            selectedCellTextbox.Text = GetCurrentCellName();

            object spreadsheetValue = spreadSheet.GetCellContents(GetCurrentCellName());

            // set the value of the spreadsheet based on if it's a formula, formula error, or double/string
            if (spreadsheetValue is Formula)
                formulaTextbox.Text = "=" + spreadsheetValue;
            else
                formulaTextbox.Text = spreadsheetValue.ToString();

            //Place the cursor at the end of the contents inserted into the formulaTextbox
            formulaTextbox.SelectionStart = formulaTextbox.Text.Length + 1;

            //User selected cell focus to text box so that they may enter contents
            formulaTextbox.Focus();
        }

        /// <summary>
        /// Refreshes the spreadsheet values with the given cells
        /// </summary>
        /// <param name="cells">Cells to be refreshed</param>
        private void refreshSpreadsheet(HashSet<string> cells)
        {
            int col, row;

            //Update each cell value in spreadsheet panel
            foreach (String cell in cells)
            {
                GetRowAndCol(out row, out col, cell);

                spreadsheetPanel1.SetValue(col, row, spreadSheet.GetCellValue(cell).ToString());
            }
        }

        /// <summary>
        /// Event handler for the "new" menu item
        /// </summary>
        /// <param name="sender">Object sending event</param>
        /// <param name="e">Event arguments</param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.getAppContext().RunForm(new MainForm());
        }

        /// <summary>
        /// Event handler for the "open" menu item
        /// </summary>
        /// <param name="sender">Object sending event</param>
        /// <param name="e">Event arguments</param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Found OpenFileDialog example from MSDN

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //Set the file extensions the open dialog shows
            openFileDialog1.Filter = "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
            //Show sprd files by default
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Update spreadsheet, clear, and then refresh the cell values, have to get saved version so that the versions match
                    spreadSheet = new Spreadsheet(openFileDialog1.FileName, s => true, s => s.ToUpper(), 
                        spreadSheet.GetSavedVersion(openFileDialog1.FileName));

                    spreadsheetPanel1.Clear();
                    refreshSpreadsheet(new HashSet<string>(spreadSheet.GetNamesOfAllNonemptyCells()));
                    //Make sure we erased the editedCell since no cells are edited now
                    editedCell = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Couldn't read file.", "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                }
            }
        }

        /// <summary>
        /// Event handler for when user types in formula textbox
        /// </summary>
        /// <param name="sender">Object sending event</param>
        /// <param name="e">Event arguments</param>
        private void formulaTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            //keep track of edited cell, so that if the user clicks away the value is still saved into cell
            editedCell = GetCurrentCellName();

            int row, col;
            String value;
            spreadsheetPanel1.GetSelection(out col, out row);
            spreadsheetPanel1.GetValue(col, row, out value);

            //Enables the user to see what they type in the formulaTextbox inserted into the cell as they type it
            spreadsheetPanel1.SetValue(col, row, formulaTextbox.Text + Convert.ToChar(e.KeyValue));

            if (e.KeyCode == Keys.Enter)
            {
                SetCells(editedCell);

                //Get rid of that horrid ding sound
                e.Handled = true;
                e.SuppressKeyPress = true;
            }


        }

        /// <summary>
        /// Event handler for the "save" menu item
        /// </summary>
        /// <param name="sender">Object sending event</param>
        /// <param name="e">Event arguments</param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Found SaveFileDialog example from MSDN

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            //Default extensions used by Save File Dialog
            saveFileDialog1.Filter = "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    spreadSheet.Save(saveFileDialog1.FileName);
                }
                catch
                {
                    MessageBox.Show("Error saving file", "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                }
            }
        }

        /// <summary>
        /// Event handler for the "bar graph" menu item
        /// </summary>
        /// <param name="sender">Object sending event</param>
        /// <param name="e">Event arguments</param>
        private void barGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.getAppContext().RunForm(new ChartForm(spreadSheet));
        }

        /// <summary>
        /// Event handler for "how to use" menu item
        /// </summary>
        /// <param name="sender">Object sending event</param>
        /// <param name="e">Event arguments</param>
        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.getAppContext().RunForm(new HowToUseForm());
        }

        /// <summary>
        /// Event handler for "close" menu item
        /// </summary>
        /// <param name="sender">Object sending event</param>
        /// <param name="e">Event arguments</param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check to see if they changed the spreadsheet before closing, if they did display dialog about saving
            if (spreadSheet.Changed == true)
            {
                if (MessageBox.Show("You have unsaved data, are you sure you want to close?", 
                    "Unsaved Data", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Close();
                }
            }
            else
                Close();
        }
    }
}
