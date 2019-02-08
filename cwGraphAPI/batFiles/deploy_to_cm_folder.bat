SET BIN_PSF_FOLDER=C:\Users\Pouya MOHTACHAM\Documents\Visual Studio 2008\Projects\Casewise\Casewise PSG Framework\Casewise.GraphAPI\bin\x86\Debug\
SET BIN_PLUGIN_FOLDER=C:\Users\Pouya MOHTACHAM\Documents\Visual Studio 2008\Projects\Casewise\Casewise PSG Framework\Casewise.GraphAPI\cwPlugins\cwPSFPlugins\bin\x86\Debug\"

SET CM_FOLDER=C:\Program Files\Casewise\CM10\BIN\

XCOPY /C /Y "%BIN_PSF_FOLDER%Casewise.GraphAPI.exe" "%CM_FOLDER%"
XCOPY /C /Y "%BIN_PSF_FOLDER%Casewise.GraphAPI.exe.config" "%CM_FOLDER%"
XCOPY /C /Y "%BIN_PSF_FOLDER%log4net.dll" "%CM_FOLDER%"
XCOPY /C /Y "%BIN_PSF_FOLDER%log4net.xml" "%CM_FOLDER%"
XCOPY /C /Y "%BIN_PLUGIN_FOLDER%cwPSFPlugins.dll "%CM_FOLDER%"
