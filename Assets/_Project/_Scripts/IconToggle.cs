using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconToggle : MonoBehaviour
{
    [SerializeField] private Sprite iconTrue, iconFalse;
    [SerializeField] private SoundManager sManager;

    private bool defaultIconState = true;

    public ToggleType toggleType;

    private Image image;
    bool state;

    private void Awake()
    {
        image = GetComponent<Image>();
        SetIcon();
    }

    private void Start()
    {
        if (!sManager) sManager = FindFirstObjectByType<SoundManager>();
    }

    public void ToggleIcon()
    {
        if (!image || !iconTrue || !iconFalse) return;
        state = !state;
        image.sprite = state ? iconTrue : iconFalse;
    }

    public void SetIcon()
    {
        switch (toggleType)
        {
            case ToggleType.music:
                image.sprite = sManager.IsMusicEnabled() ? iconTrue : iconFalse;
                state = sManager.IsMusicEnabled();
                break;

            case ToggleType.sfx:
                image.sprite = sManager.IsSFXEnabled() ? iconTrue : iconFalse;
                state = sManager.IsSFXEnabled();
                break;

            default:
                image.sprite = defaultIconState ? iconTrue : iconFalse;
                state = true;
                break;
        }
    }
}
public enum ToggleType
{
    music,
    sfx,
    pause,
    rotate,
    back,
}
