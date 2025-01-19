using System.Collections;

using UnityEngine;

public class ColoredFlash : MonoBehaviour
{
    #region Datamembers

    #region Editor Settings

    [Tooltip("Material to switch to during the flash.")]
    [SerializeField] private Material flashMaterial;

    [Tooltip("Duration of the flash.")]
    [SerializeField] private float duration;

    #endregion
    #region Private Fields

    // The SpriteRenderer that should flash.
    private SpriteRenderer spriteRenderer;

    private Material originalMaterial;

    private Coroutine flashRoutine;

    #endregion

    #endregion


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalMaterial = spriteRenderer.material;

        flashMaterial = new Material(flashMaterial); // Copy to avoid side effect
    }

    public void Flash(Color color)
    {
        if (flashRoutine != null)
        {
            // Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine(color));
    }

    private IEnumerator FlashRoutine(Color color)
    {
        // Swap to the flashMaterial.
        spriteRenderer.material = flashMaterial;

        // Set the desired color for the flash.
        flashMaterial.color = color;

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        // After the pause, swap back to the original material.
        spriteRenderer.material = originalMaterial;

        // Set the flashRoutine to null, signaling that it's finished.
        flashRoutine = null;
    }
}