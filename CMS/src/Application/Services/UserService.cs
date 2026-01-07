using CMS.src.Application.Interfaces;

namespace CMS.src.Application.Services
{
    public class UserService : IUserService
    {
        public string GenerateTemporaryPassword()
        {
            string guid = Guid.NewGuid().ToString("N").Substring(0, 10);
            return $"{guid.ToUpper()[0]}{guid.Substring(1)}!1"; 
        }
    }
}
