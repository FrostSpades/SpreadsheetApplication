using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS;

public class Spreadsheet : AbstractSpreadsheet
{
    private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();
    private DependencyGraph graph = new DependencyGraph();

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

    public Spreadsheet() { }


    /// <summary>
    /// If name is invalid, throws an InvalidNameException.
    /// 
    /// Otherwise, returns the contents (as opposed to the value) of the named cell.
    /// The return value should be either a string, a double, or a Formula.
    /// </summary>
    public override object GetCellContents(string name)
    {
        if (cells.ContainsKey(name))
        {
            return cells[name].contents;
        }

        else
        {
            throw new InvalidNameException();
        }
    }

    /// <summary>
    /// Enumerates the names of all the non-empty cells in the spreadsheet.
    /// </summary>
    public override IEnumerable<string> GetNamesOfAllNonemptyCells()
    {
        return cells.Keys;
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
    public override IList<string> SetCellContents(string name, double number)
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

        return (IList<string>)GetCellsToRecalculate(name);
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
    public override IList<string> SetCellContents(string name, string text)
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

        return (IList<string>)GetCellsToRecalculate(name);
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
    public override IList<string> SetCellContents(string name, Formula formula)
    {
        // Throws exception if name isn't valid
        if (!CheckValidName(name))
        {
            throw new InvalidNameException();
        }

        // Removes all of name's dependents and sets its new value to number
        if (cells.ContainsKey(name))
        {
            graph.ReplaceDependees(name, formula.GetVariables());
            cells[name].contents = formula;
        }

        // If cell does not exist, add it
        else
        {
            cells.Add(name, new Cell(formula));
        }

        return (IList<string>)GetCellsToRecalculate(name);
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
}
