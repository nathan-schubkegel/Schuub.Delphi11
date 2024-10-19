@pushd %~dp0
@powershell -ExecutionPolicy bypass .\build.ps1 %*
@popd