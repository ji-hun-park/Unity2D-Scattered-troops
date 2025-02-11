using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public enum Mode
    {
        Normal,
        Attack
    }
    
    // 싱글톤 패턴 적용
    public static GameManager Instance;
    public Mode mode = Mode.Normal;
    public GameObject cursorPrefab; // 커서 아이콘 프리팹
    [SerializeField] private GameObject cursorInstance; // 현재 활성화된 커서
    private SpriteRenderer cursorRenderer;
    public LayerMask groundLayer; // 마우스가 닿을 수 있는 레이어
    public List<UNIT> allUnits;  // 씬에 존재하는 모든 유닛 리스트
    public List<UNIT> selectedUnits = new List<UNIT>(); // 선택된 유닛 리스트
    public List<ENEMY> allEnemies; // 씬에 존재하는 모든 적 유닛 리스트
    private Transform enemy;
    
    private void Awake()
    {
        // Instance 존재 유무에 따라 게임 매니저 파괴 여부 정함
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 기존에 존재 안하면 이걸로 대체하고 파괴하지 않기
        }
        else
        {
            Destroy(gameObject); // 기존에 존재하면 자신파괴
        }
    }

    private void Start()
    {
        // 씬에 있는 모든 아군 리스트에 추가
        allUnits = FindObjectsByType<UNIT>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        // 씬에 있는 모든 적군 리스트에 추가
        allEnemies = FindObjectsByType<ENEMY>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        // 마우스 커서 아이콘 생성
        cursorInstance = Instantiate(cursorPrefab);
        cursorRenderer = cursorInstance.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        FollowMouse(); // 지속적으로 마우스를 따라가도록 실행

        if (Input.GetKeyDown(KeyCode.A))
        {
            mode = mode == Mode.Normal ? Mode.Attack : Mode.Normal;
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScatterUnits(selectedUnits, 1f);
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            ScatterUnits(selectedUnits, 3f);
        }

        if (Input.GetMouseButtonDown(1))
        {
            moveUnits(selectedUnits, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        
        if (Input.GetMouseButtonDown(0) && mode == Mode.Attack)
        {
            enemy = null;
            enemy = SelectEnemyInRectangle(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            mode = Mode.Normal;
            SurroundEnemy(selectedUnits, enemy, 2f);
        }
    }
    
    void FollowMouse()
    {
        cursorRenderer.color = mode == Mode.Normal ? Color.white : Color.red;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 위치에서 레이 발사
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            cursorInstance.transform.position = hit.point; // 커서 위치 갱신
        }
        
        cursorInstance.transform.position = Vector3.Lerp(
            cursorInstance.transform.position, 
            hit.point, 
            Time.deltaTime * 10
        );
    }
    
    void SurroundEnemy(List<UNIT> selectedUnits, Transform enemyT, float radius)
    {
        if (selectedUnits.Count == 0 || enemyT == null) return;

        Vector2 enemyPos = enemyT.position;
        int unitCount = selectedUnits.Count;
        float angleStep = 360f / unitCount; // 균등한 간격으로 배치

        for (int i = 0; i < unitCount; i++)
        {
            float angle = angleStep * i;
            Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            Vector2 targetPosition = enemyPos + offset;

            selectedUnits[i].MoveTo(targetPosition); // 이동 실행
        }
    }
    
    void ScatterUnits(List<UNIT> selectedUnits, float scatterRadius)
    {
        Vector2 center = GetCenter(selectedUnits);
        float angleStep = 360f / selectedUnits.Count;

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            float angle = angleStep * i + Random.Range(-15f, 15f); // 각도를 약간 랜덤하게
            Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * scatterRadius;
            Vector2 targetPosition = center + offset;

            selectedUnits[i].MoveTo(targetPosition); // 이동 함수 호출
        }
    }
    
    Transform SelectEnemyInRectangle(Vector2 Pos)
    {
        float offset = 50f;
        
        // x, y 최소/최대값 구해서 사각형 정의
        float minX = Pos.x - offset;
        float maxX = Pos.x + offset;
        float minY = Pos.y - offset;
        float maxY = Pos.y + offset;

        foreach (ENEMY curEnemy in allEnemies)
        {
            Vector2 unitPos = curEnemy.transform.position; // 유닛 위치
            if (unitPos.x >= minX && unitPos.x <= maxX && unitPos.y >= minY && unitPos.y <= maxY)
            {
                return curEnemy.transform;
            }
        }
        return null;
    } 

    void moveUnits(List<UNIT> selectedUnits, Vector2 mousePosition)
    {
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            selectedUnits[i].MoveTo(mousePosition);
        }
    }
    
    Vector2 GetCenter(List<UNIT> selectedUnits)
    {
        if (selectedUnits.Count == 0) return Vector2.zero;

        Vector2 center = Vector2.zero;
        foreach (var unit in selectedUnits)
        {
            center += (Vector2)unit.transform.position;
        }
        return center / selectedUnits.Count;
    }
}
