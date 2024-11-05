// This is free and unencumbered software released into the public domain under The Unlicense.
// You have complete freedom to do anything you want with the software, for any purpose.
// Please refer to <http://unlicense.org/>

using NUnit.Framework;
using System;
using System.IO;

namespace Schuub.Delphi11.Tests
{
  public class LoadBplTests
  {
    [Test]
    public void LoadBpl_ForRtl280_DoesNotThrow()
    {
      var architecture = IntPtr.Size == 8 ? "x64" : "x86";
      var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, architecture, "rtl280.bpl");
      IntPtr result = LoadBpl.Invoke(filePath);
      Assert.That(result, Is.Not.Zero);
    }
  }
}
