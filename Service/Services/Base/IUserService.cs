using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using UserProto;

namespace Service.Services.Base;

public interface IUserService
{
    Task<ErrorOr<Empty>> FollowUserAsync(FollowUserRequest request, int userId);
    Task<ErrorOr<Empty>> UnfollowUserAsync(UnfollowUserRequest request, int userId);
    Task<ErrorOr<Empty>> AllowUserAsync(AllowUserRequest request, int userId);
    Task<ErrorOr<Empty>> RestrictUserAsync(RestrictUserRequest request, int userId);
    Task<ErrorOr<SearchUserResponse>> SearchUserByNameAsync(SearchUserRequest request);
    Task<ErrorOr<Empty>> ToggleNotificationsAsync(ToggleNotificationsRequest request, int userId);
    Task<ErrorOr<int>> CreateUserAsync(CreateUserRequest request);
}
