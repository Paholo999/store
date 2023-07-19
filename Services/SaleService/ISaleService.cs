using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store.Services.SaleService
{
    public interface ISaleService
    {
        Task<List<Sale>> GetAllSales();
    }
}