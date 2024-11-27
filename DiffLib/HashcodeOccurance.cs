namespace DiffLib;

internal class HashcodeOccurance
{
    public HashcodeOccurance(int position, HashcodeOccurance? next)
    {
        this.Position = position;
        this.Next = next;
    }

    public int Position
    {
        get;
    }

    public HashcodeOccurance? Next
    {
        get;
        set;
    }
}
