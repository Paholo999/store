using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using store.Data;
using store.Models;
using store.Services.SaleService;
using System.Text.Json;
using Confluent.Kafka;

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

        public async Task<List<Product>?> SaleProduct(List<ListSale> products)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var totalVat = 0.00;
                var totalSale = 0.00;
                var vat = 0.12;

                foreach(var productToUpdate in products)
                {
                    var product = await _context.Products.FindAsync(productToUpdate.Id);
                    // If the stock of the current product is less than the requested amount we throw an exception
                    if(product != null)
                    {
                        if (productToUpdate.Quantity <= 0 || productToUpdate.Quantity > product.Stock)
                        {
                            // Handle invalid quantity or insufficient stock for a specific product
                            // You can skip or log these products that fail to update
                            continue;
                        }
                         
                        product.Stock -= productToUpdate.Quantity;
                        totalVat += product.UnitPrice * productToUpdate.Quantity * vat;
                        totalSale += (product.UnitPrice * productToUpdate.Quantity) + (product.UnitPrice * productToUpdate.Quantity * vat);

                        
                    } 

                }
                
                Sale sale = new()
                {
                    Vat = vat,
                    TotalVat = totalVat,
                    TotalSale = totalSale
                };

                _context.Sales.Add(sale);


            }
            catch(Exception ex)
            {   
                
                await transaction.RollbackAsync();
                throw;
            }

            
           

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return await _context.Products.ToListAsync();
        }
        
        public async Task<List<Product>?> BuyProduct(List<ListSale> products)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach(var productToUpdate in products)
                {
                    var product = await _context.Products.FindAsync(productToUpdate.Id);
                    // If the stock of the current product is less than the requested amount we throw an exception
                    if(product != null)
                    {
                       
                        product.Stock += productToUpdate.Quantity;

                    }

                }
            }
            catch(Exception ex)
            {   
                
                await transaction.RollbackAsync();
                throw;
            }


            

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await _context.Products.ToListAsync();
        }

    }
}