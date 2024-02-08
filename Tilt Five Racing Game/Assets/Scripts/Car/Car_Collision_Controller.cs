using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Collision_Controller : MonoBehaviour
{
    private Car_SFX_Controller sfxController;

    public enum CollisionType
    {
        Front,
        Side
    }
    public CollisionType collisionType;

    // =====/////===== Start of: Unity Lifecycle Functions =====/////=====
    
    void Start()
    {
        // Try to get the Car_SFX_Controller component
        sfxController = GetComponent<Car_SFX_Controller>();

        // If Car_SFX_Controller is not found, log an error
        if (sfxController == null)
        {
            Debug.LogError("Car_Collision_Controller on " + gameObject.name + ": Car_SFX_Controller script not found!");
        }
    }
    
    // =====/////===== End of: Unity Lifecycle Functions =====/////=====

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Obstacle"))
        {
            // Determine collision type based on the relative position of the collision point
            Vector3 relativeCollisionPoint = transform.InverseTransformPoint(collision.contacts[0].point);

            if (relativeCollisionPoint.z > 0.0f)
            {
                // Collision happened towards the front of the car
                collisionType = CollisionType.Front;
            }
            else
            {
                // Collision happened towards the side of the car
                collisionType = CollisionType.Side;
            }

            // Play the assigned sound
            sfxController.PlayCollisionSound(collisionType);
        }
        else
        {
            // Log the collision for debugging purposes
            Debug.Log("Collision detected with untagged object: " + collision.gameObject.name);
            //----------
// TODO: for now we're not discerning, later we should discern differently tagged objects, like metal guard rails sound differently compared to concrete walls.
//       We might want to handle it through the car, since we don't want to script every single bolt in the scene.
//       Making sure the SFX for the scenery item itself plays in the right spot might prove difficult that way, we'll have to figure something out.
//       Maybe with a temporary AudioSource?

            // Determine collision type based on the relative position of the collision point
            Vector3 relativeCollisionPoint = transform.InverseTransformPoint(collision.contacts[0].point);

            if (relativeCollisionPoint.z > 0.0f)
            {
                // Collision happened towards the front of the car
                collisionType = CollisionType.Front;
            }
            else
            {
                // Collision happened towards the side of the car
                collisionType = CollisionType.Side;
            }
            //----------
            // Play a sound for untagged objects
            sfxController.PlayCollisionSound(collisionType);
        }
    }
}
