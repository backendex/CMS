using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace CMS.src.Infrastructure.Persistence.Interceptors
{
    public class DynamicTableInterceptor : DbCommandInterceptor
    {
        private readonly string _tableName;

        public DynamicTableInterceptor(string tableName)
        {
            _tableName = tableName;
        }

        //se inserta consulta nuevo método
        public override InterceptionResult<DbDataReader> ReaderExecuting(
           DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            if (command.CommandText.Contains("\"wp_posts\""))
            {
                command.CommandText = command.CommandText.Replace("\"wp_posts\"", $"\"{_tableName}\"");
            }
            return base.ReaderExecuting(command, eventData, result);
        }

        // Intercepta comandos de escritura (INSERT, UPDATE, DELETE)
        public override InterceptionResult<int> NonQueryExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            ReplaceTableName(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        private void ReplaceTableName(DbCommand command)
        {
            //Se reemplaza la versión con comillas como sin comillas
            if (command.CommandText.Contains("wp_posts"))
            {
                command.CommandText = command.CommandText
                    .Replace("\"wp_posts\"", $"\"{_tableName}\"")
                    .Replace("wp_posts", _tableName);
            }
        }
    }
}