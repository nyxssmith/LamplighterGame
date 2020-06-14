// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class SummonPrefab : MonoBehaviour {

    public GameObject PreFab;

    private Vector3 SummonPositon;

    void Start () {
    }

    public void Update () {
        SummonPositon = this.transform.position;
    }

    public void Summon () {
        Instantiate (PreFab, SummonPositon, Quaternion.identity);

    }
}