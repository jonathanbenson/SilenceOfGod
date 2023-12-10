
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
using sog.src;

namespace sog;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    /*

    Partial class of MainWindow containing code associated with event handlers.

    */

    private void HandleVoiceModeChecked(object sender, RoutedEventArgs e)
    {
        // When the voice mode is turned on...
        // Show the voice controls and hide the non-voice controls, and listen for commands
        VoiceControls.Visibility = Visibility.Visible;
        NonVoiceControls.Visibility = Visibility.Collapsed;

        DoListen = true;
    }

    private void HandleVoiceModeUnchecked(object sender, RoutedEventArgs e)
    {
        // When the voice mode is turned off...
        // Hide the voice controls and show the non-voice controls, and stop listening for commands
        VoiceControls.Visibility = Visibility.Collapsed;
        NonVoiceControls.Visibility = Visibility.Visible;

        DoListen = false;
    }

    private void HandleBooksComboSelectionChanged(object sender, RoutedEventArgs e)
    {
        // When a new book is selected...

        // Reset both the chapter and verse to 1
        ChaptersCombo.SelectedIndex = 0;
        VersesCombo.SelectedIndex = 0;

        // Repopulate the chapters and verses according to the newly selected book
        Chapters = Bible.books[BooksCombo.SelectedIndex].chapters.Select((chapter) => chapter.chapter).ToList();
        Verses = Bible.books[BooksCombo.SelectedIndex].chapters[ChaptersCombo.SelectedIndex].verses.Select((verse) => verse.verse).ToList();;
    }

    private void HandleChaptersComboSelectionChanged(object sender, RoutedEventArgs e)
    {
        // When a new chapter is selected...

        // Reset the verse to 1
        VersesCombo.SelectedIndex = 0;

        // Repopulate the verses according to the newly selected chapter
        Verses = Bible.books[BooksCombo.SelectedIndex].chapters[ChaptersCombo.SelectedIndex].verses.Select((verse) => verse.verse).ToList();;
    }


    private void HandleHelpButtonClicked(object sender, RoutedEventArgs e)
    {
        // Open the help window when the help button is clicked
        HandleHelp(new string[] {});
    }

    private void HandleContentsButtonClicked(object sender, RoutedEventArgs e)
    {
        // Open the contents window when the table of contents button is clicked
        HandleContents(new string[] {});
    }

    private void HandleSearchButtonClicked(object sender, RoutedEventArgs e)
    {
        // Search the Bible when the search button is clicked
        HandleSearch(new string[] { BooksCombo.SelectedIndex.ToString(), ChaptersCombo.SelectedIndex.ToString(), VersesCombo.SelectedIndex.ToString() });
    }

    private void HandleNextPageButtonClicked(object sender, RoutedEventArgs e)
    {
        // Go to the next page when the next page button is clicked
        HandleNextPage(new string[] { "1" });
    }

    private void HandleBackPageButtonClicked(object sender, RoutedEventArgs e)
    {
        // Go to the previous page when the back button is pressed
        HandleBackPage(new string[] { "1" });
    }

    private async void OnContentRendered(object sender, EventArgs e)
    {
        // This function initializes the application so that pages can be properly looked up according
        // to Book-Chapter-Verse location keys (PageKeys)

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

        // Load Genesis 1:1 on startup
        CurrentPageKey = new PageKey(0, 0, 0);
        LoadPage(CurrentPageKey);

        BooksCombo.SelectedIndex = 0;
        ChaptersCombo.SelectedIndex = 0;
        VersesCombo.SelectedIndex = 0;

        timer.Stop();

        // Display the loading screen for at least as long as the LOADING_TIME
        if (timer.ElapsedMilliseconds < LOADING_TIME)
            await Task.Delay((int)(LOADING_TIME - timer.ElapsedMilliseconds));

        // Hide the loading content and show the main content
        LoadingContent.Visibility = Visibility.Hidden;
        MainContent.Visibility = Visibility.Visible;
    }


}