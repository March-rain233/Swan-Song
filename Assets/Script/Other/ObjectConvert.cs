using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ObjectConvert : JsonConverter<Object>
{
    public override Object ReadJson(JsonReader reader, System.Type objectType, Object existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var instance = System.Convert.ToInt32(reader.Value.ToString());
        return Resources.InstanceIDToObject(instance);
    }

    public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
    {
        writer.WriteValue(value.GetInstanceID());
    }
}

public class SpriteConvert : JsonConverter<Sprite>
{
    public override Sprite ReadJson(JsonReader reader, System.Type objectType, Sprite existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var value = reader.Value.ToString();
        if(value == "null")
        {
            return null;
        }
        else
        {
            return Resources.InstanceIDToObject(System.Convert.ToInt32(value)) as Sprite;
        }
    }

    public override void WriteJson(JsonWriter writer, Sprite value, JsonSerializer serializer)
    {
        if(value == null)
        {
            writer.WriteValue("null");
        }
        else
        {
            writer.WriteValue(value.GetInstanceID());
        }
    }
}

public class RandomStateConvert : JsonConverter<Random.State>
{
    public override Random.State ReadJson(JsonReader reader, System.Type objectType, Random.State existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return JsonUtility.FromJson<Random.State>(reader.Value.ToString());
    }

    public override void WriteJson(JsonWriter writer, Random.State value, JsonSerializer serializer)
    {
        writer.WriteValue(JsonUtility.ToJson(value));
    }
}