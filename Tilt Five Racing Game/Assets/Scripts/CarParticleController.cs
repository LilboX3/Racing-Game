using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarParticleController : MonoBehaviour
{
    [Header("Car Controller Script to access public data")]
    [SerializeField] private CarController carController;


    [Header("Skid Marks / Tire Marks / Tire Trails")]
    [SerializeField] private GameObject tireMarkTrail;

    [Header("Tire Smoke Particles")]
    [SerializeField] private GameObject smokeParticles;

    private WheelColliders wheelColliders;
    public WheelEmitters wheelEmitters;

    [Header("Tire Slipping Threshhold to start Drifting Emissions")]
    [SerializeField] private float slipAllowance = 0.2f; // default = 0.2f

    // Start is called before the first frame update
    private void Start()
    {
        InitializeEmitters();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateParticles();
    }

    // Instantiate and Prepare the Particle and Trail Emitters on Start()
    private void InitializeEmitters()
    {
        wheelColliders = new WheelColliders();
        wheelColliders.FR_collider = carController.frontRightWheelCollider;
        wheelColliders.FL_collider = carController.frontLeftWheelCollider;
        wheelColliders.RR_collider = carController.rearRightWheelCollider;
        wheelColliders.RL_collider = carController.rearLeftWheelCollider;
        if (smokeParticles)
        {
            wheelEmitters.FR_WheelParticles = Instantiate(smokeParticles, wheelColliders.FR_collider.transform.position - Vector3.up * wheelColliders.FR_collider.radius, Quaternion.identity, wheelColliders.FR_collider.transform)
                .GetComponent<ParticleSystem>();
            wheelEmitters.FL_WheelParticles = Instantiate(smokeParticles, wheelColliders.FL_collider.transform.position - Vector3.up * wheelColliders.FL_collider.radius, Quaternion.identity, wheelColliders.FL_collider.transform)
                .GetComponent<ParticleSystem>();
            wheelEmitters.RR_WheelParticles = Instantiate(smokeParticles, wheelColliders.RR_collider.transform.position - Vector3.up * wheelColliders.RR_collider.radius, Quaternion.identity, wheelColliders.RR_collider.transform)
                .GetComponent<ParticleSystem>();
            wheelEmitters.RL_WheelParticles = Instantiate(smokeParticles, wheelColliders.RL_collider.transform.position - Vector3.up * wheelColliders.RL_collider.radius, Quaternion.identity, wheelColliders.RL_collider.transform)
                .GetComponent<ParticleSystem>();
        }
        if (tireMarkTrail)
        {
            wheelEmitters.FR_WheelTrail = Instantiate(tireMarkTrail, wheelColliders.FR_collider.transform.position - Vector3.up * wheelColliders.FR_collider.radius, Quaternion.identity, wheelColliders.FR_collider.transform)
                .GetComponent<TrailRenderer>();
            wheelEmitters.FL_WheelTrail = Instantiate(tireMarkTrail, wheelColliders.FL_collider.transform.position - Vector3.up * wheelColliders.FL_collider.radius, Quaternion.identity, wheelColliders.FL_collider.transform)
                .GetComponent<TrailRenderer>();
            wheelEmitters.RR_WheelTrail = Instantiate(tireMarkTrail, wheelColliders.RR_collider.transform.position - Vector3.up * wheelColliders.RR_collider.radius, Quaternion.identity, wheelColliders.RR_collider.transform)
                .GetComponent<TrailRenderer>();
            wheelEmitters.RL_WheelTrail = Instantiate(tireMarkTrail, wheelColliders.RL_collider.transform.position - Vector3.up * wheelColliders.RL_collider.radius, Quaternion.identity, wheelColliders.RL_collider.transform)
                .GetComponent<TrailRenderer>();
        }
    }

    private void UpdateParticles()
    {
        WheelHit[] wheelHits = new WheelHit[4];
        wheelColliders.FR_collider.GetGroundHit(out wheelHits[0]);
        wheelColliders.FL_collider.GetGroundHit(out wheelHits[1]);
        wheelColliders.RR_collider.GetGroundHit(out wheelHits[2]);
        wheelColliders.RL_collider.GetGroundHit(out wheelHits[3]);

        if ((Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance))
        {
            wheelEmitters.FR_WheelParticles.Play();
            wheelEmitters.FR_WheelTrail.emitting = true;
        }
        else
        {
            wheelEmitters.FR_WheelParticles.Stop();
            wheelEmitters.FR_WheelTrail.emitting = false;
        }
        if ((Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance))
        {
            wheelEmitters.FL_WheelParticles.Play();
            wheelEmitters.FL_WheelTrail.emitting = true;
        }
        else
        {
            wheelEmitters.FL_WheelParticles.Stop();
            wheelEmitters.FL_WheelTrail.emitting = false;
        }
        if ((Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance))
        {
            wheelEmitters.RR_WheelParticles.Play();
            wheelEmitters.RR_WheelTrail.emitting = true;
        }
        else
        {
            wheelEmitters.RR_WheelParticles.Stop();
            wheelEmitters.RR_WheelTrail.emitting = false;
        }
        if ((Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance))
        {
            wheelEmitters.RL_WheelParticles.Play();
            wheelEmitters.RL_WheelTrail.emitting = true;
        }
        else
        {
            wheelEmitters.RL_WheelParticles.Stop();
            wheelEmitters.RL_WheelTrail.emitting = false;
        }

        // debug
        Debug.Log("Sideways Slip - FR: " + wheelHits[0].sidewaysSlip);
        Debug.Log("Sideways Slip - FL: " + wheelHits[1].sidewaysSlip);
        Debug.Log("Sideways Slip - RR: " + wheelHits[2].sidewaysSlip);
        Debug.Log("Sideways Slip - RL: " + wheelHits[3].sidewaysSlip);
    }

    [System.Serializable]
    public class WheelColliders
    {
        public WheelCollider FR_collider;
        public WheelCollider FL_collider;
        public WheelCollider RR_collider;
        public WheelCollider RL_collider;
    }

    [System.Serializable]
    public class WheelEmitters
    {
        public ParticleSystem FR_WheelParticles;
        public ParticleSystem FL_WheelParticles;
        public ParticleSystem RR_WheelParticles;
        public ParticleSystem RL_WheelParticles;

        public TrailRenderer FR_WheelTrail;
        public TrailRenderer FL_WheelTrail;
        public TrailRenderer RR_WheelTrail;
        public TrailRenderer RL_WheelTrail;
    }

}
