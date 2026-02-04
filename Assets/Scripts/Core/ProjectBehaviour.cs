using ProjectAction.AutoAttributes;
using UnityEngine;

namespace ProjectAction.Core
{
    public abstract class ProjectBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            AutoComponentBinder.Bind(this, false);
            OnProjectAwake();
        }

        protected virtual void OnProjectAwake()
        {
        }
    }
}
