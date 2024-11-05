// This is free and unencumbered software released into the public domain under The Unlicense.
// You have complete freedom to do anything you want with the software, for any purpose.
// Please refer to <http://unlicense.org/>

using NUnit.Framework;
using System.Linq;

namespace Schuub.Delphi11.Tests
{
  public class BplExportedFunction64Tests
  {
    [Test]
    public void Parse_ReadStringAsUnicode_IsParsedCorrectly()
    {
      const string mangledName = "_ZN6System8TMarshal19ReadStringAsUnicodeERKNS_11TPtrWrapperEi";
      var result = BplExportedFunction64.Parse(mangledName);
      Assert.That(result.MangledName, Is.EqualTo(mangledName));
      Assert.That(result.DemangledName, Is.EqualTo("System.TMarshal.ReadStringAsUnicode"));

      Assert.That(result.Parameters.ElementAtOrDefault(0)?.MangledName, Is.EqualTo("RKNS_11TPtrWrapperE"));
      Assert.That(result.Parameters.ElementAtOrDefault(0)?.DemangledName, Is.EqualTo("System.TPtrWrapper"));
      Assert.That(result.Parameters.ElementAtOrDefault(0)?.Flags, Is.EqualTo(BplExportedTypeFlags.Const | BplExportedTypeFlags.Reference));

      Assert.That(result.Parameters.ElementAtOrDefault(1)?.MangledName, Is.EqualTo("i"));
      Assert.That(result.Parameters.ElementAtOrDefault(1)?.DemangledName, Is.EqualTo("integer"));
      Assert.That(result.Parameters.ElementAtOrDefault(1)?.Flags, Is.EqualTo(BplExportedTypeFlags.None));

      Assert.That(result.Parameters.Count, Is.EqualTo(2), "parameter count");
    }
  }
}
