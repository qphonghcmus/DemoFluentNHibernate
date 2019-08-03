using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using CreateInitialData.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NHibernate.Metadata;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;

namespace UnitTestProject
{
    [TestClass]
    public class CreateInitialData_Test
    {
        private ISessionFactory _sessionFactory;
        private Configuration _configuration;
        private List<Product> products = new List<Product>();
        private List<Detail> details = new List<Detail>();

        [TestMethod]
        public void TestFixtureSetUp()
        {
            _configuration = new Configuration();
            _configuration.Configure();
            _configuration.AddAssembly(typeof(Product).Assembly);
            _sessionFactory = _configuration.BuildSessionFactory();
        }

        [TestInitialize]
        public void SetUpContext()
        {
            TestFixtureSetUp();
            new SchemaExport(_configuration).Execute(false, true, false);
            //CreateIntialData(500000,100);
        }

        public void CreateIntialData(int nProduct, int mDetail)
        {
            for (int i = 0; i < nProduct; i++)
            {
                Product product = new Product()
                {
                    Name = "Product " + (i + 1),
                };
                //product.Detail = new HashSet<Detail>();

                //for (int j = 0; j < mDetail; j++)
                //{
                //    Detail detail = new Detail()
                //    {
                //        Note = "Note_" + (i+1) + "_" + (j+1),
                //    };
                //    product.Detail.Add(detail);
                //    details.Add(detail);
                //}
                products.Add(product);
            }
        }

        [TestMethod]
        public void Test_Insert()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var mProduct = products.Count;
            var nDetail = details.Count;

            //using (var session = _sessionFactory.OpenStatelessSession())
            //{
            //    using (var transaction = session.BeginTransaction())
            //    {
            //        foreach (var product in products)
            //        {
            //            session.Insert(product);
            //        }
            //        transaction.Commit();
            //    }
            //}

            using (var session = _sessionFactory.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    //session.SetBatchSize(1000);
                    for (int i = 0; i < 100000; i++)
                    {
                        Product product = new Product() { Name = "Product " + (i + 1) };
                        //session.Save(product);
                        //if (i % 20 == 0)
                        //{
                        //    session.Flush();
                        //    session.Clear();
                        //}
                        session.Insert(product);

                    }

                    //session.Flush();

                    //for (int i = 0; i < nDetail; i++)
                    //{
                    //    session.Save(details[i]);
                    //    //if (i % 50 == 0)
                    //    //{
                    //    //    session.Flush();
                    //    //    session.Clear();
                    //    //}
                    //}
                    transaction.Commit();
                }
            }

            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);
        }

        [TestMethod]
        public void Test_Insert_Bulk()
        {
            Stopwatch stopwatch = new Stopwatch();

            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var table = new DataTable();
                List<string> columns = CreateTable(table, new Product());
                for (int i = 0; i < 1000000; i++)
                {
                    Product product = new Product() { Name = "Product " + (i + 1) };
                    var row = table.NewRow();
                    var properties = product.GetType().GetProperties();

                    foreach (var prop in properties)
                    {
                        if (columns.Contains(prop.Name))
                        {
                            row[prop.Name] = prop.GetValue(product);
                        }
                    }

                    table.Rows.Add(row);
                }

                stopwatch.Start();
                using (var bulkCopy = new SqlBulkCopy(session.Connection as SqlConnection))
                {
                    bulkCopy.DestinationTableName = "Product";
                    bulkCopy.BulkCopyTimeout = 50000;
                    foreach (DataColumn column in table.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    try
                    {
                        bulkCopy.WriteToServer(table);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);
        }

        public List<string> CreateTable(DataTable table, Product product)
        {
            List<string> ret = new List<string>();
            using (var session = _sessionFactory.OpenSession())
            {
                IClassMetadata classMetadata = _sessionFactory.GetClassMetadata(typeof(Product));
                string[] proppertiesOfTable_Name = classMetadata.PropertyNames;
                IType[] proppertiesOfTable_Type = classMetadata.PropertyTypes;

                var properties = product.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    if (classMetadata.IdentifierPropertyName == prop.Name)
                    {
                        table.Columns.Add(new DataColumn(prop.Name, prop.PropertyType));
                        ret.Add(prop.Name);
                    }
                    else
                    {

                        int i = Array.FindIndex(proppertiesOfTable_Name, e => e == prop.Name);
                        if (!proppertiesOfTable_Type[i].IsEntityType &&
                            !proppertiesOfTable_Type[i].IsCollectionType)
                        {
                            table.Columns.Add(new DataColumn(prop.Name, prop.PropertyType));
                            ret.Add(prop.Name);
                        }
                    }
                }
            }

            return ret;
        }

        [TestMethod]
        public void Test_Get()
        {
            Test_Get();
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                }
            }
        }
    }
}
