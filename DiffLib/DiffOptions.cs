// <copyright file="DiffOptions.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// This class is used to specify options to the diff algorithm.
/// </summary>
public class DiffOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the patience optimization is enabled. Default is <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// For more information about the patience optimization, see this question on Stack Overflow:
    /// <see href="https://stackoverflow.com/questions/4045017/what-is-git-diff-patience-for"/>.
    /// </remarks>
    public bool EnablePatienceOptimization { get; set; } = true;
}