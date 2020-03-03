using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ManualFK : MonoBehaviour
{
    public Transform Root = null;
    public Transform Tip = null;
    private int step = 0;

    void Update()
    {
        if (Root == null || Tip == null)
        {
            Debug.Log("Some inspector variable has not been assigned and is still null.");
            return;
        }

        Transform[] chain = CCD.GetChain(Root, Tip);
        if (chain.Length == 0)
        {
            Debug.Log("Given root and tip are not valid.");
            return;
        }

        GameObject rightShoulder = GameObject.Find("RightShoulder");
        GameObject rightElbow = GameObject.Find("RightElbow");
        GameObject rightWrist = GameObject.Find("RightWrist");

        // Moving hand to initial position
        if (step == 0)
        {
            rightShoulder.transform.localRotation = Quaternion.RotateTowards(rightShoulder.transform.localRotation, Quaternion.Euler(0f, 45f, 0f), 20f * Time.deltaTime);
            rightElbow.transform.localRotation = Quaternion.RotateTowards(rightElbow.transform.localRotation, Quaternion.Euler(0f, 30f, 0f), 15f * Time.deltaTime);
            rightWrist.transform.localRotation = Quaternion.RotateTowards(rightWrist.transform.localRotation, Quaternion.Euler(0f, 0f, -60f), 20f * Time.deltaTime);

            // Checking if target rotation has been reached in order to switch to the next step.
            if (rightElbow.transform.localRotation == Quaternion.Euler(0f, 30f, 0f)
                && rightShoulder.transform.localRotation == Quaternion.Euler(0f, 45f, 0f)
                && rightWrist.transform.localRotation == Quaternion.Euler(0f, 0f, -60f))
                step = 1;
        }
        // First quarter of the circle
        else if (step == 1)
        {
            rightShoulder.transform.localRotation = Quaternion.RotateTowards(rightShoulder.transform.localRotation, Quaternion.Euler(0f, 90f, -45f), 20f * Time.deltaTime);
            rightElbow.transform.localRotation = Quaternion.RotateTowards(rightElbow.transform.localRotation, Quaternion.Euler(0f, 0f, 30f), 15f * Time.deltaTime);
            rightWrist.transform.localRotation = Quaternion.RotateTowards(rightWrist.transform.localRotation, Quaternion.Euler(0f, 0f, -60f), 20f * Time.deltaTime);
            if (rightElbow.transform.localRotation == Quaternion.Euler(0f, 0f, 30f)
                && rightShoulder.transform.localRotation == Quaternion.Euler(0f, 90f, -45f)
                && rightWrist.transform.localRotation == Quaternion.Euler(0f, 0f, -60f))
                step = 2;
        }
        // Second quarter of the circle
        else if (step == 2)
        {
            rightShoulder.transform.localRotation = Quaternion.RotateTowards(rightShoulder.transform.localRotation, Quaternion.Euler(0f, 135f, 0f), 20f * Time.deltaTime);
            rightElbow.transform.localRotation = Quaternion.RotateTowards(rightElbow.transform.localRotation, Quaternion.Euler(0f, 0f, 10f), 15f * Time.deltaTime);
            rightWrist.transform.localRotation = Quaternion.RotateTowards(rightWrist.transform.localRotation, Quaternion.Euler(0f, 0f, -50f), 20f * Time.deltaTime);
            if (rightElbow.transform.localRotation == Quaternion.Euler(0f, 0f, 10f)
                && rightShoulder.transform.localRotation == Quaternion.Euler(0f, 135f, 0f)
                && rightWrist.transform.localRotation == Quaternion.Euler(0f, 0f, -50f))
                step = 3;
        }
        // Third qaurter of the circle
        else if (step == 3)
        {
            rightShoulder.transform.localRotation = Quaternion.RotateTowards(rightShoulder.transform.localRotation, Quaternion.Euler(0f, 90f, 45f), 20f * Time.deltaTime);
            rightElbow.transform.localRotation = Quaternion.RotateTowards(rightElbow.transform.localRotation, Quaternion.Euler(0f, 0f, -10f), 15f * Time.deltaTime);
            rightWrist.transform.localRotation = Quaternion.RotateTowards(rightWrist.transform.localRotation, Quaternion.Euler(0f, 0f, -60f), 20f * Time.deltaTime);
            if (rightElbow.transform.localRotation == Quaternion.Euler(0f, 0f, -10f)
                && rightShoulder.transform.localRotation == Quaternion.Euler(0f, 90f, 45f)
                && rightWrist.transform.localRotation == Quaternion.Euler(0f, 0f, -60f))
                step = 4;
        }
        // Fourth quater of the circle
        else if (step == 4)
        {
            rightShoulder.transform.localRotation = Quaternion.RotateTowards(rightShoulder.transform.localRotation, Quaternion.Euler(0f, 45f, 0f), 20f * Time.deltaTime);
            rightElbow.transform.localRotation = Quaternion.RotateTowards(rightElbow.transform.localRotation, Quaternion.Euler(0f, 30f, 0f), 15f * Time.deltaTime);
            rightWrist.transform.localRotation = Quaternion.RotateTowards(rightWrist.transform.localRotation, Quaternion.Euler(0f, 0f, -60f), 20f * Time.deltaTime);
            if (rightElbow.transform.localRotation == Quaternion.Euler(0f, 30f, 0f)
                && rightShoulder.transform.localRotation == Quaternion.Euler(0f, 45f, 0f)
                && rightWrist.transform.localRotation == Quaternion.Euler(0f, 0f, -60f))
                step = 1;
        }
    }
}
