using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Result;

namespace StellarWallet.Domain.Result
{
    public class Result<TValue, TError> : IResult<TValue, TError>
    {
        public TValue Value { get; }
        public bool IsSuccess { get; }
        public TError Error { get; }

        private Result(TValue value, bool isSuccess, TError error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<TValue, TError> Success(TValue value) => new(value, true, default);

        public static Result<TValue, TError> Failure(TError error) => new(default, false, error);
    }
}
