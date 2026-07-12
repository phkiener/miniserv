#!/bin/sh

set -euo pipefail

version="$1"
if [ -z "$version" ]; then
  echo "Usage: $0 <version>"
  exit 1;
fi

dotnet restore

# Enable once I got tests
# dotnet test --no-restore --verbosity quiet --disable-logo --solution

dotnet build --no-restore --verbosity quiet --configuration Release -p:Version=$version
dotnet pack --no-restore --no-build --verbosity quiet --configuration Release --output "./publish/" -p:Version=$version
