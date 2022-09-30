using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroller : MonoBehaviour
{
    [SerializeField] private Renderer backgroundRenderer;
    [SerializeField] private float speedMult;
    private float speed;

    private void Start()
    {
        //this.speed = QuestionManager.instance.GetGameSpeed() * speedMult;
    }

    void Update()
    {
        backgroundRenderer.material.mainTextureOffset += new Vector2(speed * 0.03f * Time.deltaTime, 0);
        if (backgroundRenderer.material.mainTextureOffset.x > 1f)
        {
            backgroundRenderer.material.mainTextureOffset = Vector2.zero;
        }
    }

    public void UpdateSpeed()
    {
        this.speed = QuestionManager.instance.GetGameSpeed() * speedMult;
    }
}
