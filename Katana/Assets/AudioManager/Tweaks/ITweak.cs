namespace AudioManager.Tweaks
{
    public interface ITweak<T>
    {
        public void Apply(T target);
    }
}