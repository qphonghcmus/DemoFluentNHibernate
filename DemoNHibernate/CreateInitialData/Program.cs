using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreateInitialData.Domain;
using CreateInitialData.Repositories;

namespace CreateInitialData
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 1000000;
            List<Product> products = new List<Product>();
            for (int i = 0; i < n; i++)
            {
                Product product = new Product() {Name = "Product " + (i + 1)};
                products.Add(product);
            }

            ProductRepository productRepository = new ProductRepository();

            if (productRepository.InsertBulk(products))
            {
                Console.WriteLine("Insert Successfully");
            }
            else
            {
                Console.WriteLine("Insert Failed");
            }

            Console.ReadLine();
        }
    }
}
