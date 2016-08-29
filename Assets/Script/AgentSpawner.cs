using UnityEngine;

using System.Collections;
using PathologicalGames;

using NodeCanvas;
using FlowCanvas;


public class AgentSpawner : MonoBehaviour {
	public float spawnTickRate = 4.0f;
	public int fishCount;

	public int maxSpawnAgent = 4;

	public SpawnPool fishPool;

	public Transform fishBase;


	public GameObject[] spawnNode;
	public int spawnNodeCount;

	public string spawnNodeTag = "SpawnNode";
	private string spawnTick = "SpawnTick";

	private int iC;

	void Start () {
		InvokeRepeating(spawnTick, 0f, spawnTickRate);

		spawnNode = GameObject.FindGameObjectsWithTag(spawnNodeTag);
		spawnNodeCount = spawnNode.Length;
	}


	public void SpawnTick(){
		
		if(fishCount<maxSpawnAgent){
			SpawnAgent(GetSpawnPoint());
		}

		else {
			return;
		}

	}

	public Transform GetSpawnPoint(){
		return spawnNode[Random.Range(0,spawnNodeCount)].transform;
	}

	public AgentControl SpawnAgent(Transform spawnPoint){
		if(fishCount<maxSpawnAgent){
			fishCount++;

			return fishPool.Spawn
				(fishBase, spawnPoint.position, spawnPoint.rotation)
					.GetComponent<AgentControl>();
		}

		return null;
	}


	public void DeSpawn (Transform tran) {
		fishPool.Despawn(tran);
		fishCount--;	
	}
		
}
