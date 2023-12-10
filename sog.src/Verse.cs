
namespace sog.src;

public class Verse
{
    /*

    A class to contain Verse information associated with Bible data

    */

    public string verse { get; set; }
    public string text { get; set; }

    public string NumAndText
    {
        get
        {
            return verse + "    " + text;
        }
    }

    public Verse(string verse, string text)
    {
        this.verse = verse;
        this.text = text;
    }
}