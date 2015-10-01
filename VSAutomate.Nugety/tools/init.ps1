# Runs the first time a package is installed in a solution, and every time the solution is opened.

param($installPath, $toolsPath, $package, $project)

# load our assembly and call init($dte, $toolsPath)
# this locks the dll and uninstall requires vs restart
# shadow-copying dlls to temp dir seems flaky so have left this as is
$buildDll = Join-Path $toolsPath "net45\VSAutomate.dll"
[System.Reflection.Assembly]::LoadFrom($buildDll)
[VSAutomate.Nuget]::Init($dte.DTE, $toolsPath)
