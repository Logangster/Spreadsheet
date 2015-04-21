// Author: Logan Gore, Oct 3

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace SS
{
    /// <summary>
    /// Represents a spreadsheet that stores information in cells
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Dictionary that carries a KeyValuePair of cell names to Cell objects
        /// </summary>
        private Dictionary<String, Cell> cells;

        /// <summary>
        /// DepencyGraph that keeps tract of cell dependencies when adding formulas to cells
        /// </summary>
        private DependencyGraph cellDependency;

        /// <summary>
        /// Creates a base Spreadsheet with no validator and normalizer, also sets version to default
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            cells = new Dictionary<string, Cell>();
            cellDependency = new DependencyGraph();
        }

        /// <summary>
        /// Creates a base SpreadSheet with a validator, normalizer, and version
        /// </summary>
        /// <param name="isValid">Function for validating cell names</param>
        /// <param name="normalize">Normalizes cell names</param>
        /// <param name="version">Version of the SpreadSheet</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            cellDependency = new DependencyGraph();
        }

        /// <summary>
        /// Creates a SpreadSheet from a file
        /// </summary>
        /// <param name="fileName">Name of spreadsheet file to load</param>
        /// <param name="isValid">Function for validating cell names</param>
        /// <param name="normalize">Normalizes cell names</param>
        /// <param name="version">Version of the Spreadsheet</param>
        public Spreadsheet(String fileName, Func<String, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            cells = new Dictionary<string,Cell>();
            cellDependency = new DependencyGraph();
 
            InitializeSpreadsheet(fileName, version);
        }

        /// <summary>
        /// Reads the spreadsheet file given and initalizes the spreadsheet
        /// </summary>
        /// <param name="fileName"></param>
        private void InitializeSpreadsheet(string fileName, string givenVersion)
        {
            string cellName = "";

            try
            {
                using (XmlReader reader = XmlReader.Create(fileName))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    Version = reader.GetAttribute(0);
                                    if (Version != givenVersion)
                                        throw new SpreadsheetReadWriteException("Invalid version given.");
                                    break;
                                case "name":
                                    if (reader.Read())
                                    {
                                        cellName = reader.Value;
                                    }
                                    break;
                                case "contents":
                                    if (reader.Read())
                                    {
                                        SetContentsOfCell(cellName, reader.Value);
                                    }
                                    break;

                            }
                        }
                    }
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Error opening file.");
            }
        }

        /// <summary>
        /// Receives the name of all the cells that aren't empty
        /// </summary>
        /// <returns>IEnumerable<string> of all the cells</string></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            if (cells.Count > 0)
            {
                foreach (KeyValuePair<String, Cell> entry in cells)
                {
                    if (entry.Value.contents.ToString() != "")
                        yield return entry.Key;
                }
            }

        }

        /// <summary>
        /// Retrieves cell contents
        /// </summary>
        /// <param name="name">Name of the cell to retrieve contents from</param>
        /// <returns>String, Formula, or double depending on what the cell contains</returns>
        public override object GetCellContents(string name)
        {
            if (name == null)
                throw new InvalidNameException();

            //normalize cell name before going further
            name = Normalize(name);
            CellNameValidator(name);

            if (cells.ContainsKey(name))
                return cells[name].contents;
            else
                return "";
        }

        /// <summary>
        /// Sets the contents of the cell by selecting proper SetCellContents method
        /// </summary>
        /// <param name="name">Cell name</param>
        /// <param name="content">Content to place into cell</param>
        /// <returns>Set of all cells to be recalcluated</returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
                throw new ArgumentNullException();

            //normalize the cell name before using name
            CellNameValidator(name);
            name = Normalize(name);


            //If cell contained a formula wipe the previous dependencies as we are adding a new formula, double, or text
            if (cells.ContainsKey(name) && cells[name].contents is Formula)
            {
                Formula f1 = (Formula)cells[name].contents;
                    foreach (string cell in f1.GetVariables())
                    {
                        cellDependency.RemoveDependency(cell, name);
                    }
            }

            //Validation passed, the cell is now going to be changed
            Changed = true;

            double result;

            // If string starts with = than it's a formula, otherwise attempt to parse it into double if it doesn't work
            // it's intended to be a string

            HashSet<string> cellsToRecalculate = new HashSet<string>();
            //Formula
            if (content.Length >= 1 && content[0] == '=')
            {
                Formula f1 = new Formula(content.Substring(1));
                cellsToRecalculate = (HashSet<string>) SetCellContents(name, f1);
            }
            //Double
            else if (double.TryParse(content, out result))
                cellsToRecalculate = (HashSet<string>) SetCellContents(name, result);
            //String
            else
                cellsToRecalculate = (HashSet<string>)SetCellContents(name, content);

            foreach (string cell in cellsToRecalculate)
            {
                if (cells[cell].contents is Formula)
                {
                   cells[cell].Calculate(VariableLookup);
                }
            }

            return cellsToRecalculate;
        }

        /// <summary>
        /// Sets the cell contents with a double
        /// </summary>
        /// <param name="name">Name of the cell to set contents inside of</param>
        /// <param name="number">Double to place in cell</param>
        /// <returns>ISet<string> of all the cells that need to be updated</string></returns>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            cells[name] = new Cell(number.ToString(), number); 

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// Sets the cell contents with a string
        /// </summary>
        /// <param name="name">Name of the cell to set contents inside of</param>
        /// <param name="text">String to place in cell</param>
        /// <returns>ISet<string> of all the cells that need to be updated</string></returns>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            cells[name] = new Cell(text);
            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// Sets the cell contents with a Formula. Throws CircularException if a circular dependency is established.
        /// </summary>
        /// <param name="name">Name of the cell to set contents inside of</param>
        /// <param name="formula">Formula to place in cell</param>
        /// <returns>ISet<string> of all the cells that need to be updated</string></returns>
        protected override ISet<string> SetCellContents(string name, SpreadsheetUtilities.Formula formula)
        {
            IEnumerable<string> variables = formula.GetVariables();

            foreach (String variable in variables)
            {
                if (!IsValid(variable))
                    throw new FormulaFormatException("Invalid cell name.");
                cellDependency.AddDependency(Normalize(variable), name);
            }

            try
            {
                IEnumerable<string> cellsToRecalculate = GetCellsToRecalculate(name);
            }
            //Oops there's been a circular exception, restore the dependencies
            catch (CircularException)
            {
                foreach (String variable in variables)
                {
                    cellDependency.RemoveDependency(Normalize(variable), name);
                }

                //Now that the dependencies are restored, we can throw the exception out
                throw new CircularException();
            }

            //No exception, change the cell to the formula and evaluate the formula also
            //cells[name] = new Cell(formula, formula.Evaluate(s => (double)cells[s].value));
            cells[name] = new Cell(formula, formula.Evaluate(VariableLookup));

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// Retrieves direct dependents of the cell
        /// </summary>
        /// <param name="name">Name of the cell to retrieve dependents from</param>
        /// <returns>Enumerable collection of all the direct dependents</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return cellDependency.GetDependents(name);
        }

        /// <summary>
        /// Validates the cell name and throws an exception if it is invalid
        /// </summary>
        /// <param name="name">Name of the cell to validate</param>
        private void CellNameValidator(string name)
        {
            if (name == null)
                throw new InvalidNameException();
            if (!Regex.IsMatch(name, @"^[a-zA-Z]+[0-9]+$"))
            {
                throw new InvalidNameException();
            }
            else if (!IsValid(name))
                throw new InvalidNameException();
        }

        /// <summary>
        /// For use with formula evaluator in setCellConents
        /// </summary>
        /// <param name="variable">variable to lookup</param>
        /// <returns></returns>
        private double VariableLookup(string variable)
        {
            variable = Normalize(variable);
            if (cells.ContainsKey(variable) && cells[variable].value is Double)
                return (Double)cells[variable].value;
            else
                throw new FormulaFormatException("Invalid variable");
        }

        /// <summary>
        /// Keeps track if the spreadsheet has changed since it was last saved
        /// </summary>
        public override bool Changed { get; protected set;}

        /// <summary>
        /// Retrieves the version last saved to file
        /// </summary>
        /// <param name="filename">Name of the file to open</param>
        /// <returns></returns>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement() && reader.Name == "spreadsheet")
                        {
                            return reader.GetAttribute("version");
                        }
                    }
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Error reading file.");
            }

            return "default";
        }

        /// <summary>
        /// Save spreadsheet to file to the specified path
        /// </summary>
        /// <param name="filename">Path to save the file to</param>
        public override void Save(string filename)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", this.Version);

                    foreach (KeyValuePair<String, Cell> cell in cells)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cell.Key);

                        if (cell.Value.contents is Formula)
                            writer.WriteElementString("contents", "=" + cell.Value.contents.ToString());
                        else
                            writer.WriteElementString("contents", cell.Value.contents.ToString());

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                //File has been saved no longer changed
                Changed = false;
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Error saving file.");
            }
        }

        /// <summary>
        /// Gets the cell's actual value opposed to contents shown
        /// </summary>
        /// <param name="name">name of the cell to retrieve contents</param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            if (name == null || !IsValid(name))
                throw new InvalidNameException();
            name = Normalize(name);
            CellNameValidator(name);

            if (!cells.ContainsKey(name))
                return "";

            return cells[name].value;
        }
    }
}
