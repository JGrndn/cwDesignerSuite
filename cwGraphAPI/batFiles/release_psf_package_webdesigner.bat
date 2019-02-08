set SAVE_FOLDER=Release_WebDesigner
set SAVE_ZIP_PREFIX=PSF_Web

cd bin/Release
cp CasewiseServicesFramework.exe ../%SAVE_FOLDER%/
cd ../%SAVE_FOLDER%
rm -rf configs
rm -rf logs
cd ..
set filename=%SAVE_ZIP_PREFIX%_%DATE:~6,4%_%DATE:~3,2%_%DATE:~0,2%
7z a -tzip %filename% %SAVE_FOLDER%
pause
