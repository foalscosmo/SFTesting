using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace CameraControllers
{
	public class BackCamera : MonoBehaviour
	{
		public Transform 		CameraTarget;
	
		[Header("BackView")]
		public Vector3 	BackViewOffset;

		public float 	BackViewCameraLookYOffset   = 3;
		public float 	BackViewCameraLookSpeed 	= 10;
		public float 	BackViewCameraFollowSpeed 	= 10;
		
		[Header("Sliders")]
		public Slider ZoomSlider;
		public Slider LookSpeedSlider;
		public Slider FollowSpeedSlider;
		public Slider LookYOffsetSlider;

		public GameObject SlidersHolder;

		private Camera _camera;
		private float  _defaultCameraFieldOfView;
		private float  _defaultBackViewCameraLookY;

		private void OnEnable()
		{
			Initialize();
		}

		private void OnDisable()
		{
			SlidersHolder.SetActive(false);
		}

		private void OnDestroy()
		{
			DeInitialize();
		}

		private void DeInitialize()
		{
			if(ZoomSlider)ZoomSlider.onValueChanged  		.RemoveAllListeners();
			if(LookSpeedSlider)LookSpeedSlider.onValueChanged.RemoveAllListeners();
			if(FollowSpeedSlider)FollowSpeedSlider.onValueChanged.RemoveAllListeners();
			if(LookYOffsetSlider)LookYOffsetSlider.onValueChanged.RemoveAllListeners();
		}

		private void Initialize()
		{
			if (!_camera)
			{
				_camera = GetComponentInChildren<Camera>();
				_defaultCameraFieldOfView = _camera.fieldOfView;
				_defaultBackViewCameraLookY = BackViewOffset.y;
				//BackViewOffset = _camera.transform.position - CameraTarget.transform.position;
				//BackViewOffset.x = 0;
			}

			SlidersHolder.SetActive(true);
		}

		public void InitDebug(float lookSp,float followSpeed,float yOffset, float zoomVal)
		{
			var active = SlidersHolder.activeSelf;
			SlidersHolder.SetActive(true);
			
			DeInitialize();
			
			LookSpeedSlider.value   = BackViewCameraLookSpeed 	= lookSp;
			FollowSpeedSlider.value = BackViewCameraFollowSpeed = followSpeed;
			LookYOffsetSlider.value = BackViewOffset.y 			= yOffset;
			_camera.fieldOfView 		= zoomVal;
			ZoomSlider.value = Mathf.Abs(zoomVal - _defaultCameraFieldOfView);
		
			ZoomSlider.onValueChanged  		.AddListener(ChangeCameraZoom);
			LookSpeedSlider.onValueChanged  .AddListener(ChangeLookSpeed);
			FollowSpeedSlider.onValueChanged.AddListener(ChangeFollowSpeed);
			LookYOffsetSlider.onValueChanged.AddListener(ChangeYOffset);
			
			SlidersHolder.SetActive(active);
		}

		private void LookAtTarget()
		{
			var lookDirection = CameraTarget.position - transform.position;
			lookDirection.y += BackViewCameraLookYOffset;
			var rot = Quaternion.LookRotation(lookDirection, Vector3.up);
			transform.rotation = Quaternion.Lerp(transform.rotation, rot, BackViewCameraLookSpeed * Time.deltaTime);
		}

		private void MoveToTarget()
		{
			var targetPos = CameraTarget.position +
			                CameraTarget.forward * BackViewOffset.z +
			                CameraTarget.right * BackViewOffset.x +
			                CameraTarget.up * BackViewOffset.y;
			transform.position = Vector3.Lerp(transform.position, targetPos, BackViewCameraFollowSpeed * Time.deltaTime);
		}

		public void ChangeCameraZoom(float value)
		{
			_camera.fieldOfView = _defaultCameraFieldOfView + value;
		}
	
		public void ChangeLookSpeed(float value)
		{
			BackViewCameraLookSpeed = value;
		}
	
		public void ChangeFollowSpeed(float value)
		{
			BackViewCameraFollowSpeed = value;
		}
	
		public void ChangeYOffset(float value)
		{
			BackViewOffset.y = _defaultBackViewCameraLookY + value;
		}

		private void FixedUpdate()
		{
			if(!GameManager.Instance.theGameIsStarted) return;
			if (GameManager.Instance.Player.transform)
				CameraTarget = GameManager.Instance.Player.transform;
			LookAtTarget();
			MoveToTarget();
		}
	}
}
