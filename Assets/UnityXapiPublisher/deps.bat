@echo off

nuget "restore" "packages.config"
mkdir "Assemblies"
COPY  "Packages\*\lib\netstandard2.0\*%CD%dll" "Assemblies"
DEL /S "Packages"
