using UnityEngine;
using Photon.Pun;

public class PlayerWeaponHitAbility : PlayerAbility
{
    private void OnTriggerEnter(Collider other)
    {
        if (!_owner.PhotonView.IsMine) return;
        if (_owner.IsDead) return;
        if (other.transform.root == _owner.transform.root) return;

        PlayerController target = other.GetComponentInParent<PlayerController>();
        if (target == null) return;
        if (target.IsDead) return;

        target.PhotonView.RPC(nameof(PlayerController.TakeDamage), target.PhotonView.Owner, 10f);
    }
}
