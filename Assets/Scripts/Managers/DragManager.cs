using UnityEngine;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public RawImage targetImage; // 그림을 그릴 UI Image (RawImage 사용 권장)
    private Vector2 selectionStart;  // 선택 시작점
    private Vector2 selectionEnd;    // 선택 끝점
    public int textureWidth = 1920; // 캔버스 너비
    public int textureHeight = 1080; // 캔버스 높이
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectArea(Input.mousePosition);
        }
    }

    void SelectArea(Vector2 mousePosition)
    {
        selectionStart = Vector2.zero;  // 선택 시작점
        selectionEnd = Vector2.zero;    // 선택 끝점
        
        // 마우스 드래그로 영역 선택
        selectionStart = mousePosition; // 시작 지점
        
        if (Input.GetMouseButtonUp(0))
        {
            selectionEnd = Input.mousePosition; // 끝 지점
            // 테두리 그리기
            Vector2 start = ScreenToTextureCoord(selectionStart);
            Vector2 end = ScreenToTextureCoord(selectionEnd);
            DrawSelectionBorder(start, end, Color.gray, brushSize); // 두께 n의 회색 테두리
        }

        // 선택 영역 지우기
        if (Input.GetKeyDown(KeyCode.Delete)) // 'Delete' 키로 영역 삭제
        {
            ClearSelectionArea(ScreenToTextureCoord(selectionStart), ScreenToTextureCoord(selectionEnd), brushSize);
        }
    }
    
    Vector2 ScreenToTextureCoord(Vector2 screenPosition)
    {
        RectTransform rectTransform = targetImage.GetComponent<RectTransform>();
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out localPoint))
        {
            float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
            float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;
            return new Vector2(Mathf.Clamp(x, 0, textureWidth - 1), Mathf.Clamp(y, 0, textureHeight - 1));
        }

        return Vector2.zero;
    }
    
    void ClearSelectionArea(Vector2 start, Vector2 end, float thickness)
    {
        int xMin = Mathf.RoundToInt(Mathf.Min(start.x, end.x));
        int xMax = Mathf.RoundToInt(Mathf.Max(start.x, end.x));
        int yMin = Mathf.RoundToInt(Mathf.Min(start.y, end.y));
        int yMax = Mathf.RoundToInt(Mathf.Max(start.y, end.y));

        // 내부 지우기
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                if (x >= 0 && x < textureWidth && y >= 0 && y < textureHeight)
                {
                    drawTexture.SetPixel(x, y, Color.white);
                }
            }
        }

        // 테두리만 다시 덮어씌움
        // 상단 변
        DrawThickLine(new Vector2(xMin, yMin), new Vector2(xMax, yMin), Color.white, thickness);
        // 하단 변
        DrawThickLine(new Vector2(xMin, yMax), new Vector2(xMax, yMax), Color.white, thickness);
        // 왼쪽 변
        DrawThickLine(new Vector2(xMin, yMin), new Vector2(xMin, yMax), Color.white, thickness);
        // 오른쪽 변
        DrawThickLine(new Vector2(xMax, yMin), new Vector2(xMax, yMax), Color.white, thickness);
        
        // 꼭짓점 부분 정확히 지우기
        ClearCorner(new Vector2(xMin, yMin), thickness);
        ClearCorner(new Vector2(xMax, yMin), thickness);
        ClearCorner(new Vector2(xMin, yMax), thickness);
        ClearCorner(new Vector2(xMax, yMax), thickness);
        
        drawTexture.Apply();
    }
}
