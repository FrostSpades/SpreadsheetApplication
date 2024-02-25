// C# File for MainPage.xaml
// @author Ethan Andrews
// @version 10/19/2023

using SpreadsheetUtilities;
using SS;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CommunityToolkit.Maui.Storage;

namespace SpreadsheetGUI;

/// <summary>
/// Example of using a SpreadsheetGUI object
/// </summary>
public partial class MainPage : ContentPage
{

    public SpreadsheetGrid savedSpreadsheetGridOne;
    public SpreadsheetGrid savedSpreadsheetGridTwo;

    /// <summary>
    /// Constructor.
    /// </summary>
	public MainPage()
    {
        InitializeComponent();

        spreadsheetGrid.SelectionChanged += displaySelection;
        spreadsheetGrid.SetSelection(2, 3);

        savedSpreadsheetGridOne = spreadsheetGrid;
        savedSpreadsheetGridTwo = new();
    }

    /// <summary>
    /// Method for when the selection is changed. Changes the text of the content box to the contents of the selected cell.
    /// </summary>
    /// <param name="grid"></param>
    private void displaySelection(ISpreadsheetGrid grid)
    {
        spreadsheetGrid.GetSelection(out int col, out int row);  // Get current selection
        spreadsheetGrid.GetContents(col, row, out string contents); // Get contents of current selection
        spreadsheetGrid.GetValue(col, row, out string value);
        
        contentBox.Text = contents;  // Set text of content box to contents of selected cell
        valueBox.Text = "Value: " + value;

        int adjustedRow = row + 1;
        nameBox.Text = (char)(col + 65) + adjustedRow.ToString(); // Stores cell name of the form ColumnNumber_RowNumber
    }


    /// <summary>
    /// Method called when Enter button is clicked. Changes the contents of the selected cell to the value of the contentBox.
    /// Displays error if exception occurs.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnButtonClicked(Object sender, EventArgs e)
    {
        // Try to change the cell
        try
        {
            spreadsheetGrid.GetSelection(out int col, out int row);
            spreadsheetGrid.SetValue(col, row, contentBox.Text);

            spreadsheetGrid.GetValue(col, row, out string value);
            valueBox.Text = "Value: " + value;
        }

        // If fails, display an error box.
        catch (FormulaFormatException)
        {
            DisplayAlert("Invalid Formula", "The formula you entered was invalid", "Close");
        }

        catch (CircularException)
        {
            DisplayAlert("Invalid Formula", "The formula you entered was circular", "Close");
        }

    }

    /// <summary>
    /// Clears the current file when clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NewClicked(Object sender, EventArgs e)
    {
        // If spreadsheetGrid has been saved, clear the grid.
        if (!spreadsheetGrid.Changed)
        {
            spreadsheetGrid.Clear();

            // Clear the boxes
            ResetBoxes();
        }
        
        else
        {
            DisplayAlert("Unsaved Changes", "Please save your changes before creating a new spreadsheet.", "Close");
        }
    }

    /// <summary>
    /// Saves the spreadsheet when saved.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SaveClicked(Object sender, EventArgs e)
    {
        // Cancellation token for file
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ctsToken = cts.Token;

        // Save the json spreadsheet values
        using var stream = new MemoryStream(Encoding.Default.GetBytes(spreadsheetGrid.Save()));
        var fileSaverResult = await FileSaver.Default.SaveAsync(".sprd", stream, ctsToken);

        if (fileSaverResult.IsSuccessful)
        {
            await DisplayAlert("Saved Successfully", "The file was saved successfully", "Close");
        }
        else
        {
            await DisplayAlert("Saved Unsuccessful", "The file was saved unsuccessfully", "Close");
        }

    }

    /// <summary>
    /// Opens any file as text and prints its contents.
    /// Note the use of async and await, concepts we will learn more about
    /// later this semester.
    /// </summary>
    private async void OpenClicked(Object sender, EventArgs e)
    {
        // If spreadsheet hasn't been saved, display error.
        if (spreadsheetGrid.Changed)
        {
            await DisplayAlert("Unsaved Changes", "Please save your changes before opening a new spreadsheet.", "Close");
            return;
        }

        // Open the file
        try
        {
            FileResult fileResult = await FilePicker.Default.PickAsync(); // Get file selection
            
            if (fileResult != null)
            {
                System.Diagnostics.Debug.WriteLine( "Successfully chose file: " + fileResult.FileName );

                bool success = spreadsheetGrid.ReplaceSpreadsheet(fileResult.FullPath);

                if (!success)
                {
                    await DisplayAlert("Invalid File", "The selected file is invalid", "Close");
                }

                ResetBoxes();
                
            }
            else
            {
                Debug.WriteLine("No file selected.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error opening file:");
            Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// When spreadsheet one button is clicked, change current spreadsheet to spreadsheet one.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SwitchSpreadsheetOne(Object sender, EventArgs e)
    {
        spreadsheetGrid.SwitchSpreadsheetOne();
        ResetBoxes();
    }

    /// <summary>
    /// When Spreadsheet Two button is clicked, change current spreadsheet to spreadsheet two.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SwitchSpreadsheetTwo(Object sender, EventArgs e)
    {
        spreadsheetGrid.SwitchSpreadsheetTwo();
        ResetBoxes();
    }

    /// <summary>
    /// Resets the boxes to the data from A1.
    /// </summary>
    private void ResetBoxes()
    {
        spreadsheetGrid.SetSelection(0, 0);
        spreadsheetGrid.GetSelection(out int col, out int row);

        nameBox.Text = "A1";

        spreadsheetGrid.GetContents(col, row, out string contents);
        contentBox.Text = contents;

        spreadsheetGrid.GetValue(col, row, out string value);
        valueBox.Text = "Value: " + value;
    }

    /// <summary>
    /// Displays help messages when help buttons are clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HelpButton(Object sender, EventArgs e)
    {
        if (sender == buttonOne)
        {
            DisplayAlert("Selected Cell", "To change the selected cell, simply click on a different cell.", "Close");
        }

        else if (sender == buttonTwo)
        {
            DisplayAlert("Change cell contents", "To change the contents of the selected cell, type the new contents into the content box. Then, click the Enter button", "Close");
        }

        else if (sender == buttonThree)
        {
            DisplayAlert("Open File", "To open a file, click Open in the file menu. The current file must be saved before opening a new file.", "Close");
        }

        else if (sender == buttonFour)
        {
            DisplayAlert("Save File", "To save a file, click Save in the file menu and navigate to the desired file.", "Close");
        }

        else if (sender == buttonFive)
        {
            DisplayAlert("New File", "To create a new file, click New in the file menu. The current file must be saved before creating a new file", "Close");
        }

        else if (sender == buttonSix)
        {
            DisplayAlert("Special Feature", "Our special feature is multiple tabs. The Spreadsheet One and Spreadsheet Two buttons change which tab you are on.", "Close");
        }

        else if (sender == buttonSeven)
        {
            DisplayAlert("How to use Special Feature?", "Click on one of the buttons (Spreadsheet One or Spreadsheet Two) to switch to that spreadsheet.", "Close");
        }
    }
}
