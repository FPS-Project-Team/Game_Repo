﻿using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof (PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f; 

    [SerializeField]
    private float thrusterForce = 1000f;

    [Header("Spring Settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;
    void Start(){
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();

        SetJointSettings(jointSpring);
    }

    void Update (){
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov; 

        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed; 

        motor.Move(_velocity); 

        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        motor.Rotate(_rotation);

        //Calculate camera rotation
        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX =  _xRot * lookSensitivity;

        motor.RotateCamera(_cameraRotationX);


        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton ("Jump")){
            _thrusterForce = Vector3.up * thrusterForce;
            SetJointSettings(0f);
        }else {
            SetJointSettings(jointSpring);
        }

        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring){
        joint.yDrive = new JointDrive {
            positionSpring = _jointSpring, 
            maximumForce = jointMaxForce
        };
    }
}
