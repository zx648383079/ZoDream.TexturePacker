namespace ZoDream.Shared.Interfaces
{
    public interface ITextReader<T>
    {
        public bool IsEnabled(string content);
        public T? Deserialize(string content, string fileName);
        public string Serialize(T data, string fileName);
    }
}
