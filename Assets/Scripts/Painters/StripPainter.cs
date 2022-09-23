using UnityEngine;

namespace Painters
{
    public class StripPainter : MonoBehaviour
    {
        public int DiodsCount => _diodColorizers.Length;

        [SerializeField] private DiodPainter[] _diodColorizers;

        public void FillStrip(Color32[] colors)
        {
            for (var i = 0; i < _diodColorizers.Length; i++)
            {
                var colorizer = _diodColorizers[i];
                colorizer.SetColor(colors[i]);
            }
        }
    }
}