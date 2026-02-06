using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs.Auth;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.src.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<AccessRole> GetRoleByIdAsync(int roleId)
        //{
        //    var role = await _context.AccessRoles
        //        .Include(r => r.RolePermissions)
        //            .ThenInclude(rp => rp.Permissions)
        //        .FirstOrDefaultAsync(r => r.Id == roleId);

        //    if (role == null) return null;

        //    return new AccessRole
        //    {
        //        Id = role.Id,
        //        NameRol = role.NameRol,
        //        RolePermissions = role.RolePermissions
        //                      .Select(rp => new PermissionDto
        //                      {
        //                          Id = rp.Permissions.Id,
        //                          PermissionKey = rp.Permissions.PermissionKey,
        //                          Description = rp.Permissions.Description
        //                      }).ToList()
        //    };
        //}

    }
}
