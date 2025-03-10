using UnityEngine;

public class FootstepSurfaceChanger : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip defaultFootstep;

    [System.Serializable]
    public class SurfaceFootstep
    {
        public LayerMask groundLayer;
        public AudioClip footstepSound;
    }

    public SurfaceFootstep[] surfaceFootsteps;

    private void Update()
    {
        ChangeFootstepSound();
    }

    private void ChangeFootstepSound()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            int hitLayer = hit.collider.gameObject.layer;

            foreach (SurfaceFootstep surface in surfaceFootsteps)
            {
                if (surface.groundLayer == (surface.groundLayer | (1 << hitLayer)))
                {
                    if (footstepSource.clip != surface.footstepSound)
                    {
                        footstepSource.clip = surface.footstepSound;
                    }
                    return;
                }
            }
        }
        if (footstepSource.clip != defaultFootstep)
            {
                footstepSource.clip = defaultFootstep;
            }
    }
    public void PlayFootstepSound()
    {
        if (!footstepSource.isPlaying)
        {
            footstepSource.Play();
        }
    }

    public void StopFootstepSound()
    {
        footstepSource.Stop();
    }

}
