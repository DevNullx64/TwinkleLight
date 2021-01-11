// Decompiled with JetBrains decompiler
// Type: LedControl.Properties.Resources
// Assembly: LedControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2625D33F-FFFD-422A-94C4-E655C9C02803
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbControl.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace LedControl.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources_
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources_()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (LedControl.Properties.Resources_.resourceMan == null)
          LedControl.Properties.Resources_.resourceMan = new ResourceManager("LedControl.Properties.Resources", typeof (LedControl.Properties.Resources_).Assembly);
        return LedControl.Properties.Resources_.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => LedControl.Properties.Resources_.resourceCulture;
      set => LedControl.Properties.Resources_.resourceCulture = value;
    }
  }
}
