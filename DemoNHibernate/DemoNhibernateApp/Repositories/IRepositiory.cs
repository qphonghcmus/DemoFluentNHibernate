using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DemoNhibernateApp.Repositories
{
    public interface IRepositiory
    {
        T Get<T>(object id) where T:class;
        IList<T> GetAll<T>() where T : class;
        IList<T> Where<T>(Expression<Func<T, bool>> condition) where T :class;
        IList<T> Where<T>(Expression<Func<T, bool>> condition,int quantity) where T :class;
        bool Insert<T>(T obj) where T : class;
        void Insert<T>(IList<T> obj) where T : class;
        bool Delete<T>(T obj) where T : class;
        void Delete<T>(IList<T> obj) where T : class;
        bool Update<T>(T obj) where T : class;
        void Update<T>(T obj, Expression<Func<T, object>> field) where T : class;
        void Commit();
        void Close();
    }
}
