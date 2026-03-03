using System;
using UnityEngine;

public class BearStat : MonoBehaviour
{
    [Header("Movement")]
    public float WalkSpeed = 2f;
    public float ChaseSpeed = 4f;
    public float ReturnSpeed = 5f;

    [Header("Range")]
    public float DetectRange = 8f;
    public float AttackRange = 2f;
    public float PatrolRadius = 6f;

    [Header("Combat")]
    public float AttackPower = 20f;
    public float AttackCooldown = 1.5f;

    [Header("Patrol")]
    public float IdleTimeMin = 2f;
    public float IdleTimeMax = 5f;

    [Header("Recovery")]
    public int RecoveryTicks = 5;
    public float RecoveryDuration = 3f;

    [Header("HP")]
    public float _hp = 150f;
    public float _maxHp = 150f;

    public float MaxHp => _maxHp;
    public float HP
    {
        get => _hp;
        set
        {
            _hp = Mathf.Clamp(value, 0f, _maxHp);
            OnHpChanged?.Invoke(_hp, _maxHp);
        }
    }

    public event Action<float, float> OnHpChanged;
}
