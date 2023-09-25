
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.StringLoading;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using TMPro;
using UnityEngine.UI;
using System;

public class Gutenberg : UdonSharpBehaviour
{
    public Pages pages;
    public Library library;
    public Text number;
    public TextMeshProUGUI information;

    private VRCUrl[] txtUrl;

    public void OnSubmitNumber()
    {
        LoadBook(int.Parse(number.text));
    }

    public void LoadBook(int bookNumber)
    {
        int index = Array.IndexOf(library.bookNumbers, bookNumber);

        if (index == -1)
        {
            information.text ="Book #"+bookNumber+" is not part of the library!";
            return;
        }

        int number = library.bookNumbers[index];
        string title = library.bookTitles[index];
        string type = library.bookTypes[index];
        string date = library.bookDates[index];
        string language = library.bookLanguages[index];
        string author = library.bookAuthors[index];
        string subject = library.bookSubjects[index];
        string loCC = library.bookLoCC[index];
        string bookShelves = library.bookBookshelves[index];
        txtUrl = library.bookTxtUrls[index];
        VRCUrl coverUrl = library.bookCoverUrls[index];

        information.text = 
            "Number: " + number +
            " Title: " + title +
            " Type: " + type +
            " Date: " + date +
            " Language: " + language +
            " Author: " + author +
            " Subject: " + subject +
            " LoCC: " + loCC +
            " Bookshelves: " + bookShelves +
            " TxtUrl: " + txtUrl[0] + "," + txtUrl[1] +
            " CoverUrl: " + coverUrl;

        Debug.Log("Downloading book : " + number);
        VRCStringDownloader.LoadUrl(txtUrl[0], (IUdonEventReceiver)this);
    }

    override public void OnStringLoadSuccess(IVRCStringDownload download)
    {
        Debug.Log("Download finished: Success!");
        pages.data = download.Result;
        pages.currentPageNumberInputField.text = "0";
        pages.Goto();
    }

    override public void OnStringLoadError(IVRCStringDownload download)
    {
        
        // Try to load using fallback URL
        if(download.Url == txtUrl[0])
        {
            Debug.Log("Download Failed: Trying fallback");
            VRCStringDownloader.LoadUrl(txtUrl[1], (IUdonEventReceiver)this);
            return;
        }
        Debug.Log("Download Failed ["+download.ErrorCode+"]: " + download.Error);
        pages.data = download.Error;
        pages.Goto(); // Renders page 0 by default
    }
}
