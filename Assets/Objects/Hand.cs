using System.Collections;
using UnityEngine;

public class Hand : MonoBehaviour
{
	[field: SerializeField] GameObject     CardPrefab           { get; set; }
	[field: SerializeField] int            HandSize             { get; set; }
	[field: SerializeField] float          HandWidth            { get; set; }
	[field: SerializeField] float          CardMovementInterval { get; set; }
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

			card.PositionInHand = new(i * cardSpacing - leftmostCardPosition,
									  CardFanCurve.Evaluate(i / (HandSize - 1f)) * CardRaiseFactor,
									  -i);

			card.RotationInHand = Quaternion.Euler(new(0, 0, -card.PositionInHand.x * CardFanningFactor));

			card.Draw();

			yield return new WaitForSeconds(CardMovementInterval);
		}
	}
}