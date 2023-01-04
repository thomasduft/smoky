#!/bin/bash

echo ----
echo Instructions: 0.2.0 r i/p
echo 0.2.0: version
echo r: release
echo i/p: install locally / publish to private nuget server

if [ -z "$1" ];
then
  echo ----
  echo No version specified! Please specify a valid version like 1.2.3 or 1.2.3-rc1!
  exit 1
fi

if [ -z "$2" ];
then
  echo ----
  echo No release info specified! For a relase provide the r flag and a valid release verion!
  exit 1
fi

echo ----
echo Starting building version $1

echo ----
echo Cleaning up
rm -r ./dist
dotnet tool uninstall -g tomware.Smoky

echo ----
echo Restore solution
dotnet restore src/smoky

echo ----
if [ $2 = "r" ];
then
  echo Packaging solution with Version = $1
  dotnet pack src/smoky -c Release -p:PackageVersion=$1 -p:Version=$1 -o ./dist/nupkgs/
fi

if [ "$3" = "i" ];
then
  echo ----
  echo Installing smoky globally with Version = $1
  dotnet tool install --global --add-source ./dist/nupkgs/ tomware.smoky
fi

echo ----
echo Done