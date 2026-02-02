using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ProjectAction.Core
{
    public sealed class RootScene : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ProjectAction.Player.PlayerController _player;
        [SerializeField] private ProjectAction.Camera.CinemachineOrbitalCamera _camera;
        [SerializeField] private ProjectAction.Input.VirtualInputBridge _virtualInput;
        [SerializeField] private Transform _spawnPoint;

        [Header("Death")]
        [SerializeField] private float _killY = -10f;

        private bool _isComplete;

        private void Start()
        {
            Run().Forget();
        }

        private async UniTask Run()
        {
            var token = this.GetCancellationTokenOnDestroy();
            var inputService = new ProjectAction.Input.InputService(_virtualInput);
            var checkpointService = new ProjectAction.Checkpoint.CheckpointService();
            System.Action goalHandler = null;

            checkpointService.SetInitial(_spawnPoint != null ? _spawnPoint : _player != null ? _player.transform : null);
            if (_player != null && _camera != null)
            {
                _player.MoveReference = _camera.transform;
            }

            var checkpoints = UnityEngine.Object.FindObjectsByType<ProjectAction.Checkpoint.CheckpointTrigger>(
                FindObjectsSortMode.None);
            foreach (var checkpoint in checkpoints)
            {
                checkpoint.Triggered += checkpointService.SetCheckpoint;
            }

            var goal = UnityEngine.Object.FindFirstObjectByType<ProjectAction.Checkpoint.GoalTrigger>();
            if (goal != null)
            {
                goalHandler = () =>
                {
                    _isComplete = true;
                    Debug.Log("Goal reached.");
                };
                goal.Triggered += goalHandler;
            }

            try
            {
                while (!token.IsCancellationRequested)
                {
                    inputService.Update();

                    var deltaTime = Time.deltaTime;
                    _player?.Tick(inputService.State, deltaTime);
                    _camera?.Tick(inputService.State, deltaTime);

                    if (!_isComplete && _player != null && _player.transform.position.y < _killY)
                    {
                        Respawn(_player, checkpointService);
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            finally
            {
                foreach (var checkpoint in checkpoints)
                {
                    checkpoint.Triggered -= checkpointService.SetCheckpoint;
                }

                if (goal != null && goalHandler != null)
                {
                    goal.Triggered -= goalHandler;
                }

                inputService.Dispose();
            }
        }

        private void Respawn(ProjectAction.Player.PlayerController player, ProjectAction.Checkpoint.CheckpointService checkpoint)
        {
            player.ResetForRespawn(checkpoint.RespawnPosition, checkpoint.RespawnRotation);
            _camera?.SnapToTarget();
        }
    }
}
