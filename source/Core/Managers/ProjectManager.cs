namespace GeNSIS.Core.Managers
{
    using GeNSIS.Core.Models;
    using LiteDB;

    internal class ProjectManager : BaseManagerByName<Project>
    {
        public ProjectManager(LiteDatabase pLiteDatabase, string pCollectionName) : base(pLiteDatabase, pCollectionName)
        {
        }

        public Project Current { get; private set; }
        public Project Open(string pName)
        {
            var p = FindByNameOrNull(pName);
            if (p != null)
                Current = p;
            return p;
        }

        public Project CreateNewProject()
        {
            var p = new Project();
            Current = p;
            return p;
        }
    }
}
