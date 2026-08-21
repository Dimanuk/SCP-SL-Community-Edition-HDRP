using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewsElement : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Date;
    public TextMeshProUGUI Content;
    public Image Background;

    public int Id { get; set; }

    private NewsLoader _loader;
    private void Start()
    {
        _loader = GetComponentInParent<NewsLoader>();
    }
    public void OnClick()
    {
        if (_loader == null)
            throw new System.NullReferenceException();

        _loader.ShowAnnouncement(Id);
    }

    public override bool Equals(object obj)
    {
        if (obj is not NewsElement other)
            return false;

        return Title == other.Title &&
               Date == other.Date &&
               Content == other.Content &&
               Background == other.Background &&
               Id == other.Id &&
               _loader == other._loader;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (Title?.GetHashCode() ?? 0);
            hash = hash * 23 + (Date?.GetHashCode() ?? 0);
            hash = hash * 23 + (Content?.GetHashCode() ?? 0);
            hash = hash * 23 + (Background?.GetHashCode() ?? 0);
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + (_loader?.GetHashCode() ?? 0);
            return hash;
        }
    }
}