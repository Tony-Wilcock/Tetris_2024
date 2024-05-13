using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] private bool canRotate = true;

    [SerializeField] private Vector3 queueOffset;

    [SerializeField] private float queuedShapeScale = 0.5f;

    [SerializeField] private LandSquareFXTag[] landSquareFX;
    [SerializeField] private TrailRenderer[] trails;

    public Gradient gradient = new Gradient();

    [System.Obsolete]
    private void Start()
    {
        landSquareFX = FindObjectsOfType<LandSquareFXTag>();
        foreach (TrailRenderer trail in trails)
        {
            trail.emitting = false;
        }
    }

    public Vector3 GetQueueOffset()
    {
        return queueOffset;
    }

    public float GetQueueScale()
    {
        return queuedShapeScale;
    }

    public void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;
    }

    public void MoveToLastPosition(Vector3 lastPos)
    {
        transform.position = lastPos;
    }

    public void FastRotate(float value)
    {
        if (canRotate) transform.Rotate(0, 0, value);
    }

    public void Rotate(float value)
    {
        if (canRotate)
        {
            transform.DORotate(transform.eulerAngles + new Vector3(0, 0, value), 0.1f);
        }
    }

    public void LandShapeFX()
    {
        int i = 0;

        foreach (Transform child in gameObject.transform)
        {
            landSquareFX[i].transform.position = new Vector3(child.position.x, child.position.y, -2f);

            ParticlePlayer particlePlayer = landSquareFX[i].GetComponent<ParticlePlayer>();

            if (particlePlayer) particlePlayer.PlayLandSquareParticles();

            i++;
        }
    }

    public void SetTrailActive()
    {
        foreach (TrailRenderer trail in trails)
        {
            trail.emitting = true;
        }
    }

    public IEnumerator SetTrailInActive()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (TrailRenderer trail in trails)
        {
            trail.emitting = false;
        }
    }
}
