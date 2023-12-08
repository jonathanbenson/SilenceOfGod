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

namespace sog;

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
public partial class MainWindow : Window, INotifyPropertyChanged
{

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

            }
        }, DispatcherPriority.Render);
    }
}