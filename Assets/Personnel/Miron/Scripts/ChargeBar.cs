using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ChargeBar : MonoBehaviour
{

    [SerializeField]
    UnityEvent onStart = new UnityEvent();

    [SerializeField]
    UnityEvent<float> onEnd = new UnityEvent<float>();

    [SerializeField]
    UnityEvent onCancel = new UnityEvent();


    [SerializeField]
    GameObject frontBar;

    [SerializeField]
    private float min = 0f;

    [SerializeField]
    private float max = 300f;

    [SerializeField]
    private float ramp = 100f;

    private float current = 0f;
    private bool hold = false;

    private void SetActiveChildren(bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }
    private void Start()
    {
        SetActiveChildren(false);
    }

    void FixedUpdate()
    {
        if (hold)
        {
            current = Mathf.Min(current + ramp * Time.deltaTime, max);
        }
        frontBar.transform.localScale = new Vector3((current - min) / (max - min), 1, 1);
    }

    public void OnAction(InputAction.CallbackContext context)
    {
		switch (context.phase)
		{
			case InputActionPhase.Performed:
				current = min;
				hold = true;
                SetActiveChildren(true);
                onStart.Invoke();
                break;
			case InputActionPhase.Canceled:
				if (hold)
                {
					hold = false;
                    SetActiveChildren(false);
                    onEnd.Invoke(current);
                }
                else
                {
                    onCancel.Invoke();
                }
                break;
			default:
				break;
		}
	}

}
