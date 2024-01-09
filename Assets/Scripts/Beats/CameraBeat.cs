using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBeat : Beat
{
    public float scaling = .9f;
    private Camera cameraRef;
    private float originalScale;
    private void Awake()
    {
        cameraRef = GetComponent<Camera>();
        if (cameraRef == null)
        {
            Destroy(this);
        }
        originalScale = cameraRef.fieldOfView;
    }
    public override void OnBeat()
    {
        StartCoroutine(CameraZoom());
    }

    public IEnumerator CameraZoom()
    {
        cameraRef.fieldOfView = cameraRef.fieldOfView * scaling;
        yield return new WaitForSeconds(.1f);
        cameraRef.fieldOfView = originalScale;
    }
}
