namespace Fiasco.Transposition
{
    public struct Node
    {
        int Score;
        int Depth;

        public Node(int score, int depth)
        {
            Score = score;
            Depth = depth;
        }
    }
}
