namespace BlueFrames.Domain.Customers.Common;

public partial class LastName : ValueOf<string, LastName>
{
    [GeneratedRegex("^[a-z ,.'-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex CompiledLastNameRegex();
    
    private static readonly Regex LastNameRegex = CompiledLastNameRegex();

    protected override void Validate()
    {
        if (!string.IsNullOrWhiteSpace(Value) && LastNameRegex.IsMatch(Value))
        {
            return;
        }

        var message = $"{Value} is not a valid last name";
        throw new ValidationException(message, [
            new ValidationFailure(nameof(LastName), message)
        ]);
    }
}