namespace GeNSIS.Core.Models.Design
{
    using GeNSIS.Core.Interfaces.Design;
    using System;
    [System.Diagnostics.DebuggerDisplay("Installer:{Installer}, Uninstaller:{Uninstaller}, FontFamily:{FontFamily}, FontSize:{FontSize}")]
    public class PageDesign : ICloneable
    {


        public PageDesign() { }

        public PageDesign(IPageDesign pPageDesign) : this()
        => UpdateValues(pPageDesign);

        public PageImages Installer { get; set; }

        public PageImages Uninstaller { get; set; }

        public string FontFamily { get; set; }

        public int FontSize { get; set; }

        public object Clone()
        {
            return new PageDesign
            {
                Installer = Installer,
                Uninstaller = Uninstaller,
                FontFamily = FontFamily,
                FontSize = FontSize,
            };
        }

        public void UpdateValues(IPageDesign pPageDesign)
        {
            Installer = new PageImages(pPageDesign.Installer);
            Uninstaller = new PageImages(pPageDesign.Uninstaller);
            FontFamily = pPageDesign.FontFamily;
            FontSize = pPageDesign.FontSize;
        }

    }


}
