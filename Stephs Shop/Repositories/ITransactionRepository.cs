﻿using Dapper;
using Microsoft.Extensions.Options;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stephs_Shop.Repositories
{
    public interface ITransactionRepository
    {
		Task<Transaction> FetchTransaction(string transactionId);
		Task<IEnumerable<Transaction>> GetAll();
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
						INTO public.transaction (id, customer_id, order_id)
						VALUES()
						returning *
					)
					INSERT INTO public.transaction_log 
					SELECT customer_id, order_id from new_transaction
				";
				return await  connection.ExecuteScalarAsync<Transaction>(query, new
				{

				});

			}
			
		}

		public async Task<Transaction> FetchTransaction(string transactionId)
		{
			using(var connection = await GetConnection())
			{
				var query = "";
				return await connection.QueryFirstAsync<Transaction>(query);
			}
		}

		public async Task<IEnumerable<Transaction>> GetAll()
		{
			using(var connection = await GetConnection())
			{
				var query = "";

				return await connection.QueryAsync<Transaction>(query);
			}
		}

		public Task ReverseTransaction()
		{
			throw new NotImplementedException();
		}
	}
}
