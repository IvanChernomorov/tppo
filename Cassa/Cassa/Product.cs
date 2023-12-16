using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassa
{
    public class Product
    {
        public int ID { get; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double Remains { get; set; }
        public string Description { get; set; }
        public string Restriction { get; }

        public Product(int ID, string name, decimal price, double remains, string description, string restriction)
        {
            this.ID = ID;
            Name = name;
            Price = price;
            Remains = remains;
            Description = description;
            Restriction = restriction;
        }
    }
}
