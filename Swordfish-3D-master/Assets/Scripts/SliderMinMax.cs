using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliderMinMax : MonoBehaviour
{
	public Text Min, Max, Cur;
	private Slider _slider;

	private Coroutine one, two;

	private void Awake()
	{
		_slider = GetComponentInParent<Slider>();
	}

	private void OnEnable()
	{
		WaiterVoid();
	}
	
	private void WaiterVoid()
	{
		Min.text = _slider.minValue.ToString("F");
		Max.text = _slider.maxValue.ToString("F");
		Cur.text = _slider.value.ToString("F");
	}

	private void CurUpdate()
	{
		if (isActiveAndEnabled)
		{
			Cur.text = _slider.value.ToString("F");
		}
	}

	private void Update()
	{
		CurUpdate();
	}
}
