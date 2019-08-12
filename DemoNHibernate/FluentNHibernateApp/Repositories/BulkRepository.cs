using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernateApp.Helper;
using NHibernate;
using NHibernate.Metadata;
using NHibernate.Type;

namespace FluentNHibernateApp.Repositories
{
    public class BulkRepository : IRepository
    {
        #region properties

        private readonly IStatelessSession _statelessSession;
        private DataTable _table;
        private List<string> _columns;

        #endregion

        #region private methods

        // Constructor
        public BulkRepository(IStatelessSession ss)
        {
            _statelessSession = ss;
        }

        private List<string> CreateTable<T>(T obj) where T : class
        {
            List<string> ret = new List<string>();

            using (var session = FluentNHibernateHelper.GetSession())
            {
                var userMetaData = FluentNHibernateHelper.GetClassMetadata(typeof(T)) as
                    NHibernate.Persister.Entity.AbstractEntityPersister;
                string[] proppertiesOfTable_Name = userMetaData.PropertyNames;
                IType[] proppertiesOfTable_Type = userMetaData.PropertyTypes;

                var properties = obj.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    if (userMetaData.IdentifierPropertyName == prop.Name)
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

        private void AddRow<T>(IList<T> objs)
        {
            foreach (var obj in objs)
            {
                var row = _table.NewRow();
                var properties = obj.GetType().GetProperties();

                foreach (var prop in properties)
                {
                    if (_columns.Contains(prop.Name))
                    {
                        row[prop.Name] = prop.GetValue(obj);
                    }
                }

                _table.Rows.Add(row);
            }
        }

        #endregion

        #region implemented methods

        public bool InsertBulk<T>(IList<T> objs) where T : class
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _table = new DataTable();
            _columns = CreateTable(objs[0]);
            AddRow(objs);



            var userMetadata =
                FluentNHibernateHelper.GetClassMetadata(typeof(T)) as
                    NHibernate.Persister.Entity.AbstractEntityPersister;
            var cols = userMetadata.KeyColumnNames;
            var table = userMetadata.TableName;

            using (var bulkCopy = new SqlBulkCopy(_statelessSession.Connection as SqlConnection))
            {
                bulkCopy.DestinationTableName = table;
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

        #endregion

        #region Null Methods

        public void Clear()
        {
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public bool Delete<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(IList<T> obj) where T : class
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(Expression<Func<T, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(object id) where T : class
        {
            throw new NotImplementedException();
        }

        public IList<T> GetAll<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public bool Insert<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Update<T>(T obj) where T : class
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T obj, Expression<Func<T, object>> field) where T : class
        {
            throw new NotImplementedException();
        }

        public IList<T> Where<T>(Expression<Func<T, bool>> condition) where T : class
        {
            throw new NotImplementedException();
        }

        public IList<T> Where<T>(Expression<Func<T, bool>> condition, int quantity) where T : class
        {
            throw new NotImplementedException();
        }
        public void Insert<T>(IList<T> obj) where T : class
        {

        }
        
        #endregion
    }
}
