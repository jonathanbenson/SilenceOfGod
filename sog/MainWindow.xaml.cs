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
using sog.src;
using System.Net.Sockets;
using System.Threading;

namespace sog
{
    /*

    The main window of the application.

    This window holds the Bible verses that the user will read

    */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // How long it takes the window to load (milliseconds)
        // ...it takes some time for the application to parse the Bible .json
        // ...and fit the verses on to the pages
        private const long LOADING_TIME = 10000;

        // Bible contents (data found in sog/bible)
        private Bible Bible;

        // Lookup the verses by a PageKey id
        private Dictionary<string, List<Verse>> PageLookup = new Dictionary<string, List<Verse>>();

        // Lookup the index (page number - 1) of a PageKey
        private Dictionary<string, int> PageKeyIndexLookup = new Dictionary<string, int>();

        // A PageKey is a Book-Chapter-Verse location in the Bible
        private List<PageKey> PageKeys = new List<PageKey>();

        // The header at the top of the reader
        // Displays <Book> <Chapter>...example: "Genesis 1"
        private string _Header = "";

        // Keep track of the current location in the Bible
        public PageKey? _CurrentPageKey;

        // A list of verses that is rendered on the screen
        public ObservableCollection<Verse> _Page = new ObservableCollection<Verse>();

        private List<string> _Books = new List<string>();
        private List<string> _Chapters = new List<string>();
        private List<string> _Verses = new List<string>();

        // A utility object to dispatch functions given a voice command
        private CommandDispatcher CommandDispatcher = new CommandDispatcher();

        // The thread involved with listening for commands
        private Thread ListenerThread;

        // State variables for the listener
        private bool DoExitListenerThread = false;
        private bool DoListen = false;
        
        // Updates for the user after they say commands
        private string _VoiceMessage = "";

        // Below are bindings for the MainWindow
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

        public string VoiceMessage
        {
            get
            {
                return _VoiceMessage;
            }
            set
            {
                _VoiceMessage = value;
                NotifyPropertyChanged("VoiceMessage");
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            // Load the Bible data contained in sog/bible
            BibleBuilder bibleBuilderService = new BibleBuilder();

            Bible = bibleBuilderService.Build("bible");

            Books = Bible.books.Select((book) => book.book).ToList();
            Chapters = Bible.books[0].chapters.Select((chapter) => chapter.chapter).ToList();
            Verses = Bible.books[0].chapters[0].verses.Select((verse) => verse.verse).ToList();

            // Initialize the CommandDispatcher
            RouteCommands();

            // Start listening for commands
            ListenerThread = new Thread(Listen)
            {
                IsBackground = true
            };
            ListenerThread.Start();

        }

        ~MainWindow()
        {
            // Exit the ListenerThread so the application doesn't hang
            DoExitListenerThread = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")  
        {  
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }  
    }
}
