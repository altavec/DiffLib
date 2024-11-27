namespace DiffLib.Tests;

[TestFixture]
public class BasicInsertDeleteDiffElementAlignerTests
{
    [Test]
    public void Align_NullCollection1_ThrowsArgumentNullException()
    {
        var aligner = new BasicInsertDeleteDiffElementAligner<int>();

        IList<int>? collection1 = null;
        IList<int> collection2 = [];

        Assert.Throws<ArgumentNullException>(() => aligner.Align(collection1!, 0, 1, collection2, 0, 1).ToArray());
    }

    [Test]
    public void Align_NullCollection2_ThrowsArgumentNullException()
    {
        var aligner = new BasicInsertDeleteDiffElementAligner<int>();

        IList<int> collection1 = []; 
        IList<int>? collection2 = null;

        Assert.Throws<ArgumentNullException>(() => aligner.Align(collection1, 0, 1, collection2!, 0, 1).ToArray());
    }

    [Test]
    public void Align_ItemsOnlyInCollection1_OutputsDeletes()
    {
        var aligner = new BasicInsertDeleteDiffElementAligner<int>();

        IList<int> collection1 =
        [
            1,
            2,
            3
        ];
        IList<int> collection2 = [];

        var elements = aligner.Align(collection1, 0, collection1.Count, collection2, 0, collection2.Count).ToArray();

        Assert.That(elements, Is.EquivalentTo(new[]
        {
            new DiffElement<int>(0, 1, null, Option.None<int>(), DiffOperation.Delete),
            new DiffElement<int>(1, 2, null, Option.None<int>(), DiffOperation.Delete),
            new DiffElement<int>(2, 3, null, Option.None<int>(), DiffOperation.Delete),
        }));
    }

    [Test]
    public void Align_ItemsOnlyInCollection2_OutputsInserts()
    {
        var aligner = new BasicInsertDeleteDiffElementAligner<int>();

        IList<int> collection1 = [];
        IList<int> collection2 =
        [
            1,
            2,
            3
        ];

        var elements = aligner.Align(collection1, 0, collection1.Count, collection2, 0, collection2.Count).ToArray();

        Assert.That(elements, Is.EquivalentTo(new[]
        {
            new DiffElement<int>(null, Option.None<int>(), 0, 1, DiffOperation.Insert),
            new DiffElement<int>(null, Option.None<int>(), 1, 2, DiffOperation.Insert),
            new DiffElement<int>(null, Option.None<int>(), 2, 3, DiffOperation.Insert),
        }));
    }
}