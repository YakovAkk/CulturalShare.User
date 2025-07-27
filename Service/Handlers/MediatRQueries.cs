using ErrorOr;
using MediatR;
using UserProto;

namespace Service.Handlers;

public class MediatRQueries
{
    public record SearchUserByNameQuery(SearchUserRequest Request) : IRequest<ErrorOr<SearchUserResponse>>;
}
