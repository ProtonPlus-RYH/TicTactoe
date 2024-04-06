using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileManager : MonoSingleton<FileManager>
{
    [HideInInspector]
    public string DataPath;
    [HideInInspector]
    public List<string> ReplayFileNames;

    void Start()
    {
        string[] path = Application.dataPath.Split("/");
        StringBuilder pathSB = new StringBuilder();
        for (int i = 0; i < path.Length - 1; i++)
        {
            pathSB.Append(path[i]);
            pathSB.Append("/");
        }
        DataPath = pathSB.ToString();
        GetReplayFilesFromFolder();
    }

    public void WriteReplay(List<string> replayInput, string fileName)
    {
        StringBuilder pathSB = new StringBuilder(DataPath);
        pathSB.Append("Replays/");
        pathSB.Append(fileName);
        pathSB.Append(".txt");
        if (!File.Exists(pathSB.ToString()))
        {
            File.WriteAllLines(pathSB.ToString(), replayInput);
        }
        else
        {
            Debug.Log("文件名重复");
        }
        GetReplayFilesFromFolder();
    }

    public void DeleteReplay(string fileName)
    {
        StringBuilder pathSB = new StringBuilder(DataPath);
        pathSB.Append("Replays/");
        pathSB.Append(fileName);
        pathSB.Append(".txt"); ;
        if (File.Exists(pathSB.ToString()))
        {
            File.Delete(pathSB.ToString());
            pathSB.Append(".meta");
            File.Delete(pathSB.ToString());
        }
        GetReplayFilesFromFolder();
    }

    public void GetReplayFilesFromFolder()
    {
        string[] ReplayFiles = Directory.GetFiles(DataPath + "Replays/", "*.txt");
        ReplayFileNames = new List<string>(ReplayFiles);
        for (int i = 0; i < ReplayFileNames.Count; i++)
        {
            string[] pathFolders = ReplayFileNames[i].Split("/");
            string[] fileName = pathFolders[pathFolders.Length - 1].Split(".");
            string[] fileNameDeletingReverseSlash = fileName[0].Split("\\");
            if (fileNameDeletingReverseSlash.Length == 2)
            {
                ReplayFileNames[i] = fileNameDeletingReverseSlash[1];
            }
            else if (fileNameDeletingReverseSlash.Length == 1)
            {
                ReplayFileNames[i] = fileNameDeletingReverseSlash[0];
            }
        }
    }

    public List<string> ReadReplay(string fileName)
    {
        List<string> result = new List<string>();
        string filePath = DataPath + "Replays/" + fileName + ".txt";
        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine());
            }
        }
        return result;
    }

    public int GetReplayFileCount()
    {
        string path = DataPath + "Replays/";
        string[] ReplayFiles = Directory.GetFiles(path, "*.txt");
        return ReplayFiles.Length;
    }
}
