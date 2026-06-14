using UnityEngine;

[RequireComponent(typeof(FPSController))]
public class FPSInputController : MonoBehaviour
{
    public FPSController FPSmotor;
    public CharacterDriver Driver;

    [SerializeField] private float checkInterval = 0.08f;

    private float _nextCheckAt;
    private bool _fire1Held, _fire2Held;

    private void Awake() => MouseLock.MouseLocked = true;

    private void Start()
    {
        if (!FPSmotor) FPSmotor = GetComponent<FPSController>();
        if (!Driver)   Driver   = GetComponent<CharacterDriver>();
        Application.targetFrameRate = 120;
    }

    private void Update()
    {
        var motor = FPSmotor;
        if (!motor || motor.character == null || !motor.character.IsMine) return;

        // ПЛАВНЫЕ оси (как в твоём оригинале)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool  jump       = Input.GetButton("Jump");        // <- всегда отдаём текущее состояние
        bool  boostHeld  = Input.GetKey(KeyCode.LeftShift);
        bool  keyFDown   = Input.GetKeyDown(KeyCode.F);
        bool  reloadDown = Input.GetKeyDown(KeyCode.R);

        bool driving = Driver && Driver.DrivingSeat != null;

        if (!driving)
        {
            // Всегда передаём и нули — убирает «скольжение»
            motor.Move(new Vector3(h, 0f, v));
            motor.Jump(jump); // <-- фикс «прыгает только 1 раз»
        }
        else
        {
            if (keyFDown) Driver.OutVehicle();
            Driver.Drive(new Vector2(h, v), jump); // тоже всегда, с нулями
        }

        // Boost как состояние, а не «импульс»
        motor.Boost(boostHeld ? 1.4f : 1f);

        if (MouseLock.MouseLocked)
        {
            motor.Aim(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

            var equip = motor.character.inventory ? motor.character.inventory.FPSEquipment : null;
            if (equip)
            {
                bool fire1 = Input.GetButton("Fire1");
                if (fire1) equip.Trigger(); else if (_fire1Held) equip.OnTriggerRelease();
                _fire1Held = fire1;

                bool fire2Down = Input.GetButtonDown("Fire2");
                bool fire2Hold = Input.GetButton("Fire2");
                if (fire2Down) equip.Trigger2(); else if (!fire2Hold && _fire2Held) equip.OnTrigger2Release();
                _fire2Held = fire2Hold;
            }
        }

        if (keyFDown && motor.character.inventory)
        {
            var cam = motor.FPSCamera.transform;
            motor.character.Interactive(cam.position, cam.forward);
        }

        if (reloadDown && motor.character.inventory && motor.character.inventory.FPSEquipment)
            motor.character.inventory.FPSEquipment.Reload();

        // Тяжёлые проверки — реже
        if (Time.time >= _nextCheckAt)
        {
            _nextCheckAt = Time.time + checkInterval;
            var cam = motor.FPSCamera.transform;
            motor.character.Checking(cam.position, cam.forward);
        }
    }
}
