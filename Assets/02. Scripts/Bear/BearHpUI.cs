using UnityEngine;
using UnityEngine.UI;

public class BearHpUI : MonoBehaviour
{
    [SerializeField] private Image _hpFillImage;

    private BearController _bear;

    private void Start()
    {
        _bear = GetComponentInParent<BearController>();

        UpdateHpUI(_bear.Stat.HP, _bear.Stat.MaxHp);
        _bear.Stat.OnHpChanged += UpdateHpUI;
    }

    private void OnDestroy()
    {
        if (_bear != null && _bear.Stat != null)
            _bear.Stat.OnHpChanged -= UpdateHpUI;
    }

    private void UpdateHpUI(float current, float max)
    {
        if (_hpFillImage != null)
            _hpFillImage.fillAmount = current / max;
    }
}
