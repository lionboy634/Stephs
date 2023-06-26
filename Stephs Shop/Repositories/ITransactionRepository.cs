using Dapper;
using Microsoft.Extensions.Options;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Options;
using System;
using System.Threading.Tasks;

namespace Stephs_Shop.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateTransaction(Guid transasction_id , string customer_id, long order_id);
		Task ReverseTransaction();
    }



	public class TransactionRepository :  PgRepository, ITransactionRepository
	{
		public TransactionRepository(IOptions<ConnectionStringOptions> option):base(option)
		{

		}
		public async Task<Transaction> CreateTransaction(Guid transaction_id, string customer_id, long order_id)
		{
			if (customer_id is null) throw new ArgumentNullException("Customer Id cannot be null");

			using(var connection = await GetConnection())
			{
				var query = @"
					WITH new_transaction as (
					INSERT 
						INTO public.transaction (id, customer_id, transaction_date)
						VALUES()
						returning *
					)
					INSERT INTO public.transaction_log 
					SELECT * from new_transaction
					
				";
				return await  connection.ExecuteScalarAsync<Transaction>(query, new
				{

				});

			}
			
		}

		public Task ReverseTransaction()
		{
			throw new NotImplementedException();
		}
	}
}
