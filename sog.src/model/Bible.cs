
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace sog.src.model;

public class Verse
{
    public string? verse { get; set; }
    public string? text { get; set; }
}

public class Chapter
{
    public string? chapter { get; set; }
    public List<Verse>? verses { get; set; }
}

public class Book
{
    public string? book { get; set; }
    public List<Chapter>? chapters { get; set; }
}

public class Bible
{
    public List<Book> books = new List<Book>();
}

public class BookBuilderService
{
    public Book Build(string source)
    {
        string fileText = File.ReadAllText(source);

        return JsonSerializer.Deserialize<Book>(fileText)!;
    }

}

public class BibleBuilderService
{
    public Bible Build(string source)
    {

        Bible b = new Bible();

        string fileText = File.ReadAllText(Path.Join(source, "Books.json"));

        List<string> bookNames = JsonSerializer.Deserialize<List<string>>(fileText)!;

        BookBuilderService bookBuilderService = new BookBuilderService();

        foreach (string bookName in bookNames)
            b.books.Add(bookBuilderService.Build(Path.Join(source, bookName.Replace(" ", string.Empty) + ".json")));

        return b;

    }
}
