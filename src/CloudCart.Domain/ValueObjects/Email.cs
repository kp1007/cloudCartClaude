using System.Text.RegularExpressions;

namespace CloudCart.Domain.ValueObjects;

public class Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be empty", nameof(value));
        }

        if (!IsValidEmail(value))
        {
            throw new ArgumentException("Invalid email format", nameof(value));
        }

        return new Email(value.ToLowerInvariant());
    }

    private static bool IsValidEmail(string email)
    {
        var emailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        return emailRegex.IsMatch(email);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Email other)
            return false;

        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString() => Value;
}
