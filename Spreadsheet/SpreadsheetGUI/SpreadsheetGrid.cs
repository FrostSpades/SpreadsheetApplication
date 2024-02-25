// SpreadsheetGrid object for application.
// @author Ethan Andrews
// @version 10/19/2023
using Font = Microsoft.Maui.Graphics.Font;
using SizeF = Microsoft.Maui.Graphics.SizeF;
using PointF = Microsoft.Maui.Graphics.PointF;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;
using CommunityToolkit.Maui.Storage;
using System.Text;

namespace SS;

/// <summary>
/// A grid that displays a spreadsheet with 26 columns (labeled A-Z) and 99 rows
/// (labeled 1-99).  Each cell on the grid can display a non-editable string.  One
/// of the cells is always selected (and highlighted).  When the selection changes, a
/// SelectionChanged event is fired.  Clients can register to be notified of
/// such events.
///
/// None of the cells are editable.  They are for display purposes only.
/// </summary>
public class SpreadsheetGrid : ScrollView, IDrawable, ISpreadsheetGrid
{
    public event SelectionChangedHandler SelectionChanged;

    // These constants control the layout of the spreadsheet grid.
    // The height and width measurements are in pixels.
    private const int DATA_COL_WIDTH = 80;
    private const int DATA_ROW_HEIGHT = 20;
    private const int LABEL_COL_WIDTH = 30;
    private const int LABEL_ROW_HEIGHT = 30;
    private const int PADDING = 4;
    private const int COL_COUNT = 26;
    private const int ROW_COUNT = 99;
    private const int FONT_SIZE = 12;

    // Columns and rows are numbered beginning with 0.  This is the coordinate
    // of the selected cell.
    private int _selectedCol;
    private int _selectedRow;

    // Coordinate of cell in upper-left corner of display
    private int _firstColumn = 0;
    private int _firstRow = 0;

    // Scrollbar positions
    private double _scrollX = 0;
    private double _scrollY = 0;

    /// <summary>
    /// Normalizer for spreadsheet.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Normalizer(string s)
    {
        return s.ToUpper();
    }

    /// <summary>
    /// Validator for spreadsheet. Doesn't accept strings that aren't in A0 - Z99.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool Validator(string s)
    {
        // If s is of length 0, return false
        if (s.Length == 0)
        {
            return false;
        }

        // Checks if first character is a letter.
        char lower = s.Substring(0).ToCharArray()[0];

        if (!Char.IsLetter(lower))
        {
            return false;
        }

        // Checks if second part of string is a number between 1 - 100 (adjusted by 1)
        if (int.TryParse(s.Substring(1, s.Length - 1), out int result))
        {
            if (result < 1 || result > 100)
            {
                return false;
            }
        }

        // If second part isn't a number, return false
        else
        {
            return false;
        }

        return true;
    }

    // The strings contained by this grid
    //private Dictionary<Address, String> _values = new();
    private Spreadsheet _spreadsheetValues = new Spreadsheet(Validator, Normalizer, "ps6");
    private Spreadsheet _savedSpreadsheetOne, _savedSpreadsheetTwo;

    // GraphicsView maintains the actual drawing of the grid and listens
    // for click events
    private GraphicsView graphicsView = new();

