using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;

public class DisplayUsabilityData : MonoBehaviour
{
    [Header("Line Colour Gradients")]
    [SerializeField] private Gradient m_computer1Colour;
    [SerializeField] private Gradient m_computer2Colour, m_computer3Colour, m_computer4Colour;
    [SerializeField, Space(20)] private Material m_lineMaterial;
    [SerializeField] private Material m_combatMaterial;
    [SerializeField] private Mesh m_cube;

    private List<string> comp1Types = new List<string>();
    private List<Vector3> comp1Pos = new List<Vector3>();
    private List<string> comp2Types = new List<string>();
    private List<Vector3> comp2Pos = new List<Vector3>();
    private List<string> comp3Types = new List<string>();
    private List<Vector3> comp3Pos = new List<Vector3>();
    private List<string> comp4Types = new List<string>();
    private List<Vector3> comp4Pos = new List<Vector3>();



    private static string returnCSV1;
    private static string returnCSV2;
    private static string returnCSV3;
    private static string returnCSV4;

    private Vector3 m_startPosition;

    // Update is called once per frame
    void Start()
    {
        InvokeRepeating("LoadComputer1Data",0,3);
        InvokeRepeating("LoadComputer2Data", 0,3);
        InvokeRepeating("LoadComputer3Data", 0, 3);
        InvokeRepeating("LoadComputer4Data", 0, 3);
    }

    void OnDrawGizmos()
    {
        Vector3 _previous = m_startPosition;
        foreach (Vector3 _pos in comp1Pos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(_pos, new Vector3(3, 3, 3));
        }
    }

    void LoadComputer1Data()
    {
        try
        {
            StartCoroutine(DownloadCSVCoroutine("1DQ_SnzBHfzTird2qjwDbvJIREqaF07XmuxDBdNoOMKM", 1));
            List<String> _seperatedCSV = new List<string>();
            _seperatedCSV = returnCSV1.Split(',').ToList();
            comp1Pos = new List<Vector3>();
            comp1Types = new List<string>();

            //find StartPos;
            for (int i = 0; i < _seperatedCSV.Count; i++)
            {
                if (_seperatedCSV[i] == "position")
                {
                    string xFixed = _seperatedCSV[i + 1].Replace("\"(", "");
                    string zFixed = _seperatedCSV[i + 3].Replace(")", "");
                    float x;
                    float.TryParse(xFixed, out x);
                    float y;
                    float.TryParse(_seperatedCSV[i + 2], out y);
                    float z;
                    float.TryParse(zFixed, out z);
                    m_startPosition = new Vector3(x, y, z);
                    break;
                }
            }

            for (int i = 0; i < _seperatedCSV.Count;)
            {
                if (_seperatedCSV[i] == "position" || _seperatedCSV[i] == "combat")
                {
                    string xFixed = _seperatedCSV[i + 1].Replace("\"(", "");
                    string zFixed = _seperatedCSV[i + 3].Replace(")", "");
                    float x;
                    float.TryParse(xFixed, out x);
                    float y;
                    float.TryParse(_seperatedCSV[i + 2], out y);
                    float z;
                    float.TryParse(zFixed, out z);




                    comp1Types.Add(_seperatedCSV[i]);
                    comp1Pos.Add(new Vector3(x, y, z));
                    i += 4;
                }
                else
                {
                    i += 1;
                }
            }
            ShowComputer1Data();
        }
        catch { Debug.LogWarning("Error Loading Computer 1 Data"); }
    }
    void ShowComputer1Data()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        GameObject _new = new GameObject();
        _new.transform.parent = this.transform;
        LineRenderer _currentLineRenderer = _new.AddComponent<LineRenderer>();
        List<Vector3> _currentLine = new List<Vector3>();
        _currentLine.Add(Vector3.zero);

