using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DemoNhibernateApp.Domain;
using DemoNhibernateApp.Repositories;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace UnitTestProject
{
    /// <summary>
    /// Summary description for TClass_Test
    /// </summary>
    [TestClass]
    public class TClass_Test
    {

        private static readonly ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static class AssertLogger
        {
            public static void AssertWithLogs(Action assert)
            {
                try
                {
                    assert();
                }
                catch (Exception e)
                {
                    log.Error(e);
                    throw e;
                }
            }
        }

        [TestMethod]
        public void Test_Get_By_Id()
        {
            List<Task> tasks = new List<Task>();
            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
                //long i1 = (long)(i+1);
                long i1 = random.Next(1, 1000000);
                var task_i = new Task(() =>
                {
                    IRepositiory repositiory = NHibernateHelper.GetRepositiory();

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    Product product =  repositiory.Get<Product>(i1);
                    stopwatch.Stop();

                    Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.15));
                    Assert.IsNotNull(product);
                    Assert.AreEqual(i1, product.Id);
                    Assert.AreEqual(i1, Convert.ToInt32(product.Name.Substring(8)));
                    log.Info("Times: " + stopwatch.Elapsed);
                });
                task_i.Start();
                tasks.Add(task_i);
            }

            Task.WaitAll(tasks.ToArray());
        }

        [TestMethod]
        public void Test_Get_Where()
        {
            List<Task> tasks = new List<Task>();
            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
                long i1 = random.Next(1, 1000000);
                var task_i = new Task(() =>
                {
                    string condition = "Product " + i1;
                    IRepositiory repositiory = NHibernateHelper.GetRepositiory();

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    IList<Product> products = repositiory.Where<Product>(p => p.Name==condition);
                    stopwatch.Stop();
                    log.Info("Times: " + stopwatch.Elapsed);

                    Assert.IsNotNull(products);
                    Assert.AreEqual(condition, products[0].Name);
                    Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.6));
                });
                task_i.Start();
                tasks.Add(task_i);
            }

            Task.WaitAll(tasks.ToArray());
        }

        [TestMethod]
        public void Test_Insert()
        {

            IList<TimeSpan> totalTime = new List<TimeSpan>();
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 1000; i++)
            {
                int j = i + 1;
                long id = (long)(j);
                var task_i = new Task(() =>
                {
                    Product p = new Product() {Name = "Product " + id};
                    IRepositiory repositiory = NHibernateHelper.GetRepositiory();

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    bool fromDb = repositiory.Insert(p);
                    Assert.IsTrue(fromDb);
                    repositiory.Commit();
                    stopwatch.Stop();

                    log.Info("Times: " + stopwatch.Elapsed);
                    totalTime.Add(stopwatch.Elapsed);
                    Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.3));

                    Product tmp = repositiory.Get<Product>(p.Id);
                    Assert.IsNotNull(tmp);
                    Assert.AreEqual("Product " + id,tmp.Name);
                });
                task_i.Start();
                tasks.Add(task_i);
            }

            Task.WaitAll(tasks.ToArray());
            TimeSpan avg = new TimeSpan(Convert.ToInt64(totalTime.Average(t => t.Ticks)));
            log.Info("Avegare - Times: " + avg);
            Assert.IsTrue(avg < TimeSpan.FromSeconds(0.25));
        }

        [TestMethod]
        public void Test_Update()
        {
            List<Task> tasks = new List<Task>();

            int max = 1000;

            for (int i = 0; i < 100; i++)
            {
                int j = i + 1;
                Task task_i;

                if (j % 2 == 0)
                {
                    task_i = new Task(() =>
                    {
                        IRepositiory repositiory = NHibernateHelper.GetRepositiory();
                        long id = j;
                        IList<Product> products = repositiory.Where<Product>(p => p.Id >= (id - 1) * 1000 && p.Id <= id * 1000);

                        Stopwatch stopwatch = new Stopwatch();
                        foreach (var product in products)
                        {
                            product.Discontinued = true;
                            product.Category = "Beverages";

                            stopwatch.Start();
                            var fromDb = repositiory.Update(product);
                            Assert.IsTrue(fromDb);
                        }
                        repositiory.Commit();
                        stopwatch.Stop();
                        log.Info("Times: " + stopwatch.Elapsed);
                        Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.7));
                    });
                }
                else
                {
                    task_i = new Task(() =>
                    {
                        IRepositiory repositiory = NHibernateHelper.GetRepositiory();
                        long id = j;
                        IList<Product> products = repositiory.Where<Product>(p => p.Id >= (id-1)*1000 && p.Id <= id*1000);

                        Stopwatch stopwatch = new Stopwatch();
                        foreach (var product in products)
                        {
                            product.Category = "Clothes";
                            product.Discontinued = false;

                            stopwatch.Start();
                            var fromDb = repositiory.Update(product);
                            Assert.IsTrue(fromDb);
                        }
                        repositiory.Commit();
                        stopwatch.Stop();
                        log.Info("Times: " + stopwatch.Elapsed);
                        Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.7));
                    });
                }

                task_i.Start();
                tasks.Add(task_i);
            }

            Task.WaitAll(tasks.ToArray());
        }

        [TestMethod]
        public void Test_Update_Same()
        {
            List<Task> tasks = new List<Task>();
            Random random = new Random();
            long id = random.Next(1, 1000000);
            int max = 500;

            IList<TimeSpan> totalTime = new List<TimeSpan>();

            //id = 1;
            for (int i = 0; i < 100; i++)
            {
                int j = i + 1;
                Task task_i = new Task(() =>
                    {
                        IRepositiory repositiory = NHibernateHelper.GetRepositiory();
                        IList<Product> products = repositiory.Where<Product>(p => p.Id >= id && p.Id <= (id + max));

                        foreach (var product in products)
                        {
                            product.Discontinued = true;
                            product.Category = "Beverages";
                        }

                        Stopwatch stopwatch = Stopwatch.StartNew();
                        foreach (var product in products)
                        {
                            var fromDb = repositiory.Update(product);
                            //Assert.IsTrue(fromDb);
                        }
                        repositiory.Commit();
                        stopwatch.Stop();
                        log.Info("Times: " + stopwatch.Elapsed);
                        totalTime.Add(stopwatch.Elapsed);
                        //Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.5));
                        
                    });
                task_i.Start();
                tasks.Add(task_i);
            }

            Task.WaitAll(tasks.ToArray());

            TimeSpan avg = new TimeSpan(Convert.ToInt64(totalTime.Average(t => t.Ticks)));
            log.Info("Avegare - Times: " + avg);
            Assert.IsTrue( avg < TimeSpan.FromSeconds(0.25));
        }

        [TestMethod]
        public void Test_Update_1_Attribute()
        {
            Random random = new Random();
            List<Task> tasks = new List<Task>();
            long i1 = random.Next(1, 1000000);
            var checkVal = "Beve";

            for (int i = 0; i < 100; i++)
            {
                int j = i + 1;
                Task task_i = new Task(() =>
                {
                    IRepositiory repository = NHibernateHelper.GetRepositiory();

                    Product product = repository.Where<Product>(p => p.Id == i1)[0];
                    Assert.IsNotNull(product);
                    product.Category = checkVal;

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    repository.Update(product, p => p.Discontinued);
                    stopwatch.Stop();
                    log.Info("Times: " + stopwatch.Elapsed);
                    Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.15));

                    product = repository.Get<Product>(i1);
                    Assert.AreEqual(checkVal, product.Category);
                });
                task_i.Start();
                tasks.Add(task_i);
            }
            Task.WaitAll(tasks.ToArray());
        }

        [TestMethod]
        public void Test_Delete()
        {
            List<Task> tasks = new List<Task>();
            IList<TimeSpan> totalTime = new List<TimeSpan>();
            int max = 100;
            int sum = 1000000;

            for (int i = 0; i < 100; i++)
            {
                int j = i + 1;

                var task_i = new Task(() =>
                {

                    IRepositiory repositiory = NHibernateHelper.GetRepositiory();
                    long id = (long)(j - 1);
                    IList<Product> products = repositiory.Where<Product>(p => p.Id > sum, max);

                    //if (product == null) return;
                    //var fromDb = repositiory.Delete(product);

                    //Assert.IsTrue(fromDb);
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    foreach (var product in products)
                    {
                        repositiory.Delete(product);
                    }

                    repositiory.Commit();
                    stopwatch.Stop();
                    log.Info("Times: " + stopwatch.Elapsed);
                    totalTime.Add(stopwatch.Elapsed);
                    Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.2));

                });
                task_i.Start();
                tasks.Add(task_i);
            }

            Task.WaitAll(tasks.ToArray());

            TimeSpan avg = new TimeSpan(Convert.ToInt64(totalTime.Average(t => t.Ticks)));
            log.Info("Avegare - Times: " + avg);
            Assert.IsTrue(avg < TimeSpan.FromSeconds(0.2));
        }

        [TestMethod]
        public void Test_All()
        {
            int sum = 1022000;
            int quantity = 1000;
            IList<TimeSpan> totalInsert = new List<TimeSpan>();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            IList<TimeSpan> totalDelete = new List<TimeSpan>();

            List<Task> tasks = new List<Task>();
            var checks = new List<long>();
            var bags = new ConcurrentDictionary<long,Tuple<int,Product>>();
            for (int i = 0; i < 60; i++)
            {
                int j = i + 1;
                Task task_i = null;
                if (j % 2 == 0)
                {
                    task_i = new Task(() =>
                    {
                        IRepositiory repositiory_1 = NHibernateHelper.GetRepositiory();

                        IList<Product> products = Prepare(quantity);
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        repositiory_1.Insert(products);
                        repositiory_1.Commit();
                        stopwatch.Stop();
                        //log.Info("Times: " + stopwatch.Elapsed);
                        totalInsert.Add(stopwatch.Elapsed);
                        Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.2));
                    });
                }
                else
                {
                    task_i = new Task(() =>
                    {
                        IRepositiory repositiory_2 = NHibernateHelper.GetRepositiory();

                        Stopwatch stopwatch = Stopwatch.StartNew();
                        IList<Product> list = repositiory_2.Where<Product>(p => p.Id >= j * quantity && p.Id <(j * quantity + quantity) );//, quantity);
                        stopwatch.Stop();
                        totalGet.Add(stopwatch.Elapsed);

                        stopwatch.Start();
                        repositiory_2.Delete(list);
                        repositiory_2.Commit();
                        stopwatch.Stop();
                        totalDelete.Add(stopwatch.Elapsed);
                    });
                }
                task_i.Start();
                tasks.Add(task_i);
            }
            Task.WaitAll(tasks.ToArray());

            IRepositiory repositiory = NHibernateHelper.GetRepositiory();
            var fromDb = repositiory.GetAll<Product>();
            Assert.AreEqual(sum, fromDb.Count);

            TimeSpan InsertAvg = new TimeSpan(Convert.ToInt64(totalInsert.Average(t => t.Ticks)));
            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            TimeSpan DeleteAvg = new TimeSpan(Convert.ToInt64(totalDelete.Average(t => t.Ticks)));
            log.Info("Insert Times: " + InsertAvg);
            log.Info("Get Times: " + GetAvg);
            log.Info("Delete Times: " + DeleteAvg);

            Assert.IsTrue(InsertAvg < TimeSpan.FromSeconds(1));
            Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.5));
            Assert.IsTrue(DeleteAvg < TimeSpan.FromSeconds(0.5));

        }

        public IList<Product> Prepare(int quantity)
        {
            IList<Product> products = new List<Product>();
            for (int i = 0; i < quantity; i++)
            {
                products.Add(new Product() {Name = "Product_" + (i + 1)});
            }

            return products;
        }

        [TestMethod]
        public void Get_N()
        {
            int quantity = 1000;
            IRepositiory repositiory = NHibernateHelper.GetRepositiory();

            Stopwatch stopwatch = Stopwatch.StartNew();
            IList<Product> products = repositiory.Where<Product>(p => p.Id > 0, quantity);
            stopwatch.Stop();
            Console.WriteLine("Times: " + stopwatch.Elapsed);

            Assert.IsNotNull(products);
            for (int i = 0; i < quantity; i++)
            {
                Assert.AreEqual(i+1, products[i].Id);
            }
        }
    }
}
