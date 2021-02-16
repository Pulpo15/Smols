using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;

    private Rigidbody rb;

    private void Start() {
        rb = transform.GetComponent<Rigidbody>();
    }

    #region Getters
    //Gets the movement Vector
    public void Move(Vector3 _velocity) {
        velocity = _velocity;
    }
    
    //Gets a rotational Vector
    public void Rotate(Vector3 _rotation) {
        rotation = _rotation;
    }

    //Gets a rotational Vector for the camera
    public void RotateCamera(Vector3 _cameraRotation) {
        cameraRotation = _cameraRotation;
    }
    #endregion

    private void FixedUpdate() {
        PerformMovement();
        PerformRotation();
    }

    #region Performers
    //Perform movement based on velocity variable
    void PerformMovement() {
        if (velocity != Vector3.zero) {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }
    
    //Perform rotation
    private void PerformRotation() {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
            cam.transform.Rotate(-cameraRotation);
    }
    #endregion
}
