// Decompiled with JetBrains decompiler
// Type: KbControlOn.Properties.Resources
// Assembly: KbControlOn, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 429CF7B2-9973-4180-BEB2-334841D88BD5
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControlOn.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace KbControlOn.Properties
{
  [DebuggerNonUserCode]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
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
        if (KbControlOn.Properties.Resources.resourceMan == null)
          KbControlOn.Properties.Resources.resourceMan = new ResourceManager("KbControlOn.Properties.Resources", typeof (KbControlOn.Properties.Resources).Assembly);
        return KbControlOn.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => KbControlOn.Properties.Resources.resourceCulture;
      set => KbControlOn.Properties.Resources.resourceCulture = value;
    }
  }
}
