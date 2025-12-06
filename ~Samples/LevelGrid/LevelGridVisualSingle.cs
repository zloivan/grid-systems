using UnityEngine;

namespace _Game.Scripts.Services.Grid.LevelGrid
{
    public class LevelGridVisualSingle : MonoBehaviour
    {
        private const string EMISSION = "_EMISSION";
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        [SerializeField] private float _emissionIntensity = 2f;
        
        private MeshRenderer _meshRenderer;
        private Material _instanceMaterial;
        private Color _originalColor;
        private bool _isHighlighted;


        private void Awake() => _meshRenderer = GetComponentInChildren<MeshRenderer>();

        public void Show(Material materialForGridVisual)
        {
            _meshRenderer.enabled = true;

            _instanceMaterial = new Material(materialForGridVisual);
            _meshRenderer.material = _instanceMaterial;

            _originalColor = _instanceMaterial.color;

            _instanceMaterial.DisableKeyword("_EMISSION");
            _isHighlighted = false;
        }

        public void Hide()
        {
            _meshRenderer.enabled = false;
            _isHighlighted = false;

            if (!_instanceMaterial) 
                return;
            
            Destroy(_instanceMaterial);
            _instanceMaterial = null;
        }

        public void Highlight()
        {
            if (_meshRenderer.enabled == false || _isHighlighted || !_instanceMaterial)
                return;

            _instanceMaterial.EnableKeyword(EMISSION);
            _instanceMaterial.SetColor(EmissionColor, _originalColor * _emissionIntensity);

            _isHighlighted = true;
        }

        public void RemoveHighlight()
        {
            if (_meshRenderer.enabled == false || !_isHighlighted || !_instanceMaterial)
                return;

            _instanceMaterial.DisableKeyword(EMISSION);
            _instanceMaterial.SetColor(EmissionColor, Color.black);

            _isHighlighted = false;
        }
    }
}