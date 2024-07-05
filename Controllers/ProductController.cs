using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using store.Models;
using store.Services.ProductService;

namespace store.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            return await _productService.GetAllProducts();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetSingleProduct(int id)
        {
            var result = await _productService.GetSingleProduct(id);
            if(result is null)
                return NotFound("Product not found");
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<ActionResult<List<Product>>> AddProduct(Product product)
        {
            var result = await _productService.AddProduct(product);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Product>>> UpdateProduct(int id, Product request)
        {
            var result = await _productService.UpdateProduct(id,request);
            if(result is null)
                return NotFound("Product not found");
            return Ok(result);
        }

<<<<<<< HEAD
        [HttpPut("SaleProduct")]
        public async Task<ActionResult<List<Product>>> SaleProduct(List<ListSale> listSales)
=======
        [HttpPut("SaleProduct/{id}/{quantity}")]
        public async Task<ActionResult<List<Product>>> SaleProduct(int id, int quantity)
>>>>>>> ea55c1144e62df6f4c5d1e75f386af6ea7b7d48b
        {
            var result = await _productService.SaleProduct(listSales);
            if(result is null)
                return NotFound("Product not found");
            return Ok(result);
        }

        [HttpPut("BuyProduct")]
        public async Task<ActionResult<List<Product>>> BuyProduct(List<ListSale> products)
        {
            var result = await _productService.BuyProduct(products);
            if(result is null)
                return NotFound("Product not found");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Product>>> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);
            if(result is null)
                return NotFound("Product not found");
            return Ok(result);
        }

    }
}