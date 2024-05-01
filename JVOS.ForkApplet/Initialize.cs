using JVOS.ApplicationAPI;

namespace JVOS.ForkApplet
{
    public class Initialize : IInitializer
    {
        public void Deinitialize()
        {
            throw new NotImplementedException();
        }

        void IInitializer.Initialize()
        {
            Communicator.Register(new ForkHubProvider());
        }
    }
}
