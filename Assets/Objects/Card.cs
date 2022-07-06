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

	[field: SerializeField] float          DrawMovementDuration       { get; set; }
	[field: SerializeField] AnimationCurve DrawMovementCurve          { get; set; }
	[field: SerializeField] float          DiscardMovementDuration    { get; set; }
	[field: SerializeField] AnimationCurve DiscardMovementCurve       { get; set; }
	[field: SerializeField] float          MouseEnterMovementDuration { get; set; }
	[field: SerializeField] AnimationCurve MouseEnterMovementCurve    { get; set; }
	[field: SerializeField] float          MouseExitMovementDuration  { get; set; }
	[field: SerializeField] AnimationCurve MouseExitMovementCurve     { get; set; }

	[field: SerializeField] TextMeshPro    TitleTextObject { get; set; }
	[field: SerializeField] TextMeshPro    BodyTextObject  { get; set; }
	[field: SerializeField] SpriteRenderer CardArtObject   { get; set; }
	[field: SerializeField] SpriteRenderer CardBackObject  { get; set; }

	public Vector3    PositionInHand { get; set; }
	public Quaternion RotationInHand { get; set; }

	IEnumerator StepTowardTargetCoroutine { get; set; }

	void OnMouseEnter()
	{
		//TODO: expose to inspector
		var dy = new Vector3(0, 50, -100);
		MoveToTarget(PositionInHand + dy, quaternion.identity, MouseEnterMovementDuration, MouseEnterMovementCurve);
	}

	void OnMouseExit()
	{
		MoveToTarget(PositionInHand, RotationInHand, MouseExitMovementDuration, MouseExitMovementCurve);
	}

	public void Draw()
	{
		//TODO: draw pile location?
		MoveToTarget(PositionInHand, RotationInHand, DrawMovementDuration, DrawMovementCurve);
	}

	public void Discard()
	{
		//TODO: discard pile location
		MoveToTarget(PositionInHand, RotationInHand, DiscardMovementDuration, DiscardMovementCurve);
	}

	void MoveToTarget(Vector3        targetPosition,
					  Quaternion     targetRotation,
					  float          movementDuration,
					  AnimationCurve movementCurve)
	{
		if (StepTowardTargetCoroutine != null)
		{
			StopCoroutine(StepTowardTargetCoroutine);
		}

		StepTowardTargetCoroutine = StepTowardTarget(targetPosition, targetRotation, movementDuration, movementCurve);
		StartCoroutine(StepTowardTargetCoroutine);
	}

	IEnumerator StepTowardTarget(Vector3        targetPosition,
								 Quaternion     targetRotation,
								 float          movementDuration,
								 AnimationCurve movementCurve)
	{
		var t               = transform;
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