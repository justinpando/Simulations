using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public MazePlayer playerPrefab;
	private MazePlayer playerInstance;

	public Maze mazePrefab;
	private Maze mazeInstance;

	public AudioSource audioSource;
	public AudioClip finished;
	public AudioClip restart;
	
	// Use this for initialization
	void Start () {
		StartCoroutine(BeginGame());
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Space)) {
			audioSource.PlayOneShot(restart);
			RestartGame();
		}
	}

	private IEnumerator BeginGame () {
		Camera.main.clearFlags = CameraClearFlags.Skybox;
		Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
		mazeInstance = Instantiate (mazePrefab) as Maze;
		yield return StartCoroutine(mazeInstance.Generate());
		playerInstance = Instantiate(playerPrefab) as MazePlayer;
		playerInstance.SetLocation(mazeInstance.GetCell(mazeInstance.RandomCoordinates));
		Camera.main.rect = new Rect(0f, 0f, .5f, .5f);
		Camera.main.clearFlags = CameraClearFlags.Depth;
	}
	
	void RestartGame () {
		StopAllCoroutines();
		Destroy (mazeInstance.gameObject);
		if (playerInstance != null) {
			Destroy(playerInstance.gameObject);
		}
		StartCoroutine(BeginGame());
	}
	
	void MazeFinished() {
		audioSource.Play();
	}
	
}