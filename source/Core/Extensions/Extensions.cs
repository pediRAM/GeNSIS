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


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeNSIS.Core.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach(var item in pItems)
                pObservableCollection.Add(item);
        }

        public static void RemoveRange<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems)
                pObservableCollection.Remove(item);
        }

        public static void MoveFirst<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems.Reverse())
            {
                int oldIndex = pObservableCollection.IndexOf(item);
                pObservableCollection.Move(oldIndex, 0);
            }
        }

        public static void MovePrev<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems.Reverse())
            {
                int oldIndex = pObservableCollection.IndexOf(item);
                if (oldIndex < 1) 
                    continue;
                pObservableCollection.Move(oldIndex, oldIndex - 1);
            }
        }

        public static void MoveLast<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems)
            {
                int oldIndex = pObservableCollection.IndexOf(item);
                pObservableCollection.Move(oldIndex, pObservableCollection.Count - 1);
            }
        }

        public static void MoveNext<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems.Reverse())
            {
                int oldIndex = pObservableCollection.IndexOf(item);
                if (oldIndex >= pObservableCollection.Count - 1) 
                    continue;
                pObservableCollection.Move(oldIndex, oldIndex + 1);
            }
        }

        public static void Sort<T>(this ObservableCollection<T> pObservableCollection, IOrderedEnumerable<T> pOrderedEnumerable)
        {
            var list = pOrderedEnumerable.ToList();
            foreach (var item in list)
            {
                int oldIndex = pObservableCollection.IndexOf(item);
                int newIndex = list.IndexOf(item);
                if (oldIndex != newIndex)
                    pObservableCollection.Move(oldIndex, newIndex);
            }
        }
    }

    public static class IEnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> pEnumerable)
            => pEnumerable.Count() == 0;

        public static bool HasElement<T>(this IEnumerable<T> pEnumerable)
            => pEnumerable != null && pEnumerable.Count() > 0;
    }

    public static class ListBoxExtensions
    {
        public static List<T> GetSelectedItems<T>(this ListBox pListBox)
        {
            var l = new List<T>();
            foreach (var item in pListBox.SelectedItems)
                l.Add((T)item);
            return l;
        }
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var dispAttr = enumValue.GetType()
              .GetMember(enumValue.ToString())
              .First()
              .GetCustomAttribute(typeof(DisplayAttribute), false) as DisplayAttribute;

            if (dispAttr == null) return enumValue.ToString();
            else return dispAttr.Name;
        }
    }

    public static class DirectoryInfoExtensions
    {
        public static void CopyTo(this DirectoryInfo di, string destinationDir, bool recursive = true)
        {
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in di.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in di.GetDirectories())
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    subDir.CopyTo(newDestinationDir, true);
                }
            }
        }
    }

    public static class StringExtensions
    {
        /// <summary>
        /// Returns the string without given number of characters at the end.
        /// </summary>
        /// <param name="pNumberOfCharsToRemove">Number of characters to remove from the end of string.</param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string RemoveFromEnd(this string str, int pNumberOfCharsToRemove)
        {
            if (pNumberOfCharsToRemove < 0)
                throw new ArgumentException("Number of chars to remove from end cannot be less than zero!", nameof(pNumberOfCharsToRemove));

            if (string.IsNullOrEmpty(str))
                return string.Empty;

            if (str.Length < pNumberOfCharsToRemove)
                return "";

            return str.Substring(0, str.Length - pNumberOfCharsToRemove);
        }

        public static string ReplaceSymbols(this string str, char? replacement = ' ')
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsLetterOrDigit(c) || c == ' ')
                    sb.Append(c);
                else if (replacement != null)
                    sb.Append(replacement);
            }
            return sb.ToString();
        }

        public static string UpperCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            
            var sb = new StringBuilder();
            bool isFirst = true;

            foreach(char c in str.ReplaceSymbols(' ').ToLower())
            {
                if (c == ' ')
                {
                    isFirst = true;
                    continue;
                }

                if (isFirst)
                {
                    sb.Append(char.ToUpper(c));
                    isFirst = false;
                }
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static string CamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            string ucc = str.UpperCamelCase();
            return char.ToLower(ucc[0]) + ucc.Substring(1);
        }

        public static string ALL_UPPER_CASE(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            do
            {
                str = str.Replace("  ", " ");
            }
            while (str.Contains("  ")) ;

            return str.ToUpper().Replace(" ", "_");
        }
    }

    public static class BooleanExtensions
    {
        public static int To01(this bool value)
            => value ? 1 : 0;
    }

    public static class KeyEventArgsExtensions
    {
        /// <summary>
        /// Modified "Sledgehammer" code from: https://stackoverflow.com/a/46471739
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Alphanumeric chars or '_' else nil ('\x00')</returns>
        public static char ToAlphaNumericChar(this System.Windows.Input.KeyEventArgs e)
        {
            char nil = '\x00';
            Key key = e.Key;
        
            if (Keyboard.IsKeyDown(Key.LeftAlt) ||
                Keyboard.IsKeyDown(Key.RightAlt) ||
                Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightAlt))
            {
                return nil;
            }

            bool caplock = Console.CapsLock;
            bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool iscap = (caplock && !shift) || (!caplock && shift);

            switch (key)
            {
                case Key.Enter: return '\n';
                case Key.A: return (iscap ? 'A' : 'a');
                case Key.B: return (iscap ? 'B' : 'b');
                case Key.C: return (iscap ? 'C' : 'c');
                case Key.D: return (iscap ? 'D' : 'd');
                case Key.E: return (iscap ? 'E' : 'e');
                case Key.F: return (iscap ? 'F' : 'f');
                case Key.G: return (iscap ? 'G' : 'g');
                case Key.H: return (iscap ? 'H' : 'h');
                case Key.I: return (iscap ? 'I' : 'i');
                case Key.J: return (iscap ? 'J' : 'j');
                case Key.K: return (iscap ? 'K' : 'k');
                case Key.L: return (iscap ? 'L' : 'l');
                case Key.M: return (iscap ? 'M' : 'm');
                case Key.N: return (iscap ? 'N' : 'n');
                case Key.O: return (iscap ? 'O' : 'o');
                case Key.P: return (iscap ? 'P' : 'p');
                case Key.Q: return (iscap ? 'Q' : 'q');
                case Key.R: return (iscap ? 'R' : 'r');
                case Key.S: return (iscap ? 'S' : 's');
                case Key.T: return (iscap ? 'T' : 't');
                case Key.U: return (iscap ? 'U' : 'u');
                case Key.V: return (iscap ? 'V' : 'v');
                case Key.W: return (iscap ? 'W' : 'w');
                case Key.X: return (iscap ? 'X' : 'x');
                case Key.Y: return (iscap ? 'Y' : 'y');
                case Key.Z: return (iscap ? 'Z' : 'z');
                case Key.D0: return (shift ? nil : '0');
                case Key.D1: return (shift ? nil : '1');
                case Key.D2: return (shift ? nil : '2');
                case Key.D3: return (shift ? nil  : '3');
                case Key.D4: return (shift ? nil  : '4');
                case Key.D5: return (shift ? nil  : '5');
                case Key.D6: return (shift ? nil  : '6');
                case Key.D7: return (shift ? nil  : '7');
                case Key.D8: return (shift ? nil  : '8');
                case Key.D9: return (shift ? nil  : '9');
                /*
                case Key.OemPlus: return (shift ? nil  : '=');
                case Key.OemMinus: return (shift ? nil  : '-');
                case Key.OemQuestion: return (shift ? nil  : '/');
                case Key.OemComma: return (shift ? nil  : ',');
                case Key.OemPeriod: return (shift ? nil  : '.');
                case Key.OemOpenBrackets: return (shift ? nil  : '[');
                case Key.OemQuotes: return (shift ? nil  : '\'');
                case Key.Oem1: return (shift ? nil  : ';');
                case Key.Oem3: return (shift ? nil  : '`');
                case Key.Oem5: return (shift ? nil  : '\\');
                case Key.Oem6: return (shift ? nil  : ']');
                case Key.Tab: return '\t';
                case Key.Space: return ' ';
                */
                case Key.OemMinus: return (shift ? '_' : nil);
                // Number Pad
                case Key.NumPad0: return '0';
                case Key.NumPad1: return '1';
                case Key.NumPad2: return '2';
                case Key.NumPad3: return '3';
                case Key.NumPad4: return '4';
                case Key.NumPad5: return '5';
                case Key.NumPad6: return '6';
                case Key.NumPad7: return '7';
                case Key.NumPad8: return '8';
                case Key.NumPad9: return '9';
                //case Key.Subtract: return '-';
                //case Key.Add: return '+';
                //case Key.Decimal: return '.';
                //case Key.Divide: return '/';
                //case Key.Multiply: return '*';

                default: return '\x00';
            }
        }
    }
}
