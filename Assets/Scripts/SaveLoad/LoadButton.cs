using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour {

    SaveManager Loader;
	void Start () {
		Button btn = this.gameObject.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
        Loader = gameObject.GetComponentInParent<SaveManager>();
	}

	void TaskOnClick(){
        Loader.Load();



	}
}
