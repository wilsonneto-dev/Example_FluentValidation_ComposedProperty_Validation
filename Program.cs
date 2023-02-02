using FluentValidation;
using Xunit;

var p = new Person("John", "Doe", null, null);

// Console.WriteLine("Hello, World!");

class Personvalidator : AbstractValidator<Person>
{
    public Personvalidator() {
        RuleFor(x => x.Mobile ?? x.Phone).NotEmpty();
    }
}

record Person(string FirstName, string LastName, string? Mobile, string? Phone);

public class Test
{
    [Theory]
    [InlineData(null, null, false)]
    public void PersonWithNull(string mobile, string phone, bool expected)
    {
        var p = new Person("John", "Doe", mobile, phone);
        var validator = new Personvalidator();
        var result = validator.Validate(p);
        Assert.Equal(expected, result.IsValid);
    }
}