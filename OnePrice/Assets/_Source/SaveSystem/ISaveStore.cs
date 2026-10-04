namespace SaveSystem
{
    public interface ISaveStore { string Read(); void Write(string json); }
}
