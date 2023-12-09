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


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const long LOADING_TIME = 10000;

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

        private CommandDispatcher CommandDispatcher = new CommandDispatcher();
        private Thread ListenerThread;
        private bool DoExitListenerThread = false;
        private bool DoListen = false;
        
        private string _VoiceMessage = "";

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

            BibleBuilder bibleBuilderService = new BibleBuilder();

            Bible = bibleBuilderService.Build("bible");

            Books = Bible.books.Select((book) => book.book).ToList();
            Chapters = Bible.books[0].chapters.Select((chapter) => chapter.chapter).ToList();
            Verses = Bible.books[0].chapters[0].verses.Select((verse) => verse.verse).ToList();

            RouteCommands();

            ListenerThread = new Thread(Listen)
            {
                IsBackground = true
            };
            ListenerThread.Start();

        }

        ~MainWindow()
        {
            DoExitListenerThread = true;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")  
        {  
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }  
    }
}
