//This is a modified version of https://gist.github.com/eppz/1ebbc1cf6a77741f56d63d3803e57ba3
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;


public class BuildPostProcessor
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
    }
}