using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace GeneticAlgorithm
{
    public static class Program
    {
        private static readonly Random Random = new Random();
        private const double MUTATE_RATE = 0.01;
        private const double BREED_RATE = 0.75;
        private const int POPULATION_SIZE = 1000;
        private const string TARGET = "Hello, Cruel World!";

        private static void Main()
        {
            string[] population = GeneratePopulation();
            int generation = 0;

            Console.WriteLine($"Using a population size of {POPULATION_SIZE},");
            Console.WriteLine($"Regenerating {BREED_RATE * 100}% of the population per generation,");
            Console.WriteLine((MUTATE_RATE * 100) + "% chance of mutation for each chromosome.");

            do
            {
                generation++;
                
                IOrderedEnumerable<(string value, int score)> results = population.Select(CheckFitness).OrderByDescending(x => x.score);
                if (results.First().value != TARGET)
                {
                    (string value, int score)[] elders = results.Take((int) (POPULATION_SIZE * (1.0 - BREED_RATE))).ToArray();
                    elders.Select(x => x.value).ToArray().CopyTo(population, 0);
                    int totalScore = elders.Sum(x => x.score);
                    for (var i = (int) (POPULATION_SIZE * (1.0 - BREED_RATE)); i < POPULATION_SIZE * BREED_RATE; i++)
                    {
                        population[i] = Breed(SelectParent(elders, totalScore).value, SelectParent(elders, totalScore).value);
                    }
                }
                else
                {
                    population = results.Select(x => x.value).ToArray();
                }

                Console.WriteLine($"Generation {generation}:\t'{results.First().value}', score: {results.First().score}");
            }
            while (population[0] != TARGET);

            Console.ReadLine();
        }

        private static char GenerateCharacter(this Random r) => (char)r.Next(32, 32 + 94);

        private static (string value, int score) SelectParent(IReadOnlyList<(string value, int score)> elders, int totalScore)
        {
            var selection = Random.NextDouble() * totalScore;
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
            return elders[0];
        }

        private static string[] GeneratePopulation()
        {
            var p = new string[POPULATION_SIZE];
            for (var i = 0; i < POPULATION_SIZE; ++i) {
                p[i] = string.Create(TARGET.Length, Random, (Span<char> chars, Random r) =>
                {
                    for (int j = 0; j < chars.Length; j++)
                    {
                        chars[j] = r.GenerateCharacter();
                    }
                });
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
                if (Random.NextDouble() < MUTATE_RATE)
                {
                    c += Random.GenerateCharacter();
                }
                else if (Random.NextDouble() < 0.5)
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