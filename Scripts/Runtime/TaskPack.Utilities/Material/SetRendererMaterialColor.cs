using Z3.NodeGraph.Core;
using Z3.NodeGraph.Tasks;
using UnityEngine;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.TaskPack.Utilities
{
    [NodeCategory(Categories.Components)]
    [NodeDescription("Set Renderer Material Color")]
    public class SetRendererMaterialColor : ActionTask
    {
        [ParameterDefinition(AutoBindType.SelfBind)]
        [SerializeField] protected Parameter<Renderer> renderer;

        public Parameter<string> property = "_Color"; // You can see properties in inspector by debug like "_UnlitColor"
        public Parameter<Color> color;

        public override string Info => $"Set Color {renderer} = {GetColorInfo(color)}";

        protected override void StartAction()
        {
            renderer.Value.material.SetColor(property.Value, color.Value);

            EndAction(true);
        }

        private string GetColorInfo(Parameter<Color> parameter)
        {
            if (parameter.IsBinding)
                return parameter.ToString();

            return parameter.Value switch
            {
                Color color when color == Color.red => "Red".ToBold(),
                Color color when color == Color.green => "Green".ToBold(),
                Color color when color == Color.blue => "Blue".ToBold(),
                Color color when color == Color.clear => "Clear".ToBold(),
                Color color when color == Color.black => "Black".ToBold(),
                Color color when color == Color.white => "White".ToBold(),
                Color color when color == Color.gray  => "Gray".ToBold(),
                Color color when color == Color.yellow => "Yellow".ToBold(),
                Color color when color == Color.cyan => "Cyan".ToBold(),
                Color color when color == Color.magenta => "Magenta".ToBold(),
                _ => parameter.ToString(),
            };
        }
    }
}