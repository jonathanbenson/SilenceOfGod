using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Speech.Recognition;
using System.Windows;
using sog.src.model;

namespace sog
{
    public partial class ContentsWindow : ExitableWindow
    {
        public ObservableCollection<string> Books = new ObservableCollection<string>();

        public ContentsWindow(SpeechRecognitionEngine recognizer, Bible bible) : base(recognizer)
        {
            InitializeComponent();

            DataContext = this;

            ContentsItemsControl.ItemsSource = Books;

            List<string> contentsEntries = new List<string>();

            int colCount = bible.books.Count / 3;

            for (int i = 0; i < colCount; i++)
            {
                contentsEntries.Add((i + 1).ToString() + ". " + (bible.books[i]).ContentsEntry);
                contentsEntries.Add((i + colCount + 1).ToString() + ". " + (bible.books[i + colCount]).ContentsEntry);
                contentsEntries.Add((i + (colCount * 2) + 1).ToString() + ". " + (bible.books[i + (colCount * 2)]).ContentsEntry);
            }

            Books.Clear();
            foreach (string contentsEntry in contentsEntries)
                Books.Add(contentsEntry);

        }
    }
}