
interface ISolve 
{
    string Description();

    // Tries on the examples.
    bool ProveA();
    bool ProveB();

    string SolveA();
    string SolveB();
}