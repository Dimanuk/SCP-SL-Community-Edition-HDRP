using System.Collections.Generic;
using UnityEngine;

namespace LightContainmentZoneDecontamination
{
    public class DecontaminationGas : MonoBehaviour
    {
        private static List<DecontaminationGas> allInstances = new List<DecontaminationGas>();

        private static bool _turnedOn;

        public static bool TurnedOn
        {
            get => _turnedOn;
            set
            {
                foreach (DecontaminationGas instance in allInstances)
                {
                    if (instance == null)
                        continue;
                    if (instance == null)
                        continue;

                    GameObject go = instance.gameObject;
                    if (go != null)
                    {
                        go.SetActive(value);
                    }
                }

                _turnedOn = value;
            }
        }

        private void Start()
        {
            allInstances.Add(this);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            allInstances.Remove(this);
        }
    }
}