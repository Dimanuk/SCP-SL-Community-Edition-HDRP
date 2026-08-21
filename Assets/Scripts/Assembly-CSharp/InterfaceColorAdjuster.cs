using UnityEngine;
using UnityEngine.UI;
using PlayerRoles;

public class InterfaceColorAdjuster : MonoBehaviour
{
    [SerializeField]
    public Graphic[] graphicsToChange;

    private void Awake()
    {
        PlayerRoleManager.OnRoleChanged += OnRoleChanged;
    }


    private void OnRoleChanged(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
    {
        if (newRole == null)
        {
            return;
        }

        Color teamColor = newRole.RoleColor; 
        ChangeColor(hub, teamColor);
    }

    private void ChangeColor(ReferenceHub hub, Color color)
    {

        if (hub == null || !hub.isLocalPlayer)
        {
            return;
        }

        if (graphicsToChange != null)
        {
            for (int i = 0; i < graphicsToChange.Length; i++)
            {
                Graphic graphic = graphicsToChange[i];

                if (graphic == null)
                {
                    continue;
                }

                Color currentColor = graphic.color;
                Color blendedColor = BlendColor(currentColor, color);
                graphic.color = blendedColor;
            }
        }

        if (GameMenu.singleton != null && GameMenu.singleton.colorableElements != null)
        {
            Graphic[] menuGraphics = GameMenu.singleton.colorableElements;
            
            for (int i = 0; i < menuGraphics.Length; i++)
            {
                Graphic graphic = menuGraphics[i];
                
                if (graphic == null)
                {
                    continue;
                }
                
                Color currentColor = graphic.color;
                Color blendedColor = BlendColor(currentColor, color);
                graphic.color = blendedColor;
            }
        }

        PlayerList.UpdateColors();
    }


    private Color BlendColor(Color baseColor, Color targetColor)
    {
        return new Color(targetColor.r, targetColor.g, targetColor.b, baseColor.a);
    }
}