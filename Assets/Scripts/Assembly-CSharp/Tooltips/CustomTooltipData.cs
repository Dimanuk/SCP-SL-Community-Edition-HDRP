using UnityEngine;

namespace Tooltips
{
    public class CustomTooltipData : MonoBehaviour, ITooltipHolder
    {
        private TooltipData[] StoredInfo { get; set; }

        TooltipData[] ITooltipHolder.StoredInfo
        {
            get => StoredInfo;
            set => StoredInfo = value;
        }
        private TooltipManager Manager { get; set; }

        private void Awake()
        {
            if (Manager == null || StoredInfo == null || StoredInfo.Length == 0)
                return;

            foreach (var data in StoredInfo)
            {
                if (data.Key != null && !string.IsNullOrEmpty(data.Text))
                {
                    Manager.StoredTips[data.Key] = data.Text;
                }
            }
        }
    }
}