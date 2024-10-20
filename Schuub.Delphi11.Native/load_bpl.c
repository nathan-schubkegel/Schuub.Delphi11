#include <windows.h>

HMODULE __declspec(dllexport) LoadBplW(wchar_t * filePath)
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
