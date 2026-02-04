using System;
using ProjectAction.AutoAttributes;
using ProjectAction.Core;
using ProjectAction.Player;
using UnityEngine;

namespace ProjectAction.Checkpoint
{
    [RequireComponent(typeof(Collider))]
    public sealed class CheckpointTrigger : ProjectBehaviour
    {
        [SerializeField] private Transform _respawnPoint;
        [SerializeField, GetComponent] private TriggerVisual _visual;

        public event Action<CheckpointTrigger> Triggered;

        public Vector3 RespawnPosition => _respawnPoint != null ? _respawnPoint.position : transform.position;
        public Quaternion RespawnRotation => _respawnPoint != null ? _respawnPoint.rotation : transform.rotation;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<PlayerController>();
            if (player == null)
            {
                return;
            }

            _visual?.SetActive();

            Triggered?.Invoke(this);
        }
    }
}
