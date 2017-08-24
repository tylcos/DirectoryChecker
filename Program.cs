using System;
using System.Collections.Generic;
using System.IO;
using System.Security;  
using System.Runtime.InteropServices;

namespace IncrementalFileChecker
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Please input directory.");

            string[] files = new string[0];
            bool valid = false;
            while (!valid)
            {
                try
                {
                    files = Directory.GetFiles(Console.ReadLine());
                    Console.WriteLine();
                }
                catch (IOException)
                {
                    Console.WriteLine("Error with directory, please input another path.");
                    continue;
                }

                valid = true;
            }
            Array.Sort(files, new NaturalStringComparer());

            int i = 1;
            long lastFileSize = 0L;
            foreach (string file in files)
            {
                int fileID = int.Parse(Path.GetFileNameWithoutExtension(file));

                if (fileID != i)
                {
                    if (fileID < i)
                    {
                        Console.WriteLine("Duplicate name : " + fileID);
                        i--;
                    }
                    else
                    {
                        int difference = fileID - i;
                        Console.WriteLine("Missing name      : " + (fileID - difference));
                        i += difference;
                    }
                }

                long currentFileSize = new FileInfo(file).Length;
                if (currentFileSize == lastFileSize)
                {
                    Console.WriteLine("Duplicate file    : " + i);
                }

                lastFileSize = currentFileSize;
                i++;
            }

            if (Console.CursorTop == 3)
                Console.WriteLine("No errors found!");
            
            Console.Read();
        }
    }

    static class SafeNativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);
    }

    public sealed class NaturalStringComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            return SafeNativeMethods.StrCmpLogicalW(a, b);
        }
    }
}
