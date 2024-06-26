using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DynamicData;
using JVOS.ApplicationAPI;
using JVOS.ApplicationAPI.Windows;
using JVOS.Controls;
using JVOS.Controls.WidgetContainer;
using JVOS.DataModel;
using JVOS.Views;
using JVOS.Widgets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;

namespace JVOS.EmbededWindows
{
    public partial class DesktopWindow : WindowContentBase
    {
        public static DesktopWindow Current;
        public DesktopVM VM;
        public DesktopIconPlacementHelper IconPlacementHelper;


        static FuncDataTemplate<string> DesktopItemDataTemplate = new((value, namescope) =>
        {
            if (value.EndsWith(".jwi"))
            {
                var c = JsonConvert.DeserializeObject<DesktopWidget>(File.ReadAllText(value));
                if(c != null)
                {
                    var j = WidgetManager.GetWidget(c); 
                    if(j != null)
                    {
                        j.Width = c.Size.Width;
                        j.Height = c.Size.Height;
                        var container = new WidgetContainer(j, value, c);
                        container.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                        container.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
                        return container;
                    }
                }
            }
            return new DesktopIcon(value);
        });

        public DesktopWindow()
        {
            InitializeComponent();
            Loaded += (a, b) =>
            {
                Frame.SetFrameVisibility(false);
                Frame.ChangeState(WindowFrameState.Maximized);
            };
            DataContext = VM = new DesktopVM();
            Current = this;
            lbDesktop.ItemTemplate = DesktopItemDataTemplate;
            var p = UserSession.CurrentUser.DesktopScreen;
            IconPlacementHelper = new(UserOptions.Current.GetPath("Desktop"), new Size((int)(p.Bounds.Width / 96), (int)(p.Bounds.Height / 104)));
        }

        public override bool AllowBringOnTop => false;
        public override bool ShowOnTaskbar => false;

        public class DesktopIconPlacementHelper
        {
            string filename;
            Size resolution;

            public DesktopIconPlacementHelper(string directory, Size resolution) {
                filename =  directory + "\\desktop.json";
                this.resolution = resolution;
                if (File.Exists(filename))
                    Placements = JsonConvert.DeserializeObject<List<DesktopItemPlacement>>(File.ReadAllText(filename)) ?? new();
            }

            List<DesktopItemPlacement> Placements = new();

            public void Sort(List<string> elements, Comparer<string> comparer)
            {
                elements.Sort(comparer);
                Placements.Clear();
                foreach (var x in elements)
                    SetPlacementForItem(GetFreePosition(), x);
            }

            public void SetPlacementForItem(Point newPlacement, string path)
            {
                var i = Placements.IndexOf(Placements.Where(x => x.Path == path).FirstOrDefault());
                if(i > -1)
                    Placements.RemoveAt(i);
                Placements.Add(new DesktopItemPlacement()
                {
                    Path = path,
                    Position = newPlacement
                });
                Save();
            }

            private Point GetFreePosition()
            {
                Point s = new(0, 0);
                while (true)
                {
                    var sh = Placements.Where(x => x.Position == s);
                    if (sh.Count() == 0)
                        return s;
                    if (!File.Exists(sh.First().Path) && !Directory.Exists(sh.First().Path))
                    {
                        Placements.Remove(sh.First());
                        Save();
                        return s;
                    }    
                    if (s.Y == resolution.Height)
                    {
                        s.Y = 0;
                        s.X++;
                    }
                    else
                        s.Y++;
                }
            }

            void Save()
            {
                File.WriteAllText(filename, JsonConvert.SerializeObject(Placements));
            }

            public Point GetPlacementForItem(string path)
            {
                var x = Placements.Where(x => x.Path == path).FirstOrDefault();
                if(x.Path == null)
                {
                    var s = new DesktopItemPlacement()
                    {
                        Path = path,
                        Position = GetFreePosition()
                    };
                    Placements.Add(s);
                    Save();
                    return s.Position;
                }
                return x.Position;
            }

            struct DesktopItemPlacement
            {
                public DesktopItemPlacement()
                {

                }

                public string Path = "";
                public Point Position = new(-1, -1);
            }
        }
    }
}
