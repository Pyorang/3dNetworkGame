using UnityEngine;

public class PlayerWeaponColliderAbility : MonoBehaviour
{
    [SerializeField] private Collider _collider;

    private void Start()
    {
        DeactiveCollider();
    }

    public void ActivateCollider()
    {
        _collider.enabled = true;
    }

    public void DeactiveCollider()
    {
        _collider.enabled = false;
    }
}
