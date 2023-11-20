namespace GeNSIS.Core.Converters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;


    public class ItemsCountToVisibilityConverter : AValueConverter
    {
        public Visibility WhenNullOrZero { get; set; } = Visibility.Visible;
        public Visibility Else { get; set; } = Visibility.Collapsed;
        public override object Convert(object pValue)
        {
            if (pValue == null) return WhenNullOrZero ;

            if (pValue is IEnumerable<object> itmes)
                if (itmes.Any()) return Else;

            return WhenNullOrZero ;
        }
    }
}
