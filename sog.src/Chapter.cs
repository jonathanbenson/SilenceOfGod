
namespace sog.src;

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