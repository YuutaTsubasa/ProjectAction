using UnityEngine;

namespace ProjectAction.AssetPolicy
{
    public abstract class AssetAdapterBase : ScriptableObject
    {
        public abstract Object AssetObject { get; }
    }

    public abstract class AssetAdapter<TAsset> : AssetAdapterBase where TAsset : Object
    {
        [SerializeField] private TAsset _primaryAsset;
        [SerializeField] private TAsset _fallbackAsset;

        public TAsset Asset
        {
            get
            {
                if (GlobalSettings.CurrentMode == AssetAccessMode.Restricted)
                {
                    return _fallbackAsset;
                }

                return _primaryAsset != null ? _primaryAsset : _fallbackAsset;
            }
        }

        public override Object AssetObject => Asset;
    }

    [CreateAssetMenu(menuName = "Project/Asset Adapter/Sprite", fileName = "SpriteAssetAdapter")]
    public sealed class SpriteAssetAdapter : AssetAdapter<Sprite> { }

    [CreateAssetMenu(menuName = "Project/Asset Adapter/Material", fileName = "MaterialAssetAdapter")]
    public sealed class MaterialAssetAdapter : AssetAdapter<Material> { }

    [CreateAssetMenu(menuName = "Project/Asset Adapter/AudioClip", fileName = "AudioClipAssetAdapter")]
    public sealed class AudioClipAssetAdapter : AssetAdapter<AudioClip> { }

    [CreateAssetMenu(menuName = "Project/Asset Adapter/GameObject", fileName = "GameObjectAssetAdapter")]
    public sealed class GameObjectAssetAdapter : AssetAdapter<GameObject> { }

    [CreateAssetMenu(menuName = "Project/Asset Adapter/Texture2D", fileName = "Texture2DAssetAdapter")]
    public sealed class Texture2DAssetAdapter : AssetAdapter<Texture2D> { }
}
