using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store.Services.SaleService
{
    public class SaleService : ISaleService
    {
        private readonly DataContext _context;

        public SaleService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Sale>> GetAllSales()
        {
            var sales = await _context.Sales.ToListAsync();
            return sales;
        }

    }
}