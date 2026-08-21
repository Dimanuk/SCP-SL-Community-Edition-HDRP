using UnityEngine;
using UnityEngine.UI;

public class IngameServerFavorite : MonoBehaviour
{
    public Sprite FullStar;
    public Sprite EmptyStar;
    public Image StarImage;

    [SerializeField]
    private Button _favoriteButton;

    private bool _favorited;

    private void Start()
    {
        var favorites = FavoriteAndHistory.LocationToList[FavoriteAndHistory.StorageLocation.Favorites];
        string serverId = FavoriteAndHistory.ServerIDLastJoined;

        _favorited = favorites.Contains(serverId);

        bool hasServerId = !string.IsNullOrEmpty(serverId);

        if (_favoriteButton != null)
            _favoriteButton.interactable = hasServerId;

        SetStar(_favorited);
    }

    private void SetStar(bool isFullStar)
    {
        if (StarImage == null)
            return;

        StarImage.sprite = isFullStar ? FullStar : EmptyStar;
    }

    public void OnButtonClick()
    {
        if (StarImage == null)
            return;
        _favorited = !_favorited;
        SetStar(_favorited);
        FavoriteAndHistory.Modify(
            FavoriteAndHistory.StorageLocation.Favorites,
            FavoriteAndHistory.ServerIDLastJoined,
            !_favorited);
    }
}