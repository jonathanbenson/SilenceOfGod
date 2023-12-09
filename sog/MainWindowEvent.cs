
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
using sog.src;

namespace sog;

public partial class MainWindow : Window, INotifyPropertyChanged
{

    private void HandleVoiceModeChecked(object sender, RoutedEventArgs e)
    {
        VoiceControls.Visibility = Visibility.Visible;
        NonVoiceControls.Visibility = Visibility.Collapsed;

        DoListen = true;
    }

    private void HandleVoiceModeUnchecked(object sender, RoutedEventArgs e)
    {
        VoiceControls.Visibility = Visibility.Collapsed;
        NonVoiceControls.Visibility = Visibility.Visible;

        DoListen = false;
    }

    private void HandleBooksComboSelectionChanged(object sender, RoutedEventArgs e)
    {
        ChaptersCombo.SelectedIndex = 0;
        VersesCombo.SelectedIndex = 0;

        Chapters = Bible.books[BooksCombo.SelectedIndex].chapters.Select((chapter) => chapter.chapter).ToList();
        Verses = Bible.books[BooksCombo.SelectedIndex].chapters[ChaptersCombo.SelectedIndex].verses.Select((verse) => verse.verse).ToList();;
    }

    private void HandleChaptersComboSelectionChanged(object sender, RoutedEventArgs e)
    {
        VersesCombo.SelectedIndex = 0;

        Verses = Bible.books[BooksCombo.SelectedIndex].chapters[ChaptersCombo.SelectedIndex].verses.Select((verse) => verse.verse).ToList();;
    }


    private void HandleHelpButtonClicked(object sender, RoutedEventArgs e)
    {
        HandleHelp(new string[] {});
    }

    private void HandleContentsButtonClicked(object sender, RoutedEventArgs e)
    {
        HandleContents(new string[] {});
    }

    private void HandleSearchButtonClicked(object sender, RoutedEventArgs e)
    {
        HandleSearch(new string[] { BooksCombo.SelectedIndex.ToString(), ChaptersCombo.SelectedIndex.ToString(), VersesCombo.SelectedIndex.ToString() });
    }

    private void HandleNextPageButtonClicked(object sender, RoutedEventArgs e)
    {
        HandleNextPage(new string[] { "1" });
    }

    private void HandleLastPageButtonClicked(object sender, RoutedEventArgs e)
    {
        HandleLastPage(new string[] { "1" });
    }

    private void HandleSpeechRecognitionTestButtonClicked(object sender, RoutedEventArgs e)
    {
        SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
        Grammar dictationGrammar = new DictationGrammar();
        recognizer.LoadGrammar(dictationGrammar);
        try
        {
            recognizer.SetInputToDefaultAudioDevice();
            RecognitionResult result = recognizer.Recognize();
            MessageBox.Show(result.Text);
        }
        catch (InvalidOperationException exception)
        {
            MessageBox.Show(exception.ToString());
        }
        finally
        {
            recognizer.UnloadAllGrammars();
        }
    }

    private async void OnContentRendered(object sender, EventArgs e)
    {
        var timer = new Stopwatch();
        timer.Start();

        PageContainer.UpdateLayout();

        for (int bookIndex = 0; bookIndex < Bible.books.Count; bookIndex++)
        {
            for (int chapterIndex = 0; chapterIndex < Bible.books[bookIndex].chapters.Count; chapterIndex++)
            {

                Page.Clear();
                PageItems.UpdateLayout();

                int verseIndex = 0;
                int start = 0;

                while (verseIndex < Bible.books[bookIndex].chapters[chapterIndex].verses.Count)
                {
                    if (PageItems.ActualHeight < PageContainer.ActualHeight)
                    {
                        Page.Add(Bible.books[bookIndex].chapters[chapterIndex].verses[verseIndex]);
                        verseIndex++;
                    }
                    else
                    {

                        Page.RemoveAt(Page.Count - 1);

                        verseIndex--;

                        AddPageKey(Page.ToList(), bookIndex, chapterIndex, start, verseIndex);
                        
                        Page.Clear();
                        
                        start = verseIndex + 1;

                    }

                    PageItems.UpdateLayout();

                }

                AddPageKey(Page.ToList(), bookIndex, chapterIndex, start, Bible.books[bookIndex].chapters[chapterIndex].verses.Count - 1);
            }
        }

        CurrentPageKey = new PageKey(0, 0, 0);
        LoadPage(CurrentPageKey);

        BooksCombo.SelectedIndex = 0;
        ChaptersCombo.SelectedIndex = 0;
        VersesCombo.SelectedIndex = 0;

        timer.Stop();


        if (timer.ElapsedMilliseconds < LOADING_TIME)
            await Task.Delay((int)(LOADING_TIME - timer.ElapsedMilliseconds));

        LoadingContent.Visibility = Visibility.Hidden;
        MainContent.Visibility = Visibility.Visible;
    }


}