using CMS.src.Domain.Entities;

namespace CMS.src.Application.Interfaces
{
    public interface ITokenService
    {
        // Este método recibirá al usuario y sus roles para meterlos en el token
        Task<string> CreateToken(User user, IList<string> roles);
    }
}
