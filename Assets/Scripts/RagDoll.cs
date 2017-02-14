using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDoll : MonoBehaviour
{
    Rigidbody[] rigids;
    Vector3[] positions;

    Rigidbody impactTarget;
    Vector3 impact;
    float impactCount;

    void Start()
    {
        impact = Vector3.zero;
        impactTarget = null;
        impactCount = 0;
        rigids = GetComponentsInChildren<Rigidbody>();
        positions = new Vector3[rigids.Length];
        for (int i = 0; i < rigids.Length; i++)
        {
            positions[i] = rigids[i].transform.position;
        }

        RagDollOff();
    }

    public void RagDollOn()
    {
        for (int i = 0; i < rigids.Length; i++)
        {
            rigids[i].isKinematic = false;
        }
    }

    public void RagDollOff()
    {
        for (int i = 0; i < rigids.Length; i++)
        {
            rigids[i].isKinematic = true;
        }
    }

    public void RagDollOffAndPosition()
    {
        RagDollOff();
        for (int i = 0; i < positions.Length; i++)
        {
            rigids[i].transform.position = positions[i];
            rigids[i].velocity = Vector3.zero;
        }
    }

    public void Hit(Vector3 position, Vector3 forward, bool isDie)
    {
        RagDollOn();
        RaycastHit hitinfo;
        Physics.Raycast(position, forward, out hitinfo);
        if (hitinfo.transform.gameObject.CompareTag("Zombie"))
        {
            impactTarget = hitinfo.transform.GetComponent<Rigidbody>();
            impactTarget.transform.parent = null;
            impact = forward * 0.7f;
            impactCount = Time.time + 0.25f;
        }
     //   if (!isDie)
     //       Invoke("RagDollOffAndPosition", 0.25f);
    }

    void Update()
    {
        if (Time.time < impactCount)
        {
            if (impactTarget)
                impactTarget.AddForce(impact, ForceMode.VelocityChange);
        }
    }

}