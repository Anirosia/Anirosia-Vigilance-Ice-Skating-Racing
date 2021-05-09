using LevelScripts;
using UnityEngine;

public class TestConnection : MonoBehaviour
{
    public GameObject a1;
    public GameObject a2;
    // Start is called before the first frame update

    private void Update(){
        var p0 = a1.GetComponent<TerrainGeneration>().EndPoint;
        var p1 = a2.GetComponent<TerrainGeneration>().StartPoint;
        p1.position = p0.position;
    }
}
