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

            StartVerseIndex = 0;
            EndVerseIndex = 9;
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

        private bool NextPage()
        {

            Chapter selectedChapter = Bible.books[BooksCombo.SelectedIndex].chapters[ChaptersCombo.SelectedIndex];

            if (EndVerseIndex == selectedChapter.verses.Count - 1)
                return false;

            Page.Clear();

            StartVerseIndex = EndVerseIndex + 1;

            int count = 0;
            for (int i = StartVerseIndex; i < StartVerseIndex + 10 && i < selectedChapter.verses.Count; i++)
            {
                Page.Add(selectedChapter.verses[i].verse + "    " + selectedChapter.verses[i].text);
                count++;
            }

            EndVerseIndex = StartVerseIndex + count - 1;

            return true;
        }

        private void HandleBookChange(int bookIndex)
        {
            if (bookIndex >= 0)
            {
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
                Book selectedBook = Bible.books[bookIndex];
                Chapter selectedChapter = Bible.books[bookIndex].chapters[chapterIndex];
                Verses = selectedChapter.verses.Select(v => v.verse).ToList();
                
                Header = $"{selectedBook.book} {selectedChapter.chapter}";

                Page.Clear();
                for (int i = 0; i < 10; i++)
                    Page.Add(selectedChapter.verses[i].verse + "    " + selectedChapter.verses[i].text);

                StartVerseIndex = 0;
                EndVerseIndex = 9;

                if (Verses.Any())
                {
                    VersesCombo.SelectedIndex = 0;
                }
            }
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
