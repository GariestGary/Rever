using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlipbookPlayer : MonoCached
{
    [SerializeField] private List<Flipbook> flipbooks = new List<Flipbook>();

	private SpriteRenderer renderer;
	private Flipbook currentFlipbook;
	private int currentFrame;
	private float currentFrameInterval;
	private float currentFlipbookTime;
	private float frameChangeTime;
	private bool pause;

	public override void Rise()
	{
		renderer = GetComponent<SpriteRenderer>();

		if(flipbooks.Count > 0)
		{
			ChangeFlipbook(flipbooks[0].FlipbookName);
		}

		StartCoroutine(FlipCoroutine());
	}

	public void ChangeFlipbook(string name)
	{
		Flipbook flip = flipbooks.Where(x => x.FlipbookName == name).FirstOrDefault();

		if (flip)
		{
			currentFlipbook = flip;
			currentFrame = 0;
			currentFlipbookTime = 0;
			currentFrameInterval = 1.0f / (float)currentFlipbook.FPS;
			frameChangeTime = currentFrameInterval;
			pause = false;
		}
		else
		{
			Debug.LogWarning(this.name + "doesn't have flipbook named " + name + " on it's FlipbookPlayer");
		}
	}

	private IEnumerator FlipCoroutine()
	{
		while(true)
		{
			if(pause)
			{
				continue;
			}

			if (currentFlipbookTime >= frameChangeTime)
			{
				currentFrame++;

				renderer.sprite = currentFlipbook.Frames[currentFrame];

				frameChangeTime = currentFlipbookTime + currentFrameInterval;
				
				if(currentFrame == currentFlipbook.Frames.Count - 1)
				{
					if(!currentFlipbook.Loop)
					{
						pause = true;
						continue;
					}

					currentFlipbookTime = 0;
					currentFrame = 0;
					frameChangeTime = currentFrameInterval;
					renderer.sprite = currentFlipbook.Frames[0];
				}
			}

			currentFlipbookTime += Time.deltaTime;

			yield return null;
		}
	}
}
