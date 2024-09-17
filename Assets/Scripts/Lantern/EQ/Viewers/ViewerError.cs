using TMPro;
using UnityEngine;

public class ViewerError : MonoBehaviour
{
    [SerializeField]
    private GameObject _error;

    [SerializeField]
    private TextMeshProUGUI _text;

    public void ShowError(string errorMessage)
    {
        _error.SetActive(true);
        _text.text = errorMessage;
    }
}
