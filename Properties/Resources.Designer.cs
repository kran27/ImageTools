using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ImageTools.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ImageTools.Properties.Resources.resourceMan == null)
          ImageTools.Properties.Resources.resourceMan = new ResourceManager("ImageTools.Properties.Resources", typeof (ImageTools.Properties.Resources).Assembly);
        return ImageTools.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ImageTools.Properties.Resources.resourceCulture;
      set => ImageTools.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap palette => (Bitmap) ImageTools.Properties.Resources.ResourceManager.GetObject(nameof (palette), ImageTools.Properties.Resources.resourceCulture);
  }
}
