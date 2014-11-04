using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {

	public Mesh[] meshes;
	public Material material;
	
	public int maxDepth;
	private int depth;	
	public float childScale;
	
	public float spawnProbability;
	
	public float maxTwist;
	public float maxRotationSpeed;
	private float rotationSpeed;	
	
	private Material[,] materials;
	
	private static Vector3[] childDirections = {
		Vector3.up,
		Vector3.down,
		Vector3.right,
		Vector3.left,
		Vector3.forward,
		Vector3.back		
	};
	
	private static Quaternion[] childOrientations = {
		//Make the child's up direction point away[ 
		Quaternion.identity,
		Quaternion.Euler(180f, 0f, 0f),
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f)		
	};
	
	// Use this for initialization
	void Start () {
		rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);	
		transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
		if(materials == null) InitializeMaterials();
		gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
		gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];	
		//gameObject.AddComponent<BillBoardBehavior>();
		if (depth < maxDepth) {
			StartCoroutine(CreateChildren());
		}
	}
	
	void Update() {
		transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
	}
	
	private void InitializeMaterials() {
		materials = new Material[maxDepth + 1, 2];
		for(int i = 0; i <= maxDepth; i++) {
			float t = i / (maxDepth - 1f);
			t *= t;		
			materials[i, 0] = new Material(material);
			materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
			materials[i, 1] = new Material(material);
			materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);			
		}
		materials[maxDepth, 0].color = Color.magenta;
		materials[maxDepth, 1].color = Color.red;
	}
	
	private void Initialize (Fractal parent, int childIndex) {
		meshes = parent.meshes;
		materials = parent.materials;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		
		childScale = parent.childScale;
		spawnProbability = parent.spawnProbability;
		
		maxTwist = parent.maxTwist;
		maxRotationSpeed = parent.maxRotationSpeed;
		
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		
		transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
		transform.localRotation = childOrientations[childIndex];
	}
	
	private IEnumerator CreateChildren () {
		for (int i = 0; i < childDirections.Length; i++) {
			if(i == 1 && depth > 0) continue;
			if(Random.value < spawnProbability) {
				yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
				new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, i);
			}
		}
	}
}
