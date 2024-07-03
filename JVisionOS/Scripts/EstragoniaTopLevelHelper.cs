using Avalonia.Input.Raw;
using Avalonia.Input;
using Avalonia;
using Godot;
using JLeb.Estragonia.Input;
using JLeb.Estragonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Avalonia.Remote.Protocol.Input;
using GDMouseButton = Godot.MouseButton;
using System.Reactive;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Platform;

namespace JVisionOS.Scripts
{
    public static class EstragoniaTopLevelHelper
    {
        static Type? GodotTopLevelImplType = Assembly.GetAssembly(typeof(GodotTopLevel))?.GetType("JLeb.Estragonia.GodotTopLevelImpl");

        public static void SimulateMouseMove(AvaloniaControl ui, Godot.InputEventMouseMotion motion, Avalonia.Point pos)
        {
            motion.Position = new Vector2(0, 0);
            var toplevel = ui.GetTopLevel();
            GodotTopLevel topLevel = ui.GetTopLevel();
            var action = (Action<RawInputEventArgs>?)GodotTopLevelImplType?.GetMethod("get_Input")?.Invoke(topLevel.PlatformImpl, new object[0]);
            if (action is not { } input)
                return;
            var args = (RawPointerEventArgs?)typeof(RawPointerEventArgs).GetConstructor(new Type[] {
                typeof(IInputDevice),
                                typeof(ulong),
                                typeof(IInputRoot),
                                typeof(RawPointerEventType),
                                typeof(Point),
                                typeof(RawInputModifiers)
            }
            )?.Invoke(
                new object[]
                {
                    GodotDevices.GetMouse(motion.Device),
                    Time.GetTicksMsec(),
                    topLevel,
                    RawPointerEventType.Move,
                    pos,
                    RawInputModifiers.None
                }
            );

            input(args);
        }

        public static void SimulateMouseAction(AvaloniaControl ui, Godot.InputEventMouseButton @event, Avalonia.Point pos)
        {
            GodotTopLevel topLevel = ui.GetTopLevel();
            var action = (Action<RawInputEventArgs>?)GodotTopLevelImplType?.GetMethod("get_Input")?.Invoke(topLevel.PlatformImpl, new object[0]);
            if (action is not { } input)
                return;
            RawPointerEventArgs? CreateButtonArgs(RawPointerEventType type)
                => (RawPointerEventArgs?)(typeof(RawPointerEventArgs).GetConstructor(new Type[]
                {
                                typeof(IInputDevice),
                                typeof(ulong),
                                typeof(IInputRoot),
                                typeof(RawPointerEventType),
                                typeof(Point),
                                typeof(RawInputModifiers)
                })?.Invoke(new object[]
                {
                                JLeb.Estragonia.Input.GodotDevices.GetMouse(@event.Device),
                                (ulong)DateTime.Now.ToBinary(),
                                topLevel,
                                type,
                                pos,
                                RawInputModifiers.None
                }));

            var args = (@event.ButtonIndex, @event.Pressed) switch
            {
                (GDMouseButton.Left, true) => CreateButtonArgs(RawPointerEventType.LeftButtonDown),
                (GDMouseButton.Left, false) => CreateButtonArgs(RawPointerEventType.LeftButtonUp),
                (GDMouseButton.Right, true) => CreateButtonArgs(RawPointerEventType.RightButtonDown),
                (GDMouseButton.Right, false) => CreateButtonArgs(RawPointerEventType.RightButtonUp),
                (GDMouseButton.Middle, true) => CreateButtonArgs(RawPointerEventType.MiddleButtonDown),
                (GDMouseButton.Middle, false) => CreateButtonArgs(RawPointerEventType.MiddleButtonUp),
                (GDMouseButton.Xbutton1, true) => CreateButtonArgs(RawPointerEventType.XButton1Down),
                (GDMouseButton.Xbutton1, false) => CreateButtonArgs(RawPointerEventType.XButton1Up),
                (GDMouseButton.Xbutton2, true) => CreateButtonArgs(RawPointerEventType.XButton2Down),
                (GDMouseButton.Xbutton2, false) => CreateButtonArgs(RawPointerEventType.XButton2Up),
                _ => null
            };

            input.Invoke(args);
        }

        private static bool IsRawPointerEventArgsHandled(RawPointerEventArgs args)
        {
            return (bool)typeof(RawPointerEventArgs).GetMethods().Where(x=>x.Name=="get_Handled").First().Invoke(args, new object[0]);
        }

        private static RawPointerPoint CreateRawPointerPoint(Point p, float pressure, Vector2 tilt)
        {
            RawPointerPoint rw = new();
            typeof(RawPointerPoint).GetField("Position")?.SetValue(rw,p);
            typeof(RawPointerPoint).GetField("Twist")?.SetValue(.0f,p);
            typeof(RawPointerPoint).GetField("Pressure")?.SetValue(pressure,p);
            typeof(RawPointerPoint).GetField("XTilt")?.SetValue(tilt.X * 90f, p);
            typeof(RawPointerPoint).GetField("YTilt")?.SetValue(tilt.Y * 90f,p);
            return rw;
        }
    }
}
