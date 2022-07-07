using TMPro;
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
	public TweenManager TweenManager   { get; set; }

	bool IsBeingDragged { get; set; }

	Vector3 MouseOffset { get; set; }

	void OnMouseDown()
	{
		// snap to MouseOver position/rotation in case OnMouseEnter tween isn't done yet
		CardGraphicsObject.transform.localPosition = MouseOverOffset;
		CardGraphicsObject.transform.localRotation = Quaternion.identity;

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

		TweenManager.Tween(CardGraphicsObject.transform,
						   MouseOverOffset,
						   Quaternion.identity,
						   MouseEnterAnimationDuration,
						   MouseEnterAnimationCurve);
	}

	void OnMouseExit()
	{
		if (IsBeingDragged)
		{
			return;
		}

		TweenManager.Tween(CardGraphicsObject.transform,
						   Vector3.zero,
						   RotationInHand,
						   MouseExitAnimationDuration,
						   MouseExitAnimationCurve);
	}

	void OnMouseUp()
	{
		if (!IsBeingDragged)
		{
			return;
		}

		TweenManager.Tween(CardGraphicsObject.transform,
						   Vector3.zero,
						   RotationInHand,
						   MouseExitAnimationDuration,
						   MouseExitAnimationCurve);
		InputManager.IsBusy = false;
		IsBeingDragged      = false;
	}

	public void Draw()
	{
		CardGraphicsObject.transform.position = DrawPileOffset + new Vector3(0, 0, transform.localPosition.z);
		TweenManager.Tween(CardGraphicsObject.transform,
						   Vector3.zero,
						   RotationInHand,
						   DrawAnimationDuration,
						   DrawAnimationCurve);
	}

	public void Discard()
	{
		CardGraphicsObject.transform.localPosition = DiscardPileOffset;
		TweenManager.Tween(CardGraphicsObject.transform,
						   Vector3.zero,
						   Quaternion.identity,
						   DiscardAnimationDuration,
						   DiscardAnimationCurve);
	}
}