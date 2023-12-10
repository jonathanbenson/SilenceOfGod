
namespace sog.src;


public class Book
{
    public string book { get; set; }
    public List<Chapter> chapters { get; set; }

    public string ContentsEntry
    {
        set
        {
            book = value;
        }
        get
        {
            return book + " 1-" + chapters.Count.ToString();
        }
    }

    public Book(string book, List<Chapter> chapters)
    {
        this.book = book;
        this.chapters = chapters;
    }
}