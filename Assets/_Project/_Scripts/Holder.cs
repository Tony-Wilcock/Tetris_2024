using UnityEngine;

public class Holder : MonoBehaviour
{
    [SerializeField] private Transform holderPosition;
    [SerializeField] private Shape heldShape = null;

    public bool canRelease = false;

    public void Catch(Shape shape)
    {
        if (heldShape || !shape)
        {
            return;
        }

        if (holderPosition)
        {
            shape.transform.position = holderPosition.position + shape.GetQueueOffset();
            shape.transform.rotation = holderPosition.rotation;
            shape.transform.localScale = new Vector3(shape.GetQueueScale(), shape.GetQueueScale(), shape.GetQueueScale());
            heldShape = shape;
        }
        else
        {
            return;
        }
    }

    public Shape Release()
    {
        heldShape.transform.localScale = Vector3.one;
        Shape tempShape = heldShape;
        heldShape = null;
        canRelease = false;
        return tempShape;
    }

    public void ResetHolder()
    {
        if (heldShape) Destroy(heldShape.gameObject);
    }

    public Shape HeldShape()
    {
        return heldShape;
    }
}
