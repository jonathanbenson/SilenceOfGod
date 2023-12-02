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

namespace sog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private Bible Bible;

        private List<string> _Books = new List<string>();
        private List<string> _Chapters = new List<string>();
        private List<string> _Verses = new List<string>();

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
            Book book = Bible.books[BooksCombo.SelectedIndex];

            Chapters = book.chapters.Select((chapter) => chapter.chapter).ToList();
            ChaptersCombo.SelectedIndex = 0;
        }

        private void HandleChaptersComboSelectionChanged(object sender, RoutedEventArgs e)
        {
            Chapter chapter = Bible.books[BooksCombo.SelectedIndex].chapters[ChaptersCombo.SelectedIndex];
        
            Verses = chapter.verses.Select((verse) => verse.verse).ToList();
            VersesCombo.SelectedIndex = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")  
        {  
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }  

    }
}
