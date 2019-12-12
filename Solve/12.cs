using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Moons = System.Collections.Generic.List<Moon>;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

[DebuggerDisplay("({x},{y},{z})")]
class Point3D : IComparable
{
    public int x;
    public int y;
    public int z;

    public string Uid
    {
        get
        {
            return $"{x}{y}{z}";
        }
    }

    public Point3D Inverted
    {
        get
        {
            return new Point3D() { x = -x, y = -y, z = -z };
        }
    }

    public Point3D Flattened
    {
        get
        {
            return new Point3D()
            {
                x = flatten(x),
                y = flatten(y),
                z = flatten(z)
            };
        }
    }

    static int flatten(int v)
    {
        if (v < 0)
        {
            return -1;
        }
        if (v > 0)
        {
            return 1;
        }
        return 0;
    }

    public static Point3D Parse(string line)
    {
        // <x=13, y=-3, z=0>
        var regex = "<x=([0-9-]+), y=([0-9-]+), z=([0-9-]+)>";

        var matches = Regex.Matches(line, regex);
        return new Point3D()
        {
            x = int.Parse(matches[0].Groups[1].Value),
            y = int.Parse(matches[0].Groups[2].Value),
            z = int.Parse(matches[0].Groups[3].Value)
        };
    }

    public void Add(Point3D other)
    {
        x += other.x;
        y += other.y;
        z += other.z;
    }

    public int CompareTo(object obj)
    {
        var other = (Point3D)obj;
        var xc = x.CompareTo(other.x);
        var yc = y.CompareTo(other.y);
        var zc = z.CompareTo(other.z);
        return xc != 0 ? xc : (yc != 0 ? yc : (zc != 0 ? zc : 0));
    }
}

[DebuggerDisplay("p=({position.uid}) v=({velocity.uid})")]
class Moon : IComparable
{
    public Moon(Point3D position)
    {
        this.position = position;
    }

    public string Uid
    {
        get
        {
            return $"{position.Uid}{velocity.Uid}";
        }
    }

    public int potential
    {
        get
        {
            return Math.Abs(position.x) + Math.Abs(position.y) + Math.Abs(position.z);
        }
    }

    public int kinetic
    {
        get
        {
            return Math.Abs(velocity.x) + Math.Abs(velocity.y) + Math.Abs(velocity.z);
        }
    }

    public int total
    {
        get
        {
            return potential * kinetic;
        }
    }

    public Point3D position;
    public Point3D velocity = new Point3D() { x = 0, y = 0, z = 0 };

    public Tuple<int, int> vector(int axis)
    {
        switch (axis)
        {
            case 0:
                return new Tuple<int, int>(position.x, velocity.x);
            case 1:
                return new Tuple<int, int>(position.y, velocity.y);
            case 2:
                return new Tuple<int, int>(position.z, velocity.z);
            default:
                throw new Exception("out of range");
        }
    }

    public int CompareTo(object obj)
    {
        var other = (Moon)obj;
        var pos = this.position.CompareTo(other.position);
        if (pos != 0)
        {
            return pos;
        }
        return this.velocity.CompareTo(other.velocity);

    }
}

[DebuggerDisplay("{moon}: {velocityDelta}")]
class Gravity
{
    public Moon moon;
    public Point3D velocityDelta;

    public void Apply()
    {
        moon.velocity.Add(velocityDelta);
    }
}

class Space
{
    public Moons moons = new Moons();

    public Moon[][] MoonPairs;

    public void ApplyGravity()
    {
        var allGravity = new List<Gravity>();
        foreach (var pair in MoonPairs)
        {
            var moon1 = pair[0];
            var moon2 = pair[1];
            var delta = new Point3D()
            {
                x = moon2.position.x - moon1.position.x,
                y = moon2.position.y - moon1.position.y,
                z = moon2.position.z - moon1.position.z,
            };
            var g1 = new Gravity() { moon = moon1, velocityDelta = delta.Flattened };
            var g2 = new Gravity() { moon = moon2, velocityDelta = delta.Inverted.Flattened };
            allGravity.Add(g1);
            allGravity.Add(g2);
        }

        // Apply all the gravity
        foreach (Gravity g in allGravity)
        {
            g.Apply();
        }
    }

    public void ApplyVelocity()
    {
        foreach (var moon in moons)
        {
            moon.position.Add(moon.velocity);
        }
    }

    public int Energy
    {
        get
        {
            return moons.Sum(m => m.total);
        }
    }

    public int Kinetic
    {
        get
        {
            return moons.Sum(m => m.kinetic);
        }
    }

    public string position
    {
        get
        {
            // Lasy, assume 4.
            return $"{moons[0].position.Uid}|{moons[1].position.Uid}|{moons[2].position.Uid}|{moons[3].position.Uid}";
        }
    }

