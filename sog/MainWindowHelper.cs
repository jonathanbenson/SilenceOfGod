using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using sog.src.model;
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
        return $"{BookIndex}-{ChapterIndex}-{VerseIndex}";
    }
}

public partial class MainWindow : Window, INotifyPropertyChanged
{
    SpeechRecognitionEngine Recognizer = new SpeechRecognitionEngine();

    private void HandleSearch(string[] args)
    {
        if (CurrentPageKey is not null)
            CurrentPageKey = new PageKey(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));

        LoadPage(CurrentPageKey);
    }

    private void AddPageKey(List<Verse> page, int bookIndex, int chapterIndex, int startVerseIndex, int endVerseIndex)
    {
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
        bool isAcceptingCommands = false;

        while (true)
        {
            if (DoExitListenerThread)
                break;
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

                text = result.Text.ToLower();

                if (isAcceptingCommands)
                {
                    if (text == "stop")
                        isAcceptingCommands = false;
                    else
                        Dispatcher.Invoke(() => CommandDispatcher.Dispatch(text) );
                }
                else if (text == "start")
                    isAcceptingCommands = true;

            }
            catch (InvalidOperationException)
            {
                VoiceMessage = "Could not recognize input from default aduio device. Try changing your microphone and restart the application.";
                Thread.Sleep(1000);
            }
            catch (Exception)
            {
                VoiceMessage = $"Invalid command: '{text}'";
                Thread.Sleep(1000);
            }
            finally
            {
                Recognizer.UnloadAllGrammars();
            }
        }
    }
}