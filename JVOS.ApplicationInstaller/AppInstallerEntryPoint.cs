using JVOS.ApplicationAPI.App;

namespace JVOS.ApplicationInstaller
{
    public class ApplicationInstallerEntryPoint : App
    {
        public ApplicationInstallerEntryPoint(AppCommunicator communicator) : base(communicator)
        {
        }

        public override void OnActivity(string name, object[] args)
        {
            switch (name)
            {
                case "main":
                    Communicator.OpenJWindow(new AppInstallerWindow());
                    return;
            }
        }
    }
}
