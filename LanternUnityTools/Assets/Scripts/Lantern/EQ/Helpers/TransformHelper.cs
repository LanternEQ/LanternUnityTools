using UnityEngine;

public static class TransformHelper
{
    public static Transform FindChildRecursive (this Transform tfm, string name)
    {
        int numChildren = tfm.childCount;

        for (int i=0; i<numChildren; i++)
            if (tfm.GetChild(i).name == name) return tfm.GetChild(i);

        for (int i=0; i<numChildren; i++)
        {
            Transform result = tfm.GetChild(i).FindChildRecursive(name);
            if (result != null) return result;
        }

        return null;
    }
}
