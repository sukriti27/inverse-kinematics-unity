<b>Cyclic Coordinate Descent (CCD)</b> - Implementation of CCD to make the character's arms reach a target object.
<p align="justify">
The Inverse Kinematics (IK) solver is able solve the target positions and rotations at the same time as shown in the figure below. Weights were added to every bone in the chain and used when updating each bone. If the character is not able to reach the target by movement of its arm, CCD is applied on the chain of chest bones to make the spine bend in various directions so that the eﬀector can reach the target position. </p>

<div align="center">
<img src="https://github.com/sukriti27/inverse-kinematics-unity/blob/master/Assets/Assignment/InverseKinematics/target.png" width="338" height="262" /> &nbsp; &nbsp;  <img src="https://github.com/sukriti27/inverse-kinematics-unity/blob/master/Assets/Assignment/InverseKinematics/ChestCCD.png" width="300" height="262" />
</div>

<br>

<b>Foot IK Mocap Editing</b> - Implementation of plausible foot correction as the character walks on an uneven terrain.
<p align="justify">
Plausible foot correction has been implemented to adjust the height and angle of the foot. Target position is found for both the ankles by casting multiple rays from a circle around the pivot point to the ground and taking the average height. This gives a smoothed height value, eliminating sudden artefacts in posture changes. A heuristic adjustment has been made for the root to avoid kneeling when the ground height becomes too high. </p>

<div align="center">
<img src="https://github.com/sukriti27/inverse-kinematics-unity/blob/master/Assets/Assignment/MocapEditing/height1.png" width="222" height="250" /> &nbsp; &nbsp; <img src="https://github.com/sukriti27/inverse-kinematics-unity/blob/master/Assets/Assignment/MocapEditing/height2.png" width="227" height="250" />  &nbsp; &nbsp; <img src="https://github.com/sukriti27/inverse-kinematics-unity/blob/master/Assets/Assignment/MocapEditing/rotationadjust.png" width="195" height="250" />
</div>

The character tries to touch the sphere added to the scene with its arms when it comes close to it. 

<div align="center">
 <img src="https://github.com/sukriti27/inverse-kinematics-unity/blob/master/Assets/Assignment/MocapEditing/touch.png" width="236" height="250" />
</div>
