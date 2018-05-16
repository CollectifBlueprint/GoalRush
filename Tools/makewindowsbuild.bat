set SOLUTIONDIR=%1
set TARGETDIR=%2

set BUILDDIR=%SOLUTIONDIR%..\Build\
set BUILDSUBDIR=%BUILDDIR%GoalRush\

if NOT EXIST BUILDDIR (
	mkdir %BUILDDIR%
	mkdir %BUILDSUBDIR%
)

xcopy /s /q %TARGETDIR%* %BUILDSUBDIR%
del /s /q %BUILDSUBDIR%*.pdb

xcopy /s /q %SOLUTIONDIR%"05 - Content\*" %BUILDSUBDIR%"05 - Content\"
xcopy /s /q %SOLUTIONDIR%"..\BuildIncludeFiles\*" %BUILDSUBDIR%
xcopy /q %SOLUTIONDIR%"..\License.txt" %BUILDSUBDIR%