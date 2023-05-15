using Microsoft.Extensions.Options;
using Npgsql;
using Stephs_Shop.Models.Options;
using System.Data.Common;
using System.Threading.Tasks;

namespace Stephs_Shop.Repositories
{
    public interface IPgRepository
    {
        Task<NpgsqlConnection> GetConnection(Db db);
    }
    public class PgRepository : IPgRepository
    {
        private ConnectionStringOptions _options;
        public PgRepository(IOptions<ConnectionStringOptions> options)
        {
            _options = options.Value;
        }

        public async Task<NpgsqlConnection> GetConnection(Db db = Db.stephsCommerce)
        {
            string connstring = "";
            switch (db)
            {
                case Db.stephsCommerce:
                    connstring = _options.CommerceDb;
                    break;
                default:
                    throw new System.Exception("Unknown database");
                    
            }

            var connection = new NpgsqlConnection(connstring);
            await connection.OpenAsync();
            return connection;
        }


       

    }
    public enum Db
    {
        stephsCommerce,
        PayCommerce
    }
}
