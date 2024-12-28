using Debug = UnityEngine.Debug;

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

#if UNITY_EDITOR == false
using System;
using System.IO;
using UnityEngine;
#endif

public static class DebugHelper
{
#if UNITY_EDITOR == false
    private readonly static string _logFileName = "RuntimeLogFile.txt";

    private readonly static string filePath;

    static DebugHelper()
    {
        filePath = Path.Combine(Application.persistentDataPath, _logFileName);
    }
#endif

    [Conditional("DEBUG_HELPER"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogMessage(this object objectToLog, string prefixMessage = "", UnityEngine.Object contextObject = null)
    {
        Debug.Log($"{prefixMessage}: {objectToLog}", contextObject);
    }

    [Conditional("DEBUG_HELPER"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogMessage(this string message, string prefixMessage = "", UnityEngine.Object contextObject = null)
    {
        Debug.Log($"{prefixMessage}: {message}", contextObject);
    }

    [Conditional("DEBUG_HELPER"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogWarning(this object objectToLog, string prefixMessage = "")
    {
        Debug.LogWarning($"{prefixMessage}: {objectToLog}");
    }

    [Conditional("DEBUG_HELPER"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogWarning(this string message, string prefixMessage = "")
    {
        Debug.LogWarning($"{prefixMessage}: {message}");
    }

    [Conditional("DEBUG_HELPER"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogError(this object objectToLog, string prefixMessage = "")
    {
        Debug.LogError($"{prefixMessage}: {objectToLog}");
    }

    [Conditional("DEBUG_HELPER"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogError(this string message, string prefixMessage = "")
    {
        Debug.LogError($"{prefixMessage}: {message}");
    }

    [Conditional("DEBUG_HELPER"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsValid(this object objectToCheck, string context = "", string objectName = "")
    {
        if (objectToCheck == null)
        {
            string message = $"Validation Check Failed! Object Is Null: {objectName}";

            if (!string.IsNullOrEmpty(context))
            {
                message = $"{context} - {message}";
            }

            Debug.LogError(message);
        }
    }

    [Conditional("DEBUG_HELPER"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogElements<T>(this IReadOnlyList<T> list, string listName = "List")
    {
        StringBuilder logMessage = new StringBuilder();

        logMessage.AppendLine($"{listName} Contents:");

        for (int i = 0; i < list.Count; i++)
        {
            logMessage.AppendLine($"Index: {i}, Value: {list[i]}");
        }

        Debug.Log(logMessage.ToString());
    }

    [Conditional("DEBUG_HELPER"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogElements<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, string dictionaryName = "Dictionary")
    {
        StringBuilder logMessage = new StringBuilder();

        logMessage.AppendLine($"{dictionaryName} Contents:");

        foreach (var kvp in dictionary)
        {
            logMessage.AppendLine($"Key: {kvp.Key}, Value: {kvp.Value}");
        }

        Debug.Log(logMessage.ToString());
    }

    [Conditional("DEBUG_HELPER"), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckCondition(bool condition, string errorMessage)
    {
        if (condition == true)
        {
            LogError(errorMessage);
        }
    }

#if UNITY_EDITOR == false && UNITY_WEBGL == false
    private readonly static bool _shouldWriteErrorsToApplicationLogFile = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void InitializeFileLogging()
    {
        if (_shouldWriteErrorsToApplicationLogFile == true)
        {
            File.WriteAllText(filePath, string.Empty + Environment.NewLine);

            Debug.unityLogger.logEnabled = true;

            Application.logMessageReceived += OnLogMessageReceived;
        }
    }

    private static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if(type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            string formattedMessage = FormatLogMessage(condition, stackTrace);

            LogToFile(formattedMessage);
        }
    }

    private static string FormatLogMessage(string condition, string stackTrace)
    {
        if (Debug.isDebugBuild)
        {
            string[] stackLines = stackTrace.Split('\n');

            if (stackLines.Length > 1)
            {
                string fileNameLine = stackLines[1].Trim();

                int startIndex = fileNameLine.LastIndexOf("/") + 1;

                int endIndex = fileNameLine.LastIndexOf(":");

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    string fileName = fileNameLine.Substring(startIndex, endIndex - startIndex);

                    string lineNumber = fileNameLine.Substring(endIndex + 1);

                    return $"{fileName} ({lineNumber}): {condition}";
                }
            }

            return $"{condition} {stackTrace}";
        }

        return $"{condition} {stackTrace}";
    }


    [Conditional("DEBUG_HELPER")]
    public static void LogToFile(string message)
    {
        string logMessage = $"{DateTime.Now}: {message}";

        try
        {
            File.AppendAllText(filePath, logMessage + Environment.NewLine);

            Debug.Log($"Log written to file: {filePath}");
        }

        catch (IOException ex)
        {
            Debug.LogError($"Failed to write log to file at {filePath}: {ex.Message}");
        }
    }
#endif
}