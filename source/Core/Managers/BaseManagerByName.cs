
namespace GeNSIS.Core.Managers
{
    using GeNSIS.Core.Interfaces;
    using LiteDB;
    using System.Collections.Generic;
    using System.Linq;


    public abstract class BaseManagerByName<T> : NoSqlClient<T> where T : IProvideNameProperty, new()
    {
        public BaseManagerByName(LiteDatabase pLiteDatabase, string pCollectionName) : base(pLiteDatabase, pCollectionName) { }

        public List<T> Items => Collection.FindAll().ToList();
        public T this[string pName] => FindByNameOrNull(pName);
        public T FindOrNull(string pName) => Collection.FindOne(x => x.Name == pName);
        public bool Contains(string pName) => Items.Any(x => x.Name == pName);

        public string Add(T pItem) => Collection.Insert(pItem).RawValue as string;
        public int AddRange(IEnumerable<T> pItems) => Collection.Insert(pItems);

        public bool Update(T pItem) => Collection.Update(pItem);

        public T FindByNameOrNull(string pName) => Collection.FindById(pName);

        public bool Remove(string pName) => Collection.Delete(pName);
        public int RemoveRange(IEnumerable<T> pItems) => Collection.DeleteMany(x => pItems.Select(x => x.Name).Contains(x.Name));
    }
}
