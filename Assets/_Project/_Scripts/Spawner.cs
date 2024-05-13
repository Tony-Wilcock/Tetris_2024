using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Shape[] shapes;
    [SerializeField] private Transform[] queuedTransforms = new Transform[3];

    private Shape[] queuedShapes = new Shape[3];

    private void Awake()
    {
        InitQueue();
    }

    private void Start()
    {
        FillQueue();
    }

    private Shape GetRandomShape()
    {
        int i = Random.Range(0, shapes.Length);

        if (shapes[i]) return shapes[i];

        Debug.Log("WARNING! Invalid Shape in Spawner!!!");
        return null;
    }

    public Shape SpawnShape()
    {
        Shape shape = null;

        shape = GetQueuedShape();
        shape.transform.position = transform.position;
        shape.transform.localScale = Vector3.one;

        if (shape) return shape;

        Debug.Log("WARNING! Invalid Shape in Spawner!!!");
        return null;
    }

    private void InitQueue()
    {
        for (int i = 0; i < queuedShapes.Length; i++)
        {
            queuedShapes[i] = null;
        }
    }

    private void FillQueue()
    {
        for (int i = 0; i < queuedShapes.Length; i++)
        {
            if (!queuedShapes[i])
            {
                queuedShapes[i] = Instantiate(GetRandomShape(), transform.position, Quaternion.identity);
                queuedShapes[i].transform.position = queuedTransforms[i].position + queuedShapes[i].GetQueueOffset();
                queuedShapes[i].transform.localScale = new Vector3(queuedShapes[i].GetQueueScale(), queuedShapes[i].GetQueueScale(), queuedShapes[i].GetQueueScale());
            }
        }
    }

    public void ResetQueue()
    {
        foreach (Shape shape in queuedShapes)
        {
            Destroy(shape.gameObject);
        }
        InitQueue();
        FillQueue();
    }

    public Shape GetQueuedShape()
    {
        Shape firstShape = null;

        if (queuedShapes[0])
        {
            firstShape = queuedShapes[0];
        }

        for (int i = 1; i < queuedShapes.Length; i++)
        {
            queuedShapes[i - 1] = queuedShapes[i];
            queuedShapes[i - 1].transform.position = queuedTransforms[i - 1].position + queuedShapes[i].GetQueueOffset();
        }

        queuedShapes[queuedShapes.Length - 1] = null;

        FillQueue();

        return firstShape;
    }
}
