using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Speech.Recognition;
using System.Threading;
using sog.src;

namespace sog;

public class PageKey
{
    /*

    A class to represent Book-Chapter-Verse locations in the Bible

    */

    public int BookIndex { get; set; }
    public int ChapterIndex { get; set; }
    public int VerseIndex { get; set; }


    public PageKey(int bookIndex, int chapterIndex, int verseIndex)
    {
        BookIndex = bookIndex;
        ChapterIndex = chapterIndex;
        VerseIndex = verseIndex;
    }

    public override string ToString()
    {
        // To be used as a key in dictionaries
        return $"{BookIndex}-{ChapterIndex}-{VerseIndex}";
    }
}

public partial class MainWindow : Window, INotifyPropertyChanged
{
    /*

    Partial class of MainWindow containing helper methods

    */

    SpeechRecognitionEngine Recognizer = new SpeechRecognitionEngine();

    private void HandleSearch(string[] args)
    {
        // Search for a Book-Chapter-Verse location in the Bible and load the page associated with that location

        if (CurrentPageKey is not null)
            CurrentPageKey = new PageKey(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));

        LoadPage(CurrentPageKey);
    }

    private void AddPageKey(List<Verse> page, int bookIndex, int chapterIndex, int startVerseIndex, int endVerseIndex)
    {
        // Associate a page's verses as PageKeys with their corersponding page

        PageKey pk1 = new PageKey(bookIndex, chapterIndex, startVerseIndex);
        PageKeys.Add(pk1);

        for (int i = startVerseIndex; i <= endVerseIndex; i++)
        {
            PageKey pk2 = new PageKey(bookIndex, chapterIndex, i);

            PageKeyIndexLookup.Add(pk2.ToString(), PageKeys.Count - 1);
            PageLookup.Add(pk2.ToString(), page);
        }
    }

    private void LoadPage(PageKey? pageKey)
    {
        // Render the page (list of verses) associated with a PageKey (Book-Chapter-Verse location) on to the screen

        Dispatcher.Invoke(() => {
            if (pageKey is not null)
            {
                Page.Clear();
                foreach (Verse v in PageLookup[pageKey.ToString()])
                    Page.Add(v);

                Header = Bible.books[pageKey.BookIndex].book + " " + (pageKey.ChapterIndex + 1).ToString();

            }
        }, DispatcherPriority.Render);
    }

    private void Listen()
    {
        /*

        Function thread to listen for commands and call their corresponding UI callbacks.

        */

        bool isAcceptingCommands = false;

        while (true)
        {
            // Ask the MainWindow if its time to exit
            if (DoExitListenerThread)
                break;
            // Ask the MainWindow if should listen
            else if (!DoListen)
            {
                Thread.Sleep(1000);
                continue;
            }

            if (isAcceptingCommands)
                VoiceMessage = "Say 'help' for a list of available commands, or 'stop' for the program to stop listening to commands";
            else
                VoiceMessage = "Say 'start' for the program to start listening to commands";

            string text = "Invalid command";

            Grammar dictationGrammar = new DictationGrammar();
            Recognizer.LoadGrammar(dictationGrammar);

            try
            {
                Recognizer.SetInputToDefaultAudioDevice();
                RecognitionResult result = Recognizer.Recognize();

                // Retrieve the string of text associated with the user's speech
                text = result.Text.ToLower();

                if (isAcceptingCommands)
                {
                    // Stop listening to commands if the user says 'stop'
                    if (text == "stop")
                        isAcceptingCommands = false;
                    // Call the appropriate UI callback if a user said a valid command
                    else
                        Dispatcher.Invoke(() => CommandDispatcher.Dispatch(text) );
                }
                // Start listening to commands if the user said 'start'
                else if (text == "start")
                    isAcceptingCommands = true;

            }
            catch (InvalidOperationException)
            {
                // The audio device could not be recognized
                VoiceMessage = "Could not recognize input from default audio device. Try changing your microphone and restart the application.";
                Thread.Sleep(2000);
            }
            catch (Exception)
            {
                // The user said an invalid command
                VoiceMessage = $"Invalid command: '{text}'";
                Thread.Sleep(2000);
            }
            finally
            {
                Recognizer.UnloadAllGrammars();
            }
        }
    }
}