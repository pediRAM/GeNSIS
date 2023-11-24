namespace GeNSIS.Core.Interfaces.Design
{
    public interface IPageDesign
    {
        IPageImages Installer { get; set; }

        IPageImages Uninstaller { get; set; }

        string FontFamily { get; set; }

        int FontSize { get; set; }
    }
}
