using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ManualFK1 : MonoBehaviour {
    private void Start()
    {
        GameObject rightShoulder = GameObject.Find("RightShoulder");
        GameObject rightElbow = GameObject.Find("RightElbow");
        GameObject rightWrist = GameObject.Find("RightWrist");
        rightShoulder.transform.localRotation = Quaternion.AngleAxis(45f, new Vector3(0f, 1f, 0f));
        rightElbow.transform.localRotation = Quaternion.AngleAxis(20f, new Vector3(0f, 1f, 0f));
        rightWrist.transform.localRotation = Quaternion.AngleAxis(10f, new Vector3(0f, 1f, 0f));
    }

    void Update() {
        GameObject rightShoulder = GameObject.Find("RightShoulder");
        GameObject rightWrist = GameObject.Find("RightWrist");
        Quaternion initialWristRotation = rightWrist.transform.rotation;
        rightShoulder.transform.rotation = Quaternion.AngleAxis(90f * Time.deltaTime, new Vector3(0f, 0f, 1f)) * rightShoulder.transform.rotation;
        rightWrist.transform.rotation = initialWristRotation;
    }
}
