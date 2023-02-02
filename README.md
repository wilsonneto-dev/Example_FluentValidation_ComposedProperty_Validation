Example:
```C#
class Personvalidator : AbstractValidator<Person>
{
    public Personvalidator()
    {
        RuleFor(x => GetPhoneNumberForValidation(x))
            .NotEmpty().WithMessage($"Mobile or Phone should not be empty");
        
        When(p => p.Country == "US" || p.Country == "CA", () =>
            RuleFor(x => GetPhoneNumberForValidation(x)).Length(10, 11)
            .WithMessage("Mobile or phone must have between 10 and 11 digits")
        );
        When(p => p.Country != "CA" && p.Country != "US", () =>
            RuleFor(x => GetPhoneNumberForValidation(x)).Length(10, 15)
            .WithMessage("Mobile or phone must have between 10 and 15 digits")
        );
    }

    public static string? GetPhoneNumberForValidation(Person x)
    {
        var number = string.IsNullOrEmpty(x.Mobile) ? x.Phone : x.Mobile;
        return new(number?.Where(char.IsDigit).ToArray());
    }
}

record Person(string FirstName, string LastName, string? Mobile, string? Phone, string? Country);
```
