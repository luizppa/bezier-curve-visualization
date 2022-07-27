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
    lineRenderer.positionCount = 2;
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  protected virtual void Update()
  {
    ControlGraphics();
    if (ShouldDrawChild())
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

  private void OnDestroy()
  {
    RemoveChild();
  }

  protected bool ShouldDrawChild()
  {
    return curveManager.GetMinDrawLevel() < pointsAtLevel;
  }

  protected virtual void ControlGraphics()
  {
    lineRenderer.enabled = curveManager.GetShowLines() && ShouldDrawChild();
    spriteRenderer.enabled = curveManager.GetShowDots() || (ShouldDrawChild() && pointsAtLevel == 1);
  }

  private void ManageLine()
  {
    if (nextPoint != null && curveManager.GetShowLines())
    {
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
      if (nextPoint.GetChildPoint() != null)
      {
        childPoint.SetNextPoint(nextPoint.GetChildPoint());
      }
      childPoint.transform.position = Vector3.Lerp(transform.position, nextPoint.transform.position, curveManager.GetLerpState());
    }
  }

  protected virtual void ManageSelf()
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
      lineRenderer.positionCount = 0;
    }
  }

  private void Remove()
  {
    RemoveChild();
    Destroy(gameObject);
  }

  public void Configure(int pointsAtLevel)
  {
    this.pointsAtLevel = pointsAtLevel;
    if (childPoint != null)
    {
      childPoint.Configure(pointsAtLevel - 1);
    }
  }

  // @region Getters

  public Point GetChildPoint()
  {
    return childPoint;
  }

  // @endregion

  // @region Setters

  public void SetNextPoint(Point nextPoint)
  {
    this.nextPoint = nextPoint;
  }

  // @endregion
}
