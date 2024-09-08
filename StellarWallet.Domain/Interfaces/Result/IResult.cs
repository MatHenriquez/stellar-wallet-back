using StellarWallet.Domain.Result;

namespace StellarWallet.Domain.Interfaces.Result
{
    public interface IResult<TValue, TError>
    {
        TValue Value { get; }
        bool IsSuccess { get; }
        TError Error { get; }
    }
}
