using System.Collections;
using UnityEngine;

public class UNIT : MonoBehaviour
{
    private GameObject _selectCircle;
    private const float MoveSpeed = 1f;
    
    private void Start()
    {
        _selectCircle = transform.GetChild(0).gameObject;
        _selectCircle.gameObject.SetActive(false);
    }
    
    public void MoveTo(Vector2 target)
    {
        StartCoroutine(MoveRoutine(target));
    }

    IEnumerator MoveRoutine(Vector2 target)
    {
        while ((Vector2)transform.position != target)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, MoveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void Select()
    {
        _selectCircle.gameObject.SetActive(true);
    }

    public void Deselect()
    {
        _selectCircle.gameObject.SetActive(false);
    }
}
