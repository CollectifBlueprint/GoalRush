Windows settings

- Set an environment variable named "gimp" referencing the gimp-console-xxx.exe. (like "C:\Program Files\GIMP 2\bin\gimp-console-2.8.exe")

- Set an environment variable named "Protogo" referencing the protogo project folder

- Copy the scm files in the gimp script user directory (like %HOMEPATH%\.gimp-2.8\scripts)

- Example of use :
Export.bat as in the ArenaLarge render dir
call %Protogo%\Tools\PostBlender\ConvertArenaMaterial.bat "material0000.png" "export\material0000.png"
call %Protogo%\Tools\PostBlender\Collision.bat "collision0000.png" "export\collision0000.png"
call %Protogo%\Tools\PostBlender\BoostAO.bat "AO0000.png" "export\AO0000.png"
call %Protogo%\Tools\PostBlender\BoostAO.bat "AONoFloor0000.png" "export\AONoFloor0000.png"
call %Protogo%\Tools\PostBlender\Preview.bat "AO0000.png" "export\preview.png"
call %Protogo%\Tools\PostBlender\CopyToContent.bat "export" "C:\Workspace\ProtoGo\Project\05 - Content\Arenas\ArenaLarge"