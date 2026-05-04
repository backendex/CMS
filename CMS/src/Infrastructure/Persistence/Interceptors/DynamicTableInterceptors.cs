using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace CMS.src.Infrastructure.Persistence.Interceptors
{
    public class DynamicTableInterceptor : DbCommandInterceptor
    {
        private readonly string _tableName;

        public DynamicTableInterceptor(string tableName)
        {
            _tableName = tableName;
        }

        // Este se encarga de los SELECT (GetPostAsync)
        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            ManipulateCommand(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        // Este se encarga de los INSERT/UPDATE/DELETE (CreatePostAsync, UpdatePostAsync)
        public override InterceptionResult<int> NonQueryExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            ManipulateCommand(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        private void ManipulateCommand(DbCommand command)
        {
            // IMPORTANTE: EF Core por defecto usará el nombre de la clase "BlogPost" 
            // o el nombre que definiste en ToTable() en el SQL generado.
            command.CommandText = command.CommandText.Replace("BlogPost", _tableName);
        }
    }
}