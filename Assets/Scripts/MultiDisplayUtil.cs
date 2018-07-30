using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

public class MultiDisplayUtil
{
    delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll")]
    static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll")]
    static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public bool Contains(int x, int y)
        {
            return x >= this.left && x < this.right && y >= this.top && y < this.bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct MonitorInfo
    {
        /// <summary>
        /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function. 
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public int size;

        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. 
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public Rect monitor;

        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications, 
        /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor. 
        /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars. 
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public Rect workArea;

        /// <summary>
        /// The attributes of the display monitor.
        /// 
        /// This member can be the following value:
        ///   1 : MONITORINFOF_PRIMARY
        /// </summary>
        public uint flags;
    }

    /// <summary>
    /// The struct that contains the display information
    /// </summary>
    public class DisplayInfo
    {
        public string Availability { get; set; }
        public string ScreenHeight { get; set; }
        public string ScreenWidth { get; set; }
        public Rect MonitorArea { get; set; }
        public Rect WorkArea { get; set; }
    }

    /// <summary>
    /// Collection of display information
    /// </summary>
    public class DisplayInfoCollection : List<DisplayInfo>
    {
    }

    /// <summary>
    /// Returns the number of Displays using the Win32 functions
    /// </summary>
    /// <returns>collection of Display Info</returns>
    public static DisplayInfoCollection GetDisplays()
    {
        DisplayInfoCollection col = new DisplayInfoCollection();

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
            {
                MonitorInfo mi = new MonitorInfo();
                mi.size = Marshal.SizeOf(mi);
                bool success = GetMonitorInfo(hMonitor, ref mi);
                if (success)
                {
                    DisplayInfo di = new DisplayInfo();
                    di.ScreenWidth = (mi.monitor.right - mi.monitor.left).ToString();
                    di.ScreenHeight = (mi.monitor.bottom - mi.monitor.top).ToString();
                    di.MonitorArea = mi.monitor;
                    di.WorkArea = mi.workArea;
                    di.Availability = mi.flags.ToString();
                    col.Add(di);
                }
                return true;
            }, IntPtr.Zero);
        return col;
    }


    public static int GetCurrentDisplay()
    {
#if UNITY_EDITOR
        if (!IsGameWindow(EditorWindow.focusedWindow))
            return -1;

        return GetGameViewDisplay(EditorWindow.focusedWindow);
#else
        var displays = GetDisplays();
        POINT cursorPos;
        GetCursorPos(out cursorPos);
        for (int i = 0; i < displays.Count; i++)
        {
            var v = displays[i];
            if (v.WorkArea.Contains(cursorPos.x, cursorPos.y))
                return i;
        }

        return -1;
#endif
    }

#if UNITY_EDITOR

    public static bool IsGameWindow(EditorWindow window)
    {
        if (window == null)
            return false;

        if (window.GetType() != GetGameWindowType())
            return false;

        return true;
    }

    public static UnityEngine.Rect GetGameViewPosition(EditorWindow window)
    {
        return window.position;
    }

    public static int GetGameViewDisplay(EditorWindow window)
    {
        Type viewType = GetGameWindowType();
        FieldInfo fieldInf = viewType.GetField("m_TargetDisplay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)fieldInf.GetValue(window);
    }

    private static Type GetGameWindowType()
    {
        Type gameViewType = Type.GetType("UnityEditor.GameView,UnityEditor");
        return gameViewType;
    }

#endif

}
