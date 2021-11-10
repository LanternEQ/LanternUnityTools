#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(StringStringDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}
#endif