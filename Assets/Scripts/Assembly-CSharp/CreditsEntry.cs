using UnityEngine;
using System.Linq;

public class CreditsEntry
{
    public string Title { get; set; }
    public string Name { get; set; }
    public bool Multi { get; set; }
    public Color Color { get; set; }

    public CreditsEntry() { }

    public CreditsEntry(string title, string name)
    {
        Title = title;
        Multi = false;
        Name = name;
        Color = Color.white;       
    }

    public CreditsEntry(string title, string name, Color color)
    {
        Title = title;
        Multi = false;
        Name = name;
        Color = color;
    }

    public CreditsEntry(string name)
    {
        Multi = false;
        Title = "";
        Name = name;
    }

    public CreditsEntry(string[] names)
    {
        Multi = true;
        Title = "";
        Name = names.Aggregate("", (current, n) => current + n + "\n");
    }
}