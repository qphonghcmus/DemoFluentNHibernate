using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreateInitialData.Domain;
using NHibernate.Metadata;
using NHibernate.Type;

namespace CreateInitialData.Repositories
{
    public class ProductRepository
    {
        private DataTable _table;
        private List<string> _columns;

        public ProductRepository()
        {
            _table = new DataTable();
            _columns = CreateTable(new Product());
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

        

    }
}
