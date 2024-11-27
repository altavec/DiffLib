// <copyright file="MergeConflictException.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// This exception should be thrown by implementations of <see cref="IMergeConflictResolver{T}"/> to indicate a failure to resolve a conflict.
/// </summary>
public class MergeConflictException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MergeConflictException"/> class.
    /// </summary>
    /// <param name="message">The message indicating what the reason for the failure was.</param>
    /// <param name="commonBase">The common base of the elements involved in the conflict.</param>
    /// <param name="left">The left side of the conflict.</param>
    /// <param name="right">The right side of the conflict.</param>
    public MergeConflictException(string message, IEnumerable<object?> commonBase, IEnumerable<object?> left, IEnumerable<object?> right)
        : base(message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        this.CommonBase = commonBase.ToArray() ?? throw new ArgumentNullException(nameof(commonBase));
        this.Left = left.ToArray() ?? throw new ArgumentNullException(nameof(left));
        this.Right = right.ToArray() ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>
    /// Gets the common base of the elements involved in the conflict.
    /// </summary>
    public object?[] CommonBase { get; }

    /// <summary>
    /// Gets the left side of the conflict.
    /// </summary>
    public object?[] Left { get; }

    /// <summary>
    /// Gets the right side of the conflict.
    /// </summary>
    public object?[] Right { get; }
}