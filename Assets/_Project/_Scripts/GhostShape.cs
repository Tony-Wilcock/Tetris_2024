using UnityEngine;

public class GhostShape : MonoBehaviour
{
    [SerializeField] private Shape ghostShape;
    [SerializeField] private Color ghostColour = new Color(1f, 1f, 1f, 0.2f);

    private bool hitBottom = false;

    public void DrawShape(Shape originalShape, Board gameBoard)
    {
        if (!ghostShape)
        {
            ghostShape = Instantiate(originalShape, originalShape.transform.position, originalShape.transform.rotation) as Shape;
            ghostShape.gameObject.name = originalShape.gameObject.name + " Ghost Shape";
            ghostShape.gameObject.transform.parent = transform;

            SpriteRenderer[] allRenderers = ghostShape.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer r in allRenderers)
            {
                r.color = ghostColour;
            }
        }
        else
        {
            ghostShape.transform.position = originalShape.transform.position;
            ghostShape.transform.rotation = originalShape.transform.rotation;
        }

        hitBottom = false;

        while (!hitBottom)
        {
            ghostShape.Move(Vector3.down);

            if (!gameBoard.IsValidPosition(ghostShape))
            {
                ghostShape.Move(Vector3.up);
                hitBottom = true;
            }
        }
    }

    public void ResetGhost()
    {
        Destroy(ghostShape.gameObject);
    }
}
