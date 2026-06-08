using CameraController.Interface;
using CameraController.Service;
using PlayerInput.Interface;
using UnityEngine;
using Zenject;
namespace Gameplay.Component
{
    public class MoveComponent : MonoBehaviour
    {
        [Inject]
        private IInputModule _input;
        [Inject]
        private ICameraDirection _cameraDirection;
        [SerializeField]
        private float _moveSpeed = 5f;

        [SerializeField]
        private float _rotateSpeed = 15f;

        private Transform _selfTransform;

        private void Awake()
        {
            _selfTransform = transform;
        }

        void Update()
        {
            
            RotateTowardsCameraForward();

            MoveByCameraDirection();
        }

        private void RotateTowardsCameraForward()
        {
            Vector3 cameraForward = _cameraDirection.Forward;

            // 相机没有有效方向就退出
            if (cameraForward.magnitude < 0.01f) return;

            // 平滑旋转朝向相机前方
            _selfTransform.rotation = Quaternion.Lerp(
                _selfTransform.rotation,
                Quaternion.LookRotation(cameraForward),
                _rotateSpeed * Time.deltaTime
            );
        }

        private void MoveByCameraDirection()
        {
            Vector2 inputDir = _input.MoveInput;
            if (inputDir.magnitude < 0.1f) return;

            // 计算移动方向
            Vector3 moveDir = _cameraDirection.Forward * inputDir.y + _cameraDirection.Right * inputDir.x;
            moveDir.y = 0; // 保持水平移动
            moveDir.Normalize();

            // 世界空间移动
            _selfTransform.Translate(moveDir * _moveSpeed * Time.deltaTime, Space.World);
        }
    }
}