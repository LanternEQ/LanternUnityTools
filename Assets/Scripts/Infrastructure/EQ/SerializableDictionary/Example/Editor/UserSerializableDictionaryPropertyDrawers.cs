#if UNITY_EDITOR
using Infrastructure.EQ.SerializableDictionary.Editor;
using UnityEditor;

namespace Infrastructure.EQ.SerializableDictionary.Example.Editor
{
    [CustomPropertyDrawer(typeof(StringStringDictionary))]
    public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
}
#endif
