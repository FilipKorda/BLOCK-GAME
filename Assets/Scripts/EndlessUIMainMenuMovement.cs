using UnityEngine;

public class EndlessUIMainMenuMovement : MonoBehaviour
{
    public float speed = 100f;
    public RectTransform leftObject;
    public RectTransform middleObject;
    public RectTransform rightObject;

    private float objectWidth;
    private RectTransform canvasRect;

    void Start()
    {
        objectWidth = middleObject.rect.width;
        canvasRect = GetComponent<RectTransform>();
        ResetPositions();
    }

    void Update()
    {
        leftObject.anchoredPosition += Vector2.right * speed * Time.deltaTime;
        middleObject.anchoredPosition += Vector2.right * speed * Time.deltaTime;
        rightObject.anchoredPosition += Vector2.right * speed * Time.deltaTime;

        if (leftObject.anchoredPosition.x >= canvasRect.rect.width / 2 + objectWidth / 2)
        {
            leftObject.anchoredPosition = new Vector2(middleObject.anchoredPosition.x - objectWidth, leftObject.anchoredPosition.y);
            Swap(ref leftObject, ref middleObject, ref rightObject);
        }
        else if (middleObject.anchoredPosition.x >= canvasRect.rect.width / 2 + objectWidth / 2)
        {
            middleObject.anchoredPosition = new Vector2(rightObject.anchoredPosition.x - objectWidth, middleObject.anchoredPosition.y);
            Swap(ref middleObject, ref rightObject, ref leftObject);
        }
        else if (rightObject.anchoredPosition.x >= canvasRect.rect.width / 2 + objectWidth / 2)
        {
            rightObject.anchoredPosition = new Vector2(leftObject.anchoredPosition.x - objectWidth, rightObject.anchoredPosition.y);
            Swap(ref rightObject, ref leftObject, ref middleObject);
        }
    }

    void ResetPositions()
    {
        leftObject.anchoredPosition = new Vector2(-objectWidth, 0);
        middleObject.anchoredPosition = new Vector2(0, 0);
        rightObject.anchoredPosition = new Vector2(objectWidth, 0);
    }

    void Swap(ref RectTransform a, ref RectTransform b, ref RectTransform c)
    {
        RectTransform temp = a;
        a = b;
        b = c;
        c = temp;
    }
}
