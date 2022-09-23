using System;

using UnityEngine;

namespace Painters
{
    [Serializable]
    public class DiodPainter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;

        [SerializeField] private Light _light;

        public void SetColor(Color32 color)
        {
            _renderer.color = color;
            _light.color = color;
        }
    }
}