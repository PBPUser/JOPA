using JVOS.ApplicationAPI.App;

namespace JVOS.Xbox352Emulator
{
    public class Xbox352Init : App
    {
        public Xbox352Init(AppCommunicator communicator) : base(communicator)
        {
        }

        public override void OnActivity(string name, object[] args)
        {
            switch (name)
            {
                case "main":
                    Communicator.OpenJWindow(new Xbox352Window());
                    return;
            }
        }
    }
}
