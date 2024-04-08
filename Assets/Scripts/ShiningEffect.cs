using UnityEngine;
using UnityEngine.UI;

public class ShiningEffect : MonoBehaviour
{
    private float transparentValue;
    private int trend;
    public float shiningSpeed;
    public float maxTransparentValue;
    public Image image;
    public Color OriginalColor;

    void Start()
    {
        if (!PlayerPrefs.HasKey("IfUsePointer"))
        {
            PlayerPrefs.SetInt("IfUsePointer", 1);
        }
        transparentValue = 0;
        trend = 1;
        OriginalColor = image.color;
        if(maxTransparentValue>1 || maxTransparentValue <= 0)
        {
            maxTransparentValue = 0.5f;
        }
    }


    private void FixedUpdate()
    {
        if (transparentValue > maxTransparentValue)
        {
            trend = -1;
        }
        if (transparentValue <= 0)
        {
            trend = 1;
        }
        transparentValue += Time.deltaTime * trend * shiningSpeed * PlayerPrefs.GetInt("IfUsePointer");
        image.color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, transparentValue);
    }
}
