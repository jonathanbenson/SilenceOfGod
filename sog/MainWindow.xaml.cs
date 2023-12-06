﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using sog.src.model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Security.Principal;

namespace sog
{

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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private int StartVerseIndex = 0;
        private int EndVerseIndex = 0;

        private Bible Bible;

        Dictionary<string, List<Verse>> PageLookup = new Dictionary<string, List<Verse>>();

        private string _Header = "";
        public PageKey? _CurrentPageKey;
        public ObservableCollection<Verse> _Page = new ObservableCollection<Verse>();

        private List<string> _Books = new List<string>();
        private List<string> _Chapters = new List<string>();
        private List<string> _Verses = new List<string>();

        public string Header
        {
            get
            {
                return _Header;
            }
            set
            {
                _Header = value;
                NotifyPropertyChanged("Header");
            }
        }

        public PageKey? CurrentPageKey
        {
            get
            {
                return _CurrentPageKey;
            }
            set
            {
                _CurrentPageKey = value;
                NotifyPropertyChanged("CurrentPageKey");
            }
        }

        public ObservableCollection<Verse> Page
        {
            get
            {
                return _Page;
            }
            set
            {
                _Page = value;
            }
        }

        public List<string> Books
        {
            get
            {
                return _Books;
            }
            set
            {
                _Books = value;
                NotifyPropertyChanged("Books");
            }
        }
        public List<string> Chapters
        {
            get
            {
                return _Chapters;
            }
            set
            {
                _Chapters = value;
                NotifyPropertyChanged("Chapters");
            }
        }
        public List<string> Verses
        {
            get
            {
                return _Verses;
            }
            set
            {
                _Verses = value;
                NotifyPropertyChanged("Verses");
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            BibleBuilder bibleBuilderService = new BibleBuilder();

            Bible = bibleBuilderService.Build("bible");

            
            Books = Bible.books.Select((book) => book.book).ToList();

        }

        private void HandleVoiceModeChecked(object sender, RoutedEventArgs e)
        {
            VoiceControls.Visibility = Visibility.Visible;
            NonVoiceControls.Visibility = Visibility.Collapsed;
        }

        private void HandleVoiceModeUnchecked(object sender, RoutedEventArgs e)
        {
            VoiceControls.Visibility = Visibility.Collapsed;
            NonVoiceControls.Visibility = Visibility.Visible;
        }

        private void HandleBooksComboSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentPageKey is not null)
                CurrentPageKey = new PageKey(CurrentPageKey.BookIndex, 0, 0);

            LoadPage(CurrentPageKey);
        }

        private void HandleChaptersComboSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentPageKey is not null)
                CurrentPageKey = new PageKey(CurrentPageKey.BookIndex, CurrentPageKey.ChapterIndex, 0);

            LoadPage(CurrentPageKey);
        }

        private void HandleVersesComboSelectionChanged(object sender, RoutedEventArgs e)
        {
            LoadPage(CurrentPageKey);
        }

        private void HandleNextPageButtonClicked(object sender, RoutedEventArgs e)
        {

            if (CurrentPageKey is not null)
            {

                Chapter selectedChapter = Bible.books[CurrentPageKey.BookIndex].chapters[CurrentPageKey.ChapterIndex];

                int lastVerseIndexOfPage = Convert.ToInt32(Page[Page.Count - 1].verse) - 1;

                if (lastVerseIndexOfPage == selectedChapter.verses.Count - 1)
                {

                    if (CurrentPageKey.ChapterIndex == Bible.books[CurrentPageKey.BookIndex].chapters.Count - 1)
                    {
                        if (CurrentPageKey.BookIndex == Bible.books.Count - 1)
                            CurrentPageKey = new PageKey(0, 0, 0);
                        else
                            CurrentPageKey = new PageKey(CurrentPageKey.BookIndex + 1, 0, 0);
                    }
                    else
                        CurrentPageKey = new PageKey(CurrentPageKey.BookIndex, CurrentPageKey.ChapterIndex + 1, 0);
                }
                else
                    CurrentPageKey = new PageKey(CurrentPageKey.BookIndex, CurrentPageKey.ChapterIndex, lastVerseIndexOfPage + 1);
                
                LoadPage(CurrentPageKey);
                
            }
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            

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

                            for (int i = start; i < verseIndex; i++)
                                PageLookup.Add(new PageKey(bookIndex, chapterIndex, i).ToString(), Page.ToList());
                            
                            Page.Clear();
                            
                            start = verseIndex;

                        }

                        PageItems.UpdateLayout();

                    }

                    for (int i = start; i < Bible.books[bookIndex].chapters[chapterIndex].verses.Count; i++)
                        PageLookup.Add(new PageKey(bookIndex, chapterIndex, i).ToString(), Page.ToList());
                }
            }

            CurrentPageKey = new PageKey(0, 0, 0);
            LoadPage(CurrentPageKey);

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

                    Chapters = Bible.books[pageKey.BookIndex].chapters.Select((chapter) => chapter.chapter).ToList();
                    Verses = Bible.books[pageKey.BookIndex].chapters[pageKey.ChapterIndex].verses.Select((verse) => verse.verse).ToList();
                }
            }, DispatcherPriority.Render);
        }

        private void HandleVerseChange()
        {
            // TODO
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")  
        {  
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }  

    }
}
