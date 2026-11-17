namespace UniversalSmartHome.Domain.Commands;

/// <summary>
/// Encapsulates the outcome of validating a device command against device capabilities.
/// </summary>
public sealed class CommandValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<string> Errors { get; }

    private CommandValidationResult(bool isValid, IEnumerable<string>? errors = null)
    {
        IsValid = isValid;
        Errors = (errors ?? []).ToList();
    }

    public static CommandValidationResult Success() => new(true);
    public static CommandValidationResult Failure(string error) => new(false, [error]);
    public static CommandValidationResult Failure(IEnumerable<string> errors) => new(false, errors);
}
