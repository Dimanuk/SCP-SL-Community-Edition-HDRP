using System;
using UnityEngine;

namespace GameCore
{
    [Serializable]
    public class CommandHint
    {
        public string name;

        public string shortDesc;

        [Multiline]
        public string fullDesc;
    }
}
