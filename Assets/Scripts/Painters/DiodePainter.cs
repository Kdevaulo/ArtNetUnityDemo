using System;

using UnityEngine;

namespace UnityArtNetDemo.Painters
{
    [Serializable, AddComponentMenu(nameof(DiodePainter) + " in " + nameof(Painters))]
    public class DiodePainter : MonoBehaviour
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