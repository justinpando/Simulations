using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public Maze mazePrefab;
	
	private Maze mazeInstance;

	public AudioSource audioSource;
	public AudioClip finished;
	public AudioClip restart;
	
	// Use this for initialization
	void Start () {
		BeginGame();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Space)) {
			audioSource.PlayOneShot(restart);
			RestartGame();
		}
	}

	void BeginGame () {
		mazeInstance = Instantiate (mazePrefab) as Maze;
		StartCoroutine(mazeInstance.Generate());
	}
	
	void RestartGame () {
		StopAllCoroutines();
		Destroy (mazeInstance.gameObject);
		BeginGame();
	}
	
	void MazeFinished() {
		audioSource.Play();
	}
	
}