using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusAbility : PlayerAbility
{
    [SerializeField] private Image _hpFillImage;
    [SerializeField] private Image _staminaFillImage;

    private void Start()
    {
        UpdateHpUI(_owner.Stat.HP, _owner.Stat.MaxHp);
        UpdateStaminaUI(_owner.Stat.Stamina, _owner.Stat.MaxStamina);

        _owner.Stat.OnHpChanged += UpdateHpUI;
        _owner.Stat.OnStaminaChanged += UpdateStaminaUI;
    }

    private void OnDestroy()
    {
        _owner.Stat.OnHpChanged -= UpdateHpUI;
        _owner.Stat.OnStaminaChanged -= UpdateStaminaUI;
    }

    private void UpdateHpUI(float current, float max)
    {
        if (_hpFillImage != null)
            _hpFillImage.fillAmount = current / max;
    }


    private void UpdateStaminaUI(float current, float max)
    {
        if (_staminaFillImage != null)
            _staminaFillImage.fillAmount = current / max;
    }
}
