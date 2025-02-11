using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RectTransform pauseUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseUI.gameObject.SetActive(!pauseUI.gameObject.activeSelf);
        }
    }
}
