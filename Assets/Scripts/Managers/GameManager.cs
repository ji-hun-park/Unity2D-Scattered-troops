using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴 적용
    public static GameManager Instance;
    [SerializeField] private float scatterRadius;
    
    public List<UNIT> allUnits;  // 씬에 존재하는 모든 유닛 리스트
    public List<UNIT> selectedUnits = new List<UNIT>(); // 선택된 유닛 리스트
    
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

        scatterRadius = 1f;
    }

    private void Start()
    {
        allUnits = FindObjectsByType<UNIT>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ScatterUnits(selectedUnits, scatterRadius);
        }

        if (Input.GetMouseButtonDown(1))
        {
            moveUnits(selectedUnits, Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
