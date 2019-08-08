using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using FluentNHibernateApp.Entities;
using FluentNHibernateApp.Repositories;
using FluentNHibernateApp.Views;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Mapping;

namespace UnitTestProject
{
    /// <summary>
    /// Summary description for FNH_Test_2
    /// </summary>
    [TestClass]
    public class FNH_Test_2
    {
        #region properties

        // Log manager ghi log
        private static readonly ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        #endregion

        #region private methods

        /// <summary>
        /// Tạo và trả về một list các đối tượng Blog 
        /// </summary>
        /// <param name="quantity"> số lượng obj của list </param>
        /// <returns></returns>
        private IList<Blog> Prepare(int quantity)
        {
            IList<Blog> blogs = new List<Blog>();
            for (int i = 0; i < quantity; i++)
            {
                Blog blog = new Blog()
                {
                    Author = (i + 1),
                    Category = "Sport",
                    Content = "Content of Blog " + (i + 1),
                    Date_Publish = DateTime.Now,
                    Image = "URL of Image",
                    IsActive = true,
                    Views = 0,
                    Title = "Title of Blog " + (i + 1),
                    Summary = "Summary of Blog " + (i + 1)
                };
                blogs.Add(blog);
            }

            return blogs;
        }

        #endregion

        #region Test methods

        /// <summary>
        /// Insert record
        /// </summary>
        [TestMethod]
        public void Test_Insert_Record()
        {
            List<Blog> blogs = new List<Blog>();
            int quantity = 1;
            for (int i = 0; i < quantity; i++)
            {
                Blog blog = new Blog()
                {
                    Author = (i + 1),
                    Category = "Sport",
                    Content = "Content of Blog " + (i + 1),
                    Date_Publish = DateTime.Now,
                    Image = "URL of Image",
                    IsActive = true,
                    Views = 0,
                    Title = "Title of Blog " + (i + 1),
                    Summary = "Summary of Blog " + (i + 1)
                };
                blogs.Add(blog);
            }

            var repository = FluentNHibernateHelper.GetRepositiory();
            Stopwatch stopwatch = Stopwatch.StartNew();
            repository.Insert<Blog>(blogs);
            //repository.Commit();
            stopwatch.Stop();
            log.Info("Log Info - Insert times: " + stopwatch.Elapsed);
            Console.WriteLine("Insert times: " + stopwatch.Elapsed);
            //repository.Close();
        }

        /// <summary>
        /// Insert số lượng lớn record
        /// </summary>
        [TestMethod]
        public void Test_Create_Bulk_Record()
        {
            List<Blog> blogs = new List<Blog>();
            int quantity = 5000000;
            for (int i = 0; i < quantity; i++)
            {
                Blog blog = new Blog()
                {
                    Author = (i + 1),
                    Category = "Sport",
                    Content = "Content of Blog " + (i + 1),
                    Date_Publish = DateTime.Now,
                    Image = "URL of Image",
                    IsActive = true,
                    Views = 0,
                    Title = "Title of Blog " + (i + 1),
                    Summary = "Summary of Blog " + (i + 1)
                };
                blogs.Add(blog);
            }

            var repository = FluentNHibernateHelper.GetBulkRepository();
            repository.InsertBulk(blogs);
            //repository.Close();
        }

        [TestMethod]
        public void Insert_Bulk_Record()
        {
            int n = 7;
            for (int i = 0; i < n; i++)
            {
                Test_Create_Bulk_Record();
            }
        }

