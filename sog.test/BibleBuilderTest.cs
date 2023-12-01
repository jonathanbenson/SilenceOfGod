
using sog.src.model;

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

        BookBuilderTest bookBuilderTest = new BookBuilderTest();

        bookBuilderTest.AssertFirst3Books(genesis, exodus, leviticus);

    }
}