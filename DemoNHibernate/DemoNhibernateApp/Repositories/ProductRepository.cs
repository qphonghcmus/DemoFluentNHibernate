using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Channels;
using DemoNhibernateApp.Domain;
using NHibernate.Metadata;
using NHibernate.Type;

namespace DemoNhibernateApp.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private DataTable _table;
        private List<string> _columns;

        public ProductRepository()
        {
            //_table = new DataTable();
            //_columns = CreateTable(new Product());
        }

        private List<string> CreateTable(Product product)
        {
            List<string> ret = new List<string>();

            using (var session = NHibernateHelper.OpenSession())
            {
                IClassMetadata classMetadata = NHibernateHelper.GetClassMetadata(typeof(Product));
                string[] proppertiesOfTable_Name = classMetadata.PropertyNames;
                IType[] proppertiesOfTable_Type = classMetadata.PropertyTypes;

                var properties = product.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    if (classMetadata.IdentifierPropertyName == prop.Name)
                    {
                        _table.Columns.Add(new DataColumn(prop.Name, prop.PropertyType));
                        ret.Add(prop.Name);
                    }
                    else
                    {

                        int i = Array.FindIndex(proppertiesOfTable_Name, e => e == prop.Name);
                        if (!proppertiesOfTable_Type[i].IsEntityType &&
                            !proppertiesOfTable_Type[i].IsCollectionType)
                        {
                            _table.Columns.Add(new DataColumn(prop.Name, prop.PropertyType));
                            ret.Add(prop.Name);
                        }
                    }
                }
            }

            return ret;
        }

        public bool InsertBulk(List<Product> products)
        {

            AddRow(products);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var session = NHibernateHelper.OpenStatelessSession())
            using (var bulkCopy = new SqlBulkCopy(session.Connection as SqlConnection))
            {
                bulkCopy.DestinationTableName = "Product";
                bulkCopy.BulkCopyTimeout = 50000;
                foreach (DataColumn column in _table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                try
                {
                    bulkCopy.WriteToServer(_table);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Inserting times: " + stopwatch.Elapsed);
            return true;
        }

        private void AddRow(List<Product> products)
        {
            foreach (var product in products)
            {
                var row = _table.NewRow();
                var properties = product.GetType().GetProperties();

                foreach (var prop in properties)
                {
                    if (_columns.Contains(prop.Name))
                    {
                        row[prop.Name] = prop.GetValue(product);
                    }
                }

                _table.Rows.Add(row);
            }
        }


        public void Add(Product product)
        {
            using (var session = NHibernateHelper.OpenSession())
            using(var transaction = session.BeginTransaction())
            {
                session.SaveOrUpdate(product);
                transaction.Commit();
            }
        }

        public void Update(Product product)
        {
            Add(product);
        }

        public void Remove(Product product)
        {
            using (var session = NHibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Delete(product);
                transaction.Commit();
            }
        }

        public Product GetById(long Id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.Get<Product>(Id);
            }
        }

        public Product GetByName(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.Query<Product>().FirstOrDefault(p => p.Name == name);
            }
        }

        public IList<Product> Where(Expression<Func<Product, bool>> condition)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.Query<Product>().Where(condition).ToList();
            }
        }

        public IList<Product> GetAll()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.Query<Product>().ToList();
            }
        }

        public void Update(Product product, Expression<Func<Product, object>> field)
        {
            MemberExpression m;
            if (field.Body is MemberExpression)
            {
                m = field.Body as MemberExpression;
            }
            else
            {
                m = (field.Body as UnaryExpression).Operand as MemberExpression;
            }


            object value = null;
            var allProperties = product.GetType().GetProperties();
            foreach (var propertyInfo in allProperties)
            {
                if (propertyInfo.Name == m.Member.Name)
                {
                    value = propertyInfo.GetValue(product);
                    break;
                }
            }

            string sql = $"Update {product.GetType().Name} Set " + m.Member.Name + "= N\'" + value.ToString() + "\' where Id = " +
                         "N\'" +
                         product.Id.ToString() +
                         "\'";


            using (var session = NHibernateHelper.OpenStatelessSession())
            using(var transaction = session.BeginTransaction())
            {
                session.CreateSQLQuery(sql).AddEntity(typeof(Product)).UniqueResult();
                transaction.Commit();
            }
        }
    }
}
