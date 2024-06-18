using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using JVOS.ApplicationAPI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace JVOS
{
    public static class WindowGridController
    {
        private static Subject<double> hStretchSource = new Subject<double>();
        private static Subject<double> hLeftSource = new Subject<double>();
        private static Subject<double> hRightSource = new Subject<double>();
        private static Subject<double> vStretchSource = new Subject<double>();
        private static Subject<double> vTopSource = new Subject<double>();
        private static Subject<double> vBottomSource = new Subject<double>();

        private static double hStretch = 2;
        private static double vStretch = 2;
        private static double hLeft = 2;
        private static double vTop = 2;
        private static double hRight = 2;
        private static double vBottom = 2;


        static WindowGridController()
        {
            hStretchSource.OnNext(hStretch);
            hLeftSource.OnNext(hLeft);
            hRightSource.OnNext(hRight);
            vStretchSource.OnNext(vStretch);
            vTopSource.OnNext(vTop);
            vBottomSource.OnNext(vBottom);
        }

        public static void UpdateSubjectsHWhenDesktopResized(double newWidth, double newHeight)
        {
            double
                xp = newWidth / hStretch,
                yp = newHeight / vStretch;
            UpdateHStretch(newWidth);
            UpdateVStretch(newHeight);
            UpdateHLeft(hLeft * xp);
            UpdateHRight(hRight * xp);
            UpdateVTop(vTop * yp);
            UpdateVBottom(vBottom * yp);
        }

        public static void UpdateHSubjectsWhenWindowResized(double value, bool right)
        {
            if (right)
                value = hStretch - value;
            double rightValue = hStretch - value;
            UpdateHLeft(value);
            UpdateHRight(rightValue);
        }

        public static void UpdateVSubjectsWhenWindowResized(double value, bool bottom)
        {
            if (bottom)
                value = vStretch - value;
            double bottomValue = vStretch - value;
            UpdateVTop(value);
            UpdateVBottom(bottomValue);
        }

        private static void RefreshHorizontal()
        {
            UpdateHStretch(hStretch);
            UpdateHLeft(hLeft);
            UpdateHRight(hRight);
        }

        private static void RefreshVertical()
        {
            UpdateVStretch(vStretch);
            UpdateVTop(vTop);
            UpdateVBottom(vBottom);
        }

        public static (IDisposable, IDisposable?) SubscribeToHorizontal(SystemWindowFrame window, HorizontalAlignment? align)
        {
            var binding = window.Bind(SystemWindowFrame.WidthProperty, ToHSubject(align));
            IDisposable? binding2 = null;
            if (align == HorizontalAlignment.Right)
                binding2 = window.WindowTranslateGrid.Bind(TranslateTransform.XProperty, ToHSubject(HorizontalAlignment.Left));
            RefreshHorizontal();
            return (binding, binding2);
        }
        public static (IDisposable, IDisposable?) SubscribeToVertical(SystemWindowFrame window, VerticalAlignment? align)
        {
            var binding = window.Bind(SystemWindowFrame.HeightProperty, ToVSubject(align));
            IDisposable? binding2 = null;
            if(align == VerticalAlignment.Bottom)
                binding2 = window.WindowTranslateGrid.Bind(TranslateTransform.YProperty, ToVSubject(VerticalAlignment.Top));
            RefreshVertical();
            return (binding, binding2);
        }

        public static void Unsubscribe((IDisposable, IDisposable?) bindings)
        {
            bindings.Item1.Dispose();
            bindings.Item2?.Dispose();
        }

        private static Subject<double>? ToHSubject(HorizontalAlignment? align) => align switch
        {
            HorizontalAlignment.Stretch => hStretchSource,
            HorizontalAlignment.Left => hLeftSource,
            HorizontalAlignment.Right => hRightSource,
            _ => null
        };

        private static Subject<double>? ToVSubject(VerticalAlignment? align) => align switch
        {
            VerticalAlignment.Stretch => vStretchSource,
            VerticalAlignment.Top => vTopSource,
            VerticalAlignment.Bottom => vBottomSource,
            _ => null
        };

        private static void UpdateHStretch(double newValue)
        {
            hStretch = newValue;
            hStretchSource.OnNext(newValue);
        }

        private static void UpdateVStretch(double newValue)
        {
            vStretch = newValue;
            vStretchSource.OnNext(newValue);
        }

        private static void UpdateHLeft(double newValue)
        {
            hLeft = newValue;
            hLeftSource.OnNext(newValue);
        }

        private static void UpdateHRight(double newValue)
        {
            hRight = newValue;
            hRightSource.OnNext(newValue);
        }

        private static void UpdateVTop(double newValue)
        {
            vTop = newValue;
            vTopSource.OnNext(newValue);
        }

        private static void UpdateVBottom(double newValue)
        {
            vBottom = newValue;
            vBottomSource.OnNext(newValue);
        }
    }
}
