using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager:SingleMonoBehaviour<MainManager> {

	public Button btn_server;
	public Button btn_client;

	protected override void Awake ()
	{
		base.Awake ();
		btn_server.onClick.AddListener (()=>{
			UnityEngine.SceneManagement.SceneManager.LoadScene("Server");
		});
		btn_client.onClick.AddListener (()=>{
			UnityEngine.SceneManagement.SceneManager.LoadScene("Client");
		});
	}

}
