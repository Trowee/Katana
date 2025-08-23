namespace Assets.Scripts.Core
{
    public interface ISelectable
    {
        public void Hover();
        public void Unhover();
        public void Select();
        public void Deselect();
    }
}
