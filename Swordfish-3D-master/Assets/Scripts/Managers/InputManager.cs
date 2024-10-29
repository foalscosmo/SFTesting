using System.Collections.Generic;
using Managers;
using SW_Tutorial;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    [HideInInspector]
    public bool ButtonControls = true;

   // public Toggle UseJoystickToggle;
        
    private float _horizontalInput;
    private float _verticalInput;
    private float _arrowDistance;
    
    [HideInInspector]public FloatingJoystick _floatingJoystick;

    private void Awake()
    {
        Instance = this;
        _floatingJoystick = FindObjectOfType<FloatingJoystick>();
        OnInputtypeChange(true);
        //OnInputtypeChange(false);
        //UseJoystickToggle.onValueChanged.AddListener(OnInputtypeChange);
    }

    private void Start()
    {
        _isTutorial = TutorialManager.instance != null;
        if (_isTutorial)
            _tutorialSW = GameObject.FindObjectOfType<TutorialSWController>();
    }

    public bool JoystickIsActive()
    {
        return _floatingJoystick.joystickIsActive;
    }

    private void OnInputtypeChange(bool arg)
    {
        /*ButtonControls = !arg;
        _floatingJoystick.OnPointerUp(new PointerEventData());
        _floatingJoystick.enabled = arg;
        _floatingJoystick.GetComponent<Image>().raycastTarget = arg;*/
    }

    public void SetInputs(Vector3 inputs, float length)
    {
        _horizontalInput = inputs.x;
        _verticalInput = inputs.z;
        _arrowDistance = length;
    }

    private bool _isTutorial = false;
    private TutorialSWController _tutorialSW;
    
    private void Update()
    {
        if (_isTutorial)
        {
            _tutorialSW.SetInput(_horizontalInput,_verticalInput,false,_arrowDistance);
            return;
        }
        if (GameManager.Instance.Player)
        {
            GameManager.Instance.Player.SetInput(_horizontalInput,_verticalInput,false,_arrowDistance);
        }
    }

    #region Invisible Button Controls

    public void OnSteerRight(bool isDown)
    {
    }
	
    public void OnSteerLeft(bool isDown)
    {
    }

    #endregion

    public void Boost()
    {
        if (!GameManager.Instance.Player._boost)
        {
            GameManager.Instance.Player.Boost();
        }
    }

}
