using JVOS.ApplicationAPI;

namespace JVOS.ApplicationInstaller
{
    public class Initialize : IInitializer
    {
        public void Deinitialize()
        {

        }

        void IInitializer.Initialize()
        {
            Communicator.Register(new AppInstallerEntryPoint());
            if (!Directory.Exists(PlatformSpecifixController.GetLocalFilePath("Caches")))
            {
                Directory.CreateDirectory(PlatformSpecifixController.GetLocalFilePath("Caches"));
            }
            if (!Directory.Exists(PlatformSpecifixController.GetLocalFilePath("Caches\\ApplicationInstaller")))
            {
                Directory.CreateDirectory(PlatformSpecifixController.GetLocalFilePath("Caches\\ApplicationInstaller"));
            }

        }
    }
}
