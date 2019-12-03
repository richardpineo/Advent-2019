
interface ISolve 
{
    string Description();

    // Tries on the examples.
    bool Prove(bool isA);
 
    string Solve(bool isA);
}