namespace GeNSIS.Core.Models.Design
{
    using GeNSIS.Core.Interfaces.Design;
    using System;


    [System.Diagnostics.DebuggerDisplay("Icon:{Icon}, HeaderImage:{HeaderImage}, WizardImage:{WizardImage}")]
    public class PageImages : IPageImages, ICloneable
    {


        public PageImages() { }

        public PageImages(IPageImages pPageImages) : this()
            => UpdateValues(pPageImages);

        public string Icon { get; set; }

        public string HeaderImage { get; set; }

        public string WizardImage { get; set; }

        public object Clone()
        {
            return new PageImages
            {
                Icon = Icon,
                HeaderImage = HeaderImage,
                WizardImage = WizardImage,
            };
        }

        public void UpdateValues(IPageImages pPageImages)
        {
            Icon = pPageImages.Icon;
            HeaderImage = pPageImages.HeaderImage;
            WizardImage = pPageImages.WizardImage;
        }

    }


}
