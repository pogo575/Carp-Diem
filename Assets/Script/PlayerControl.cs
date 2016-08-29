using UnityEngine;
using System.Collections;
using Flux;
using UnityEngine.UI;
using Thinksquirrel.CShake;
public class PlayerControl : MonoBehaviour {
	public int playerScore;

	public string scoreText;
	public Text  scoreTextUI;

	public string healthText;
	public Text  healthTextUI;

	public bool controlActive;
	public AgentControl playerAgent;

	public float cameraHeight = 15f;
	public float cameraDamp = 3f;
	public float inputSmooth = 2f;
	public float inputLerp = 1.2f;
	public Transform cameraTarget;


	private Transform cameraTransform;
	private Vector3 cameraTargetPos;
	private Vector3 cameraGoal;
	private Vector3 cameraVelocity;

	private Vector3 mGoal;
	private Vector3 inputGoal;
	private Vector3 startPosition;
	public float v;
	public float h;

	private string vertical = "Vertical";
	private string horizontal =  "Horizontal";


	private Vector3 inputVelocity;
	public CameraShake bigShake; 
	public CameraShake smallShake; 
	void Start () {
		cameraTransform = Camera.main.transform;
		controlActive = false;
		if(playerAgent)
			startPosition = playerAgent.transform.position;
	}


	public FSequence startSeq;
	public void GameStart(){
		startSeq.Play();
		controlActive = true;
		Reset();
	}

	public FSequence gameOverSeq;

	public void GameOver(){
		gameOverSeq.Play();
		controlActive = false;
	}

	public FSequence replay;

	public void Replay(){
		replay.Play();
		controlActive = true;
		Reset();
	}

	public FSequence gameOverToMenuSeq;
	public void GOToMenu(){
		Application.LoadLevel(1);
		controlActive = false;

	}


	public SECTR_AudioCue buttonAudioCue;
	public void PlayButtonAudio(){
		SECTR_AudioSystem.Play(buttonAudioCue, transform.position, false);
	}

	public SECTR_AudioCue scoreAudioCue;


	public void HitEffect(){
		smallShake.Shake();

	}

	public void ScoreUp(){
		SECTR_AudioSystem.Play(scoreAudioCue, transform.position, false);
		playerScore += 100;
		bigShake.Shake();
		UpdateScoreText();
	}

	public void Reset(){
		playerScore = 0;
		UpdateScoreText();
		playerAgent.thisTransform.position = startPosition;
		playerAgent.stat.health = 5;
	}

	void UpdateScoreText(){
		scoreTextUI.text = (""+playerScore);
	}


	void Update () {
		
		mGoal.x = Mathf.SmoothStep(mGoal.x, Input.GetAxis(horizontal),inputSmooth * Time.deltaTime) ;
		mGoal.z = Mathf.SmoothStep(mGoal.z,Input.GetAxis(vertical),inputSmooth*Time.deltaTime );

		inputGoal = Vector3.Slerp(inputGoal, mGoal, inputLerp * Time.deltaTime);

		if(controlActive){
			playerAgent.moveDirection = inputGoal;

			CameraUpdate();


			if(playerAgent.stat.health<0f)
				GameOver();
		}

		healthText = ("" + playerAgent.stat.health);
		healthTextUI.text = healthText;

	}



	void CameraUpdate(){

		cameraTargetPos.x =cameraTarget.position.x;
		cameraTargetPos.y = cameraHeight;
		cameraTargetPos.z = cameraTarget.position.z;

		cameraGoal = Vector3.SmoothDamp(cameraGoal,cameraTargetPos, ref cameraVelocity, cameraDamp*Time.deltaTime );
		cameraTransform.position = cameraGoal;

	}


}

