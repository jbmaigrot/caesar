using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
    private RectTransform rectComponent;
    private float rotateSpeed = 300f;

#if CLIENT
    private void Start()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		rectComponent = GetComponent<RectTransform>();
    }

    private void Update()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		rectComponent.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
#endif
}