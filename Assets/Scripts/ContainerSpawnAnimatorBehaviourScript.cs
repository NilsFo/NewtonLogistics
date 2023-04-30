using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerSpawnAnimatorBehaviourScript : MonoBehaviour
{
    [SerializeField] private Transform body;
    
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform animationPoint;

    [SerializeField] private float animationTime = 3f;
    
    [SerializeField] private float currentTime = 0f;
    [SerializeField] private bool inAnimation = false;
    
    public void Spawn()
    {
        currentTime = 0f;
        inAnimation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (inAnimation)
        {
            if (currentTime < animationTime)
            {
                body.position = Vector3.Lerp(animationPoint.position, spawnPoint.position,currentTime/animationTime);
                body.rotation = Quaternion.Lerp(animationPoint.rotation, spawnPoint.rotation,currentTime/animationTime);
                currentTime += Time.deltaTime;
            }
            else if(currentTime >= animationTime)
            {
                body.position = spawnPoint.position;
                body.rotation = spawnPoint.rotation;
                currentTime = animationTime;
                inAnimation = false;
            }
        }
        
    }
}
