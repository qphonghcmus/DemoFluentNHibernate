using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernateApp.Repositories;
using NHibernate;

namespace FluentNHibernateApp.Views
{
    public class ViewAPI : IViewAPI, IDisposable
    {

        #region properties

        private ISession session;

        #endregion

        #region private methods

        // Constructor
        public ViewAPI(ISession _session)
        {
            session = _session;
        }

        private void Refresh()
        {
            if (!session.IsOpen)
            {
                session.Reconnect();
            }
        }
        #endregion

        #region Implemented Methods

        public T Get<T>(object id) where T : class
        {
            Refresh();
            return session.Get<T>(id);
        }

        public IList<T> GetAll<T>() where T : class
        {
            Refresh();
            return session.Query<T>().ToList();
        }

        public IList<T> Where<T>(Expression<Func<T, bool>> condition) where T : class
        {
            Refresh();
            return session.Query<T>().Where(condition).ToList();
        }

        public IList<T> Where<T>(Expression<Func<T, bool>> condition, int quantity) where T : class
        {
            Refresh();
            return session.Query<T>().Where(condition).Take(quantity).ToList();
        }

        public void Clear()
        {
            Refresh();
            session.Clear();
        }

        public void Close()
        {
            session.Close();
            Dispose();
        }
        public void Dispose()
        {
            session?.Dispose();
        }

        #endregion


    }
}
