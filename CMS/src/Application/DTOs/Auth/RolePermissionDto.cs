namespace CMS.src.Application.DTOs.Auth
{
    
    public class RolePermissionDto
    {
        public int RoleId { get; set; }
        public List<int> PermissionId { get; set; }
    }
}
