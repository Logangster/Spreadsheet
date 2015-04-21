using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS
{
    class Cell
    {
        /// <summary>
        /// Raw contents of cell
        /// </summary>
        public object contents { get; private set; }
        /// <summary>
        /// Evaluated contents of cell
        /// </summary>
        public object value { get; set; }

        /// <summary>
        /// Creates a new cell
        /// </summary>
        /// <param name="contents">Contents to be inserted into cell(unevaluated)</param>
        /// <param name="value">Evaluated contents</param>
        public Cell(object contents, object value)
        {
            this.contents = contents;
            this.value = value;
        }

        /// <summary>
        /// Creates a new cell
        /// </summary>
        /// <param name="contents">Single contents if not entering a Formula</param>
        public Cell(object contents): this(contents, contents)
        {

        }

        public void Calculate(Func<string, double> lookup)
        {
            if (this.contents is Formula)
            {
                Formula f1 = (Formula) this.contents;
                this.value = f1.Evaluate(lookup);
            }
                
        }
    }
}
