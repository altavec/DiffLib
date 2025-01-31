// <copyright file="HashcodeOccurance.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

internal class HashcodeOccurance(int position, HashcodeOccurance? next)
{
    public int Position { get; } = position;

    public HashcodeOccurance? Next { get; set; } = next;
}