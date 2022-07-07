using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Card : MonoBehaviour
{
	[field: SerializeField] string  TitleText { get; set; }
	[field: SerializeField] string  BodyText  { get; set; }
	[field: SerializeField] Texture CardArt   { get; set; }
	[field: SerializeField] Texture CardBack  { get; set; }

	[field: SerializeField] float          DrawAnimationDuration       { get; set; }
	[field: SerializeField] AnimationCurve DrawAnimationCurve          { get; set; }
	[field: SerializeField] float          DiscardAnimationDuration    { get; set; }
	[field: SerializeField] AnimationCurve DiscardAnimationCurve       { get; set; }
	[field: SerializeField] float          MouseEnterAnimationDuration { get; set; }
	[field: SerializeField] AnimationCurve MouseEnterAnimationCurve    { get; set; }
	[field: SerializeField] float          MouseExitAnimationDuration  { get; set; }
	[field: SerializeField] AnimationCurve MouseExitAnimationCurve     { get; set; }
	[field: SerializeField] Vector3        DrawPileOffset              { get; set; }
	[field: SerializeField] Vector3        DiscardPileOffset           { get; set; }
	[field: SerializeField] Vector3        MouseOverOffset             { get; set; }

	[field: SerializeField] GameObject     CardGraphicsObject { get; set; }
	[field: SerializeField] TextMeshPro    TitleTextObject    { get; set; }
	[field: SerializeField] TextMeshPro    BodyTextObject     { get; set; }
	[field: SerializeField] SpriteRenderer CardArtObject      { get; set; }
	[field: SerializeField] SpriteRenderer CardBackObject     { get; set; }

	public Quaternion   RotationInHand { get; set; }
	public InputManager InputManager   { get; set; }

	IEnumerator AnimationStepCoroutine { get; set; }
	bool        IsBeingDragged         { get; set; }

	Vector3 MouseOffset { get; set; }

	void OnMouseDown()
	{
		MouseOffset         = CardGraphicsObject.transform.position - InputManager.MousePosition;
		InputManager.IsBusy = true;
		IsBeingDragged      = true;
	}

	void OnMouseDrag()
	{
		CardGraphicsObject.transform.position = InputManager.MousePosition + MouseOffset;
	}

	void OnMouseEnter()
	{
		if (InputManager.IsBusy)
		{
			return;
		}

		Animate(MouseOverOffset, quaternion.identity, MouseEnterAnimationDuration, MouseEnterAnimationCurve);
	}

	void OnMouseExit()
	{
		if (IsBeingDragged)
		{
			return;
		}

		Animate(Vector3.zero, RotationInHand, MouseExitAnimationDuration, MouseExitAnimationCurve);
	}

	void OnMouseUp()
	{
		if (!IsBeingDragged)
		{
			return;
		}

		Animate(Vector3.zero, RotationInHand, MouseExitAnimationDuration, MouseExitAnimationCurve);
		InputManager.IsBusy = false;
		IsBeingDragged      = false;
	}

	public void Draw()
	{
		CardGraphicsObject.transform.position = DrawPileOffset + new Vector3(0, 0, transform.localPosition.z);
		Animate(Vector3.zero, RotationInHand, DrawAnimationDuration, DrawAnimationCurve);
	}

	public void Discard()
	{
		CardGraphicsObject.transform.localPosition = DiscardPileOffset;
		Animate(Vector3.zero, Quaternion.identity, DiscardAnimationDuration, DiscardAnimationCurve);
	}

	void Animate(Vector3        targetPosition,
				 Quaternion     targetRotation,
				 float          movementDuration,
				 AnimationCurve movementCurve)
	{
		if (AnimationStepCoroutine != null)
		{
			StopCoroutine(AnimationStepCoroutine);
		}

		AnimationStepCoroutine = AnimationStep(targetPosition, targetRotation, movementDuration, movementCurve);
		StartCoroutine(AnimationStepCoroutine);
	}

	IEnumerator AnimationStep(Vector3        targetPosition,
							  Quaternion     targetRotation,
							  float          movementDuration,
							  AnimationCurve movementCurve)
	{
		var t               = CardGraphicsObject.transform;
		var initialPosition = t.localPosition;
		var initialRotation = t.localRotation;
		var initialTime     = Time.time;
		var ratio           = 0f;

		while (ratio < 1f)
		{
			ratio           = (Time.time - initialTime) / movementDuration;
			t.localPosition = Vector3.Lerp(initialPosition, targetPosition, movementCurve.Evaluate(ratio));
			t.localRotation = Quaternion.Lerp(initialRotation, targetRotation, movementCurve.Evaluate(ratio));

			yield return new WaitForEndOfFrame();
		}
	}
}