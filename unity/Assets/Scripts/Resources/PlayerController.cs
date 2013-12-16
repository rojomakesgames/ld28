﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float maxSpeed;
	public float strikingPower;
	public float swingDuration;
	public float hp = 1;

	int state = 0;
	
	float hitForce = 0;
	float timeHitStarted = 0;

	float hitAxis = 1;

	bool clubSwitchDown = false;

	float baseCameraOrtho = 5;

	AudioClip swingAudio;

	Animator playerAnimator;

	bool hasTeleport = true;

	// Use this for initialization
	void Start () 
	{
		swingAudio = Resources.Load<AudioClip>("swing");

		playerAnimator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(hp <= 0 && state < 100)
		{
			this.Die ();
		}

		//movement
		float axis = Input.GetAxis("Horizontal");
		if(axis < 0 && hitAxis > 0)
		{
			this.Flip();
		}
		else if(axis > 0 && hitAxis < 0)
		{
			this.Flip();
		}

		//hit ball
		axis = Input.GetAxis("Fire1");
		GameObject ball = GameObject.Find("mainball");
		Ball ballComp = ball.GetComponent<Ball>();
		if(axis != 0 && ballComp.canBeHit)
		{
			hitForce += Time.smoothDeltaTime * strikingPower;

			if(timeHitStarted == 0)
			{
				timeHitStarted = Time.time;
				state = 1;
				playerAnimator.SetInteger("state", 1);
			}
			else if(Time.time - timeHitStarted >= swingDuration)
			{
				state = 2;
				playerAnimator.SetInteger("state", 2);
			}
		}
		else if(hitForce > 0)
		{
			state = 2;
			playerAnimator.SetInteger("state", 2);
		}

		//clubs
		ClubBag bag = GameObject.Find("clubbag").GetComponent<ClubBag>();
		axis = Input.GetAxis("Vertical");
		if(axis != 0 && !clubSwitchDown)
		{
			clubSwitchDown = true;


			if(axis > 0)
			{
				bag.PreviousClub();
			}
			else
			{
				bag.NextClub();
			}
		}
		else if(axis == 0)
		{
			clubSwitchDown = false;
		}

		//reload level / confirm
		axis = Input.GetAxis("Confirm");
		if(axis != 0)
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		//camera
		GameObject cam = GameObject.Find("Main Camera");
		axis = Input.GetAxis("MouseDown");
		if(axis != 0 && ballComp.canBeHit)
		{
			float mouseDragX = Input.GetAxis("Mouse X");
			float mouseDragY = Input.GetAxis("Mouse Y");
			Vector3 pos = cam.transform.position;
			pos.x -= mouseDragX;
			pos.y -= mouseDragY;
			cam.transform.position = pos;
		}

		axis = Input.GetAxis("Mouse ScrollWheel");
		if(axis != 0 && ballComp.canBeHit)
		{
			cam.GetComponent<Camera>().orthographicSize -= axis;
		}

		//ui
		float ortho = cam.GetComponent<Camera>().orthographicSize;
		bag.gameObject.transform.position = cam.transform.position + new Vector3(ortho * 1.2f, ortho * -.9f, -cam.transform.position.z);
		float scale = ortho / baseCameraOrtho;
		bag.gameObject.transform.localScale = new Vector3(scale, scale, 1);

		//cheat move
		if(Input.GetMouseButtonDown(1))
		{
			Vector3 worldPos = cam.camera.ScreenToWorldPoint(Input.mousePosition);
			Vector3 pos = this.gameObject.transform.position;
			pos.x = worldPos.x;
			pos.y = worldPos.y;
			this.gameObject.transform.position = pos;

			ball.gameObject.transform.position = this.gameObject.transform.position;
			ball.gameObject.rigidbody2D.velocity = new Vector3(0, 0, 0);
		}
	}

	void hitBall()
	{
		GameObject ball = GameObject.Find("mainball");

		ClubBag bag = GameObject.Find("clubbag").GetComponent<ClubBag>();
		Vector2 dir = bag.CurrentClubVector();
		dir.Scale(new Vector2(hitForce * hitAxis, hitForce));

		ball.rigidbody2D.AddForce(dir);

		this.PlaySwing();
		ball.audio.PlayOneShot(ball.audio.clip);
	}

	void Flip()
	{
		hitAxis *= -1;
		Vector3 scale = this.transform.localScale;
		scale.x *= -1;
		this.transform.localScale = scale;
	}

	void setIdleState()
	{
		hitForce = 0;
		timeHitStarted = 0;
		state = 0;
		playerAnimator.SetInteger("state", 0);
	}

	void PlaySwing()
	{
		audio.PlayOneShot(swingAudio);
	}

	void Die()
	{
		this.gameObject.layer = 31;

		state = 100;
		playerAnimator.SetInteger("state", 100);
	}
	
	void Dying()
	{
		state = 101;
		playerAnimator.SetInteger("state", 101);
	}

	public void Damage(int dmg)
	{
		hp -= dmg;
	}

	public bool isDead()
	{
		return (state >= 100);
	}
}
