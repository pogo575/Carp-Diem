using UnityEngine;
using System.Collections;

public class AgentMouth : MonoBehaviour {
	public AgentControl owner;
	public AgentControl targetAgent;
	public ParticleSystem attackParticle;
	// Use this for initialization
	void Start () {
		thisTransform = transform;
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other) {


		targetAgent = other.GetComponent<AgentControl>();
	
		if(owner && targetAgent){
			if(targetAgent != owner){
			if(targetAgent.GetBit(owner.stat.attack))
				if(owner.playerControlled)
					owner.Score();

			if(attackParticle)
				attackParticle.Play();
				if( biteAudioCue != null && thisTransform != null )
				SECTR_AudioSystem.Play(biteAudioCue, thisTransform.position,false);
			}
		}
	

		targetAgent = null;
	}

	void OnTriggerStay (Collider other){
		if(other.transform != owner.thisTransform)
		owner.ReTestWander();
	}

	private Transform thisTransform;
	public SECTR_AudioCue biteAudioCue;

}
