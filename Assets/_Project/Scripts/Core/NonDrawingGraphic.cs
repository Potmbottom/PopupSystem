using UnityEngine.UI;

namespace PopupShowcase.Core
{
    public class NonDrawingGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
