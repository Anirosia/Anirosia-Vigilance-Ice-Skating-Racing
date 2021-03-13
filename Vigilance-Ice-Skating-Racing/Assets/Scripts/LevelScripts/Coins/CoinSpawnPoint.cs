using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawnPoint : MonoBehaviour
{
    public int coinID = 0;
    public Color gizmoColour = Color.yellow;

    private bool m_bInitialized = false;
    private GameObject m_goCoin;

    private void OnEnable()
    {
        if (!m_bInitialized) { m_bInitialized = true; return; }


        m_goCoin = ObjectPool.Instance.GetObjectFromPool("Coin");
        m_goCoin.transform.position = transform.position;
        m_goCoin.SetActive(true);
    }
    private void OnDisable()
    {
        if (m_goCoin != null) ObjectPool.Instance.SetObjectInPool(m_goCoin);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
