
namespace sog.src;

public class Verse
{
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