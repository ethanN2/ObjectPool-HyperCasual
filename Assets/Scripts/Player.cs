/*******************************************************************
* Copyright         : 2025 Ethan
* File Name         : Player
* Description       : this is MonoBehaviour template i created.
*                     If you want to improve it. Just go to
*                     https://github.com/ethanN2/Unity_Script_Template
/******************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.Serialization;

[AddComponentMenu("Default/Player")]
public class Player : MonoBehaviour
{
    #region Enums

    #endregion

    #region Fields

    [SerializeField] private Camera camera;
    [SerializeField] private float  distanceSpawnerObject;
    [SerializeField] private float  rotationSpeed;
    [SerializeField] private float  forceToBall;
    [SerializeField] private int    timeToSpawnEachBallInSeconds;

    private Vector2   startTouchPosition;
    private bool      isDragging = false;
    private Coroutine _coroutine;

    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Unity Methods

    // Awake is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // These functions will be called when the attached GameObject is enabled.
    private void OnEnable()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (!PoolSystem.HasInstance || !PoolSystem.Instance.IsInitialized) return;

        var isStartSpawn = false;
        // TODO: Touch on Screen
        // Mobile touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            HandleTouchRotation(touch.phase, touch.position);
            isStartSpawn = true;
        }

        // Mouse input (for editor testing)
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
            isDragging         = true;
            isStartSpawn       = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 currentPosition = Input.mousePosition;
            RotateCamera(currentPosition);
            isStartSpawn = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // TODO: Spawn Object 30 balls in 1 minute  ->  so every 2 secs we shoot 1 ball
        if (isStartSpawn)
        {
            _coroutine ??= StartCoroutine(ShootBallIn(timeToSpawnEachBallInSeconds));
        }
        else if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    // These functions will be called when the attached GameObject is toggled.
    private void OnDisable()
    {
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (camera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(camera.transform.position, camera.transform.position + camera.transform.forward * distanceSpawnerObject);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(camera.transform.position + camera.transform.forward * distanceSpawnerObject, 0.1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
    }

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    private IEnumerator ShootBallIn(int seconds)
    {
        if (seconds > 0)
            yield return new WaitForSeconds(seconds);

        var ball = PoolSystem.Instance.GetObject().GetComponent<IBall>();
        if (ball == null)
        {
            throw new Exception("Ball doesn't have BallBehaviour");
        }

        // TODO: Shoot Object
        ball.SetPosition(camera.transform.position + camera.transform.forward * distanceSpawnerObject);
        ball.Shoot(camera.transform.forward, forceToBall);
        _coroutine = null;
    }

    private void HandleTouchRotation(TouchPhase touchPhase, Vector2 touchPosition)
    {
        switch (touchPhase)
        {
            case TouchPhase.Began:
                startTouchPosition = touchPosition;
                isDragging         = true;
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (isDragging)
                {
                    RotateCamera(touchPosition);
                }

                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isDragging = false;
                break;
        }
    }

    private void RotateCamera(Vector3 currentPosition)
    {
        float deltaX = currentPosition.x - startTouchPosition.x;

        // Rotate only if dragging horizontally
        if (Mathf.Abs(deltaX) > 5f)
        {
            // Determine rotation direction based on touch side
            float screenMid = Screen.width / 2;

            if (startTouchPosition.x < screenMid && deltaX < 0)
            {
                // Left half: rotate left
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }
            else if (startTouchPosition.x >= screenMid && deltaX > 0)
            {
                // Right half: rotate right
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }

            startTouchPosition = currentPosition;
        }
    }

    #endregion
}