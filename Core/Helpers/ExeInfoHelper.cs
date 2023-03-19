/***************************************************************************************
* GeNSIS - a free and open source NSIS installer script generator tool.                *
* Copyright (C) 2023 Pedram Ganjeh Hadidi                                              *
*                                                                                      *
* This file is part of GeNSIS.                                                         *
*                                                                                      *
* GeNSIS is free software: you can redistribute it and/or modify it under the terms    *
* of the GNU General Public License as published by the Free Software Foundation,      *
* either version 3 of the License, or any later version.                               *
*                                                                                      *
* GeNSIS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;  *
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR     *
* PURPOSE. See the GNU General Public License for more details.                        *
*                                                                                      *
* You should have received a copy of the GNU General Public License along with GeNSIS. *
* If not, see <https://www.gnu.org/licenses/>.                                         *
****************************************************************************************/


using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GeNSIS.Core.Helpers
{
    internal static class ExeInfoHelper
    {
        internal static void AutoNameInstallerExe(AppDataViewModel pAppData)
        {
            if (string.IsNullOrWhiteSpace(pAppData.AppName)) return;

            string build = string.IsNullOrWhiteSpace(pAppData.AppBuild) ? null : $"_{pAppData.AppBuild.Trim()}";
            string arch = pAppData.Is64BitApplication ? "x64" : "x32";
            pAppData.InstallerFileName = $"Setup_{pAppData.AppName}_{pAppData.AppVersion}{build}_{arch}.exe";
        }
        internal static void AutoSetProperties(AppDataViewModel pAppData)
        {
            var info = GetExeData(pAppData.ExeName);
            pAppData.Is64BitApplication = info.IsX64;
            pAppData.AppName = Path.GetFileNameWithoutExtension(pAppData.ExeName);

            pAppData.AppBuild = GetMachineTypeShortString(GetMachineTypeOfExe(pAppData.ExeName));

            if (info.Version.EndsWith(".0.0"))
                pAppData.AppVersion = info.Version.Substring(0, info.Version.Length - 4);
            else if (info.Version.EndsWith(".0"))
                pAppData.AppVersion = info.Version.Substring(0, info.Version.Length - 2);
            else
                pAppData.AppVersion = info.Version;

            var fvi = FileVersionInfo.GetVersionInfo(pAppData.ExeName);
            if (!string.IsNullOrWhiteSpace(fvi.CompanyName))
            {
                var fullCompany = string.Join("-", fvi.CompanyName.Replace(" ", "-").Split(Path.GetInvalidFileNameChars()));
                if (fullCompany.Length >= 20 && fullCompany.Count(x => x == '-') > 1)
                    pAppData.Company = new string(fullCompany.Split('-', StringSplitOptions.RemoveEmptyEntries).Where(s => !string.IsNullOrEmpty(s)).Select(s => s[0]).ToArray());
                else
                    pAppData.Company = fullCompany;

                pAppData.Publisher = fvi.CompanyName;
            }
        }

        /// <summary>
        /// From: http://zuga.net/articles/cs-how-to-determine-if-a-program-process-or-file-is-32-bit-or-64-bit/#getpekind
        /// </summary>
        internal enum BinaryType : uint
        {
            /// <summary>
            /// A 32-bit Windows-based application.
            /// </summary>
            SCS_32BIT_BINARY = 0,

            /// <summary>
            /// A 64-bit Windows-based application.
            /// </summary>
            SCS_64BIT_BINARY = 6,

            /// <summary>
            /// An MS-DOS © based application.
            /// </summary>
            SCS_DOS_BINARY = 1,

            /// <summary>
            /// A 16-bit OS/2-based application.
            /// </summary>
            SCS_OS216_BINARY = 5,

            /// <summary>
            /// A PIF file that executes an MS-DOS © based application
            /// </summary>
            SCS_PIF_BINARY = 3,

            /// <summary>
            /// A POSIX © based application.
            /// </summary>
            SCS_POSIX_BINARY = 4,

            /// <summary>
            /// A 16-bit Windows-based application.
            /// </summary>
            SCS_WOW_BINARY = 2
        }

        [DllImport("kernel32.dll")]
        static extern bool GetBinaryType(string lpApplicationName, out BinaryType lpBinaryType);

        public static (bool IsManaged, bool IsX64, string Version) GetExeData(string pExePath)
        {
            string fileVersion = null;
            try
            {
                fileVersion = FileVersionInfo.GetVersionInfo(pExePath).FileVersion;
                var asm = Assembly.LoadFrom(pExePath);
                //<-- Exe is managed (no exception).
                if (GetBinaryType(pExePath, out BinaryType bt) && (bt == BinaryType.SCS_64BIT_BINARY))
                    return (true, true, fileVersion);

                return (true, false, fileVersion);
            }
            catch { }

            try
            {
                if(IsUnmanagedExe64Bit(pExePath) == true)
                    return (false, true, fileVersion);
                else
                    return (false, false, fileVersion);
            }
            catch { }
            return (false, false, null);
        }

        #region Unmanaged Executalbe
        // From: https://stackoverflow.com/questions/1001404/check-if-unmanaged-dll-is-32-bit-or-64-bit

        /// <summary>
        /// Returns TRUE if the exe is 64-bit, FALSE if 32-bit, and NULL if unknown.
        /// </summary>
        /// <param name="pExePath">Path to *.exe file.</param>
        /// <returns>TRUE: x64, FALSE: x86, NULL: unknown.</returns>
        private static bool? IsUnmanagedExe64Bit(string pExePath)
        {
            switch (GetMachineTypeOfExe(pExePath))
            {
                case MachineType.IMAGE_FILE_MACHINE_AMD64:
                case MachineType.IMAGE_FILE_MACHINE_IA64:
                    return true;
                case MachineType.IMAGE_FILE_MACHINE_I386:
                    return false;
                default:
                    return null;
            }
        }

        internal static string GetMachineTypeShortString(MachineType pMachineType)
        {
            switch(pMachineType)
            {
                case MachineType.IMAGE_FILE_MACHINE_AM33: return "AM33";
                case MachineType.IMAGE_FILE_MACHINE_AMD64: return "AMD64";
                case MachineType.IMAGE_FILE_MACHINE_ARM: return "ARM";
                case MachineType.IMAGE_FILE_MACHINE_EBC: return "EBC";
                case MachineType.IMAGE_FILE_MACHINE_I386: return "I386";
                case MachineType.IMAGE_FILE_MACHINE_IA64: return "IA64";
                case MachineType.IMAGE_FILE_MACHINE_M32R: return "M32R";
                case MachineType.IMAGE_FILE_MACHINE_MIPS16: return "MIPS16";
                case MachineType.IMAGE_FILE_MACHINE_MIPSFPU: return "MIPSFPU";
                case MachineType.IMAGE_FILE_MACHINE_MIPSFPU16: return "MIPSFPU16";
                case MachineType.IMAGE_FILE_MACHINE_POWERPC: return "POWERPC";
                case MachineType.IMAGE_FILE_MACHINE_POWERPCFP: return "POWERPCFP";
                case MachineType.IMAGE_FILE_MACHINE_R4000: return "R4000";
                case MachineType.IMAGE_FILE_MACHINE_SH3: return "SH3";
                case MachineType.IMAGE_FILE_MACHINE_SH3DSP: return "SH3DSP";
                case MachineType.IMAGE_FILE_MACHINE_SH4: return "SH4";
                case MachineType.IMAGE_FILE_MACHINE_SH5: return "SH5";
                case MachineType.IMAGE_FILE_MACHINE_THUMB: return "THUMB";
                case MachineType.IMAGE_FILE_MACHINE_WCEMIPSV2: return "WCEMIPSV2";
                case MachineType.IMAGE_FILE_MACHINE_ARM64: return "ARM64";
            }
            return string.Empty;
        }

        internal enum MachineType : ushort
        {
            IMAGE_FILE_MACHINE_UNKNOWN = 0x0,
            IMAGE_FILE_MACHINE_AM33 = 0x1d3,
            IMAGE_FILE_MACHINE_AMD64 = 0x8664,
            IMAGE_FILE_MACHINE_ARM = 0x1c0,
            IMAGE_FILE_MACHINE_EBC = 0xebc,
            IMAGE_FILE_MACHINE_I386 = 0x14c,
            IMAGE_FILE_MACHINE_IA64 = 0x200,
            IMAGE_FILE_MACHINE_M32R = 0x9041,
            IMAGE_FILE_MACHINE_MIPS16 = 0x266,
            IMAGE_FILE_MACHINE_MIPSFPU = 0x366,
            IMAGE_FILE_MACHINE_MIPSFPU16 = 0x466,
            IMAGE_FILE_MACHINE_POWERPC = 0x1f0,
            IMAGE_FILE_MACHINE_POWERPCFP = 0x1f1,
            IMAGE_FILE_MACHINE_R4000 = 0x166,
            IMAGE_FILE_MACHINE_SH3 = 0x1a2,
            IMAGE_FILE_MACHINE_SH3DSP = 0x1a3,
            IMAGE_FILE_MACHINE_SH4 = 0x1a6,
            IMAGE_FILE_MACHINE_SH5 = 0x1a8,
            IMAGE_FILE_MACHINE_THUMB = 0x1c2,
            IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x169,
            IMAGE_FILE_MACHINE_ARM64 = 0xaa64
        }

        private static MachineType GetMachineTypeOfExe(string pExePath)
        {
            // See http://www.microsoft.com/whdc/system/platform/firmware/PECOFF.mspx
            // Offset to PE header is always at 0x3C.
            // The PE header starts with "PE\0\0" =  0x50 0x45 0x00 0x00,
            // followed by a 2-byte machine type field (see the document above for the enum).
            //
            using (var fs = new FileStream(pExePath, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                fs.Seek(0x3c, SeekOrigin.Begin);
                int peOffset = br.ReadInt32();

                fs.Seek(peOffset, SeekOrigin.Begin);
                uint peHead = br.ReadUInt32();

                if (peHead != 0x00004550) // "PE\0\0", little-endian
                    throw new Exception("Can't find PE header");

                return (MachineType)br.ReadUInt16();
            }
        }
        #endregion
    }
}
