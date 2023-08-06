using Dapper;
using Microsoft.Extensions.Options;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Entities;
using Stephs_Shop.Models.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stephs_Shop.Repositories
{

    public interface ICustomerRepository
    {
        Task<int> CreateShoppingSession(CustomerSession session);
        Task<Address> GetCustomerAddress(string customerid);
        Task AddAddress(string id, Address address);
        Task UpdateAddress(string id, Address address);
        Task<IEnumerable<Customer>> GetAllCustomers(DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int offset = 0, int limit = 0);
        Task<IEnumerable<Customer>> FetchCustomers(string searchTerm);
        Task CreateUserAsync(User userDetails);

        Task UpdateContact(string user_id, string contact);
        Task<int> GetCustomersCount();





	}
    public class CustomerRepository : PgRepository, ICustomerRepository
    {
        public CustomerRepository(IOptions<ConnectionStringOptions> options) : base(options)
        {

        }
         
        public async Task CreateUserAsync(User userDetails)
        {
            using(var connection = await GetConnection())
            {
                var query = @"
                   INSERT INTO public.customer(
	                    id, firstname, lastname, status, created_at, modified_at, email)
	                    VALUES (@userId, @Firstname, @Lastname, 'active' , NOW(), NOW(), @email);

                ";

                await connection.ExecuteScalarAsync(query, new
                {
                    userId = userDetails.Id,
                    Firstname = userDetails.FirstName,
                    Lastname = userDetails.LastName,
                    email = userDetails.Email
                });
            }
        }

		public async Task<int> GetCustomersCount()
		{
			using (var connection = await GetConnection())
			{
				var query = @"
                 SELECT count(1) from public.customer c
                 left join public.customer_address ca
                  on c.id = ca.customer";


				return await connection.ExecuteScalarAsync<int>(query);
			}

		}

		public async Task<IEnumerable<Customer>> GetAllCustomers(DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int offset = 0, int limit = 0)
        {
            using(var connection = await GetConnection())
            {
                var query = @"
                 SELECT * from public.customer c
                 left join public.customer_address ca
                  on c.id = ca.customer";

                if(startDate != null || endDate != null)
                {
                    query += "";
                }

                if(limit != 0)
                {
                    query += " OFFSET @offset LIMIT @limit ";
                }
                
            return await connection.QueryAsync<Customer>(query, new
            {
                offset = offset,
                limit = limit
            });
            }

        }

        public async Task UpdateContact(string user_id, string contact)
        {
            using (var connection = await GetConnection())
            {
                var query = @"update public.customer_address set
                    addressline1 = @address1, addressline2
                    where customer = @userid ";
                await connection.ExecuteScalarAsync(query, new
                {
                    userid = user_id
                });
            }
        }


        public async Task<IEnumerable<Customer>> FetchCustomers(string searchTerm)
        {
            using(var connection = await GetConnection())
            {
                var query = @"
                 SELECT c.* from public.customer c
                 left join public.customer_address ca
                 on c.id = ca.customer
                 where firstname ilike @term
                 or lastname ilike @term
                 or email ilike @term
                 ";

                return await connection.QueryAsync<Customer>(query, new
                {
                    term = searchTerm
                });

            }
        }

        public async Task<int> CreateShoppingSession(CustomerSession session)
        {
            using(var connection = await GetConnection())
            {
                var query = "INSERT INTO public.customer_shopping_session (customer, total, createdat)" +
                    "VALUES (@customer, @total + 1, current_timestamp)" +
                    "returning id";
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        #region address
        public async Task<Address> GetCustomerAddress(string customerid)
        {
            using (var connection = await GetConnection())
            {
                var query = "select ca.* from public.customer_address ca" +
                    "join public.customer c on c.id = ca.customer" +
                    "where  c.id = @id";

                return await connection.QueryFirstOrDefaultAsync<Address>(query, new
                {
                    id = customerid
                });

            }
        }

        public async Task<Customer> GetCustomerById(string customerId)
        {
            using(var connection = await GetConnection())
            {
                var query = @"
                 select c.id, concat(firstname, lastname) as fullname, email, ca.addressline1, ca.addressline2 from customer c
                  left join customer_address ca
                  on ca.user_id = c.id
                  where c.id = @Id
                 ";
                return await connection.QueryFirstAsync<Customer>(query, new
                {
                    Id = customerId
                });
            }
        }

        public async Task GetCustomerOrders()
        {
            using (var connection = await GetConnection())
            {
                var query = @"
                ";
            }
        }
        public async Task AddAddress(string id, Address address)
        {
            using(var connection = await GetConnection())
            {
                var query = "insert into public.customer_address(user_id, addressline1, addressline2, addressline3, postalcode)" +
                    "values(@user, @addressline1, @addressline2, @addressline3, @postalcode)";

                await connection.ExecuteScalarAsync(query, new
                {
                    user_id = id,
                    addressline1 = address.Addressline1,
                    addressline2 = address.Addressline2,
                    addressline3 = address.Addressline3,
                    postalcode = address.PostalCode
                });
            }
        }

        public async Task UpdateAddress(string id, Address address)
        {
            using(var connection = await GetConnection())
            {
                var query = @"update public.customer_address set
                    addressline1 = @address1, addressline2 = @address2, addressline3 = @address3, postalcode = @postcode
                    where customer = @userid ";
                await connection.ExecuteScalarAsync(query, new
                {
                    userid = id,
                    address1 = address.Addressline1,
                    address2 = address.Addressline2,
                    address3 = address.Addressline3,
                    postcode = address.PostalCode
                });
            }
        }

        #endregion

    }
}



