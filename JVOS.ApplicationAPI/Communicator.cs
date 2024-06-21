using Avalonia.Media.Imaging;
using JVOS.ApplicationAPI.App;
using JVOS.ApplicationAPI.Hub;
using JVOS.ApplicationAPI.Windows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.ApplicationAPI
{
    public class DialogFileSystemBrowsingEventArgs
    {
        public DialogFileSystemBrowsingEventArgs(bool isDir, Action<DialogFileSystemBrowsingResult> doAfter, bool isMultiple = false, string filter = "*")
        {
            IsDirectory = isDir;
            DoAfter = doAfter;
            Filter = filter;
            IsMultiple = isMultiple;
        }

        public bool IsDirectory;
        public bool IsMultiple;
        public Action<DialogFileSystemBrowsingResult> DoAfter;
        public string Filter;
    }

    public struct DialogFileSystemBrowsingResult
    {
        public string[] Path = new string[0];
        public bool IsSuccessfull = true;

        public DialogFileSystemBrowsingResult(bool isSuccess, string[] pathes)
        {
            IsSuccessfull = isSuccess;
            Path = pathes;
        }
    }

    public static class Communicator
    {
        public static event EventHandler<WindowContentBase>? WindowOpenRequest;
        public static event EventHandler<WindowFrameBase>? FrameOpenRequest;
        public static event EventHandler<WindowFrameBase>? WindowCloseRequest;
        public static event EventHandler<WindowOpenRequest>? WindowOpen;
        public static event EventHandler<HubProvider>? RegisterExternalhub;
        public static event EventHandler<IEntryPoint>? RegisterEntrypoint;
        public static event EventHandler<IEntryPoint>? UnregisterEntrypoint;
        public static event EventHandler<IProtocol>? RegisterProtocol;
        public static event EventHandler<IProtocol>? UnregisterProtocol;
        public static event EventHandler<IMessageDialog>? ShowDialog;
        public static event EventHandler<(Bitmap?, string, string)>? NotificationShown;
        public static event EventHandler<string>? RegisterApp;
        public static event EventHandler<string>? CommandRun;
        public static event EventHandler<string>? PathRun;
        public static event EventHandler<DialogFileSystemBrowsingEventArgs>? BrowsePathRequest;

        public static IAppLoader AppLoader;

        public static void OpenWindow(WindowContentBase window)
        {
            if (WindowOpenRequest != null)
                WindowOpenRequest.Invoke(null, window);
        }
        public static void BrowseDirectory(Action<DialogFileSystemBrowsingResult> after)
        {
            if (BrowsePathRequest != null)
                BrowsePathRequest.Invoke(null, new (true, after));
        }
        public static void BrowseFile(Action<DialogFileSystemBrowsingResult> after, string filter = "*")
        {
            if (BrowsePathRequest != null)
                BrowsePathRequest.Invoke(null, new (false, after));
        }

        public static void OpenWindow(WindowOpenRequest request)
        {
            if (WindowOpen != null)
                WindowOpen.Invoke(null, request);
        }
        public static void OpenFrame(WindowFrameBase frame)
        {
            if(FrameOpenRequest != null)
                FrameOpenRequest.Invoke(null, frame);
        }

        public static void CloseWindow(WindowFrameBase window)
        {
            if (WindowCloseRequest != null)
                WindowCloseRequest.Invoke(null, window);
        }

        public static void RegisterApplication(string path)
        {
            if (RegisterApp != null)
                RegisterApp.Invoke(null, path);
        }
        public static void RunPath(string path)
        {
            if (PathRun != null)
                PathRun.Invoke(null, path);
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
        private static void OnRegisterHub(HubProvider hubProvider)
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
        public static void Register(HubProvider hubprov)
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

        private static bool isMobileMode = false;

        public static bool IsGodot { get; internal set; }

        public static bool IsInMobileMode()
        {
            return isMobileMode;
        }

        public static void SetInMobileMode(bool value)
        {
            isMobileMode = value;
        }

        public static bool LoadApp(string path, object[]? args, out App.App? app)
        {
            if (AppLoader != null)
                return AppLoader.LoadApp(path, args, out app);
            app = null;
            return false;
        }
    }
}
