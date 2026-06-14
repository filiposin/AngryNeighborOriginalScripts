using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public FPSCamera FPScamera;
    public GameObject[] PlayerObjects;

    [Header("Опционально: корень, где лежит оружие игрока")]
    [SerializeField] private Transform weaponRoot;
    [SerializeField] private string hammerName = "Hammer_FPS";
    [SerializeField, Min(0.05f)] private float requeryInterval = 0.25f;

    private CharacterSystem character;
    private bool _lastIsMine;
    private bool _lastHasHammer;
    private float _nextQueryAt;

    // Глобальное событие об экипировке молота (singleplayer ок)
    public static event Action<bool> HammerEquippedChanged;

    private void Awake()
    {
        character = GetComponent<CharacterSystem>();
        if (!FPScamera) FPScamera = GetComponentInChildren<FPSCamera>(true);
    }

    private void Start()
    {
        ApplyIsMine(force: true);
        CheckHammer(force: true); // первый прогон, чтобы разослать событие
    }

    private void Update()
    {
        ApplyIsMine();
        CheckHammer(); // лёгкая проверка в своём поддереве, а не по всей сцене
    }

    // Если из системы инвентаря есть событие "смена оружия", зови это вместо CheckHammer
    public void NotifyHammerEquipped(bool equipped)
    {
        if (equipped == _lastHasHammer) return;
        _lastHasHammer = equipped;
        HammerEquippedChanged?.Invoke(equipped);
    }

    private void ApplyIsMine(bool force = false)
    {
        bool isMine = character && character.IsMine;
        if (!force && isMine == _lastIsMine) return;
        _lastIsMine = isMine;

        if (FPScamera) FPScamera.gameObject.SetActive(isMine);
        if (PlayerObjects != null)
            for (int i = 0; i < PlayerObjects.Length; i++)
                if (PlayerObjects[i]) PlayerObjects[i].SetActive(!isMine);
    }

    private void CheckHammer(bool force = false)
    {
        if (!weaponRoot) return; // если не задан — лучше дергать Notify из системы экипировки

        if (!force && Time.unscaledTime < _nextQueryAt) return;
        _nextQueryAt = Time.unscaledTime + requeryInterval;

        bool has = false;
        var trs = weaponRoot.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < trs.Length; i++)
        {
            var t = trs[i];
            if (t.GetComponent<HammerTag>()) { has = true; break; }
            string n = t.name;
            if (n.StartsWith(hammerName) || n.StartsWith(hammerName + "(Clone)")) { has = true; break; }
        }

        if (has != _lastHasHammer)
        {
            _lastHasHammer = has;
            HammerEquippedChanged?.Invoke(has);
        }
    }

    public void Hide(Transform trans, bool hide)
    {
        foreach (Transform child in trans) child.gameObject.SetActive(hide);
        trans.gameObject.SetActive(hide);
    }
}

// Пустой маркер на префаб молота — для быстрого точного поиска
public class HammerTag : MonoBehaviour {}
