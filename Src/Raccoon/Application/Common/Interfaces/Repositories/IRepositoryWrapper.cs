using Application.Common.Interfaces.Repositories.RefreshToken;

namespace Application.Common.Interfaces.Repositories
{
    public interface IRepositoryWrapper
    {
        IRefreshTokenRepository RefreshTokenRepository { get; }
    }
}
