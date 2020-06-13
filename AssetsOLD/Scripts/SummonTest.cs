// Instantiates 10 copies of Prefab each 2 units apart from each other

using UnityEngine;
using System.Collections;

public class SummonTest : MonoBehaviour
{
    public Transform prefab;
    void Start()
    {
    
    }

    public void SummonBunchOfCubes(){
        for (int i = 0; i < 10; i++)
        {
            Instantiate(prefab, new Vector3(i * 2.0F, 0, 0), Quaternion.identity);
        }
    }
}