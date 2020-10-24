// Instantiates 10 copies of Prefab each 2 units apart from each other

using System.Collections;
using UnityEngine;

public class FlameLighter : MonoBehaviour
{

    private FlameController FlameControllerToLight;

    void Start()
    {
        FlameControllerToLight = this.gameObject.GetComponent<FlameController>();
        FlameControllerToLight.SetLitStatus(true);
        FlameControllerToLight.SetTimeLeft(-1);
    }

    public void Update()
    {
        if (!FlameControllerToLight.GetLitStatus())
        {
            FlameControllerToLight.SetLitStatus(true);
            FlameControllerToLight.SetTimeLeft(-1);
            Destroy(this);
        }
    }

}