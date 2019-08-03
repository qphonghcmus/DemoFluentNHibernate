using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CreateInitialData.Domain;
using DemoNhibernateApp.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Action;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Metadata;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;
using NHibernateHelper = CreateInitialData.Repositories.NHibernateHelper;

namespace UnitTestProject
{
    [TestClass]
    public class CRUD_Test
    {
        private ISessionFactory _sessionFactory;
        private Configuration _configuration;
        private List<Product> _products = new List<Product>();

        [TestMethod]
        public void TestFixtureSetUp()
        {
            _configuration = new Configuration();
            _configuration.Configure();
            //_configuration.AddAssembly(typeof(Product).Assembly);
            _configuration.AddAssembly(typeof(DemoNhibernateApp.Domain.Product).Assembly);
            _sessionFactory = _configuration.BuildSessionFactory();
        }

        [TestMethod]
        public void SetUpContext()
        {
            TestFixtureSetUp();
            //new SchemaExport(_configuration).Execute(false, true, false);
            //CreateInitialData();
        }

        private ISessionFactory BuildSessionFactory()
        {
            Configuration cfg = new Configuration();
            cfg.Configure();
            cfg.AddAssembly(typeof(DemoNhibernateApp.Domain.Product).Assembly);
            return cfg.BuildSessionFactory();
        }

