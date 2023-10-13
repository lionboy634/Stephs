using Dapper;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Options;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Options;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stephs_Shop.Repositories
{
	public interface IOrderRepository
	{
		Task UpdateOrderDeliveryStatus(int orderId);
		Task<IEnumerable<Order>> GetAllOrders(int limit = 100, int offset = 0);
		Task<Order> GetOrderById(string id);
		Task<long> AddOrderDetail(string user, decimal total);
		Task AddOrderItem();

    }
	public class OrderRepository : PgRepository, IOrderRepository
	{
		public OrderRepository(IOptions<ConnectionStringOptions> option) : base(option)
		{

		}
		
		public async Task<IEnumerable<Order>> GetAllOrders(int limit = 100, int offset = 0)
		{

			using(var connection = await GetConnection())
			{
				var query = @"
					SELECT
					o.*
						from public.customer_order_details o
						 JOIN public.customer_order_items i
						 on o.id = i.order_id
						 JOIN public.customer c
						 on c.id = o.user_id
						 JOIN public.product p
						 on p.id = i.product_id
						 ORDER BY o.id
						 OFFSET @Offset
						 LIMIT @Limit ";

				return await connection.QueryAsync<Order>(query, new
				{
					Offset = offset,
					Limit = limit
				});
			}
			

		}

		public async Task GetUnDeliveredOrders(string address)
		{
			using(var connection = await GetConnection())
			{
				var query = @"
				 SELECT
					* from public.customer_order_details o
					 join public.customer_order_items i
					 on o.id = i.order_id
					 join public.customer c
					 on c.id = o.user_id
				     left join public.customer_address ca
					 on c.id = ca.customer
					 join public.product p
					 on p.id = i.product_id
					WHERE delivered is null
					order by o.id 
					LIMIT 100
					";

				if (!string.IsNullOrEmpty(address))
				{
					query += "and ca.addressline1 = @Address or ca.addressline2 = @Address";
				}
				await connection.QueryAsync(query);	

			}
		}


		public async  Task<Order> GetOrderById(string id)
		{
			using(var connection = await GetConnection())
			{
				var query = @"
				SELECT order_id, total, quantity
				 from public.customer_order_details o
				 join public.customer_order_items i
				 on o.id = i.order_id
				 join public.customer c
				 on c.id = o.user_id
				 join public.product p
				 on p.id = i.product_id
				where o.id = @Id
				";  

				return connection.QueryFirstOrDefault<Order>(query);
			}
		}

		public async Task<IEnumerable<Order>> GetCustomerOrders(string customerId)
		{
			using(var connection = await GetConnection())
			{
				var query = @"
				SELECT
				* from public.customer_order_details o
				 join public.customer_order_items i
				 on o.id = i.order_id
				 join public.customer c
				 on c.id = o.user_id
				 join public.product p
				 on p.id = i.product_id
				 where c.id = @CustomerId
				";
				return await connection.QueryAsync<Order>(query, new
				{
					CustomerId = customerId
				});
				
			}
		}

		public async Task<long> AddOrderDetail(string user, decimal total)
		{
			using(var connection = await GetConnection())
			{
                var query = @"
					INSERT 
						into public.customer_order_details(user_id, total, created_at)
						VALUES(@User, @Total, NOW()) 
						returning user_id";
				return await connection.ExecuteScalarAsync<long>(query, new
				{
					User = user,
					Total = total
				});

            }

        }


        public async Task AddOrderItem()
        {
            using (var connection = await GetConnection())
            {
                var query = @"
					INSERT 
						into public.order_item()
						VALUES() ";
                await connection.ExecuteScalarAsync(query);

            }

        }

        public async Task UpdateOrderDeliveryStatus(int orderId)
		{
			using(var connection = await GetConnection())
			{
				var query = @"
				 UPDATE public.customer_order_details
				 SET delivered = true
				 where id = @OrderId
				";

				await connection.ExecuteScalarAsync(query, new
				{
					OrderId = orderId
				});
			}
		}

	}
}
