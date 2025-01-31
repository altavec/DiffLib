namespace DiffLib.Tests;

using DiffLib.Alignment;

public class ElementSimilarityDiffElementAlignerTests
{
    [Test]
    public void Constructor_NullSimilarityFunc_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => new ElementSimilarityDiffElementAligner<int>(null!));
}