        public void CreateInitialData()
        {
            using (var session = _sessionFactory.OpenStatelessSession())
            {
                var table = new DataTable();
                List<string> columns = CreateTable(table, new Product());
                for (int i = 0; i < 1000000; i++)
                {
                    Product product = new Product()
                    {
                        Name = "Product " + (i + 1),
                        
                    };
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
        public void Test_Get_All()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var session = NHibernateHelper.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    _products = session.Query<Product>()
                        .Where(p => Convert.ToInt32(p.Name.Substring(7)) > 0 && Convert.ToInt32(p.Name.Substring(7)) <= 2000)
                        .ToList();
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);

            Assert.AreEqual(2000, _products.Count);
            //Assert.AreEqual(1,_products[0].Id);
            Assert.AreEqual(1,Convert.ToInt32(_products[0].Name.Substring(7)));
            
        }

        [TestMethod]
        public void Test_Delete()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Query<Product>()
                        //.Where(p => Convert.ToInt32(p.Name.Substring(8)) > 2000 && Convert.ToInt32(p.Name.Substring(8)) < 10000)
                        .Delete();
                    //.Where(p => p.Id > 100 && p.Id < 200).ToList();
                    transaction.Commit();
                }
            }
            

            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);
        }

        [TestMethod]
        public void SetUp()
        {

        }

        [TestMethod]
        public void Test_Get()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IProductRepository repository = new ProductRepository();
            var fromDb = repository.Where(p =>
                Convert.ToInt32(p.Name.Substring(8)) > 0 && Convert.ToInt32(p.Name.Substring(8)) <= 2000);
            //var fromDb = repository.GetAll();

            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);

            Assert.IsNotNull(fromDb);
            Assert.AreEqual(2000,fromDb.Count);
        }

        [TestMethod]
        public void Test_Get_Async()
        {
            //SetUpContext();
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                int i1 = i;
                var task_i = new Task(() => Task_Get(i1));
                task_i.Start();
                tasks.Add(task_i);
            }

            Task.WaitAll(tasks.ToArray());


            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);
        }

        private void Task_Get(int i)
        {
            //var sessionFactory = BuildSessionFactory();
            IList<DemoNhibernateApp.Domain.Product> fromDb;
            using (var session = DemoNhibernateApp.Repositories.NHibernateHelper.OpenSession())
            {

                fromDb = session.Query<DemoNhibernateApp.Domain.Product>()
                    .Where(p =>
                        p.Id > i * 1000 &&
                        p.Id <= (i * 1000 + 1000))
                    .ToList();

                //fromDb = session.QueryOver<DemoNhibernateApp.Domain.Product>()
                //    .Where(p=> p.Id > i * 1000 && p.Id <= (i*1000 +1000))
                //    .List();

                Assert.IsNotNull(fromDb);
                if(fromDb.Count()!=0)
                    Assert.AreEqual(1000, fromDb.Count());
                //Console.WriteLine("Task " + (i + 1));
            }

        }

        [TestMethod]
        public void Test_Update()
        {
            IProductRepository repository = new ProductRepository();
            DemoNhibernateApp.Domain.Product product = repository.GetById(1);
            product.Discontinued = false;
            Stopwatch stopwatch = Stopwatch.StartNew();
            repository.Update(product, p=>p.Discontinued);

            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);

            DemoNhibernateApp.Domain.Product fromDb = repository.GetById(1);
            Assert.IsNotNull(fromDb);
            Assert.AreEqual(false, fromDb.Discontinued);
            Assert.AreEqual(1, fromDb.Id);
        }

        [TestMethod]
        public void Test_Where()
        {
            SetUpContext();
            Product product;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    product = session.Get<Product>((long)1);
                }
            }

            product.Discontinued = true;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Update(product,p => p.Discontinued);
            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);

        }

        [TestMethod]
        public void Test_Delete_Async()
        {
            //SetUpContext();
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                int i1 = i;
                var task_i = new Task(() => DoTask(i1));
                task_i.Start();
                tasks.Add(task_i);
            }
            Task.WaitAll(tasks.ToArray());

            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);
            
        }

        public void DoTask(int i)
        {

            using (var session = DemoNhibernateApp.Repositories.NHibernateHelper.OpenSession())
            {
                lock (session)
                {
                    using (var transaction = session.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        var fromDb = session.Query<DemoNhibernateApp.Domain.Product>()
                            .Where(p =>
                                Convert.ToInt32(p.Name.Substring(8)) > i * 10000 &&
                                Convert.ToInt32(p.Name.Substring(8)) <= (i * 10000 + 10000))
                            //.WithLock(LockMode.Force)
                            .Delete();
                        //.AsEnumerable();
                        //Assert.AreEqual(10000,fromDb.Count());
                        //foreach (DemoNhibernateApp.Domain.Product product in fromDb)
                        //{
                        //    session.Merge(product);
                        //    session.Flush();
                        //    session.Delete(product);
                        transaction.Commit();

                    }

                    //Console.WriteLine("Task " + (i + 1));
                }
            }
        }

        public void Refresh(ISession session)
        {
            if (session != null && session.IsOpen)
            {
                session.Connection.Open();
            }
        }

        [TestMethod]
        public void Test_Update_Async()
        {
            SetUpContext();
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10000; i++)
            {
                int i1 = i;
                var task_i = new Task(() => Task_Update(i1));
                task_i.Start();
                tasks.Add(task_i);
            }
            Task.WaitAll(tasks.ToArray());

            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);

        }

        public void Task_Update(int i)
        {

            //var sessionFactory = BuildSessionFactory();
            //IEnumerable<DemoNhibernateApp.Domain.Product> fromDb;

            using (var session = DemoNhibernateApp.Repositories.NHibernateHelper.OpenSession())
            {
                //lock (session)
                //{
                using (var transaction = session.BeginTransaction())
                {

                    //session.Query<DemoNhibernateApp.Domain.Product>()
                    //    .Where(p =>
                    //        Convert.ToInt32(p.Name.Substring(8)) > i * 10000 &&
                    //        Convert.ToInt32(p.Name.Substring(8)) <= (i * 10000 + 10000))
                    //    .Update(p => new DemoNhibernateApp.Domain.Product() {Discontinued = !p.Discontinued});

                    DemoNhibernateApp.Domain.Product product =
                        session
                        .Get<DemoNhibernateApp.Domain.Product>((long)(i + 1));
                    //.Query<DemoNhibernateApp.Domain.Product>().Where(p => p.Id == (long)(4000000 + i) + 1).ElementAt(0);
                    product.Discontinued = true;
                    session.Update(product);
                    transaction.Commit();


                    Console.WriteLine("Task " + (i + 1));
                }
                //}
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


            using (var session = _sessionFactory.OpenStatelessSession())
            using (var transaction = session.BeginTransaction())
            {
                session.CreateSQLQuery(sql).AddEntity(typeof(Product)).UniqueResult();
                transaction.Commit();
            }
        }


    }
}
