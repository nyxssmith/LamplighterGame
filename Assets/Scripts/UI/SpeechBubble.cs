using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{

    public TextMesh text;

    // how long object stays around
    private float LifeTimeLength = 5.0f;
    private float TTL;


    private float StartDayTime = 10.0f;
    private float EndDayTime = 165.0f;

    private DaylightCycle TimeCycle;

    void Start()
    {
        TTL = LifeTimeLength;
        var DaylightCycleList = FindObjectsOfType<DaylightCycle>();
        TimeCycle = DaylightCycleList[0];

    }

    void Update()
    {
        // look at camera
        transform.LookAt(Camera.main.transform);

        // do time down
        if (TTL <= 0.0f)
        {
            Destroy(this.gameObject);
        }
        else
        {
            TTL -= Time.deltaTime;
        }

        // if day or night change font color
        float currentTime = TimeCycle.GetTime();
        if(currentTime > StartDayTime && currentTime < EndDayTime ){
            text.color = Color.black;
        }else{
            text.color = Color.white;
        }


    }

    public void SetText(string newText)
    {
        text.text = newText;
    }

    public void moveUp(){
        transform.position = transform.position+new Vector3(0.0f,0.2f,0.0f);
    }



}
