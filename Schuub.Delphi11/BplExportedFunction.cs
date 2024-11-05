﻿// This is free and unencumbered software released into the public domain under The Unlicense.
// You have complete freedom to do anything you want with the software, for any purpose.
// Please refer to <http://unlicense.org/>

using System.Collections.Generic;

namespace Schuub.Delphi11
{
  public class BplExportedFunction
  {
    public string MangledName { get; set; }

    public string DemangledName { get; set; }

    public List<BplExportedType> Parameters { get; } = new List<BplExportedType>();
  }
}
