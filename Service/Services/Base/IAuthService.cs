using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using UserProto;

namespace Service.Services.Base;

public interface IAuthService
{
    Task<ErrorOr<SignInResponse>> GetSignInAsync(SignInRequest request);
    Task<ErrorOr<Empty>> SignOutAsync(int userId);
}
