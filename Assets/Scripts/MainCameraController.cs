using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class MainCameraController : MonoBehaviour
{
    private GameStateBehaviourScript _gameState;
    private CinemachineBrain cmBrain;
    private CinemachineVirtualCamera virtualCamera;
    public GameObject cameraObj;
    public Transform CameraPos => cameraObj.transform;
    public GameObject secretFollowObj;
    private Camera camera;

    [Header("Who to look at?")] public List<GameObject> followList;

    [Header("Zoom preferences")] public float zoomMax = 90;
    private float zoomDefault = 0;
    private float defaultZ;
    public float orthographicZoomScale = 0.5f;
    public float orthographicZoomSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindObjectOfType<GameStateBehaviourScript>();
        cmBrain = FindObjectOfType<CinemachineBrain>();
        defaultZ = transform.position.z;

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
        if (virtualCamera != null) zoomDefault = virtualCamera.m_Lens.OrthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (followList.Count == 0)
        {
            ResetFollowToPlayer();
        }

        secretFollowObj.transform.position = _gameState.player.transform.position;

        // Moving & Zooming Camera
        if (virtualCamera != null)
        {
            // Move
            secretFollowObj.transform.position = GetMiddlePos();

            // Zoom
            Bounds bounds = GetFollowBounds();
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = bounds.size.x / bounds.size.y;
            float dim = Mathf.Max(bounds.size.x, bounds.size.y);
            dim = dim * orthographicZoomScale;
            float zoom = Mathf.Clamp(dim, zoomDefault, zoomMax);
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, zoom,
                Time.deltaTime * orthographicZoomSpeed);
        }
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