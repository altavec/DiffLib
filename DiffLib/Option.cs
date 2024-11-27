// <copyright file="Option.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

/// <summary>
/// This type functions similar to <see cref="Nullable{T}"/> except that it can hold any type of value and is used for situations where you may or may not have a value.
/// </summary>
/// <typeparam name="T">The type of option.</typeparam>
public readonly struct Option<T> : IEquatable<Option<T>>, IEquatable<T>
{
    private readonly T value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Option{T}"/> struct.
    /// </summary>
    /// <param name="value">The value of this <see cref="Option{T}"/>.</param>
    public Option(T value)
    {
        this.value = value;
        this.HasValue = true;
    }

    /// <summary>
    /// Gets an <see cref="Option{T}"/> that has no value.
    /// </summary>
    public static Option<T> None => default;

    /// <summary>
    /// Gets the value of this <see cref="Option{T}"/>.
    /// </summary>
    public readonly T Value => this.HasValue ? this.value : throw new InvalidOperationException("This Option<T> does not have a value");

    /// <summary>
    /// Gets a value indicating whether this <see cref="Option{T}"/> has a value.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// Implements equality operator.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    public static implicit operator Option<T>(T value) => new(value);

    /// <summary>
    /// Implements inequality operator.
    /// </summary>
    /// <param name="option">The option.</param>
    /// <returns>The result.</returns>
    public static explicit operator T(Option<T> option) => option.Value;

    /// <summary>
    /// Implements equality operator.
    /// </summary>
    /// <param name="option">The option.</param>
    /// <param name="other">The other.</param>
    /// <returns>The result.</returns>
    public static bool operator ==(Option<T> option, Option<T> other) => option.Equals(other);

    /// <summary>
    /// Implements inequality operator.
    /// </summary>
    /// <param name="option">The option.</param>
    /// <param name="other">The other.</param>
    /// <returns>The result.</returns>
    public static bool operator !=(Option<T> option, Option<T> other) => !option.Equals(other);

    /// <summary>
    /// Gets the <see cref="Value"/> of this <see cref="Option{T}"/>, or the default value for <typeparamref name="T"/> if it has no value.
    /// </summary>
    /// <returns>The value or default.</returns>
    public T? GetValueOrDefault() => this.HasValue ? this.Value : default;

    /// <inheritdoc/>
    public readonly bool Equals(Option<T> other) => EqualityComparer<T?>.Default.Equals(this.value, other.value) && this.HasValue == other.HasValue;

    /// <inheritdoc/>
    public readonly bool Equals(T other) => this.HasValue && EqualityComparer<T?>.Default.Equals(this.value, other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is not null && obj is Option<T> option && this.Equals(option);

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        unchecked
        {
            return (EqualityComparer<T?>.Default.GetHashCode(this.value) * 397) ^ this.HasValue.GetHashCode();
        }
    }

    /// <inheritdoc/>
    public override readonly string ToString() => this.HasValue ? this.value?.ToString() ?? string.Empty : string.Empty;
}