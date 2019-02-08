SET XML_DIR=C:\dev\devTeam\webDesigner\XMLExport
CD C:\Program Files\Casewise\CM10\BIN

START /W XMLImportExport.exe -import -toexistingmodel -model:UNITTEST -filename:"%XML_DIR%\ObjectTypeExport.xml" -errorfile:"%XML_DIR%\import_error.log"

CD %XML_DIR%
pause


