using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using static WindowManager;
using UnityEngine.XR;
using System.Security.Cryptography;

public class WindowManager : MonoBehaviour {
    public Vector2 ForegroundMin { get; private set; }
    public Vector2 ForegroundMax { get; private set; }
    public Vector2 ForegroundVelocity { get; private set; }

    private Vector2 lastForegroundMin;

    private delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

    private void Start() {
    }

    private void Update() {
        lastForegroundMin = ForegroundMin;
        IntPtr hWnd = GetForegroundWindow();
        RECT rect = default;
        GetWindowRect(hWnd, ref rect);
        // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowrect#remarks
        // this is supposed to be the function to get the rect excluding the drop shadow but it don't work
        // so we're gonna fudge the values manually based on pixel peeping done on windows 10 :)
        // might be off on some versions or themes? no idea!
        // if windows wants better support then they should make their api actually work
        // incredibly cursed knowledge: the windows client rect considers the drop shadow to be 1 pixel wider on the right than the left
        rect.left += 7;
        rect.right -= 8;
        //DwmGetWindowAttribute(hWnd, 8, ref rect, 16); // 8 = DWMWA_EXTENDED_FRAME_BOUNDS, 16 = sizeof(RECT)
        /*GCHandle rectHandle = GCHandle.Alloc(rect);
        try {
            DwmGetWindowAttribute(hWnd, 8, GCHandle.ToIntPtr(rectHandle), 16); // 8 = DWMWA_EXTENDED_FRAME_BOUNDS, 16 = sizeof(RECT)
        } finally {
            if (rectHandle.IsAllocated) {
                rectHandle.Free();
            }
        }*/
        ForegroundMin = new Vector2(rect.left, rect.top);
        ForegroundMax = new Vector2(rect.right, rect.bottom);
        ForegroundVelocity = (ForegroundMin - lastForegroundMin) / Time.deltaTime;
    }

    // fuck this dude there's no reliable way to tell whether a window is visible i swear to god
    public void UpdateWindowList() {
        List<IntPtr> result = new List<IntPtr>();
        GCHandle listHandle = GCHandle.Alloc(result);
        try {
            EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
            EnumWindows(childProc, GCHandle.ToIntPtr(listHandle));
        } finally {
            if (listHandle.IsAllocated) {
                listHandle.Free();
            }
        }
        foreach (IntPtr hWnd in result) {
            if (IsIconic(hWnd)) {
                continue;
            }
            /*if (!IsWindowVisible(hWnd)) {
                continue;
            }*/
            WINDOWPLACEMENT wndpl = default;
            wndpl.length = 60;
            if (!GetWindowPlacement(hWnd, ref wndpl)) {
                continue;
            }
            if (wndpl.showCmd == ShowWindowCommands.Hide || wndpl.showCmd == ShowWindowCommands.Minimized) {
                continue;
            }
            RECT rect = default;
            if (!GetWindowRect(hWnd, ref rect)) {
                continue;
            }
            if (rect.right - rect.left < 100 || rect.bottom - rect.top < 100) {
                continue;
            }
            if (rect.left < -32 || rect.top < -32 || rect.right > Screen.width + 32 || rect.bottom > Screen.height + 32) {
                continue;
            }
            StringBuilder captionSB = new StringBuilder(1024);
            GetWindowText(hWnd, captionSB, captionSB.Capacity);
            string caption = captionSB.ToString();
            if (caption == string.Empty) {
                continue;
            }
            Debug.Log(caption + ": " + wndpl.showCmd + ": " + rect.left + " " + rect.top + " " + rect.right + " " + rect.bottom);
        }
    }

    private static bool EnumWindow(IntPtr handle, IntPtr pointer) {
        GCHandle gch = GCHandle.FromIntPtr(pointer);
        List<IntPtr> list = gch.Target as List<IntPtr>;
        if (list == null) {
            throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
        }
        list.Add(handle);
        return true;
    }

    [DllImport("user32.dll", SetLastError = false)]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll", SetLastError = false)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumWindows(EnumWindowProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = false)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsIconic(IntPtr hWnd);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [DllImport("user32.dll", SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    [DllImport("dwmapi.dll", SetLastError = false)]
    private static extern int DwmGetWindowAttribute(IntPtr hWnd, uint dwAttribute, ref RECT pvAttribute, uint cbAttribute);

    private enum ShowWindowCommands : int {
        Hide = 0,
        Normal = 1,
        Minimized = 2,
        Maximized = 3,
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WINDOWPLACEMENT {
        public int length;
        public int flags;
        public ShowWindowCommands showCmd;
        public System.Drawing.Point ptMinPosition;
        public System.Drawing.Point ptMaxPosition;
        public System.Drawing.Rectangle rcNormalPosition;
    }

    [DllImport("user32.dll", SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
}
