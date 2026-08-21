using UnityEngine;
using UnityEngine.UI;

namespace OperationalGuide
{
    public class OperationalGuide : MonoBehaviour
    {
        public static readonly float MinZoom;
        public static readonly float MaxZoom;
        public static Vector3 ScaleModifier;
        public static OperationalGuide Instance;

        public OperationalTabController[] Tabs;
        public GameObject Camera;
        public Animator FadeAnimator;
        public GameObject Back;
        public GameObject FullscreenPannable;
        public RawImage FullscreenPannableImage;

        private const float ZoomIn = 1f;
        private const float ZoomOut = -1f;

        static OperationalGuide()
        {
            MinZoom = 1.1f;
            MaxZoom = 27f;

            ScaleModifier = new Vector3(0.1f, 0.1f, 0f);
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            Camera.SetActive(true);
        }

        private void OnDisable()
        {
            Camera.SetActive(false);
        }

        public void ButtonChangeTab(int index)
        {
            for (int i = 0; i < Tabs.Length; i++)
            {
                Tabs[i].gameObject.SetActive(i == index);
                if (i == index)
                {
                    Tabs[i].EnablePage();
                }
            }
        }

        public OperationalTabController GetActiveTab()
        {
            for (int i = 0; i < Tabs.Length; i++)
            {
                if (Tabs[i].gameObject.activeSelf)
                {
                    return Tabs[i];
                }
            }
            return null;
        }

        public void BackCurrentPage()
        {
            if (FullscreenPannable.activeSelf)
            {
                FullscreenPannable.SetActive(false);
                return;
            }

            for (int i = 0; i < Tabs.Length; i++)
            {
                if (Tabs[i].gameObject.activeSelf)
                {
                    for (int j = 0; j < Tabs[i].Pages.Length; j++)
                    {
                        if (Tabs[i].Pages[j].gameObject.activeSelf)
                        {
                            Tabs[i].Pages[j].ForceTurnOff();
                            Tabs[i].gameObject.SetActive(false);
                            return;
                        }
                    }
                }
            }
        }
        
        public void ToggleCurrentPage()
        {
            for (int i = 0; i < Tabs.Length; i++)
            {
                if (Tabs[i].gameObject.activeSelf)
                {
                    for (int j = 0; j < Tabs[i].Pages.Length; j++)
                    {
                        if (Tabs[i].Pages[j].DescriptionPage != null &&
                            Tabs[i].Pages[j].gameObject.activeSelf)
                        {
                            Tabs[i].Pages[j].ToggleDescriptionMenu();
                            Tabs[i].Pages[j].ToString();
                        }
                    }
                    return;
                }
            }
        }

        public static void ChangeZoom(Transform transform, bool increase, bool panImage = false)
        {
            if (panImage)
            {
                Vector3 pos = transform.localPosition;
                float mx = Input.GetAxis("Mouse X") * 50f;
                float my = Input.GetAxis("Mouse Y") * 50f;
                transform.localPosition = pos + new Vector3(mx, my, 0f);
                return;
            }

            Vector3 scale = transform.localScale;

            if (!increase && scale.x <= MinZoom)
                return;
            if (increase && scale.x >= MaxZoom)
                return;

            Vector3 modifier = ScaleModifier * (increase ? ZoomIn : ZoomOut);
            transform.localScale = scale + modifier;
        }

        // -----------------------------------------------------------------
        // Input
        // -----------------------------------------------------------------
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackCurrentPage();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OperationalTabController tab = GetActiveTab();
                tab?.IncrementIndex(-1);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                OperationalTabController tab = GetActiveTab();
                tab?.IncrementIndex(1);
            }

            if (FullscreenPannable.activeSelf)
            {
                bool middleMouse = Input.GetMouseButton(2);
                float scroll = Input.GetAxis("Mouse ScrollWheel");

                if (middleMouse || scroll != 0f)
                {
                    Transform t = FullscreenPannableImage.transform;

                    bool increase = scroll > 0f;

                    ChangeZoom(t, increase, middleMouse);
                }
            }
        }
    }
}
