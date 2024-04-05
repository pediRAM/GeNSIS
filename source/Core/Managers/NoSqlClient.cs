

namespace GeNSIS.Core.Managers
{
    using LiteDB;
    using System;

    public class NoSqlClient<T> : IDisposable where T : new()
    {
        protected LiteDatabase m_DB = null;
        protected string m_CollectionName;
        private bool s_IsDisposed = false;


        public NoSqlClient(LiteDatabase pLiteDatabase, string pCollectionName)
        {
            m_DB = pLiteDatabase;
            m_CollectionName = pCollectionName;
        }

        protected ILiteCollection<T> Collection => m_DB.GetCollection<T>(m_CollectionName);

        public string CollectionName => m_CollectionName;

        public void Dispose()
        {
            if (s_IsDisposed) return;
            s_IsDisposed = true;
            m_DB.Dispose();
        }
        ~NoSqlClient() => Dispose();
    }

}

