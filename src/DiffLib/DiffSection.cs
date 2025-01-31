// <copyright file="DiffSection.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// This struct holds a section of matched or unmatch element portions from the two collectoins.
/// </summary>
[ExcludeFromCodeCoverage]
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]
public readonly record struct DiffSection(bool IsMatch, int LengthInCollection1, int LengthInCollection2)
{
    /// <summary>
    /// Returns the fully qualified type name of this instance.
    /// </summary>
    /// <returns>A <see cref="string"/> containing a fully qualified type name.</returns>
    public override string ToString() => this switch
    {
        { IsMatch: true } => $"{this.LengthInCollection1} matched",
        _ when this.LengthInCollection1 == this.LengthInCollection2 => $"{this.LengthInCollection1} did not match",
        { LengthInCollection1: 0 } => $"{this.LengthInCollection2} was present in collection2, but not in collection1",
        { LengthInCollection2: 0 } => $"{this.LengthInCollection1} was present in collection1, but not in collection2",
        _ => $"{this.LengthInCollection1} did not match with {this.LengthInCollection2}",
    };
}