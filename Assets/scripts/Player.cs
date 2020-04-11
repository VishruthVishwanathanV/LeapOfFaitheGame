using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public float speed;
	public int stop;
	public int swipeSensitivity;

	public GameObject preTerrain;
	public GameObject sucTerrain;
	//Logic to loop the terrain Map
	float terrainStep;
	float terrainLoop;

	private float verticalStartTouchPosition , verticalEndTouchPosition , horizontalStartTouchPosition , horizontalEndTouchPosition;
	private Vector3 horizontalMovement = new Vector3( 6.0f , 0.0f , 0.0f );

	private Rigidbody rb;
	private Animator remy;

	Object[] trees = new Object[ 100 ];
	Object[] pointSystems = new Object[ 100 ];
	List<GameObject> treeList = new List<GameObject>();
	List<GameObject> pointSystemList = new List<GameObject>();
	public double treeWidth = 400.0;
	public double initPosTree = 0.0;

	void Start(){

		rb = GetComponent<Rigidbody> ();
		remy = GetComponent<Animator> ();
		stop = 0;
		swipeSensitivity = 20;

		//Loogping terrain logic
		terrainStep = 500;
		terrainLoop = 1;

		//Logic to add trees for certain position		
		trees = Resources.LoadAll( "Trees" );
		//treeList = (GameObject) trees;
		foreach ( Object tree in trees ) {
			GameObject tempGameObject = ( GameObject ) tree;
			treeList.Add( tempGameObject );
		}

		pointSystems = Resources.LoadAll( "Points" );
		Debug.Log("pointsSystems Count is "+ pointSystems.Length);
		//pointSystemList = (GameObject) pointSystems;
		foreach ( Object pointSystem in pointSystems ) {
			GameObject tempGameObject = ( GameObject ) pointSystem;
			pointSystemList.Add( tempGameObject );
		}
		Debug.Log("Text: pointSystem List"+ pointSystemList);
		//basicPS(0);
		buildTree();

	}

	void Update(){

		//Logic for the terrain loop
		float z = gameObject.transform.position.z;

		if( z > ( ( terrainStep * terrainLoop ) + 20 ) ){

			terrainLoop += 1;

			if( ( preTerrain.transform.position.z + 500 ) < z ){

				preTerrain.transform.position = new Vector3( preTerrain.transform.position.x , preTerrain.transform.position.y , preTerrain.transform.position.z + ( terrainStep * 2 ) ) ;

			}
			if( ( sucTerrain.transform.position.z + 500 ) < z ){

				sucTerrain.transform.position = new Vector3( sucTerrain.transform.position.x , sucTerrain.transform.position.y , sucTerrain.transform.position.z + ( terrainStep * 2 ) ) ;

			}
		}

		/*//Logic for the terrain lopp
		if(  Input.GetAxis("Vertical") > 0 ) {
			
			remy.SetTrigger( "jump" );
			
		}*/


		//Logic for the input touches and movements in the mobile
		if ( Input.touchCount > 0 )
		{
			playerControl();
		}

	}

	void OnCollisionEnter(Collision collision)
	{

		if (collision.gameObject.CompareTag("coin"))
		{
			Destroy(collision.gameObject);
		}
		else if (collision.gameObject.CompareTag("obstacles"))
		{
			remy.SetTrigger("fallForward");
			rb.velocity = new Vector3(0, 0, 0);
			UnityEngine.SceneManagement.SceneManager.LoadScene("mainMenu");
			stop = 1;
		}
	}


	void FixedUpdate(){

		//To change the speed of the player
		if( stop == 1 ){

			rb.velocity = new Vector3( 0 , 0 , 0 );

		} else {

			rb.velocity = new Vector3( 0 , 0 , speed );

		}

		//Logic for the Looping of the trees sideways
		if( ( int )transform.position.z > ( initPosTree - 100.0 ) ){

			initPosTree = initPosTree + 500.0;
	
			buildTree();//Logic to loop build trees

		}

	}


	//Build tree loop for the building of trees
	void buildTree(){
		
		int distanceBetweenTree = 40;
		float treeDistanceFromCenter = 25f;
		double i;

		basicPS((int)initPosTree);

		for( i = initPosTree; i < ( treeWidth + initPosTree ); i++ ){

			//initPosTree = initPosTree + 500;
			i = i + distanceBetweenTree;

			int randTreeCountLeft = ( int )Random.Range ( 0 , treeList.Count - 1 );
			GameObject selectedTreeLeft = treeList[ randTreeCountLeft ];
			Instantiate( selectedTreeLeft , new Vector3( treeDistanceFromCenter, 0 , ( float ) i ) , Quaternion.identity );

			int randTreeCountRight = ( int )Random.Range ( 0 , treeList.Count );
			GameObject selectedTreeRight = treeList[ randTreeCountRight ];
			Instantiate( selectedTreeRight , new Vector3( -treeDistanceFromCenter , 0 , ( float ) i ), Quaternion.identity );

			selectedTreeLeft.gameObject.tag = "treeOut";
			selectedTreeRight.gameObject.tag = "treeOut";

		}

		//Logic to remove unwanted trees
		GameObject[] treesBehind = GameObject.FindGameObjectsWithTag( "treeOut" );

		if( treesBehind != null ){

			for( int j = 0; j < treesBehind.Length; j++ ){

				GameObject thisGameObject = treesBehind[ j ];

				if( thisGameObject.transform.position.z < transform.position.z ){

					Destroy( treesBehind[ j ] );

				}
			}
		}

		//Instantiate a gameObject for creating
	}

	//Basic point system for the Initial
	void basicPS(int origin){

		int end = origin + 500;
		int calculatedEnd = origin +50;
		int randPS = ( int )Random.Range ( 0 , pointSystemList.Count - 1 );

		Debug.Log("The Value of the iniside the function randPS is "+ randPS);

		Debug.Log("The Value of the length of the transform origin is "+ origin );
		//initialize from origin+50
		 

		for( int i=origin+50 ; i<end ; i=i+150 ){

			int randPlacing = ( int )Random.Range ( 0 , 2 );
			Debug.Log("The Value of the PSIndex1 is "+ randPlacing);

			switch(randPlacing){
			case 0:
				Instantiate( pointSystemList[ randPS ] , new Vector3( -5 , 0 , ( float ) i  ), Quaternion.identity );
				break;
			case 1:
				Instantiate( pointSystemList[ randPS ] , new Vector3( 0 , 0 , ( float ) i ), Quaternion.identity );
				break;
			case 2:
				Instantiate( pointSystemList[ randPS ] , new Vector3( 5 , 0 , ( float ) i ), Quaternion.identity );
				break;
			}
			Debug.Log("The Value of the PSIndex1 is "+ calculatedEnd);

		}

	}

	private void playerControl()
	{
		//Logic for the input touches and movements in the mobile
		for (int i = 0; i < Input.touchCount; i++)
		{

			Touch touch = Input.GetTouch(i);

			if (touch.phase == TouchPhase.Began)
			{

				verticalStartTouchPosition = touch.position.y;
				horizontalStartTouchPosition = touch.position.x;

			}
			else if (touch.phase == TouchPhase.Ended)
			{

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
				else if ((horizontalEndTouchPosition > horizontalStartTouchPosition) &&
						((horizontalEndTouchPosition - horizontalStartTouchPosition) > swipeSensitivity) && transform.position.x <= 0.0f)
				{

					rb.MovePosition(transform.position + horizontalMovement);

				}
				else if ((horizontalEndTouchPosition < horizontalStartTouchPosition) &&
						((horizontalStartTouchPosition - horizontalEndTouchPosition) > swipeSensitivity) && transform.position.x >= 0.0f)
				{

					rb.MovePosition(transform.position - horizontalMovement);
				}
			}
		}
	}

}
