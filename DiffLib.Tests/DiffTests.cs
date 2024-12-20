﻿namespace DiffLib.Tests;

[TestFixture]
public class DiffTests
{
    [Test]
    public void CalculateSections_NullCollection1_ThrowsArgumentNullException()
    {
        List<string>? collection1 = null;
        var collection2 = new List<string>();
        EqualityComparer<string> comparer = EqualityComparer<string>.Default;

        Assert.Throws<ArgumentNullException>(() => Diff.CalculateSections(collection1!, collection2, comparer));
    }

    [Test]
    public void CalculateSections_NullCollection2_ThrowsArgumentNullException()
    {
        var collection1 = new List<string>();
        List<string>? collection2 = null;
        EqualityComparer<string> comparer = EqualityComparer<string>.Default;

        Assert.Throws<ArgumentNullException>(() => Diff.CalculateSections(collection1, collection2!, comparer));
    }

    [Test]
    public void AlignElements_NullCollection1_ThrowsArgumentNullException()
    {
        IList<int>? collection1 = null;
        IList<int> collection2 = [];
        IEnumerable<DiffSection> diffSections = [];
        IDiffElementAligner<int> aligner = new BasicInsertDeleteDiffElementAligner<int>();
        Assert.Throws<ArgumentNullException>(() => Diff.AlignElements(collection1!, collection2, diffSections, aligner));
    }

    [Test]
    public void AlignElements_NullCollection2_ThrowsArgumentNullException()
    {
        IList<int> collection1 = [];
        IList<int>? collection2 = null;
        IEnumerable<DiffSection> diffSections = [];
        IDiffElementAligner<int> aligner = new BasicInsertDeleteDiffElementAligner<int>();
        Assert.Throws<ArgumentNullException>(() => Diff.AlignElements(collection1, collection2!, diffSections, aligner));
    }

    [Test]
    public void AlignElements_NullDiffSections_ThrowsArgumentNullException()
    {
        IList<int> collection1 = [];
        IList<int> collection2 = [];
        IEnumerable<DiffSection>? diffSections = null;
        IDiffElementAligner<int> aligner = new BasicInsertDeleteDiffElementAligner<int>();
        Assert.Throws<ArgumentNullException>(() => Diff.AlignElements(collection1, collection2, diffSections!, aligner));
    }

    [Test]
    public void AlignElements_NullAligner_ThrowsArgumentNullException()
    {
        IList<int> collection1 = [];
        IList<int> collection2 = [];
        IEnumerable<DiffSection> diffSections = [];
        IDiffElementAligner<int>? aligner = null;
        Assert.Throws<ArgumentNullException>(() => Diff.AlignElements(collection1, collection2, diffSections, aligner!));
    }

    [Test]
    public void SimpleDiff_ProducesCorrectResults()
    {
        const string text1 = "This is a test of the diff implementation, with some text that is deleted.";
        const string text2 = "This is another test of the same implementation, with some more text.";

        DiffSection[] diff = Diff.CalculateSections(text1.ToCharArray(), text2.ToCharArray()).ToArray();

        Assert.That(diff, Is.EquivalentTo(new[]
        {
            new DiffSection(isMatch: true, lengthInCollection1: 9, lengthInCollection2: 9), // same        "This is a"
            new DiffSection(isMatch: false, lengthInCollection1: 0, lengthInCollection2: 6), // add        "nother"
            new DiffSection(isMatch: true, lengthInCollection1: 13, lengthInCollection2: 13), // same      " test of the "
            new DiffSection(isMatch: false, lengthInCollection1: 4, lengthInCollection2: 4), // replace    "same" with "diff"
            new DiffSection(isMatch: true, lengthInCollection1: 27, lengthInCollection2: 27), // same      " implementation, with some "
            new DiffSection(isMatch: false, lengthInCollection1: 0, lengthInCollection2: 5), // add        "more "
            new DiffSection(isMatch: true, lengthInCollection1: 4, lengthInCollection2: 4), // same        "text"
            new DiffSection(isMatch: false, lengthInCollection1: 16, lengthInCollection2: 0), // delete    " that is deleted"
            new DiffSection(isMatch: true, lengthInCollection1: 1, lengthInCollection2: 1), // same        "."
        }));
    }

    [Test]
    public void Diff_WithNullElements()
    {
        var collection1 = new[]
        {
            "Line 1", "Line 2", null, "Line 3", "Line 4",
        };

        var collection2 = new[]
        {
            "Line 1", null, "Line 2", "Line 4",
        };

        DiffSection[] sections = Diff.CalculateSections(collection1, collection2).ToArray();

        Assert.That(sections, Is.EquivalentTo(new[]
        {
            new DiffSection(isMatch: true, lengthInCollection1: 1, lengthInCollection2: 1),
            new DiffSection(isMatch: false, lengthInCollection1: 0, lengthInCollection2: 1),
            new DiffSection(isMatch: true, lengthInCollection1: 1, lengthInCollection2: 1),
            new DiffSection(isMatch: false, lengthInCollection1: 2, lengthInCollection2: 0),
            new DiffSection(isMatch: true, lengthInCollection1: 1, lengthInCollection2: 1),
        }));
    }

    [Test]
    public void Align_WithNullElements()
    {
        var collection1 = new[]
        {
            "Line 1", "Line 2", null, "Line 3", "Line 4",
        };

        var collection2 = new[]
        {
            "Line 1", null, "Line 2", "Line 4",
        };

        DiffSection[] sections = Diff.CalculateSections(collection1, collection2).ToArray();
        var elements = Diff.AlignElements(collection1, collection2, sections, new StringSimilarityDiffElementAligner());

        Assert.That(elements, Is.EquivalentTo(new[]
        {
            new DiffElement<string?>(0, "Line 1", 0, "Line 1", DiffOperation.Match),
            new DiffElement<string?>(null, Option.None<string?>(), 1, null, DiffOperation.Insert),
            new DiffElement<string?>(1, "Line 2", 2, "Line 2", DiffOperation.Match),
            new DiffElement<string?>(2, null, null, Option.None<string?>(), DiffOperation.Delete),
            new DiffElement<string?>(3, "Line 3", null, Option.None<string?>(), DiffOperation.Delete),
            new DiffElement<string?>(4, "Line 4", 3, "Line 4", DiffOperation.Match),
        }));
    }
}