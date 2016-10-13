using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;


   public class DisnableUtil
   {    
       /// <summary>  
       /// 窗口左上角x  
       /// </summary>  
       public static int winPosX;
       /// <summary>  
       /// 窗口左上角y  
       /// </summary>  
       public static int winPosY;

       [DllImport("user32.dll")]
       static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
       [DllImport("user32.dll")]
       static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
       [DllImport("user32.dll")]
       static extern IntPtr GetForegroundWindow();

       [DllImport("User32.dll", EntryPoint = "GetSystemMetrics")]
       public static extern IntPtr GetSystemMetrics(int nIndex);

       const int SM_CXSCREEN = 0x00000000;
       const int SM_CYSCREEN = 0x00000001;

       const uint SWP_SHOWWINDOW = 0x0040;
       const int GWL_STYLE = -16;
       const int WS_BORDER = 1;
       const int WS_POPUP = 0x800000;

       // Use this for initialization  
       public static void disnable(int width,int height)
       {
           //当前屏幕分辨率  
           int resWidth = (int)GetSystemMetrics(SM_CXSCREEN);
           int resHeight = (int)GetSystemMetrics(SM_CYSCREEN);

           winPosX = resWidth / 2 - width / 2;
           winPosY = resHeight / 2 - height / 2 - 1;
           SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_POPUP);
           //测试发现左下角坐标为（0,1),修改如下  
           bool result = SetWindowPos(GetForegroundWindow(), 0, winPosX, winPosY, width, height, SWP_SHOWWINDOW);
       }  
    }

