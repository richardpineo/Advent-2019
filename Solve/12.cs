using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Moons = System.Collections.Generic.List<Moon>;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

[DebuggerDisplay("({x}, {y}, {z})")]
class Point3D : IComparable
{
    public int x;
    public int y;
    public int z;

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

[DebuggerDisplay("p=({position.x}, {position.y}, {position.z}) v=({velocity.x}, {velocity.y}, {velocity.z})")]
class Moon : IComparable
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

    public void ApplyGravity()
    {
        var allGravity = new List<Gravity>();
        var pairs = Util.GetKCombs(moons, 2);
        foreach (var pair in pairs)
        {
            var moon1 = pair.ElementAt(0);
            var moon2 = pair.ElementAt(1);
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
        return
        ProveFor(ExampleA1, 10, 179) &&
        ProveFor(ExampleA2, 100, 1940);
    }

    public bool ProveFor(string file, int steps, int answer)
    {
        var energy = CalculateEnergy(file, steps);
        if (energy != answer)
        {
            Console.WriteLine($"  Expected {answer} but got {energy}");
        }
        return energy == answer;
    }

    public string Solve(bool isA)
    {
        return CalculateEnergy(Input, 1000).ToString();
    }

    public int CalculateEnergy(string filename, int numSteps)
    {
        var lines = File.ReadAllLines(filename, Encoding.UTF8);
        var space = new Space();
        foreach (var line in lines)
        {
            var p = Point3D.Parse(line);
            space.moons.Add(new Moon(p));
        }

        for (int i = 0; i < numSteps; i++)
        {
            space.ApplyGravity();
            space.ApplyVelocity();
        }
        return space.Energy;
    }
}
