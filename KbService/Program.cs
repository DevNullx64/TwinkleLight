// Decompiled with JetBrains decompiler
// Type: KbService.Program
// Assembly: KbService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1EE4A412-47D9-4D2C-8294-3E09959B4F08
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbService.exe

using System.ServiceProcess;

namespace KbService
{
  internal static class Program
  {
    private static void Main() => ServiceBase.Run(new ServiceBase[1]
    {
      (ServiceBase) new Service1()
    });
  }
}
