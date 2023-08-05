/*
GeNSIS (GEnerates NullSoft Installer Script)
Copyright (C) 2023 Pedram GANJEH HADIDI

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using GeNSIS.Core.Models;
using System.Collections.Generic;

namespace GeNSIS.Core.Helpers
{
    internal static class LanguageHelper
    {
        private static readonly string[] s_MostSpokenLangNames = new string[] { "English", "French", "German", "Hindi", "Italian", "SimpChinese", "TradChinese", "Spanish", "Arabic", };

        public static string[] GetNamesOfMostSpockenLanguages() => s_MostSpokenLangNames;

        public static List<Language> GetLanguages()
        {
            return new List<Language>
            {
                new Language { Code="en", Name = "English",     DisplayName = "English",            Order = 0 },
                new Language { Code="cn", Name = "SimpChinese", DisplayName = "Simple Chinese",     Order = 1 },
                new Language { Code="cn", Name = "TradChinese", DisplayName = "Traditional Chinese",Order = 2 },
                new Language { Code="es", Name = "Spanish",     DisplayName = "Spanish",            Order = 3 },
                new Language { Code="es", Name = "SpanishInternational", DisplayName = "Spanish International", Order = 4 },
                new Language { Code="in", Name = "Hindi",       DisplayName = "Hindi",      Order = 5 },
                new Language { Code="fr", Name = "French",      DisplayName = "French",     Order = 6 },
                new Language { Code="sa", Name = "Arabic",      DisplayName = "Arabic",     Order = 7 },
                new Language { Code="ru", Name = "Russian",     DisplayName = "Russian",    Order = 8 },
                new Language { Code="pt", Name = "Portuguese",  DisplayName = "Portuguese", Order = 9 },
                new Language { Code="br", Name = "PortugueseBR", DisplayName = "Portuguese Brazil", Order = 10 },
                new Language { Code="de", Name = "German",      DisplayName = "German",     Order = 13 },
                new Language { Code="vn", Name = "Vietnamese",  DisplayName = "Vietnamese", Order = 17 },
                new Language { Code="tr", Name = "Turkish",     DisplayName = "Turkish",    Order = 18 },
                new Language { Code="id", Name = "Indonesian",  DisplayName = "Indonesian", Order = 10 },
                new Language { Code="jp", Name = "Japanese",    DisplayName = "Japanese",   Order = 12 },
                new Language { Code="kr", Name = "Korean",      DisplayName = "Korean",     Order = 19 },
                new Language { Code="it", Name = "Italian",     DisplayName = "Italian",    Order = 1012 },
                new Language { Code="nl", Name = "Dutch",       DisplayName = "Dutch",      Order = 1013 },
                new Language { Code="dk", Name = "Danish",      DisplayName = "Danish",     Order = 1014 },
                new Language { Code="se", Name = "Swedish",     DisplayName = "Swedish",    Order = 1015 },
                new Language { Code="no", Name = "Norwegian",   DisplayName = "Norwegian",  Order = 1077 },
                new Language { Code="no", Name = "NorwegianNynorsk", DisplayName = "Norwegian Nynorsk", Order = 1078 },
                new Language { Code="fi", Name = "Finnish",     DisplayName = "Finnish",    Order = 1060 },
                new Language { Code="gr", Name = "Greek",       DisplayName = "Greek",      Order = 1066 },
                new Language { Code="pl", Name = "Polish",      DisplayName = "Polish",     Order = 1020 },
                new Language { Code="ua", Name = "Ukrainian",   DisplayName = "Ukrainian",  Order = 1021 },
                new Language { Code="cz", Name = "Czech",       DisplayName = "Czech",      Order = 1022 },
                new Language { Code="sk", Name = "Slovak",      DisplayName = "Slovak",     Order = 1023 },
                new Language { Code="hr", Name = "Croatian",    DisplayName = "Croatian",   Order = 1024 },
                new Language { Code="bg", Name = "Bulgarian",   DisplayName = "Bulgarian",  Order = 1025 },
                new Language { Code="hu", Name = "Hungarian",   DisplayName = "Hungarian",  Order = 1026 },
                new Language { Code="th", Name = "Thai",        DisplayName = "Thai",       Order = 1027 },
                new Language { Code="ro", Name = "Romanian",    DisplayName = "Romanian",   Order = 1028 },
                new Language { Code="lv", Name = "Latvian",     DisplayName = "Latvian",    Order = 1029 },
                new Language { Code="mk", Name = "Macedonian",  DisplayName = "Macedonian", Order = 1030 },
                new Language { Code="ee", Name = "Estonian",    DisplayName = "Estonian",   Order = 1031 },
                new Language { Code="lt", Name = "Lithuanian",  DisplayName = "Lithuanian", Order = 1033 },
                new Language { Code="si", Name = "Slovenian",   DisplayName = "Slovenian",  Order = 1034 },
                new Language { Code="rs", Name = "Serbian",     DisplayName = "Serbian",    Order = 1035 },
                new Language { Code="rs", Name = "SerbianLatin", DisplayName = "Serbian Latin", Order = 1036 },
                new Language { Code="ir", Name = "Farsi",       DisplayName = "Farsi",      Order = 1038 },
                new Language { Code="il", Name = "Hebrew",      DisplayName = "Hebrew",     Order = 1039 },
                new Language { Code="mn", Name = "Mongolian",   DisplayName = "Mongolian",  Order = 1041 },
                new Language { Code="lu", Name = "Luxembourgish", DisplayName = "Luxembourgish", Order = 1042 },
                new Language { Code="al", Name = "Albanian",    DisplayName = "Albanian",   Order = 1043 },
                new Language { Code="fr", Name = "Breton",      DisplayName = "Breton",     Order = 1044 },
                new Language { Code="by", Name = "Belarusian",  DisplayName = "Belarusian", Order = 1045 },
                new Language { Code="is", Name = "Icelandic",   DisplayName = "Icelandic",  Order = 1046 },
                new Language { Code="my", Name = "Malay",       DisplayName = "Malay",      Order = 1047 },
                new Language { Code="ba", Name = "Bosnian",     DisplayName = "Bosnian",    Order = 1048 },
                new Language { Code="ku", Name = "Kurdish",     DisplayName = "Kurdish",    Order = 1049 },
                new Language { Code="ie", Name = "Irish",       DisplayName = "Irish",      Order = 1050 },
                new Language { Code="uz", Name = "Uzbek",       DisplayName = "Uzbek",      Order = 1051 },
                new Language { Code="es", Name = "Galician",    DisplayName = "Galician",   Order = 1052 },
                new Language { Code="za", Name = "Afrikaans",   DisplayName = "Afrikaans",  Order = 1053 },
                new Language { Code="es", Name = "Catalan",     DisplayName = "Catalan",    Order = 1054 },
                new Language { Code="es", Name = "Esperanto",   DisplayName = "Esperanto",  Order = 1055 },
                new Language { Code="es", Name = "Asturian",    DisplayName = "Asturian",   Order = 1056 },
                new Language { Code="sq", Name = "Basque",      DisplayName = "Basque",     Order = 1057 },
                new Language { Code="af", Name = "Pashto",      DisplayName = "Pashto",     Order = 1058 },
                new Language { Code="gb-sct", Name = "ScotsGaelic", DisplayName = "Scots Gaelic", Order = 1059 },
                new Language { Code="ge", Name = "Georgian",    DisplayName = "Georgian",   Order = 1068 },
                new Language { Code="gb-wls", Name = "Welsh",   DisplayName = "Welsh",      Order = 1062 },
                new Language { Code="am", Name = "Armenian",    DisplayName = "Armenian",   Order = 1069 },
                new Language { Code="cors", Name = "Corsican",  DisplayName = "Corsican",   Order = 1070 },
                new Language { Code="tatar", Name = "Tatar",    DisplayName = "Tatar",      Order = 1071 },

            };
        }
    }
}
