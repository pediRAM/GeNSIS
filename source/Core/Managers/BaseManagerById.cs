
namespace GeNSIS.Core.Managers
{
    using GeNSIS.Core.Interfaces;
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;


    public abstract class BaseManagerById<T> : NoSqlClient<T> where T : IProvideIdProperty, new()
    {
        public BaseManagerById(LiteDatabase pLiteDatabase, string pCollectionName) : base(pLiteDatabase, pCollectionName) { }
        public List<T> Items => Collection.FindAll().ToList();
        public T this[int pId] => FindByIdOrNull(pId);
        public T FindOrNull(int pId) => Collection.FindOne(x => x.Id == pId);
        public bool Contains(int pId) => Items.Any(x => x.Id == pId);

        public int Add(T pItem) => Collection.Insert(pItem).AsInt32;
        public int AddRange(IEnumerable<T> pItems) => Collection.Insert(pItems);

        public bool Update(T pItem) => Collection.Update(pItem);

        public T FindByIdOrNull(int pId) => Collection.FindById(pId);

        public bool Remove(int pId) => Collection.Delete(pId);
        public int RemoveRange(IEnumerable<T> pItems) => Collection.DeleteMany(x => pItems.Select(x => x.Id).Contains(x.Id));

        
    }
}
