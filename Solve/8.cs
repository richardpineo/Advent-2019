using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Layer = System.Collections.Generic.List<string>;

class Solve8 : ISolve
{
    public string Description()
    {
        return "Day 8: Space Image Format";
    }

    const string Input = "Input//8.txt";
    const string ExampleB = "Examples//1b.txt";

    public bool Prove(bool isA)
    {
        return isA ? ProveA() : ProveB();
    }

    private bool ProveA()
    {
        var image = "123456789012";
        var pixels = image.ToCharArray();
        var width = 3;
        var height = 2;
        var layers = new List<Layer>();
        for (var startIndex = 0; startIndex < pixels.Length; startIndex = layers.Count * width * height)
        {
            var newLayer = new Layer();
            for (var h = 0; h < height; h++)
            {
                var line = new StringBuilder();
                for (var w = 0; w < width; w++)
                {
                    line.Append(pixels[startIndex + h * width + w]);

                }
                newLayer.Add(line.ToString());
            }
            layers.Add(newLayer);
        }

        Console.WriteLine();
        for (int i = 0; i < layers.Count; i++)
        {
            var header = $"Layer {i + 1}: ";
            for (int j = 0; j < layers[i].Count; j++)
            {
                Console.Write("".PadLeft(8));
                if (j > 0)
                {
                    Console.Write(String.Format("".PadLeft(header.Length)));
                }
                else
                {
                    Console.Write(header);
                }

                Console.WriteLine(layers[i][j]);
            }
            Console.WriteLine();
        }

        return true;
    }

    public bool ProveB()
    {
        return false;
    }

    public string Solve(bool isA)
    {
        return isA ? SolveA() : SolveB();
    }

    public string SolveA()
    {
        return "NOTIMPL";
        //   return Solve();
    }

    public string SolveB()
    {
        return "NOTIMPL";
        // return Solve();
    }
}
