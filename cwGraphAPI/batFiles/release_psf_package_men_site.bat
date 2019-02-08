set SAVE_FOLDER=Release_MEN_SITE
set SAVE_ZIP_PREFIX=PSF_MEN_SITE
set DROP_FOLDER=C:/Users/Pouya MOHTACHAM/Dropbox/Public/MEN
set SITE_PATH=C:/dev/wamp/www/publicationscw
set SITE_NAME=MENAuto

cd bin/Release
cp CasewiseServicesFramework.exe ../%SAVE_FOLDER%/
cd ../%SAVE_FOLDER%
rm -rf configs
rm -rf logs
cd ..
set filename=%SAVE_ZIP_PREFIX%_%DATE:~6,4%_%DATE:~3,2%_%DATE:~0,2%
7z a -tzip %filename% %SAVE_FOLDER%

@rem copy tar ball to drop box folder
copy "./%filename%.zip" "%DROP_FOLDER%/"


cd "%SITE_PATH%/%SITE_NAME%"
rm -rf Lib
xcopy /E /I "%SITE_PATH%/Lib" "%SITE_PATH%/%SITE_NAME%/Lib"
set SITE_FILENAME=%SAVE_ZIP_PREFIX%_%SITE_NAME%_%DATE:~6,4%_%DATE:~3,2%_%DATE:~0,2%

rm -rf "%SITE_FILENAME%"

cd "%SITE_PATH%"
7z a -tzip "%SITE_FILENAME%" "%SITE_NAME%"

copy "./%SITE_FILENAME%.zip" "%DROP_FOLDER%/"

pause
