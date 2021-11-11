using System.Collections;
using UnityEngine;

public class WorldLightController : MonoBehaviour
{
    private Coroutine _lightning;
    
    public void UpdateTime(float time)
    {
        var newColor = _lightning == null ? WorldLightColor.Evaluate(time) : Color.white;
        Shader.SetGlobalColor("_DayNightColor", newColor);
    }

    public void TriggerLightning()
    {
        _lightning = StartCoroutine(DoLightningRoutine());
    }
    
    private IEnumerator DoLightningRoutine()
    {
        yield return new WaitForSeconds(0.033f);
        _lightning = null;
    }

    private void OnDestroy()
    {
        Shader.SetGlobalColor("_DayNightColor", Color.white);
    }
}
