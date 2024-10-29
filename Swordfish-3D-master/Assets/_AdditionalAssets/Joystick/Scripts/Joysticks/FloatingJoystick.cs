using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloatingJoystick : Joystick
{
    //private JoysticButtonController jbc;

    public bool followCenter = false;
    private float handleLimit = 180;

    private Vector2 joystickCenter = Vector2.zero;
    private Vector2 startPos = Vector2.zero;
    private bool pressed = false;
    
    [HideInInspector]public bool joystickIsActive = false;

    private Image[] _images;

    private Camera _mainCamera;


    private bool trueTouch = false;

    public bool disableImage = true;

    private void Awake()
    {
        //jbc = this.GetComponent<JoysticButtonController>();
        _mainCamera = Camera.main;
    }

    void Start()
    {
        startPos = background.position;
        _images = this.transform.GetChild(0).GetComponentsInChildren<Image>();

        
        for (var i = 0; i < _images.Length; i++)
        {
            if (disableImage)
            {
                var color = _images[i].color;
                color.a = 0;
                _images[i].color = color;
            }
            
            _images[i].enabled = false;
        }
        //pressed = false;
        //jbc.SetColor(false);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickCenter;

        var mag = direction.magnitude;

        inputVector = direction / (background.sizeDelta.x / 4f);
        inputVector = Vector2.ClampMagnitude(inputVector, 1);
        handleLimit = background.lossyScale.x * background.sizeDelta.x / 2f;

        ClampJoystick();

        if (followCenter)
        {
            handle.position = eventData.position;
            background.position = joystickCenter + Direction.normalized * Mathf.Clamp(((eventData.position - joystickCenter).magnitude - handleLimit), 0, 10000);
            joystickCenter = background.position;
        }
        else
        {
            direction = Vector2.ClampMagnitude(direction, handleLimit / 2);
            handle.position = joystickCenter + direction;
        }

        #region Screen To XZ world space
        Vector3 tempVector = new Vector3(_mainCamera.transform.forward.x, 0, _mainCamera.transform.forward.z);
        Vector3 joystickWorldSpace = new Vector3(handle.position.x - joystickCenter.x, 0, handle.position.y - joystickCenter.y).normalized;
        float cameraAngleY = Vector3.SignedAngle(tempVector, Vector3.forward, Vector3.up);
        Vector3 rotatedVector = Quaternion.AngleAxis(-cameraAngleY, Vector3.up) * joystickWorldSpace;

        InputManager.Instance.SetInputs(rotatedVector, mag);
        /*if (rotatedVector.magnitude > 0.1)
        {
        }*/
        
        /*if (inputType == InputType.InputA)
        {
            InputManager.instance.vectorDirectionA = rotatedVector;
            InputManager.instance.inputMagnitudeA = inputVector.magnitude;
        }
        else if (inputType == InputType.InputB)
        {
            InputManager.instance.vectorDirectionB = rotatedVector;
            InputManager.instance.inputMagnitudeB = inputVector.magnitude;
        }
        else if (inputType == InputType.InputC)
        {
            InputManager.instance.vectorDirectionC = rotatedVector;
            InputManager.instance.inputMagnitudeC = inputVector.magnitude;
        }*/
        #endregion
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        trueTouch = true;
        background.position = eventData.position;
        handle.anchoredPosition = Vector2.zero;
        joystickCenter = eventData.position;
        joystickIsActive = true;
        
        for (var i = 0; i < _images.Length; i++)
        {
            _images[i].enabled = true;
        }

        //jbc.SetColor(true);

        /*if (inputType == InputType.InputA)
        {
            InputManager.instance.vectorDirectionA = Vector3.zero;
            InputManager.instance.inputMagnitudeA = 0;
        }
        else if (inputType == InputType.InputB)
        {
            InputManager.instance.vectorDirectionB = Vector3.zero;
            InputManager.instance.inputMagnitudeB = 0;
            InputManager.instance.pressedB = true;
        }
        else if (inputType == InputType.InputC)
        {
            InputManager.instance.vectorDirectionC = Vector3.zero;
            InputManager.instance.inputMagnitudeC = 0;
            InputManager.instance.pressedC = true;
        }*/
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        trueTouch = false;
        background.position = startPos;
        handle.position = startPos;
        inputVector = Vector2.zero;
        joystickIsActive = false;
        
        for (var i = 0; i < _images.Length; i++)
        {
            _images[i].enabled = false;
        }
        
        InputManager.Instance.SetInputs(Vector3.zero, 0);

        //jbc.SetColor(false);

        /*if (inputType == InputType.InputA)
        {
            InputManager.instance.vectorDirectionA = Vector3.zero;
            InputManager.instance.inputMagnitudeA = 0;
        }
        else if (inputType == InputType.InputB)
        {
            InputManager.instance.vectorDirectionB = Vector3.zero;
            InputManager.instance.inputMagnitudeB = 0;
            InputManager.instance.pressedB = false;
        }
        else if (inputType == InputType.InputC)
        {
            InputManager.instance.vectorDirectionC = Vector3.zero;
            InputManager.instance.inputMagnitudeC = 0;
            InputManager.instance.pressedC = false;
        }*/
    }
}