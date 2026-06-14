using LeTai.Asset.TranslucentImage;
using UnityEngine;

public class BlurMaster : MonoBehaviour
{
    [Header("В инвентарь ставьте Inventory, в настройки Settings")]
    [SerializeField] private TranslucentImageSource source;
    [SerializeField] private GameObject fps;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject settings;

    [Header("Деллей если игрок сдохнет (можете ставить больше чтобы было больше фпс)")]
    [SerializeField, Min(0.05f)] private float requeryInterval = 0.25f;

    private bool blurOn;
    private float nextQueryAt;

    private void OnEnable()
    {
        ResolveRefs(force:true);
        Apply();
    }

    private void Update()
    {
        ResolveRefs();

        bool menuOpen =
            (inventory && inventory.activeInHierarchy) ||
            (settings  && settings.activeInHierarchy);

        if (menuOpen != blurOn)
        {
            blurOn = menuOpen;
            Apply();
        }
    }

    private void ResolveRefs(bool force = false)
    {
        if (!force && Time.unscaledTime < nextQueryAt) return;
        nextQueryAt = Time.unscaledTime + requeryInterval;

        if (!source)
        {
#if UNITY_2023_1_OR_NEWER
            source = Object.FindFirstObjectByType<TranslucentImageSource>(FindObjectsInactive.Include);
#else
            source = Object.FindObjectOfType<TranslucentImageSource>(true);
#endif
            if (source) Apply(); 
        }

        if (!inventory) inventory = GameObject.Find("Inventory");
        if (!settings)  settings  = GameObject.Find("Settings");
    }

    private void Apply()
    {
        if (source) source.enabled = blurOn;
        if (fps)    fps.SetActive(!blurOn);
    }

    public void SetMenuOpen(bool open)
    {
        blurOn = open;
        Apply();
    }
}
