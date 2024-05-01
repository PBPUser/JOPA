using JVOS.ApplicationAPI;

namespace JVOS.ProgressiveWebApplicationSubSystem
{
    public class AppInitialize : IInitializer
    {
        public void Deinitialize()
        {

        }

        public void Initialize()
        {
            ApplicationAPI.Communicator.Register(new PWAProtocol("pwa"));
            ApplicationAPI.Communicator.Register(new PWAProtocol("https"));
            ApplicationAPI.Communicator.Register(new PWAProtocol("http"));
        }
    }
}
