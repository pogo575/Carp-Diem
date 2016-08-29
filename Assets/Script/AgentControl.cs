using UnityEngine;

using System.Collections;

using PathologicalGames;
using RootMotion.FinalIK;

using NodeCanvas.BehaviourTrees;
using NodeCanvas.StateMachines;
using FlowCanvas;


public class AgentControl : MonoBehaviour {
	public ParticleSystem swimEffect;
	public float magToSwimEffect = .3f;

	public AgentControl parentAgent;
	public FishArt art;
	public bool playerControlled;


	public float wanderRate = 10.0f;
	public float wanderTime;

	public AgentStats stat;

	public Transform graphichRoot;
	public Transform tailTargetRoot;
	public Transform headTargetRoot;

	public float headDirMultiplier  = 0.35f;

	public float maxTailFlick = 45f;
	public float swimTime;
	public float swimFlickRate =1.5f;

	public Vector3 tailDirGoal;
	public Vector3 headDirGoal;

	public float dirMod = 3.0f;

	private Vector3 mGoal;


	public AnimationCurve swimCurve;

	public AgentControl targetAgent;

	private Vector3 _moveDirection;
	public Vector3 moveDirection{get{return _moveDirection;} set{_moveDirection = value;}}
	public Vector3 tM;
	private Vector3 moveGoal;
	private Vector3 mVel;
	public float moveSmooth = 2.0f;

	public float vMag;
	private Quaternion turnGoal; 

	private int iC;


	private PlayerControl playerControl;

	void Start () {
		Init();
	}

	public void Init(){
		
		thisAgent = GetComponent<AgentControl>();
		controller = GetComponent<CharacterController>();
		thisMouth = GetComponentInChildren<AgentMouth>();

		if(thisMouth)
			thisMouth.owner = thisAgent;

		if(thisTransform ==  null)
			thisTransform = transform;
		stat.InitStats();
		if(playerControl == null)
			playerControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerControl>();
	}


	public void OnSpawned(SpawnPool pool){
		Init();

		spawner = pool.GetComponent<AgentSpawner>();
		art.SelectRandomskin();
		tM.x = Random.Range(-1f,1f);
		tM.z = Random.Range(-1f,1f);

	}

	public void OnDespawned(SpawnPool pool){

		if(playerControlled){
			
			GameOver();
		}
	}



	public bool GetBit(float at){
		stat.health --;
		ReTestWander();
		if(playerControlled && playerControl)
			playerControl.HitEffect();

		if(stat.health<1f){
			
		
			GetKilled();
			return true;
		}
		return false;
	}

	public void GetKilled(){
		
		if(spawner)
			spawner.DeSpawn(thisTransform);
		
	}

	public void Score(){
		if(playerControl && playerControlled)
			playerControl.ScoreUp();
	}

	void Update(){
		dTime = Time.deltaTime;

		if(playerControl.controlActive){
			MoveAgent();
			stat.UpdateStats(dTime, vMag);
		}

	}

	private AgentControl tempAgent;


	private Vector3 posAdjust;

	public void ReTestWander(){
		if(playerControl.controlActive){
			tM.x = Random.Range(-1f,1f);
			tM.z = Random.Range(-1f,1f);
		}
	}



	public void MoveAgent(){
		
		if(playerControlled){
			mGoal =Vector3.RotateTowards(mGoal, moveDirection, 6f * dTime, 180f ).normalized;
			vMag = moveDirection.magnitude*stat.speed;
		}
		else {
			if(wanderRate < wanderTime){
				tM.x = Random.Range(-1f,1f);
				tM.z = Random.Range(-1f,1f);
				wanderTime = Random.Range(0f,wanderRate*0.5f);
			}
			else 
				wanderTime += dTime;

			mGoal =Vector3.RotateTowards(mGoal, tM, 6f * dTime, 180f ).normalized;
			vMag = tM.magnitude*stat.speed;
		}

		if(controller){
			moveGoal = Vector3.SmoothDamp(moveGoal, mGoal, ref mVel, moveSmooth*dTime);

			if(moveGoal != Vector3.zero){
				turnGoal =  Quaternion.LookRotation(moveGoal);
				thisTransform.rotation = turnGoal;
			}

			vMag =  Mathf.Clamp(vMag, 0f, stat.speed);
			if(controller.transform.gameObject.activeInHierarchy)
			controller.Move(thisTransform.forward * (vMag) );

			posAdjust = thisTransform.position;
			posAdjust.y = 0f;
			thisTransform.position = posAdjust;
		}

		yVelocity =thisTransform.eulerAngles.y - lastY;
		swimTime += dTime*swimFlickRate;

		if(swimTime > 1f)
			swimTime = 0f;

		AnimateAgent();
		lastY = thisTransform.eulerAngles.y;
		if(swimEffect){
		if(vMag > magToSwimEffect){
			if(!swimEffect.isPlaying)
				swimEffect.Play();	
		}
		else{
			if(swimEffect.isPlaying)
				swimEffect.Stop();	
		}
		}
	}

	public void AnimateAgent(){
		tailDirGoal.y = (swimCurve.Evaluate(swimTime) * maxTailFlick)*vMag+(yVelocity*(dirMod*-1));
		headDirGoal.y = (((swimCurve.Evaluate(swimTime) * maxTailFlick)*vMag)*-1)*headDirMultiplier+(yVelocity*dirMod);
		tailTargetRoot.localEulerAngles = tailDirGoal;
		headTargetRoot.localEulerAngles = headDirGoal;
	}

	void GameOver(){
		Debug.Log("no eggs... GAME OVER!");
	}


	private AgentControl thisAgent;
	private AgentMouth thisMouth;
	private CharacterController controller;
	private float dTime;
	private AgentSpawner spawner;

	[HideInInspector]public Transform thisTransform;

	private float lastY;
	public float yVelocity;

	public void OnTriggerEnter(Collider other){
		targetAgent = other.GetComponent<AgentControl>();
	}

}


[System.Serializable]
public class AgentStats{

	public float speed = 0.3f  ;
	public float maxSpeed = 0.5f ;
	public AnimationCurve speedCurve;


	public float health;
	public float maxHealth=5.0f;

	public float attack = 1.0f ;
	public float maxAttack = 5.0f;

	public void InitStats( ){
		health = 5f;
	}

	public void UpdateStats(float dTime, float speedMag){

		health = Mathf.Clamp(health, 0f, maxHealth);

		speed = Mathf.SmoothStep(speed, maxSpeed*speedMag, speedMag*dTime);//speedCurve.Evaluate(speedMag) * maxSpeed;
		speed = Mathf.Clamp(speed, 0, maxSpeed);
	}

}