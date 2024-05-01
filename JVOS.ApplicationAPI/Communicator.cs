using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public static class Communicator
    {
        public static event EventHandler<IJWindow>? WindowOpenRequest;
        public static event EventHandler<IJWindowFrame>? WindowCloseRequest;
        public static event EventHandler<IHubProvider>? RegisterExternalhub;
        public static event EventHandler<IEntryPoint>? RegisterEntrypoint;
        public static event EventHandler<IEntryPoint>? UnregisterEntrypoint;
        public static event EventHandler<IProtocol>? RegisterProtocol;
        public static event EventHandler<IProtocol>? UnregisterProtocol;
        public static event EventHandler<IMessageDialog>? ShowDialog;
        public static event EventHandler<(Bitmap?, string, string)>? NotificationShown;
        public static event EventHandler<string>? RegisterApp;
        public static event EventHandler<string>? CommandRun;

        public static void OpenWindow(IJWindow window)
        {
            if (WindowOpenRequest != null)
                WindowOpenRequest.Invoke(null, window);
        }
        public static void CloseWindow(IJWindowFrame window)
        {
            if (WindowCloseRequest != null)
                WindowCloseRequest.Invoke(null, window);
        }

        public static void RegisterApplication(string path)
        {
            if(RegisterApp != null)
                RegisterApp.Invoke(null, path);
        }

        public static void ShowNotification((Bitmap?, string, string) notification)
        {
            if(NotificationShown != null)
                NotificationShown.Invoke(null, notification);
        }

        public static void ShowMessageDialog(IMessageDialog dialog)
        {
            if (ShowDialog != null)
                ShowDialog.Invoke(null, dialog);
        }

        private static void OnRegisterProtocol(IProtocol protocol)
        {
            if (RegisterProtocol != null)
                RegisterProtocol.Invoke(null, protocol);
        }
        private static void OnUnregisterProtocol(IProtocol protocol)
        {
            if (UnregisterProtocol != null)
                UnregisterProtocol.Invoke(null, protocol);
        }
        private static void OnRegisterEntryPoint(IEntryPoint entryPoint)
        {
            if (RegisterEntrypoint != null)
                RegisterEntrypoint.Invoke(null, entryPoint);
        }
        private static void OnUnregisterEntryPoint(IEntryPoint entryPoint)
        {
            if (UnregisterEntrypoint != null)
                UnregisterEntrypoint.Invoke(null, entryPoint);
        }
        private static void OnRegisterHub(IHubProvider hubProvider)
        {
            if(RegisterExternalhub != null)
                RegisterExternalhub(null, hubProvider);
        }

        public static void Register(IProtocol protocol)
        {
            OnRegisterProtocol(protocol);
        }
        public static void Register(IEntryPoint entryPoint)
        {
            OnRegisterEntryPoint(entryPoint);
        }
        public static void Register(IHubProvider hubprov)
        {
            OnRegisterHub(hubprov);
        }
        
        public static void Unregister(IProtocol protocol)
        {
            OnUnregisterProtocol(protocol);
        }
        public static void Unregister(IEntryPoint entryPoint)
        {
            OnUnregisterEntryPoint(entryPoint);
        }

        public static void RunCommand(string direction)
        {
            if (CommandRun != null)
                CommandRun.Invoke(null, direction);
        }

    }
}
