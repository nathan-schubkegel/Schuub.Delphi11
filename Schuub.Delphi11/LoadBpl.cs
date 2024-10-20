// This is free and unencumbered software released into the public domain under The Unlicense.
// You have complete freedom to do anything you want with the software, for any purpose.
// Please refer to <http://unlicense.org/>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Schuub.Delphi11
{
  /// <summary>
  /// Class for loading BPLs so their methods can be invoked by C#.
  /// </summary>
  public static class LoadBpl
  {
    [DllImport("Schuub.Delphi11.Native.x64",
      EntryPoint = "LoadBplW",
      CharSet = CharSet.Unicode,
      SetLastError = true)]
    private static extern IntPtr LoadBpl64(
      [MarshalAs(UnmanagedType.LPWStr)] string fileNameOrPath);

    [DllImport("Schuub.Delphi11.Native.x86",
      EntryPoint = "LoadBplW",
      CharSet = CharSet.Unicode,
      SetLastError = true)]
    private static extern IntPtr LoadBpl32(
      [MarshalAs(UnmanagedType.LPWStr)] string fileNameOrPath);

    /// <summary>
    /// Loads the given BPL so its methods can be invoked by C#.
    /// </summary>
    /// <param name="fileNameOrPath">The file name or path of a BPL to load.
    /// If file name or relative path is given, then Dynamic-link library search order
    /// is used to find it. See https://learn.microsoft.com/en-us/windows/win32/dlls/dynamic-link-library-search-order
    /// If a full path is given, then the file at that path is loaded.</param>
    /// <exception cref="Win32Exception">Thrown on failure.</exception>
    /// <returns>The HMODULE of the loaded BPL, which is needed later to
    /// use methods and types declared in the BPL.</returns>
    public static IntPtr Invoke(string fileNameOrPath)
    {
      // LoadLibrary() documentation on MSDN says
      // "When specifying a path, be sure to use backslashes (\), not forward slashes (/)."
      // so... uh... I guess just fix it for the caller?
      fileNameOrPath = fileNameOrPath.Replace('/', '\\');

      return IntPtr.Size == 8
        ? LoadBpl64(fileNameOrPath)
        : LoadBpl32(fileNameOrPath);
    }
  }
}
