using UnityEngine;

namespace ProjectAction.AssetPolicy
{
    [CreateAssetMenu(menuName = "Project/Global Settings", fileName = "GlobalSettings")]
    public sealed class GlobalSettings : ScriptableObject
    {
        [SerializeField] private AssetAccessMode _assetAccessMode = AssetAccessMode.Restricted;

        private static GlobalSettings _current;

        public AssetAccessMode AssetAccessMode => _assetAccessMode;

        public static GlobalSettings Current
        {
            get
            {
                if (_current != null)
                {
                    return _current;
                }

                _current = Resources.Load<GlobalSettings>("GlobalSettings");
                return _current;
            }
        }

        public static AssetAccessMode CurrentMode =>
            Current != null ? Current.AssetAccessMode : AssetAccessMode.Restricted;
    }
}
