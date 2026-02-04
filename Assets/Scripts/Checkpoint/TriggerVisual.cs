using UnityEngine;
using UnityEngine.Rendering;

namespace ProjectAction.Checkpoint
{
    public sealed class TriggerVisual : MonoBehaviour
    {
        private static readonly int BASE_COLOR_ID = Shader.PropertyToID("_BaseColor");

        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _inactiveColor = new Color(1f, 0.95f, 0.2f, 0.35f);
        [SerializeField] private Color _activeColor = new Color(0.1f, 1f, 0.2f, 0.6f);

        public void SetInactive()
        {
            ApplyColor(_inactiveColor);
        }

        public void SetActive()
        {
            ApplyColor(_activeColor);
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

            var material = EnsureMaterial(_renderer);
            if (material == null)
            {
                return;
            }

            material.SetColor(BASE_COLOR_ID, color);
        }

        private static Material EnsureMaterial(Renderer renderer)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Standard");
            if (shader == null)
            {
                return null;
            }

            var material = renderer.material;
            if (material == null || material.shader != shader)
            {
                material = new Material(shader);
                renderer.material = material;
            }

            ConfigureTransparentMaterial(material);
            return material;
        }

        private static void ConfigureTransparentMaterial(Material material)
        {
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetFloat("_AlphaClip", 0f);
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)RenderQueue.Transparent;
        }
    }
}
