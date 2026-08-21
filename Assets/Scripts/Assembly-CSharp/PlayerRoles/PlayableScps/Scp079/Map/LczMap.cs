using System.Collections.Generic;
using MapGeneration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoles.PlayableScps.Scp079.Map
{
    public class LczMap : ProceduralZoneMap
    {
        private const float UnnamedAlpha = 0.2f;
        private const float UnnamedSize = 0.7f;

        [SerializeField]
        private ProceduralZoneMap _hczMap;

        [SerializeField]
        private ProceduralZoneMap _ezMap;

        [SerializeField]
        private Vector2 _spacing;

        protected override void PlaceRooms()
        {
            base.PlaceRooms();
        }

        protected override void PostProcessRooms()
        {
            base.PostProcessRooms();

            Vector2 offset = _hczMap.RectBounds.center - base.RectBounds.center;
            offset += Vector2.up * (_spacing.y + _hczMap.RectBounds.extents.y + base.RectBounds.extents.y);
            offset += Vector2.right * (base.RectBounds.extents.x + _spacing.x - _hczMap.RectBounds.extents.x);

            foreach (KeyValuePair<RoomIdentifier, Image> room in NodesByRoom)
            {
                ProcessName(room.Key, room.Value);
                room.Value.rectTransform.anchoredPosition += offset;
            }

            ZoneLabel.rectTransform.anchoredPosition += offset;

            base.RectBounds = new Bounds(
                base.RectBounds.center + (Vector3)offset,
                base.RectBounds.size);
        }

        private void ProcessName(RoomIdentifier room, Image img)
        {
            if (room.Name != RoomName.Unnamed)
                return;

            string text = room.name;
            text = text.Remove(0, text.IndexOf('(') + 1);
            text = text.Remove(text.IndexOf(')'));

            if (int.TryParse(text, out int result))
            {
                TextMeshProUGUI label = img.GetComponentInChildren<TextMeshProUGUI>();
                label.text = string.Format(label.text, result);
                label.alpha = UnnamedAlpha;
                label.fontSize *= UnnamedSize;
            }
        }
    }
}
