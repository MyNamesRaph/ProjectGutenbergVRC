
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Pages : UdonSharpBehaviour
{
    public string data;
    //public int linesPerPage = 39;
    //public int charsPerLine = 49;
    public int charsPerPage = 1024;

    public int currentPageNumber = 0;
    public Text currentPageNumberText;
    public InputField currentPageNumberInputField;

    public TextMeshProUGUI pageText;
    private string pageTextSlice;
    
    /// <summary>
    /// Renders the next page from the previously rendered one
    /// </summary>
    public void Next()
    {
        currentPageNumber++;
        Debug.Log("Loading next page: " + currentPageNumber);
        pageTextSlice = GetSlice();

        UpdateUI();
    }

    /// <summary>
    /// Renders the previous page from the previously rendered one
    /// </summary>
    public void Previous()
    {
        if (currentPageNumber == 0) return;

        currentPageNumber--;
        Debug.Log("Loading previous page: " + currentPageNumber);
        pageTextSlice = GetSlice();

        UpdateUI();
    }

    /// <summary>
    /// Renders the page at the number entered in currentPageNumberText
    /// </summary>
    public void Goto()
    {
        Debug.Log("Loading page: " + currentPageNumberText.text);
        currentPageNumber = int.Parse(currentPageNumberText.text);
        if (currentPageNumber < 0) currentPageNumber = 0;
        pageTextSlice = GetSlice();

        UpdateUI();
    }

    /// <summary>
    /// Updates the UI elements of the currently selected page
    /// </summary>
    public void UpdateUI()
    {
        Debug.Log("Changing page number text to " + currentPageNumber.ToString());
        currentPageNumberInputField.text = currentPageNumber.ToString();
        Debug.Log("Updating page display");
        pageText.text = pageTextSlice;
    }

    /// <summary>
    /// Gets the slice corresponding to the selected page
    /// </summary>
    private string GetSlice()
    {
        if (data == null || data.Length == 0) return string.Empty;

        int offset = charsPerPage * currentPageNumber;
        int lastCharInPage = offset + charsPerPage;

        // The last page is loaded
        if (lastCharInPage > data.Length)
        {
            currentPageNumber = GetNumberOfPages();
            return data.Substring(data.Length - charsPerPage);
        }

        // Any other page
        int startIndex = offset;
        int endIndex = Mathf.Min(offset + charsPerPage, data.Length);

        // Avoid cutting words at the start of a page
        while (startIndex > 0 && !char.IsWhiteSpace(data[startIndex]))
        {
            startIndex--;
        }

        // Avoid cutting words at the end of a page
        while (endIndex < data.Length && !char.IsWhiteSpace(data[endIndex - 1]))
        {
            endIndex++;
        }

        return data.Substring(startIndex, endIndex - startIndex);
    }

    private int GetNumberOfPages()
    {
        return Mathf.CeilToInt(data.Length / charsPerPage);
    }
}
