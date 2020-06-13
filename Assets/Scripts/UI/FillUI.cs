using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FillUI : MonoBehaviour {

    public float percentage = 100f;

    public void SetTo(float percentageToSetTo){
        percentage = percentageToSetTo;
    }

    void Update()
    {
        GetComponent<Image>().fillAmount = percentage; 
    }
    
}
