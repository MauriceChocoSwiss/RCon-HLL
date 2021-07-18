using System.Globalization;
using System.Resources;

namespace RconClient.Properties
{
  //[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  //[DebuggerNonUserCode]
  //[CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    //[EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (RconClient.Properties.Resources.resourceMan == null)
          RconClient.Properties.Resources.resourceMan = new ResourceManager("RconClient.Properties.Resources", typeof (RconClient.Properties.Resources).Assembly);
        return RconClient.Properties.Resources.resourceMan;
      }
    }

    //[EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => RconClient.Properties.Resources.resourceCulture;
      set => RconClient.Properties.Resources.resourceCulture = value;
    }
  }
}
