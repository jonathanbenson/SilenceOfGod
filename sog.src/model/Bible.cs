
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace sog.src.model;

public class Verse
{
    public string verse { get; set; }
    public string text { get; set; }

    public Verse(string verse, string text)
    {
        this.verse = verse;
        this.text = text;
    }
}

public class Chapter
{
    public string chapter { get; set; }
    public List<Verse> verses { get; set; }

    public Chapter(string chapter, List<Verse> verses)
    {
        this.chapter = chapter;
        this.verses = verses;
    }
}

public class Book
{
    public string book { get; set; }
    public List<Chapter> chapters { get; set; }

    public Book(string book, List<Chapter> chapters)
    {
        this.book = book;
        this.chapters = chapters;
    }
}

public class Bible
{
    public List<Book> books = new List<Book>();
}

public class BookBuilder
{
    public Book Build(string source)
    {
        string fileText = File.ReadAllText(source);

        return JsonSerializer.Deserialize<Book>(fileText)!;
    }

}

public class BibleBuilder
{
    public Bible Build(string source)
    {

        Bible b = new Bible();

        string fileText = File.ReadAllText(Path.Join(source, "Books.json"));

        List<string> bookNames = JsonSerializer.Deserialize<List<string>>(fileText)!;

        BookBuilder bookBuilderService = new BookBuilder();

        foreach (string bookName in bookNames)
            b.books.Add(bookBuilderService.Build(Path.Join(source, bookName.Replace(" ", string.Empty) + ".json")));

        return b;

    }
}
