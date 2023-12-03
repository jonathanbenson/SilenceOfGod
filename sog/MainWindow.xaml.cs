using System;
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

namespace sog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private int StartVerseIndex = 0;
        private int EndVerseIndex = 0;

        private Bible Bible;

        private string _Header = "";
        public ObservableCollection<string> _Page = new ObservableCollection<string>();

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

        public ObservableCollection<string> Page
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
            Chapters = Bible.books[0].chapters.Select((chapter) => chapter.chapter).ToList();
            Verses = Bible.books[0].chapters[0].verses.Select((verse) => verse.verse).ToList();

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
            HandleBookChange(BooksCombo.SelectedIndex);
        }

        private void HandleChaptersComboSelectionChanged(object sender, RoutedEventArgs e)
        {
            HandleChapterChange(BooksCombo.SelectedIndex, ChaptersCombo.SelectedIndex);
        }

        private void HandleNextPageButtonClicked(object sender, RoutedEventArgs e)
        {
            NextPage();
        }

        private void NextPage()
        {
            int bookIndex = BooksCombo.SelectedIndex;
            int chapterIndex = ChaptersCombo.SelectedIndex;

            Chapter selectedChapter = Bible.books[bookIndex].chapters[chapterIndex];

            if (EndVerseIndex == selectedChapter.verses.Count - 1)
            {
                if (bookIndex == Bible.books.Count - 1)
                    return;

                if (chapterIndex == Bible.books[bookIndex].chapters.Count - 1)
                    HandleBookChange(bookIndex + 1);
                else
                    HandleChapterChange(bookIndex, chapterIndex + 1);
            }
            else
                LoadPage(BooksCombo.SelectedIndex, ChaptersCombo.SelectedIndex, EndVerseIndex + 1);

        }

        private void HandleBookChange(int bookIndex)
        {
            if (bookIndex >= 0)
            {
                BooksCombo.SelectedIndex = bookIndex;

                Book selectedBook = Bible.books[bookIndex];
                Chapters = selectedBook.chapters.Select(c => c.chapter).ToList();

                if (Chapters.Any())
                {
                    ChaptersCombo.SelectedIndex = 0;
                    HandleChapterChange(bookIndex, 0);
                }
            }
        }

        private void HandleChapterChange(int bookIndex, int chapterIndex)
        {
            if (bookIndex >= 0 && chapterIndex >= 0)
            {
                ChaptersCombo.SelectedIndex = chapterIndex;

                Book selectedBook = Bible.books[bookIndex];
                Chapter selectedChapter = Bible.books[bookIndex].chapters[chapterIndex];
                Verses = selectedChapter.verses.Select(v => v.verse).ToList();
                
                LoadPage(bookIndex, chapterIndex, 0);

                if (Verses.Any())
                {
                    VersesCombo.SelectedIndex = 0;
                }
            }
        }

        private List<Verse> LoadPage(int bookIndex, int chapterIndex, int verseIndex)
        {
            Book selectedBook = Bible.books[bookIndex];
            Chapter selectedChapter = Bible.books[bookIndex].chapters[chapterIndex];
            Verses = selectedChapter.verses.Select(v => v.verse).ToList();

            Header = $"{selectedBook.book} {selectedChapter.chapter}";

            Page.Clear();

            List<Verse> verses = new List<Verse>();

            PageItems.UpdateLayout();
            PageContainer.UpdateLayout();

            Dispatcher.Invoke(() =>
            {
                for (int i = verseIndex; PageItems.ActualHeight < PageContainer.ActualHeight && i < selectedChapter.verses.Count; i++)
                {
                    Verse v = selectedChapter.verses[i];
                    Page.Add(v.verse + "    " + v.text);
                    verses.Add(v);

                    PageItems.UpdateLayout();
                    PageContainer.UpdateLayout();
                }

                if (Page.Count > 0 && verseIndex + Page.Count < selectedChapter.verses.Count)
                {
                    Page.RemoveAt(Page.Count - 1);
                }

                StartVerseIndex = verseIndex;
                EndVerseIndex = verseIndex + Page.Count - 1;

            }, DispatcherPriority.Loaded);

            return verses;
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
