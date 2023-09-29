// Class that simulates spreadsheet behavior.
// @author Ethan Andrews
// @version September 22, 2023

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS;

public class Spreadsheet : AbstractSpreadsheet
{
    private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();
    private DependencyGraph graph = new DependencyGraph();
    private Func<string, bool> isValid;
    private Func<string, string> normalize;

    /// <summary>
    /// Class for storing cell data
    /// </summary>
    class Cell
    {
        public object contents { get; set; }

        public Cell(string contents)
        {
            this.contents = contents;
        }

        public Cell(double contents)
        {
            this.contents = contents;
        }

        public Cell(Formula contents)
        {
            this.contents = contents;
        }
    }

    public Spreadsheet() : this(s => true, s => s, "default")
    {
        Changed = false;
    }

    public Spreadsheet(Func<string, bool> validityFunction, Func<string, string> normalizeFunction, string version) : base(version)
    {
        isValid = validityFunction;
        normalize = normalizeFunction;
        Changed = false;
    }

    public Spreadsheet(string path, Func<string, bool> validityFunction, Func<string, string> normalizeFunction, string version) : base(version)
    {
        
        isValid = validityFunction;
        normalize = normalizeFunction;
        Changed = false;
        
        /// REMEMBER TO IMPLEMENT PATH CONSTRUCTOR
    }


    /// <summary>
    /// If name is invalid, throws an InvalidNameException.
    /// 
    /// Otherwise, returns the contents (as opposed to the value) of the named cell.
    /// The return value should be either a string, a double, or a Formula.
    /// </summary>
    public override object GetCellContents(string name)
    {
        // Throws an exception if name is invalid
        if (!CheckValidName(name))
        {
            throw new InvalidNameException();
        }

        if (cells.ContainsKey(name))
        {
            return cells[name].contents;
        }

        else
        {
            return "";
        }
    }

    /// <summary>
    /// Enumerates the names of all the non-empty cells in the spreadsheet.
    /// </summary>
    public override IEnumerable<string> GetNamesOfAllNonemptyCells()
    {
        IEnumerable<string> allNames = cells.Keys;
        List<string> names = new List<string>();

        foreach (string name in allNames)
        {
            if (!cells[name].contents.Equals("")) 
            {
                names.Add(name);
            }
        }

        return names;
    }


    public override IList<string> SetContentsOfCell(string name, string content)
    {
        // Normalizes name
        string nameNormalized = normalize(name);

        // If name isn't valid, throw an exception
        if (!isValid(nameNormalized))
        {
            throw new InvalidNameException();
        }

        if (double.TryParse(content, out double result))
        {
            return SetCellContents(name, result);
        }

        else if (content.Length != 0 && content[0] == '=')
        {
            return SetCellContents(name, new Formula(content, normalize, isValid));
        }

        else
        {
            return SetCellContents(name, content);
        }
    }


    /// <summary>
    /// If name is invalid, throws an InvalidNameException.
    /// 
    /// Otherwise, the contents of the named cell becomes number.  The method returns a
    /// list consisting of name plus the names of all other cells whose value depends, 
    /// directly or indirectly, on the named cell.
    /// 
    /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    /// list {A1, B1, C1} is returned.
    /// </summary>
    protected override IList<string> SetCellContents(string name, double number)
    {
        // Throws exception if name isn't valid
        if (!CheckValidName(name))
        {
            throw new InvalidNameException();
        }

        // Removes all of name's dependees and sets its new value to number
        if (cells.ContainsKey(name))
        {
            graph.ReplaceDependees(name, new List<string>());
            cells[name].contents = number;
        }

        // If cell does not exist, add it
        else
        {
            cells.Add(name, new Cell(number));
        }

        return GetCells(name);
    }

