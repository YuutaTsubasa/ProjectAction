using System;
using UnityEngine;

namespace ProjectAction.Checkpoint
{
    [RequireComponent(typeof(Collider))]
    public sealed class GoalTrigger : MonoBehaviour
    {
        public event Action Triggered;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<ProjectAction.Player.PlayerController>();
            if (player == null)
            {
                return;
            }

            var visual = GetComponent<TriggerVisual>();
            visual?.SetActive();

            Triggered?.Invoke();
        }
    }
}
