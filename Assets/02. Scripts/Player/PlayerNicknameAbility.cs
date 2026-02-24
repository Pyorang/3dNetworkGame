using TMPro;
using UnityEngine;

public class PlayerNicknameAbility : PlayerAbility
{
    [SerializeField] private TextMeshProUGUI _nicknameTextUI;

    private void Start()
    {
        if (_owner.PhotonView.IsMine)
        {
            _nicknameTextUI.color = Color.green;
        }
        else
        {
            _nicknameTextUI.color = Color.red;
        }

        _nicknameTextUI.text = _owner.PhotonView.Owner.NickName;
    }
}
