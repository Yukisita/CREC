/*
Message Service for MAUI
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System.Diagnostics;

namespace CREC.Services;

public static class MessageService
{
    public static void ShowMessage(string message, string title = "CREC")
    {
        Debug.WriteLine($"[{title}] {message}");
        
        // In a full implementation, this would show a platform-specific dialog
        // For now, we'll just use Debug output
    }

    public static void ShowError(string message, string title = "CREC Error")
    {
        Debug.WriteLine($"[ERROR - {title}] {message}");
    }

    public static void ShowWarning(string message, string title = "CREC Warning")
    {
        Debug.WriteLine($"[WARNING - {title}] {message}");
    }
}