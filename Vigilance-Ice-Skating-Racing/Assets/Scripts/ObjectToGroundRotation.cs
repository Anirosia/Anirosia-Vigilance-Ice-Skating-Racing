using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToGroundRotation : MonoBehaviour
{
    public LayerMask layerMask;
    private Vector3 _origin;
    private Vector2 _dir = Vector2.down;
    private float _dist = 1.5f;
    private RaycastHit2D _hit = new RaycastHit2D();
    private void OnEnable(){
        StartCoroutine(CutOff());
    }

    private IEnumerator CutOff(){
        StartCoroutine(SetObjectRotation());
        yield return new WaitForSeconds(0.25f);
        StopAllCoroutines();
        yield return null;
    }
    private IEnumerator SetObjectRotation(){
        while (true){
            _origin = transform.position + new Vector3(.25f, .25f);
            _hit = (Physics2D.Raycast(_origin, _dir, _dist, layerMask));
            var groundNormal = _hit.normal;
            Quaternion groundRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            transform.rotation = groundRotation;
            Debug.DrawRay(_origin, _dir * _dist);
            yield return null;
        }
    }
}
