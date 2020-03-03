using UnityEngine;

[RequireComponent(typeof(Actor))]
public class InteractiveIK : MonoBehaviour {
    public Transform Root = null;
    public Transform Tip = null;
    public Transform Target = null;
    public Transform Chest4 = null;
    public Transform Chest = null;

    public bool SeedZeroPose = true;

    private Actor Actor = null;
    private Matrix4x4[] ZeroPose = null;

    void Start() {
        Actor = GetComponent<Actor>();
        ZeroPose = Actor.GetBoneTransformations();
    }

    void Update() {
        if(Root == null || Tip == null || Target == null) {
            Debug.Log("Some inspector variable has not been assigned and is still null.");
            return;
        }
        if(SeedZeroPose) {
            Actor.SetBoneTransformations(ZeroPose);
        }
        CCD.Solve(Root, Tip, Target.position, Target.rotation, Chest, Chest4);
    }
}
