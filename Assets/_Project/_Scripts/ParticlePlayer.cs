using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] landSquareParticles;

    private void Start()
    {
        if (landSquareParticles == null) landSquareParticles = GetComponentsInChildren<ParticleSystem>();
    }

    public void PlayLandSquareParticles()
    {
        foreach (ParticleSystem particleSystem in landSquareParticles)
        {
            particleSystem.Stop();
            particleSystem.Play();
        }
    }
}
