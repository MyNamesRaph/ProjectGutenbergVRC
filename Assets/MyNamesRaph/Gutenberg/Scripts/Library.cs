
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
#endif

public class Library : UdonSharpBehaviour
{
    /////////////////
    /// Book Data ///
    /////////////////
    [HideInInspector]
    public int[] bookNumbers;
    [HideInInspector]
    public string[] bookTitles;
    [HideInInspector]
    public string[] bookTypes;
    [HideInInspector]
    public string[] bookDates;
    [HideInInspector]
    public string[] bookLanguages;
    [HideInInspector]
    public string[] bookAuthors;
    [HideInInspector]
    public string[] bookSubjects;
    [HideInInspector]
    public string[] bookLoCC;
    [HideInInspector]
    public string[] bookBookshelves;
    [HideInInspector]
    public VRCUrl[][] bookTxtUrls;
    [HideInInspector]
    public VRCUrl[] bookCoverUrls;
    
    /// <summary>
    /// CSV Data
    /// </summary>
    public TextAsset pgCatalog;
}


#if !COMPILER_UDONSHARP && UNITY_EDITOR
[CustomEditor(typeof(Library))]
public class LibraryGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Library library = (Library)target;

        if (GUILayout.Button("Generate Library"))
        {
            Undo.RecordObject(library, "Generating Library");
            Debug.Log("Starting library generation ...");

            // Clear previous data
            library.bookNumbers = new int[0];
            library.bookTitles = new string[0];
            library.bookTypes = new string[0];
            library.bookDates = new string[0];
            library.bookLanguages = new string[0];
            library.bookAuthors = new string[0];
            library.bookSubjects = new string[0];
            library.bookLoCC = new string[0];
            library.bookBookshelves = new string[0];
            library.bookTxtUrls = new VRCUrl[2][];
            library.bookCoverUrls = new VRCUrl[0];

            EditorUtility.DisplayProgressBar("Generating Library", "Parsing CSV ...", 0);

            Byte[] data = library.pgCatalog.bytes;
            MemoryStream dataStream = new MemoryStream(data);
            using (TextFieldParser parser = new TextFieldParser(dataStream))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                List<int> numbers = new List<int>();           //0
                List<string> types = new List<string>();       //1
                List<string> dates = new List<string>();       //2
                List<string> titles = new List<string>();      //3
                List<string> languages = new List<string>();   //4
                List<string> authors = new List<string>();     //5
                List<string> subjects = new List<string>();    //6
                List<string> loCC = new List<string>();        //7
                List<string> bookshelves = new List<string>(); //8

                                                                // Calculated from numbers
                List<VRCUrl[]> txtUrls = new List<VRCUrl[]>();  //0
                List<VRCUrl> coverUrls = new List<VRCUrl>();    //0

                
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    // LineNumber starts at 1 and is incremented on ReadFields()
                    // so the first read is actually 2
                    if (parser.LineNumber == 2) continue; // Skip headers

                    EditorUtility.
                        DisplayProgressBar(
                            "Generating Library",
                            "Reading Line:" + (parser.LineNumber - 1).ToString(),
                            parser.LineNumber * 0.0000125f
                        // We don't know how many there are since the library is always updating
                        // but 80k entries is accurate enough to display for a while
                        );

                    numbers.Add(int.Parse(fields[0]));
                    types.Add(fields[1]);
                    dates.Add(fields[2]);
                    titles.Add(fields[3]);
                    languages.Add(fields[4]);
                    authors.Add(fields[5]);
                    subjects.Add(fields[6]);
                    loCC.Add(fields[7]);
                    bookshelves.Add(fields[8]);

                    var urls = GenerateURLs(int.Parse(fields[0]));

                    txtUrls.Add(urls.TxtUrls);
                    coverUrls.Add(urls.CoverUrl);
                }

                EditorUtility.DisplayProgressBar("Generating Library","Copying list to array: Numbers",0.09f);
                library.bookNumbers = numbers.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: Titles", 0.18f);
                library.bookTitles = titles.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: Types", 0.27f);
                library.bookTypes = types.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: Dates", 0.36f);
                library.bookDates = dates.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: Languages", 0.45f);
                library.bookLanguages = languages.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: Authors", 0.54f);
                library.bookAuthors = authors.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: Subjects", 0.63f);
                library.bookSubjects = subjects.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: LoCC", 0.72f);
                library.bookLoCC = loCC.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: Bookshelves", 0.81f);
                library.bookBookshelves = bookshelves.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: TxtUrls", 0.90f);
                library.bookTxtUrls = txtUrls.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: CoverUrls", 0.99f);
                library.bookCoverUrls = coverUrls.ToArray();

                EditorUtility.DisplayProgressBar("Generating Library", "Copying list to array: Done", 1f);
            }
            EditorUtility.ClearProgressBar();
            Debug.Log("Finished library generation !");
        }

        EditorGUILayout.Separator();

        StringBuilder sb = new StringBuilder("Collection Info:");
        sb.Append("\nIf the numbers are different, something may be wrong with the csv file");
        sb.Append("\nTitles: ");
        sb.Append(library.bookTitles.Length.ToString());
        sb.Append("\nTypes: ");
        sb.Append(library.bookTypes.Length.ToString());
        sb.Append("\nDates: ");
        sb.Append(library.bookDates.Length.ToString());
        sb.Append("\nLanguages: ");
        sb.Append(library.bookLanguages.Length.ToString());
        sb.Append("\nAuthors: ");
        sb.Append(library.bookAuthors.Length.ToString());
        sb.Append("\nSubjects: ");
        sb.Append(library.bookSubjects.Length.ToString());
        sb.Append("\nLoCC: ");
        sb.Append(library.bookLoCC.Length.ToString());
        sb.Append("\nBookshelves: ");
        sb.Append(library.bookBookshelves.Length.ToString());
        sb.Append("\nTxtUrls: ");
        //sb.Append(library.bookTxtUrls.Length.ToString());
        sb.Append("\nCoverUrls: ");
        sb.Append(library.bookCoverUrls.Length.ToString());

        EditorGUILayout.HelpBox(sb.ToString(),MessageType.Info);
    }

    const string MIRROR_URL = "http://mirror.csclub.uwaterloo.ca/gutenberg/";
    /// <summary>
    /// Generates the URLs for the given bookNumber, sadly this is not 100% accurate
    /// because the database is inconsistant.
    /// </summary>
    /// <param name="bookNumber">The number of the book to generate for</param>
    (VRCUrl[] TxtUrls,VRCUrl CoverUrl) GenerateURLs(int bookNumber)
    {
        string number = bookNumber.ToString();
        StringBuilder sb = new StringBuilder(MIRROR_URL);
        
        if(bookNumber < 10)
        {
            sb.Append("/0/");
        }
        else
        {
            for (int i = 0; i < number.Length - 1; i++)
            {
                sb.Append(number[i]);
                sb.Append('/');
            }
        }
        sb.Append(number);
        sb.Append('/');

        string baseUrl = sb.ToString();

        // "-0.txt" files seem more common, there are other formats
        // but it would be impractical to support all of them.
        string txtPrimary = baseUrl + number + "-0.txt";
        string txtSecondary = baseUrl + number + ".txt";
        VRCUrl[] txtUrls = { new VRCUrl(txtPrimary), new VRCUrl(txtSecondary) };

        // Once again we're ignoring other formats.
        string cover = baseUrl + number + "-h/images/cover.jpg";
        VRCUrl coverUrl = new VRCUrl(cover);

        return (txtUrls, coverUrl);
    }
}
#endif