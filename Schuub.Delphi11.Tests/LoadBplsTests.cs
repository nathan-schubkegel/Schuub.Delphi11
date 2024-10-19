using NUnit.Framework;

namespace Schuub.Delphi11.Tests
{
  public class LoadBplsTests
  {
    [Test]
    public void LoadBpls_DoesNotThrow()
    {
      LoadBpls.Invoke();
    }
  }
}
