using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JVOS.ApplicationAPI;
using JVOS.Screens;
using JVOS.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS
{
    public class UserOptions
    {
        static string UsersLocation;
        public static UserOptions? Current;
        public static List<UserOptions> Users;
        public Theme Theme;
        [JsonIgnore]
        public Bitmap? DesktopBitmap;
        [JsonIgnore]
        public string MenuDirectory;
        [JsonIgnore]
        public string UserDirectory;

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
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Bitmap Base64ToImage(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    return new Bitmap(ms);
            }
            catch (Exception e)
            {
                App.SendNotification($"{e.Message}");
                App.SendNotification($"{e.StackTrace}");
                App.SendNotification($"{e.Source}");
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
            if (userOptions.DesktopImage != null)
                userOptions.DesktopBitmap = userOptions.DesktopImage.EndsWith("default://") ? LoadImage(userOptions.DesktopImage ?? "") : Base64ToImage(userOptions.DesktopImage ?? "");
            else
                userOptions.DesktopBitmap = Base64ToImage("");
            ColorScheme.ApplyScheme(userOptions.ColorScheme, userOptions.ColorScheme.UseDarkScheme, userOptions.ColorScheme.AccentTitle, userOptions.ColorScheme.AccentBar);
            Current = userOptions;
            UserOptions.Current.PrepareDirectory();
            DesktopScreen.SetRunningAppButtonWidth(userOptions.HideAppTooltips);
        }

        public void PrepareDirectory()
        {
            CreateIfIsntExsits(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}"));
            CreateIfIsntExsits(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\AppData"));
            CreateIfIsntExsits(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\MenuItems"));
            CreateIfIsntExsits(PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\Desktop"));

            UserDirectory = PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}");
            MenuDirectory = PlatformSpecifixController.GetLocalFilePath($"Users\\{Username}\\MenuItems");
        }

        public void SaveColorScheme()
        {

        }

        static UserOptions()
        {
            UsersLocation = PlatformSpecifixController.GetLocalFilePath("Users");
            Users = new List<UserOptions>();
            Load();
        }

        protected UserOptions()
        {
            var assets = AssetLoader.GetAssets(new Uri("avares://JVOS/Assets/default_avatar.png"), null);
            Username = "Undefined";
            Password = "";
            if (assets.Count() > 0)
                Base64Avatar = ImageToBase64(new Bitmap(assets.First().AbsolutePath));
            else
                Base64Avatar = "";
            
            DesktopImage = "";
            JsonPath = Path.Combine(UsersLocation, "Undefined.jvon");
            ColorScheme = new ColorScheme();
        }

        public UserOptions(string username, string password)
        {
            
            
            Username = username;
            Password = password;
            Base64Avatar = ImageToBase64(new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/default_avatar.png"))));
            int i = 1;
            DesktopImage = "";
            JsonPath = Path.Combine(UsersLocation, $"{username}.jvon");
            ColorScheme = new ColorScheme();
            while (File.Exists(JsonPath))
            {
                i++;
                JsonPath = Path.Combine(UsersLocation, $"{username} ({i}).jvon");
            }
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
        public void SaveUser(string filename)
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(this));
        }

        public void SetWallpaper(string path)
        {
            var bitmap = new Bitmap(path);
            DesktopImage = ImageToBase64(bitmap);
            DesktopBitmap = bitmap;
            if(DesktopScreen.CurrentDesktop != null)
                DesktopScreen.CurrentDesktop.SetBackground(DesktopBitmap, true);
            Save();
        }

        public string GetPath(string v)
        {
            return UserDirectory + "\\" + v;
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
}
