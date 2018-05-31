---------------------------------------------------------------------
-               GOAL RUSH DEVELOPER README                          -  
---------------------------------------------------------------------


COMPILING INSTRUCTIONS
---------------------------------------------------------------------

++ Compile source code ++ 
run Project/01 - MonoGame/mono Protobuild.exe
load Project/ProtoGo.Linux.sln or Project/ProtoGo.Windows.sln and build

On linux you may need :
libxi-dev
libopenal-dev
libsdl-mixer1.2

On Windows you may need :
OpenAL 1.1 Core SDK

Tested on Ubuntu 16.04 and later versions
Tested on Windows 7 and later versions

++ Compile Shaders ++
If you need to modify or recompile existing shaders, you will have to 
build the MonoGame 2MGFX Tool. 2MGFX is Windows only. Linux programmers 
have to run 2MGFX on a windows platform to perform .fx shaders compilation.

- Windows 10 Install :

Download DirectX End-User Runtimes (June 2010) at 
https://www.microsoft.com/en-us/download/details.aspx?id=8109. 
It provides the DirectX redistributable and D3D compilers used by 2MGFX.
Run the installer and extract files to a <c:/MyDxInstall> like directory. 
You might get an error if files are extracted in the <UserName> directory.
Run c:/MyDxInstall/DXSETUP.exe

Open Project\01 - MonoGame\Tools\2MGFX\2MGFX.Windows.csproj and save 
it as a .sln file. Install the SharpDX Assemblies 4.0.1 Nugget Package. 
To do so you can copy this command in the Package Manager console : 
PM> Install-Package SharpDX -Version 4.0.1

Build the 2MGFX csproj in Release and Debug Configurations.

Shaders will now be recompiled if modified at GoalRush runtime. 
Alternatively you can manually compile shaders using 2MGFX.exe
  