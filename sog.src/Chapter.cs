
namespace sog.src;

public class Chapter
{
    /*

    A class to contain Chapter information associated with Bible data

    */

    public string chapter { get; set; }
    public List<Verse> verses { get; set; }

    public Chapter(string chapter, List<Verse> verses)
    {
        this.chapter = chapter;
        this.verses = verses;
    }
}