using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public UnityEngine.UI.Text text;
    protected bool running = false;
    protected CanvasGroup cg;
    protected System.Action cb;

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            cg.alpha = Mathf.Lerp(cg.alpha, 1.0f, 0.5f);
        }
        else
        {
            cg.alpha = Mathf.Lerp(cg.alpha, 0.0f, 0.5f);
        }

        if (Input.anyKeyDown)
        {
            running = false;
            Time.timeScale = 1.0f;
            if (cb != null)
                cb.Invoke();
        }
    }

    public void ShowText(string text, System.Action OnComplete)
    {
        running = true;
        this.text.text = text;
        Time.timeScale = 0.0f;
        this.cb = OnComplete;
    }

    public void ShowText(List<string> text, System.Action OnComplete)
    {
        if (text.Count > 0)
        {
            ShowText(text[0], () => ShowText(text.GetRange(1, text.Count - 1), OnComplete));
        }
        else
        {
            OnComplete.Invoke();
        }
    }
}
