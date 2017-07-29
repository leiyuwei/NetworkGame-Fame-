using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Ball : MonoBehaviour
{

	public Rigidbody2D rigidbody;
	public Vector2 m_preVelocity = Vector3.zero;
	public float speed = 10;
	public UnityAction onBottom;

	void Awake ()
	{
		rigidbody = GetComponent<Rigidbody2D> ();
	}

	public void OnCollisionEnter2D (Collision2D collision)
	{
		if (collision.gameObject.name.Trim () == "bottom") {
			rigidbody.velocity = Vector2.zero;
			Debug.Log ("Stop");
			if (onBottom != null) {
				onBottom ();
			}
			return;
		}
		if (collision.gameObject.name.Trim().IndexOf("plant")!=-1) {
			SoundManager.Instance.PlaySE (SoundManager.Instance.se);
		}
		ContactPoint2D contactPoint = collision.contacts [0];
		Vector2 newDir = Vector2.zero;
		Vector2 curDir = m_preVelocity;
		if (collision.gameObject.name.IndexOf ("stone") != -1) {
			Destroy (collision.gameObject);
			SoundManager.Instance.PlaySE (SoundManager.Instance.hit);
		} 
		newDir = Vector2.Reflect (curDir.normalized, contactPoint.normal);
//		Debug.Log (newDir + ":" + Mathf.Abs (Vector2.Dot (newDir.normalized, new Vector2 (1, 0))));
		if (Mathf.Abs (Vector2.Dot (newDir.normalized, new Vector2 (1, 0))) > 0.8f) {
			newDir = new Vector2 (newDir.x < 0 ? newDir.x + 0.3f : newDir.x - 0.3f, newDir.y);
		}
		rigidbody.velocity = newDir.normalized * speed;
		m_preVelocity = rigidbody.velocity;
	}

	public void Play ()
	{
		gameObject.SetActive (true);
		transform.position = new Vector3 (0, -3.8f, 0);
		rigidbody.velocity = new Vector2 (1, 1).normalized * speed;
		m_preVelocity = rigidbody.velocity;
	}

}
