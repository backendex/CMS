using CMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CMS.src.Infrastructure.Persistence
{
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
        {
            if (context is ApplicationDbContext dynamicContext)
            {
                return (context.GetType(), dynamicContext.CurrentTableName, designTime);
            }

            return (context.GetType(), designTime);
        }

    }
}
