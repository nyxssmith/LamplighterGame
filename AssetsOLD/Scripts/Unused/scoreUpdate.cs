using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreUpdate : MonoBehaviour {

    public Transform player;
    public Text scoreTxt;

	
	// Update is called once per frame
	void Update () {

        scoreTxt.text = player.position.z.ToString("0");
	}
}
