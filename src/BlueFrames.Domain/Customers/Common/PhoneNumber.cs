namespace BlueFrames.Domain.Customers.Common;

public partial class PhoneNumber : ValueOf<string, PhoneNumber>
{
    [GeneratedRegex(@"^((\+44)|(0)) ?\d{4} ?\d{6}$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex CompiledPhoneNumberRegex();
    
    private static readonly Regex PhoneNumberRegex = CompiledPhoneNumberRegex();

    protected override void Validate()
    {
        Value = Value?.Replace(" ", string.Empty);
        
        if (!string.IsNullOrWhiteSpace(Value) && PhoneNumberRegex.IsMatch(Value))
        {
            return;
        }

        var message = $"{Value} is not a valid phone number";
        throw new ValidationException(message, [
            new ValidationFailure(nameof(PhoneNumber), message)
        ]);
    }
}