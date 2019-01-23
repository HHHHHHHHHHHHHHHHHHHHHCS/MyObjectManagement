using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistenStorage : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.dataPath + "/..", "saveFile.save");
    }

    public void Save(PersistableObject o)
    {
        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            o.Save(new GameDataWriter(writer));
        }
    }

    public void Load(PersistableObject o)
    {
        using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            o.Load(new GameDataReader(reader));
        }
    }
}
