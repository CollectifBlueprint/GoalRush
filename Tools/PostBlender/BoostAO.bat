set inFile=%1%
set inFile=%inFile:"=%
set inFile=%inFile:\=\\%

set outFile=%2%
set outFile=%outFile:"=%
set outFile=%outFile:\=\\%

%gimp% -i -b "(script-boostAO-post-process \"%inFile%\" \"%outFile%\")" -b "(gimp-quit 0)"