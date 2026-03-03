using UnityEngine;
using Photon.Pun;

public class PlayerWeaponHitAbility : PlayerAbility
{
    private void OnTriggerEnter(Collider other)
    {
        if (!_owner.PhotonView.IsMine) return;
        if (_owner.IsDead) return;
        if (other.transform.root == _owner.transform.root) return;

        // 플레이어 공격
        PlayerController target = other.GetComponentInParent<PlayerController>();
        if (target != null && !target.IsDead)
        {
            target.PhotonView.RPC(
                nameof(PlayerController.TakeDamage),
                target.PhotonView.Owner,
                _owner.Stat.AttackPower,
                _owner.PhotonView.Owner.NickName
            );
            return;
        }

        // 곰 공격 (MasterClient에게 RPC)
        BearController bear = other.GetComponentInParent<BearController>();
        if (bear != null)
        {
            bear.photonView.RPC(
                nameof(BearController.TakeDamage),
                RpcTarget.MasterClient,
                _owner.Stat.AttackPower,
                _owner.PhotonView.Owner.NickName
            );
        }
    }
}
