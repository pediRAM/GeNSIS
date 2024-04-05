using GeNSIS.Core.Models.Design;
using LiteDB;

namespace GeNSIS.Core.Managers
{
    public class DesignManager : BaseManagerByName<PageDesign>
    {
        public DesignManager(LiteDatabase pLiteDatabase, string pCollectionName) : base(pLiteDatabase, pCollectionName)
        {
        }
    }
}
