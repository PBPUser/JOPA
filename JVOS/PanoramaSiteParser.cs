using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Threading.Tasks.Sources;
using Avalonia.Styling;
using System.Xml.Linq;

namespace JVOS.PanoramaBar
{
    public class PanoramaSite
    {
        
        public event EventHandler<EventArgs>? Loaded;
        public event EventHandler<string>? LineWritten;
        StreamWriter writer;
        string unknownClasses = "";

        public string TitleMessage = "";
        public string TitleAuthor = "";
        public List<Section> TopSections = new List<Section>();
        public List<Section> SubSections = new List<Section>();

        public PanoramaSite()
        {
            writer = File.CreateText("html.txt");
        }

        public void Load()
        {
            new Thread(() =>
            {
                var http = new HttpClient();
                string httpStr = http.GetStringAsync("https://panorama.pub/").GetAwaiter().GetResult();
                File.WriteAllText("xui.txt", httpStr);
                var doc = new HtmlDocument() {  };
                
                doc.LoadHtml(httpStr);
                ScanNode(doc.DocumentNode, "");
                Dispatcher.UIThread.Invoke(() =>
                {
                    
                    OnLoaded(new EventArgs());
                });
                writer.Close();
                WriteLine(unknownClasses);
            }).Start();
        }

        public void ScanNode(HtmlNode node, string prefix)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Document:
                    foreach(var child in node.ChildNodes)
                        ScanNode(child, prefix);
                    break;
                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "div":
                            ScanDiv(node);
                            return;
                        case "a":
                            ScanA(node);
                            foreach (var child in node.ChildNodes)
                                ScanNode(child, prefix + "   ");
                            return;
                        case "img":
                        case "em":
                        case "span":
                        case "nav":
                        case "h1":
                        case "svg":
                        case "h2":
                        case "h3":
                        case "path":
                        case "ul":
                        case "meta":
                        case "footer":
                        case "source":
                        case "circle":
                        case "strong":
                        case "li":
                        case "script":
                        case "p":
                        case "head":
                        case "polyline":
                        case "link":
                        case "line":
                        case "picture":
                        case "html":
                        case "title":
                        case "body":
                            foreach (var child in node.ChildNodes)
                                ScanNode(child, prefix + "   ");
                            return;
                        default:
                            throw new Exception(node.Name);
                    }
                case HtmlNodeType.Text:
                    writer.WriteLine(node.InnerText);
                    return;
                case HtmlNodeType.Comment:
                    return;
            }
        }

        public void ScanA(HtmlNode node)
        {
            foreach(var attr in node.Attributes)
            {
                if(attr.Name == "class")
                {
                    switch(attr.Value)
                    {
                        case "bg-cover bg-center bg-no-repeat min-h-[380px] rounded-md select-none relative":
                            string title = "";
                            foreach (var xnode in node.ChildNodes)
                                if (xnode.Name == "div")
                                    title = xnode.InnerText;
                            string image = "";
                            foreach(var xattr in node.Attributes)
                                if(xattr.Name == "data-bg-image-jpeg")
                                    image = xattr.Value;
                            WriteLine(title.Remove(0, 29));
                            WriteLine(image);
                            TopSections.Add(new Section(title.Remove(0, 29), image, ""));
                            break;
                        case "flex flex-col rounded-md hover:text-secondary hover:bg-accent/[.1] mb-2 items-center p-2":
                            string title1 = "", image1 = "";
                            foreach (var xnode in node.ChildNodes)
                                if (string.Join(' ',xnode.GetClasses()) == "pt-2 text-xl font-semibold text-center")
                                {
                                    foreach(var xxnode in xnode.ChildNodes)
                                    {
                                        if (xxnode.Name == "#text")
                                            title1 = xxnode.InnerText.Remove(0, 29);
                                    }
                                }
                                else if(xnode.Name == "div")
                                    foreach (var xxnode in xnode.ChildNodes)
                                        if (xxnode.Name == "picture")
                                            foreach (var xxxnode in xxnode.ChildNodes)
                                                if (xxxnode.Name == "img")
                                                    foreach(var xattr in xxxnode.Attributes)
                                                        if(xattr.Name == "src")
                                                            image1 = xattr.Value;
                            
                            WriteLine(title1);
                            WriteLine(image1);
                            SubSections.Add(new Section(title1, image1, ""));
                            break;
                    }
                }
            }
            foreach (var child in node.ChildNodes)
                ScanNode(child, "   ");
        }

        public void ScanDiv(HtmlNode node)
        {
            foreach(var attr in node.Attributes)
            {
                switch (attr.Name)
                {
                    case "class":
                        switch (attr.Value)
                        {
                            case "text-xs px-1.5":
                                TitleMessage = node.ChildNodes[0].InnerText;
                                WriteLine(TitleMessage);
                                TitleAuthor = node.ChildNodes[1].InnerText;
                                return;
                        }
                        break;
                }
            }
            foreach (var child in node.ChildNodes)
                 ScanNode(child, "   ");


        }

        private void OnLoaded(EventArgs e) {
            Loaded!(this, e);
        }

        private void WriteLine(string text)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                LineWritten?.Invoke(this, text);
            });
        }

        public struct Section
        {
            public string Title;
            public string Image;
            public string Url;

            public Section(string title, string image, string url)
            {
                Title = title;
                Image = image;
                Url = url;
            }
        }
    }
}
