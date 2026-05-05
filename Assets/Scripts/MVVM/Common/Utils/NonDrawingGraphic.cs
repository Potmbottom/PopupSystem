using UnityEngine.UI;

namespace PopupShowcase.MVVM.Common
{
    public class NonDrawingGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
