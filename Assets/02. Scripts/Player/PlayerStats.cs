using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("이동")]
    public float MoveSpeed = 7f;
    public float JumpForce = 2.5f;
    public float RotationSpeed = 100f;

    [Header("전투")]
    public float AttackPower = 25f;
    public float HP = 100f;
}
