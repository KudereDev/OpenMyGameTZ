using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeCheckController : MonoBehaviour
{
    Vector3 startingPosition;
    Vector3 endingPosition;
    Vector2 screenSesitivity = new Vector2(Screen.width / 2, Screen.height / 2);



    private void Update()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR_WIN
        if (Input.touchCount > 0) 
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began) 
            {
                startingPosition = Input.GetTouch(0).position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                endingPosition = Input.GetTouch(0).position;

                Debug.Log($"Mouse touch ended, xDelta:{endingPosition.x - startingPosition.x}, yDelta:{endingPosition.y - startingPosition.y}");
            }
        }
#elif UNITY_EDITOR_WIN
        if (Input.GetMouseButtonDown(0)) 
        {
            startingPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endingPosition = Input.mousePosition;

            Debug.Log($"Mouse touch ended, xDelta:{endingPosition.x - startingPosition.x}, yDelta:{endingPosition.y - startingPosition.y}");
        }
#endif

    }

    private void PointSwipeType(float xDelta, float yDelta)
    {
        if (Mathf.Abs(xDelta) < screenSesitivity.x && Mathf.Abs(yDelta) < screenSesitivity.y)
        {
            return;
        }

        if (Mathf.Abs(xDelta) >= Mathf.Abs(yDelta))
        {
            if (xDelta < 0) 
            {
                //Swipe left
            }
            else
            {
                //Swipe right
            }
        }
        else 
        {
            if (yDelta < 0)
            {
                //Swipe down
            }
            else
            {
                //Swipe up
            }
        }
    }
}

public enum SwipeType
{
    Left,
    Right,
    Up,
    Down,
}
