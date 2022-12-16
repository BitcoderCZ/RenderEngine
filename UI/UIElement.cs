using GameEngine.Inputs;

namespace GameEngine.UI
{
    public abstract class UIElement : IUIElement
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Active { get; set; } = true;

        public abstract void Draw(Engine engine, UIManager manager);
        public virtual void Update(Engine engine, UIManager manager, double deltaTime) { }

        public virtual void OnKeyDown(UIManager manager, Key key, Modifiers modifiers) { }
        public virtual void OnKeyUp(UIManager manager, Key key, Modifiers modifiers) { }

        public virtual void OnMouseDown(UIManager manager, MouseButtons btn, bool onElement) { }
        public virtual void OnMouseUp(UIManager manager, MouseButtons btn, bool onElement) { }
    }
}
