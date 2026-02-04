using System;
using UnityEngine;

namespace ProjectAction.Checkpoint
{
    [RequireComponent(typeof(Collider))]
    public sealed class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField] private Transform _respawnPoint;
        [SerializeField] private TriggerVisual _visual;

        public event Action<CheckpointTrigger> Triggered;

        public Vector3 RespawnPosition => _respawnPoint != null ? _respawnPoint.position : transform.position;
        public Quaternion RespawnRotation => _respawnPoint != null ? _respawnPoint.rotation : transform.rotation;

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

            Triggered?.Invoke(this);
        }
    }
}
