#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public class EditingModule : Module
{

    public enum Projection { Naive, Advanced };                               //For subtasks 2 & 3

    public LayerMask Ground = ~0;
    public int LeftHip = 0;
    public int LeftAnkle = 0;
    public int RightHip = 0;
    public int RightAnkle = 0;

    public bool FixBending = false;                                         //For subtask 4 change to true in the inspector
    public bool RootAdjustment = false;                                     //For subtask 3 change to true in the inspector
    public Projection GroundProjection = Projection.Naive;                  //For subtasks 2 & 3 change to Advanced in the inspector

    [System.NonSerialized] private const float RayCastLength = 100f;

    public override ID GetID()
    {
        return ID.Editing;
    }

    public override Module Initialise(MotionData data)
    {
        Data = data;
        Ground = LayerMask.GetMask("Ground");
        LeftHip = data.Source.FindBone("LeftHip").Index;
        LeftAnkle = data.Source.FindBone("LeftAnkle").Index;
        RightHip = data.Source.FindBone("RightHip").Index;
        RightAnkle = data.Source.FindBone("RightAnkle").Index;
        return this;
    }

    //Implement the editing in here
    protected override void DerivedCallback(MotionEditor editor)
    {
        Transform leftAnkleTransform = editor.GetActor().FindTransform(Data.Source.Bones[LeftAnkle].Name);
        Transform leftHipTransform = editor.GetActor().FindTransform(Data.Source.Bones[LeftHip].Name);

        // Chain from left hip to left ankle
        Transform[] leftLegChain = GetChain(leftHipTransform, leftAnkleTransform);
        if (leftLegChain.Length == 0)
        {
            Debug.Log("Given root and tip are not valid.");
            return;
        }

        Transform rightAnkleTransform = editor.GetActor().FindTransform(Data.Source.Bones[RightAnkle].Name);
        Transform rightHipTransform = editor.GetActor().FindTransform(Data.Source.Bones[RightHip].Name);

        // Chain from right hip to right ankle
        Transform[] rightLegChain = GetChain(rightHipTransform, rightAnkleTransform);
        if (rightLegChain.Length == 0)
        {
            Debug.Log("Given root and tip are not valid.");
            return;
        }

        // Target heights for left and right ankle
        Vector3 leftAnkleTargetPosition, rightAnkleTargetPosition;
        if (GroundProjection == Projection.Naive)
        {
            leftAnkleTargetPosition = leftAnkleTransform.position + new Vector3(0f, GetHeight(leftAnkleTransform.position), 0f);
            rightAnkleTargetPosition = rightAnkleTransform.position + new Vector3(0f, GetHeight(rightAnkleTransform.position), 0f);
        }
        else
        {
            // Smoothed height value through multiple raycasts in a circle around the pivot point
            leftAnkleTargetPosition = leftAnkleTransform.position + new Vector3(0f, GetSmoothedHeight(leftAnkleTransform.position), 0f);
            rightAnkleTargetPosition = rightAnkleTransform.position + new Vector3(0f, GetSmoothedHeight(rightAnkleTransform.position), 0f);
        }

        if (RootAdjustment)
        {
            // Heuristic adjustment for root
            float target = Mathf.Min(leftAnkleTransform.position.y, rightAnkleTransform.position.y) - Mathf.Min(leftAnkleTargetPosition.y, rightAnkleTargetPosition.y);
            editor.GetActor().transform.position += -Vector3.up * target;
        }

        float sqrDistance;
        int iterationCount = 0;

        // CCD IK for left leg
        do
        {
            for (int i = leftLegChain.Length - 1; i >= 0; i--)
            {
                Vector3 boneToEffector = leftAnkleTransform.position - leftLegChain[i].position;
                Vector3 boneToGoal = leftAnkleTargetPosition - leftLegChain[i].position;
                leftLegChain[i].rotation = Quaternion.FromToRotation(boneToEffector, boneToGoal) * leftLegChain[i].rotation;

                sqrDistance = (leftAnkleTransform.position - leftAnkleTargetPosition).sqrMagnitude;
                if (sqrDistance <= 0.001f)
                    break;
            }
            iterationCount++;
        } while (iterationCount < 50);

        iterationCount = 0;

        // CCD IK for right leg
        do
        {
            for (int i = rightLegChain.Length - 1; i >= 0; i--)
            {
                Vector3 boneToEffector = rightAnkleTransform.position - rightLegChain[i].position;
                Vector3 boneToGoal = rightAnkleTargetPosition - rightLegChain[i].position;
                rightLegChain[i].rotation = Quaternion.FromToRotation(boneToEffector, boneToGoal) * rightLegChain[i].rotation;

                sqrDistance = (rightAnkleTransform.position - rightAnkleTargetPosition).sqrMagnitude;
                if (sqrDistance <= 0.001f)
                    break;
            }
            iterationCount++;
        } while (iterationCount < 50);

        // Adjust foot orientation based on normal
        leftAnkleTransform.rotation = Quaternion.FromToRotation(Vector3.up, GetNormal(leftAnkleTransform.position)) * leftAnkleTransform.rotation;
        rightAnkleTransform.rotation = Quaternion.FromToRotation(Vector3.up, GetNormal(rightAnkleTransform.position)) * rightAnkleTransform.rotation;

        GameObject leftWrist = GameObject.Find("LeftWrist");
        GameObject sphere = GameObject.Find("Sphere");

        // Try to touch the sphere with the left arm if it is close to it
        if (Vector3.Distance(sphere.transform.position, leftWrist.transform.position) < 1.5f)
        {
            Vector3 targetLeftWristPosition = Physics.ClosestPoint(leftWrist.transform.position, sphere.transform.GetComponent<Collider>(), sphere.transform.position, sphere.transform.rotation);

            GameObject leftShoulder = GameObject.Find("LeftShoulder");
            // Chain from left shoulder to left wrist
            Transform[] leftArmChain = GetChain(leftShoulder.transform, leftWrist.transform);
            if (leftArmChain.Length == 0)
            {
                Debug.Log("Given root and tip are not valid.");
                return;
            }

            iterationCount = 0;

            // CCD IK for left arm
            do
            {
                for (int i = leftArmChain.Length - 1; i >= 0; i--)
                {
                    Vector3 boneToEffector = leftWrist.transform.position - leftArmChain[i].position;
                    Vector3 boneToGoal = targetLeftWristPosition - leftArmChain[i].position;
                    leftArmChain[i].rotation = Quaternion.FromToRotation(boneToEffector, boneToGoal) * leftArmChain[i].rotation;
                    sqrDistance = (leftWrist.transform.position - targetLeftWristPosition).sqrMagnitude;
                    if (sqrDistance <= 0.001f)
                        break;
                }
                iterationCount++;
            } while (iterationCount < 10);
        }

        // Try to touch the sphere with the right arm if it is close to it
        GameObject rightWrist = GameObject.Find("RightWrist");

        if (Vector3.Distance(sphere.transform.position, rightWrist.transform.position) < 1.5f)
        {
            Vector3 targetRightWristPosition = Physics.ClosestPoint(rightWrist.transform.position, sphere.transform.GetComponent<Collider>(), sphere.transform.position, sphere.transform.rotation);

            GameObject rightShoulder = GameObject.Find("RightShoulder");
            // Chain from right shoulder to right wrist
            Transform[] rightArmChain = GetChain(rightShoulder.transform, rightWrist.transform);
            if (rightArmChain.Length == 0)
            {
                Debug.Log("Given root and tip are not valid.");
                return;
            }

            iterationCount = 0;

            // CCD IK for right arm
            do
            {
                for (int i = rightArmChain.Length - 1; i >= 0; i--)
                {
                    Vector3 boneToEffector = rightWrist.transform.position - rightArmChain[i].position;
                    Vector3 boneToGoal = targetRightWristPosition - rightArmChain[i].position;
                    rightArmChain[i].rotation = Quaternion.FromToRotation(boneToEffector, boneToGoal) * rightArmChain[i].rotation;
                    sqrDistance = (rightWrist.transform.position - targetRightWristPosition).sqrMagnitude;
                    if (sqrDistance <= 0.001f)
                        break;
                }
                iterationCount++;
            } while (iterationCount < 10);
        }
    }

    //Function for getting smoothed height value through multiple raycasts in a circle around the pivot point
    private float GetSmoothedHeight(Vector3 origin)
    {
        RaycastHit hit;
        float average = 0f, radius = 0.05f;
        const int SAMPLE_COUNT = 500;
        for (int i = 0; i < SAMPLE_COUNT; i++)
        {
            Vector2 random = Random.insideUnitCircle * radius;
            Ray ray = new Ray(new Vector3(origin.x + random.x, origin.y + RayCastLength / 2f, origin.z + random.y), Vector3.down);
            Physics.Raycast(ray, out hit, RayCastLength, Ground);
            average += hit.point.y;
        }
        return average / SAMPLE_COUNT;
    }

    //Use this function to get the ground height
    private float GetHeight(Vector3 origin)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(origin.x, origin.y + RayCastLength / 2f, origin.z), Vector3.down, out hit, RayCastLength, Ground))
        {
            return hit.point.y;
        }
        else
        {
            return origin.y;
        }
    }

    //Use this function to get the ground normal
    private Vector3 GetNormal(Vector3 origin)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(origin.x, origin.y + RayCastLength / 2f, origin.z), Vector3.down, out hit, RayCastLength, Ground))
        {
            return hit.normal;
        }
        else
        {
            return Vector3.up;
        }
    }

    //Optional scene drawing function
    protected override void DerivedDraw(MotionEditor editor)
    {
        UltiDraw.Begin();

        Transform leftAnkle = editor.GetActor().FindTransform(Data.Source.Bones[LeftAnkle].Name);
        Transform rightAnkle = editor.GetActor().FindTransform(Data.Source.Bones[RightAnkle].Name);

        UltiDraw.DrawWireCircle(leftAnkle.position, 0.08f, Color.red);
        UltiDraw.DrawWireCircle(rightAnkle.position, 0.08f, Color.red);

        UltiDraw.End();
    }

    public static Transform[] GetChain(Transform root, Transform end)
    {
        if (root == null || end == null)
        {
            return new Transform[0];
        }
        List<Transform> chain = new List<Transform>();
        Transform joint = end;
        chain.Add(joint);
        while (joint != root)
        {
            joint = joint.parent;
            if (joint == null)
            {
                return new Transform[0];
            }
            else
            {
                chain.Add(joint);
            }
        }
        chain.Reverse();
        return chain.ToArray();
    }

    //Optional inspector fields
    protected override void DerivedInspector(MotionEditor editor)
    {
        Ground = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(EditorGUILayout.MaskField("Ground", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(Ground), InternalEditorUtility.layers));
        LeftHip = EditorGUILayout.Popup("Left Hip", LeftHip, Data.Source.GetBoneNames());
        LeftAnkle = EditorGUILayout.Popup("Left Ankle", LeftAnkle, Data.Source.GetBoneNames());
        RightHip = EditorGUILayout.Popup("Right Hip", RightHip, Data.Source.GetBoneNames());
        RightAnkle = EditorGUILayout.Popup("Right Ankle", RightAnkle, Data.Source.GetBoneNames());
        GroundProjection = (Projection)EditorGUILayout.EnumPopup("Ground Projection", GroundProjection);
        RootAdjustment = EditorGUILayout.Toggle("Root Adjustment", RootAdjustment);
        FixBending = EditorGUILayout.Toggle("Fix Bending", false);
    }

    //Ingore this function
    protected override void DerivedLoad(MotionEditor editor)
    {

    }

    //Ingore this function
    public override TimeSeries ExtractSeries(MotionEditor editor, TimeSeries timeSeries = null)
    {
        return null;
    }

    //Ingore this function
    protected override void DerivedHandlePrecomputation()
    {

    }

    //Ingore this function
    protected override void DerivedGUI(MotionEditor editor)
    {

    }

}
#endif