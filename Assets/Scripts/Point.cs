using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
  protected int pointsAtLevel = 0;
  protected Point nextPoint = null;
  protected Point childPoint = null;
  protected LineRenderer lineRenderer = null;
  protected SpriteRenderer spriteRenderer = null;
  protected CurveManager curveManager = null;

  protected virtual void Start()
  {
    curveManager = FindObjectOfType<CurveManager>();
    lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.SetWidth(0.07f, 0.07f);
    lineRenderer.SetVertexCount(2);
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  protected virtual void Update()
  {
    if (curveManager.GetMinDrawLevel() < pointsAtLevel)
    {
      ManageLine();
      ManageChildPoint();
      ManageSelf();
    }
    else
    {
      RemoveChild();
    }
  }

  private void ManageLine()
  {
    if (nextPoint != null)
    {
      lineRenderer.enabled = true;
      lineRenderer.positionCount = 2;
      lineRenderer.SetPosition(0, transform.position);
      lineRenderer.SetPosition(1, nextPoint.transform.position);
    }
  }

  private void ManageChildPoint()
  {
    if (nextPoint != null)
    {
      if (childPoint == null)
      {
        childPoint = curveManager.CreatePoint();
        childPoint.Configure(pointsAtLevel - 1);
      }
      if (nextPoint.GteChildPoint() != null)
      {
        childPoint.SetNextPoint(nextPoint.GteChildPoint());
      }
      childPoint.transform.position = Vector3.Lerp(transform.position, nextPoint.transform.position, curveManager.GetLerpState());
    }
  }

  private void ManageSelf()
  {
    if (pointsAtLevel == 1)
    {
      if (curveManager.GetCurveIndicator() != this)
      {
        curveManager.SetCurveIndicator(this);
      }
      spriteRenderer.color = Color.blue;
    }
    else
    {
      spriteRenderer.color = Color.black;
    }
  }

  private void RemoveChild()
  {
    if (childPoint != null)
    {
      childPoint.Remove();
      childPoint = null;
      lineRenderer.SetVertexCount(0);
    }
  }

  private void Remove()
  {
    RemoveChild();
    Destroy(gameObject);
  }

  public void SetNextPoint(Point nextPoint)
  {
    this.nextPoint = nextPoint;
  }

  public Point GteChildPoint()
  {
    return childPoint;
  }

  public void Configure(int pointsAtLevel)
  {
    this.pointsAtLevel = pointsAtLevel;
    if (childPoint != null)
    {
      childPoint.Configure(pointsAtLevel - 1);
    }
  }

  private void OnDestroy()
  {
    RemoveChild();
  }
}
