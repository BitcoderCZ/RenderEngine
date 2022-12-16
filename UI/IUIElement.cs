using GameEngine.Inputs;

namespace GameEngine.UI
{
    public interface IUIElement
    {
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        bool Active { get; set; }

        void Draw(Engine engine, UIManager manager);
        void Update(Engine engine, UIManager manager, double deltaTime);

        void OnKeyDown(UIManager manager, Key key, Modifiers modifiers);
        void OnKeyUp(UIManager manager, Key key, Modifiers modifiers);

        void OnMouseDown(UIManager manager, MouseButtons btn, bool onElement);
        void OnMouseUp(UIManager manager, MouseButtons btn, bool onElement);
    }
}
