using UnityEngine;
using System.Collections;

public class flock : MonoBehaviour {
    // Dumb comment to test github connection.
    public float speed = 0.5f;
    float rotationSpeed = 4.0f;
    float minSpeed = 0.2f;
    float maxSpeed = 1.0f;
    Vector3 avgHeading;
    Vector3 avgPosition;
    float neighbourDistance = 10.0f;
    public Vector3 newGoalPos;

    bool turning = false;

	// Use this for initialization
	void Start () 
    {
        SetFishSpeed(Random.Range(minSpeed, maxSpeed));
	}
	
    void OnTriggerEnter(Collider other)
    {
        if (!turning)
        {
            newGoalPos = this.transform.position - other.gameObject.transform.position;
        }

        turning = true;
    }

    void OnTriggerExit(Collider other)
    {
        turning = false;
    }

	// Update is called once per frame
	void Update () 
    {
        if (turning)
        {
            Vector3 direction = newGoalPos - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

            SetFishSpeed(Random.Range(minSpeed, maxSpeed));
        }
        else
        {
            if (Random.Range(0, 10) < 1)
            {
                ApplyRules();
            }
        }

        transform.Translate(0, 0, Time.deltaTime * speed);
	}

    void ApplyRules()
    {
        GameObject[] gos;
        gos = globalFlock.allFish;

        Vector3 vcenter = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;

        Vector3 goalPos = globalFlock.goalPos;

        float dist;

        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    vcenter += go.transform.position;
                    groupSize++;

                    if (dist < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    flock anotherFlock = go.GetComponent<flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vcenter = vcenter/groupSize + (goalPos - this.transform.position);
            speed = gSpeed/groupSize;
            SetFishSpeed(speed);

            Vector3 direction = (vcenter + vavoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void SetFishSpeed(float speed)
    {
        this.GetComponent<Animator>().speed = speed;
    }
}
