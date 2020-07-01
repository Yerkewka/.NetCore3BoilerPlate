using Persistence.Base;
using Application.Common.Interfaces.Repositories.RefreshToken;

namespace Persistence.Repository.RefreshToken
{
    public class RefreshTokenRepository : BaseRepository<Domain.Entities.Indentity.RefreshToken>, IRefreshTokenRepository
    {
    }
}
