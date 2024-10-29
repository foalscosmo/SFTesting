using System;
using System.Collections;
using DG.Tweening;
using Managers;
using SW_Tutorial;
using UnityEngine;
using UnityEngine.UI;

namespace CameraControllers
{
    public enum CameraFollowType
    {
        TopView,
        BackView
    }
    
    public class CameraTopDown : MonoBehaviour
    {
        public static CameraTopDown Instance;
        
        public GameObject CameraTarget;

        private Vector3 _currentPos;
        public Camera  _camera;
        public float   _defaultCameraFieldOfView;

        //[Range(0.01f, 1f)]
        public float CameraFlow;

        [Range(0.00f, 100f)]
        public float SpeedOffset;
    
        [Header("Sliders")]
        public Slider ZoomSlider;
        public Slider FlowSlider;
        public Slider OffsetSlider;
        public Slider horizontalRotationOfCamera;
        public Slider verticalRotationOfCamera;
    
        public GameObject SlidersHolder;

        private Vector3 _currentForward;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            ChangeCameraZoom(0);
        }

        private void Initialize()
        {
            if (!_camera)
            {
                _camera                   = GetComponentInChildren<Camera>();
                _defaultCameraFieldOfView = 60f;
                ZoomSlider.value = 0f;
            }
        
            SlidersHolder.SetActive(true);
        }

        private bool isZoomedIn = false;
        public void IncreaseDecreaseCameraDistance(bool increase)
        {
            if (increase == isZoomedIn)
                return;
            isZoomedIn = increase;
            var finalpos = increase
                ? 10
                : -10;
            
            _camera.DOFieldOfView(_camera.fieldOfView + finalpos, 10);
        }

        public float GetZoomLevel()
        {
            return _camera.fieldOfView;
        }

        public void DoZoom(float newZoom, float duration)
        {
            if(_camera.fieldOfView >= 100) return;
            DOTween.To(() => _camera.fieldOfView, x => _camera.fieldOfView = x, newZoom, duration);
        }
        public void InitDebug(float flowSpeed,float yOffset, float zoomVal, bool isOrtho, float orthoZoom, float vertical)
        {
            var active = SlidersHolder.activeSelf;
            SlidersHolder.SetActive(true);
            Deinitialize();
            if (isOrtho)
            {
                if (!_camera.orthographic)
                {
                    ChangeToOrthographic();
                }
                ZoomSlider.value = _defaultCameraFieldOfView = orthoZoom - 5;
            }
            else
            {
                ZoomSlider.value = Mathf.Abs(zoomVal - _defaultCameraFieldOfView);
            }

            
            ChangeCameraZoom(ZoomSlider.value);
            verticalRotationOfCamera.value = vertical;
            ChangeCameraVerticalRot(vertical);
            ZoomSlider.onValueChanged  .AddListener(ChangeCameraZoom);
            verticalRotationOfCamera.onValueChanged.AddListener(ChangeCameraVerticalRot);
            SlidersHolder.SetActive(active);
            ZoomSlider.value = zoomVal;
            ChangeCameraZoom(zoomVal);
        }
        
    
        private void ChangeCameraHorizontalRot(float value)
        {
            SpeedOffset = value;
        }
        
        private void ChangeCameraVerticalRot(float value)
        {
            var rot = transform.eulerAngles;
            rot.x = value;
            transform.eulerAngles = rot;
        }
        
        private void OnDestroy()
        {
            Deinitialize();
        }

        private void Deinitialize()
        {
            if(ZoomSlider)ZoomSlider.onValueChanged  .RemoveAllListeners();
            if(FlowSlider) FlowSlider.onValueChanged  .RemoveAllListeners();
            if(OffsetSlider) OffsetSlider.onValueChanged.RemoveAllListeners();
        }

        private void OnDisable()
        {
            if (SlidersHolder != null)
            {
                SlidersHolder.SetActive(false);
            }
        }
    
        private void OnEnable()
        {
            Initialize();
            _camera.fieldOfView = _defaultCameraFieldOfView;
        }
        
        private void LateUpdate()
        {
            if (TutorialManager.instance == null)
            {
                if(!GameManager.Instance.theGameIsStarted)
                    return;
                if (GameManager.Instance.Player)
                    CameraTarget = GameManager.Instance.Player.gameObject;
            }
     
            _currentPos = CameraTarget.transform.position;
            _currentPos.y = 0.0f;
            this.transform.position = _currentPos;
        }

        public void ChangeCameraZoom(float value)
        {
            if  (_camera.orthographic)  _camera.orthographicSize = 5 + value; 
            else _camera.fieldOfView =  _defaultCameraFieldOfView + value;
        }
    
        private void ChangeCameraFlow(float value)
        {
            CameraFlow = value;
        }
    
        private void ChangeCameraOffset(float value)
        {
            SpeedOffset = value;
        }

        public void ChangeToOrthographic()
        {
            _camera.orthographic = !_camera.orthographic;
            //_camera.nearClipPlane = _camera.orthographic ? -2 : 0.01f;
        } 
    }
}