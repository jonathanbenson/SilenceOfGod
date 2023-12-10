
using sog.src;

namespace sog.test;

[TestClass]
public class BibleBuilderTest
{
    [TestMethod]
    public void TestBuild()
    {
        BibleBuilder bb = new BibleBuilder();

        Bible bible = bb.Build("bible");

        Assert.AreEqual(3, bible.books.Count);

        Book genesis = bible.books[0];
        Book exodus = bible.books[1];
        Book leviticus = bible.books[2];

        Assert.AreEqual("Genesis", genesis.book);
        Assert.AreEqual("Exodus", exodus.book);
        Assert.AreEqual("Leviticus", leviticus.book);

        Assert.AreEqual(50, genesis.chapters.Count);
        Assert.AreEqual(40, exodus.chapters.Count);
        Assert.AreEqual(27, leviticus .chapters.Count);

        string genesis11 = "In the beginning God created the heaven and the earth.";
        string exodus11 = "Now these are the names of the children of Israel, which came into Egypt; every man and his household came with Jacob.";
        string leviticus11 = "And the LORD called unto Moses, and spake unto him out of the tabernacle of the congregation, saying,";

        Assert.AreEqual(genesis11, genesis.chapters[0].verses[0].text);
        Assert.AreEqual(exodus11, exodus.chapters[0].verses[0].text);
        Assert.AreEqual(leviticus11, leviticus.chapters[0].verses[0].text);
    }
}