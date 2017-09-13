using System;
using UnityEngine;

using UnityStandardAssets.CrossPlatformInput;
[RequireComponent(typeof(TransparencyController))]
[RequireComponent(typeof(CameraDampingController))]
public class CameraPositionController : MonoBehaviour {
   
    //position of our camera
    Transform cameraPosition;
    //position of the pivot point
    Transform pivotPoint;
    //the object that will be child to the player which our pivot point will follow it's position
    GameObject pivotPointPosition;
    [Tooltip("Check this if you want the camera to be free from a players rotation")]
    public bool freeRoation;
    //these values pertain to our camera, distance from player, and offset for obstacle avoidence
    [Tooltip("Set the maximum distace from our camera to the player. (Default is 10)")]
    public float maxCameraDistance;
    [Tooltip("Set the minimum distace from our camera to the player. (Default is 0.3)")]
    public float minCameraDistance;
    [Tooltip("Set how high the camera can go. (Default is 75)")]
    public float maxCameraAngle;
    [Tooltip("Set how low the camera can go. (Default is -30)")]
    public float minCameraAngle;
    float distance;
	float defaultDistance;
	float distanceOffset;

    //h and v are out horizontal and vertical axis input
	float h;
	float v;
  
    [Tooltip("Set the sensitivity of camera movement when using the mouse (Default is 10)")]
    public int sensitivity;

	//these are used for clamping our camerea rotation
	float currentx;
	float currenty;
	bool aboveX;

    //the rotation and position vector of our camera's position
	Vector3 currentRotation;
	Vector3 targetposition;

    [Tooltip("The mask that the SphereCast will hit objects. This will be set to default automatically.")]
    public LayerMask sphereCastMask;
    [Tooltip("Set the size of the SphereCast for the camera's obstacle avoidence. You will want to put this higher or lower your camera's near clip pane if the camera is clipping through objects. (Default is 0.3)")]
    public float castRadius;

    TransparencyController transparency;
    [Tooltip("Allow the player to turn transparent when the camera is close enough (This requires your player to have a skinned mesh renderer.)")]
    public bool allowTransparency;
    [Tooltip("Set how far from the player that it is turned transparent. (Default is 1)")]
    public float transparencyDistance;
    

