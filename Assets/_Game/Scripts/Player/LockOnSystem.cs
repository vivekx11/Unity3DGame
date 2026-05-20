using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Soft lock-on system. Smoothly orbits camera around target using simple interpolation. Uses a target transform assigned by player input.
    /// Editor tips: Requires a Cinemachine Virtual Camera in the scene. This script provides a target to camera; tie into Cinemachine's Follow/LookAt.
    /// Design notes: Lightweight; for full feature set integrate with Cinemachine ClearShot or custom camera rigs.
    /// </summary>
    public class LockOnSystem : MonoBehaviour
    {
        public Transform currentTarget;
        public float orbitSpeed = 3f;
        public float transitionSpeed = 5f;

        private Transform cameraTarget; // a pivot the camera follows

        private void Start()
        {
            cameraTarget = new GameObject("CameraPivot").transform;
            cameraTarget.position = transform.position + Vector3.up * 1.5f;
        }

        private void Update()
        {
            if (currentTarget == null)
            {
                cameraTarget.position = Vector3.Lerp(cameraTarget.position, transform.position + Vector3.up * 1.5f, Time.deltaTime * transitionSpeed);
                return;
            }

            // orbit around target
            cameraTarget.position = Vector3.Lerp(cameraTarget.position, currentTarget.position + Vector3.up * 1.5f, Time.deltaTime * transitionSpeed);
            cameraTarget.RotateAround(currentTarget.position, Vector3.up, orbitSpeed * Time.deltaTime);
        }

        public void SetTarget(Transform t)
        {
            currentTarget = t;
        }
    }
}
