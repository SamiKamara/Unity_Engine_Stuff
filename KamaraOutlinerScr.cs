/*
This script draws outline to Unity Text component by cloning the 
text and offsetting custom amount of clones from the original text.
*/

using UnityEngine;
using UnityEngine.UI;

public class KamaraOutlinerScr : MonoBehaviour
{
    [HideInInspector] public bool CallForUpdate;
    [HideInInspector] public bool DrawOutline = true;
    [HideInInspector] public bool DoOgUgClone = true;
    [Range(1, 32)] [SerializeField] public int Density = 16;
    [Range(0.005f, 0.02f)] [SerializeField] public float OutlineDistance = 0.008f;
    public Color OutlineColor;
    private Text MyText;
    private GameObject[] Clones;
    private GameObject OgUgClone;
    private GameObject ShadowCloneParent;
    private string textEcho;
    private int densityEcho;
    private float outlineDistanceEcho;
    private Color colorEcho;
    
    private void OnEnable()
    {
        EnableContent();
    }

    private void EnableContent()
    {
        CheckForMyText();

        if(!MyText)
            return;

        SetEchos();
    }

    void Update()
    {
        UpdateContent();

        if(MyText.text != textEcho)
        {
            CallForUpdate = true;
        }
        else if(Density != densityEcho)
        {
            CallForUpdate = true;
        }
        else if(!Mathf.Approximately(OutlineDistance, outlineDistanceEcho))
        {
            CallForUpdate = true;
        }
        else if(colorEcho != OutlineColor)
        {
            CallForUpdate = true;
        }

        SetEchos();
    }

    private void SetEchos()
    {
        textEcho = MyText.text;
        densityEcho = Density;
        outlineDistanceEcho = OutlineDistance;
        colorEcho = OutlineColor;
    }

    private void LateUpdate()
    {
        if(CallForUpdate)
        {               
            EnableContent();
            DestroyImmediate(ShadowCloneParent);

            CallForUpdate = false;
            DoOgUgClone = true;
            DrawOutline = true;
        }
    }

    private void UpdateContent()
    {
        if(DoOgUgClone)
        {
            DoOgUgClone = false;
            OgUgClone = GameObject.Instantiate(transform.gameObject);
        }

        if(DrawOutline)
        {
            DrawOutline = false;
            Clones = new GameObject[Density];
            SpawnCloneParent();
            SpawnClones();
        }
    }

    private void SpawnClones()
    {
        for(int i = 0; i < Density; i++)
        {
            SpawnClone(i);
        }
    }

    private void SpawnClone(int id)
    {
        if(!OgUgClone)
            return;

        if(!ShadowCloneParent)
            return;

        GameObject c = GameObject.Instantiate(OgUgClone);
        c.GetComponent<Text>().color = OutlineColor;
        c.transform.SetParent(ShadowCloneParent.transform);
        c.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        c.GetComponent<RectTransform>().rotation = transform.GetComponent<RectTransform>().rotation;
        c.name = id.ToString();
        Destroy(c.GetComponent<KamaraOutlinerScr>());
        Clones[id] = c;
        c.transform.position = ShadowCloneParent.transform.position + OuterEdgePoint(id);
    }

    private void CheckForMyText()
    {
        if(!MyText)
        {
            MyText = GetComponent<Text>();
        }
    }

    private void SpawnCloneParent()
    {
        if(!ShadowCloneParent)
        {
            ShadowCloneParent = new GameObject("ShadowCloneParentGob");
            ShadowCloneParent.transform.SetParent(transform.parent);
            int si = ShadowCloneParent.transform.GetSiblingIndex();
            ShadowCloneParent.transform.SetSiblingIndex(si - 1);
            ShadowCloneParent.transform.rotation = transform.rotation;
            ShadowCloneParent.transform.localScale = transform.localScale;
            ShadowCloneParent.transform.position = transform.position;
        }
    }

    private Vector3 OuterEdgePoint(int step)
    {
        float myAngle = (float)step * (360.0f / (float)Density) * Mathf.Deg2Rad;
        return transform.TransformDirection(new Vector3(Mathf.Cos(myAngle), Mathf.Sin(myAngle), 0f) * OutlineDistance);
    }
}
