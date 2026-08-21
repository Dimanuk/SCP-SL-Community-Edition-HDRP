using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerRoles.PlayableScps.Scp079
{
    public static class Scp079Keybinds
    {
        public class ActionDefinition : NewInput.ActionDefinition
        {
            public ActionDefinition(ActionName action, KeyCode kc)
                : base(action, kc, ActionCategory.Scp079, NonCollidingActions)
            {
            }
        }

        private const string Scp079Prefix = "Scp079";

        private static bool _nonCollidingSet;
        private static ActionName[] _nonColliding;

        public static readonly ActionName[] UsedActions = new ActionName[6]
        {
            ActionName.MoveForward,
            ActionName.MoveBackward,
            ActionName.MoveLeft,
            ActionName.MoveRight,
            ActionName.Shoot,
            ActionName.Inventory
        };

        public static ActionName[] NonCollidingActions
        {
            get
            {
                if (_nonCollidingSet)
                    return _nonColliding;

                List<ActionName> list = new List<ActionName>();
                foreach (ActionName value in Enum.GetValues(typeof(ActionName)))
                {
                    if (!value.ToString().StartsWith(Scp079Prefix) && !UsedActions.Contains(value))
                        list.Add(value);
                }

                _nonCollidingSet = true;
                _nonColliding = list.ToArray();
                return _nonColliding;
            }
        }
    }
}