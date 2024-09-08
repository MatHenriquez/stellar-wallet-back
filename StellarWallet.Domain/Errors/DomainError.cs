namespace StellarWallet.Domain.Errors
{
    public class DomainError(string? errorMessage, ErrorType errorType)
    {
        public string? ErrorMessage { get; set; } = errorMessage;
        public ErrorType ErrorType { get; set; } = errorType;
        public static DomainError NotFound(string? errorMessage = "Given parameter not found.") => new(errorMessage, ErrorType.NotFound);
        public static DomainError Invalid(string? errorMessage = "Invalid parameter.") => new(errorMessage, ErrorType.Invalid);
        public static DomainError Conflict(string? errorMessage = "Conflict with existing data.") => new(errorMessage, ErrorType.Conflict);
        public static DomainError ExternalServiceError(string? errorMessage = "External service error.") => new(errorMessage, ErrorType.ExternalServiceError);
    }
}
