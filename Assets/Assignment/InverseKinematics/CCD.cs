using System.Collections.Generic;
using UnityEngine;

public class CCD
{

    public static void Solve(Transform root, Transform tip, Vector3 targetPosition, Quaternion targetRotation, Transform chest, Transform chest4)
    {
        // Chain from collar to wrist
        Transform[] chain = GetChain(root, tip);
        if (chain.Length == 0)
        {
            Debug.Log("Given root and tip are not valid.");
            return;
        }

        float sqrDistance = 0f;
        int iterationCount = 0;

        // Weights for updating position and rotation
        float[] positionWeights = { 0.2f, 0.7f, 0.6f, 0.5f };
        float[] rotationWeights = { 0.1f, 0.5f, 0.5f, 0.8f };

        // CCD
        do
        {
            for (int i = chain.Length - 1; i >= 0; i--)
            {
                if (i == (chain.Length - 1))
                    chain[i].rotation = Quaternion.Slerp(chain[i].rotation, targetRotation, rotationWeights[i]);
                else
                {
                    chain[i].rotation = Quaternion.Slerp(chain[i].rotation, targetRotation, rotationWeights[i]);

                    Vector3 boneToEffector = tip.position - chain[i].position;
                    Vector3 boneToGoal = targetPosition - chain[i].position;
                    Quaternion newRotation = Quaternion.FromToRotation(boneToEffector, boneToGoal) * chain[i].rotation;
                    chain[i].rotation = Quaternion.Slerp(chain[i].rotation, newRotation, positionWeights[i]);
                }

                // Calculating error
                sqrDistance = (tip.position - targetPosition).sqrMagnitude;
                if (sqrDistance <= 0.01f)
                    break;
            }
            iterationCount++;
        } while (iterationCount < 10);

        // Apply CCD on chest bones if the target could not be reached by movement of the arm
        if (Vector3.Distance(tip.position, targetPosition) > 0.1f)
        {
            float[] chestWeights = { 0.3f, 0.3f, 0.1f, 0.5f };

            // Chain from chest to chest4
            Transform[] chestChain = GetChain(chest.transform, chest4.transform);
            if (chestChain.Length == 0)
            {
                Debug.Log("Given root and tip are not valid.");
                return;
            }

            iterationCount = 0;

            // CCD
            do
            {
                for (int i = chestChain.Length - 1; i >= 0; i--)
                {
                    Vector3 boneToEffector = tip.position - chestChain[i].position;
                    Vector3 boneToGoal = targetPosition - chestChain[i].position;
                    Quaternion newRotation = Quaternion.FromToRotation(boneToEffector, boneToGoal) * chestChain[i].rotation;
                    chestChain[i].rotation = Quaternion.Slerp(chestChain[i].rotation, newRotation, chestWeights[i]);

                    sqrDistance = (tip.position - targetPosition).sqrMagnitude;
                    if (sqrDistance <= 0.01f)
                        break;
                }
                iterationCount++;
            } while (iterationCount < 10 && sqrDistance > 0.01f);
        }
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

}
