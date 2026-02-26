using UnityEngine;

public static class GameObjectArrayExtensions
{
    public static void ApplyRadialForce(this GameObject[] items, float upForce, float spreadForce)
    {
        int count = items.Length;
        for (int i = 0; i < count; i++)
        {
            Rigidbody rb = items[i].GetComponent<Rigidbody>();
            if (rb == null) continue;

            float angle = (360f / count) * i * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 force = Vector3.up * upForce + dir * spreadForce;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
