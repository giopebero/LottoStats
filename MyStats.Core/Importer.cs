using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using MyStats.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyStats.Core
{
    public class Importer
    {
        public IEnumerable<Draw> ImportData(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var drawDictionary = new Dictionary<DateTime, Draw>();
            foreach (var line in lines)
            {
                var data = GetDrawData(line);
                var drawDate = AddDraw(data, drawDictionary);
                var drawRow = GetDrawRow(data);
                drawDictionary[drawDate].AddRow(drawRow);
            }

            var draws = drawDictionary.Select(d => d.Value).ToList();
            var repo = new Repository();
            repo.Save(draws);
            return draws;
        }

        private static string[] GetDrawData(string line)
        {
            var data = line.Split('\t');
            return data;
        }

        private static DateTime AddDraw(string[] data, Dictionary<DateTime, Draw> draws)
        {
            var drawDate = DateTime.Parse(data[0]);
            if (!draws.ContainsKey(drawDate))
            {
                draws.Add(drawDate, new Draw
                {
                    Date = drawDate,
                    Rows = new List<DrawRow>()
                });
            }

            return drawDate;
        }

        private static DrawRow GetDrawRow(string[] data)
        {
            var drawRow = new DrawRow
            {
                Wheel = (Wheel)Enum.Parse(typeof(Wheel), data[1])
            };

            data.ToList().Skip(2).Take(5).ToList().ForEach(stringNumber => drawRow.AddNumber(stringNumber.ToInt()));
            return drawRow;
        }
    }

    public class Draw
    {
        [BsonId]
        public DateTime Date { get; set; }
        public List<DrawRow> Rows { get; set; }

        public void AddRow(DrawRow row)
        {
            Rows.Add(row);
        }
    }

    public class DrawRow
    {
        public DrawRow()
        {
            Numbers = new List<int>();
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public Wheel Wheel { get; set; }
        public List<int> Numbers { get; set; }

        public void AddNumber(int number)
        {
            Numbers.Add(number);
        }
    }

    public static class StrinUtilities
    {
        public static int ToInt(this string intValue)
        {
            return int.Parse(intValue);
        }
    }
}
