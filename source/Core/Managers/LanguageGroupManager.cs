using GeNSIS.Core.Models;
using LiteDB;

namespace GeNSIS.Core.Managers
{
    public class LanguageGroupManager : BaseManagerByName<LanguageGroup>
    {
        public LanguageGroupManager(LiteDatabase pLiteDatabase, string pCollectionName) : base(pLiteDatabase, pCollectionName)
        {
        }
    }
}
