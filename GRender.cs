using UnityEngine;
using UnityEngine.UI;

/* 
 * 适用于有AlphaBlend模式材质的RenterTexture
 * https://github.com/garsonlab/RenderTuxture-AlphaBlend
 * Garson
 */
public class GRender : MonoBehaviour
{
    [SerializeField]
    private Camera m_BlackCamera;
    [SerializeField]
    private Camera m_WhiteCamera;
    [SerializeField]
    private float m_OrthographicSize = 3.2f;
    [SerializeField]
    private Vector2 m_TextureSize;
    [SerializeField]
    private RenderTexture m_BlackTexture;
    [SerializeField]
    private RenderTexture m_WhiteTexture;
    [SerializeField]
    private Material m_Material;
    [SerializeField]
    private RawImage m_Image;

    /// <summary>
    /// 创建RenderTexture
    /// </summary>
    /// <param name="name">名字</param>
    /// <param name="pos">相机位置</param>
    /// <param name="size">相机大小</param>
    /// <param name="img">渲染目标</param>
    /// <returns>GRender</returns>
    public static GRender Create(string name, Vector3 pos, Vector2 size, RawImage img)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = pos;
        var render = obj.AddComponent<GRender>();
        render.m_TextureSize = size;
        render.m_Image = img;
        render.Start();
        return render;
    }


    protected void Start()
    {
        if (!m_BlackCamera)
           m_BlackCamera = CreateCamera("Black Camera", Color.black);
        if (!m_WhiteCamera)
            m_WhiteCamera = CreateCamera("White Camera", Color.white);
        if (!m_BlackTexture)
            m_BlackTexture = CreateTexture();
        if (!m_WhiteTexture)
            m_WhiteTexture = CreateTexture();

        m_BlackCamera.targetTexture = m_BlackTexture;
        m_WhiteCamera.targetTexture = m_WhiteTexture;
        
        if (!m_Material)
            m_Material = CreateMaterial();

        AddImage(m_Image);
    }
    
    protected void OnDestroy()
    {
        if(m_Material)
            Destroy(m_Material);
        if(m_BlackTexture)
            Destroy(m_BlackTexture);
        if(m_WhiteTexture)
            Destroy(m_WhiteTexture);

        if (m_BlackCamera)
            Destroy(m_BlackCamera.gameObject);
        if(m_WhiteCamera)
            Destroy(m_WhiteCamera.gameObject);
    }


    /// <summary>正交相机大小</summary>
    public float orthographicSize
    {
        get { return m_OrthographicSize; }
        set { ChangeCameraSize(value); }
    }

    /// <summary>是否灰度化</summary>
    public bool grey
    {
        set
        {
            if (m_Material)
                m_Material.SetFloat("_Grey", value ? 1 : 0);
        }
    }

    /// <summary>当前位置</summary>
    public Vector3 Center
    {
        get { return transform.position; }
    }

    /// <summary>添加渲染目标</summary>
    public void AddImage(RawImage img)
    {
        if(!img)
            return;
        img.texture = m_WhiteTexture;
        if (m_Material)
            img.material = m_Material;
    }
    
    private Camera CreateCamera(string name, Color bgColor)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0, 0, -100);

        var camera = obj.AddComponent<Camera>();
        camera.backgroundColor = bgColor;
        camera.orthographic = true;
        camera.orthographicSize = m_OrthographicSize;
        camera.allowMSAA = false;
        camera.allowHDR = false;
        return camera;
    }

    private RenderTexture CreateTexture()
    {
        return new RenderTexture((int)m_TextureSize.x, (int)m_TextureSize.y, 24, RenderTextureFormat.ARGB32)
        {
            antiAliasing = 1,
            filterMode = FilterMode.Bilinear,
            anisoLevel = 0,
            useMipMap = false
        };
    }

    private Material CreateMaterial()
    {
        Material mat = new Material(Shader.Find("UI/GRenderShader"));
        mat.SetTexture("_MainTex", m_WhiteTexture);
        mat.SetTexture("_BlackTex", m_BlackTexture);
        return mat;
    }

    private void ChangeCameraSize(float value)
    {
        m_OrthographicSize = value;
        if (m_BlackCamera)
            m_BlackCamera.orthographicSize = value;
        if (m_WhiteCamera)
            m_WhiteCamera.orthographicSize = value;
    }
    
}
