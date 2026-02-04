using System;
using UnityEngine;

namespace ProjectAction.Checkpoint
{
    [RequireComponent(typeof(Collider))]
    public sealed class GoalTrigger : MonoBehaviour
    {
        [SerializeField] private TriggerVisual _visual;

        public event Action Triggered;

        private void EnsureVisual()
        {
            if (_visual == null)
            {
                _visual = GetComponent<TriggerVisual>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<ProjectAction.Player.PlayerController>();
            if (player == null)
            {
                return;
            }

            EnsureVisual();
            _visual?.SetActive();

            Triggered?.Invoke();
        }
    }
}
