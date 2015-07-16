using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FeatureToggle;

namespace ExampleConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = LoadFromFile("./model.xml");
            var toggle = new Toggle(new Hasher());
            var count = 0;

            try
            {
                foreach (var identifer in model.Identifiers)
                {
                    var willSeeFeature = toggle.IsFeatureOn(model.Config, identifer);
                    Console.WriteLine("{0} for {1}", willSeeFeature ? "True " : "False", identifer);
                    if (willSeeFeature) count++;
                }

                Console.WriteLine();
                Console.WriteLine("{0} of {1} users ({2}) will see the feature", count, model.Identifiers.Count, (count/(double) model.Identifiers.Count).ToString("P"));
                Console.WriteLine("Using config: {0}", model.Config);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static DataModel LoadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                CreateFile(filename);
            }

            var settings = new XmlReaderSettings
            {
                CloseInput = true,
                ConformanceLevel = ConformanceLevel.Document,
                IgnoreComments = true
            };
            using (var reader = XmlReader.Create(filename, settings))
            {
                var serializer = new XmlSerializer(typeof(DataModel));
                return (DataModel)serializer.Deserialize(reader);
            }
        }

        static void CreateFile(string filename)
        {
            var model = new DataModel
            {
                Config = "percent=.5",
                Identifiers = new List<string>()
            };

            for (int i = 0; i < 100; i++)
            {
                model.Identifiers.Add(Guid.NewGuid().ToString());
            }


            var serializer = new XmlSerializer(typeof(DataModel));

            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, model);
            }
        }
    }

    public class DataModel
    {
        public string Config { get; set; }
        public List<string> Identifiers { get; set; }
    }
}
