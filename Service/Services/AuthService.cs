using AuthenticationBackProto;
using CulturalShare.Foundation.EnvironmentHelper.Configurations;
using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Repositories;
using Service.Helper;
using Service.Services.Base;
using UserProto;

namespace Service.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<AuthService> _logger;
    private readonly AuthenticationBackGrpcService.AuthenticationBackGrpcServiceClient _authenticationBackGrpcServiceClient;
    private readonly JwtServicesConfig _jwtServicesConfig;
    private readonly IMicroserviceAuthWrapper _microserviceAuthWrapper;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ILogger<AuthService> logger,
        AuthenticationBackGrpcService.AuthenticationBackGrpcServiceClient authenticationBackGrpcServiceClient,
        JwtServicesConfig jwtServicesConfig,
        IMicroserviceAuthWrapper microserviceAuthWrapper)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _logger = logger;
        _authenticationBackGrpcServiceClient = authenticationBackGrpcServiceClient;
        _jwtServicesConfig = jwtServicesConfig;
        _microserviceAuthWrapper = microserviceAuthWrapper;
    }

    public async Task<ErrorOr<SignInResponse>> GetSignInAsync(SignInRequest request)
    {
        var user = await _userRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            _logger.LogError($"{nameof(GetSignInAsync)} request. User with email = {request.Email} doesn't exist!");
            return Error.NotFound("UserNotFound", $"User with email = {request.Email} doesn't exist!");
        }

        var isPasswordValid = _passwordService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);

        if (!isPasswordValid)
        {
            _logger.LogDebug($"Invalid login attempt for user with email = {user.Email}");
            return Error.Validation("InvalidCredentials", "Email or password is incorrect.");
        }

        var userToken = await _microserviceAuthWrapper.ExecuteWithUnaryCallAsync(headers => _authenticationBackGrpcServiceClient.GetUserTokenAsync(new UserTokenRequest
        {
            UserId = user.Id,
            Email = user.Email
        }, headers: headers));

        if (userToken == null)
        {
            return Error.Failure("ServiceTokenError", "Could not retrieve user token.");
        }

        return new SignInResponse()
        {
            AccessToken = userToken.AccessToken, 
            RefreshToken = userToken.RefreshToken,
            AccessTokenExpiresInSeconds = userToken.AccessTokenExpiresInSeconds,
            RefreshTokenExpiresInSeconds = userToken.RefreshTokenExpiresInSeconds,
        };
    }



    public async Task<ErrorOr<Empty>> SignOutAsync(int userId)
    {
        _logger.LogInformation("SignOut request received");

        var jwtConfig = _jwtServicesConfig.ServicesJwtConfigs.FirstOrDefault(x => x.ServiceId == "user-service");

        if (jwtConfig == null)
        {
            _logger.LogError("JWT configuration for the service not found.");
            return Error.Failure("JwtConfigError", "JWT configuration for the service not found.");
        }

        await _microserviceAuthWrapper.ExecuteWithUnaryCallAsync(headers => _authenticationBackGrpcServiceClient.RevokeUserTokenAsync(new RevokeUserTokenRequest()
        {
            UserId = userId
        }, headers: headers));

        return new Empty();
    }


}
