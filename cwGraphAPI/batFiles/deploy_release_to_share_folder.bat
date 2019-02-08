@ECHO OFF

SET BIN_INSTALLER_FOLDER=C:\Users\Pouya MOHTACHAM\Documents\Visual Studio 2008\Projects\Casewise\Casewise PSG Framework\Casewise.GraphAPI\
CD "C:\Program Files\Inno Setup 5\"
iscc.exe "%BIN_INSTALLER_FOLDER%webDesigner.iss"

SET DEST_FOLDER=C:\share\webDesigner\
XCOPY /C /Y "%BIN_INSTALLER_FOLDER%Output\setup-casewise-webdesigner*.exe" "%DEST_FOLDER%"

cd %BIN_INSTALLER_FOLDER%