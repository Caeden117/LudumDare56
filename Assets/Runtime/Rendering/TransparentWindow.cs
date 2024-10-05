#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
using System;
using System.Runtime.InteropServices;
#endif

using UnityEngine;

public class TransparancyManager : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_LAYERED = 0x80000;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int LWA_COLORKEY = 0x1;
    private const int LWA_ALPHA = 0x2;
    private const int HWND_TOPMOST = -1;
    private const int SWP_FRAMECHANGED = 32 | 64;

    private IntPtr hWnd;

    private void Start()
    {
        // Get active window handle
        hWnd = GetActiveWindow();

        // Make window layered but not input-transparent
        SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) | WS_EX_LAYERED);

        // Adjust color key and alpha for selective transparency
        SetLayeredWindowAttributes(hWnd, 0xFF00FF, 255, LWA_COLORKEY); // You might not need LWA_COLORKEY

        // Make window appear above all others
        var fWidth = Screen.width;
        var fHeight = Screen.height;
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, fWidth, fHeight, SWP_FRAMECHANGED);
    }
#endif
}