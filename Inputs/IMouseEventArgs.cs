using GameEngine.Maths.Vectors;

namespace GameEngine.Inputs
{
    public interface IMouseEventArgs
    {
        Vector2D Position { get; }

        MouseButtons Buttons { get; }

        int WheelDelta { get; }

        int ClickCount { get; }
    }
}
