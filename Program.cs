using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using LeeSin.Util;
using LeeSin.Controller;

namespace Lee
{
    static class Program
    {
        static void Main(string[] args)
        {
            ControllerHandler.GameStart();
            GameControl.LoadPlugin();
        }
    }
}
