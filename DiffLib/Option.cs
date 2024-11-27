// <copyright file="Option.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// The option class.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "This is by design.")]
public static class Option
{
    /// <summary>
    /// Gets an <see cref="Option{T}"/> that has no value.
    /// </summary>
    /// <typeparam name="T">The type of option.</typeparam>
    /// <returns>The option.</returns>
    public static Option<T> None<T>() => EmptyOption<T>.Value;

    private static class EmptyOption<T>
    {
#pragma warning disable CA1825, IDE0300, CS0649
        internal static readonly Option<T> Value;
#pragma warning restore CA1825, IDE0300, CS0649
    }
}
