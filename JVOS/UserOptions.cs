using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.App;
using JVOS.Controls;
using JVOS.DataModel;
using JVOS.Screens;
using JVOS.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;
using Path = System.IO.Path;

namespace JVOS
{
    public class UserOptions
    {
        static string UsersLocation;
        public static UserOptions? Current;
        public static List<UserOptions> Users;

        private Dictionary<string, object> _userValues;

        [JsonIgnore]
        public RecentFilesManager RecentManager;

        public Theme Theme;
        [JsonIgnore]
        public Bitmap? DesktopBitmap;
        [JsonIgnore]
        public string MenuDirectory;
        [JsonIgnore]
        public string UserDirectory;
        public bool Prepared = false;

        #region PRIVATE_UTILITY
        private static void CreateIfIsntExsits(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        
        public static string ImageToBase64(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, 100);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        public static byte[] ImageToBytes(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, 100);
                return ms.ToArray();
            }
        }

        public static Bitmap? Base64ToImage(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    return new Bitmap(ms);
            }
            catch (Exception e)
            {
                Debug.WriteLine(base64String);
                Debug.WriteLine($"{e.Message}");
                Debug.WriteLine($"{e.StackTrace}");
                Debug.WriteLine($"{e.Source}");
                return null;
            }

        }
        private static Bitmap LoadImage(string path)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(path);
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    return new Bitmap(ms);
            }
            catch(Exception e)
            {
                App.SendNotification($"{e.Message}");
                App.SendNotification($"{e.StackTrace}");
                App.SendNotification($"{e.Source}");
                return null;
            }

        }
        #endregion

        public static void SetLockscreenUserOptions(UserOptions userOptions)
        {
            if (Current == userOptions)
                return;
            App.Current.Resources["Username"] = userOptions.Username;
            App.Current.Resources["Userimage"] = Base64ToImage(userOptions.Base64Avatar??"");
            var desktopImage = PlatformSpecifixController.GetLocalFilePath($"Users\\{userOptions.Username}\\AppData\\JVOS\\desktop.png");
            var profileImage = PlatformSpecifixController.GetLocalFilePath($"Users\\{userOptions.Username}\\AppData\\JVOS\\profile.png");
            var lockscreenImage = PlatformSpecifixController.GetLocalFilePath($"Users\\{userOptions.Username}\\AppData\\JVOS\\lockscreen.png");
            if (!File.Exists(desktopImage))
                userOptions.SetDesktopImage(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Wallpapers/background_light.png"))));
            else
                userOptions.ApplyDesktopImage(new Bitmap(desktopImage));
            if (!File.Exists(profileImage))
                userOptions.SetProfileImage(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/default_avatar.png"))));
            else
                userOptions.ApplyProfileImage(new Bitmap(profileImage));
            if (!File.Exists(lockscreenImage))
                userOptions.SetLockscreenImage(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Wallpapers/background_light.png"))));
            else
                userOptions.ApplyLockscreenImage(new Bitmap(lockscreenImage));
            App.Current.Resources["DesktopStretch"] = userOptions.GetUserValue<Stretch>("DesktopStretch", Stretch.Fill);
            ColorScheme.ApplyScheme(ColorScheme.CreateColorSchemeFromColor(userOptions.Theme.BaseColor), userOptions.ColorScheme.UseDarkScheme, userOptions.ColorScheme.AccentTitle, userOptions.ColorScheme.AccentBar);
            Current = userOptions;
            DesktopScreen.SetRunningAppButtonWidth(userOptions.HideAppTooltips);
        }

        public void PrepareDirectory()
        {
            CreateIfIsntExsits(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}"));
            CreateIfIsntExsits(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\AppData"));
            CreateIfIsntExsits(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\AppData\\JVOS"));
            CreateIfIsntExsits(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\MenuItems"));
            CreateIfIsntExsits(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\Desktop"));

            UserDirectory = PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}");
            MenuDirectory = PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\MenuItems");
        }

        public void SetDesktopImage(Bitmap bitmap)
        {
            bitmap.Save(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\AppData\\JVOS\\desktop.png"));
            ApplyDesktopImage(bitmap);
        }

        public void SetLockscreenImage(Bitmap bitmap)
        {
            bitmap.Save(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\AppData\\JVOS\\lockscreen.png"));
            ApplyLockscreenImage(bitmap);
        }

        public void SetProfileImage(Bitmap bitmap)
        {
            bitmap.Save(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\AppData\\JVOS\\profile.png"));
            ApplyProfileImage(bitmap);
        }

        private void ApplyDesktopImage(Bitmap bitmap)
        {
            App.Current.Resources["DesktopImage"] = bitmap;
        }

        private void ApplyLockscreenImage(Bitmap bitmap)
        {
            App.Current.Resources["LockImage"] = bitmap;
        }

        private void ApplyProfileImage(Bitmap bitmap)
        {
            App.Current.Resources["ProfileImage"] = bitmap;
        }

        public T GetUserValue<T>(string name, T defaultValue)
        {
            if (!_userValues.ContainsKey("name"))
                return defaultValue;
            return (T)_userValues[name];
        }

        public void SetUserValue(string name, object value)
        {
            _userValues[name] = value;
            SaveUser();
        }

        public void SaveColorScheme()
        {

        }

        static UserOptions()
        {
            UsersLocation = PlatformSpecifixController.GetLocalFilePath("Users");
            Users = new List<UserOptions>();
            Load();
            

            if(Users.Count == 0)
            {
                Users.Add(new UserOptions());
                Save();
                if (!Directory.Exists("Applications/JVOS.OOBE"))
                    Communicator.ShowMessageDialog(new MessageDialog("Problem", "OOBE dosen't installed, \nplease install it first."));
                else
                    AppCommunicator.OpenApplication("Applications/JVOS.OOBE", "oobe");
            }
        }

        protected UserOptions() : this("Undefined", "")
        {

        }

        public UserOptions(string username, string password)
        {
            _userValues = new();
            Username = username;
            Password = password;
            JsonPath = Path.Combine(UsersLocation, $"{username}.jvon");
            ColorScheme = new ColorScheme();
            int i = 1;
            while (File.Exists(JsonPath))
            {
                i++;
                JsonPath = Path.Combine(UsersLocation, $"{username} ({i}).jvon");
            }
            PrepareDirectory();
            RecentManager = new RecentFilesManager(GetPath("AppData\\JVOS\\recents.json"));
        }

        public static void Test()
        {

        }

        private static void Load()
        {
            if (!Directory.Exists(UsersLocation))
            {
                LoadDefaults();
                return;
            }
            Users.Clear();
            string[] files = Directory.GetFiles(UsersLocation);
            foreach(string file in files) {
                var user = LoadUser(file);
                if(user != null)
                    Users.Add(user);
            }
        }

        private static void LoadDefaults()
        {
            Users.Clear();
            Users.Add(new UserOptions());
            Directory.CreateDirectory(UsersLocation);
            Save();
        }

        public static void Save()
        {
            foreach (UserOptions user in Users)
                if(user.JsonPath != null)
                    user.SaveUser(user.JsonPath);
        }

        /// <summary>
        /// Loads user options from JVON (json) file
        /// </summary>
        /// <param name="filename">JVON file path</param>
        /// <returns></returns>
        public static UserOptions? LoadUser(string filename) {
            var user = JsonConvert.DeserializeObject<UserOptions>(File.ReadAllText(filename));
            if (user == null)
                return null;
            user.JsonPath = filename;
            return user;
        }

        public override bool Equals(object? obj)
        {
            var objct = obj as UserOptions;
            if (objct == null)
                return false;
            return
                (Username == objct.Username) && (JsonPath == objct.JsonPath);
        }

        /// <summary>
        /// Saves user options to JVON (json) file
        /// </summary>
        /// <param name="filename">JVON file path</param>
        /// <returns></returns>
        public void SaveUser(string? filename = null)
        {
            filename = filename ?? this._jsonpath;
            File.WriteAllText(filename, JsonConvert.SerializeObject(this));
        }

        public string GetPath(string v)
        {
            return UserDirectory + "\\" + v;
        }

        public string GetShortPath(string v)
        {
            return UserDirectory.Replace(Directory.GetCurrentDirectory(), "") + "\\" + v;
        }

        internal void CreateProgramShortcuts()
        {
            string path = GetPath("AppData\\StartMenu\\All");
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            foreach(var dir in Directory.GetDirectories(PlatformSpecifixController.GetLocalFilePath("Applications")))
            {
                var f = $"{dir}\\manifest.json";
                if (File.Exists(f))
                {
                    AppManifest? manifest = JsonConvert.DeserializeObject<AppManifest>(f);
                    if (manifest == null)
                        continue;
                    if(manifest.Activities.Length == 1)
                    {
                        File.WriteAllText(path + "\\" + manifest.Name + ".jnk", JsonConvert.SerializeObject(new Shortcut($"app://Applications\\{dir.Split('\\').Where(x => !String.IsNullOrEmpty(x)).Last()} {manifest.Activities[0]}", "", "")));
                    }
                    else
                    {
                        foreach(var activity in manifest.Activities)
                        {
                            File.WriteAllText(path + "\\" + manifest.Name + " [" + activity  + "].jnk", JsonConvert.SerializeObject(new Shortcut($"app://Applications\\{dir.Split('\\').Where(x => !String.IsNullOrEmpty(x)).Last()} {activity}", "", "")));
                        }
                    }
                }
            }
            File.WriteAllText($"{path}\\Ease of Access.jnk", JsonConvert.SerializeObject(new Shortcut("shell://embeded JVOS.EmbededWindows.EaseOfAccess", ImageToBase64(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Lockscreen/easeofaccess.png")))), "Access for everyone!")));
            File.WriteAllText($"{path}\\Files.jnk", JsonConvert.SerializeObject(new Shortcut("shell://embeded JVOS.EmbededWindows.FileBrowser", ImageToBase64(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/folder.png")))), "With ZHABA Support!")));
            File.WriteAllText($"{path}\\Run.jnk", JsonConvert.SerializeObject(new Shortcut("shell://embeded JVOS.EmbededWindows.RunDialog", ImageToBase64(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/run.png")))), "AMOGUS")));
            File.WriteAllText($"{path}\\Settings.jnk", JsonConvert.SerializeObject(new Shortcut("shell://embeded JVOS.EmbededWindows.Preferences.PreferencesHub", ImageToBase64(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/preferences.png")))), "PERSOANALIZE JOPA LIKE GOD!")));
            File.WriteAllText($"{path}\\Task Manager.jnk", JsonConvert.SerializeObject(new Shortcut("shell://embeded JVOS.EmbededWindows.TaskManager.TaskManager", ImageToBase64(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/taskmgr.png")))), "With ZHABA Support!")));
        }

        public string? Username {
            get => _username;
            set => _username = value;
        }
        
        public string? Password {
            get => _password;
            set => _password = value;
        }

        public string? DesktopImage {
            get => _desktop;
            set => _desktop = value;
        }

        public string? Base64Avatar {
            get => _base64avatar;
            set => _base64avatar = value;
        }

        public bool HideAppTooltips
        {
            get => _hideTooltips;
            set
            {
                _hideTooltips = value;
                Save();
            }
        }

        [JsonIgnore]
        public string? JsonPath {
            get => _jsonpath;
            private set => _jsonpath = value;
        }

        [JsonIgnore]
        public ColorScheme? ColorScheme
        {
            get => _colorScheme;
            set => _colorScheme = value;
        }

        private ColorScheme? _colorScheme;
        private string? _username;
        private string? _password;
        private string? _base64avatar;
        private string? _desktop;
        private bool _hideTooltips = true;
        [JsonIgnore]
        private string? _jsonpath;
    }

    public class RecentFilesManager
    {

        public RecentFilesManager(string jsonPath)
        {
            JsonPath = jsonPath;
            Recents = File.Exists(jsonPath) ? JsonConvert.DeserializeObject<List<Recent>>(File.ReadAllText(jsonPath))??new List<Recent>() : new List<Recent>();
        }

        public void AddRecent(string path)
        {
            Recents.RemoveMany(Recents.Where(x => x.Path == path));
            while(Recents.Count > 100)
                Recents.RemoveAt(new Random().Next(0, Recents.Count));
            Recents.Add(new Recent(path, path.Split().Last()));
            File.WriteAllText(JsonPath, JsonConvert.SerializeObject(Recents));
        }

        public void RemoveRecent(string path)
        {
            Recents.RemoveMany(Recents.Where(x => x.Path == path));
            File.WriteAllText(JsonPath, JsonConvert.SerializeObject(Recents));
        }

        private string JsonPath;
        public List<Recent> Recents;
    }
}
