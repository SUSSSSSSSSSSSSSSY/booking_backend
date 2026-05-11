namespace Booking.Contracts.Responses.Auth;

public class AuthResult
{
    public bool Succeeded { get; set; }
    public AuthResponseDto? Data { get; set; }
    public AuthErrorCode ErrorCode { get; set; } = AuthErrorCode.None;

    public static AuthResult Success(AuthResponseDto data)
    {
        return new AuthResult
        {
            Succeeded = true,
            Data = data,
            ErrorCode = AuthErrorCode.None
        };
    }

    public static AuthResult Failure(AuthErrorCode errorCode)
    {
        return new AuthResult
        {
            Succeeded = false,
            ErrorCode = errorCode
        };
    }
}