using UnityEngine;
using UnityEngine.UI;

namespace OperationalGuide
{
    public class OperationalTabController : MonoBehaviour
    {
        public OperationalPage[] Pages;
        public Button[] Buttons;

        private int _currentIndex;

        /* ==========================================================
           Public API
           ========================================================== */

        public void ButtonChangePage(int index)
        {
            if (_currentIndex == index)
                return;

            if (Pages == null)
                return;

            for (int i = 0; i < Pages.Length; i++)
            {
                if (Pages[i] == null)
                    continue;

                if (i == index)
                {
                    Pages[i].ToggleDescriptionMenu();
                }
                else
                {
                    Pages[i].ForceTurnOff();
                }

                if (index != i && Buttons != null && i < Buttons.Length && Buttons[i] != null)
                {
                    Buttons[i].OnDeselect(null);
                }
            }

            _currentIndex = index;
        }

        public void IncrementIndex(int index)
        {
            int newIndex = _currentIndex + index;

            if (Pages == null)
            {
                ButtonChangePage(0);
                return;
            }

            if (newIndex < 0)
            {
                newIndex = Pages.Length - 1;
            }
            else if (newIndex >= Pages.Length)
            {
                ButtonChangePage(0);
                return;
            }

            _currentIndex = newIndex;

            for (int i = 0; i < Pages.Length; i++)
            {
                if (Pages[i] == null)
                    continue;

                if (i == _currentIndex)
                {
                    Pages[i].ToggleDescriptionMenu();
                }
                else
                {
                    Pages[i].ForceTurnOff();
                }

                if (_currentIndex != i && Buttons != null && i < Buttons.Length && Buttons[i] != null)
                {
                    Buttons[i].OnDeselect(null);
                }
            }
        }

        public void EnablePage()
        {
            IncrementIndex(0);
        }

        public OperationalPage GetActivePage()
        {
            if (Pages == null)
                return null;

            for (int i = 0; i < Pages.Length; i++)
            {
                if (Pages[i] == null)
                    continue;

                if (Pages[i].gameObject != null && Pages[i].gameObject.activeSelf)
                {
                    return Pages[i];
                }
            }

            return null;
        }

        /* ==========================================================
           Unity Lifecycle
           ========================================================== */

        private void Update()
        {
            if (Pages == null || Buttons == null)
                return;

            for (int i = 0; i < Pages.Length; i++)
            {
                if (i == _currentIndex)
                {
                    if (i < Buttons.Length && Buttons[i] != null)
                    {
                        Buttons[i].OnDeselect(null);
                    }
                }
            }
        }
    }
}
