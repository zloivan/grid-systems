using UnityEngine;
using UnityEngine.UI;

namespace IKhom.grid_systems.Runtime.components
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private Text _debugLabel;
        private object _gridObject;

        public virtual void SetGridObject(object gridObject)
        {
            _gridObject = gridObject;
        }

        protected virtual void Update()
        {
            if (_gridObject != null)
                _debugLabel.text = _gridObject.ToString();
        }
    }
}