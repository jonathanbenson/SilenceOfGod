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

        private Dictionary<string, List<Verse>> PageLookup = new Dictionary<string, List<Verse>>();
        private Dictionary<string, int> PageKeyIndexLookup = new Dictionary<string, int>();
        private List<PageKey> PageKeys = new List<PageKey>();

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
            if (CurrentPageKey is not null && PageKeyIndexLookup[CurrentPageKey.ToString()] < PageKeys.Count - 1)
                CurrentPageKey = PageKeys[PageKeyIndexLookup[CurrentPageKey.ToString()] + 1];

            LoadPage(CurrentPageKey);
        }

        private void HandleLastPageButtonClicked(object sender, RoutedEventArgs e)
        {
            if (CurrentPageKey is not null && PageKeyIndexLookup[CurrentPageKey.ToString()] > 0)
                CurrentPageKey = PageKeys[PageKeyIndexLookup[CurrentPageKey.ToString()] - 1];

            LoadPage(CurrentPageKey);
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

                    Chapters = Bible.books[pageKey.BookIndex].chapters.Select((chapter) => chapter.chapter).ToList();
                    Verses = Bible.books[pageKey.BookIndex].chapters[pageKey.ChapterIndex].verses.Select((verse) => verse.verse).ToList();
                }
            }, DispatcherPriority.Render);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")  
        {  
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }  

    }
}
