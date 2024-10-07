using UnityEngine;
using UnityEngine.UI;

public class FoodItem
{
    public bool Ready { get; private set; } = false;
    public Texture2D Tex2D { get; private set; } = null;
    public RenderTexture RT { get; private set; } = null;
    public GameObject PrefabInstance { get; private set; } = null;

    public FoodItem() {}

    public FoodItem(Texture2D texture2D) {
        LoadTexture2D(texture2D);
    }

    public void LoadTexture2D(Texture2D texture2D) {
        Tex2D = texture2D;
        RT = new RenderTexture(Tex2D.width, Tex2D.height, 0, Tex2D.graphicsFormat, Tex2D.mipmapCount);
        Sync2DToRT();
        Ready = true;
    }

    public void Sync2DToRT() {
        //Graphics.CopyTexture(Tex2D, RT);
        RenderTexture oldRT = RenderTexture.active;
        RenderTexture.active = RT;
        Graphics.Blit(Tex2D, RT);
        RenderTexture.active = oldRT;
    }

    public void SyncRTTo2D() {
        //Graphics.CopyTexture(RT, Tex2D);
        RenderTexture oldRT = RenderTexture.active;
        RenderTexture.active = RT;
        Tex2D.ReadPixels(new Rect(0, 0, RT.width, RT.height), 0, 0);
        Tex2D.Apply();
        RenderTexture.active = oldRT;
    }

    public void SetPrefabInstance(GameObject prefabInstance) {
        PrefabInstance = prefabInstance;
        RawImage rawImage = prefabInstance.GetComponent<RawImage>();
        rawImage.texture = RT;
        rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Tex2D.width);
        rawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Tex2D.height);
    }
}
