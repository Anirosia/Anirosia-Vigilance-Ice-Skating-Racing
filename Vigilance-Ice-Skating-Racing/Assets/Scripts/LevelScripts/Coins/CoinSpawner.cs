using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    private void OnEnable()
    {
        EndlessLevelManager.OnLevelAdded += SpawnCoins;
    }

    private void SpawnCoins(GameObject level)
    {
        
    }
}
