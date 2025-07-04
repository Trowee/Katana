namespace AudioManager
{
    public interface IAppliable<T>
    {
        public void Apply(T target);
    }
}