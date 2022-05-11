using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private float jointSpace = 0.25f;
    private int segmentLength = 35;
    private float lineWidth = 0.3f;

    private LineRenderer lineRenderer;
    private List<Joint> joints = new List<Joint>();
    
    public Transform startAnchor;
    public Vector3 endAnchor;
    public Transform end_anchor;

    public struct Joint
    {
        public Vector3 positionAfter;
        public Vector3 positionBefore;

        public Joint(Vector3 position)
        {
            this.positionAfter = position;
            this.positionBefore = position;
        }
    }

    // Use this for initialization
    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = startAnchor.position;

        for (int i = 0; i < segmentLength; i++)
        {
            this.joints.Add(new Joint(ropeStartPoint));
            ropeStartPoint.y -= jointSpace;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        Simulate();
    }

    private void Simulate()
    {
        // SIMULATION
        Vector3 gravity = new Vector3(0f, -1f,0f);

        for (int i = 1; i < this.segmentLength; i++)
        {
            Joint firstJoint = joints[i];
            Vector3 velocity = firstJoint.positionAfter - firstJoint.positionBefore;
            firstJoint.positionBefore = firstJoint.positionAfter;
            firstJoint.positionAfter += velocity;
            firstJoint.positionAfter += gravity * Time.fixedDeltaTime;
            joints[i] = firstJoint;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        //Constrant to First Point 
        Joint firstJoint = joints[0];
        firstJoint.positionAfter = startAnchor.position;
        this.joints[0] = firstJoint;

        //Constrant to Second Point 
        Joint endJoint = joints[joints.Count - 1];
        //endAnchor = endJoint.positionAfter;
        endJoint.positionAfter = end_anchor.position;
        joints[joints.Count - 1] = endJoint;

        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            Joint first= joints[i];
            Joint second = joints[i + 1];

            float dist = (first.positionAfter - second.positionAfter).magnitude;
            float error = Mathf.Abs(dist - jointSpace);

            Vector3 dir = Vector2.zero;

            if (dist > jointSpace)
            {
                dir = (first.positionAfter - second.positionAfter).normalized;
            }
            else if (dist < jointSpace)
            {
                dir = (second.positionAfter - first.positionAfter).normalized;
            }
            Vector3 amount = dir * error;
            if (i != 0)
            {
                first.positionAfter -= amount * 0.5f;
                joints[i] = first;
                second.positionAfter += amount * 0.5f;
                joints[i + 1] = second;
            }
            else
            {
                second.positionAfter += amount;
                joints[i + 1] = second;
            }
        }
    }

    private void DrawRope()
    { 
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[segmentLength];
        for (int i = 0; i < segmentLength; i++)
        {
            ropePositions[i] = joints[i].positionAfter;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }
}
