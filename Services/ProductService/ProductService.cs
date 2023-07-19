using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using store.Data;
using store.Models;
using store.Services.SaleService;

namespace store.Services.ProductService
{
    public class ProductService : IProductService
    {
        
        private readonly DataContext _context;
        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return await _context.Products.ToListAsync();
        }

        public async Task<List<Product>?> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product is null)
                return null;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            
            return await _context.Products.ToListAsync();
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }

        public async Task<Product?> GetSingleProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return null;
            return product;
        }

        public async Task<List<Product>?> UpdateProduct(int id, Product request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return null;
            product.Name = request.Name;
            product.UnitPrice = request.UnitPrice;
            product.Description = request.Description;

            await _context.SaveChangesAsync();
            
            return await _context.Products.ToListAsync();
        }

        public async Task<List<Product>?> SaleProduct(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return null;
            product.Stock = product.Stock - quantity;

            Sale sale = new Sale();
            sale.Quantity = quantity;
            sale.Vat = 0.12;
            sale.TotalVat = (product.UnitPrice*quantity)*0.12;
            sale.TotalSale = (product.UnitPrice*quantity) + ((product.UnitPrice*quantity)*0.12);
            _context.Sales.Add(sale);

            await _context.SaveChangesAsync();
            
            return await _context.Products.ToListAsync();
        }
        
        public async Task<List<Product>?> BuyProduct(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
                return null;
            product.Stock = product.Stock + quantity;

            await _context.SaveChangesAsync();
            
            return await _context.Products.ToListAsync();
        }

    }
}