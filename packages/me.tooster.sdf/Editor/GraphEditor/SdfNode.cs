using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;
namespace me.tooster.sdf.Editor.GraphEditor.Editor.GraphEditor {
    [Serializable]
    public class SdfNode : Node {
        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddOutputPort<Func<Vector3, float>>("sdf").Build();
            context.AddInputPort<Vector3>("position").Build();
            context.AddOutputPort<float>("distance").Build();
        }
    }



    [UseWithContext(typeof(RaymarchingContext))]
    [Serializable]
    public class RaymarchSdfNode : BlockNode {
        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort<Func<Vector3, float>>("sdf").Build();
            context.AddOutputPort<Vector3>("position").Build();
        }
    }



    [Serializable]
    public class RaymarchingContext : ContextNode {
        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddOutputPort<float>("distance").WithConnectorUI(PortConnectorUI.Arrowhead).Build();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context) {
            context.AddOption<bool>("useNormal").WithDefaultValue(true).Build();
        }
    }
}
