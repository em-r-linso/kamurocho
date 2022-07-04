using System.Collections;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
	[field: SerializeField] string  TitleText { get; set; }
	[field: SerializeField] string  BodyText  { get; set; }
	[field: SerializeField] Texture CardArt   { get; set; }
	[field: SerializeField] Texture CardBack  { get; set; }

	[field: SerializeField] TextMeshPro    TitleTextObject { get; set; }
	[field: SerializeField] TextMeshPro    BodyTextObject  { get; set; }
	[field: SerializeField] SpriteRenderer CardArtObject   { get; set; }
	[field: SerializeField] SpriteRenderer CardBackObject  { get; set; }

	public void MoveToTarget(Vector3        targetPosition,
							 Quaternion     targetRotation,
							 float          movementDuration,
							 AnimationCurve movementCurve)
	{
		StartCoroutine(StepTowardTarget(targetPosition, targetRotation, movementDuration, movementCurve));
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