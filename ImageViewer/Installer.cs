using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Tama;
using Tama.ImageViewer;

namespace ImageViewer
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            string applicationPath = Context.Parameters["assemblypath"];
            string workingDir = Path.GetDirectoryName(applicationPath);
            string description = "Provides lightning fast image viewing for your everyday pleasure.";

            Program.GetShortcutFullPaths(out string desktopUser, out string startMenuUser);

            Helpers.CreateShortcut(desktopUser, applicationPath, workingDir, description, null);
            Helpers.CreateShortcut(startMenuUser, applicationPath, workingDir, description, null);

            //Installer doesn't allow internet access, there is no other way to download anything.
            Process.Start(applicationPath, "-downloadExampleImages");

            base.OnAfterInstall(savedState);
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            Program.GetShortcutFullPaths(out string desktopUser, out string startMenuUser);

            if (File.Exists(desktopUser))
                File.Delete(desktopUser);

            if (File.Exists(startMenuUser))
                File.Delete(startMenuUser);

            base.OnBeforeUninstall(savedState);
        }
    }
}
