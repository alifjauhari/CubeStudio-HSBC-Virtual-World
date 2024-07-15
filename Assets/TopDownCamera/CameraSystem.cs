using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace CodeMonkey.CameraSystem
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private bool useEdgeScrolling = false;
        [SerializeField] private bool useDragPan = false;
        [SerializeField] private float fieldOfViewMax = 50;
        [SerializeField] private float fieldOfViewMin = 10;
        [SerializeField] private float followOffsetMin = 5f;
        [SerializeField] private float followOffsetMax = 50f;
        [SerializeField] private float followOffsetMinY = 10f;
        [SerializeField] private float followOffsetMaxY = 50f;
        [SerializeField] private float cameraMovementSpeed;
        [SerializeField] private float edgeScrollingSpeed;
        [SerializeField] private float dragSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float keyRotationSpeedMultiplier = 3f; // Multiplier for Q and E key rotation

        private bool dragPanMoveActive;
        private Vector2 lastMousePosition;
        private float targetFieldOfView = 50;
        private Vector3 followOffset;

        private void Awake()
        {
            followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        }

        private void Update()
        {
            HandleCameraMovement();

            if (useEdgeScrolling)
            {
                HandleCameraMovementEdgeScrolling();
            }

            if (useDragPan)
            {
                HandleCameraMovementDragPan();
            }

            HandleCameraRotation();

            //HandleCameraZoom_FieldOfView();
            HandleCameraZoom_MoveForward();
            //HandleCameraZoom_LowerY();
        }

        private void HandleCameraMovement()
        {
            Vector3 inputDir = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
            if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
            if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
            if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

            transform.position += moveDir * cameraMovementSpeed * Time.deltaTime;
        }

        private void HandleCameraMovementEdgeScrolling()
        {
            Vector3 inputDir = new Vector3(0, 0, 0);

            int edgeScrollSize = 20;

            if (Input.mousePosition.x < edgeScrollSize)
            {
                inputDir.x = -1f;
            }
            if (Input.mousePosition.y < edgeScrollSize)
            {
                inputDir.z = -1f;
            }
            if (Input.mousePosition.x > Screen.width - edgeScrollSize)
            {
                inputDir.x = +1f;
            }
            if (Input.mousePosition.y > Screen.height - edgeScrollSize)
            {
                inputDir.z = +1f;
            }

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

            transform.position += moveDir * edgeScrollingSpeed * Time.deltaTime;
        }

        private void HandleCameraMovementDragPan()
        {
            Vector3 inputDir = new Vector3(0, 0, 0);

            if (Input.GetMouseButtonDown(0))
            {
                dragPanMoveActive = true;
                lastMousePosition = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                dragPanMoveActive = false;
            }

            if (dragPanMoveActive)
            {
                Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;

                float dragPanSpeed = 1f;
                inputDir.x = -mouseMovementDelta.x * dragPanSpeed;
                inputDir.z = -mouseMovementDelta.y * dragPanSpeed;

                lastMousePosition = Input.mousePosition;
            }

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

            transform.position += moveDir * dragSpeed * Time.deltaTime;
        }

        private void HandleCameraRotation()
        {
            // Mouse Right-Click Rotation
            if (Input.GetMouseButton(1))
            {
                Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;
                transform.eulerAngles += new Vector3(0, mouseMovementDelta.x * rotationSpeed, 0);
            }

            // Q and E Keys Rotation
            if (Input.GetKey(KeyCode.Q))
            {
                transform.eulerAngles += new Vector3(0, rotationSpeed * Time.deltaTime * keyRotationSpeedMultiplier, 0);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.eulerAngles += new Vector3(0, -rotationSpeed * Time.deltaTime * keyRotationSpeedMultiplier, 0);
            }

            // Touchpad Two-Finger Swipe Rotation
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroLastPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOneLastPos = touchOne.position - touchOne.deltaPosition;

                Vector2 lastDir = touchZeroLastPos - touchOneLastPos;
                Vector2 currentDir = touchZero.position - touchOne.position;

                float angle = Vector2.SignedAngle(lastDir, currentDir);

                Debug.Log($"Touch 0: {touchZero.position}, Touch 1: {touchOne.position}, Angle: {angle}");

                transform.eulerAngles += new Vector3(0, angle * rotationSpeed, 0);
            }

            lastMousePosition = Input.mousePosition;
        }

        private void HandleCameraZoom_FieldOfView()
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                targetFieldOfView -= 5;
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                targetFieldOfView += 5;
            }

            targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);

            float zoomSpeed = 10f;
            cinemachineVirtualCamera.m_Lens.FieldOfView =
                Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
        }

        private void HandleCameraZoom_MoveForward()
        {
            Vector3 zoomDir = followOffset.normalized;

            float zoomAmount = 3f;
            if (Input.mouseScrollDelta.y > 0)
            {
                followOffset -= zoomDir * zoomAmount;
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                followOffset += zoomDir * zoomAmount;
            }

            if (followOffset.magnitude < followOffsetMin)
            {
                followOffset = zoomDir * followOffsetMin;
            }

            if (followOffset.magnitude > followOffsetMax)
            {
                followOffset = zoomDir * followOffsetMax;
            }

            float zoomSpeed = 10f;
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
        }

        private void HandleCameraZoom_LowerY()
        {
            float zoomAmount = 3f;
            if (Input.mouseScrollDelta.y > 0)
            {
                followOffset.y -= zoomAmount;
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                followOffset.y += zoomAmount;
            }

            followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMinY, followOffsetMaxY);

            float zoomSpeed = 10f;
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
        }

    }

}
