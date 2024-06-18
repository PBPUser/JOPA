using JVOS.ApplicationAPI.App;

namespace JVOS.OOBE
{
    public class OOBEEntryPoint : App
    {
        public OOBEEntryPoint(AppCommunicator communicator) : base(communicator)
        {
        }

        public override void OnActivity(string name, object[] args)
        {
            switch (name)
            {
                case "nikitos":
                    Communicator.OpenJWindow(new Nikitos());
                    break;

            }
        }
    }
}
