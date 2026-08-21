using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NorthwoodLib.Pools;
using System.Collections.Generic;

public class NewCredits : MonoBehaviour
{
    public GameObject root;
    public GameObject canvas;
    public bool StartScroll;
    public float Speed;

    private const float OriginalSpeed = 1.45f;
    private const float FastSpeed = 6f;

    private bool _paused;

    public RectTransform Content;
    public RectTransform PrefCategory;
    public RectTransform PrefElementWithRole;
    public RectTransform PrefElement;

    public AudioSource CreditsMusic;

    private bool _loaded;
    private int _scrollDelay;
    private float _height;

    private RectTransform[] _contentChildren;
    private VerticalLayoutGroup layoutGroup;
    private MainMenuSoundtrackController menuSoundtrack;

    public NewCredits()
    {
        Speed = OriginalSpeed;
        _scrollDelay = 100;
    }

    private void OnEnable()
    {
        layoutGroup = Content.GetComponent<VerticalLayoutGroup>();
    }

    public void OnButtonClick()
    {
        if (!_loaded)
        {
            Load();
            _loaded = true;
        }

        if (menuSoundtrack == null)
            menuSoundtrack = FindAnyObjectByType<MainMenuSoundtrackController>();
        
        canvas.SetActive(false);
        Content.localPosition = new Vector3(Content.localPosition.x, 0f, 0f);
        StartScroll = true;

        if (_contentChildren != null)
        {
            foreach (var child in _contentChildren)
            {
                if (child == null) continue;
                if (!child.gameObject.activeSelf)
                    child.gameObject.SetActive(true);
            }
        }

        root.SetActive(true);

        if (menuSoundtrack != null && CreditsMusic != null)
        {
            menuSoundtrack.SoundtrackState = MainMenuSoundtrackController.MenuSoundtrackState.Muted;
            CreditsMusic.Play();
        }
    }

    public void OnBackButtonClick()
    {
        canvas.SetActive(true);
        root.SetActive(false);
        StartScroll = false;

        if (menuSoundtrack != null && CreditsMusic != null)
        {
            menuSoundtrack.SoundtrackState = MainMenuSoundtrackController.MenuSoundtrackState.MenuJustLoaded;
            CreditsMusic.Stop();
        }
    }


    private void Update()
    {
        Speed = Input.GetKey(KeyCode.RightArrow) ? FastSpeed : OriginalSpeed;

        if (Input.GetKey(KeyCode.LeftArrow))
            Speed = -Speed;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            _paused = !_paused;
    }

    private void FixedUpdate()
    {
        if (!_loaded)
        {
            Load();
            _loaded = true;
        }

        if (root == null || !root.activeSelf)
            return;

        if (!StartScroll)
            return;

        if (_scrollDelay > 0)
        {
            _scrollDelay--;
            return;
        }

        if (layoutGroup != null && layoutGroup.enabled)
        {
            _height = Screen.height + Content.rect.height + 0f; 
            layoutGroup.enabled = false;
        }

        if (_paused)
            return;

        if (ScrollAndCanEnd())
        {
            canvas.SetActive(true);
            root.SetActive(false);
            StartScroll = false;

            if (menuSoundtrack != null && CreditsMusic != null)
            {
                menuSoundtrack.SoundtrackState = MainMenuSoundtrackController.MenuSoundtrackState.MenuJustLoaded;
                CreditsMusic.Stop();
            }
            return;
        }

        if (_contentChildren != null)
        {
            for (int i = 0; i < _contentChildren.Length; i++)
            {
                var child = _contentChildren[i];
                if (child == null) continue;

                bool visible = child.position.y > 0f && child.position.y <= Screen.height;
                if (child.gameObject.activeSelf != visible)
                    child.gameObject.SetActive(visible);
            }
        }
    }


    private bool ScrollAndCanEnd()
    {
        if (Content.localPosition.y >= _height)
            return true;

        if (_paused)
            return false;

        if (Speed <= 0f && Content.localPosition.y <= 0f)
            return false;

        Content.Translate(Vector3.up * Speed);

        var lp = Content.localPosition;
        Content.localPosition = new Vector3(
            lp.x,
            Mathf.Clamp(lp.y, 0f, _height),
            lp.z);

        return false;
    }

    private void Initialize()
    {
        StartScroll = false;
        Content.localPosition = new Vector3(Content.localPosition.x, 0f, 0f);

        if (_contentChildren != null)
        {
            foreach (var child in _contentChildren)
            {
                if (child == null) continue;
                if (!child.gameObject.activeSelf)
                    child.gameObject.SetActive(true);
            }
        }
    }

    private void Load()
    {
        List<RectTransform> list = NorthwoodLib.Pools.ListPool<RectTransform>.Shared.Rent();

        if (CreditsData.Data != null)
        {
            foreach (var category in CreditsData.Data)
            {
                if (category.Records == null || category.Records.Length == 0)
                    continue;

                var cat = Instantiate(PrefCategory, Content, true);
                cat.GetComponent<TextMeshProUGUI>().SetText(category.Header);
                cat.localScale = Vector3.one;
                list.Add(cat);

                foreach (var entry in category.Records)
                {
                    if (!string.IsNullOrEmpty(entry.Title))
                    {
                        var el = Instantiate(PrefElementWithRole, Content, true);
                        el.localScale = Vector3.one;

                        var comp = el.GetComponent<NewCreditsElement>();
                        string displayName = entry.Name == "<current nickname>"
                            ? Welcome.CurrentNickname
                            : entry.Name;

                        comp.Name.SetText(displayName);
                        comp.Role.SetText(entry.Title);
                        comp.Background.color = entry.Color;

                        list.Add(el);
                    }
                    else
                    {
                        string displayName = entry.Name;
                        if (!string.IsNullOrEmpty(displayName) && entry.Multi)
                            displayName = displayName.Replace("\n", " ");

                        var el = Instantiate(PrefElement, Content, true);
                        el.localScale = Vector3.one;
                        el.GetComponent<TextMeshProUGUI>().SetText(displayName);
                        list.Add(el);
                    }
                }
            }
        }

        _contentChildren = list.ToArray();
        NorthwoodLib.Pools.ListPool<RectTransform>.Shared.Return(list);
    }
}
