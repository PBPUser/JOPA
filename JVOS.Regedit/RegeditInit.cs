using JVOS.ApplicationAPI.App;

namespace JVOS.Regedit
{
    public class RegeditInit : App
    {
        public RegeditInit(AppCommunicator communicator) : base(communicator)
        {
        }

        public override void OnActivity(string name, object[] args)
        {
            switch (name) {
                case "main":
                    Communicator.OpenJWindow(new RegeditWindow());
                    break;
            }
            base.OnActivity(name, args);
        }
    }
}
