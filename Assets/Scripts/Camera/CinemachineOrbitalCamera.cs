using Unity.Cinemachine;
using UnityEngine;
using ProjectAction.Core;

namespace ProjectAction.Camera
{
    [DisallowMultipleComponent]
    public sealed class CinemachineOrbitalCamera : ProjectBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineCamera _virtualCamera;
        [SerializeField] private CinemachineOrbitalFollow _orbitalFollow;
        [SerializeField] private CinemachineRotationComposer _rotationComposer;
        [SerializeField] private Transform _target;

        [Header("Orbit")]
        [SerializeField] private float _distance = 8f;
        [SerializeField] private float _height = 0f;
        [SerializeField] private float _minPitch = -15f;
        [SerializeField] private float _maxPitch = 45f;
        [SerializeField] private float _lookSensitivity = 0.2f;
        [SerializeField] private float _positionDamping = 0.08f;
        [SerializeField] private float _lookAtHeight = 0f;

        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        private void Reset()
        {
            _virtualCamera = GetComponent<CinemachineCamera>();
            _orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
            _rotationComposer = GetComponent<CinemachineRotationComposer>();
        }

        private void OnValidate()
        {
            _distance = Mathf.Max(0.01f, _distance);
            if (_maxPitch < _minPitch)
            {
                (_minPitch, _maxPitch) = (_maxPitch, _minPitch);
            }

            ApplyOrbitSettings();
        }

        public void Tick(ProjectAction.Input.InputState input, float deltaTime)
        {
            if (_orbitalFollow == null || _virtualCamera == null || _target == null)
            {
                return;
            }

            ApplyTargets();
            ApplyOrbitSettings();
            ApplyRotationSettings();

            var look = input.Look.Value * _lookSensitivity;
            _orbitalFollow.HorizontalAxis.Value += look.x;
            _orbitalFollow.VerticalAxis.Value = Mathf.Clamp(
                _orbitalFollow.VerticalAxis.Value - look.y,
                _minPitch,
                _maxPitch);
        }

        public void SnapToTarget()
        {
            if (_orbitalFollow == null || _virtualCamera == null || _target == null)
            {
                return;
            }

            var rotation = Quaternion.Euler(_orbitalFollow.VerticalAxis.Value, _orbitalFollow.HorizontalAxis.Value, 0f);
            var desiredPosition = _target.position + rotation * new Vector3(0f, _height, -_distance);
            _orbitalFollow.ForceCameraPosition(desiredPosition, rotation);
        }

        private void ApplyTargets()
        {
            _virtualCamera.Target = new CameraTarget
            {
                TrackingTarget = _target,
                LookAtTarget = _target,
                CustomLookAtTarget = true
            };
        }

        private void ApplyOrbitSettings()
        {
            if (_orbitalFollow == null)
            {
                return;
            }

            _orbitalFollow.TargetOffset = new Vector3(0f, _height, 0f);
            _orbitalFollow.Radius = _distance;
            _orbitalFollow.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
            _orbitalFollow.HorizontalAxis.Wrap = true;
            _orbitalFollow.HorizontalAxis.Range = new Vector2(-180f, 180f);
            _orbitalFollow.VerticalAxis.Wrap = false;
            _orbitalFollow.VerticalAxis.Range = new Vector2(_minPitch, _maxPitch);
            _orbitalFollow.TrackerSettings.PositionDamping = new Vector3(_positionDamping, _positionDamping, _positionDamping);
        }

        private void ApplyRotationSettings()
        {
            if (_rotationComposer == null)
            {
                return;
            }

            _rotationComposer.TargetOffset = new Vector3(0f, _lookAtHeight, 0f);
        }
    }
}
