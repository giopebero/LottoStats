using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using MyStats.Core.Models;

namespace MyStats.Core
{
    public class Repository
    {
        private readonly MongoClient _mongoClient;

        public Repository()
        {
            _mongoClient = new MongoClient(ConfigurationManager.AppSettings["mongoConnection"]);
        }

        public void Save(IEnumerable<Draw> draws)
        {
            var db = _mongoClient.GetDatabase("stats");
            var collection = db.GetCollection<Draw>("draw");
            collection.DeleteMany(FilterDefinition<Draw>.Empty);
            collection.InsertMany(draws);
        }

        public IEnumerable<Draw> Get(int? takeLastDraws)
        {
            var db = _mongoClient.GetDatabase("stats");
            var collection = db.GetCollection<Draw>("draw");
            var draws = collection.Find(FilterDefinition<Draw>.Empty).ToList();

            if (takeLastDraws.HasValue)
            {
                draws.Reverse();
                draws = draws.Take(takeLastDraws.Value).ToList();
            }

            return draws;
        }

        public Draw Get(DateTime date)
        {
            var db = _mongoClient.GetDatabase("stats");
            var collection = db.GetCollection<Draw>("draw");
            return collection.Find(draw => draw.Date == date).Single();
        }

        public IEnumerable<Delay> GetDelays()
        {
            var delays = new List<Delay>();
            foreach (var w in (Wheel[]) Enum.GetValues(typeof(Wheel)))
            {
                delays.AddRange(GetDelays(w));
            }

            return delays.OrderByDescending(d => d.Draws);
        }

        public IEnumerable<Delay> GetDelays(Wheel wheel)
        {
            var draws = Get(null).ToList();
            var drawsCount = draws.Count();
            var numbers = Enumerable.Range(1, 90).ToDictionary(n => n, n => drawsCount);
            var drawNum = 0;
            foreach (var draw in draws)
            {
                drawNum++;
                foreach (var row in draw.Rows.Where(r => r.Wheel == wheel))
                {
                    foreach (var rowNumber in row.Numbers)
                    {
                        numbers[rowNumber] = drawsCount - drawNum;
                    }
                }
            }
            return numbers.Select(pair => new Delay
            {
                Wheel = wheel,
                Number = pair.Key,
                Draws = pair.Value
            }).OrderByDescending(d => d.Draws);
        }

        public IEnumerable<Frequence> GetFrequencies(int? takeLastDraws)
        {
            var delays = new List<Frequence>();
            foreach (var w in (Wheel[])Enum.GetValues(typeof(Wheel)))
            {
                delays.AddRange(GetFrequencies(w, takeLastDraws));
            }

            return delays.OrderByDescending(d => d.Draws);
        }

        public IEnumerable<Frequence> GetFrequencies(Wheel wheel, int? takeLastDraws)
        {
            var draws = Get(takeLastDraws).ToList();

            var numbers = Enumerable.Range(1, 90).ToDictionary(n => n, n => 0);
            foreach (var draw in draws)
            {
                foreach (var row in draw.Rows.Where(r => r.Wheel == wheel))
                {
                    foreach (var rowNumber in row.Numbers)
                    {
                        numbers[rowNumber]++;
                    }
                }
            }
            return numbers.Select(pair => new Frequence
            {
                Wheel = wheel,
                Number = pair.Key,
                Draws = pair.Value
            }).OrderByDescending(d => d.Draws);
        }
    }
}