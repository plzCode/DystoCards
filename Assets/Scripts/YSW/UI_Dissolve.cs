using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dissolve : MonoBehaviour
{
    [SerializeField] private float _dissolveTime = 0.75f;

    [SerializeField] private List<Image> _image;
    [SerializeField] private List<Material> _materials;

    public Material material_Effect;

    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private int _verticalDissolveAmount = Shader.PropertyToID("_VerticalDissolve");

    [SerializeField] private List<Transform> transforms;

    private void Start()
    {   
        _materials = new List<Material>();

        if (_image != null && material_Effect != null)
        {
            foreach (var image in _image)
            {
                image.material = material_Effect;
                _materials.Add(image.material);
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
            float lerpedVerticalDissolve = Mathf.Lerp(0f, 1.2f, (elapsedTime / _dissolveTime));

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
                        //Àý¹ÝÂë µÉ ¶§ ²¨µÑ Transform
                        foreach(var obj in transforms)
                        {
                            obj.gameObject.SetActive(false);
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
                        foreach(var obj in transforms)
                        {
                            //Àý¹Ý Âë µÉ ‹š ³ªÅ¸³¾ Transform
                            obj.gameObject.SetActive(true);
                        }
                    }
                }

                yield return null;
            }

            
        }
    }
}




