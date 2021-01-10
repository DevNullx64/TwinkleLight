// Decompiled with JetBrains decompiler
// Type: KbService.ProjectInstaller
// Assembly: KbService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1EE4A412-47D9-4D2C-8294-3E09959B4F08
// Assembly location: C:\Program Files (x86)\EA\KbControl\KbService.exe

using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace KbService
{
  [RunInstaller(true)]
  public class ProjectInstaller : Installer
  {
    private IContainer components;
    private ServiceProcessInstaller serviceProcessInstaller1;
    private ServiceInstaller serviceInstaller1;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.serviceProcessInstaller1 = new ServiceProcessInstaller();
      this.serviceInstaller1 = new ServiceInstaller();
      this.serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
      this.serviceProcessInstaller1.Password = (string) null;
      this.serviceProcessInstaller1.Username = (string) null;
      this.serviceInstaller1.Description = "KbControl Services";
      this.serviceInstaller1.DisplayName = "KbService";
      this.serviceInstaller1.ServiceName = "KbService";
      this.serviceInstaller1.StartType = ServiceStartMode.Automatic;
      this.Installers.AddRange(new Installer[2]
      {
        (Installer) this.serviceProcessInstaller1,
        (Installer) this.serviceInstaller1
      });
    }

    public ProjectInstaller() => this.InitializeComponent();
  }
}
