using System.ComponentModel.DataAnnotations;

namespace GeNSIS.Core.Serialization
{
    public enum EDeSerializers
    {
        [Display(Name ="json")]
        JSON,

        [Display(Name = "xml")]
        XML,
    }
}
