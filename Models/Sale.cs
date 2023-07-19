using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store.Models
{
    public class Sale
    {
        public int Id {get; set;}
        public int Quantity {get; set;}
        public double Vat {get; set;}
        public double TotalVat{get; set;}
        public double TotalSale{get; set;}
        
    }
}