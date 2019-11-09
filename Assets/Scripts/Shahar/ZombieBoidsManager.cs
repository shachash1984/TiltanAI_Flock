using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBoidsManager : MonoBehaviour
{

    public static ZombieBoidsManager S;

    public GameObject _zombiePrefab;
    public Transform target;
    public int numZombies = 100;
    public float maxVelocity = 30f;
    public float minVelocity = 0f;
    public float spawnRadius = 20f;
    public float spawnVelocity = 2f;
    public float nearDistance = 5f;
    public float collisionDistance = 2f;
    public float velocityMatchingAmt = 0.01f;
    public float flockCenteringAmt = 0.15f;
    public float cllisionAvoidanceAmt = -0.5f;
    public float targetAttractionAmount = 0.01f;
    public float targetAvoidanceDistance = 15f;
    public float targetAvoidanceAmt = 0.75f;
    public float velocityLerpAmt = 0.25f;


    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        for (int i = 0; i < numZombies; i++)
        {
            Instantiate(_zombiePrefab);
        }
    }
}
