using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MHYStart.Programes
{
    internal class MyRegedit
    {
        public static object? Read(string path, string key)
        {
            return Registry.GetValue(path, key, "");
        }

        public static void Set(string path, string key, byte[] value)
        {
            Registry.SetValue(path, key, value);
        }

    }
}
