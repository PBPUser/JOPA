using Avalonia.Platform;
using JVOS.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVOS.EmbededWindows.Preferences
{
    public class DesktopSettingsVM : ViewModelBase
    {
        public DesktopSettingsVM()
        {
            LoadIncluded();
        }

        void LoadIncluded()
        {
            IncludedWallpapers = AssetLoader.GetAssets(new("avares://JVOS/Assets/Wallpapers"), null).ToList();
        }

        private List<Uri> includedWallpapers = new();
        public List<Uri> IncludedWallpapers
        {
            get => includedWallpapers;
            set => this.RaiseAndSetIfChanged(ref includedWallpapers, value);
        }
    }
}