    //initialize any values that have not been set
    void Start () {
        try
        {
            cameraPosition = GameObject.FindGameObjectWithTag("cameraPosition").transform;
            pivotPoint = GameObject.FindGameObjectWithTag("pivotPoint").transform;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("pivotPoint or cameraPosition not found. Make sure tags are set properly.");
        }
        if (maxCameraDistance == 0) maxCameraDistance = 10.0f;
        if (minCameraDistance == 0) minCameraDistance = 0.3f;
        if (maxCameraAngle == 0) maxCameraAngle = 75;
        if (minCameraAngle == 0) minCameraAngle = -30;
        distance = maxCameraDistance / 2;
        defaultDistance = distance;       
        if (sphereCastMask.value == 0) sphereCastMask.value = 1;
        if (castRadius == 0) castRadius = 0.3f;      
        if (allowTransparency) transparency = GetComponent<TransparencyController>();
        if(transparencyDistance == 0) transparencyDistance = 1.0f;
        if (sensitivity == 0) sensitivity = 10;
        //sets up our camera for free rotation independent from the players rotation
        if (freeRoation)
        {
            pivotPointPosition = new GameObject();
            pivotPointPosition.name = "pivotPointPosition";
            pivotPointPosition.transform.position = pivotPoint.transform.position;
            pivotPointPosition.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;         
            this.gameObject.transform.parent = null;            
        }


	}

   
    void FixedUpdate() {
        
        //these get our current rotation angles of the pivot point.      
        currentx = pivotPoint.transform.eulerAngles.x;
        currenty = pivotPoint.transform.eulerAngles.y;

        //keeps our pivotPoint at the players position
       if (freeRoation) this.gameObject.transform.position = pivotPointPosition.transform.position;
       
        //these lines keep our z-axis always at 0 that way out camera doesnt tilt
        pivotPoint.transform.rotation = Quaternion.Euler(currentx, currenty, 0);
        cameraPosition.transform.rotation = Quaternion.Euler(currentx, currenty, 0);
        //gets joystick or mouse movement
        h = sensitivity * CrossPlatformInputManager.GetAxis("Mouse X");
        v = sensitivity * CrossPlatformInputManager.GetAxis("Mouse Y");

        //gets if we have mouse or joystick input and clamps the camera
        if (h != 0 || v != 0) clampCamera();
        
        //this is obstacle avoidence for our camera
        RaycastHit hit;
        //forward vector of our camera
        Vector3 camForward = cameraPosition.transform.forward;
        //right vector of our camera
        Vector3 camRight = cameraPosition.transform.right;
    
        //this is the main spherecast that creates our distance offset.
        //This algorithm is a very nice obstacle avoidance system that can be used for other projects.
        if(Physics.SphereCast(pivotPoint.transform.position,castRadius,cameraPosition.transform.position-pivotPoint.transform.position,out hit, defaultDistance + 0.3f, sphereCastMask))
        {
             float cDistance = Vector3.Distance(pivotPoint.transform.position, hit.point);
            //the + 0.3f is for clipping issues
             distanceOffset = defaultDistance - cDistance + 0.3f;
        }
        else
        {
           distanceOffset = 0;
        }


        Quaternion targRotation = Quaternion.LookRotation (camForward,Vector3.up);
		//end of obstacle avoidence

	//Calculates the camera's distance from player.
	if (Math.Abs(Input.GetAxis ("Mouse ScrollWheel")) > 0) {
            defaultDistance += -Input.GetAxis ("Mouse ScrollWheel");
            
                //prevents our default distance to get any closer to the player.
                if (defaultDistance < minCameraDistance)
             {
                defaultDistance = minCameraDistance;
             }
	}

		//sets our camera's distance
		distance = defaultDistance - distanceOffset;

        //If transparency is enable we will check if our camera is close enough to turn the player transparent.
        if (allowTransparency)
        {
            if (distance < transparencyDistance) transparency.transparent();
            else transparency.solid();
        }

        //maximum distance our camera can be from our player.
        //we cannot set the minimum defaultDistance because it will set when the collision moves the camera within that parameter.
        //what that means is if our obstacle avoidence pushes the camera into our player it won't force the camera out and cause more graphical issues.
        if (distance <= minCameraDistance) {
		distance = minCameraDistance;
	} else if (distance >= maxCameraDistance) {
		distance = maxCameraDistance;
		defaultDistance = maxCameraDistance;

	}

        //this is ran last because we want to calculate everything before we use the new values.
		//this calculates our new position from the player using the distance float and relative positions.
		targetposition = pivotPoint.transform.position - camForward * distance;
		cameraPosition.transform.position = targetposition;
		transform.rotation = targRotation;
        
	} //end of fixed update

    //Clamps our camera. We need to get above and below x-axis because eulerangles does not have negatives.
    void clampCamera()
    {
        if (cameraPosition.transform.eulerAngles.x <= maxCameraAngle)
        {
            aboveX = true;
            cameraPosition.transform.RotateAround(pivotPoint.transform.position, cameraPosition.transform.right, sensitivity * -v * Time.deltaTime);
            cameraPosition.transform.RotateAround(pivotPoint.transform.position, cameraPosition.transform.up, sensitivity * h * Time.deltaTime);

        }
        else if (aboveX == true)
        {
            if (v > 0)
            {
                cameraPosition.transform.RotateAround(pivotPoint.transform.position, cameraPosition.transform.right, sensitivity * -v * Time.deltaTime);
            }
            cameraPosition.transform.RotateAround(pivotPoint.transform.position, cameraPosition.transform.up, sensitivity * h * Time.deltaTime);

        }
        if (cameraPosition.transform.eulerAngles.x - 360 >= minCameraAngle)
        {
            aboveX = false;

            cameraPosition.transform.RotateAround(pivotPoint.transform.position, cameraPosition.transform.right, sensitivity * -v * Time.deltaTime);
            cameraPosition.transform.RotateAround(pivotPoint.transform.position, cameraPosition.transform.up, sensitivity * h * Time.deltaTime);

        }
        else if (aboveX == false)
        {
            if (v < 0)
            {
                cameraPosition.transform.RotateAround(pivotPoint.transform.position, cameraPosition.transform.right, sensitivity * -v * Time.deltaTime);
            }
            cameraPosition.transform.RotateAround(pivotPoint.transform.position, cameraPosition.transform.up, sensitivity * h * Time.deltaTime);
        }

    }


}
