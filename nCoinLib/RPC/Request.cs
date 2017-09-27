using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace nCoinLib.RPC
{
    class Request
    {
        public Request(string Command, object[] Arguments)
            : this()
        {
            this.Command = Command;
            this.Arguments = Arguments;
        }
        public Request()
        {
            JsonRpc = "1.0";
            Id = 1;
        }
        public string JsonRpc
        {
            get;
            set;
        }
        public int Id
        {
            get;
            set;
        }
        public string Command
        {
            get;
            set;
        }
        public object[] Arguments
        {
            get;
            set;
        }

        public void WriteJSON(TextWriter writer)
        {
            var jsonWriter = new JsonTextWriter(writer);
            WriteJSON(jsonWriter);
            jsonWriter.Flush();
        }

        internal void WriteJSON(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            WriteProperty(writer, "jsonrpc", JsonRpc);
            WriteProperty(writer, "id", Id);
            WriteProperty(writer, "method", Command);

            writer.WritePropertyName("params");
            writer.WriteStartArray();

            if (Arguments != null)
            {
                for (int i = 0; i < Arguments.Length; i++)
                {
                    if (Arguments[i] is JToken)
                    {
                        ((JToken)Arguments[i]).WriteTo(writer);
                    }
                    else if (Arguments[i] is Array)
                    {
                        writer.WriteStartArray();
                        foreach (var x in (Array)Arguments[i])
                        {
                            writer.WriteValue(x);
                        }
                        writer.WriteEndArray();
                    }
                    else
                    {
                        writer.WriteValue(Arguments[i]);
                    }
                }
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        private void WriteProperty<TValue>(JsonTextWriter writer, string property, TValue value)
        {
            writer.WritePropertyName(property);
            writer.WriteValue(value);
        }
    }
}
