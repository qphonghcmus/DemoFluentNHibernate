using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FluentNHibernateApp.Views
{
    public interface IViewAPI
    {
        // Get object T by Id
        T Get<T>(object id) where T : class;

        // Get All objects T
        IList<T> GetAll<T>() where T : class;

        // Get list of objects T by "condition" 
        IList<T> Where<T>(Expression<Func<T, bool>> condition) where T : class;

        // Get "quantity" objects T by condition
        IList<T> Where<T>(Expression<Func<T, bool>> condition, int quantity) where T : class;

        // clear session
        void Clear();

        // Disconnect and clean session
        void Close();

        
    }
}
