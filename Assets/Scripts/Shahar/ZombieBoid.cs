using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBoid : MonoBehaviour
{
    public static List<ZombieBoid> zombies;
    public static Transform target;

    private Vector3 _velocity;
    private Vector3 _newVelocity;
    private Vector3 _newPosition;

    private List<ZombieBoid> _neighbors;
    private List<ZombieBoid> _collisionRisks;
    private ZombieBoid _closest;

    private void Awake()
    {
        if(zombies == null)
        {
            zombies = new List<ZombieBoid>();
        }
        zombies.Add(this);
        if (target == null)
            target = ZombieBoidsManager.S.target;

        Vector3 randomPos = Random.insideUnitSphere * ZombieBoidsManager.S.spawnRadius;
        randomPos.y = 1;
        transform.position = randomPos;
        _velocity = Random.onUnitSphere;
        _velocity *= ZombieBoidsManager.S.spawnVelocity;
        _velocity.y = 1;

        _neighbors = new List<ZombieBoid>();
        _collisionRisks = new List<ZombieBoid>();

        transform.SetParent(ZombieBoidsManager.S.transform);

        Color randColor = Color.black;
        while (randColor.r + randColor.g + randColor.b < 1.0)
        {
            randColor = new Color(Random.value, Random.value, Random.value);
        }
        GetComponent<Renderer>().material.color = randColor;

    }

    private void Update()
    {
        List<ZombieBoid> tempNeighbors = GetNeighbors(this);
        _newVelocity = _velocity;
        _newPosition = transform.position;

        Vector3 neighborVel = GetAverageVelocity(_neighbors);
        _newVelocity += neighborVel * ZombieBoidsManager.S.velocityMatchingAmt;

        Vector3 neighborCenterOffset = GetAveragePosition(_neighbors) - transform.position;
        _newPosition += neighborCenterOffset * ZombieBoidsManager.S.flockCenteringAmt;

        Vector3 dist;
        if(_collisionRisks.Count > 0)
        {
            Vector3 collisionAveragePos = GetAveragePosition(_collisionRisks);
            dist = collisionAveragePos - transform.position;
            _newVelocity += dist * ZombieBoidsManager.S.cllisionAvoidanceAmt;
        }

        dist = target.position - transform.position;
        if(dist.magnitude > ZombieBoidsManager.S.targetAvoidanceDistance)
        {
            _newVelocity += dist * ZombieBoidsManager.S.targetAttractionAmount;
        }
        else
        {
            _newVelocity = Vector3.zero;
            //_newVelocity -= dist * ZombieBoidsManager.S.targetAvoidanceDistance * ZombieBoidsManager.S.targetAvoidanceAmt;
        }
    }

    private void LateUpdate()
    {
        _velocity = (1 - ZombieBoidsManager.S.velocityLerpAmt) * _velocity + ZombieBoidsManager.S.velocityLerpAmt * _newVelocity;
        if(_velocity.magnitude > ZombieBoidsManager.S.maxVelocity)
        {
            _velocity = _velocity.normalized * ZombieBoidsManager.S.maxVelocity;
        }
        if (_velocity.magnitude < ZombieBoidsManager.S.minVelocity)
        {
            _velocity = _velocity.normalized * ZombieBoidsManager.S.minVelocity;
        }
        _newPosition = transform.position + _velocity * Time.deltaTime;
        _newPosition.y = 1;

        transform.LookAt(_newPosition);
        transform.position = _newPosition;
    }

    private List<ZombieBoid> GetNeighbors(ZombieBoid zomboid)
    {
        float closestDistance = float.MaxValue;
        Vector3 delta;
        float dist;
        _neighbors.Clear();
        _collisionRisks.Clear();

        foreach (ZombieBoid zb in zombies)
        {
            if (zb == zomboid)
                continue;
            delta = zb.transform.position - transform.position;
            dist = delta.magnitude;
            if(dist < closestDistance)
            {
                closestDistance = dist;
                _closest = zb;
            }
            if(dist < ZombieBoidsManager.S.nearDistance)
            {
                _neighbors.Add(zb);
            }
            if(dist < ZombieBoidsManager.S.collisionDistance)
            {
                _collisionRisks.Add(zb);
            }
        }
        if(_neighbors.Count == 0)
        {
            _neighbors.Add(_closest);
        }
        return _neighbors;
    }

    public Vector3 GetAveragePosition(List<ZombieBoid> zomboids)
    {
        Vector3 sum = Vector3.zero;
        if (zomboids.Count == 0)
            return sum;
        foreach (ZombieBoid zb in zomboids)
        {
            sum += zb.transform.position;
        }
        Vector3 center = sum / zomboids.Count;
        return center;
    }

    public Vector3 GetAverageVelocity(List<ZombieBoid> zomboids)
    {
        Vector3 sum = Vector3.zero;
        if (zomboids.Count == 0)
            return sum;
        foreach (ZombieBoid zb in zomboids)
        {
            sum += zb._velocity;
        }
        Vector3 center = sum / zomboids.Count;
        return center;
    }
}
