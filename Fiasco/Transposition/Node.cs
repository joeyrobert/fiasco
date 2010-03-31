namespace Fiasco.Transposition
{
    public struct Node
    {
        int Score;
        int Depth;
        bool Set;

        public Node(int score, int depth)
        {
            Score = score;
            Depth = depth;
            Set = true;
        }
    }
}