        for (int i = 0; i < comp1Pos.Count; i++)
        {

            if (comp1Types[i] == "combat")
            {
                _new = new GameObject("CombatMarker");
                _new.transform.parent = this.transform;
                _new.transform.position = comp1Pos[i];
                _new.transform.localScale = new Vector3(20, 20, 20);
                MeshFilter _meshFilt = _new.AddComponent<MeshFilter>();
                MeshRenderer _meshRend = _new.AddComponent<MeshRenderer>();
                _meshFilt.mesh = m_cube;
                _meshRend.material = m_combatMaterial;
            }
            if(Vector3.Distance(comp1Pos[i],m_startPosition) < 10)
            {
                _currentLineRenderer.material = m_lineMaterial;
                _currentLineRenderer.colorGradient = m_computer1Colour;
                _currentLineRenderer.positionCount = _currentLine.Count();
                _currentLineRenderer.SetPositions(_currentLine.ToArray());
                _currentLine.Clear();
                _new = new GameObject(i.ToString());
                _new.transform.parent = this.transform;
                _new.AddComponent<LineRenderer>();
                _currentLineRenderer = _new.GetComponent<LineRenderer>();
                _currentLineRenderer.material = m_lineMaterial;
                _currentLineRenderer.colorGradient = m_computer1Colour;
                _currentLine.Add(comp1Pos[i]);
            }
            else
            {
                _currentLine.Add(comp1Pos[i]);
            }
            if (i == comp1Pos.Count-1)
            {
                _currentLineRenderer.positionCount = _currentLine.Count();
                _currentLineRenderer.SetPositions(_currentLine.ToArray());
            }
        }
    }
    void LoadComputer2Data()
    {
        try
        {
            StartCoroutine(DownloadCSVCoroutine("1DEK3zs54rDlG1K9eOUdx3LKpOr2qiYlNBNLyYG1FnX0", 2));
            List<String> _seperatedCSV = new List<string>();
            _seperatedCSV = returnCSV2.Split(',').ToList();
            comp2Pos = new List<Vector3>();
            comp2Types = new List<string>();

            for (int i = 0; i < _seperatedCSV.Count;)
            {
                if (_seperatedCSV[i] == "position" || _seperatedCSV[i] == "combat")
                {
                    string xFixed = _seperatedCSV[i + 1].Replace("\"(", "");
                    string zFixed = _seperatedCSV[i + 3].Replace(")", "");
                    float x;
                    float.TryParse(xFixed, out x);
                    float y;
                    float.TryParse(_seperatedCSV[i + 2], out y);
                    float z;
                    float.TryParse(zFixed, out z);




                    comp2Types.Add(_seperatedCSV[i]);
                    comp2Pos.Add(new Vector3(x, y, z));
                    i += 4;
                }
                else
                {
                    i += 1;
                }
            }
            ShowComputer2Data();
        }
        catch { Debug.LogWarning("Error Loading Computer 2 Data"); }
    }
    void ShowComputer2Data()
    {
        GameObject _new = new GameObject();
        _new.transform.parent = this.transform;
        LineRenderer _currentLineRenderer = _new.AddComponent<LineRenderer>();
        _currentLineRenderer.material = m_lineMaterial;
        _currentLineRenderer.colorGradient = m_computer2Colour;
        List<Vector3> _currentLine = new List<Vector3>();
        _currentLine.Add(Vector3.zero);

        for (int i = 0; i < comp2Pos.Count; i++)
        {

            if (comp2Types[i] == "combat")
            {
                _new = new GameObject("CombatMarker");
                _new.transform.parent = this.transform;
                _new.transform.position = comp2Pos[i];
                _new.transform.localScale = new Vector3(20, 20, 20);
                MeshFilter _meshFilt = _new.AddComponent<MeshFilter>();
                MeshRenderer _meshRend = _new.AddComponent<MeshRenderer>();
                _meshFilt.mesh = m_cube;
                _meshRend.material = m_combatMaterial;
            }
            if (Vector3.Distance(comp2Pos[i], m_startPosition) < 10)
            {
                _currentLineRenderer.material = m_lineMaterial;
                _currentLineRenderer.colorGradient = m_computer2Colour;
                _currentLineRenderer.positionCount = _currentLine.Count();
                _currentLineRenderer.SetPositions(_currentLine.ToArray());
                _currentLine.Clear();
                _new = new GameObject(i.ToString());
                _new.transform.parent = this.transform;
                _new.AddComponent<LineRenderer>();
                _currentLineRenderer = _new.GetComponent<LineRenderer>();
                _currentLineRenderer.material = m_lineMaterial;
                _currentLineRenderer.colorGradient = m_computer2Colour;
                _currentLine.Add(comp2Pos[i]);
            }
            else
            {
                _currentLine.Add(comp2Pos[i]);
            }
            if (i == comp2Pos.Count - 1)
            {
                _currentLineRenderer.positionCount = _currentLine.Count();
                _currentLineRenderer.SetPositions(_currentLine.ToArray());
            }
        }
    }

    void LoadComputer3Data()
    {
        try
        {
            StartCoroutine(DownloadCSVCoroutine("1wrQjNiztX9p05JZvF0tGi2cLVBx96bxYLpas9HOcTaE", 3));
            List<String> _seperatedCSV = new List<string>();
            _seperatedCSV = returnCSV3.Split(',').ToList();
            comp3Pos = new List<Vector3>();
            comp3Types = new List<string>();
            for (int i = 0; i < _seperatedCSV.Count;)
            {
                if (_seperatedCSV[i] == "position" || _seperatedCSV[i] == "combat")
                {
                    string xFixed = _seperatedCSV[i + 1].Replace("\"(", "");
                    string zFixed = _seperatedCSV[i + 3].Replace(")", "");
                    float x;
                    float.TryParse(xFixed, out x);
                    float y;
                    float.TryParse(_seperatedCSV[i + 2], out y);
                    float z;
                    float.TryParse(zFixed, out z);




                    comp3Types.Add(_seperatedCSV[i]);
                    comp3Pos.Add(new Vector3(x, y, z));
                    i += 4;
                }
                else
                {
                    i += 1;
                }
            }
            ShowComputer3Data();
        }
        catch { Debug.LogWarning("Error Loading Computer 3 Data"); }
    }
    void ShowComputer3Data()
    {
        GameObject _new = new GameObject();
        _new.transform.parent = this.transform;
        LineRenderer _currentLineRenderer = _new.AddComponent<LineRenderer>();
        _currentLineRenderer.material = m_lineMaterial;
        _currentLineRenderer.colorGradient = m_computer3Colour;
        List<Vector3> _currentLine = new List<Vector3>();
        _currentLine.Add(Vector3.zero);

        for (int i = 0; i < comp3Pos.Count; i++)
        {

            if (comp3Types[i] == "combat")
            {
                _new = new GameObject("CombatMarker");
                _new.transform.parent = this.transform;
                _new.transform.position = comp3Pos[i];
                _new.transform.localScale = new Vector3(20, 20, 20);
                MeshFilter _meshFilt = _new.AddComponent<MeshFilter>();
                MeshRenderer _meshRend = _new.AddComponent<MeshRenderer>();
                _meshFilt.mesh = m_cube;
                _meshRend.material = m_combatMaterial;
            }
            if (Vector3.Distance(comp3Pos[i], m_startPosition) < 10)
            {
                _currentLineRenderer.material = m_lineMaterial;
                _currentLineRenderer.colorGradient = m_computer3Colour;
                _currentLineRenderer.positionCount = _currentLine.Count();
                _currentLineRenderer.SetPositions(_currentLine.ToArray());
                _currentLine.Clear();
                _new = new GameObject(i.ToString());
                _new.transform.parent = this.transform;
                _new.AddComponent<LineRenderer>();
                _currentLineRenderer = _new.GetComponent<LineRenderer>();
                _currentLineRenderer.material = m_lineMaterial;
                _currentLineRenderer.colorGradient = m_computer3Colour;
                _currentLine.Add(comp3Pos[i]);
            }
            else
            {
                _currentLine.Add(comp3Pos[i]);
            }
            if (i == comp3Pos.Count - 1)
            {
                _currentLineRenderer.positionCount = _currentLine.Count();
                _currentLineRenderer.SetPositions(_currentLine.ToArray());
            }
        }
    }

    void LoadComputer4Data()
    {
        try
        {
            StartCoroutine(DownloadCSVCoroutine("1ZSkbxNEaUR9ronil0Fpbpv5glyaJ0MPzfrD5f4MbeXk", 4));
            List<String> _seperatedCSV = new List<string>();
            _seperatedCSV = returnCSV4.Split(',').ToList();
            comp4Pos = new List<Vector3>();
            comp4Types = new List<string>();
            for (int i = 0; i < _seperatedCSV.Count;)
            {
                if (_seperatedCSV[i] == "position" || _seperatedCSV[i] == "combat")
                {
                    string xFixed = _seperatedCSV[i + 1].Replace("\"(", "");
                    string zFixed = _seperatedCSV[i + 3].Replace(")", "");
                    float x;
                    float.TryParse(xFixed, out x);
                    float y;
                    float.TryParse(_seperatedCSV[i + 2], out y);
                    float z;
                    float.TryParse(zFixed, out z);




                    comp4Types.Add(_seperatedCSV[i]);
                    comp4Pos.Add(new Vector3(x, y, z));
                    i += 4;
                }
                else
                {
                    i += 1;
                }
            }
            ShowComputer4Data();
        }
        catch { Debug.LogWarning("Error Loading Computer 3 Data"); }
    }
    void ShowComputer4Data()
    {
        GameObject _new = new GameObject();
        _new.transform.parent = this.transform;
        LineRenderer _currentLineRenderer = _new.AddComponent<LineRenderer>();
        _currentLineRenderer.material = m_lineMaterial;
        _currentLineRenderer.colorGradient = m_computer4Colour;
        List<Vector3> _currentLine = new List<Vector3>();
        _currentLine.Add(Vector3.zero);

        for (int i = 0; i < comp4Pos.Count; i++)
        {

            if (comp4Types[i] == "combat")
            {
                _new = new GameObject("CombatMarker");
                _new.transform.parent = this.transform;
                _new.transform.position = comp4Pos[i];
                _new.transform.localScale = new Vector3(20, 20, 20);
                MeshFilter _meshFilt = _new.AddComponent<MeshFilter>();
                MeshRenderer _meshRend = _new.AddComponent<MeshRenderer>();
                _meshFilt.mesh = m_cube;
                _meshRend.material = m_combatMaterial;
            }
            if (Vector3.Distance(comp4Pos[i], m_startPosition) < 10)
            {
                _currentLineRenderer.material = m_lineMaterial;
                _currentLineRenderer.colorGradient = m_computer4Colour;
                _currentLineRenderer.positionCount = _currentLine.Count();
                _currentLineRenderer.SetPositions(_currentLine.ToArray());
                _currentLine.Clear();
                _new = new GameObject(i.ToString());
                _new.transform.parent = this.transform;
                _new.AddComponent<LineRenderer>();
                _currentLineRenderer = _new.GetComponent<LineRenderer>();
                _currentLineRenderer.material = m_lineMaterial;
                _currentLineRenderer.colorGradient = m_computer4Colour;
                _currentLine.Add(comp4Pos[i]);
            }
            else
            {
                _currentLine.Add(comp4Pos[i]);
            }
            if (i == comp4Pos.Count - 1)
            {
                _currentLineRenderer.positionCount = _currentLine.Count();
                _currentLineRenderer.SetPositions(_currentLine.ToArray());
            }
        }
    }
    #region CSVParser
    public static IEnumerator DownloadCSVCoroutine(string docId,int computerID)
    {
        string url =
            "https://docs.google.com/spreadsheets/d/" + docId + "/export?format=csv";

        WWWForm form = new WWWForm();
        WWW download = new WWW(url, form);

        yield return download;

        if (!string.IsNullOrEmpty(download.error))
        {
            Debug.Log("Error downloading: " + download.error);
        }
        else
        {
            switch (computerID)
            {
                case 1:
                    returnCSV1 = download.text;
                    break;
                case 2:
                    returnCSV2 = download.text;
                    break;
                case 3:
                    returnCSV3 = download.text;
                    break;
                case 4:
                    returnCSV4 = download.text;
                    break;
            }
        }
    }

    //CSV reader from https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/

    public static readonly string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    public static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    public static readonly char[] TRIM_CHARS = { '\"' };

    public static string DecodeSpecialCharsFromCSV(string value)
    {
        value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "").Replace("<br>", "\n").Replace("<c>", ",");
        return value;
    }
    #endregion
}