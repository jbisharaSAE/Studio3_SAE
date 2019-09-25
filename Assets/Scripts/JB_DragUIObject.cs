using UnityEngine;
using UnityEngine.EventSystems; // 1

public class JB_DragUIObject : MonoBehaviour
    , IPointerClickHandler // 2
    , IDragHandler
    , IPointerEnterHandler
    , IPointerExitHandler
// ... And many more available!
{
    SpriteRenderer sprite;
    Color target = Color.red;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (sprite)
            sprite.color = Vector4.MoveTowards(sprite.color, target, Time.deltaTime * 10);
    }

    public void OnPointerClick(PointerEventData eventData) // 3
    {
        print("I was clicked");
        target = Color.blue;
    }

    

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.dragging)
        print("I'm being dragged!");
        target = Color.magenta;
        //rect.localPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);



        //Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

        rect.transform.position = Camera.main.WorldToViewportPoint(Input.mousePosition);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        target = Color.green;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        target = Color.red;
    }
}