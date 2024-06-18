using Avalonia.Layout;
using JVOS.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    public class UserSession
    {
        public static event EventHandler<UserSession> UserLogin;
        public static event EventHandler<UserSession> UserLogout;
        public static event EventHandler<UserSession> UserStart;
        public static event EventHandler<UserSession> UserStop;

        public static UserSession? CurrentUser;

        public event EventHandler<UserSession> WhenStart;
        public event EventHandler<UserSession> WhenStop;
        public event EventHandler<UserSession> WhenLogin;
        public event EventHandler<UserSession> WhenLogout;

        public static List<UserSession> Sessions;
        public UserOptions UserOptions;
        public DesktopScreen DesktopScreen;

        private bool isStarted = false;

        static UserSession()
        {
            Sessions = new List<UserSession>();
        }

        public UserSession(UserOptions UserOptionsForSession)
        {
            if(UserOptionsForSession == null)
            {
                throw new Exception();
            }    
            UserOptions = UserOptionsForSession;
            DesktopScreen = new DesktopScreen();
        }

        public static UserSession CreateOrGetSession(UserOptions options)
        {
            UserSession? session = Sessions.Find(x => x.UserOptions.Equals(options));
            if (session == null)
            {
                session = new UserSession(options);
                Sessions.Add(session);
            }
            return session;
        }

        public static void LogoutFromUser(UserSession session)
        {
            Sessions.Remove(session);
            session.OnLogout();
            session.OnStop();
        }

        /// <summary>
        /// Starts session and logins in
        /// </summary>
        public void Start()
        {
            if (isStarted)
                return;
            isStarted = true;
            OnStart();
            Login();
        }

        public void Stop()
        {
            if (!isStarted)
                return;
            OnStop();
            if (CurrentUser == this)
                Logout();
        }

        public void Logout()
        {
            OnLogout();
            if(CurrentUser == this)
                CurrentUser = null;
        }

        public void Login() 
        {
            if(!isStarted)
            {
                Start();
                return;
            }
            OnLogin();
            if (CurrentUser != null)
                CurrentUser.Logout();
            CurrentUser = this;
        }

        private void OnStart()
        {
            if (WhenStart != null)
                WhenStart.Invoke(this, this);
            if (UserStart != null)
                UserStart.Invoke(this, this);
        }

        private void OnStop()
        {
            if (WhenStop != null)
                WhenStop.Invoke(this, this);
            if (UserStop != null)
                UserStop.Invoke(this, this);
        }

        private void OnLogin()
        {
            if (WhenLogin != null)
                WhenLogin.Invoke(this, this);
            if (UserLogin != null)
                UserLogin.Invoke(this, this);
        }

        private void OnLogout()
        {
            if (WhenLogout != null)
                WhenLogout.Invoke(this, this);
            if (UserLogout != null)
                UserLogout.Invoke(this, this);
        }
    }
}
