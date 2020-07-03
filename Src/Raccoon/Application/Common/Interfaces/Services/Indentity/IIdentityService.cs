using Domain.Models.Indentity;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Services.Indentity
{
    public interface IIdentityService
    {
        Task<bool> SendCodeAsync(string username, bool debug = false, CancellationToken cancellationToken = default);
        Task<AuthenticationResult> LoginAsync(string username, string code, CancellationToken cancellationToken = default);
        Task<bool> RegisterAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(string username, CancellationToken cancellationToken = default);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    }
}
