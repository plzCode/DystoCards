using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private float _dissolveTime = 0.75f;

    [SerializeField] private List<SpriteRenderer> _spriteRenderers;
    private List<Material> _materials;

    [SerializeField] private Transform _name;
    [SerializeField] private Transform _stats;

    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private int _verticalDissolveAmount = Shader.PropertyToID("_VerticalDissolve");

    private void Start()
    {
        //_spriteRenderers = GetComponentInChildren<SpriteRenderer>();
        _materials = new List<Material>();

        if (_spriteRenderers != null)
        {
            foreach (var spriteRenderer in _spriteRenderers)
            {
                _materials.Add(spriteRenderer.material);
            }
        }

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(Vanish(true, true));
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(Appear(true, true));
        }
    }


    public IEnumerator Vanish(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0f, 1.1f, (elapsedTime / _dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(0f, 1.1f, (elapsedTime / _dissolveTime));

            if (useDissolve)
            {
                for (int i = 0; i < _materials.Count; i++)
                {
                    _materials[i].SetFloat(_dissolveAmount, lerpedDissolve);                    
                }
            }

            if (useVertical)
            {
                for (int i = 0; i < _materials.Count; i++)
                {
                    _materials[i].SetFloat(_verticalDissolveAmount, lerpedVerticalDissolve);
                    if (lerpedVerticalDissolve >= 0.4f)
                    {
                        if (_name.gameObject.activeSelf)
                        {
                            _name.gameObject.SetActive(false);
                            _stats.gameObject.SetActive(false);
                        }
                    }
                }
            }

            yield return null;
        }


    }


    public IEnumerator Appear(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(1.1f, 0f, (elapsedTime / _dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(1.1f, 0f, (elapsedTime / _dissolveTime));

            if (useDissolve)
            {
                for (int i = 0; i < _materials.Count; i++)
                {
                    _materials[i].SetFloat(_dissolveAmount, lerpedDissolve);
                }
            }

            if (useVertical)
            {
                for (int i = 0; i < _materials.Count; i++)
                {
                    _materials[i].SetFloat(_verticalDissolveAmount, lerpedVerticalDissolve);
                    if (lerpedVerticalDissolve >= 0.4f)
                    {
                        if(!_name.gameObject.activeSelf)
                        {
                            _name.gameObject.SetActive(true);
                            _stats.gameObject.SetActive(true);
                        }
                    }
                }

                yield return null;
            }


        }
    }
}




