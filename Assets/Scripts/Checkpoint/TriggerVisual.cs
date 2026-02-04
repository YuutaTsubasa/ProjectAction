using UnityEngine;

namespace ProjectAction.Checkpoint
{
    public sealed class TriggerVisual : MonoBehaviour
    {
        private static readonly int BASE_COLOR_ID = Shader.PropertyToID("_BaseColor");

        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _inactiveColor = new Color(1f, 0.95f, 0.2f, 0.35f);
        [SerializeField] private Color _activeColor = new Color(0.1f, 1f, 0.2f, 0.6f);

        private Material _cachedMaterial;
        private bool _ownsCachedMaterial;
        private MaterialPropertyBlock _propertyBlock;

        public void SetInactive()
        {
            ApplyColor(_inactiveColor);
        }

        public void SetActive()
        {
            ApplyColor(_activeColor);
        }

        private void OnDestroy()
        {
            ReleaseCachedMaterial();
        }

        private void EnsureRenderer()
        {
            if (_renderer != null)
            {
                return;
            }

            _renderer = GetComponent<Renderer>();
        }

        private void ApplyColor(Color color)
        {
            EnsureRenderer();
            if (_renderer == null)
            {
                return;
            }

            if (!EnsureMaterial())
            {
                return;
            }

            _propertyBlock ??= new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(BASE_COLOR_ID, color);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        private bool EnsureMaterial()
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Standard");
            if (shader == null)
            {
                return false;
            }

            var material = _renderer.sharedMaterial;
            if (material == null || material.shader != shader)
            {
                if (_cachedMaterial == null || _cachedMaterial.shader != shader)
                {
                    ReleaseCachedMaterial();
                    _cachedMaterial = new Material(shader);
                    _ownsCachedMaterial = true;
                }

                material = _cachedMaterial;
                _renderer.sharedMaterial = material;
            }
            else if (_cachedMaterial != material)
            {
                ReleaseCachedMaterial();
                _cachedMaterial = material;
                _ownsCachedMaterial = false;
            }

            TriggerMaterialUtility.ConfigureTransparentMaterial(material);
            return true;
        }

        private void ReleaseCachedMaterial()
        {
            if (!_ownsCachedMaterial || _cachedMaterial == null)
            {
                _cachedMaterial = null;
                _ownsCachedMaterial = false;
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(_cachedMaterial);
            }
            else
            {
                DestroyImmediate(_cachedMaterial);
            }

            _cachedMaterial = null;
            _ownsCachedMaterial = false;
        }

    }
}
