using FluentAssertions;
using FluentValidation;

namespace tst;

public class UnitTest1
{
    private readonly Xunit.Abstractions.ITestOutputHelper _output;

    public UnitTest1(Xunit.Abstractions.ITestOutputHelper output) => _output = output;

    [Theory]
    [InlineData("1234567890", null, true)]
    [InlineData(null, "1234567890", true)]
    [InlineData(null, null, false)]
    [InlineData("", "1234567890", true)]
    public void ValidatePhoneNumberNullsOrEmpties(string mobile, string phone, bool expected)
    {
        var p = new Person("John", "Doe", mobile, phone, "BR");
        var t = Personvalidator.GetPhoneNumberForValidation(p);
        var validator = new Personvalidator();
        var result = validator.Validate(p);
        result.Errors?.ForEach(err => _output.WriteLine(err.ToString()));
        result.IsValid.Should().Be(expected, "test");
    }
    
    [Theory]
    [InlineData("123213", "CA", false)]
    [InlineData("123213", "US", false)]
    [InlineData("123213", "BR", false)]
    [InlineData("1232131230", "CA", true)]
    [InlineData("1232131230", "US", true)]
    [InlineData("1232131230", "BR", true)]
    [InlineData("12321312301", "CA", true)]
    [InlineData("12321312301", "US", true)]
    [InlineData("12321312301", "BR", true)]
    [InlineData("123213123012", "CA", false)]
    [InlineData("123213123012", "US", false)]
    [InlineData("123213123012", "BR", true)]
    [InlineData("123213123012312", "CA", false)]
    [InlineData("123213123012345", "US", false)]
    [InlineData("123213123012345", "BR", true)]
    [InlineData("1232131230123126", "CA", false)]
    [InlineData("1232131230123456", "US", false)]
    [InlineData("1232131230123456", "BR", false)]
    public void ChangeValidationByCountry(string phone, string country, bool expected)
    {
        var p = new Person("John", "Doe", null, phone, country);
        _output.WriteLine(Personvalidator.GetPhoneNumberForValidation(p));
        var validator = new Personvalidator();
        var result = validator.Validate(p);
        result.Errors?.ForEach(err => _output.WriteLine(err.ToString()));
        result.IsValid.Should().Be(expected);
    }
}

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
