using System.Collections;
using UnityEngine;

public class Hand : MonoBehaviour
{
	[field: SerializeField] GameObject     CardPrefab            { get; set; }
	[field: SerializeField] int            HandSize              { get; set; }
	[field: SerializeField] float          HandWidth             { get; set; }
	[field: SerializeField] float          CardMovementInterval  { get; set; }
	[field: SerializeField] float          CardFanningFactor     { get; set; }
	[field: SerializeField] float          CardRaiseFactor       { get; set; }
	[field: SerializeField] AnimationCurve CardFanCurve          { get; set; }
	[field: SerializeField] float          CardEdgeColliderWidth { get; set; }

	[field: SerializeField] InputManager InputManager { get; set; }
	[field: SerializeField] TweenManager TweenManager { get; set; }

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

			var t = card.transform;

			t.localPosition = new(i * cardSpacing - leftmostCardPosition,
								  CardFanCurve.Evaluate(i / (HandSize - 1f)) * CardRaiseFactor,
								  -i);

			card.RotationInHand = Quaternion.Euler(new(0, 0, -t.localPosition.x * CardFanningFactor));

			var c = card.GetComponent<BoxCollider2D>();
			if (i == 0)
			{
				c.size   = new(CardEdgeColliderWidth, c.size.y);
				c.offset = new(-(CardEdgeColliderWidth - cardSpacing) / 2f, c.offset.y);
			}
			else if (i == HandSize - 1)
			{
				c.size   = new(CardEdgeColliderWidth, c.size.y);
				c.offset = new((CardEdgeColliderWidth - cardSpacing) / 2f, c.offset.y);
			}
			else
			{
				c.size = new(cardSpacing, c.size.y);
			}

			card.InputManager = InputManager;
			card.TweenManager = TweenManager;

			card.Draw();

			yield return new WaitForSeconds(CardMovementInterval);
		}
	}
}