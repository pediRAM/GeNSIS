namespace GeNSIS.Core.Helpers
{
    using GeNSIS.Core.Enums;
    using GeNSIS.Core.Models;
    using System;


    public class PDC
    {
        public const int LABEL_LEFT_STD = 10;
        public const int LABEL_LEFT_XTW = 5;

        public const int W_LABEL = 20;


        public const int INPUT_LEFT_STD = LABEL_LEFT_STD + W_LABEL;
        public const int INPUT_LEFT_XTW = LABEL_LEFT_XTW + W_LABEL;

        public const int INPUT_HEIGHT = 14;
        public const int INPUT_H_DIFF = 2;

        
        public const int W_CHECKBOX = 10;
        public const int W_NUMBER = 15;
        public const int W_TEXTBOX = 30;
        public const int W_IPBOX = 30;
        public const int W_BUTTON_BROWSE = 5;
        public const int W_PATH = 100 - (LABEL_LEFT_XTW + W_LABEL + W_BUTTON_BROWSE + 3);
        
        public const int H_INPUT = 12;
        public const int H_LABEL = 10;
        public const int L_BUTTON_BROWSE = INPUT_LEFT_XTW + W_PATH + 1;


        public PDC(int pCounter, bool pIsExtraWideMode)
        {
            Counter = pCounter;
            IsExtraWideMode = pIsExtraWideMode;
        }


        public int Counter { get; private set; }
        public bool IsExtraWideMode { get; private set; }

        public void Increment() => Counter++;

        public PosDim GetGroupBoxPosDim(int pTotalUIs)
        {
            int totalHeight = (pTotalUIs + 1) * 14 + 2;

            if (IsExtraWideMode)
                return new PosDim(0, 0, 100, totalHeight);
            else
                return new PosDim(5, 0, 90, totalHeight);
        }

        private int CalcLabelHeight() => Counter * INPUT_HEIGHT;
        private int CalcInputHeight() => CalcLabelHeight() - INPUT_H_DIFF;

        private PosDim GetLabelPosDim()
            => IsExtraWideMode ? new PosDim(LABEL_LEFT_XTW, CalcLabelHeight(), W_LABEL, H_LABEL) : new PosDim(LABEL_LEFT_STD, CalcLabelHeight(), W_LABEL, H_LABEL);

        private PosDim GetCheckBoxPosDim()
            => IsExtraWideMode ? new PosDim(INPUT_LEFT_XTW, CalcInputHeight(), W_CHECKBOX, H_INPUT) : new PosDim(INPUT_LEFT_STD, CalcInputHeight(), W_CHECKBOX, H_INPUT);

        private PosDim GetNumberBoxPosDim()
            => IsExtraWideMode ? new PosDim(INPUT_LEFT_XTW, CalcInputHeight(), W_NUMBER, H_INPUT) : new PosDim(INPUT_LEFT_STD, CalcInputHeight(), W_NUMBER, H_INPUT);

        private PosDim GetTextBoxPosDim()
            => IsExtraWideMode ? new PosDim(INPUT_LEFT_XTW, CalcInputHeight(), W_TEXTBOX, H_INPUT) : new PosDim(INPUT_LEFT_STD, CalcInputHeight(), W_TEXTBOX, H_INPUT);

        private PosDim GetIPBoxPosDim()
            => IsExtraWideMode ? new PosDim(INPUT_LEFT_XTW, CalcInputHeight(), W_IPBOX, H_INPUT) : new PosDim(INPUT_LEFT_STD, CalcInputHeight(), W_IPBOX, H_INPUT);

        private PosDim GetPathPosDim()
            => new PosDim(INPUT_LEFT_XTW, CalcInputHeight(), W_PATH, H_INPUT);

        private PosDim GetBrowseButtonPosDim()
            => new PosDim(L_BUTTON_BROWSE, CalcInputHeight(), W_BUTTON_BROWSE, H_INPUT);

        public PosDim GetPosDim(ESettingType pSettingType)
        {
            switch (pSettingType)
            {
                case ESettingType.Label:        return GetLabelPosDim();                    
                case ESettingType.ButtonBrowse: return GetBrowseButtonPosDim();

                case ESettingType.File:
                case ESettingType.Directory:    return GetPathPosDim();

                case ESettingType.Password:
                case ESettingType.String:       return GetTextBoxPosDim();

                case ESettingType.Integer:      return GetNumberBoxPosDim();
                case ESettingType.Boolean:      return GetCheckBoxPosDim();
                case ESettingType.IPAddress:    return GetIPBoxPosDim();
#if DEBUG
                default: throw new NotImplementedException();
#endif
            }

            return new PosDim(0, 0, 0, 0);
        }
    }
}