        /// <summary>
        /// Tìm kiếm theo trường Author
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Author()
        {
            int quantity = 100;
            int nTask = 1000;
            Random random = new Random();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            for (int i = 0; i < nTask; i++)
            {
                int j = random.Next(1, 50000000);
                var viewapi = ViewHelper.GetViewApi();
                Stopwatch stopwatch = Stopwatch.StartNew();
                IList<Blog> blogs = viewapi.Where<Blog>(blog => blog.Author >= j, quantity);
                stopwatch.Stop();
                log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                Assert.IsNotNull(blogs);
            }
            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.4));
            log.Info("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        /// <summary>
        /// Tìm kiếm theo trường Id
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Id()
        {
            int quantity = 100;
            int nTask = 100;
            Random random = new Random();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            for (int i = 0; i < nTask; i++)
            {
                int j = random.Next(1, 50000000);
                var viewapi = ViewHelper.GetViewApi();
                Stopwatch stopwatch = Stopwatch.StartNew();
                IList<Blog> blogs = viewapi.Where<Blog>(blog => blog.Id == j);
                stopwatch.Stop();
                log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                Assert.IsNotNull(blogs);
            }
            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.4));
            log.Info("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        /// <summary>
        /// Tìm kiếm theo trường Summary (string)
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Summary()
        {
            int quantity = 100;
            int nTask = 100;
            Random random = new Random();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            for (int i = 0; i < nTask; i++)
            {
                int j = random.Next(1, 50000000);
                var viewapi = ViewHelper.GetViewApi();
                var valCheck = "Summary of Blog " + j;
                Stopwatch stopwatch = Stopwatch.StartNew();
                IList<Blog> blogs = viewapi.Where<Blog>(blog => blog.Summary == valCheck, quantity);
                stopwatch.Stop();
                log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                Assert.IsNotNull(blogs);
            }
            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.4));
            log.Info("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        /// <summary>
        /// Tìm kiếm bất đồng bộ theo trường Author
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Author_Async()
        {
            bool IsRun = true;
            int nTask = 1000;
            Random random = new Random();
            int quantity = 100;
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            IList<Task> tasks = new List<Task>();
            for (int i = 0; i < nTask; i++)
            {

                Task task_i = new Task(() =>
                {
                    while (IsRun)
                    {
                    }
                    int j = random.Next(1, 5000000);
                    var viewapi = ViewHelper.GetViewApi();
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    IList<Blog> blogs = viewapi.Where<Blog>(blog => blog.Author == j, quantity);
                    stopwatch.Stop();
                    log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                    Assert.IsNotNull(blogs);
                    totalGet.Add(stopwatch.Elapsed);

                });
                task_i.Start();
                tasks.Add(task_i);
            }
            IsRun = false;
            Task.WaitAll(tasks.ToArray());
            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            log.Info("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        /// <summary>
        /// Tìm kiếm bất đồng bộ theo trường Id
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Id_Async()
        {
            bool IsRun = true;
            int nTask = 1000;
            Random random = new Random();
            int quantity = 100;
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            IList<Task> tasks = new List<Task>();
            for (int i = 0; i < nTask; i++)
            {
                Task task_i = new Task(() =>
                {
                    while (IsRun)
                    {
                    }

                    long j = random.Next(1, 50000000);
                    var viewapi = ViewHelper.GetViewApi();
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    IList<Blog> blogs = viewapi.Where<Blog>(blog => blog.Id >= j, quantity);
                    stopwatch.Stop();
                    log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                    Assert.IsNotNull(blogs);
                    totalGet.Add(stopwatch.Elapsed);
                    viewapi.Close();
                });
                task_i.Start();
                tasks.Add(task_i);
            }

            IsRun = false;
            Task.WaitAll(tasks.ToArray());

            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            log.Info("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        /// <summary>
        /// Tìm kiếm bất đồng bộ theo trường Summary
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Summary_Async()
        {
            bool IsRun = true;
            int nTask = 1000;
            Random random = new Random();
            int quantity = 100;
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            IList<Task> tasks = new List<Task>();
            for (int i = 0; i < nTask; i++)
            {
                Task task_i = new Task(() =>
                {
                    while (IsRun)
                    {

                    }
                    int j = random.Next(1, 50000000);
                    var valCheck = "Summary of Blog " + j;
                    var viewapi = ViewHelper.GetViewApi();
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    IList<Blog> blogs = viewapi.Where<Blog>(blog => blog.Summary == valCheck, quantity);
                    stopwatch.Stop();
                    log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                    Assert.IsNotNull(blogs);
                    totalGet.Add(stopwatch.Elapsed);

                });
                task_i.Start();
                tasks.Add(task_i);
            }

            IsRun = false;
            Task.WaitAll(tasks.ToArray());
            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            log.Info("Get Times: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }


        /// <summary>
        /// Tạo ngẫu nhiên và Insert record 
        /// </summary>
        [TestMethod]
        public void Test_Insert()
        {
            int quantity = 100;
            Random random = new Random();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            int sum = 0;
            int nTask = 100;
            for (int i = 0; i < nTask; i++)
            {
                int j = i + 1;
                quantity = random.Next(20, 50);
                sum += quantity;
                #region create items
                IList<Blog> blogs = new List<Blog>();

                for (int k = 0; k < quantity; k++)
                {
                    Blog blog = new Blog()
                    {
                        Author = (k + 1),
                        Category = "Sport",
                        Content = "Content of Blog " + (k + 1),
                        Date_Publish = DateTime.Now,
                        Image = "URL of Image",
                        IsActive = true,
                        Views = 0,
                        Title = "Title of Blog " + (k + 1),
                        Summary = "Summary of Blog " + (k + 1)
                    };
                    blogs.Add(blog);
                }

                #endregion

                var repository = FluentNHibernateHelper.GetRepositiory();
                Stopwatch stopwatch = Stopwatch.StartNew();
                repository.Insert(blogs);
                stopwatch.Stop();
                log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                //Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.1));
            }

            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Insert Times: " + GetAvg);
            Console.WriteLine("Insert Times: " + GetAvg);
            log.Info("Log Info - Sum of objects : " + sum);
        }

        /// <summary>
        /// Tạo ngẫu nhiên record và Insert bất đồng bộ
        /// </summary>
        [TestMethod]
        public void Test_Insert_Async()
        {

            int quantity = 50;
            int nTask = 1000;
            Random random = new Random();
            IList<Task> tasks = new List<Task>();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            int sum = 0;
            for (int i = 0; i < nTask; i++)
            {
                int j = i + 1;
                var task_i = new Task(() =>
                {
                    quantity = random.Next(20, 50);
                    sum += quantity;
                    #region create items
                    IList<Blog> blogs = new List<Blog>();
                    for (int k = 0; k < quantity; k++)
                    {
                        Blog blog = new Blog()
                        {
                            Author = (k + 1),
                            Category = "Sport",
                            Content = "Content of Blog " + (k + 1),
                            Date_Publish = DateTime.Now,
                            Image = "URL of Image",
                            IsActive = true,
                            Views = 0,
                            Title = "Title of Blog " + (k + 1),
                            Summary = "Summary of Blog " + (k + 1)
                        };
                        blogs.Add(blog);
                    }

                    #endregion

                    var repository = FluentNHibernateHelper.GetRepositiory();
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    repository.Insert(blogs);
                    stopwatch.Stop();
                    log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                    totalGet.Add(stopwatch.Elapsed);
                    //Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.1));
                });
                task_i.Start();
                tasks.Add(task_i);
            }
            Task.WaitAll(tasks.ToArray());
            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            //Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Insert Times: " + GetAvg);
            Console.WriteLine("Insert Times: " + GetAvg);
            log.Info("Log Info - Sum of objects : " + sum);
        }

        /// <summary>
        /// Update record
        /// </summary>
        [TestMethod]
        public void Test_Update()
        {

            int quantity = 50;
            int nTask = 1;
            Random random = new Random();

            IList<TimeSpan> totalGet = new List<TimeSpan>();
            for (int i = 0; i < nTask; i++)
            {
                int from = random.Next(1, 500000);
                var repository = FluentNHibernateHelper.GetRepositiory();
                IList<Blog> blogs = repository.Where<Blog>(b => b.Author >= 1 && b.Author <= 10);
                foreach (var blog in blogs)
                {
                    blog.IsActive = true;
                }

                Stopwatch stopwatch = Stopwatch.StartNew();
                foreach (var blog in blogs)
                {
                    repository.Update(blog);
                }
                repository.Commit();
                stopwatch.Stop();
                log.Info("Log Info - Times: " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                //Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.1));
            }

            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Updating Times: " + GetAvg);
        }

        /// <summary>
        /// Update record bất đồng bộ
        /// </summary>
        [TestMethod]
        public void Test_Update_Async()
        {

            int quantity = 100;
            int nTask = 1000;
            Random random = new Random();
            IList<Task> tasks = new List<Task>();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            for (int i = 0; i < nTask; i++)
            {
                Task task_i = new Task(() =>
                {
                    long from = random.Next(1, 50000000);
                    long to = from + quantity;
                    var repository = FluentNHibernateHelper.GetRepositiory();
                    IList<Blog> blogs = repository.Where<Blog>(b => b.Id >= from && b.Id < to);
                    foreach (var blog in blogs)
                    {
                        blog.IsActive = false;
                    }

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    foreach (var blog in blogs)
                    {
                        repository.Update(blog);
                    }
                    repository.Commit();
                    stopwatch.Stop();
                    log.Info("Log Info - Times: " + stopwatch.Elapsed);
                    totalGet.Add(stopwatch.Elapsed);
                    //Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.1));
                });

                task_i.Start();
                tasks.Add(task_i);
            }
            Task.WaitAll(tasks.ToArray());
            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Updating Times: " + GetAvg);
            Console.WriteLine("Updating Times: " + GetAvg);
        }

        /// <summary>
        /// Delete record
        /// </summary>
        [TestMethod]
        public void Test_Delete()
        {

            Random random = new Random();
            int quantity = 500;
            int nTask = 18;
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            for (int i = 0; i < nTask; i++)
            {
                //int j = random.Next(1, 5000000);
                int j = i;
                var from = 30000 - j* quantity;
                var to = 30000 - j * quantity - quantity;
                var repository = FluentNHibernateHelper.GetRepositiory();
                Stopwatch stopwatch = Stopwatch.StartNew();
                repository.Delete<Blog>(b => b.Id <= from && b.Id > to);
                repository.Commit();
                stopwatch.Stop();
                log.Info("Log Info - Times: " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                //Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.1));
            }

            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            //Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Deleting Times: " + GetAvg);
            Console.WriteLine("Deleting Times: " + GetAvg);
        }

        /// <summary>
        /// Delete record bất đồng bộ
        /// </summary>
        [TestMethod]
        public void Test_Delete_Async()
        {

            int quantity = 50;
            int nTask = 100;
            Random random = new Random();
            IList<Task> tasks = new List<Task>();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            for (int i = 0; i < nTask; i++)
            {
                Task task_i = new Task(() =>
                {
                    int j = random.Next(1, 5000000);
                    var repository = FluentNHibernateHelper.GetRepositiory();
                    IList<Blog> blogs = repository.Where<Blog>(b => b.Author >= j, quantity);

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    repository.Delete(blogs);
                    repository.Commit();
                    stopwatch.Stop();
                    log.Info("Log Info - Times: " + stopwatch.Elapsed);
                    totalGet.Add(stopwatch.Elapsed);
                    //Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.1));

                });
                task_i.Start();
                tasks.Add(task_i);
            }
            Task.WaitAll(tasks.ToArray());
            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            //Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Deleting Times: " + GetAvg);
            Console.WriteLine("Log Info - Deleting Times: " + GetAvg);
        }

        /// <summary>
        /// Chạy các Task bất đồng bộ : Insert - Get - Delete
        /// </summary>
        [TestMethod]
        public void Test_Multiple_Task()
        {
            int sum = 50000000;
            int quantity = 500;
            int nTask = 60;
            bool IsRun = true;
            IList<TimeSpan> totalInsert = new List<TimeSpan>();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            IList<TimeSpan> totalDelete = new List<TimeSpan>();
            List<Task> tasks = new List<Task>();
            int temp_count_delete = -1;
            int start = 30000;

            for (int i = 0; i < nTask; i++)
            {
                int j = i + 1;
                Task task_i = null;

                // Insert
                if (j % 3 == 0)
                {
                    task_i = new Task(() =>
                    {
                        while (IsRun) { }

                        var repository1 = FluentNHibernateHelper.GetRepositiory();
                        IList<Blog> blogs = Prepare(quantity);
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        repository1.Insert(blogs);
                        stopwatch.Stop();
                        totalInsert.Add(stopwatch.Elapsed);
                    });
                }
                // Delete
                else if (j % 3 == 1)
                {
                    var tmp = temp_count_delete + 1;
                    temp_count_delete++;
                    task_i = new Task(() =>
                    {
                        while (IsRun) { }

                        var repository2 = FluentNHibernateHelper.GetRepositiory();
                        int from = start + tmp * quantity;
                        int to = from + quantity;

                        Stopwatch stopwatch = Stopwatch.StartNew();
                        repository2.Delete<Blog>(p => p.Id > from && p.Id <= to);
                        repository2.Commit();
                        stopwatch.Stop();

                        totalDelete.Add(stopwatch.Elapsed);
                    });
                }
                // Get
                else
                {
                    task_i = new Task(() =>
                    {
                        while (IsRun) { }

                        var viewapi = ViewHelper.GetViewApi();
                        long from = new Random().Next(1, 50000000);
                        long to = from + quantity;

                        Stopwatch stopwatch = Stopwatch.StartNew();
                        IList<Blog> blogs = viewapi.Where<Blog>(p => p.Author < to && p.Author >= from);
                        stopwatch.Stop();

                        totalGet.Add(stopwatch.Elapsed);
                    });
                }
                task_i.Start();
                tasks.Add(task_i);
            }

            IsRun = false;
            Task.WaitAll(tasks.ToArray());

            TimeSpan InsertAvg = new TimeSpan(Convert.ToInt64(totalInsert.Average(t => t.Ticks)));
            TimeSpan GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            TimeSpan DeleteAvg = new TimeSpan(Convert.ToInt64(totalDelete.Average(t => t.Ticks)));
            log.Info("Log Info - Insert Times: " + InsertAvg);
            log.Info("Log Info - Get Times: " + GetAvg);
            log.Info("Log Info - Delete Times: " + DeleteAvg);

            //Assert.IsTrue(InsertAvg < TimeSpan.FromSeconds(1));
            //Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(1));
            //Assert.IsTrue(DeleteAvg < TimeSpan.FromSeconds(1));
            Console.WriteLine("Log Info - Insert Times: " + InsertAvg);
            Console.WriteLine("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Log Info - Delete Times: " + DeleteAvg);
        }

        /// <summary>
        /// Delete record trùng nhau
        /// </summary>
        [TestMethod]
        public void Test_Delete_Same()
        {
            int quantity = 10;

            var repository1 = FluentNHibernateHelper.GetRepositiory();
            IList<Blog> blogs1 = repository1.Where<Blog>(b => b.Author == 2, quantity);


            var repository2 = FluentNHibernateHelper.GetRepositiory();
            IList<Blog> blogs2 = repository2.Where<Blog>(b => b.Author == 2, quantity);

            repository2.Delete(blogs2);
            repository2.Commit();

            repository1.Delete(blogs1);
            repository1.Commit();

            repository1.Close();
            repository2.Close();

        }

        #endregion
    }
}
