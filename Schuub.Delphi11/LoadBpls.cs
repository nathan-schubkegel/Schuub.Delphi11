// This is free and unencumbered software released into the public domain under The Unlicense.
// You have complete freedom to do anything you want with the software, for any purpose.
// Please refer to <http://unlicense.org/>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Schuub.Delphi11
{
  /// <summary>
  /// Class for loading the BPLs that are packaged with this library
  /// so their methods can be invoked by C#.
  /// </summary>
  public static class LoadBpls
  {
    [DllImport("QuickSetSharp.DelphiInterop.Native.x64",
      EntryPoint = "LoadBplsW",
      CharSet = CharSet.Unicode,
      SetLastError = true)]
    private static extern int LoadBpls64(
      [MarshalAs(UnmanagedType.LPWStr)] string applicationDirPath);

    [DllImport("QuickSetSharp.DelphiInterop.Native.x86",
      EntryPoint = "LoadBplsW",
      CharSet = CharSet.Unicode,
      SetLastError = true)]
    private static extern int LoadBpls32(
      [MarshalAs(UnmanagedType.LPWStr)] string applicationDirPath);

    /// <summary>
    /// Loads the BPLs that are packaged with this library
    /// so their methods can be invoked by C#.
    /// </summary>
    /// <exception cref="Win32Exception">Thrown on failure.</exception>
    public static void Invoke()
    {
      var applicationDirPath = AppDomain.CurrentDomain.BaseDirectory;

      int result = IntPtr.Size == 8
        ? LoadBpls64(applicationDirPath)
        : LoadBpls32(applicationDirPath);

      if (result == 0)
      {
        throw new Win32Exception();
      }
    }
  }
}
