using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Player targetPlayer;
    public Transform pivot;
    public Vector3 CameraPos;

    public float MIN_Y;
    public float MAX_Y;

    float currentX = 0.0f;
    float currentY = 0.0f;
    float sensivityX = 4.0f;
    float sensivityY = 1.0f;

    public LayerMask layer;

    public Transform WatchPlayer = null;
    public Transform tempTarget = null;

    float spotTimer;

    public Transform UICanvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if(Input.GetKeyDown(KeyCode.F9))
        {
            CameraPos.x *= -1;
        }
    }

    public void AttachWatchPlayer(Transform t)
    {
        WatchPlayer = t;
        spotTimer = 0.5f;
    }

    public void DetachWatchPlayer(Transform t)
    {
        WatchPlayer = null;
    }

    public void AttachTempTarget(Transform t)
    {
        tempTarget = t;
        spotTimer = 0.5f;
    }

    public void DetachTempTarget()
    {
        tempTarget = null;
    }

    void LateUpdate()
    {
        if (tempTarget)
        {
            Vector3 t = (tempTarget.position - transform.position) * (8.0f / 10.0f);
            transform.position = Vector3.Lerp(transform.position, t, 0.01f);
            transform.GetChild(0).rotation = Quaternion.identity;
            Camera.main.transform.LookAt(tempTarget);
            return;
        }

        if (tempTarget == null && spotTimer > 0)
            spotTimer -= Time.deltaTime;

        float dontgetinput = 1.0f;

        if (targetPlayer != null)
        {
            if (targetPlayer.isInvOn || targetPlayer.isWorkBenchOn)
            {
                dontgetinput = 0.0f;
            }
        }

        //rotation
        currentX += sensivityX * Input.GetAxis("Mouse X") * dontgetinput;
        currentY += sensivityY * Input.GetAxis("Mouse Y") * dontgetinput;

        Camera.main.transform.localRotation = Quaternion.identity;

        Vector3 eulerAngleAxis = new Vector3();
        eulerAngleAxis.x = -currentY;
        eulerAngleAxis.y = currentX;

        currentX = Mathf.Repeat(currentX, 360);
        currentY = Mathf.Clamp(currentY, MIN_Y, MAX_Y);

        Quaternion newRotation = Quaternion.Slerp(pivot.localRotation, Quaternion.Euler(eulerAngleAxis), Time.deltaTime * 10.0f);
        pivot.localRotation = newRotation;

        //position
        RaycastHit hit;
        if (Physics.SphereCast(pivot.position, 0.1f, Camera.main.transform.position - pivot.position, out hit, Mathf.Abs(CameraPos.z), layer))
        {
            float hitDist = hit.distance;
            Vector3 sphereCastCenter = pivot.position + ((Camera.main.transform.position - pivot.position).normalized * hitDist);
            Camera.main.transform.position = sphereCastCenter;
        }
        else
            Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, CameraPos, Time.deltaTime * 5.0f);

        if (WatchPlayer != null)
        {
            if (spotTimer > 0)
                transform.position = Vector3.Lerp(transform.position, WatchPlayer.transform.position, 0.1f);
            else
                transform.position = WatchPlayer.transform.position;
        }
        else if (targetPlayer != null)
        {
            if (spotTimer > 0)
                transform.position = Vector3.Lerp(transform.position, targetPlayer.transform.position, 0.1f);
            else
                transform.position = targetPlayer.transform.position;
        }
    }
}
