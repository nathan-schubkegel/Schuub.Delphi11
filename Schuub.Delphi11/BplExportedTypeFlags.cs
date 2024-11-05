// This is free and unencumbered software released into the public domain under The Unlicense.
// You have complete freedom to do anything you want with the software, for any purpose.
// Please refer to <http://unlicense.org/>

using System;

namespace Schuub.Delphi11
{
  [Flags]
  public enum BplExportedTypeFlags
  {
    None = 0,
    Reference = 1, // C++ & ref-qualifier... tends to be used when passing a pointer to structs
    Const = 2, // tends to be used when passing a pointer to structs
  }
}
