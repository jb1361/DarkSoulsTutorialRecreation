

# INSTRUCTIONS

	[REQUIRED]
	If you do not have CrossPlatformInput package you must import it. 
	To do this go to:  Assets->import package->CrossPlatformInput
	This is required since the asset allows you to use a controller to rotate the camera.

Step 1: If your scene does not have a camera then create a camera and set it's tag to "Main Camera."

Step 2: Place The ThirdPersonCameraModule into your scene then position the module where you want your camera to rotate around.
		If you only want to rotate your camera around a player or object that doesn't rotate then you are finished.

[OPTIONAL]
Step 3: You may child the module to the player or object to keep the Hierarchy cleaner but the module will be unparented during runtime.

Step 4: If you want your camera to be free from a players or objects rotation then check "Free Rotation."
		[REQUIRED]
		You must have your players or object tag set to "Player" or else it will break.

Step 5: Press play and check to see if everything is working correctly.
		There will be an object called pivotPointPosition that is a child to the object tagged player.
		

[OPTIONAL]
Step 6: If you want to be able to use an Xbox controller then you must goto Edit->Project Settings->Input
		From there you will right click and select "Duplicate Array Elemtent" for the "Mouse X" and "Mouse Y" inputs.
		On the second set of inputs you created you will change:
		 Mouse X: Type-> Joystick Axis
				  Axis-> 4th Axis (Joysticks)
				  Joy Num->Get Motion from all Joysticks
				  Sensitivity-> 1

	    Mouse Y: Type-> Joystick Axis
				 Axis-> 5th Axis (Joysticks)
				 Joy Num->Get Motion from all Joysticks
				 Sensitivity-> 1
				 						


 You can also easily Implement this camera system with Unity's Standard Asset third person character.
 Goto Assets->Import Package->Characters
 Once you import the package find the ThirdPersonCharacter Prefab under the folder Standard Assets->Characters->ThirdPersonCharacter->Prefab
 Place the ThirdPersonCOntroller prefab in your scene and tag it as "Player".
 Now The Camera Module will be set up with the character which offers a very nice third person Character. 



 Controls: Mouse movement to rotate the camera.
		   Scroll wheel to increase or decrease camera distance from the player or object.
		   Right thumbstick on Xbox controller to rotate camera. (Only if you set up the input)

 #END OF INSTRUCTIONS



 #SCRIPTS AND VARIABLES

 You have a range of options that you may edit and tweak to your liking.

 -Camera Position Controller-
 This script controls camera position, movement, and sets up the the module for free rotation.
 You have these options to adjust the camera.
 This script reuires the CrossPlatformInput, Camera Daping Controller, Transparency Controller, and objects with cameraPosition, pivotPoint, and Player tags.

 Max Camera Distance-The maximum distance the camera can be from the player or object.
 Min Camerea Distance-The minimum distance the camera can be from the player or object.
 Max Camera Angle-The maximum angle the camera can go above the player or object.
 Min Camera Angle-The minimum angle the camera can go above the player or object.
 Sensitivity-The sensitivity of camera rotation.
 Sphere Cast Mask-Select which layer that you want the obstacle avoidance to avoid. (if you don't want what camera to avoid an object then change that objects layer)
 Cast Radius-The radius of the Sphere Cast. (if you have any sort of clipping issues then make this larger or increase your camera's far clip pane)
 Allow Transparency-Check this if you want your player or object to turn transparent when the camera gets close enough. (this will require your player to have a skinned mesh renderer)
 Transparency Distance-The distance at which the player or object turns transparent.



 -Camera Damping Controller-
 This script moves the main camera to the cameraPosition object while damping the movement to avoid jittering.
 This script requires Objects with the tags Main Camera, pivoPoint, and cameraPosition

 Dampingx-Dampens the x-axis movement of the camera.
 Dampingy-Dampens the y-axis movement of the camera.
 Dampingz-Dampens the z-axis movement of the camera.


 -Transparency Controller-
 This script has two functions, transparent() and solid()
 The only thing this script require is an object tagged "Player"
 This script does not require the Camera Position Controller or the Camera Damping Controller to function so you can use this script for other purposed outside of this module.
 
 The script will provide an error if there is no skined mesh renderer or no objects with the "Player" tag.
 It was designed that way so it does not need a reference to the Camera Position Controller to check if Allow Tranparency is true.

 Transparency Ammount-How transparent the player or object will be.



 -Mouse Lock-
 This script locks your cursor and makes it not visible and vis versa when you press escape.
 This script is entirely optional and is not required, you may remove this scirpt and have no issues.


 #END OF SCRIPTS AND VARIABLES
  

  #INCLUDED IN PACKAGE

  -ThirdPersonCameraModule Prefab
  -Camera Position Controller
  -Camera Damping Controller
  -Transparency Controller
  -Mouse Lock


  If you have any issues or questions please email me at: justinbutler4@hotmail.com


