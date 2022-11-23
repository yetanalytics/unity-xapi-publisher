#!/bin/bash

nuget restore packages.config -PackagesDirectory Packages
mkdir Assemblies
cp Packages/*/lib/netstandard2.0/*.dll Assemblies
rm -rf Packages
