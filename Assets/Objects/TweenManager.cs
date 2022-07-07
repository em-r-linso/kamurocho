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
					  AnimationCurve tweenCurve)
	{
		// initialize the dictionary if this it the first time it's being used
		Coroutines ??= new();

		// create a new coroutine for this tween
		var coroutine = TweenStep(transformToTween, targetPosition, targetRotation, tweenDuration, tweenCurve);

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
						  AnimationCurve tweenCurve)
	{
		var initialPosition = transformToTween.localPosition;
		var initialRotation = transformToTween.localRotation;
		var initialTime     = Time.time;
		var timeRatio       = 0f;

		// tween until time is 1/1 (position ratio will also be 1/1)
		while (timeRatio < 1f)
		{
			timeRatio = (Time.time - initialTime) / tweenDuration;
			var positionRatio = tweenCurve.Evaluate(timeRatio);

			transformToTween.localPosition = Vector3.Lerp(initialPosition, targetPosition, positionRatio);
			transformToTween.localRotation = Quaternion.Lerp(initialRotation, targetRotation, positionRatio);

			yield return new WaitForEndOfFrame();
		}

		// upon completion, remove the coroutine from the dictionary
		Coroutines.Remove(transformToTween);
	}
}