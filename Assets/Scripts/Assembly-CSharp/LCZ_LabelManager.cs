using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration;
using NorthwoodLib;
using UnityEngine;

public class LCZ_LabelManager : MonoBehaviour
{
    [Serializable]
    public class LCZ_Label_Preset
    {
        public string nameToContain;
        public Material mat;
    }

    public LCZ_Label_Preset[] chars;
    public Material[] numbers;

    private List<LCZ_Label> _labels = new List<LCZ_Label>();
    private List<GameObject> _rooms = new List<GameObject>();

    private void Start()
    {
        SeedSynchronizer.OnMapGenerated += RefreshLabels;

        _labels = GameObject.FindObjectsByType<LCZ_Label>(FindObjectsSortMode.None).ToList();

        Transform t = transform;
        int childCount = t.childCount;
        
        for (int i = 0; i < childCount; i++)
        {
            Transform child = t.GetChild(i);
            string childName = child.name;

            if (childName.StartsWith("Root_", StringComparison.Ordinal) || 
                childName.StartsWith("LCZ_", StringComparison.Ordinal))
            {
                _rooms.Add(child.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        SeedSynchronizer.OnMapGenerated -= RefreshLabels;
    }

    public void RefreshLabels()
    {
        if (_labels == null || _rooms == null)
            return;

        foreach (LCZ_Label label in _labels)
        {
            if (label == null)
                continue;

            bool noMatchFound = true;

            Transform labelTransform = label.transform;
            Vector3 labelPosition = labelTransform.position;
            Vector3 forwardOffset = labelTransform.forward * 10f;
            Vector3 checkPosition = labelPosition + forwardOffset;

            foreach (GameObject room in _rooms)
            {
                if (room == null)
                    continue;

                Transform roomTransform = room.transform;
                Vector3 roomPosition = roomTransform.position;
                
                if (Vector3.Distance(roomPosition, checkPosition) >= 10f)
                    continue;

                string roomName = room.name;

                foreach (LCZ_Label_Preset preset in chars)
                {
                    if (preset == null || string.IsNullOrEmpty(preset.nameToContain))
                        continue;

                    if (!StringUtils.Contains(roomName, preset.nameToContain, StringComparison.Ordinal))
                        continue;

                    noMatchFound = false;

                    int number = 0;
                    
                    if (StringUtils.Contains(roomName, "(", StringComparison.Ordinal))
                    {
                        try
                        {
                            int openIndex = roomName.IndexOf('(');
                            if (openIndex >= 0)
                            {
                                string afterOpen = roomName.Remove(0, openIndex + 1);
                                int closeIndex = afterOpen.IndexOf(')');
                                if (closeIndex >= 0)
                                {
                                    string numberStr = afterOpen.Remove(closeIndex);
                                    number = int.Parse(numberStr);
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                    if (numbers != null && number >= 0 && number < numbers.Length)
                    {
                        label.Refresh(preset.mat, numbers[number]);
                    }
                }
            }

            if (noMatchFound && chars != null && chars.Length > 0 && numbers != null && numbers.Length > 0)
            {
                label.Refresh(chars[0].mat, numbers[0]);
            }
        }
    }
}