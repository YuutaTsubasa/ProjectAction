using System;
using UnityEngine;

namespace ProjectAction.Checkpoint
{
    [RequireComponent(typeof(Collider))]
    public sealed class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField] private Transform _respawnPoint;

        public event Action<CheckpointTrigger> Triggered;

        public Vector3 RespawnPosition => _respawnPoint != null ? _respawnPoint.position : transform.position;
        public Quaternion RespawnRotation => _respawnPoint != null ? _respawnPoint.rotation : transform.rotation;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<ProjectAction.Player.PlayerController>();
            if (player == null)
            {
                return;
            }

            var visual = GetComponent<TriggerVisual>();
            visual?.SetActive();

            Triggered?.Invoke(this);
        }
    }
}
