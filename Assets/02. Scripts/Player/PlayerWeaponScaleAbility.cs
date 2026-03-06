using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerWeaponScaleAbility : MonoBehaviourPunCallbacks
{
    [Header("Settings")]
    [SerializeField] private Transform weaponTransform;
    private const float BaseScaleIncrement = 0.1f;
    private const float DefaultScale = 1.0f;

    private void OnEnable()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(WaitAndSubscribe());
        }
    }

    private void OnDisable()
    {
        if (photonView.IsMine && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnLocalScoreChanged -= RequestScaleSync;
        }
    }

    private IEnumerator WaitAndSubscribe()
    {
        while (ScoreManager.Instance == null)
        {
            yield return null;
        }

        ScoreManager.Instance.OnLocalScoreChanged -= RequestScaleSync;
        ScoreManager.Instance.OnLocalScoreChanged += RequestScaleSync;

        RequestScaleSync(ScoreManager.Instance.GetLocalScore());
    }

    private void RequestScaleSync(int score)
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(SyncWeaponScaleRPC), RpcTarget.AllBuffered, score);
        }
    }

    [PunRPC]
    private void SyncWeaponScaleRPC(int score)
    {
        if (weaponTransform == null) return;

        int scaleStep = score / 1000;
        float newScale = DefaultScale + (scaleStep * BaseScaleIncrement);

        weaponTransform.localScale = Vector3.one * newScale;
    }
}