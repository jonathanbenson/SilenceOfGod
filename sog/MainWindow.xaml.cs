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

namespace sog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Bible Bible;

        public MainWindow()
        {
            InitializeComponent();

            BibleBuilder bibleBuilderService = new BibleBuilder();

            Bible = bibleBuilderService.Build("bible");

            DataContext = new {
                
                Books = Bible.books.Select((book) => book.book),
                Chapters = Bible.books[0].chapters.Select((chapter) => chapter.chapter),
                Verses = Bible.books[0].chapters[0].verses.Select((verse) => verse.verse)

            };

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

        private void HandleBooksSelectionChanged(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void HandleChaptersSelectionChanged(object sender, RoutedEventArgs e)
        {
            // TODO
        }

    }
}
