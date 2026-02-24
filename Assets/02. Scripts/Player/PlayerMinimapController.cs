using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerMinimapController : MonoBehaviourPun
{
    [Header("Minimap Camera")]
    public Camera MinimapCamera;

    [Header("Minimap Player Icon")]
    public Image MinimapIcon;
    public Sprite MyPlayerSprite;
    public Sprite OtherPlayerSprite;

    private RenderTexture _myRenderTexture;

    private void Start()
    {
        if (photonView.IsMine)
        {
            _myRenderTexture = new RenderTexture(256, 256, 16);
            MinimapCamera.targetTexture = _myRenderTexture;

            RawImage minimapUI = GameObject.Find("MinimapRawImage")?.GetComponent<RawImage>();
            if (minimapUI != null)
                minimapUI.texture = _myRenderTexture;

            if (MinimapIcon != null && MyPlayerSprite != null)
                MinimapIcon.sprite = MyPlayerSprite;
        }
        else
        {
            MinimapCamera.enabled = false;

            if (MinimapIcon != null && OtherPlayerSprite != null)
                MinimapIcon.sprite = OtherPlayerSprite;
        }
    }

    private void OnDestroy()
    {
        if (_myRenderTexture != null)
        {
            _myRenderTexture.Release();
            Destroy(_myRenderTexture);
        }
    }
}
