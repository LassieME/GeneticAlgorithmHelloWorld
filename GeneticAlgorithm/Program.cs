using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace GeneticAlgorithm
{
    public static class Program
    {
        private static readonly Random random = new Random();
        private const double MUTATE_RATE = 0.01;
        private const double BREED_RATE = 0.75;
        private const int POPULATION_SIZE = 1000;
        private const string TARGET = "Hello, Cruel World!";

        private static void Main()
        {
            List<string> population = GeneratePopulation();
            int generation = 0;

            Console.WriteLine($"Using a population size of {POPULATION_SIZE},");
            Console.WriteLine($"Regenerating {BREED_RATE * 100}% of the population per generation,");
            Console.WriteLine((MUTATE_RATE * 100) + "% chance of mutation for each chromosome.");

            do
            {
                generation++;

                var results = population.Select(CheckFitness).OrderByDescending(x => x.score).ToArray();
                if (results[0].value != TARGET)
                {
                    var elders = results.Take((int) (1000.0 * (1.0 - BREED_RATE))).ToArray();
                    population = elders.Select(x => x.value).ToList();
                    var totalScore = elders.Sum(x => x.score);
                    for (var i = 0; i < POPULATION_SIZE * BREED_RATE; ++i)
                    {
                        population.Add(Breed(SelectParent(elders, totalScore).value, SelectParent(elders, totalScore).value));
                    }
                }
                else
                {
                    population = results.Select(x => x.value).ToList();
                }

                Console.WriteLine($"Generation {generation}: '{results[0].value}', score: {results[0].score}");
            }
            while (population[0] != TARGET);

            Console.ReadLine();
        }

        private static char GenerateCharacter() => (char)random.Next(32, 32 + 94);

        private static (string value, int score) SelectParent((string value, int score)[] elders, int totalScore)
        {
            var selection = random.NextDouble() * totalScore;
            var sum = 0;
            foreach (var e in elders)
            {
                sum += e.score;
                if (selection <= sum)
                {
                    return e;
                }
            }
            Debug.Fail("");
            return elders.FirstOrDefault();
        }

        private static List<string> GeneratePopulation()
        {
            var p = new List<string>(1000);
            for (var i = 0; i < POPULATION_SIZE; ++i) {
                var x = "";
                foreach(var c in TARGET) {
                    x += GenerateCharacter();
                }
                p.Add(x);
            }
            return p;
        }

        private static (string value, int score) CheckFitness(string x)
        {
            var r = (value: x, score: 0);
            for (int i = 0; i < TARGET.Length; i++)
            {
                if (x[i] == TARGET[i])
                    r.score++;
            }
            return r;
        }

        private static string Breed(string p1, string p2)
        {
            var c = "";
            for (int i = 0; i < TARGET.Length; i++)
            {
                if (random.NextDouble() < MUTATE_RATE)
                {
                    c += GenerateCharacter();
                }
                else if (random.NextDouble() < 0.5)
                {
                    c += p1[i];
                }
                else
                {
                    c += p2[i];
                }
            }
            return c;
        }
    }
}