using Dapper;
using Microsoft.Extensions.Options;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stephs_Shop.Repositories
{

    public interface ICartRepository
    {
        Task AddCart(Cart cart, string sessionId);
        Task RemoveCart(int id, string sessionId);
		Task<IEnumerable<Cart>> LoadCart(int id, string sessionId);


	}
	public class CartRepository : PgRepository, ICartRepository
	{
		private readonly IOptions<ConnectionStringOptions> _options;

		public CartRepository(IOptions<ConnectionStringOptions> options) : base(options)
		{

		}
		public async Task AddCart(Cart cart, string sessionId)
		{
			using (var connection = await GetConnection())
			{
				var query = @"
					INSERT INTO public.cart(session_id, created_at)
					VALUES()
				";

				await connection.ExecuteScalarAsync(query);
			}
		}

		public async Task<IEnumerable<Cart>> LoadCart(int id, string sessionId)
		{
			using(var connection = await GetConnection())
			{
				var query = @"
					SELECT * from public.cart
					WHERE id = @cartId AND session_id = @sid
				";


				return await connection.QueryAsync<Cart>(query, new
				{
					sid = sessionId,
					cartId = id

				});	
			}
		}

		public Task RemoveCart(int id, string sessionId)
		{
			throw new System.NotImplementedException();
		}
	}
}
