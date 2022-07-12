#/bin/sh

nuget restore packages.config
mkdir Assemblies
cp Packages/*/lib/netstandard2.0/*.dll Assemblies
rm -rf Packages