    public string Uid
    {
        get
        {
            // Lasy, assume 4.
            return $"{moons[0].Uid}{moons[1].Uid}{moons[2].Uid}{moons[3].Uid}";
        }
    }

    public Tuple<int, int>[] vector(int axis)
    {
        var tuples = new Tuple<int, int>[4];
        tuples[0] = moons[0].vector(axis);
        tuples[1] = moons[1].vector(axis);
        tuples[2] = moons[2].vector(axis);
        tuples[3] = moons[3].vector(axis);
        return tuples;
    }

}

class Solve12 : ISolve
{
    public string Description()
    {
        return "Day 12: The N-Body Problem";
    }

    const string ExampleA1 = "Examples//12a-1.txt";
    const string ExampleA2 = "Examples//12a-2.txt";
    const string Input = "Input//12.txt";

    public bool Prove(bool isA)
    {
        if (isA)
        {
            return
                ProveAFor(ExampleA1, 10, 179) &&
                ProveAFor(ExampleA2, 100, 1940);
        }
        else
        {
            return
                ProveBFor(ExampleA1, 2772) &&
                ProveBFor(ExampleA2, 4686774924);
        }
    }

    bool ProveAFor(string file, int steps, int answer)
    {
        var energy = CalculateEnergy(MakeSpace(file), steps);
        if (energy != answer)
        {
            Console.WriteLine($"  Expected {answer} but got {energy}");
        }
        return energy == answer;
    }

    static long GreatestCommonFactor(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    /*
     static long LeastCommonMultiple(long a, long b)
      {
          return (a / GreatestCommonFactor(a, b)) * b;
      }
      */
    long LeastCommonMultiple(long[] values)
    {
        // Initialize result 
        long ans = values[0];

        // ans contains LCM of arr[0], ..arr[i] 
        // after i'th iteration, 
        for (int i = 1; i < values.Length; i++)
        {
            ans = (((values[i] * ans)) /
                    (GreatestCommonFactor(values[i], ans)));
        }

        return ans;
    }

    bool ProveBFor(string file, long answer)
    {
        var attempt = FindCycle(file);
        if (attempt != answer)
        {
            Console.WriteLine($"  Expected {answer} but got {attempt}");
        }
        return attempt == answer;
    }

    long FindCycle(string file)
    {
        var repeatX = FindRepeatingState(MakeSpace(file), 0);
        var repeatY = FindRepeatingState(MakeSpace(file), 1);
        var repeatZ = FindRepeatingState(MakeSpace(file), 2);
        var values = new long[] { repeatX, repeatY, repeatZ };
        return LeastCommonMultiple(values);
    }

    public string Solve(bool isA)
    {
        return isA ? CalculateEnergy(MakeSpace(Input), 1000).ToString() : SolveB();
    }

    Space MakeSpace(string filename)
    {
        var lines = File.ReadAllLines(filename, Encoding.UTF8);
        var space = new Space();
        foreach (var line in lines)
        {
            var p = Point3D.Parse(line);
            space.moons.Add(new Moon(p));
        }
        // Only works for 4.
        space.MoonPairs = new Moon[][]{
            new Moon[] { space.moons[0], space.moons[1] },
            new Moon[] { space.moons[0], space.moons[2] },
            new Moon[] { space.moons[0], space.moons[3] },
            new Moon[] { space.moons[1], space.moons[2] },
            new Moon[] { space.moons[1], space.moons[3] },
            new Moon[] { space.moons[2], space.moons[3] }
        };
        return space;
    }

    int CalculateEnergy(Space space, int numSteps)
    {
        StepSpace(space, numSteps);
        return space.Energy;
    }

    void StepSpace(Space space, long numSteps)
    {
        for (long i = 0; i < numSteps; i++)
        {
            space.ApplyGravity();
            space.ApplyVelocity();
        }
    }

    bool differentVector(Tuple<int, int>[] a, Tuple<int, int>[] b)
    {
        return
            a[0].Item1 != b[0].Item1 ||
            a[0].Item2 != b[0].Item2 ||
            a[1].Item1 != b[1].Item1 ||
            a[1].Item2 != b[1].Item2 ||
            a[2].Item1 != b[2].Item1 ||
            a[2].Item2 != b[2].Item2 ||
            a[3].Item1 != b[3].Item1 ||
            a[3].Item2 != b[3].Item2;

    }

    long FindRepeatingState(Space space, int axis)
    {
        var toFind = space.vector(axis);
        space.ApplyGravity();
        space.ApplyVelocity();
        long period = 1;
        while (differentVector(space.vector(axis), toFind))
        {
            space.ApplyGravity();
            space.ApplyVelocity();
            period++;
        }
        return period;
    }

    private string SolveB()
    {
        long cycle = FindCycle(Input);
        return cycle.ToString();
    }
}
