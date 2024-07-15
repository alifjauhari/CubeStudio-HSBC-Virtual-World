using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private Toggle wallToggle;
    [SerializeField] private Toggle viewModeToggle;
    [SerializeField] private Toggle editorModeToggle;
    [SerializeField] private GameObject wallObject;
    [SerializeField] private GameObject linesContainer;

    public UnityEvent onViewMode;
    public UnityEvent onBuildMode;

    private void Start()
    {
        StartCoroutine(FindLinesContainer());

        if (wallToggle != null && wallObject != null)
        {
            wallToggle.isOn = true;
            wallObject.SetActive(true);
            wallToggle.onValueChanged.AddListener(OnWallToggleValueChanged);
        }

        if (viewModeToggle != null && editorModeToggle != null)
        {
            viewModeToggle.onValueChanged.AddListener(OnViewModeToggleValueChanged);
            editorModeToggle.onValueChanged.AddListener(OnEditorModeToggleValueChanged);
        }
    }

    private IEnumerator FindLinesContainer()
    {
        while (linesContainer == null)
        {
            GameObject foundObject = GameObject.Find("LinesContainer");
            if (foundObject != null)
            {
                linesContainer = foundObject;
                Debug.Log("LinesContainer found!");
                break;
            }

            Debug.Log("LinesContainer not found, trying again in 2 seconds...");
            yield return new WaitForSeconds(2f);
        }
    }

    private void OnWallToggleValueChanged(bool isOn)
    {
        if (wallObject != null)
        {
            wallObject.SetActive(isOn);
        }
    }

    private void OnViewModeToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            editorModeToggle.isOn = false;
            onViewMode.Invoke();
            linesContainer.SetActive(false);
        }
        else
        {
            editorModeToggle.isOn = true;
        }
    }

    private void OnEditorModeToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            viewModeToggle.isOn = false;
            onBuildMode.Invoke();
            linesContainer.SetActive(true);
        }
        else
        {
            viewModeToggle.isOn = true;
        }
    }
}
