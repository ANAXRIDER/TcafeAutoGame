using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace TcafeAutoGame
{
    class ClosedPopupMsg
    {
        private const Int32 WM_COMMAND = 0x0111;
        private const Int32 WM_INITDIALOG = 0x0110;
        private static IntPtr _pWH_CALLWNDPROCRET = IntPtr.Zero;
        private static HookProcedureDelegate _WH_CALLWNDPROCRET_PROC = new HookProcedureDelegate(ClosedPopupMsg.WH_CALLWNDPROCRET_PROC);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType hooktype, HookProcedureDelegate callback, IntPtr hMod, UInt32 dwThreadId);

        [DllImport("user32.dll")]
        private static extern IntPtr UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern Int32 CallNextHookEx(IntPtr hhk, Int32 nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder text, Int32 maxLength);

        [DllImport("user32.dll")]
        private static extern Boolean SetWindowText(IntPtr hWnd, String lpString);

        [DllImport("user32.dll")]
        private static extern Int32 GetClassName(IntPtr hWnd, StringBuilder lpClassName, Int32 nMaxCount);

        [DllImport("user32.dll")]
        private static extern Boolean EnumChildWindows(IntPtr hWndParent, EnumerateWindowDelegate callback, IntPtr data);

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetDlgCtrlID(IntPtr hwndCtl);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        private enum HookType
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CWPRETSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public UInt32 message;
            public IntPtr hwnd;
        };

        private delegate Int32 HookProcedureDelegate(Int32 iCode, IntPtr pWParam, IntPtr pLParam);
        private delegate Boolean EnumerateWindowDelegate(IntPtr pHwnd, IntPtr pParam);

        internal static void Hook()
        {            
            if (ClosedPopupMsg._pWH_CALLWNDPROCRET == IntPtr.Zero)
            {                
                ClosedPopupMsg._pWH_CALLWNDPROCRET = SetWindowsHookEx(HookType.WH_CALLWNDPROCRET, _WH_CALLWNDPROCRET_PROC, IntPtr.Zero, (uint) GetCurrentThreadId());
            }
        }

        internal static void Unhook()
        {
            if (ClosedPopupMsg._pWH_CALLWNDPROCRET != IntPtr.Zero)
            {
                UnhookWindowsHookEx(ClosedPopupMsg._pWH_CALLWNDPROCRET);
            }
        }

        private static Int32 WH_CALLWNDPROCRET_PROC(Int32 iCode, IntPtr pWParam, IntPtr pLParam)
        {
            if (iCode < 0)
            {
                return CallNextHookEx(ClosedPopupMsg._pWH_CALLWNDPROCRET, iCode, pWParam, pLParam);
            }

            CWPRETSTRUCT cwp = (CWPRETSTRUCT)Marshal.PtrToStructure(pLParam, typeof(CWPRETSTRUCT));

            if (cwp.message == WM_INITDIALOG)
            {
                Int32 iLength = GetWindowTextLength(cwp.hwnd);

                foreach (IntPtr pChildOfDialog in ClosedPopupMsg.listChildWindows(cwp.hwnd))
                {
                    iLength = GetWindowTextLength(pChildOfDialog);
                    if (iLength > 0)
                    {
                        Int32 ctrlId = GetDlgCtrlID(pChildOfDialog);
                        SendMessage(cwp.hwnd, WM_COMMAND, new IntPtr(ctrlId), pChildOfDialog);
                    }
                }
            }

            return CallNextHookEx(ClosedPopupMsg._pWH_CALLWNDPROCRET, iCode, pWParam, pLParam);
        }

        private static List<IntPtr> listChildWindows(IntPtr p)
        {
            List<IntPtr> lstChildWindows = new List<IntPtr>();
            GCHandle gchChildWindows = GCHandle.Alloc(lstChildWindows);

            try
            {
                ClosedPopupMsg.EnumChildWindows(p, new EnumerateWindowDelegate(ClosedPopupMsg.enumWindowsCallback), GCHandle.ToIntPtr(gchChildWindows));
            }
            finally
            {
                if (gchChildWindows.IsAllocated)
                {
                    gchChildWindows.Free();
                }
            }

            return lstChildWindows;
        }

        private static bool enumWindowsCallback(IntPtr hwnd, IntPtr p)
        {
            GCHandle gch = GCHandle.FromIntPtr(p);
            if (gch != null)
            {
                List<IntPtr> lstChildWindows = gch.Target as List<IntPtr>;

                if (lstChildWindows == null)
                {
                    throw new InvalidCastException("Exception");
                }
                lstChildWindows.Add(hwnd);
            }

            return true;
        }
    }
}