    /// <summary>
    /// Changed value based on whether file is saved. True if file is not saved.
    /// </summary>
    public bool Changed
    {
        get
        {
            return _spreadsheetValues.Changed;
        }
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public SpreadsheetGrid()
    {
        BackgroundColor = Colors.LightGray;
        graphicsView.Drawable = this;
        graphicsView.HeightRequest = LABEL_ROW_HEIGHT + (ROW_COUNT + 1) * DATA_ROW_HEIGHT;
        graphicsView.WidthRequest = LABEL_COL_WIDTH + (COL_COUNT + 1) * DATA_COL_WIDTH;
        graphicsView.BackgroundColor = Colors.LightGrey;
        graphicsView.EndInteraction += OnEndInteraction;
        this.Content = graphicsView;
        this.Scrolled += OnScrolled;
        this.Orientation = ScrollOrientation.Both;

        // Saved spreadsheet values for multiple tab functionality.
        _savedSpreadsheetOne = _spreadsheetValues;
        _savedSpreadsheetTwo = new Spreadsheet(Validator, Normalizer, "ps6");
    }

    /// <summary>
    /// Clears the spreadsheet.
    /// </summary>
    public void Clear()
    {
        // Deletes the current spreadsheet.
        if (_spreadsheetValues == _savedSpreadsheetOne)
        {
            _savedSpreadsheetOne = new Spreadsheet(Validator, Normalizer, "ps6");
            _spreadsheetValues = _savedSpreadsheetOne;
        }

        else
        {
            _savedSpreadsheetTwo = new Spreadsheet(Validator, Normalizer, "ps6");
            _spreadsheetValues = _savedSpreadsheetTwo;
        }

        Invalidate();
    }

    /// <summary>
    /// Replaces the spreadsheet with the spreadsheet found at file path. Returns true if successful.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool ReplaceSpreadsheet(string filePath)
    {
        // If changing spreadsheet is successful, return true
        try
        {
            if (_spreadsheetValues == _savedSpreadsheetOne)
            {
                _spreadsheetValues = new Spreadsheet(filePath, Validator, Normalizer, "ps6");
                _savedSpreadsheetOne = _spreadsheetValues;
            } else
            {
                _spreadsheetValues = new Spreadsheet(filePath, Validator, Normalizer, "ps6");
                _savedSpreadsheetTwo = _spreadsheetValues;
            }
            
            return true;
        }

        catch (SpreadsheetReadWriteException)
        {
            return false;
        }
    }

    /// <summary>
    /// Sets the value of cell in column "col" and row "row" to string c.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public bool SetValue(int col, int row, string c)
    {
        // If address isn't valid, return false
        if (InvalidAddress(col, row))
        {
            return false;
        }

        // Adjusts the row by 1
        int adjustedRow = row + 1;

        string a = (char)(col + 65) + adjustedRow.ToString(); // Stores cell name of the form ColumnNumber_RowNumber

        _spreadsheetValues.SetContentsOfCell(a, c);
        Invalidate();
        return true;
    }

    /// <summary>
    /// Gets the contents of cell in column col and row row, and outputs in string c.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public bool GetContents(int col, int row, out string c)
    {
        // If address is invalid, return false and set c to ""
        if (InvalidAddress(col, row))
        {
            c = "";
            return false;
        }

        // Adjust the row
        int adjustedRow = row + 1;

        string a = (char)(col + 65) + adjustedRow.ToString(); // Stores cell name of the form ColumnNumber_RowNumber
        Object value = _spreadsheetValues.GetCellContents(a); // Get value of selected cell

        // return value if string
        if (value is String)
        {
            c = value.ToString();
        }

        // convert to string if double
        else if (value is double)
        {
            c = value.ToString();
        }

        // Change c to formula string
        else
        {
            c = "=" + value.ToString();
        }

        return true;
    }

    /// <summary>
    /// Gets the value of cell in column col and row row, and outputs in string c.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public bool GetValue(int col, int row, out string c)
    {
        // If address is invalid, return false and set c to ""
        if (InvalidAddress(col, row))
        {
            c = "";
            return false;
        }

        
        int adjustedRow = row + 1;

        string a = (char)(col + 65) + adjustedRow.ToString(); // Stores cell name of the form ColumnNumber_RowNumber
        Object value = _spreadsheetValues.GetCellValue(a); // Get value of selected cell
        
        // return value if string
        if (value is String)
        {
            c = value.ToString();
        }

        // convert to string if double
        else if (value is double)
        {
            c = value.ToString();
        }

        // Change c to #ERROR if it returns a FormulaError
        else
        {
            c = "#ERROR";
        }

        return true;
    }

    /// <summary>
    /// Sets the currently selected cell to col, row.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool SetSelection(int col, int row)
    {
        if (InvalidAddress(col, row))
        {
            return false;
        }
        _selectedCol = col;
        _selectedRow = row;
        Invalidate();
        return true;
    }

    /// <summary>
    /// Gets the currently selected cell.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    public void GetSelection(out int col, out int row)
    {
        col = _selectedCol;
        row = _selectedRow;
    }

    /// <summary>
    /// Checks if address is invalid.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private bool InvalidAddress(int col, int row)
    {
        return col < 0 || row < 0 || col >= COL_COUNT || row >= ROW_COUNT;
    }

    /// <summary>
    /// Listener for click events on the grid.
    /// </summary>
    private void OnEndInteraction(object sender, TouchEventArgs args)
    {
        PointF touch = args.Touches[0];
        OnMouseClick(touch.X, touch.Y);
    }

    /// <summary>
    /// Returns the json version of the spreadsheet and sets spreadsheet to unchanged.
    /// </summary>
    /// <returns></returns>
    public string Save()
    {
        _spreadsheetValues.SetUnchanged();
        return _spreadsheetValues.GetJSON();
    }

    /// <summary>
    /// Switches the spreadsheet to spreadsheetOne
    /// </summary>
    public void SwitchSpreadsheetOne()
    {
        _spreadsheetValues = _savedSpreadsheetOne;
    }

    /// <summary>
    /// Switches the spreadsheet to spreadsheetTwo
    /// </summary>
    public void SwitchSpreadsheetTwo()
    {
        _spreadsheetValues = _savedSpreadsheetTwo;
    }

    /// <summary>
    /// Listener for scroll events. Redraws the panel, maintaining the
    /// row and column headers.
    /// </summary>
    private void OnScrolled(object sender, ScrolledEventArgs e)
    {
        _scrollX = e.ScrollX;
        _firstColumn = (int)e.ScrollX / DATA_COL_WIDTH;
        _scrollY = e.ScrollY;
        _firstRow = (int)e.ScrollY / DATA_ROW_HEIGHT;
        Invalidate();
    }

    /// <summary>
    /// Determines which cell, if any, was clicked.  Generates a SelectionChanged
    /// event.  All of the indexes are zero based.
    /// </summary>
    /// <param name="e"></param>
    private void OnMouseClick(float eventX, float eventY)
    {
        int x = (int)(eventX - _scrollX - LABEL_COL_WIDTH) / DATA_COL_WIDTH + _firstColumn;
        int y = (int)(eventY - _scrollY - LABEL_ROW_HEIGHT) / DATA_ROW_HEIGHT + _firstRow;
        if (eventX > LABEL_COL_WIDTH && eventY > LABEL_ROW_HEIGHT && (x < COL_COUNT) && (y < ROW_COUNT))
        {
            _selectedCol = x;
            _selectedRow = y;
            if (SelectionChanged != null)
            {
                SelectionChanged(this);
            }
        }
        Invalidate();
    }

    /// <summary>
    /// Calls redraw event for spreadsheet.
    /// </summary>
    private void Invalidate()
    {
        graphicsView.Invalidate();
    }

    /// <summary>
    /// Draw method for application.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="dirtyRect"></param>
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Move the canvas to the place that needs to be drawn.
        canvas.SaveState();
        canvas.Translate((float)_scrollX, (float)_scrollY);

        // Color the background of the data area white
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(
            LABEL_COL_WIDTH,
            LABEL_ROW_HEIGHT,
            (COL_COUNT - _firstColumn) * DATA_COL_WIDTH,
            (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT);

        // Draw the column lines
        int bottom = LABEL_ROW_HEIGHT + (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT;
        canvas.DrawLine(0, 0, 0, bottom);
        for (int x = 0; x <= (COL_COUNT - _firstColumn); x++)
        {
            canvas.DrawLine(
                LABEL_COL_WIDTH + x * DATA_COL_WIDTH, 0,
                LABEL_COL_WIDTH + x * DATA_COL_WIDTH, bottom);
        }

        // Draw the column labels
        for (int x = 0; x < COL_COUNT - _firstColumn; x++)
        {
            DrawColumnLabel(canvas, x,
                (_selectedCol - _firstColumn == x) ? Font.Default : Font.DefaultBold);
        }

        // Draw the row lines
        int right = LABEL_COL_WIDTH + (COL_COUNT - _firstColumn) * DATA_COL_WIDTH;
        canvas.DrawLine(0, 0, right, 0);
        for (int y = 0; y <= ROW_COUNT - _firstRow; y++)
        {
            canvas.DrawLine(
                0, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT,
                right, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT);
        }

        // Draw the row labels
        for (int y = 0; y < (ROW_COUNT - _firstRow); y++)
        {
            DrawRowLabel(canvas, y,
                (_selectedRow - _firstRow == y) ? Font.Default : Font.DefaultBold);
        }

        // Highlight the selection, if it is visible
        if ((_selectedCol - _firstColumn >= 0) && (_selectedRow - _firstRow >= 0))
        {
            canvas.DrawRectangle(
                LABEL_COL_WIDTH + (_selectedCol - _firstColumn) * DATA_COL_WIDTH + 1,
                              LABEL_ROW_HEIGHT + (_selectedRow - _firstRow) * DATA_ROW_HEIGHT + 1,
                              DATA_COL_WIDTH - 2,
                              DATA_ROW_HEIGHT - 2);
        }

        // Redraws each of the cells
        foreach (string name in _spreadsheetValues.GetNamesOfAllNonemptyCells())
        {
            // Get the coordinates of name
            int[] coords = getCoords(name);
            int col = coords[0] - _firstColumn;
            int row = coords[1] - _firstRow;
            String text = "";

            // Get the value of name
            if (GetValue(col, row, out string c))
            {
                text = c;
            }

            // Redraw name's value
            SizeF size = canvas.GetStringSize(text, Font.Default, FONT_SIZE + FONT_SIZE * 1.75f);
            canvas.Font = Font.Default;
            if (col >= 0 && row >= 0)
            {
                canvas.DrawString(text,
                    LABEL_COL_WIDTH + col * DATA_COL_WIDTH + PADDING,
                    LABEL_ROW_HEIGHT + row * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - size.Height) / 2,
                    size.Width, size.Height, HorizontalAlignment.Left, VerticalAlignment.Center);
            }
        }

        canvas.RestoreState();
    }

    /// <summary>
    /// Private method for getting the coordinates of a string s.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private int[] getCoords(string s)
    {
        int[] returnValue = new int[2];
        String sSub = s.Substring(1);
        char[] charArray = s.Substring(1).ToCharArray();

        returnValue[0] = (int)(s.Substring(0).ToCharArray()[0]) - 65;

        if (int.TryParse(s.Substring(1, s.Length - 1), out int result))
        {
            returnValue[1] = result - 1;
        }

        return returnValue;
    }

    /// <summary>
    /// Draws a column label.  The columns are indexed beginning with zero.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="x"></param>
    /// <param name="f"></param>
    private void DrawColumnLabel(ICanvas canvas, int x, Font f)
    {
        String label = ((char)('A' + x + _firstColumn)).ToString();
        SizeF size = canvas.GetStringSize(label, f, FONT_SIZE + FONT_SIZE * 1.75f);
        canvas.Font = f;
        canvas.FontSize = FONT_SIZE;
        canvas.DrawString(label,
              LABEL_COL_WIDTH + x * DATA_COL_WIDTH + (DATA_COL_WIDTH - size.Width) / 2,
              (LABEL_ROW_HEIGHT - size.Height) / 2, size.Width, size.Height,
              HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    /// <summary>
    /// Draws a row label.  The rows are indexed beginning with zero.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="y"></param>
    /// <param name="f"></param>
    private void DrawRowLabel(ICanvas canvas, int y, Font f)
    {
        String label = (y + 1 + _firstRow).ToString();
        SizeF size = canvas.GetStringSize(label, f, FONT_SIZE + FONT_SIZE * 1.75f);
        canvas.Font = f;
        canvas.FontSize = FONT_SIZE;
        canvas.DrawString(label,
            LABEL_COL_WIDTH - size.Width - PADDING,
            LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - size.Height) / 2,
            size.Width, size.Height,
              HorizontalAlignment.Right, VerticalAlignment.Center);
    }
}
