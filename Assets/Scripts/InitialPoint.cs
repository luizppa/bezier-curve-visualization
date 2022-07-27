using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialPoint : Point
{
  private bool pointDragged = false;
  private int index;

  // Start is called before the first frame update
  new void Start()
  {
    base.Start();
  }

  // Update is called once per frame
  new void Update()
  {
    ManagePosition();
    base.Update();
    // spriteRenderer.color = Color.red;
  }

  protected override void ControlGraphics()
  {
    base.ControlGraphics();
    spriteRenderer.enabled = true;
  }

  protected override void ManageSelf()
  {
    // noop
  }

  void ManagePosition()
  {
    if (pointDragged)
    {
      Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
      curveManager.ResetCurve();
    }
  }

  public void SetIndex(int index)
  {
    this.index = index;
  }

  public void InsertPoint(InsertPosition position)
  {
    int index = position == InsertPosition.Before ? this.index : this.index + 1;
    curveManager.InsertInitialPoint(index);
  }

  void OnMouseDown()
  {
    pointDragged = true;
  }

  void OnMouseUp()
  {
    pointDragged = false;
  }
}
