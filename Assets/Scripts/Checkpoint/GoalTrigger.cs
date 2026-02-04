using System;
using ProjectAction.AutoAttributes;
using ProjectAction.Core;
using ProjectAction.Player;
using UnityEngine;

namespace ProjectAction.Checkpoint
{
    [RequireComponent(typeof(Collider))]
    public sealed class GoalTrigger : ProjectBehaviour
    {
        [SerializeField, GetComponent] private TriggerVisual _visual;

        public event Action Triggered;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<PlayerController>();
            if (player == null)
            {
                return;
            }

            _visual?.SetActive();

            Triggered?.Invoke();
        }
    }
}
