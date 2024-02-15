namespace WpfUniversity.StartUpHelpers
{
    public interface IAbstractFactory<T>
    {
        T Create();
    }
}