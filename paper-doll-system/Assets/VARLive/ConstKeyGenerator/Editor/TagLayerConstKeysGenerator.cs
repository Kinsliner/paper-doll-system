using System.Linq;

namespace VARLive.Tool
{
    public class TagLayerConstKeysGenerator : ConstKeysGenerator
    {
        public override string GetName()
        {
            return "更新Tag和Layer";
        }

        public override void Generate()
        {
            CreateConstSctipt("Tags", UnityEditorInternal.InternalEditorUtility.tags.ToList());

            CreateConstSctipt("Layers", UnityEditorInternal.InternalEditorUtility.layers.ToList());
        }
    }
}
