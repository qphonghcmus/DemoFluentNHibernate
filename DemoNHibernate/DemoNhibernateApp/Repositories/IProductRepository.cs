using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DemoNhibernateApp.Domain;

namespace DemoNhibernateApp.Repositories
{
    public interface IProductRepository
    {
        void Add(Product product);
        void Update(Product product);
        void Remove(Product product);
        Product GetById(long Id);
        Product GetByName(string name);
        IList<Product> Where(Expression<Func<Product, bool>> condition);
        IList<Product> GetAll();
        void Update(Product product, Expression<Func<Product, object>> field);
    }
}
