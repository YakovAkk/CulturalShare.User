using ErrorOr;
using MediatR;
using static Service.Handlers.MediatRCommands;
using UserProto;
using Service.Services.Base;

namespace Service.Services.Handlers.Command;

public class SignInHandler : IRequestHandler<SignInCommand, ErrorOr<SignInResponse>>
{
    private readonly IAuthService _authService;
    public SignInHandler(IAuthService authService) => _authService = authService;
    public Task<ErrorOr<SignInResponse>> Handle(SignInCommand request, CancellationToken cancellationToken) =>
        _authService.GetSignInAsync(request.Request);
}
