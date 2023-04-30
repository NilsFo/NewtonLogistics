using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class MainCameraController : MonoBehaviour
{
    private GameStateBehaviourScript _gameState;
    private CinemachineVirtualCamera virtualCamera;
    public GameObject cameraObj;
    public Transform CameraPos => cameraObj.transform;
    public GameObject secretFollowObj;
    private Camera camera;

    [Header("Who to look at?")] public List<GameObject> followList;

    [Header("Zoom preferences")] public float zoomMax = 90;
    private float zoomMinCurrent = 0;
    public float zoomMinNear = 0;
    public float zoomMinFar = 0;
    private float defaultZ;
    public float orthographicZoomScaleDefault = 0.5f;
    public float orthographicZoomScaleZoomOut = 0.5f;
    private float orthographicZoomScaleCurrent = 0;
    public float orthographicZoomSpeedAuto = 1.0f;
    public float orthographicZoomSpeedPlayerRequest = 4.0f;
    public bool playerRequestsZoomOut;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindObjectOfType<GameStateBehaviourScript>();
        defaultZ = transform.position.z;
        orthographicZoomScaleCurrent = orthographicZoomScaleDefault;

        if (followList == null)
        {
            followList = new List<GameObject>();
        }

        if (followList.Count == 0)
        {
            ResetFollowToPlayer();
        }

        camera = Camera.main;
        var brain = (camera == null) ? null : camera.GetComponent<CinemachineBrain>();
        virtualCamera = (brain == null) ? null : brain.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (virtualCamera != null)
        {
            zoomMinNear = virtualCamera.m_Lens.OrthographicSize;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (followList.Count == 0)
        {
            ResetFollowToPlayer();
        }

        secretFollowObj.transform.position = _gameState.player.transform.position;

        // input
        playerRequestsZoomOut = Keyboard.current.tabKey.isPressed;

        // Moving & Zooming Camera
        if (virtualCamera != null)
        {
            // Move
            secretFollowObj.transform.position = GetMiddlePos() + GetVelocityVector();

            // Lerping Zoom
            float lz = orthographicZoomScaleDefault;
            float lm = zoomMinNear;
            float orthographicZoomSpeedUsed = orthographicZoomSpeedAuto;
            if (playerRequestsZoomOut)
            {
                lz = orthographicZoomScaleZoomOut;
                lm = zoomMinFar;
                orthographicZoomSpeedUsed = orthographicZoomSpeedPlayerRequest;
            }

            orthographicZoomScaleCurrent =
                Mathf.Lerp(orthographicZoomScaleCurrent, lz, Time.deltaTime * orthographicZoomSpeedPlayerRequest);
            zoomMinCurrent =
                Mathf.Lerp(zoomMinCurrent, lm, Time.deltaTime * orthographicZoomSpeedPlayerRequest);

            // Zoom
            Bounds bounds = GetFollowBounds();
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = bounds.size.x / bounds.size.y;
            float dim = Mathf.Max(bounds.size.x, bounds.size.y / 2.0f);
            dim = dim * orthographicZoomScaleCurrent;
            float zoom = Mathf.Clamp(dim, zoomMinCurrent, zoomMax);

            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, zoom,
                Time.deltaTime * orthographicZoomSpeedUsed);
        }
    }

    public Vector3 GetVelocityVector()
    {
        var velocity = _gameState.player.rb.velocity;
        return velocity;
    }

    public Vector3 GetMiddlePos()
    {
        Vector3 ret = new Vector3();
        Bounds bounds = GetFollowBounds();

        ret.z = defaultZ;
        ret.x = bounds.center.x;
        ret.y = bounds.center.y;

        return ret;
    }

    private Bounds GetFollowBounds()
    {
        Bounds bounds = new Bounds(followList[0].transform.position, new Vector3());
        for (var i = 0; i < followList.Count; i++)
        {
            bounds.Encapsulate(followList[i].transform.position);
        }

        return bounds;
    }

    public void ResetFollowToPlayer()
    {
        followList.Clear();
        followList.Add(_gameState.player.gameObject);
    }

    public void AddFollowTarget(GameObject o)
    {
        if (!followList.Contains(o))
        {
            followList.Add(o);
        }
    }

    public bool RemoveFollowTarget(GameObject o)
    {
        return followList.Remove(o);
    }
}