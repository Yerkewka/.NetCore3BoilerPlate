using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Services
{
    public interface ISmsService
    {
        Task<bool> SendAsync(string mobileNumber, string content, CancellationToken cancellationToken = default);
    }
}
