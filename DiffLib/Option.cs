namespace DiffLib;

/// <summary>
/// This type functions similar to <see cref="Nullable{T}"/> except that it can hold any type of value and is used for situations where you may or may not have a value.
/// </summary>
/// <typeparam name="T">The type of option.</typeparam>
public readonly struct Option<T> : IEquatable<Option<T>>, IEquatable<T>
{
    private readonly T value;

    /// <summary>
    /// Constructs a new instance of <see cref="Option{T}"/> with the specified value.
    /// </summary>
    /// <param name="value">
    /// The value of this <see cref="Option{T}"/>.
    /// </param>
    public Option(T value)
    {
        this.value = value;
        this.HasValue = true;
    }

    /// <summary>
    /// Gets the value of this <see cref="Option{T}"/>.
    /// </summary>
    public readonly T Value => this.HasValue ? this.value : throw new InvalidOperationException("This Option<T> does not have a value");

    /// <summary>
    /// Gets the <see cref="Value"/> of this <see cref="Option{T}"/>, or the default value for <typeparamref name="T"/> if it has no value.
    /// </summary>
    public T? GetValueOrDefault() => this.HasValue ? this.Value : default;

    /// <summary>
    /// Gets whether this <see cref="Option{T}"/> has a value.
    /// </summary>
    public bool HasValue
    {
        get;
    }

    /// <summary>
    /// Implements equality operator.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator Option<T>(T value) => new(value);

    /// <summary>
    /// Implements inequality operator.
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public static explicit operator T(Option<T> option) => option.Value;

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public readonly bool Equals(Option<T> other)
    {
        var equalityComparer = EqualityComparer<T?>.Default;

        return equalityComparer!.Equals(this.value, other.value) && this.HasValue == other.HasValue;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public readonly bool Equals(T other)
    {
        if (!this.HasValue)
        {
            return false;
        }

        var equalityComparer = EqualityComparer<T?>.Default;

        return equalityComparer.Equals(this.value, other);
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <returns>
    /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
    /// </returns>
    /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
    public override bool Equals(object? obj) => obj is not null && obj is Option<T> && this.Equals((Option<T>)obj);

    /// <summary>
    /// Implements equality operator.
    /// </summary>
    /// <param name="option"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool operator ==(Option<T> option, Option<T> other) => option.Equals(other);

    /// <summary>
    /// Implements inequality operator.
    /// </summary>
    /// <param name="option"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool operator !=(Option<T> option, Option<T> other) => !option.Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer that is the hash code for this instance.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override readonly int GetHashCode()
    {
        unchecked
        {
            var equalityComparer = EqualityComparer<T?>.Default;

            return (equalityComparer.GetHashCode(this.value) * 397) ^ this.HasValue.GetHashCode();
        }
    }

    /// <summary>
    /// Returns the fully qualified type name of this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> containing a fully qualified type name.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override readonly string ToString()
    {
        var result = this.HasValue ? this.value?.ToString() ?? String.Empty : String.Empty;
        return result;
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that has no value.
    /// </summary>
    public static Option<T> None => new();
}