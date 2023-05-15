using Dapper;
using Microsoft.Extensions.Options;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Options;
using System.Threading.Tasks;

namespace Stephs_Shop.Repositories
{
	public interface IReportRepository
	{
		Task<DashBoardReport> GetDashBoardReport();
	}
	public class ReportRepository : PgRepository, IReportRepository
	{
		public ReportRepository(IOptions<ConnectionStringOptions> option) : base(option)
		{
			
		}

		const string SalesReport = @"SELECT count(1) from public.customer_order_details ";
		const string OrdersReport = @"SELECT count(1) from public.customer_order_details";
		const string ProductInventory = "SELECT count(1) from public.product_inventory";

		public async Task<DashBoardReport> GetDashBoardReport()
		{
			using(var connection = await GetConnection())
			{
				var query = @$"SELECT
					
					( {SalesReport} ) SalesReport,
					( {OrdersReport} ) OrdersReport,
					( {ProductInventory} ) ProductInventory

				";

				return await connection.QuerySingleAsync<DashBoardReport>(query);
			}
			
		}
		


	}
}
