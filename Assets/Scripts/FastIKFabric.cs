using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FastIKFabric : MonoBehaviour
{
    [SerializeField] private int chainLength = 2;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject pole;
    
    [Header("Solver Parameters")]
    [SerializeField] private int iterations = 10;
    [SerializeField] private float delta = 0.001f;
    [Range(0, 1)]
    [SerializeField] private float snapBackStrength = 1f;

    protected float[] bonesLength;
    protected float completeLength;
    protected Transform[] bones;
    protected Vector3[] positions;
    protected Vector3[] startDirectionSucc;
    protected Quaternion[] startRotationBone;
    protected Quaternion startRotationTarget;
    protected Quaternion startRotationRoot;

    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    private void Init() 
    {
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];
        startDirectionSucc = new Vector3[chainLength + 1];
        startRotationBone = new Quaternion[chainLength + 1];

        startRotationTarget = target.transform.rotation;
        completeLength = 0;

        Transform current = transform;
        for (int i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            startRotationBone[i] = current.rotation;

            if (i == bones.Length - 1)
            {
                startDirectionSucc[i] = target.transform.position - current.position;
            }
            else 
            {
                startDirectionSucc[i] = bones[i + 1].position - current.position;
                bonesLength[i] = startDirectionSucc[i].magnitude;
                completeLength += bonesLength[i];
            }

            current = current.parent;
        }
    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK() 
    {
        if (target == null)
            return;

        if (bonesLength.Length != chainLength)
            Init();

        // Get position
        for (int i = 0; i < bones.Length; i++)
        {
            positions[i] = bones[i].position;
        }

        Quaternion rootRot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        Quaternion rootRotDiff = rootRot * Quaternion.Inverse(startRotationRoot);

        // Calculations
        // Further away than max length
        if ((target.transform.position - bones[0].position).sqrMagnitude >= completeLength * completeLength)
        {
            // Stretch it
            Vector3 direction = (target.transform.position - positions[0]).normalized;

            // Set everything after root
            for (int i = 1; i < positions.Length; i++)
            {
                positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
            }
        }
        // Closer than max length
        else 
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + rootRotDiff * startDirectionSucc[i], snapBackStrength);
            }

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                // Backward algorithm
                for (int i = positions.Length - 1; i > 0; i--)
                {
                    if (i == positions.Length - 1)
                    {
                        // Set leaf bone to target position
                        positions[i] = target.transform.position;
                    }
                    else 
                    {
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i];
                    }
                }

                // Forward algorithm
                for (int i = 1; i < positions.Length; i++)
                {
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * bonesLength[i - 1];
                }

                // Stop calculations if leaf bone is close enough to target
                if ((positions[positions.Length - 1] - target.transform.position).sqrMagnitude < delta * delta)
                {
                    break;
                }
            }

            // Pole calculations
            if (pole != null)
            {
                for (int i = 1; i < positions.Length - 1; i++)
                {
                    Plane plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                    Vector3 projectedPole = plane.ClosestPointOnPlane(pole.transform.position);
                    Vector3 projectedBone = plane.ClosestPointOnPlane(positions[i]);
                    float angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                    positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
                }
            }
        }

        // Set position & rotation
        for (int i = 0; i < bones.Length; i++)
        {
            if (i == positions.Length - 1)
            {
                bones[i].rotation = target.transform.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
            }
            else 
            {
                bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], positions[i + 1] - positions[i]) * startRotationBone[i];
            }
            bones[i].position = positions[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Transform current = this.transform;
        for (int i = 0; i < chainLength && current != null && current.parent != null; i++)
        {
            float scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
}
