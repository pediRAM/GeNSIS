
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
:: Author: Pedram GANJEH-HADIDI
:: Date: 2023-10-13
:: Script Description: if service-name is installed then stop and wait for it to stop.
:: Project Website: https://github.com/pediRAM/GeNSIS
:: -----------------------------------------------------------------------------------

@ECHO OFF
SET ServiceName=%1
SETLOCAL ENABLEDELAYEDEXPANSION

:: Check if the service is installed.
SC QUERY %ServiceName% > NUL 2>&1
IF %ERRORLEVEL% EQU 0 (
    ECHO Service is installed. Stopping it...
    SC STOP %ServiceName%
    :WAIT_STOP
    SC QUERY %ServiceName% | findstr "STOPPED" > NUL
    IF %ERRORLEVEL% NEQ 0 (
        ECHO Waiting 5 seconds for the service to stop...
        TIMEOUT /T 5 > NUL
        GOTO WAIT_STOP
    ) ELSE (
        ECHO Service has stopped.
    )
) ELSE (
	ECHO Service not installed/found.
)

ENDLOCAL
