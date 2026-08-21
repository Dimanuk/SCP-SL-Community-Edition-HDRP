using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FavoriteAndHistory : MonoBehaviour
{
    public enum StorageLocation
    {
        History = 0,
        Favorites = 1,
        IPHistory = 2
    }

    public const int MaxHistoryAmount = 10;

    public static readonly List<string> Favorites = new List<string>();
    public static readonly List<string> History = new List<string>();
    public static readonly List<string> IPHistory = new List<string>();

    public static string ServerIDLastJoined;

    public static readonly Dictionary<StorageLocation, List<string>> LocationToList = new Dictionary<StorageLocation, List<string>>();
    private static readonly Dictionary<StorageLocation, string> StorageEnumToPath = new Dictionary<StorageLocation, string>();

    static FavoriteAndHistory()
    {
        LocationToList[StorageLocation.History] = History;
        LocationToList[StorageLocation.Favorites] = Favorites;
        LocationToList[StorageLocation.IPHistory] = IPHistory;

        StorageEnumToPath[StorageLocation.History] = "history.txt";
        StorageEnumToPath[StorageLocation.Favorites] = "favorites.txt";
        StorageEnumToPath[StorageLocation.IPHistory] = "iphistory.txt";

        Load(StorageLocation.History);
        Load(StorageLocation.Favorites);
        Load(StorageLocation.IPHistory);
    }

    public static string GetPath(StorageLocation location)
    {
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string fileName = StorageEnumToPath[location];
        return Path.Combine(folder, "SCP Secret Laboratory", fileName);
    }

    public static void ResetServerID()
    {
        ServerIDLastJoined = string.Empty;
    }

    public static void Load(StorageLocation location)
    {
        var list = LocationToList[location];
        list.Clear();

        string path = GetPath(location);

        if (!File.Exists(path))
        {
            Revent(StorageEnumToPath[location]);
            return;
        }

        try
        {
            using var reader = new StreamReader(path);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line) && !list.Contains(line))
                    list.Add(line);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load {location}: {ex.Message}");
            Revent(StorageEnumToPath[location]);
        }
    }

    public static void Modify(StorageLocation location, string id, bool delete = false)
    {
        if (string.IsNullOrEmpty(id))
            return;

        var list = LocationToList[location];

        list.RemoveAll(x => x == id);

        if (!delete)
        {
            list.Add(id);
        }

        // Trim before persisting so the file never keeps the overflow entries on disk.
        // Both History and IPHistory are capped (Favorites is unbounded by design).
        if (location == StorageLocation.History || location == StorageLocation.IPHistory)
        {
            HistoryLimit(location, id);
        }

        SaveToFile(location);
    }

    private static void HistoryLimit(StorageLocation location, string id)
    {
        var list = LocationToList[location];

        // Add() appends newest to the end, so index 0 is the oldest entry.
        // Drain every overflow entry, not just one, in case the list is heavily over the cap.
        while (list.Count > MaxHistoryAmount)
        {
            list.RemoveAt(0);
        }
    }

    public static bool IsInStorage(StorageLocation location, string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        return LocationToList[location].Contains(id);
    }

    private static void SaveToFile(StorageLocation location)
    {
        string path = GetPath(location);
        var list = LocationToList[location];

        try
        {
            using var writer = new StreamWriter(path, false);
            foreach (var entry in list)
            {
                writer.WriteLine(entry);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save {location}: {ex.Message}");
        }
    }

    private static void Revent(string fileName)
    {
        Debug.Log($"Reventing: {fileName}");

        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SCP Secret Laboratory",
            fileName);

        try
        {
            using var writer = new StreamWriter(path);
        }
        catch { }
    }
}