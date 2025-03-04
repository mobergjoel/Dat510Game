using UnityEngine;

public class FootstepSurfaceChanger : MonoBehaviour
{
    public AudioSource footstepSource;
    public AudioClip defaultFootstep;

    [System.Serializable]
    public class SurfaceFootstep
    {
        public LayerMask groundLayer; // Assign different layers for different surfaces
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
                    footstepSource.clip = surface.footstepSound;
                    return;
                }
            }
        }

        footstepSource.clip = defaultFootstep;
    }
}
