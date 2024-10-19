#include <windows.h>
#include <stdint.h>
#include <wchar.h>
#include <string.h>
#include <stdlib.h>

static HMODULE LoadBplW(wchar_t * filePath)
{
  HMODULE h = LoadLibraryW(filePath);
  if (h == 0)
  {
    return 0;
  }
  
  // BPLs have a 'Initialize' method that needs to be invoked to run 'initialization'
  // sections of units in the BPL - see delphi source of function LoadPackage() in SysUtils.pas
  FARPROC p = GetProcAddress(h, "Initialize");
  if (p == 0) 
  {
    return 0;
  }
  
  // technically the calling convention needs to be borland register call
  // but since there are no arguments, stdcall works just as well
  void __declspec(stdcall) (*foo)( void );
  foo = p;
  foo();
  
  return h;
}

static wchar_t * localFilePath = 0;

static HMODULE LoadBplByFileNameW(wchar_t * applicationDirPath, wchar_t * fileName)
{
  const wchar_t * architecture = sizeof(void*) == 8 ? L"x64" : L"x86";

  // free the last allocated file path
  if (localFilePath != 0)
  {
    free(localFilePath);
    localFilePath = 0;
  }

  size_t applicationDirPathLength = wcslen(applicationDirPath);
  size_t filePathLength = applicationDirPathLength + 1 + 
    wcslen(architecture) + 1 + 
    wcslen(fileName) + 1;

  wchar_t * localFilePath = malloc(filePathLength * sizeof(wchar_t));
  if (localFilePath == 0)
  {
    SetLastError(ERROR_NOT_ENOUGH_MEMORY);
    return 0;
  }

  wcscpy(localFilePath, applicationDirPath);
  if (!(applicationDirPathLength == 0 ||
      applicationDirPath[applicationDirPathLength - 1] == '/' ||
      applicationDirPath[applicationDirPathLength - 1] == '\\'))
  {
    wcscat(localFilePath, L"/");
  }
  wcscat(localFilePath, architecture);
  wcscat(localFilePath, L"/");
  wcscat(localFilePath, fileName);
  
  return LoadBplW(localFilePath);
}

// not static because other files in this C project will reference it via externs
HMODULE rtl280 = 0;

int32_t __declspec(dllexport) LoadBplsW(wchar_t * applicationDirPath)
{
  if (rtl280 == 0)
  {
    rtl280 = LoadBplByFileNameW(applicationDirPath, L"rtl280.bpl");
  }

  return rtl280 != 0;
}