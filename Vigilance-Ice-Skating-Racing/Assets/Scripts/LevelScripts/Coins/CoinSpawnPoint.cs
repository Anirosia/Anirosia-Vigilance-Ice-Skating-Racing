using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawnPoint : MonoBehaviour
{
    public int coinID = 0;
    public Color gizmoColour = Color.yellow;
    //private void OnEnable() => PlayerSpawnSystem.AddSpawnPoint(transform);

    //private void OnEnable() => PlayerSpawnSystem.RemoveSpawnPoint(transform);

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
