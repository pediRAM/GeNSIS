
namespace GeNSIS.Core.Serialization
{
    using GeNSIS.Core.Models;

    public interface IDeSerializer
    {
        string DisplayName { get; }
        string Extension { get; }

        Project ToProject(string pModelString);
        string ToString(Project project);

    }
}
