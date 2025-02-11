using UnityEngine;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public RawImage targetImage; // 그림을 그릴 UI Image (RawImage 사용 권장)
    private Texture2D drawTexture;
    private Vector2 selectionStart;  // 선택 시작점
    private Vector2 selectionEnd;    // 선택 끝점
    public int textureWidth = 1920; // 캔버스 너비
    public int textureHeight = 1080; // 캔버스 높이
    public float brushSize = 5f;
    
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
            DrawSelectionBorder(start, end, Color.green, brushSize); // 두께 n의 회색 테두리
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
    
    void DrawSelectionBorder(Vector2 start, Vector2 end, Color borderColor, float thickness)
    {
        int xMin = Mathf.RoundToInt(Mathf.Min(start.x, end.x));
        int xMax = Mathf.RoundToInt(Mathf.Max(start.x, end.x));
        int yMin = Mathf.RoundToInt(Mathf.Min(start.y, end.y));
        int yMax = Mathf.RoundToInt(Mathf.Max(start.y, end.y));

        // 상단 변
        DrawThickLineForSelect(new Vector2(xMin, yMin), new Vector2(xMax, yMin), borderColor, thickness);
        // 하단 변
        DrawThickLineForSelect(new Vector2(xMin, yMax), new Vector2(xMax, yMax), borderColor, thickness);
        // 왼쪽 변
        DrawThickLineForSelect(new Vector2(xMin, yMin), new Vector2(xMin, yMax), borderColor, thickness);
        // 오른쪽 변
        DrawThickLineForSelect(new Vector2(xMax, yMin), new Vector2(xMax, yMax), borderColor, thickness);

        drawTexture.Apply(); // 변경 사항 적용
    }
    
    void DrawThickLineForSelect(Vector2 start, Vector2 end, Color color, float thickness)
    {
        int x0 = Mathf.RoundToInt(start.x);
        int y0 = Mathf.RoundToInt(start.y);
        int x1 = Mathf.RoundToInt(end.x);
        int y1 = Mathf.RoundToInt(end.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            for (int tx = -Mathf.CeilToInt(thickness / 2); tx <= Mathf.FloorToInt(thickness / 2); tx++)
            {
                for (int ty = -Mathf.CeilToInt(thickness / 2); ty <= Mathf.FloorToInt(thickness / 2); ty++)
                {
                    int px = x0 + tx;
                    int py = y0 + ty;

                    if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                    {
                        drawTexture.SetPixel(px, py, color);
                    }
                }
            }

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
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
                    drawTexture.SetPixel(x, y, Color.clear);
                }
            }
        }

        // 테두리만 다시 덮어씌움
        // 상단 변
        DrawThickLineForSelect(new Vector2(xMin, yMin), new Vector2(xMax, yMin), Color.clear, thickness);
        // 하단 변
        DrawThickLineForSelect(new Vector2(xMin, yMax), new Vector2(xMax, yMax), Color.clear, thickness);
        // 왼쪽 변
        DrawThickLineForSelect(new Vector2(xMin, yMin), new Vector2(xMin, yMax), Color.clear, thickness);
        // 오른쪽 변
        DrawThickLineForSelect(new Vector2(xMax, yMin), new Vector2(xMax, yMax), Color.clear, thickness);
        
        // 꼭짓점 부분 정확히 지우기
        ClearCorner(new Vector2(xMin, yMin), thickness);
        ClearCorner(new Vector2(xMax, yMin), thickness);
        ClearCorner(new Vector2(xMin, yMax), thickness);
        ClearCorner(new Vector2(xMax, yMax), thickness);
        
        drawTexture.Apply();
    }
    
    void ClearCorner(Vector2 corner, float thickness)
    {
        int cx = Mathf.RoundToInt(corner.x);
        int cy = Mathf.RoundToInt(corner.y);

        for (int dx = -Mathf.CeilToInt(thickness / 2); dx <= Mathf.FloorToInt(thickness / 2); dx++)
        {
            for (int dy = -Mathf.CeilToInt(thickness / 2); dy <= Mathf.FloorToInt(thickness / 2); dy++)
            {
                int px = cx + dx;
                int py = cy + dy;

                if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                {
                    drawTexture.SetPixel(px, py, Color.clear);
                }
            }
        }
    }
}
