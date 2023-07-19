using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using store.Models;
namespace store.Services.ProductService
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProducts();
        Task<Product?> GetSingleProduct(int id);
        Task<List<Product>> AddProduct(Product product);   
        Task<List<Product>?> UpdateProduct(int id, Product request);
        Task<List<Product>?> DeleteProduct(int id);
        Task<List<Product>?> SaleProduct(int id, int quantity);
        Task<List<Product>?> BuyProduct(int id, int quantity);
    }
}