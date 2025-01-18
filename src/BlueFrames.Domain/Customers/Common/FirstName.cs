namespace BlueFrames.Domain.Customers.Common;

public partial class FirstName : ValueOf<string, FirstName>
{
    [GeneratedRegex("^[a-z ,.'-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex CompiledFirstNameRegex();
    
    private static readonly Regex FirstNameRegex = CompiledFirstNameRegex();

    protected override void Validate()
    {
        if (!string.IsNullOrWhiteSpace(Value) && FirstNameRegex.IsMatch(Value))
        {
            return;
        }

        var message = $"{Value} is not a valid first name";
        throw new ValidationException(message, [
            new ValidationFailure(nameof(FirstName), message)
        ]);
    }
}