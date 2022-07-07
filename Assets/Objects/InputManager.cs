using UnityEngine;

public class InputManager : MonoBehaviour
{
	[field: SerializeField] Camera Camera { get; set; }

	public bool IsBusy { get; set; }

	public Vector3 MousePosition
	{
		get => Camera.ScreenToWorldPoint(Input.mousePosition);
	}
}