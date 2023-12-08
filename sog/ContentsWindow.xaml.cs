using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using sog.src.model;

namespace sog
{
    public partial class ContentsWindow : Window
    {
        public ObservableCollection<string> Books = new ObservableCollection<string>();

        public ContentsWindow(Bible bible)
        {
            InitializeComponent();

            DataContext = this;


            ContentsItemsControl.ItemsSource = Books;

            Books.Clear();
            foreach (var (book, i) in bible.books.Select((value, i) => (value, i)))
                Books.Add((i+1).ToString() + ". " + book.ContentsEntry);

        }
    }
}