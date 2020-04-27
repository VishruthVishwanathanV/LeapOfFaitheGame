using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public class Player : MonoBehaviour {

	public float speed;
	public float maxSpeedValue = 130.0f;
	public int stop;
	public int swipeSensitivity;
	public float touchSensitivity;
	public System.Boolean cameraTilt;
	public int cameraShake = 10;
	public int cameraShakeMax = 10;
	public Vector3 cameraOriginalPosition;
	public float currentCameraShakeDistance = 1.0f;
	public float maxCameraShakeDistance = 1.0f;
	public float signValue = -1;

	public GameObject preTerrain;
	public GameObject sucTerrain;
	//Logic to loop the terrain Map
	float terrainStep;
	float terrainLoop;
	public camScript cameraScript;

	private float verticalStartTouchPosition, verticalEndTouchPosition, horizontalStartTouchPosition, horizontalEndTouchPosition;
	private Vector3 horizontalMovement = new Vector3(6.0f, 0.0f, 0.0f);

	private Rigidbody rb;
	private Animator remy;

	Object[] trees = new Object[100];
	
	List<GameObject> treeList = new List<GameObject>();	
	public double treeWidth = 400.0;
	public double initPosTree = 0.0;
	private BoxCollider playerCollider;

	void Start() {

		cameraScript = GameObject.Find("Main Camera").GetComponent<camScript>();
		rb = GetComponent<Rigidbody>();
		remy = GetComponent<Animator>();
		stop = 0;
		swipeSensitivity = 20;
		touchSensitivity = Screen.width / 2;
		cameraTilt = false;

		rb.velocity = new Vector3(0, 0, speed);

		//Loogping terrain logic
		terrainStep = 500;
		terrainLoop = 1;

		//Logic to add trees for certain position		
		trees = Resources.LoadAll("Trees");
		//treeList = (GameObject) trees;
		foreach (Object tree in trees) {
			GameObject tempGameObject = (GameObject)tree;
			treeList.Add(tempGameObject);
		}

		
		playerCollider = GetComponent<Collider>() as BoxCollider;

		buildTree();

	}

	void Update() {

		//Logic for the terrain loop
		float z = gameObject.transform.position.z;

		if (z > ((terrainStep * terrainLoop) + 20)) {

			terrainLoop += 1;

			if ((preTerrain.transform.position.z + 500) < z) {

				preTerrain.transform.position = new Vector3(preTerrain.transform.position.x, preTerrain.transform.position.y, preTerrain.transform.position.z + (terrainStep * 2));

			}
			if ((sucTerrain.transform.position.z + 500) < z) {

				sucTerrain.transform.position = new Vector3(sucTerrain.transform.position.x, sucTerrain.transform.position.y, sucTerrain.transform.position.z + (terrainStep * 2));

			}
		}

		//Logic for the input touches and movements in the mobile
		if (Input.touchCount > 0)
		{
			playerControl();
		}else if ( Input.GetKeyDown(KeyCode.W) )
		{
			remy.SetTrigger("jump");
		}else if ( Input.GetKeyDown(KeyCode.S) )
		{
			remy.SetTrigger("slide");
		}else if (Input.GetKeyDown(KeyCode.D))
		{
			Vector3 cameraCurrentPosition = cameraScript.getPosition();
			if ( rb.transform.position.x < 5.0f ) {
				cameraScript.setPosition(new Vector3(cameraCurrentPosition.x + 5.0f, cameraCurrentPosition.y, cameraCurrentPosition.z));
				rb.position = new Vector3(rb.position.x + 5.0f, rb.position.y, rb.position.z);
			}
			else
			{
				cameraTilt = true;
				if (speed >= 70.0f)
				{
					speed -= 50.0f;
				}
			}
		}else if (Input.GetKeyDown(KeyCode.A))
		{
			if (rb.transform.position.x > -5.0f)
			{
				Vector3 cameraCurrentPosition = cameraScript.getPosition();
				cameraScript.setPosition(new Vector3(cameraCurrentPosition.x - 5.0f, cameraCurrentPosition.y, cameraCurrentPosition.z));
				rb.position = new Vector3(rb.position.x - 5.0f, rb.position.y, rb.position.z);
			}
			else
			{
				cameraTilt = true;
				if (speed >= 70.0f)
				{
					speed -= 50.0f;
				}
			}
		}

		if ( cameraTilt )
		{
			shakeCamera();
		}

		//Logic for the Looping of the trees sideways
		if ((int)rb.transform.position.z > (initPosTree - 100.0))
		{

			initPosTree = initPosTree + 500.0;

			buildTree();//Logic to loop build trees

		}

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("coin"))
		{
			Destroy(other.gameObject);
			ScoreScript.scoreValue += 1;

			if (speed < maxSpeedValue && (ScoreScript.scoreValue % 100f) == 0f)
			{
				speed += 10.0f;
			}
		}
		if (other.gameObject.CompareTag("obstacles"))
		{
			remy.SetTrigger("fallForward");
			rb.velocity = new Vector3(0, 0, 0);
			stop = 1;
		}
	}


	void FixedUpdate() {

		if ( stop == 1 ) {
			rb.velocity = new Vector3(0, 0, 0);
		}
		else { 
			rb.velocity = new Vector3(0, 0, speed);
		}
	}


	//Build tree loop for the building of trees
	void buildTree() {

		int distanceBetweenTree = 40;
		float treeDistanceFromCenter = 25f;
		double i;
		pointSystem((int)initPosTree);
		for (i = initPosTree; i < (treeWidth + initPosTree); i++) {

			
			i = i + distanceBetweenTree;

			int randTreeCountLeft = (int)Random.Range(0, treeList.Count - 1);
			GameObject selectedTreeLeft = treeList[randTreeCountLeft];
			Instantiate(selectedTreeLeft, new Vector3(treeDistanceFromCenter, 0, (float)i), Quaternion.identity);

			int randTreeCountRight = (int)Random.Range(0, treeList.Count);
			GameObject selectedTreeRight = treeList[randTreeCountRight];
			Instantiate(selectedTreeRight, new Vector3(-treeDistanceFromCenter, 0, (float)i), Quaternion.identity);

			selectedTreeLeft.gameObject.tag = "treeOut";
			selectedTreeRight.gameObject.tag = "treeOut";

		}

		//Logic to remove unwanted trees
		GameObject[] treesBehind = GameObject.FindGameObjectsWithTag("treeOut");

		if (treesBehind != null) {

			for (int j = 0; j < treesBehind.Length; j++) {

				GameObject thisGameObject = treesBehind[j];

				if (thisGameObject.transform.position.z < transform.position.z) {

					Destroy(treesBehind[j]);

				}
			}
		}

		//Instantiate a gameObject for creating
	}

	//Basic point system for the Initial
	void pointSystem(int origin) {
		
		if( origin < 300)
		{
			basicPS( origin );
		} else
		{	
			int choiceToSelectPS = (int)Random.Range(0, 2);
			Debug.Log("The Value of the Random Variable is :" + choiceToSelectPS);
			switch (choiceToSelectPS) { 
				case 0: advancedPS1(origin); break;
				case 1: advancedPS2(origin); break;
				default: advancedPS2(origin); break;
			}
		}

	}

		
	void basicPS(int origin)
	{
		Object[] pointSystems = new Object[100];
		pointSystems = Resources.LoadAll("Points");
		List<GameObject> localPointSystemList = new List<GameObject>();

		//pointSystemList = (GameObject) pointSystems;
		foreach (Object pointSystem in pointSystems)
		{
			GameObject tempGameObject = (GameObject)pointSystem;
			localPointSystemList.Add(tempGameObject);
		}
		int end = origin + 500;
		int calculatedEnd = origin + 50;

		Debug.Log("This is the advancedThe Value of origin is " + origin);

		for (int i = origin + 50; i < end; i = i+150)
		{

			int randPlacing = (int)Random.Range(0, 3);		
			int randPS = (int)Random.Range(0, localPointSystemList.Count);
			Debug.Log("The Valie of the randPS is " + randPS + "The Value of the PSIndex1 is " + randPlacing);
			switch (randPlacing)
			{
				case 0:
					Instantiate(localPointSystemList[randPS], new Vector3(-5, 0, (float)i), Quaternion.identity);
					break;
				case 1:
					Instantiate(localPointSystemList[randPS], new Vector3(0, 0, (float)i), Quaternion.identity);
					break;
				case 2:
					Instantiate(localPointSystemList[randPS], new Vector3(5, 0, (float)i), Quaternion.identity);
					break;
			}
		}
	}

	void advancedPS1(int origin)
	{
		Object[] pointSystems = new Object[100];
		pointSystems = Resources.LoadAll("Points");
		List<GameObject> localPointSystemList = new List<GameObject>();
		//pointSystemList = (GameObject) pointSystems;
		foreach (Object pointSystem in pointSystems)
		{
			GameObject tempGameObject = (GameObject)pointSystem;
			localPointSystemList.Add(tempGameObject);
		}
		int end = origin + 500;
		int calculatedEnd = origin + 50;

		Debug.Log("This is the advancedThe Value of origin is " + origin);

		for (int i = origin + 60; i < end; i = i + 150)
		{

			int randPlacing = (int)Random.Range(0, 3);
			int randPS = (int)Random.Range(0, localPointSystemList.Count);
			Debug.Log("The Valie of the randPS is " + randPS + "The Value of the PSIndex1 is " + randPlacing);
			switch (randPlacing)
			{
				case 0:
					Instantiate(localPointSystemList[randPS], new Vector3(-5, 0, (float)i), Quaternion.identity);
					break;
				case 1:
					Instantiate(localPointSystemList[randPS], new Vector3(0, 0, (float)i), Quaternion.identity);
					break;
				case 2:
					Instantiate(localPointSystemList[randPS], new Vector3(5, 0, (float)i), Quaternion.identity);
					break;
			}
		}
	}

	void advancedPS2(int origin)
	{
		Object[] pointSystems = new Object[100];
		pointSystems = Resources.LoadAll("Points-2");
		List<GameObject> localPointSystemList = new List<GameObject>();
		//pointSystemList = (GameObject) pointSystems;
		Debug.Log("inside the advancedPS2"+ pointSystems.Length);
		foreach (Object pointSystem in pointSystems)
		{
			GameObject tempGameObject = (GameObject)pointSystem;
			localPointSystemList.Add(tempGameObject);
		}
		int end = origin + 500;
		int calculatedEnd = origin + 50;
		int randomPSNumber1 = (int)Random.Range(0, localPointSystemList.Count);
		Debug.Log("This is the advancedThe Value of origin is " + origin);
		Instantiate(localPointSystemList[randomPSNumber1], new Vector3(0, 0, (float)origin), Quaternion.identity);
		int randomPSNumber2 = (int)Random.Range(0, localPointSystemList.Count);
		Instantiate(localPointSystemList[randomPSNumber2], new Vector3(0, 0, (float)origin+ 250), Quaternion.identity);
	}

	private void playerControl()
	{
		//Logic for the input touches and movements in the mobile
		Touch touch = Input.GetTouch(0);
		switch (touch.phase)
		{
			case TouchPhase.Began:
				verticalStartTouchPosition = touch.position.y;
				horizontalStartTouchPosition = touch.position.x;
				break;

			case TouchPhase.Ended:
				verticalEndTouchPosition = touch.position.y;
				horizontalEndTouchPosition = touch.position.x;

				if (verticalEndTouchPosition > verticalStartTouchPosition &&
					((verticalEndTouchPosition - verticalStartTouchPosition) > swipeSensitivity))
				{

					remy.SetTrigger("jump");

				}
				else if (verticalEndTouchPosition < verticalStartTouchPosition &&
						((verticalStartTouchPosition - verticalEndTouchPosition) > swipeSensitivity))
				{

					remy.SetTrigger("slide");

				}
				else if ((horizontalStartTouchPosition - horizontalEndTouchPosition) < swipeSensitivity)
				{
					Vector3 cameraCurrentPosition = cameraScript.getPosition();

					if ((horizontalStartTouchPosition < touchSensitivity && transform.position.x == -5.0f) ||
						(horizontalStartTouchPosition >= touchSensitivity && transform.position.x == 5.0f))
					{
						cameraTilt = true;
						if (speed >= 70.0f)
						{
							speed -= 50.0f;
						}
					}
					else if (horizontalStartTouchPosition >= touchSensitivity && transform.position.x < 5.0f)
					{
						cameraScript.setPosition(new Vector3(cameraCurrentPosition.x + 5.0f, cameraCurrentPosition.y, cameraCurrentPosition.z));
						rb.position = new Vector3(rb.position.x + 5.0f, rb.position.y, rb.position.z);						
					}
					else if (horizontalStartTouchPosition < touchSensitivity && transform.position.x > -5.0f)
					{
						cameraScript.setPosition(new Vector3(cameraCurrentPosition.x - 5.0f, cameraCurrentPosition.y, cameraCurrentPosition.z));
						rb.position = new Vector3(rb.position.x - 5.0f, rb.position.y, rb.position.z);						
					}

				}
				break;
		}

	}

	private void shakeCamera(){

		var cameraScript = GameObject.Find("Main Camera").GetComponent<camScript>();
		
		signValue *= -1;
		float cameraMovement = currentCameraShakeDistance * signValue;
		currentCameraShakeDistance -= currentCameraShakeDistance / cameraShake;

		if ( cameraShake == cameraShakeMax )
		{
			cameraOriginalPosition = cameraScript.getPosition();

			cameraScript.setPosition( new Vector3( ( cameraOriginalPosition.x + cameraMovement ) , cameraOriginalPosition.y , cameraOriginalPosition.z ) );

			cameraShake--;

		}else if ( cameraShake > 1 )
		{
			cameraScript.setPosition(new Vector3((cameraOriginalPosition.x + cameraMovement), cameraOriginalPosition.y, cameraOriginalPosition.z));
			cameraShake--;
		}
		else
		{
			cameraScript.setPosition(new Vector3( cameraOriginalPosition.x, cameraOriginalPosition.y, cameraOriginalPosition.z));
			cameraTilt = false;
			cameraShake = cameraShakeMax;
			currentCameraShakeDistance = maxCameraShakeDistance;
		}

	}

}
