using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveManager : MonoBehaviour
{

  private LineRenderer lineRenderer = null;
  private List<Vector3> curvePoints = new List<Vector3>();

  private float lerpState = 0f;
  private int direction = 1;

  private int minDrawLevel = 0;
  private bool startedAnimating = true;
  private bool finishedAnimating = false;

  private Point curveIndicator = null;
  private bool needToDrawCurve = false;
  private bool drawingCurve = false;

  private bool showLines = true;
  private bool showDots = true;


  [SerializeField] List<InitialPoint> initialPoints = new List<InitialPoint>();
  [SerializeField] Material curveMaterial = null;
  [SerializeField] GameObject pointPrefab;
  [SerializeField] GameObject initialPointPrefab;
  [SerializeField] float lerpSpeed = 0.5f;


  void Start()
  {
    DontDestroyOnLoad(this);
    if (FindObjectsOfType(GetType()).Length > 1)
    {
      Destroy(gameObject);
    }
    lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.SetWidth(0.07f, 0.07f);
    lineRenderer.material = curveMaterial;

    minDrawLevel = initialPoints.Count;
    AssignPoints();
  }

  void Update()
  {
    DrawCurve();
    CalculateLerp();
  }

  void CalculateLerp()
  {
    if (minDrawLevel < initialPoints.Count)
    {
      startedAnimating = false;
      finishedAnimating = false;
      if (lerpState > 1f)
      {
        direction = -1;
        finishedAnimating = true;
      }
      else if (lerpState < 0f)
      {
        direction = 1;
        startedAnimating = true;
      }
      lerpState += lerpSpeed * direction * Time.deltaTime;
    }
  }

  private void DrawCurve()
  {
    if (needToDrawCurve)
    {
      if (!drawingCurve && startedAnimating)
      {
        drawingCurve = true;
      }
      if (drawingCurve && finishedAnimating)
      {
        drawingCurve = false;
        needToDrawCurve = false;
      }
      if (drawingCurve)
      {
        Vector3 newPoint = Vector3.zero + curveIndicator.transform.position;
        curvePoints.Add(newPoint);
        lineRenderer.positionCount = curvePoints.Count;
        lineRenderer.SetPosition(curvePoints.Count - 1, newPoint);
      }
    }
  }

  void AssignPoints()
  {
    for (int i = 0; i < initialPoints.Count; i++)
    {
      if (i < initialPoints.Count - 1)
      {
        initialPoints[i].SetNextPoint(initialPoints[i + 1]);
      }
      initialPoints[i].Configure(initialPoints.Count);
      initialPoints[i].SetIndex(i);
    }
  }

  public void InsertInitialPoint(int index)
  {
    initialPoints.Insert(index, CreateInitialPoint());
    minDrawLevel += 1;
    curveIndicator = null;
    ResetCurve();
    AssignPoints();
  }

  public InitialPoint CreateInitialPoint()
  {
    GameObject newPoint = Instantiate(initialPointPrefab);
    return newPoint.GetComponent<InitialPoint>();
  }

  public Point CreatePoint()
  {
    GameObject newPoint = Instantiate(pointPrefab);
    return newPoint.GetComponent<Point>();
  }

  public void AdvanceAnimation()
  {
    if (minDrawLevel > 0)
    {
      minDrawLevel--;
    }
  }

  public Point GetCurveIndicator()
  {
    return curveIndicator;
  }

  public void ResetCurve()
  {
    EraseCurve();
    drawingCurve = false;
    if (curveIndicator != null)
    {
      needToDrawCurve = true;
    }
    else
    {
      needToDrawCurve = false;
    }
  }

  private void EraseCurve()
  {
    curvePoints.Clear();
    lineRenderer.positionCount = 0;
  }

  public void ResetAnimation()
  {
    ResetCurve();
    curveIndicator = null;
    lerpState = 0f;
    direction = 1;
    startedAnimating = true;
    finishedAnimating = false;
    needToDrawCurve = false;
    drawingCurve = false;

    while (initialPoints.Count > 0)
    {
      Point removedPoint = initialPoints[initialPoints.Count - 1];
      initialPoints.RemoveAt(initialPoints.Count - 1);
      Destroy(removedPoint.gameObject);
    }
    InsertInitialPoint(0);
    initialPoints[0].transform.position = new Vector3(-4f, 0f, 0f);
    InsertInitialPoint(1);
    initialPoints[1].transform.position = new Vector3(4f, 0f, 0f);
    minDrawLevel = initialPoints.Count;
    AssignPoints();
  }

  // @region Getters

  public float GetLerpState()
  {
    return lerpState;
  }

  public int GetMinDrawLevel()
  {
    return minDrawLevel;
  }

  public bool GetShowLines(){
    return showLines;
  }

  public bool GetShowDots(){
    return showDots;
  }

  // @endregion

  // @region Setters

  public void SetCurveIndicator(Point curveIndicator)
  {
    this.curveIndicator = curveIndicator;
    this.curvePoints.Clear();
    needToDrawCurve = true;
  }

  public void SetShowLines(bool show){
    showLines = show;
  }

  public void SetShowDots(bool show){
    showDots = show;
  }

  // @endregion
}
