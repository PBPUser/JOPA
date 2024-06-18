using Avalonia.Controls;
using Avalonia.Interactivity;
using JVOS.Controls;
using JVOS.ApplicationAPI;
using JVOS.PanoramaBar;
using System;
using JVOS.ApplicationAPI.Hub;

namespace JVOS.Hubs
{
    public partial class PanoramaHub : HubWindow
    {
        public PanoramaHub()
        {
            InitializeComponent();
            refresh.Content = "<";
            refresh.Click += (a, b) =>
            {
                Refresh();
            };
            Refresh();
        }

        PanoramaDateParser.Topic 
            CurrentTopic;

        DateTime
            CurrentDate;

        int
            CurrentPage;

        bool
            IsBasicPageLoaded = true,
            IsDatesPageLoaded = true;

        public void Refresh()
        {
            CurrentDate = DateTime.Now;
            CurrentPage = 1;
            NextDate();
            stackContent.Children.Clear();
        }

        private Plate CreatePlate(PanoramaSite.Section Section, string topic)
        {
            Plate plate = new Plate() { Margin = new Avalonia.Thickness(16), Width = 256 };
            plate.LoadInfo(Section, topic);
            return plate;
        }

        public Border CreateClayButton(string Content, bool UseIconFont = false, System.EventHandler<RoutedEventArgs>? Click = null)
        {
            Button b = new Button() { Content = Content };
            if (UseIconFont)
                b.FontFamily = new Avalonia.Media.FontFamily("Jcons");
            if(Click != null)
                b.Click += Click;
            Border border = new Border() { CornerRadius = new Avalonia.CornerRadius(16), Height = 32, Width = 100, Child = b };
            return border;
        }

        public void NextDate(PanoramaDateParser.Topic? Topic = null)
        {
            bool TopicIsNull = Topic == null;
            if(Topic == null)
            {
                int x = ((int)CurrentTopic) + 1;
                if(x == 4)
                {
                    CurrentDate = CurrentDate.AddDays(-1);
                    CurrentPage++;
                    x = 0;
                }
                Topic = (PanoramaDateParser.Topic)(x);
                CurrentTopic = Topic.Value;
            }
            else if (Topic == PanoramaDateParser.Topic.politics || Topic == PanoramaDateParser.Topic.society)
                CurrentDate = CurrentDate.AddDays(-1);
            else
                CurrentPage++;
            int j = 0;
            PanoramaDateParser parser = new PanoramaDateParser(CurrentDate, CurrentPage, Topic.Value);
            parser.Loaded += (a, b) =>
            {
                Grid g = new Grid() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center };
                g.ColumnDefinitions.Add(new ColumnDefinition());
                g.ColumnDefinitions.Add(new ColumnDefinition());
                StackPanel stack1 = new StackPanel();
                StackPanel stack2 = new StackPanel();
                g.Children.Add(stack1);
                g.Children.Add(stack2);
                Grid.SetColumn(stack2, 1);
                stackContent.Children.Add(g);
                foreach (var section in parser.Sections)
                {
                    if (j % 2 == 0)
                        stack1.Children.Add(CreatePlate(section, Topic.Value.ToString()));
                    else
                        stack2.Children.Add(CreatePlate(section, Topic.Value.ToString()));
                    j++;
                }
                IsDatesPageLoaded = true;
            };
            IsDatesPageLoaded = false;
            parser.Load();
        }
    }
}
