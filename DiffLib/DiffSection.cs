// <copyright file="DiffSection.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// This struct holds a section of matched or unmatch element portions from the two collectoins.
/// </summary>
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]
public readonly struct DiffSection : IEquatable<DiffSection>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiffSection"/> struct.
    /// </summary>
    /// <param name="isMatch"><see langword="true"/> if a match was found between the two collections; otherwise, <see langword="false"/>.</param>
    /// <param name="lengthInCollection1">How many elements from the first collection this section contains.</param>
    /// <param name="lengthInCollection2">How many elements from the second collection this section contains.</param>
    public DiffSection(bool isMatch, int lengthInCollection1, int lengthInCollection2)
    {
        this.IsMatch = isMatch;
        this.LengthInCollection1 = lengthInCollection1;
        this.LengthInCollection2 = lengthInCollection2;
    }

    /// <summary>
    /// Gets a value indicating whether there the section specifies a match between the two collections or
    /// portions that could not be matched.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if a match was found between the two collections;
    /// otherwise, <see langword="false"/>.
    /// </value>
    public bool IsMatch
    {
        get;
    }

    /// <summary>
    /// Gets the number of elements from the first collection this section contains.
    /// </summary>
    public int LengthInCollection1 { get; }

    /// <summary>
    /// Gets the number of elements from the second collection this section contains.
    /// </summary>
    public int LengthInCollection2 { get; }

    /// <summary>
    /// Implements the equality operator.
    /// </summary>
    /// <param name="section1">The first section.</param>
    /// <param name="section2">The second section.</param>
    /// <returns>The result.</returns>
    public static bool operator ==(DiffSection section1, DiffSection section2) => section1.Equals(section2);

    /// <summary>
    /// Implements the inequality operator.
    /// </summary>
    /// <param name="section1">The first section.</param>
    /// <param name="section2">The second section.</param>
    /// <returns>The result.</returns>
    public static bool operator !=(DiffSection section1, DiffSection section2) => !section1.Equals(section2);

    /// <inheritdoc/>
    public readonly bool Equals(DiffSection other) => this.IsMatch == other.IsMatch && this.LengthInCollection1 == other.LengthInCollection1 && this.LengthInCollection2 == other.LengthInCollection2;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DiffSection diffSection && this.Equals(diffSection);

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        unchecked
        {
            var hashCode = this.IsMatch.GetHashCode();
            hashCode = (hashCode * 397) ^ this.LengthInCollection1;
            hashCode = (hashCode * 397) ^ this.LengthInCollection2;
            return hashCode;
        }
    }

    /// <inheritdoc/>
    public override readonly string ToString() => this switch
    {
        { IsMatch: true } => $"{this.LengthInCollection1} matched",
        _ when this.LengthInCollection1 == this.LengthInCollection2 => $"{this.LengthInCollection1} did not match",
        { LengthInCollection1: 0 } => $"{this.LengthInCollection2} was present in collection2, but not in collection1",
        { LengthInCollection2: 0 } => $"{this.LengthInCollection1} was present in collection1, but not in collection2",
        _ => $"{this.LengthInCollection1} did not match with {this.LengthInCollection2}",
    };
}