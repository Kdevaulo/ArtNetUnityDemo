using UnityEngine;

namespace UnityArtNetDemo.Painters
{
    [AddComponentMenu(nameof(StripPainter) + " in " + nameof(Painters))]
    public class StripPainter : MonoBehaviour
    {
        public int DiodesCount => _diodeColorizers.Length;

        [SerializeField] private DiodePainter[] _diodeColorizers;

        public void FillStrip(Color32[] colors)
        {
            for (var i = 0; i < _diodeColorizers.Length; i++)
            {
                var colorizer = _diodeColorizers[i];
                colorizer.SetColor(colors[i]);
            }
        }

        public void SetDiodePainters(DiodePainter[] painters)
        {
            _diodeColorizers = painters;
        }
    }
}