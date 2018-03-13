using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkIndicator : MonoBehaviour{
    ParticleSystem particleSystem;
    ParticleSystem.VelocityOverLifetimeModule velOverLifeTime;
    Vector3 startPosition;

    float velOverTimeStart;

    float xDir, zDir;
    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        velOverTimeStart = particleSystem.velocityOverLifetime.x.constant;
        velOverLifeTime = GetComponent<ParticleSystem>().velocityOverLifetime;
    }

    private void Update()
    {

    }
    public void Init(GridCell from, GridCell to)
    {
        transform.position = GridManager.CellToWorldPos(from);

        ParticleSystem.MinMaxCurve curveX = new ParticleSystem.MinMaxCurve();
        ParticleSystem.MinMaxCurve curveZ = new ParticleSystem.MinMaxCurve();
        curveX.constant = (from.columnPos - to.columnPos) * -velOverTimeStart;
        curveZ = (from.rowPos - to.rowPos) * velOverTimeStart;
        velOverLifeTime.x = curveX;
        velOverLifeTime.z = curveZ;
    }
}
