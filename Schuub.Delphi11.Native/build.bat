@rem This is free and unencumbered software released into the public domain under The Unlicense.
@rem You have complete freedom to do anything you want with the software, for any purpose.
@rem Please refer to http://unlicense.org/

@pushd %~dp0
@powershell -ExecutionPolicy bypass .\build.ps1 %*
@popd