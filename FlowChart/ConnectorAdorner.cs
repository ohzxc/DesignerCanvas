using System.Windows.Documents;

namespace FlowChart
{
    internal class ConnectorAdorner: Adorner
    {
        private FlowCanvas canvas;
        private Connector connector;

        public ConnectorAdorner(FlowCanvas canvas, Connector connector): base(canvas)
        {
            this.canvas = canvas;
            this.connector = connector;
        }
    }
}