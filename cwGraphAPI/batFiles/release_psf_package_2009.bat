set DROP_FOLDER=C:/Users/Pouya MOHTACHAM/Dropbox/Public/PSF

cd bin/x86/Release-2009
cp CasewiseServicesFramework.exe ../../PSF_Release_2009.2/
cd ../../PSF_Release
rm -rf configs
rm -rf logs
cd ../..
set filename=PSF_%DATE:~6,4%_%DATE:~3,2%_%DATE:~0,2%
7z a -tzip %filename% PSF_Release_2009.2

copy "./%filename%.zip" "%DROP_FOLDER%/"

pause
