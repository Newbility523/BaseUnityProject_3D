using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Engine;

[AddComponentMenu("UI/Effects/NewText")]
public class NewText : Text
{
    // Start is called before the first frame update
    [SerializeField]
    public ColorTag colorTag = ColorTag.None;
    protected override void Awake()
    {
        Debug.Log("Awake");
        base.Awake();
        Color c;
        if (ColorUtility.TryParseHtmlString(ColorConfig.GetColor(this.colorTag), out c))
        {
            Text t = transform.GetComponent<Text>();
            if (t != null)
            {
                t.color = c;
            }
        }
    }

    //protected override void Start()
    //{
    //    Debug.Log("Start");
    //    base.Start();
    //}

    //protected override void OnEnable()
    //{
    //    Debug.Log("OnEnable");
    //    base.OnEnable();
    //}
}
