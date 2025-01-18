namespace BlueFrames.Domain.Customers.Common;

public partial class Email : ValueOf<string, Email>
{
    [GeneratedRegex(@"^[\w!#$%&’*+/=?`{|}~^-]+(?:\.[\w!#$%&’*+/=?`{|}~^-]+)*@(?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex CompiledEmailMyRegex();
    
    private static readonly Regex EmailRegex = CompiledEmailMyRegex();
    
    protected override void Validate()
    {
        if (!string.IsNullOrWhiteSpace(Value) && EmailRegex.IsMatch(Value))
        {
            return;
        }

        var message = $"{Value} is not a valid email address";
        throw new ValidationException(message, [
            new ValidationFailure(nameof(Email), message)
        ]);
    }
}
