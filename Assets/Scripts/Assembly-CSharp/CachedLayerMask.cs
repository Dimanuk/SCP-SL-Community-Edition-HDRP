
using UnityEngine;

public class CachedLayerMask
{
    public int _cachedMask;

    public readonly string[] _layers;

    public LayerMask Mask
    {
        get
        {
            if (_cachedMask == 0)
            {
                _cachedMask = LayerMask.GetMask(_layers);
            }

            return _cachedMask;
        }
    }

    public CachedLayerMask(params string[] layers)
    {
        _layers = layers;
    }

    public static implicit operator int(CachedLayerMask mask)
    {
        return mask.Mask;
    }
}
