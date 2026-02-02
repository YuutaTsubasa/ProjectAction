using UnityEngine;

namespace ProjectAction.Checkpoint
{
    public sealed class CheckpointService
    {
        private Vector3 _respawnPosition;
        private Quaternion _respawnRotation;

        public Vector3 RespawnPosition => _respawnPosition;
        public Quaternion RespawnRotation => _respawnRotation;

        public void SetInitial(Transform transform)
        {
            if (transform == null)
            {
                return;
            }

            _respawnPosition = transform.position;
            _respawnRotation = transform.rotation;
        }

        public void SetCheckpoint(CheckpointTrigger checkpoint)
        {
            if (checkpoint == null)
            {
                return;
            }

            _respawnPosition = checkpoint.RespawnPosition;
            _respawnRotation = checkpoint.RespawnRotation;
        }
    }
}
