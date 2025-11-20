public class Reference
{
    public string Book { get; private set; }
    public int StartVerse { get; private set; }
    public int EndVerse { get; private set; }

    public Reference(string book, int verse)
    {
        Book = book;
        StartVerse = verse;
        EndVerse = verse;
    }

    public Reference(string book, int startVerse, int endVerse, int v)
    {
        Book = book;
        StartVerse = startVerse;
        EndVerse = endVerse;
    }

    public override string ToString()
    {
        return EndVerse == StartVerse ? $"{Book} {StartVerse}" : $"{Book} {StartVerse}-{EndVerse}";
    }
}