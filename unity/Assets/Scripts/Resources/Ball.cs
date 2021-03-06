﻿using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
	public bool canBeHit;
	public bool canBounce = true;

	float bounceDelay = .5f;
	float bounceTime = 0;

	float sleepTime = 0;
	float neededSleepTime = .33f;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!this.canBounce)
		{
			this.bounceTime += Time.deltaTime;

			if(this.bounceTime >= this.bounceDelay)
			{
				this.bounceTime = 0;
				this.canBounce = true;
			}
		}

		if(this.rigidbody2D.velocity.magnitude < .05)
		{
			if(sleepTime == 0)
			{
				sleepTime = Time.time;
			}

			if(Time.time - sleepTime >= neededSleepTime)
			{
				this.rigidbody2D.velocity *= 0;
				this.rigidbody2D.Sleep();

				sleepTime = 0;
				canBeHit = true;
			}
		}
		else
		{
			sleepTime = 0;
			canBeHit = false;
		}

		if(!canBeHit)
		{
			GameObject cam = GameObject.Find("Main Camera");
			Vector2 path = new Vector2(this.transform.position.x - cam.transform.position.x, this.transform.position.y - cam.transform.position.y);
			float speedMod = this.gameObject.rigidbody2D.velocity.magnitude * .15f;
			path *= Time.smoothDeltaTime * ((speedMod > 1) ? speedMod : 1);
			Vector3 camPos = cam.transform.position;
			camPos.x += path.x;
			camPos.y += path.y;

			if(camPos.x < 8.18)
			{
				camPos.x = 8.18f;
			}
			if(camPos.x > 21.269)
			{
				camPos.x = 21.269f;
			}
			
			if(camPos.y < 5.66)
			{
				camPos.y = 5.66f;
			}
			if(camPos.y > 12.63)
			{
				camPos.y = 12.63f;
			}

			cam.transform.position = camPos;
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		Enemy en = coll.gameObject.GetComponent<Enemy>();

		if(en)
		{
			en.Damage(1);
		}

		audio.PlayOneShot(audio.clip);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//Debug.Log("trigger enter");
		if(other.tag == "Ground")
		{
			this.rigidbody2D.drag = 1;
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(this.rigidbody2D.velocity.magnitude < .25f && other.tag == "Ground")
		{
			this.rigidbody2D.velocity *= .75f;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Ground")
		{
			this.rigidbody2D.drag = .1f;
		}
	}
}
