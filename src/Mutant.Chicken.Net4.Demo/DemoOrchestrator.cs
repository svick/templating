﻿using System;
using System.IO;
using System.Text;
using Mutant.Chicken.Runner;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mutant.Chicken.Demo
{
    public class DemoOrchestrator : Orchestrator
    {
        private JsonSerializer _serializer;
        private Func<VariableCollection, VariableCollection> _root;

        private JsonSerializer CreateSerializer()
        {
            JsonSerializer serializer = new JsonSerializer
            {
                Converters =
                {
                    new PathMatcherConverter(),
                    new OperationProviderConverter(),
                    new VariableCollectionConverter(_root),
                    new RunSpecConverter(),
                    new SpecialConverter()
                }
            };

            return serializer;
        }
        public DemoOrchestrator(Func<VariableCollection, VariableCollection> root)
        {
            _root = root;
            _serializer = CreateSerializer();
        }

        protected override IGlobalRunSpec RunSpecLoader(Stream runSpec)
        {
            OperationProviderConverter.Count = 0;
            using (TextReader reader = new StreamReader(runSpec, Encoding.UTF8, true, 8192, true))
            using (JsonReader jReader = new JsonTextReader(reader))
            {
                JToken doc = JToken.ReadFrom(jReader);
                return doc.ToObject<DemoGlobalRunSpec>(_serializer);
            }
        }
    }
}