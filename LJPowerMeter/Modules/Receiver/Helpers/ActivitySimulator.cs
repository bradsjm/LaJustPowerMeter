using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LaJust.PowerMeter.Modules.Receiver.Helpers
{
    public static class ActivitySimulator
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct Input
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendInput(int nInputs, Input[] pInputs, int cbSize);

        private const int INPUT_MOUSE = 0;
        private const int INPUT_MOUSE_MOVE = 1;

        /// <summary>
        /// Simulates moving the mouse.
        /// </summary>
        public static void MoveMouse()
        {
            Input[] Inputs = new Input[1];
            Inputs[0].type = INPUT_MOUSE;
            Inputs[0].mi.dwFlags = INPUT_MOUSE_MOVE;
            Inputs[0].mi.dx = 0;
            Inputs[0].mi.dy = 0;
            int result = SendInput(Inputs.Length, Inputs, Marshal.SizeOf(Inputs[0].GetType()));
        }
    }
}
