using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dissolve_Modify : MonoBehaviour
{
    [SerializeField] private float _dissolveTime = 0.75f;
    [SerializeField] private List<Image> _image;
    [SerializeField] private List<Transform> transforms;

    public Material material_Effect;

    private List<Material> _materials = new List<Material>();

    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private int _verticalDissolveAmount = Shader.PropertyToID("_VerticalDissolve");

    private void Awake()
    {
        // 머티리얼 초기화 (한 번만)
        if (_image != null && material_Effect != null)
        {
            foreach (var img in _image)
            {
                var mat = new Material(material_Effect);
                img.material = mat;
                _materials.Add(mat);
            }
        }
    }

    public IEnumerator Vanish(bool useDissolve, bool useVertical)
    {
        // 시작 상태 강제 보이게
        if (useDissolve)
            SetDissolve(0f);
        if (useVertical)
            SetVerticalDissolve(0f);

        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0f, 1.1f, elapsedTime / _dissolveTime);
            float lerpedVerticalDissolve = Mathf.Lerp(0f, 1.2f, elapsedTime / _dissolveTime);

            if (useDissolve)
                SetDissolve(lerpedDissolve);

            if (useVertical)
            {
                SetVerticalDissolve(lerpedVerticalDissolve);

                if (lerpedVerticalDissolve >= 0.1f)
                    SetTransformsActive(false);
            }

            yield return null;
        }

        UIManager.Instance.TogglePanel(gameObject);
    }

    public IEnumerator Appear(bool useDissolve, bool useVertical)
    {
        UIManager.Instance.TogglePanel(gameObject);

        // 시작 상태 강제 안보이게
        if (useDissolve)
            SetDissolve(1.1f);
        if (useVertical)
        {
            SetVerticalDissolve(1.2f);
            SetTransformsActive(false);
        }

        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(1.1f, 0f, elapsedTime / _dissolveTime);
            float lerpedVerticalDissolve = Mathf.Lerp(1.2f, 0f, elapsedTime / _dissolveTime);

            if (useDissolve)
                SetDissolve(lerpedDissolve);

            if (useVertical)
            {
                SetVerticalDissolve(lerpedVerticalDissolve);

                if (lerpedVerticalDissolve <= 0.1f)
                    SetTransformsActive(true);
            }

            yield return null;
        }
    }

    private void SetDissolve(float value)
    {
        foreach (var mat in _materials)
            mat.SetFloat(_dissolveAmount, value);
    }

    private void SetVerticalDissolve(float value)
    {
        foreach (var mat in _materials)
            mat.SetFloat(_verticalDissolveAmount, value);
    }

    private void SetTransformsActive(bool active)
    {
        foreach (var t in transforms)
            t.gameObject.SetActive(active);
    }
}
