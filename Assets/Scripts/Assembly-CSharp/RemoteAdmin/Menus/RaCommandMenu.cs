using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NorthwoodLib.Pools;
using RemoteAdmin.Elements;
using TMPro;
using Tooltips;
using UnityEngine;

namespace RemoteAdmin.Menus
{
    public class RaCommandMenu : MonoBehaviour, ITooltipHolder
    {
        [field: SerializeField]
        protected string DefaultFormat { get; set; }

        [field: SerializeField]
        public List<ValueButton> Options { get; set; }

        [field: SerializeField]
        protected TMP_InputField InputFieldText { get; set; }

        [field: SerializeField]
        public TooltipData[] StoredInfo { get; set; }

        [field: SerializeField]
        protected TooltipManager TooltipManager { get; set; }

        public RaCommandMenu()
        {
            DefaultFormat = "{0} {1} {2}";
            Options = new List<ValueButton>();
        }

        public virtual void SendCommand(string command, string format = "")
        {
            if (string.IsNullOrEmpty(format))
                format = DefaultFormat;

            string builtCommand = BuildCommand(command, format);

            if (ReferenceHub.TryGetLocalHub(out ReferenceHub hub))
            {
                hub.queryProcessor.CmdSendQuery(builtCommand);
            }
        }

        protected virtual string BuildCommand(string command, string format)
        {
            StringBuilder sb = StringBuilderPool.Shared.Rent();
            try
            {
                string[] parts = new string[4];
                parts[0] = command;
                parts[1] = GetSelectedPlayers(sb);
                parts[2] = GetSelectedOptions(sb);
                parts[3] = GetInputFieldText();

                return string.Format(format, parts);
            }
            finally
            {
                StringBuilderPool.Shared.Return(sb);
            }
        }

        protected virtual string GetSelectedPlayers(StringBuilder builder)
        {
            builder.Clear();
            foreach (var record in PlayerRecord.Instances)
            {
                if (record.IsSelected)
                {
                    builder.Append(record.PlayerId);
                    builder.Append('.');
                }
            }
            string result = builder.ToString();
            builder.Clear();
            return result;
        }

        protected virtual string GetSelectedOptions(StringBuilder builder)
        {
            builder.Clear();
            foreach (var option in Options.OrderBy(b => b.ChoiceId))
            {
                if (!option.IsSelected)
                    continue;

                if (option.gameObject == null || !option.gameObject.activeInHierarchy)
                    continue;

                builder.Append(option.Value);
            }
            string result = builder.ToString();
            builder.Clear();
            return result;
        }

        protected virtual string GetInputFieldText()
        {
            if (InputFieldText == null)
                return string.Empty;

            if (string.IsNullOrEmpty(InputFieldText.text))
                return string.Empty;

            if (!InputFieldText.gameObject.activeInHierarchy)
                return string.Empty;

            return InputFieldText.text;
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        private void Update()
        {
            OnUpdate();
        }

        private void Start()
        {
            OnStart();

            if (StoredInfo != null && TooltipManager != null)
            {
                foreach (var tooltipData in StoredInfo)
                {
                    if (tooltipData.Key != null)
                    {
                        string text = tooltipData.Text.Replace("\n", Environment.NewLine);
                        TooltipManager.StoredTips[tooltipData.Key] = text;
                    }
                }
            }

            if (Options != null)
            {
                foreach (var option in Options)
                {
                    if (option != null && option.CommandMenu == null)
                        option.CommandMenu = this;
                }
            }
        }
    }
}