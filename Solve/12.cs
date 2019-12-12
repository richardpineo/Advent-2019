using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Moons = System.Collections.Generic.List<Moon>;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

[DebuggerDisplay("({x},{y},{z})")]
class Point3D
{
    public int x;
    public int y;
    public int z;

    public Point3D inverted
    {
        get
        {
            return new Point3D() { x = -x, y = -y, z = -z };
        }
    }

    public Point3D flattened
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

    public static Point3D parse(string line)
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

    public void add(Point3D other)
    {
        x += other.x;
        y += other.y;
        z += other.z;
    }
}

[DebuggerDisplay("p=({position.uid}) v=({velocity.uid})")]
class Moon
{
    public Moon(Point3D position)
    {
        this.position = position;
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
}

[DebuggerDisplay("{moon}: {velocityDelta}")]
class Gravity
{
    public Moon moon;
    public Point3D velocityDelta;

    public void Apply()
    {
        moon.velocity.add(velocityDelta);
    }
}

class Space
{
    public Moons moons = new Moons();

    public Moon[][] moonPairs;

    public void applyGravity()
    {
        var allGravity = new List<Gravity>();
        foreach (var pair in moonPairs)
        {
            var moon1 = pair[0];
            var moon2 = pair[1];
            var delta = new Point3D()
            {
                x = moon2.position.x - moon1.position.x,
                y = moon2.position.y - moon1.position.y,
                z = moon2.position.z - moon1.position.z,
            };
            var g1 = new Gravity() { moon = moon1, velocityDelta = delta.flattened };
            var g2 = new Gravity() { moon = moon2, velocityDelta = delta.inverted.flattened };
            allGravity.Add(g1);
            allGravity.Add(g2);
        }

        // Apply all the gravity
        foreach (Gravity g in allGravity)
        {
            g.Apply();
        }
    }

    public void applyVelocity()
    {
        foreach (var moon in moons)
        {
            moon.position.add(moon.velocity);
        }
    }

    public int energy
    {
        get
        {
            return moons.Sum(m => m.total);
        }
    }

    public int kinetic
    {
        get
        {
            return moons.Sum(m => m.kinetic);
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
        var energy = calculateEnergy(makeSpace(file), steps);
        if (energy != answer)
        {
            Console.WriteLine($"  Expected {answer} but got {energy}");
        }
        return energy == answer;
    }

    bool ProveBFor(string file, long answer)
    {
        var attempt = findCycle(file);
        if (attempt != answer)
        {
            Console.WriteLine($"  Expected {answer} but got {attempt}");
        }
        return attempt == answer;
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

    long findCycle(string file)
    {
        var repeatX = findRepeatingState(makeSpace(file), 0);
        var repeatY = findRepeatingState(makeSpace(file), 1);
        var repeatZ = findRepeatingState(makeSpace(file), 2);
        var values = new long[] { repeatX, repeatY, repeatZ };
        return LeastCommonMultiple(values);
    }

    public string Solve(bool isA)
    {
        return isA ? calculateEnergy(makeSpace(Input), 1000).ToString() : SolveB();
    }

    Space makeSpace(string filename)
    {
        var lines = File.ReadAllLines(filename, Encoding.UTF8);
        var space = new Space();
        foreach (var line in lines)
        {
            var p = Point3D.parse(line);
            space.moons.Add(new Moon(p));
        }
        // Only works for 4.
        space.moonPairs = new Moon[][]{
            new Moon[] { space.moons[0], space.moons[1] },
            new Moon[] { space.moons[0], space.moons[2] },
            new Moon[] { space.moons[0], space.moons[3] },
            new Moon[] { space.moons[1], space.moons[2] },
            new Moon[] { space.moons[1], space.moons[3] },
            new Moon[] { space.moons[2], space.moons[3] }
        };
        return space;
    }

    int calculateEnergy(Space space, int numSteps)
    {
        for (long i = 0; i < numSteps; i++)
        {
            space.applyGravity();
            space.applyVelocity();
        }
        return space.energy;
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

    long findRepeatingState(Space space, int axis)
    {
        var toFind = space.vector(axis);
        space.applyGravity();
        space.applyVelocity();
        long period = 1;
        while (differentVector(space.vector(axis), toFind))
        {
            space.applyGravity();
            space.applyVelocity();
            period++;
        }
        return period;
    }

    private string SolveB()
    {
        long cycle = findCycle(Input);
        return cycle.ToString();
    }
}
