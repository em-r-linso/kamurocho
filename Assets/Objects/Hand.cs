using System.Collections;
using UnityEngine;

public class Hand : MonoBehaviour
{
	[field: SerializeField] GameObject     CardPrefab           { get; set; }
	[field: SerializeField] int            HandSize             { get; set; }
	[field: SerializeField] float          HandWidth            { get; set; }
	[field: SerializeField] float          CardMovementDuration { get; set; }
	[field: SerializeField] float          CardMovementInterval { get; set; }
	[field: SerializeField] AnimationCurve CardMovementCurve    { get; set; }
	[field: SerializeField] Vector3        CardDrawPosition     { get; set; }
	[field: SerializeField] float          CardFanningFactor    { get; set; }
	[field: SerializeField] float          CardRaiseFactor      { get; set; }
	[field: SerializeField] AnimationCurve CardFanCurve         { get; set; }

	void Start()
	{
		StartCoroutine(MoveCards());
	}

	IEnumerator MoveCards()
	{
		var cardSpacing          = HandWidth           / HandSize;
		var leftmostCardPosition = (HandSize - 1) / 2f * cardSpacing;
		for (var i = 0; i < HandSize; i++)
		{
			var card = Instantiate(CardPrefab, transform).GetComponent<Card>();

			card.transform.localPosition = CardDrawPosition;

			var targetPosition = new Vector3(i * cardSpacing - leftmostCardPosition,
											 CardFanCurve.Evaluate(i / (HandSize - 1f)) * CardRaiseFactor,
											 -i);

			var targetRotation = Quaternion.Euler(new(0, 0, -targetPosition.x * CardFanningFactor));

			card.MoveToTarget(targetPosition, targetRotation, CardMovementDuration, CardMovementCurve);

			yield return new WaitForSeconds(CardMovementInterval);
		}
	}
}