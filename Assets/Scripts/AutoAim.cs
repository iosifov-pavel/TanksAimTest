using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoAim : MonoBehaviour
{
    [SerializeField]
    private float _radius;
    [SerializeField]
    private LayerMask _aimMask;
    [SerializeField]
    private LayerMask _onbstacleMask;
    [SerializeField]
    private Vector2 _angleConstraints;
    [SerializeField]
    private Transform _cannonShootPoint;

    private Tank _target;
    private Dictionary<int, Tank> _cachedTanks;
    private RaycastHit[] _hits;

    private void Awake()
    {
        _hits = new RaycastHit[Constants.HitBufferSize];
        _cachedTanks = new Dictionary<int, Tank>();
    }

    private void Update()
    {
        CheckEnemies();
    }

    private void CheckEnemies()
    {
        var ray = new Ray(_cannonShootPoint.transform.position, _cannonShootPoint.transform.forward);
        var hits = Physics.SphereCastNonAlloc(ray, _radius, _hits, Constants.RayLength, _aimMask);
        if (hits == 0)
        {
            return;
        }
        var tanks = new List<Tank>();
        foreach (var hit in _hits)
        {
            if (hit.transform == null)
            {
                continue;
            }
            var goID = hit.transform.gameObject.GetInstanceID();
            if (!_cachedTanks.ContainsKey(goID))
            {
                var tankComponent = hit.transform.GetComponent<Tank>();
                _cachedTanks.Add(goID, tankComponent);
                tanks.Add(tankComponent);
            }
            else
            {
                tanks.Add(_cachedTanks[goID]);
            }
        }
        var sortedhits = CheckAngleConstraints(CheckObstacles(tanks));
        _target = GetTarget(sortedhits);
        if (_target != null)
        {
            Debug.DrawLine(_cannonShootPoint.position, _target.Target.position, Color.magenta);
        }
    }

    private List<Tank> CheckAngleConstraints(IEnumerable<Tank> tanks)
    {
        List<Tank> result = new List<Tank>();
        foreach (var tank in tanks)
        {
            var aimDirection = (tank.Target.position - _cannonShootPoint.position).normalized;
            var shootDirection = _cannonShootPoint.transform.forward;
            if (aimDirection.z <= 0)
            {
                continue;
            }
            var angleY = 90 - Vector3.Angle(aimDirection, _cannonShootPoint.transform.up);
            aimDirection.y = 0;
            var angleX = Vector3.Angle(shootDirection, aimDirection);
            if (angleX > _angleConstraints.x || angleY > _angleConstraints.y)
            {
                continue;
            }
            result.Add(tank);
        }
        return result;
    }
    private List<Tank> CheckObstacles(IEnumerable<Tank> tanks)
    {
        List<Tank> result = new List<Tank>();
        foreach (var tank in tanks)
        {
            var checkRay = tank.Target.position - _cannonShootPoint.position;
            var hitObstacle = Physics.Raycast(_cannonShootPoint.position, checkRay, checkRay.magnitude, _onbstacleMask);
            if (hitObstacle)
            {
                continue;
            }
            result.Add(tank);
        }
        return result;
    }

    private Tank GetTarget(List<Tank> tanks)
    {
        Tank target = null;
        if (tanks.Count == 0)
        {
            return target;
        }
        var maxPriority = tanks.Max(t => t.Priority);
        var tanksWithMaxPriority = tanks.Where(t => t.Priority == maxPriority);
        var minAngle = Constants.MinAngle;
        foreach (var tank in tanksWithMaxPriority)
        {
            var shootDeflection = Vector3.Angle(_cannonShootPoint.forward, tank.Target.position - _cannonShootPoint.position);
            if (shootDeflection <= minAngle)
            {
                minAngle = shootDeflection;
                target = tank;
            }
        }
        return target;
    }

}
