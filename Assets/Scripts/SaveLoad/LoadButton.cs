using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour {

    SaveManager Loader;
	void Start () {
		Button btn = this.gameObject.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
        Loader = new SaveManager();
	}

	void TaskOnClick(){
        Loader.Load();



	}
}
