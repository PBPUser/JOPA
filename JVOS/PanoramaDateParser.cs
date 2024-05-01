using Avalonia.Threading;
using HtmlAgilityPack;
using JVOS.ApplicationAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static JVOS.PanoramaBar.PanoramaSite;

namespace JVOS.PanoramaBar
{
    public class PanoramaDateParser
    {
        private DateTime _date;
        private string _url;
        private int _page;
        private Topic topic;

        public event EventHandler<EventArgs>? Loaded;
        public event EventHandler<string>? LineWritten;
        StreamWriter writer;
        string unknownClasses = "";

        public List<Section> Sections = new List<Section>();

        public enum Topic
        {
            politics,
            society,
            science,
            economics
        }

        public PanoramaDateParser(DateTime? date = null, int page = 1, Topic topic = Topic.politics)
        {
            _date = date == null ? DateTime.MinValue : date.Value;
            this.topic = topic;
            this._page = page;
            this._url = GenUrl();
        }

        private string GenUrl()
        {
            if (topic == Topic.politics || topic == Topic.society)
                return $"https://panorama.pub/{topic}/{_date.Day.ToString("00")}-{_date.Month.ToString("00")}-{_date.Year}";
            return $"https://panorama.pub/{topic}?page={_page}";
        }

        public void Load()
        {
            new Thread(() =>
            {
                var http = new HttpClient();

                string httpStr;
                try
                {
                    httpStr = http.GetStringAsync(_url).GetAwaiter().GetResult();
                }
                catch
                {
                    goto loaded;
                }

                if (!Directory.Exists(PlatformSpecifixController.GetLocalFilePath("debug")))
                    Directory.CreateDirectory(PlatformSpecifixController.GetLocalFilePath("debug"));
                string debugLoc = PlatformSpecifixController.GetLocalFilePath("debug/PanoramaDateTest.txt");

                File.WriteAllText(debugLoc, httpStr);
                var doc = new HtmlDocument() { };
                writer = File.CreateText(debugLoc);
                doc.LoadHtml(httpStr);
                ScanNode(doc.DocumentNode, "");
                writer.Close();
            loaded:
                Dispatcher.UIThread.Invoke(() =>
                {
                    OnLoaded(new EventArgs());
                });
                WriteLine(unknownClasses);
            }).Start();
        }

        public void ScanNode(HtmlNode node, string prefix)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Document:
                    foreach (var child in node.ChildNodes)
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
            string title = "";
            string image = "";
            string url = "none";
            bool isSection = false;
            foreach (var attr in node.Attributes)
            {
                switch(attr.Name)
                {
                    case "class":
                        if(attr.Value == "flex flex-col rounded-md hover:text-secondary hover:bg-accent/[.1] mb-2")
                        {
                            isSection = true;
                            foreach(var childNode in node.ChildNodes)
                            {
                                if (childNode.Name != "div")
                                    continue;
                                bool hasClass = false;
                                foreach(var childAttrib in childNode.Attributes)
                                    if (childAttrib.Name == "class")
                                    {
                                        hasClass = true;
                                        if (childAttrib.Value.Equals("pt-2 text-xl lg:text-lg xl:text-base text-center font-semibold"))
                                            title = childNode.InnerText;
                                    }
                                if (!hasClass)
                                    foreach (var subChildNode in childNode.ChildNodes)
                                    {
                                        if (subChildNode.Name != "img")
                                            continue;
                                        foreach (var subChildAttrib in subChildNode.Attributes)
                                            if (subChildAttrib.Name == "src")
                                                image = subChildAttrib.Value;
                                    }
                            }
                        }
                        break;
                    case "href":
                        url = attr.Value;
                        break;
                }
            }
            if(isSection)
                this.Sections.Add(new Section(title, image, url));
            foreach (var child in node.ChildNodes)
                ScanNode(child, "   ");
        }

        public void ScanDiv(HtmlNode node)
        {
            foreach (var attr in node.Attributes)
            {
                switch (attr.Name)
                {
                    case "class":
                        switch (attr.Value)
                        {
                            
                        }
                        break;
                }
            }
            foreach (var child in node.ChildNodes)
                ScanNode(child, "   ");


        }

        private void OnLoaded(EventArgs e)
        {
            if (Loaded != null)
                Loaded!(this, e);
        }

        private void WriteLine(string text)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                LineWritten?.Invoke(this, text);
            });
        }
    }
}
