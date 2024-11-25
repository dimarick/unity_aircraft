using System;
using UnityEditor;
using UnityEngine;

public class Aircraft : MonoBehaviour
{
    public Transform cam;
    public Transform crosshair;
    public Transform ground;
    public Transform missiles;
    public Transform missileTemplate;

    private Missile[] missilesRb = new Missile[2];
    private Rigidbody rb;
    private float thrust = 0F;
    private float thrustStep = 0.03F;
    private float thrustUnStep = 0.2F;
    private float maxThrust = 5F;
    
    private float yaw = 0F;
    private float yawStep = 0.01F;
    private float yawUnStep = 0.1F;
    private float maxYaw = 1F;
    
    private float pitch = 0F;
    private float pitchStep = 0.01F;
    private float pitchUnStep = 0.1F;
    private float maxPitch = 1F;
    
    private float roll = 0F;
    private float rollStep = 0.01F;
    private float rollUnStep = 0.1F;
    private float maxRoll = 1F;
    
    private float stopRotation = 0F;
    private float stopRotationStep = 0.05F;
    private float maxStopRotation = 5F;
    
    private bool broken = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = 1000F;
        rb.maxAngularVelocity = 0.5F;
        
        CreateMissiles();
    }

    private void CreateMissiles()
    {
        var leftMissile = Instantiate(missileTemplate, missiles);
        var rightMissile = Instantiate(missileTemplate, missiles);
        leftMissile.Translate(-2, 0, 0);
        rightMissile.Translate(2, 0, 0);
        missilesRb[0] = leftMissile.GetComponent<Missile>();
        missilesRb[1] = rightMissile.GetComponent<Missile>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(ground.tag) && !broken && rb.linearVelocity.magnitude > 2)
        {
            DestroyAircraft();
        }
    }

    void FixedUpdate()
    {
        if (broken)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            thrust = Math.Min(maxThrust, thrust + thrustStep);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            thrust = Math.Max(-maxThrust, thrust - thrustStep);
        }
        else
        {
            thrust = thrust > 0 ? Math.Max(0, thrust - thrustUnStep) : Math.Min(0, thrust + thrustUnStep);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            yaw = Math.Max(-maxYaw, yaw - yawStep);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            yaw = Math.Min(maxYaw, yaw + yawStep);
        }
        else
        {
            yaw = yaw > 0 ? Math.Max(0, yaw - yawUnStep) : Math.Min(0, yaw + yawUnStep);
        }

        if (Input.GetKey(KeyCode.Keypad8))
        {
            pitch = Math.Min(maxPitch, pitch + pitchStep);
        } 
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            pitch = Math.Max(-maxPitch, pitch - pitchStep);
        }
        else
        {
            pitch = pitch > 0 ? Math.Max(0, pitch - pitchUnStep) : Math.Min(0, pitch + pitchUnStep);
        }

        if (Input.GetKey(KeyCode.Keypad5))
        {
            stopRotation = Math.Min(maxStopRotation, stopRotation + stopRotationStep);
        }
        else
        {
            stopRotation = 0;
        }

        if (Input.GetKey(KeyCode.Keypad4))
        {
            roll = Math.Min(maxRoll, roll + rollStep);
        } 
        else if (Input.GetKey(KeyCode.Keypad6))
        {
            roll = Math.Max(-maxRoll, roll - rollStep);
        }
        else
        {
            roll = roll > 0 ? Math.Max(0, roll - rollUnStep) : Math.Min(0, roll + pitchUnStep);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireMissile();
        }

        if (thrust != 0)
        {
            rb.AddForce(transform.forward * thrust, ForceMode.VelocityChange);
        }

        if (yaw != 0)
        {
            rb.AddTorque(transform.up * yaw, ForceMode.VelocityChange);
        }

        if (pitch != 0)
        {
            rb.AddTorque(transform.right * pitch, ForceMode.VelocityChange);
        }

        if (roll != 0)
        {
            rb.AddTorque(transform.forward * roll, ForceMode.VelocityChange);
        }

        rb.angularVelocity *= 1 - 0.05F;
        rb.linearVelocity *=  1 - 0.01F;

        cam.position = transform.position;
        cam.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        cam.Translate(new Vector3(0, 10, -30), cam);

        crosshair.position = cam.position;
        crosshair.rotation = cam.rotation;
        crosshair.Translate(new Vector3(0, 0, 20), cam);

        RaycastHit hit;
        var point = transform.forward * 1000;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000))
        {
            point = hit.point;
        }
        var positionY = point.y - (point.y - cam.position.y) * (point.x - crosshair.position.x) /  (point.x - cam.position.x);

        if (float.IsNaN(positionY))
        {
            positionY = cam.position.y;
        }

        crosshair.position = new Vector3(crosshair.position.x, positionY, crosshair.position.z);
        Debug.Log("Thrust " + thrust + ", Yaw " + yaw + ", Pitch " + pitch + ", Roll " + roll + ", Vel " + rb.linearVelocity + " Target" + point);

    }

    private void FireMissile()
    {
        var launches = 0;
        foreach (var missile in missilesRb)
        {
            if (missile.launched)
            {
                continue;
            }
            
            missile.LaunchMissile();
            launches++;
            
            break;
        }

        if (launches == 0)
        {
            CreateMissiles();
        }
    }

    private void DestroyAircraft()
    {
        broken = true;
        rb.useGravity = true;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        var position = transform.position;
        var velocity = rb.linearVelocity;

        if (velocity.magnitude > 0)
        {
            Handles.color = Color.red;
            Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(velocity), rb.linearVelocity.magnitude, EventType.Repaint);
        }
        
        Handles.color = Color.blue;
        Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(transform.forward), 10.0F, EventType.Repaint);

        Handles.color = Color.green;
        Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(transform.up), 10.0F, EventType.Repaint);
    }

    /**
     * Возведение в квадрат с сохранением знака
     */
    private float SignedSquare(float x)
    {
        return Math.Sign(x) * x * x;
    }
}
