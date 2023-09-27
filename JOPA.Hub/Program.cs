using System;
using System.Diagnostics;

namespace JOPA.Hub;

public class Program
{
    public static void Main(string[] args)
    {
        string dirW = "D:\\Profile\\PCTS\\jvsans\\Symbols";
        string[] files = Directory.GetFiles(dirW);
        string dir = new DirectoryInfo(dirW) + "\\SVG";
        Directory.CreateDirectory(dir);
        foreach (string file in files)
        {
            FileInfo f = new FileInfo(file);
            string i = $"{dir}\\{f.Name}.svg";
            RunCmd($"D:\\PortableApps\\InkscapePortable\\App\\Inkscape\\bin\\inkscape.exe --actions=\"select-all;selection-trace:1,false,true,true,1,1.0,0.20;export-filename:{i};export-do;\" \"{file}\" --batch-process");
            Console.WriteLine($"{file} => {i}");
        }
    }

    public static void RunCmd(string cmd)
    {
        var x = Process.Start("cmd", $"/c {cmd}");
        x.WaitForExit();
    }
}