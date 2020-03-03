using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject rightWrist = GameObject.Find("RightWrist");
        Debug.Log("Position of right wrist in world space: " + rightWrist.transform.position);
        Debug.Log("Position of right wrist relative to its parent i.e. right elbow: " + rightWrist.transform.localPosition);

        rightWrist.transform.rotation = Quaternion.Euler(20f, 50f, 10f);
        Debug.Log("Right wrist rotation after using Quaternion.Euler: " + rightWrist.transform.eulerAngles);

        rightWrist.transform.rotation = Quaternion.AngleAxis(10f, new Vector3(0f, 0f, 1f));
        rightWrist.transform.rotation = Quaternion.AngleAxis(20f, new Vector3(1f, 0f, 0f)) * rightWrist.transform.rotation;
        rightWrist.transform.rotation = Quaternion.AngleAxis(50f, new Vector3(0f, 1f, 0f)) * rightWrist.transform.rotation;
        Debug.Log("Right wrist rotation after using Quaternion.AngleAxis: " + rightWrist.transform.eulerAngles);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = this.transform.position + new Vector3(-0.1f, 0f, 0f) * Time.deltaTime;
    }
}
