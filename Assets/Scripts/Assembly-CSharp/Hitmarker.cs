using Mirror;
using PlayerRoles.Spectating;
using UnityEngine;

public class Hitmarker : MonoBehaviour
{
    public struct HitmarkerMessage : NetworkMessage
    {
        public byte Size;
        public HitmarkerMessage(byte size) => Size = size;
    }

    [SerializeField] private AnimationCurve _sizeOverTime;
    [SerializeField] private AnimationCurve _opacityOverTime;
    [SerializeField] private CanvasRenderer _targetImage;
    [SerializeField] private float _animationTime;

    private float _timer;
    private float _targetSize;
    private static Hitmarker _singleton;
    private const float MaxSize = 2.55f;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        RegisterClientHandler();
        CustomNetworkManager.OnClientStarted += RegisterClientHandler;
        CustomNetworkManager.OnClientReady += RegisterClientHandler;
    }

    private static void RegisterClientHandler()
    {
        if (NetworkClient.active)
            NetworkClient.ReplaceHandler<HitmarkerMessage>(HitmarkerMsgReceived, false);
    }

    private static void HitmarkerMsgReceived(HitmarkerMessage msg)
    {
        if (_singleton != null)
        {
            _singleton._targetSize = (msg.Size / 255f) * MaxSize;
            _singleton._timer = 0f;
        }
    }

    private void Awake()
    {
        _singleton = this;
    }

    private void Update()
    {
        if (_timer > _animationTime) return;

        _timer += Time.deltaTime;
        _targetImage.SetAlpha(_opacityOverTime.Evaluate(_timer));
        _targetImage.transform.localScale = Vector3.one * (_sizeOverTime.Evaluate(_timer) * _targetSize);
    }

    public static void PlayHitmarker(float size)
    {
        _singleton._targetSize = size;
        _singleton._timer = 0f;
    }

    public static void SendHitmarker(ReferenceHub hub, float size)
    {
        size = Mathf.Clamp(size, 0f, MaxSize);
        if (hub.isLocalPlayer)
        {
            PlayHitmarker(size);
        }
        else
        {
            SpectatorNetworking.SendToSpectatorsOf(new HitmarkerMessage((byte)Mathf.RoundToInt(size / MaxSize * 255f)), hub, includeTarget: true);
        }
    }

    public static void SendHitmarker(NetworkConnection conn, float size)
    {
        if (ReferenceHub.TryGetHub(conn.identity.gameObject, out var hub))
        {
            SendHitmarker(hub, size);
        }
    }
}