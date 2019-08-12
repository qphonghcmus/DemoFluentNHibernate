using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FluentNHibernateApp.Repositories
{
    public interface IRepository 
    {
        // Get object T by Id
        T Get<T>(object id) where T : class;

        // Get All objects T
        IList<T> GetAll<T>() where T : class;

        // Get list of objects T by "condition" 
        IList<T> Where<T>(Expression<Func<T, bool>> condition) where T : class;

        // Get "quantity" objects T by condition
        IList<T> Where<T>(Expression<Func<T, bool>> condition, int quantity) where T : class;

        // Insert obj
        bool Insert<T>(T obj) where T : class;

        // Insert list objects of T
        void Insert<T>(IList<T> obj) where T : class;
        
        // Insert bulk objects
        bool InsertBulk<T>(IList<T> objs) where T : class;

        // Delete obj
        bool Delete<T>(T obj) where T : class;

        // Delete list of objects T
        void Delete<T>(IList<T> obj) where T : class;

        // Delete obj - "condition"
        void Delete<T>(Expression<Func<T, bool>> condition);

        // Update obj
        bool Update<T>(T obj) where T : class;
        
        // Update only property "field" of obj
        void Update<T>(T obj, Expression<Func<T, object>> field) where T : class;


        // commit transaction
        void Commit();

        // commit transaction
        void Clear();

        // Disconnect and clean session
        void Close();
    }
}
