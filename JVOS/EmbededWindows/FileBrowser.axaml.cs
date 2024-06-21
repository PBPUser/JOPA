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
using JVOS.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace JVOS.EmbededWindows
{
    public partial class FileBrowser : WindowContentBase
    {
        FilesVM VM;
        
        public enum SelectMode
        {
            None,
            File,
            Directory
        }

        SelectMode Mode = SelectMode.None;

        static FuncDataTemplate<string> FileItemDataTemplate = new((value, namescope) =>
        {
            FileIcon icon = new FileIcon(value)
            {
                Height = 32
            };
            return icon;
        });

        public override void Closed()
        {
            AfterBrowse?.Invoke(new DialogFileSystemBrowsingResult(false, new string[] { }));
            base.Closed();
        }

        Action<DialogFileSystemBrowsingResult>? AfterBrowse;

        public FileBrowser(SelectMode selectMode = SelectMode.None, Action<DialogFileSystemBrowsingResult>? afterBrowse = null, string filter = "*", string path = "")
        {
            InitializeComponent();
            Mode = selectMode;
            AfterBrowse = afterBrowse;
            if(afterBrowse != null)
            {
                ok.Click += (a, b) =>
                {
                    Frame.Close();
                    afterBrowse.Invoke(new DialogFileSystemBrowsingResult(true, new string[] { VM.Elements[listFiles.SelectedIndex] }));
                };
                cancel.Click += (a, b) =>
                {
                    Frame.Close();
                    afterBrowse.Invoke(new DialogFileSystemBrowsingResult(false, new string[] { }));
                };
                BrowsePanel.IsVisible = true;
            }
            gobtn.Click += (a, b) => VM.GoToDir(tb.Text);
            upbtn.Click += (a, b) =>
            {
                var txsp = tb.Text.Replace("/", "\\").Split("\\").ToList();
                txsp.Remove(""); 
                if (txsp.Count == 1)
                {
                    return;
                }
                txsp.RemoveAt(txsp.Count - 1);
                VM.GoToDir(String.Join("\\", txsp));
            };
            Icon = new Bitmap(AssetLoader.Open(new Uri("avares://JVOS/Assets/Shell/folder.png")));
            Title = "Files";
            listTree.Tapped += TreeTap;
            listFiles.DoubleTapped += Tap;
            listFiles.ItemTemplate = listTree.ItemTemplate = FileItemDataTemplate;
            DataContext = VM = new FilesVM(path, filter);
        }

        private void TreeTap(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (listTree.SelectedIndex == -1)
                return;
            if (listTree.SelectedIndex >= VM.Tree.Count())
                return;
            VM.GoToDir(VM.Tree[listTree.SelectedIndex]);
        }

        private void Tap(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (listFiles.SelectedIndex == -1)
                return;
            if (listFiles.SelectedIndex >= VM.Elements.Count())
                return;
            string dir = VM.Elements[listFiles.SelectedIndex];
            if (Directory.Exists(dir))
            {
                Debug.WriteLine("Directory exists");
                VM.GoToDir(dir);
            }
            else if (File.Exists(dir))
            {
                Debug.WriteLine("File exists");
                switch (Mode)
                {
                    case SelectMode.None: Communicator.RunPath(dir); break;
                    case SelectMode.File: Communicator.RunPath(dir); break;
                }
            }
            else
            {

                Debug.WriteLine(dir);
            }
        }
    }
}
