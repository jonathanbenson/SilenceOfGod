using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
namespace sog.src;

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

class BookBuilder
{
    public Book Build(string source)
    {
        string fileText = File.ReadAllText(source);

        return JsonSerializer.Deserialize<Book>(fileText)!;
    }

}