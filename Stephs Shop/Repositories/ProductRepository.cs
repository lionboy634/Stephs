using Dapper;
using Microsoft.Extensions.Options;
using Stephs_Shop.Models;
using Stephs_Shop.Models.Options;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stephs_Shop.Repositories
{

    public interface IProductRepository
    {
        Task<int> AddProduct(Product product);
        Task DeleteProduct(int id);
        Task UpdateProduct(int id, Product product);
        Task<IEnumerable<ProductSubCategory>> GetProductSubCategory(string category);
        Task<IEnumerable<Product>> GetAllProduct(int limit = 0, int offset = 0);
        Task<Product> GetProduct(int id);
        Task<IEnumerable<Product>> GetProducts( int limit = 0, int offset = 0);
        Task<bool> CheckProductExists(int id);
        Task<IEnumerable<ProductCategory>> GetProductCategory();
        Task UpdateUploadedImage(int productId, FileStackResponse product);
        Task AddProductToInventory(int productId, int quantity);

	}


    public class ProductRepository : PgRepository, IProductRepository
    {
        public ProductRepository(IOptions<ConnectionStringOptions> options) : base(options)
        {

        }

        public async Task<int> AddProduct(Product product)
        {
            using(var connection = await GetConnection())
            {
                var query = @"
                   
                        INSERT INTO public.product(name, image_url, price,  category_id, inventory_id)
                        VALUES(@Name, @Imageurl, @Price, 1, 1000)  returning id
                    ";
                return await connection.ExecuteScalarAsync<int>(query, new
                {
                    Name = product.name,
                    Imageurl = product.image_url,
                    Price = product.price,
                    Descripton = product.description,
                    CategoryId = product.category_id,
                    InventoryId= product.inventory_id
                });   
            }
        }


        public async Task<bool> CheckProductExists(int id)
        {
            using (var connection = await GetConnection())
            {
                var query = @"
                 SELECT exists(
                    SELECT 1 FROM public.product 
                    WHERE id = @id
                )";

                return await connection.QuerySingleAsync<bool>(query);
            }

        }

        


        public async Task  DeleteProduct(int id)
        {
            using(var connection = await GetConnection())
            {
                var query = "DELETE FROM public.product WHERE id = @id";
                await connection.ExecuteScalarAsync(query, new { id = id});
            }
        }


        public async Task UpdateProduct(int id, Product product)
        {
            using(var connection = await GetConnection())
            {
                var query = "UPDATE public.stephs_product" +
                    "SET price=@price, name=@name, description=@desc where id = @id";
                await connection.ExecuteScalarAsync(query, new
                {
                    price = product.price,
                    name = product.name,
                    desc = product.description
                    
                });
            }
        }

        public async Task<IEnumerable<ProductSubCategory>> GetProductSubCategory(string category)
        {
            using(var connection = await GetConnection())
            {

                var query = @"
                            select sc.* from main_category mc
                            join product_category pc
                            on mc.id = pc.main_category_id
                            join product_sub_category sc
                            on sc.category = pc.id
                            where mc.name = @category ";

                return await connection.QueryAsync<ProductSubCategory>(query, new
                {
                    category = category
                });

            }
        }
		public async Task<IEnumerable<Product>> GetAllProduct(int limit = 0 , int offset = 0)
		{
			using (var connection = await GetConnection())
			{
				var query = @"
                SELECT
                    id, name
                    from public.product
                    OFFSET @Offset LIMIT @Limit
                ";
				return await connection.QueryAsync<Product>(query, new
                {
                    Limit = limit,
                    Offset = offset
                });
			}


		}

		public async Task<IEnumerable<ProductCategory>> GetProductCategory()
        {
            using (var connection = await GetConnection())
            {
                var query = @"select
                            id, name
                            from product_category";

                return await connection.QueryAsync<ProductCategory>(query);
            }
        }



        public async Task FetchProduct()
        {
            using(var connection = await GetConnection())
            {
                var query = @"
                    select name from product
                    where name ilike '@product%'
                    or name ilike '%product'
                    " ;
            }
        }

        public async Task<Product> GetProduct(int id)
        {
            using(var connection = await GetConnection())
            {
                var query = @"
                 select id, name from public.product 
                 where id = @Id
                ";

                return await connection.QuerySingleOrDefaultAsync<Product>(query, new
                {
                    Id = id
                });
            }
        }

        public async Task<IEnumerable<Product>> GetProducts(int limit = 0, int offset = 0)
        {
            using (var connection = await GetConnection())
            {
                var query = @"
                 SELECT * from public.product 
                     WHERE id = @Id
                     offset @Offset
                     LIMIT @Limit
                ";

                return await connection.QueryAsync<Product>(query, new
                {
                    Offset = offset,
                    Limit = limit
                });
            }



        }

        public async Task UpdateUploadedImage(int productId, FileStackResponse product)
        {
            using(var connection = await GetConnection())
            {
				var query = @"
                 UPDATE public.product
                    SET image_url = @imageUrl,
                        filepath = @filePath,
                        updated_at = NOW()
                    where id = @productId
                ";

                await connection.ExecuteScalarAsync(query, new
                {
                    productId = productId,
                    filePath = product.Filename,
                    imageUrl = product.Url
                });
			}
           

        }

        public async Task AddProductToInventory(int productId, int quantity)
        {
            using(var connection = await GetConnection())
            {
                var query = @"
                 INSERT INTO public.product_inventory(quantity, created_at, updated_at, inventory_name)
                            VALUES(@Quantity, NOW(), NOW(), "")
                ";
                await connection.ExecuteScalarAsync(query);
            }

        }
    }
}


