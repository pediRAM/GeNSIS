
:: -----------------------------------------------------------------------------------
:: This program is free software: you can redistribute it and/or modify
:: it under the terms of the GNU General Public License as published by
:: the Free Software Foundation, either version 3 of the License, or
:: (at your option) any later version.
::
:: This program is distributed in the hope that it will be useful,
:: but WITHOUT ANY WARRANTY; without even the implied warranty of
:: MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
:: GNU General Public License for more details.
::
:: You should have received a copy of the GNU General Public License
:: along with this program.  If not, see <https://www.gnu.org/licenses/>.
:: 
:: Copyright (C) by Pedram GANJEH-HADIDI
:: Author: Pedram GANJEH HADIDI
:: Date: 2023-10-13
:: Script Description: installs a service.
:: Project Website: https://github.com/pediRAM/GeNSIS
:: -----------------------------------------------------------------------------------

@ECHO OFF

SET ServiceName=%1
SET ServiceUserType=%2
SET ServiceExePath=%3
SET ServiceUserName=%4
SET ServicePwd=%5

:: Check if the service is installed.
SC QUERY %ServiceName% > NUL
IF %ERRORLEVEL% EQU 0 (
    ECHO ********************************************
    ECHO * Service is already installed!            *
	ECHO * Please uninstall the service first!      *
    ECHO *                                          *
	ECHO * This window will be closed automatically *
    ECHO * in about 10 seconds...                   *
    ECHO ********************************************
	TIMEOUT /T 10
    GOTO END
)


:INSTALL
ECHO Installing the service...

IF "%ServiceUserType%"=="SpecificUser" (
	SC CREATE %ServiceName% binPath= "%ServiceExePath%" obj= "%ServiceUserName%" type= own password= "%ServicePwd%" > NUL
	if %errorlevel% EQU 0 (
		GOTO SUCCESS
	) ELSE (
		GOTO ERROR
	)
) ELSE (
	SC CREATE %ServiceName% binPath= "%ServiceExePath%" obj= "%ServiceUserName%" type= own > NUL
	if %errorlevel% EQU 0 (
		GOTO SUCCESS
	) ELSE (
		GOTO ERROR
	)
)


:SUCCESS
ECHO Installing service succeeded.
EXIT 0


:ERROR
ECHO Installing service failed!
TIMEOUT /T 5
EXIT 1


:END
EXIT 1

