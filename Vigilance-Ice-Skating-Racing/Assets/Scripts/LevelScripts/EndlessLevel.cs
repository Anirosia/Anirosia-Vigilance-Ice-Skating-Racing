using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessLevel : MonoBehaviour
{
    [Header("Assignables")]
    public Transform nextLevelTransform;
    public GameObject coinSpawnPointParent;
    public GameObject varianceParent;
    //--------------------Private--------------------
    private GameObject[] coinSpawnPoints;
    private GameObject[] variance;

    private void Start()
    {
        if (coinSpawnPointParent.transform.childCount != 0)
        {
            coinSpawnPoints = new GameObject[coinSpawnPointParent.transform.childCount];

            for (int i = 0; i < coinSpawnPointParent.transform.childCount; i++)
            {
                coinSpawnPoints[i] = coinSpawnPointParent.transform.GetChild(i).gameObject;
            }
        }
        if (varianceParent.transform.childCount != 0)
        {
            variance = new GameObject[varianceParent.transform.childCount];

            for (int i = 0; i < varianceParent.transform.childCount; i++)
            {
                variance[i] = varianceParent.transform.GetChild(i).gameObject;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nextLevelTransform.position, 0.1f);
    }

    public void SetupLevel(int difficulty)
    {
        SpawnCoins();

        ApplyVariance(difficulty);
    }

    public void ApplyVariance(int difficulty)
    {
        if (variance.Length == 0) return;

        var chance = Random.value;

        if (chance > 0.2f * difficulty) return;

        int randomIndex = Random.Range(0, variance.Length);

        variance[randomIndex].SetActive(true);
    }
    public void SpawnCoins() 
    {
        if (coinSpawnPoints.Length == 0) return;

        var chance = Random.value;

        if (chance > 0.2f) return;

        int randomIndex = Random.Range(0, coinSpawnPoints.Length);

        coinSpawnPoints[randomIndex].SetActive(true);
    }
}
