using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenManager : MonoBehaviour
{
	Dictionary<Transform, IEnumerator> Coroutines { get; set; }

	public void Tween(Transform      transformToTween,
					  Vector3        targetPosition,
					  Quaternion     targetRotation,
					  float          tweenDuration,
					  AnimationCurve tweenCurve,
					  bool           isLocal = false)
	{
		// initialize the dictionary if this it the first time it's being used
		Coroutines ??= new();

		// create a new coroutine for this tween
		var coroutine = TweenStep(transformToTween, targetPosition, targetRotation, tweenDuration, tweenCurve, isLocal);

		// if there's already a coroutine running for this transform, stop it and update the entry
		if (Coroutines.ContainsKey(transformToTween))
		{
			StopCoroutine(Coroutines[transformToTween]);
			Coroutines[transformToTween] = coroutine;
		}

		// if there's no coroutine for this transform, add a new entry
		else
		{
			Coroutines.Add(transformToTween, coroutine);
		}

		// start the coroutine
		StartCoroutine(coroutine);
	}

	IEnumerator TweenStep(Transform      transformToTween,
						  Vector3        targetPosition,
						  Quaternion     targetRotation,
						  float          tweenDuration,
						  AnimationCurve tweenCurve,
						  bool           isLocal)
	{
		// start at the current time, which represents 0% tween completion
		var initialTime     = Time.time;
		var tweenCompletion = 0f;

		// set initial position/rotation
		Vector3    initialPosition;
		Quaternion initialRotation;
		if (isLocal)
		{
			initialPosition = transformToTween.localPosition;
			initialRotation = transformToTween.localRotation;
		}
		else
		{
			initialPosition = transformToTween.position;
			initialRotation = transformToTween.rotation;
		}

		// tween until completion is 100&
		while (tweenCompletion < 1f)
		{
			// calculate progress as ratio of completion
			tweenCompletion = (Time.time - initialTime) / tweenDuration;
			var positionRatio = tweenCurve.Evaluate(tweenCompletion);

			// calculate new position/rotation with lerps
			var position = Vector3.Lerp(initialPosition, targetPosition, positionRatio);
			var rotation = Quaternion.Lerp(initialRotation, targetRotation, positionRatio);

			// set the new position/rotation
			if (isLocal)
			{
				transformToTween.localPosition = position;
				transformToTween.localRotation = rotation;
			}
			else
			{
				transformToTween.position = position;
				transformToTween.rotation = rotation;
			}

			// do it again next frame
			yield return new WaitForEndOfFrame();
		}

		// upon completion, remove the coroutine from the dictionary
		Coroutines.Remove(transformToTween);
	}
}