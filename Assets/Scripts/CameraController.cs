using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public Transform cameraTarget;
    
    public Tilemap areaMap;
    // limits for the camera size due to the map size in the two variables below
    private Vector3 bottomLeftLimit;
    private Vector3 upperRightLimit;

    private float halfHeight;
    private float halfWidth;

    public int musicToPlay;
    private bool musicStarted;

    void Start()
    {
        //cameraTarget = PlayerController.instance.transform;
        cameraTarget = FindObjectOfType<PlayerController>().transform;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        bottomLeftLimit = areaMap.localBounds.min + new Vector3(halfWidth,halfHeight,0);
        upperRightLimit = areaMap.localBounds.max + new Vector3(-halfWidth,-halfHeight,0);
    
        // Set the map bounds to be used to set player bounds
        PlayerController.instance.SetBounds(areaMap.localBounds.min, areaMap.localBounds.max);
    }

    void LateUpdate()
    {
        transform.position = new Vector3(cameraTarget.position.x, cameraTarget.position.y, transform.position.z);        

        // Keeping the camera in the bounds of the game map
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, upperRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, upperRightLimit.y), transform.position.z);
    
        if (!musicStarted) {
            musicStarted = true;
            AudioManager.instance.PlayBGM(musicToPlay);
        }
    }
}
