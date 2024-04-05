namespace GeNSIS.Core.Models.Design
{
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.Interfaces.Design;
    using LiteDB;
    using System;


    [System.Diagnostics.DebuggerDisplay("Installer:{Installer}, Uninstaller:{Uninstaller}, FontFamily:{FontFamily}, FontSize:{FontSize}")]
    public class PageDesign : IProvideNameProperty, ICloneable
    {

        public PageDesign() { }

        public PageDesign(IPageDesign pPageDesign) : this()
            => UpdateValues(pPageDesign);


        [BsonId]
        public string Name { get; set; }

        public PageImages Installer { get; set; }

        public PageImages Uninstaller { get; set; }

        public string FontFamily { get; set; }

        public int FontSize { get; set; }

        public void Compensate()
        {
            if (Uninstaller.Icon == null && Installer.Icon != null)
                Uninstaller.Icon = Installer.Icon;

            if (Uninstaller.HeaderImage == null && Installer.HeaderImage != null)
                Uninstaller.HeaderImage = Installer.HeaderImage;

            if (Uninstaller.WizardImage ==  null && Installer.WizardImage != null)
                Uninstaller.WizardImage = Installer.WizardImage;
        }

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
