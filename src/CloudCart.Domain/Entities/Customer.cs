using CloudCart.Domain.ValueObjects;

namespace CloudCart.Domain.Entities;

public class Customer : BaseEntity
{
    public Email Email { get; private set; } = null!;
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PhoneNumber { get; private set; }

    // Navigation properties
    public ICollection<Order> Orders { get; private set; }
    public ShoppingCart? ShoppingCart { get; private set; }

    private Customer()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        PhoneNumber = string.Empty;
        Orders = new List<Order>();
    }

    public static Customer Create(string email, string firstName, string lastName, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        return new Customer
        {
            Email = Email.Create(email),
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber ?? string.Empty
        };
    }

    public void Update(string firstName, string lastName, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber ?? string.Empty;
        MarkAsUpdated();
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}
