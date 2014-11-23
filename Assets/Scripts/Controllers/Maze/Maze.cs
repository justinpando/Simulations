using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Maze : MonoBehaviour {

	public IntVector2 size;
	
	public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
		}
	}	
	
	public MazeRoomSettings[] roomSettings;
	private List<MazeRoom> rooms = new List<MazeRoom>();
	
	public MazeCell cellPrefab;
	private MazeCell[,] cells;
	
	public MazePassage passagePrefab;
	public MazeWall[] wallPrefabs;
	public MazeDoor doorPrefab;
	
	[Range(0f, 1f)]
	public float doorProbability;

	public float generationStepDelay;

	public bool completed = false;
	
	public AudioSource audioSource;
	public AudioClip placeCell;
	public AudioClip placeWall;
	public AudioClip finished;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private MazeRoom CreateRoom (int indexToExclude) {
		MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
		newRoom.settingsIndex = Random.Range(0, roomSettings.Length);
		if (newRoom.settingsIndex == indexToExclude) {
			newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSettings.Length;
		}
		newRoom.settings = roomSettings[newRoom.settingsIndex];
		rooms.Add(newRoom);
		return newRoom;
	}	
	
	public bool ContainsCoordinates (IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
	}	
	
	//Returns the MazeCell at the given coordinates
	public MazeCell GetCell(IntVector2 coordinates) {
		return cells[coordinates.x, coordinates.z];
	}
	
	public IEnumerator Generate () {
		completed = false;
		WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
		cells = new MazeCell[size.x, size.z];
		List<MazeCell> activeCells = new List<MazeCell>();
		//Create the first cell
		DoFirstGenerationStep(activeCells);
		while (activeCells.Count > 0) {
			yield return delay;
			DoNextGenerationStep(activeCells);
		}
		completed = true;
		audioSource.PlayOneShot(finished);
		
		for (int i = 0; i < rooms.Count; i++) {
			rooms[i].Hide();
		}
	}
	
	private void DoFirstGenerationStep (List<MazeCell> activeCells) {
		//Place a cell in a random spot		
		MazeCell newCell = CreateCell(RandomCoordinates);
		newCell.Initialize(CreateRoom(-1));
		activeCells.Add(newCell);		
	}
	
	private void DoNextGenerationStep (List<MazeCell> activeCells) {
		//Get current cell
		//int currentIndex = activeCells.Count - 1;
		int currentIndex = Random.Range(0, activeCells.Count - 1);
		MazeCell currentCell = activeCells[currentIndex];
		if (currentCell.IsFullyInitialized) {
			activeCells.RemoveAt(currentIndex);
			return;		
		}
		//Get a random direction
		MazeDirection direction = currentCell.RandomUninitializedDirection;
		IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
		//If the new coordinates are contained by the maze,
		if (ContainsCoordinates(coordinates)) {
			//Check if there is a cell there
			MazeCell neighbor = GetCell(coordinates);
			//If not,
			if(neighbor == null) {
				//Create a new cell, and a passage
				neighbor = CreateCell(coordinates);
				CreatePassage(currentCell, neighbor, direction);
				activeCells.Add(neighbor);
			} 
			else if (currentCell.room.settingsIndex == neighbor.room.settingsIndex) {
				CreatePassageInSameRoom(currentCell, neighbor, direction);
			}			
			//if there is, create a wall
			else {
				CreateWall(currentCell, neighbor, direction);		
			}
		}
		//Else go backward in the list
		else {
			CreateWall(currentCell, null, direction);
		}
	}			
	
	private MazeCell CreateCell (IntVector2 coordinates) {
		MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
		cells[coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = transform;
		newCell.transform.localPosition = 
			new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);

		audioSource.PlayOneShot(placeCell);
		return newCell;
	}	
	//Creates passages between two cells
	private void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage prefab = Random.value < doorProbability ? doorPrefab : passagePrefab;
		MazePassage passage = Instantiate(prefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(prefab) as MazePassage;
		
		if (passage is MazeDoor) {
			otherCell.Initialize(CreateRoom(cell.room.settingsIndex));
		}
		else {
			otherCell.Initialize(cell.room);
		}		
		
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}
	
	private void CreatePassageInSameRoom (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());
		if (cell.room != otherCell.room) {
			MazeRoom roomToAssimilate = otherCell.room;
			cell.room.Assimilate(roomToAssimilate);
			rooms.Remove(roomToAssimilate);
			Destroy(roomToAssimilate);
		}
	}	
	
	// Creates walls between two cells
	private void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazeWall wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
		wall.Initialize(cell, otherCell, direction);
		audioSource.PlayOneShot(placeWall);
		if (otherCell != null) {
			wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
			wall.Initialize(otherCell, cell, direction.GetOpposite());
		}
	}	
}
