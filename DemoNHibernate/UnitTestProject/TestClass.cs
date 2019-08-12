using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentNHibernateApp.Entities;
using FluentNHibernateApp.Helper;
using FluentNHibernateApp.Repositories;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace UnitTestProject
{
    /// <summary>
    ///     Summary description for FNH_Test_2
    /// </summary>
    [TestClass]
    public class Test_Clas
    {
        #region properties

        // Log manager ghi log
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region private methods

        /// <summary>
        ///     Tạo và trả về một list các đối tượng Blog có Author từ 50 triệu trở đi
        /// </summary>
        /// <param name="quantity"> số lượng obj của list </param>
        /// <returns></returns>
        private IList<Blog> Prepare_2(int quantity)
        {
            IList<Blog> blogs = new List<Blog>();
            Random random = new Random();
            long seed = random.Next(70000001, 200000000);
            for (var i = 0; i < quantity; i++)
            {
                var blog = new Blog
                {
                    Author = seed + i,
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

        /// <summary>
        ///     Tạo và trả về một list các đối tượng Blog
        /// </summary>
        /// <param name="quantity"> số lượng obj của list </param>
        /// <returns></returns>
        private IList<Blog> Prepare(int quantity, int index)
        {
            IList<Blog> blogs = new List<Blog>();
            Random random = new Random();
            for (var i = 0; i < quantity; i++)
            {
                var blog = new Blog
                {
                    Author = 5000000 + (index) * quantity + (i + 1),
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

        /// <summary>
        /// Tính giá trị thực thi trung bình của một Task (Get, Insert, Delete)
        /// </summary>
        /// <param name="totalGet">list các giá trị thực thi qua các lần của 1 task</param>
        /// <param name="avgGet">list lưu các giá trị trung bình</param>
        private void Average(ConcurrentBag<IList<TimeSpan>> totalGet, IList<TimeSpan> avgGet)
        {
            int times = 0;
            foreach (var item in totalGet)
            {
                times++;
                if (item.Count != 0)
                {
                    var avg = new TimeSpan(Convert.ToInt64(item.Average(t => t.Ticks)));
                    log.Info("Log - Info - Get Times - " + times + " : " + avg);
                    avgGet.Add(avg);
                }
            }
        }


        #endregion

        #region Test methods

        /// <summary>
        ///     Insert record
        /// </summary>
        [TestMethod]
        public void Test_Insert_Record()
        {
            var blogs = new List<Blog>();
            var quantity = 1;
            for (var i = 0; i < quantity; i++)
            {
                var blog = new Blog
                {
                    Author = i + 1,
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
            var stopwatch = Stopwatch.StartNew();
            repository.Insert<Blog>(blogs);
            //repository.Commit();
            stopwatch.Stop();
            log.Info("Log Info - Insert times: " + stopwatch.Elapsed);
            Console.WriteLine("Insert times: " + stopwatch.Elapsed);
            //repository.Close();
        }

        /// <summary>
        ///     Insert số lượng lớn record
        /// </summary>
        [TestMethod]
        public void Test_Create_Bulk_Record()
        {
            var blogs = new List<Blog>();
            var quantity = 5000000;
            int ind = 206500000;
            for (var i = 0; i < quantity; i++)
            {
                var blog = new Blog
                {
                    Author = (i+1) + (ind*1),
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
            var n = 7;
            for (var i = 0; i < n; i++) Test_Create_Bulk_Record();
        }

        /// <summary>
        ///     Tìm kiếm theo trường Author
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Author()
        {
            var quantity = 100;
            var nTask = 1000;
            var random = new Random();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            var viewapi = ViewHelper.GetViewApi();
            for (var i = 0; i < nTask; i++)
            {
                var j = random.Next(1, 50000000);
                var to = j + quantity;
                var stopwatch = Stopwatch.StartNew();
                var blogs = viewapi.Where<Blog>(blog => blog.Author >= j && blog.Author < to);
                stopwatch.Stop();
                log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                Assert.IsNotNull(blogs);
            }
            viewapi.Clear();
            viewapi.Close();
            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.4));
            log.Info("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        /// <summary>
        ///     Tìm kiếm theo trường Id
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Id()
        {
            var quantity = 100;
            var nTask = 100;
            var random = new Random();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            var viewapi = ViewHelper.GetViewApi();
            for (var i = 0; i < nTask; i++)
            {
                var j = random.Next(1, 50000000);
                var to = j + quantity;
                var stopwatch = Stopwatch.StartNew();
                var blogs = viewapi.Where<Blog>(blog => blog.Id >= j && blog.Id < to );
                stopwatch.Stop();
                log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                Assert.IsNotNull(blogs);
            }
            viewapi.Clear();
            viewapi.Close();
            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            log.Info("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        /// <summary>
        ///     Tìm kiếm theo trường Summary (string)
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Summary()
        {
            var quantity = 100;
            var nTask = 10;
            var random = new Random();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            var viewapi = ViewHelper.GetViewApi();
            for (var i = 0; i < nTask; i++)
            {
                var j = random.Next(1, 50000000);
                var valCheck = "Summary of Blog " + j;
                var stopwatch = Stopwatch.StartNew();
                var blogs = viewapi.Where<Blog>(blog => blog.Summary == valCheck);
                stopwatch.Stop();
                log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                Assert.IsNotNull(blogs);
            }
            viewapi.Clear();
            viewapi.Close();

            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            log.Info("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        [TestMethod]
        public void Test_Get_By_Summary_With_Index()
        {
            var quantity = 100;
            var nTask = 100;
            var random = new Random();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            var viewapi = ViewHelper.GetViewApi();
            for (var i = 0; i < nTask; i++)
            {
                var j = random.Next(1, 50000000);
                var valCheck = "Summary of Blog " + j;
                var from = j - 100;
                var to = j + 100;
                var stopwatch = Stopwatch.StartNew();
                //var blogs = viewapi.Where<Blog>(blog => blog.Author >= from && blog.Author <= to && blog.Summary == valCheck);
                var blogs = viewapi.Where<Blog>(blog => blog.Id >= from && blog.Id <= to && blog.Summary == valCheck);
                stopwatch.Stop();
                log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                Assert.IsNotNull(blogs);
            }
            viewapi.Clear();
            viewapi.Close();

            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            log.Info("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        /// <summary>
        ///     Tìm kiếm bất đồng bộ theo trường Author
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Author_Async()
        {
            var IsRun = true;
            var nTask = 100;
            var random = new Random();
            var quantity = 100;
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            IList<Task> tasks = new List<Task>();
            ConcurrentBag<IList<TimeSpan>> allGet = new ConcurrentBag<IList<TimeSpan>>();

            for (var i = 0; i < nTask; i++)
            {
                var j = i + 1;
                var task_i = new Task(() =>
                {
                    var count = 0;
                    IList<TimeSpan> get_time_i = new List<TimeSpan>();
                    allGet.Add(get_time_i);
                    var viewapi = ViewHelper.GetViewApi();
                    while (IsRun)
                    {
                        count++;
                        long from = random.Next(1, 50000000);
                        long to = from + quantity;
                        var stopwatch = Stopwatch.StartNew(); 
                        var blogs = viewapi.Where<Blog>(blog => blog.Author >= from && blog.Author < to);
                        stopwatch.Stop();
                        Assert.IsNotNull(blogs);
                        get_time_i.Add(stopwatch.Elapsed);
                        viewapi.Clear();

                    }
                    viewapi.Close();
                });
                task_i.Start();
                tasks.Add(task_i);
            }

            Task tempTask = new Task(() =>
            {
                while (true) { }
            });
            tempTask.Start();
            tempTask.Wait(300000);

            IsRun = false;
            Task.WaitAll(tasks.ToArray());
            int times = 0;
            foreach (var item in allGet)
            {
                times++;
                if (item.Count !=0)
                {
                    var avg = new TimeSpan(Convert.ToInt64(item.Average(t => t.Ticks)));
                    log.Info("Log Info - Get Times - " + times + " : " + avg);
                    totalGet.Add(avg);
                }

            }
            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            log.Info("Log Info - Average Get time - Total: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);

        }

        /// <summary>
        ///     Tìm kiếm bất đồng bộ theo trường Id
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Id_Async()
        {
            var IsRun = true;
            var nTask = 100;
            var random = new Random();
            var quantity = 100;
            IList<TimeSpan> totalGet = new List<TimeSpan>();    
            IList<Task> tasks = new List<Task>();
            ConcurrentBag<IList<TimeSpan>> allGet = new ConcurrentBag<IList<TimeSpan>>();

            for (var i = 0; i < nTask; i++)
            {
                var j = i + 1;
                var task_i = new Task(() =>
                {
                    var count = 0;
                    IList<TimeSpan> get_time_i = new List<TimeSpan>();
                    allGet.Add(get_time_i);
                    var viewapi = ViewHelper.GetViewApi();
                    while (IsRun)
                    {
                        count++;
                        long from = random.Next(1, 50000000);
                        long to = from + quantity;
                        var stopwatch = Stopwatch.StartNew();  
                        var blogs = viewapi.Where<Blog>(blog => blog.Id >= from && blog.Id < to);
                        stopwatch.Stop();
                        Assert.IsNotNull(blogs);
                        get_time_i.Add(stopwatch.Elapsed);
                        viewapi.Clear();
                    }
                    viewapi.Close();
                });
                task_i.Start();
                tasks.Add(task_i);
            }

            Task tempTask = new Task(() =>
            {
                while (true) { }
            });
            tempTask.Start();
            tempTask.Wait(300000);

            IsRun = false;
            Task.WaitAll(tasks.ToArray());

            int times = 0;
            foreach (var item in allGet)
            {
                times++;
                if (item.Count != 0)
                {
                    var avg = new TimeSpan(Convert.ToInt64(item.Average(t => t.Ticks)));
                    log.Info("Log Info - Get Times - " + times + " : " + avg);
                    totalGet.Add(avg);
                }
            }

            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            log.Info("Log Info - Average Get time - Total: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        /// <summary>
        ///     Tìm kiếm bất đồng bộ theo trường Summary
        /// </summary>
        [TestMethod]
        public void Test_Get_By_Summary_Async()
        {
            var IsRun = true;
            var nTask = 10;
            var random = new Random();
            var quantity = 100;
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            IList<Task> tasks = new List<Task>();
            ConcurrentBag<IList<TimeSpan>> allGet = new ConcurrentBag<IList<TimeSpan>>();

            for (var i = 0; i < nTask; i++)
            {
                var j = i + 1;
                var task_i = new Task(() =>
                {
                    var count = 0;
                    IList<TimeSpan> get_time_i = new List<TimeSpan>();
                    allGet.Add(get_time_i);
                    var viewapi = ViewHelper.GetViewApi();
                    while (IsRun)
                    {
                        count++;
                        var temp = random.Next(1, 50000000);
                        var valCheck = "Summary of Blog " + temp;
                        var stopwatch = Stopwatch.StartNew();  
                        var blogs = viewapi.Where<Blog>(blog => blog.Summary == valCheck, quantity);
                        stopwatch.Stop();
                        Assert.IsNotNull(blogs);
                        get_time_i.Add(stopwatch.Elapsed);
                    }
                    viewapi.Close();
                });
                task_i.Start();
                tasks.Add(task_i);
            }

            Task tempTask = new Task(() =>
            {
                while (true) { }
            });
            tempTask.Start();
            tempTask.Wait(10000);

            IsRun = false;
            Task.WaitAll(tasks.ToArray());

            int times = 0;
            foreach (var item in allGet)
            {
                times++;
                var avg = new TimeSpan(Convert.ToInt64(item.Average(t => t.Ticks)));
                log.Info("Log Info - Get Times - " + times + " : " + avg);
                totalGet.Add(avg);
            }

            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            log.Info("Log Info - Average Get time - Total: " + GetAvg);
            Console.WriteLine("Get Times: " + GetAvg);
        }

        /// <summary>
        ///     Tạo ngẫu nhiên và Insert record
        /// </summary>
        [TestMethod]
        public void Test_Insert()
        {
            var quantity = 50;
            var random = new Random();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            var sum = 0;
            var nTask = 1000;

            IRepository repository = FluentNHibernateHelper.GetRepositiory();
            for (var i = 0; i < nTask; i++)
            {
                var j = i + 1;
                sum += quantity;

                IList<Blog> blogs = Prepare(quantity,i);

                var stopwatch = Stopwatch.StartNew();
                repository.Insert(blogs);
                stopwatch.Stop();
                log.Info("Log Info - " + j + " : " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                //Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.1));
            }
            repository.Close();
            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            //Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Insert Times: " + GetAvg);
            Console.WriteLine("Insert Times: " + GetAvg);
            log.Info("Log Info - Sum of objects : " + sum);
        }

        /// <summary>
        ///     Tạo ngẫu nhiên record và Insert bất đồng bộ
        /// </summary>
        [TestMethod]
        public void Test_Insert_Async()
        {
            var quantity = 50;
            var nTask = 1000;
            var random = new Random();
            IList<Task> tasks = new List<Task>();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            var sum = 0;
            for (var i = 0; i < nTask; i++)
            {
                var j = i + 1;
                var task_i = new Task(() =>
                {
                    quantity = random.Next(20, 50);
                    sum += quantity;

                    #region create items

                    IList<Blog> blogs = new List<Blog>();
                    for (var k = 0; k < quantity; k++)
                    {
                        var blog = new Blog
                        {
                            Author = k + 1,
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
                    var stopwatch = Stopwatch.StartNew();
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
            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            //Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Insert Times: " + GetAvg);
            Console.WriteLine("Insert Times: " + GetAvg);
            log.Info("Log Info - Sum of objects : " + sum);
        }

        /// <summary>
        ///     Update record
        /// </summary>
        [TestMethod]
        public void Test_Update()
        {
            var quantity = 50;
            var nTask = 100;
            var random = new Random();

            IList<TimeSpan> totalGet = new List<TimeSpan>();
            var repository = FluentNHibernateHelper.GetRepositiory();
            for (var i = 0; i < nTask; i++)
            {
                var from = random.Next(1, 50000000);
                var to = from + quantity;
                var blogs = repository.Where<Blog>(b => b.Author >= from && b.Author < to);
                foreach (var blog in blogs) blog.IsActive = false;

                var stopwatch = Stopwatch.StartNew();
                foreach (var blog in blogs) repository.Update(blog);
                repository.Commit();
                stopwatch.Stop();
                log.Info("Log Info - Times: " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
                //Assert.IsTrue(stopwatch.Elapsed < TimeSpan.FromSeconds(0.1));
            }
            repository.Clear();
            repository.Close();
            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            //Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Updating Times: " + GetAvg);
            Console.WriteLine("Log Info - Updating Times: " + GetAvg);
        }

        /// <summary>
        ///     Update record bất đồng bộ
        /// </summary>
        [TestMethod]
        public void Test_Update_Async()
        {
            var quantity = 100;
            var nTask = 1000;
            var random = new Random();
            IList<Task> tasks = new List<Task>();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            for (var i = 0; i < nTask; i++)
            {
                var task_i = new Task(() =>
                {
                    long from = random.Next(1, 50000000);
                    var to = from + quantity;
                    var repository = FluentNHibernateHelper.GetRepositiory();
                    var blogs = repository.Where<Blog>(b => b.Id >= from && b.Id < to);
                    foreach (var blog in blogs) blog.IsActive = false;

                    var stopwatch = Stopwatch.StartNew();
                    foreach (var blog in blogs) repository.Update(blog);
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
            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Updating Times: " + GetAvg);
            Console.WriteLine("Updating Times: " + GetAvg);
        }

        /// <summary>
        ///     Delete record
        /// </summary>
        [TestMethod]
        public void Test_Delete()
        {
            int sum = 50099130;
            var quantity = 100;
            var nTask = 1;
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            var repository = FluentNHibernateHelper.GetRepositiory();
            for (var i = 0; i < nTask; i++)
            {
                var j = i;
                var from = sum - j * quantity;
                var to = sum - j * quantity - quantity;
                var stopwatch = Stopwatch.StartNew();
                repository.Delete<Blog>(b => b.Author <= sum && b.Author > 50000000);
                repository.Commit();
                stopwatch.Stop();
                log.Info("Log Info - Times: " + stopwatch.Elapsed);
                totalGet.Add(stopwatch.Elapsed);
            }
            repository.Close();
            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            log.Info("Log Info - Deleting Times: " + GetAvg);
            Console.WriteLine("Deleting Times: " + GetAvg);
        }

        /// <summary>
        ///     Delete record bất đồng bộ
        /// </summary>
        [TestMethod]
        public void Test_Delete_Async()
        {
            var quantity = 50;
            var nTask = 100;
            var random = new Random();
            IList<Task> tasks = new List<Task>();
            IList<TimeSpan> totalGet = new List<TimeSpan>();
            for (var i = 0; i < nTask; i++)
            {
                var task_i = new Task(() =>
                {
                    var j = random.Next(1, 5000000);
                    var repository = FluentNHibernateHelper.GetRepositiory();
                    var blogs = repository.Where<Blog>(b => b.Author >= j, quantity);

                    var stopwatch = Stopwatch.StartNew();
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
            var GetAvg = new TimeSpan(Convert.ToInt64(totalGet.Average(t => t.Ticks)));
            //Assert.IsTrue(GetAvg < TimeSpan.FromSeconds(0.1));
            log.Info("Log Info - Deleting Times: " + GetAvg);
            Console.WriteLine("Log Info - Deleting Times: " + GetAvg);
        }

        /// <summary>
        ///     Chạy các Task bất đồng bộ : Insert - Get - Delete
        /// </summary>
        [TestMethod]
        public void Test_Multiple_Task()
        {
            var sum = 50000000;
            var quantity = 50;
            var nTask = 60;
            var IsRun = true;
            Random random = new Random();
            ConcurrentBag<IList<TimeSpan>> totalInsert = new ConcurrentBag<IList<TimeSpan>>();
            ConcurrentBag<IList<TimeSpan>> totalGet = new ConcurrentBag<IList<TimeSpan>>();
            ConcurrentBag<IList<TimeSpan>> totalDelete = new ConcurrentBag<IList<TimeSpan>>();
            var tasks = new List<Task>();
            var temp_count_delete = -1;
            var start = 0;

            long nInserted = 0;

            for (var i = 0; i < nTask; i++)
            {
                var j = i + 1;
                Task task_i = null;

                // Insert
                if (j % 3 == 1)
                {
                    task_i = new Task(() =>
                    {
                        var count = 0;
                        IList<TimeSpan> insert_i = new List<TimeSpan>();
                        totalInsert.Add(insert_i);
                        var repository1 = FluentNHibernateHelper.GetRepositiory();
                        while (IsRun)
                        {
                            count++;
                            var blogs = Prepare_2(quantity);
                            var stopwatch = Stopwatch.StartNew();
                            try
                            {
                                repository1.Insert(blogs);
                            }
                            catch (Exception e)
                            {
                                // Lỗi trùng key khi do phát sinh ngẫu nhiên các object
                            }
                            
                            stopwatch.Stop();
                            insert_i.Add(stopwatch.Elapsed);
                            nInserted += quantity;
                            repository1.Clear();
                        }
                        repository1.Close();
                    });
                }
                // Delete
                else if (j % 3 == 2)
                {
                    
                    task_i = new Task(() =>
                    {
                        var count = 0;
                        IList<TimeSpan> delete_i = new List<TimeSpan>();
                        totalDelete.Add(delete_i);
                        var repository2 = FluentNHibernateHelper.GetRepositiory();
                        while (IsRun)
                        {
                            var from = random.Next(1, 50000000);
                            var to = from + quantity;

                            var stopwatch = Stopwatch.StartNew();
                            repository2.Delete<Blog>(p => p.Id >= from && p.Id < to);
                            repository2.Commit();
                            stopwatch.Stop();
                            nInserted -= quantity;
                            delete_i.Add(stopwatch.Elapsed);
                            repository2.Clear();
                        }
                        repository2.Close();
                    });
                }
                // Get
                else
                {
                    task_i = new Task(() =>
                    {
                        var count = 0;
                        IList<TimeSpan> get_i = new List<TimeSpan>();
                        totalGet.Add(get_i);
                        var viewapi = ViewHelper.GetViewApi();
                        while (IsRun)
                        {

                            long from = new Random().Next(1, 50000000);
                            var to = from + quantity;

                            var stopwatch = Stopwatch.StartNew();
                            var blogs = viewapi.Where<Blog>(p => p.Author < to && p.Author >= from);
                            stopwatch.Stop();
                            Assert.IsNotNull(blogs);
                            get_i.Add(stopwatch.Elapsed);
                            viewapi.Clear();
                        }
                        viewapi.Close();
                    });
                }

                task_i.Start();
                tasks.Add(task_i);
            }

            Task tempTask = new Task(() =>
            {
                while (true) { }
            });
            tempTask.Start();
            tempTask.Wait(60000);

            IsRun = false;
            Task.WaitAll(tasks.ToArray());

            #region 
            /*
            var rep = FluentNHibernateHelper.GetRepositiory();
            long frm = sum;
            long temp = 500;
            // Xoa phan tu du thua hoac chen phan tu thieu


            if (nInserted < 0)
            {
                long count = -nInserted;
                while (count != 0)
                {
                    IList<Blog> tempList = Prepare_2((int)temp);
                    try
                    {
                        rep.Insert(tempList);
                    }
                    catch
                    {
                        nInserted += temp;

                    }
                    count -= temp;
                }
            }
            else
            {
                while (nInserted != 0)
                {
                    var to = frm + nInserted;
                    rep.Delete<Blog>(p => p.Author > sum && p.Author <= to);
                    rep.Commit();
                    nInserted -= temp;
                    frm = to;
                }

            }
            rep.Clear();
            rep.Close();
            */
            #endregion


            IList<TimeSpan> avgGet = new List<TimeSpan>();
            IList<TimeSpan> avgInsert = new List<TimeSpan>();
            IList<TimeSpan> avgDelete = new List<TimeSpan>();

            Average(totalGet, avgGet);
            Average(totalInsert, avgInsert);
            Average(totalDelete,avgDelete);


            var InsertAvg = new TimeSpan(Convert.ToInt64(avgInsert.Average(t => t.Ticks)));
            var GetAvg = new TimeSpan(Convert.ToInt64(avgGet.Average(t => t.Ticks)));
            var DeleteAvg = new TimeSpan(Convert.ToInt64(avgDelete.Average(t => t.Ticks)));
            log.Info("Log Info - Insert Times: " + InsertAvg);
            log.Info("Log Info - Get Times: " + GetAvg);
            log.Info("Log Info - Delete Times: " + DeleteAvg);

            Console.WriteLine("Log Info - Insert Times: " + InsertAvg);
            Console.WriteLine("Log Info - Get Times: " + GetAvg);
            Console.WriteLine("Log Info - Delete Times: " + DeleteAvg);
        }

        /// <summary>
        ///     Delete record trùng nhau
        /// </summary>
        [TestMethod]
        public void Test_Delete_Same()
        {
            var quantity = 10;

            var repository1 = FluentNHibernateHelper.GetRepositiory();
            var blogs1 = repository1.Where<Blog>(b => b.Author == 2, quantity);


            var repository2 = FluentNHibernateHelper.GetRepositiory();
            var blogs2 = repository2.Where<Blog>(b => b.Author == 2, quantity);

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