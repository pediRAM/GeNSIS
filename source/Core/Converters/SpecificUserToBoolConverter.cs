using GeNSIS.Core.Enums;

namespace GeNSIS.Core.Converters
{

    public class SpecificUserToBoolConverter : AValueConverter
    {
        public override object Convert(object pValue)
        {
            if (pValue == null)
                return false;

            return ((EServiceUserType)pValue == EServiceUserType.SpecificUser);
        }
    }
}
