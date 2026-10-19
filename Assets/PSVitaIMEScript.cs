using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSVitaIMEScript : MonoBehaviour {

	static PSVitaIMEScript instance;

	void Awake () {
		// Keep only one copy alive if the scene with this object is loaded again.
		if (instance != null && instance != this) {
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		if (Application.platform == RuntimePlatform.PSP2) Sony.Vita.Dialog.Main.Initialise();
	}

	// Update is called once per frame
	void Update () {
		if (Application.platform == RuntimePlatform.PSP2) Sony.Vita.Dialog.Main.Update();
	}
}
