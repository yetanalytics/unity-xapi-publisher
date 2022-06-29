#/bin/sh

nuget restore packages.config
cp Packages/*/lib/netstandard2.0/*.dll Plugins
rm -rf Packages/*