    /// <summary>
    /// If name is invalid, throws an InvalidNameException.
    /// 
    /// Otherwise, the contents of the named cell becomes text.  The method returns a
    /// list consisting of name plus the names of all other cells whose value depends, 
    /// directly or indirectly, on the named cell.
    /// 
    /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    /// list {A1, B1, C1} is returned.
    /// </summary>
    protected override IList<string> SetCellContents(string name, string text)
    {

        // Throws exception if name isn't valid
        if (!CheckValidName(name))
        {
            throw new InvalidNameException();
        }

        // Removes all of name's dependees and sets its new value to number
        if (cells.ContainsKey(name))
        {
            graph.ReplaceDependees(name, new List<string>());
            cells[name].contents = text;
        }

        // If cell does not exist, add it
        else
        {
            cells.Add(name, new Cell(text));
        }

        return GetCells(name);
    }

    /// <summary>
    /// If name is invalid, throws an InvalidNameException.
    /// 
    /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
    /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
    /// 
    /// Otherwise, the contents of the named cell becomes formula.  The method returns a
    /// list consisting of name plus the names of all other cells whose value depends,
    /// directly or indirectly, on the named cell.
    /// 
    /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    /// list {A1, B1, C1} is returned.
    /// </summary>
    protected override IList<string> SetCellContents(string name, Formula formula)
    {
        // Throws exception if name isn't valid
        if (!CheckValidName(name))
        {
            throw new InvalidNameException();
        }

        IEnumerable<string> oldDependees;
        object oldContents;

        // If cell does not exist, add it
        if (!cells.ContainsKey(name))
        {
            oldDependees = new List<string>();
            oldContents = "";

            cells.Add(name, new Cell(formula));
            graph.ReplaceDependees(name, formula.GetVariables());
        }

        // If cell does exist, change its contents and store old contents.
        else
        {
            oldDependees = graph.GetDependees(name);
            oldContents = cells[name].contents;

            graph.ReplaceDependees(name, formula.GetVariables());
            cells[name].contents = formula;
        }

        // Try to update data
        try
        {
            return GetCells(name);
        }

        // If exception is thrown, undo changes and throw CircularException
        catch (CircularException)
        {
            graph.ReplaceDependees(name, oldDependees);
            cells[name].contents = oldContents;
            throw new CircularException();
        }
    }

    /// <summary>
    /// Private method for checking if name is valid.
    /// </summary>
    /// <param name="token"></param>
    /// <exception cref="FormulaFormatException"></exception>
    private bool CheckValidName(string token)
    {
        bool firstCharacter = true;

        foreach (char c in token)
        {
            // Checks if first character is letter or underscore
            if (firstCharacter) 
            { 
                if (c != '_' && !Char.IsLetter(c))
                {
                    return false;
                }

                firstCharacter = false;
            }

            // Checks if other characters are letters, underscores, or digits
            else
            {
                if (!Char.IsLetterOrDigit(c) && c != '_')
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Private method for returning the cells that depend on name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private IList<string> GetCells(string name)
    {
        // Gets the list of dependents and stores it in a list
        IEnumerable<string> cellList = GetCellsToRecalculate(name);
        IList<string> c = new List<string>();

        foreach (string cell in cellList)
        {
            c.Add(cell);
        }

        return c;
    }


    /// <summary>
    /// Returns an enumeration, without duplicates, of the names of all cells whose
    /// values depend directly on the value of the named cell.  In other words, returns
    /// an enumeration, without duplicates, of the names of all cells that contain
    /// formulas containing name.
    /// 
    /// For example, suppose that
    /// A1 contains 3
    /// B1 contains the formula A1 * A1
    /// C1 contains the formula B1 + A1
    /// D1 contains the formula B1 - C1
    /// The direct dependents of A1 are B1 and C1
    /// </summary>
    protected override IEnumerable<string> GetDirectDependents(string name)
    {
        return graph.GetDependents(name);
    }

    public override void Save(string filename)
    {
        throw new NotImplementedException();
    }

    public override object GetCellValue(string name)
    {
        throw new NotImplementedException();
    }

    
}
