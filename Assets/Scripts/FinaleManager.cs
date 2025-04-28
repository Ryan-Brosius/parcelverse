using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_STANDALONE_WIN
using System.Windows.Forms;
using System.Windows;
#endif
using System.IO;

public class FinaleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //TriggerPopup("Hello", "World");
        //TriggerEnding();
        CopyFiles();
    }

    public void TriggerEnding()
    {
        string name = GetWindowsUsername();
        string text1 = $"Hello {name}! My last delivery today is for you!";
        string caption1 = $"Delivery for {name}!";
        TriggerPopup(text1, caption1);

        string text2 = $"I could tell you were helping to guide me throughout my adventure {name}";
        string caption2 = $"Thanks {name}!";
        TriggerPopup(text2, caption2);

        string text3 = $"I have delivered a \'package\' to your dimension, its apparently called the \'Desktop\'. This is where we part ways now {name}, thank you for the help!";
        string caption3 = $"Desktop Dimension";
        TriggerPopup(text3, caption3);
    }

    public void TriggerPopup(string text, string caption)
    {
#if UNITY_STANDALONE_WIN
        MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
    }

    public static string GetWindowsUsername()
    {
#if UNITY_STANDALONE_WIN
        string fullUsername = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        string username = fullUsername.Contains("\\") ? fullUsername.Split('\\')[1] : fullUsername;
        return char.ToUpper(username[0]) + username.Substring(1).ToLower();
#else
    string username = System.Environment.UserName;
    return char.ToUpper(username[0]) + username.Substring(1).ToLower();
#endif
    }

    void CopyFiles()
    {
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string folderPath = Path.Combine(desktopPath, "Package");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        else
        {
            return;
        }

        string txtFile = Path.Combine(UnityEngine.Application.streamingAssetsPath, "Package/Letter.txt");
        string gifFile = Path.Combine(UnityEngine.Application.streamingAssetsPath, "package/Gift.gif");

        string destinationTextFilePath = Path.Combine(folderPath, "Letter.txt");
        string destinationGiffFilePath = Path.Combine(folderPath, "Gift.gif");

        try
        {
            if (File.Exists(txtFile))
            {
                File.Create(destinationTextFilePath).Close();
                //File.Copy(txtFile, destinationTextFilePath, true);
                var content = File.ReadAllText(txtFile);
                content = content.Replace("{name}", GetWindowsUsername());
                File.WriteAllText(destinationTextFilePath, content);
            }

            if (File.Exists(gifFile))
            {
                File.Copy(gifFile, destinationGiffFilePath, true);
            }
        }
        catch (System.Exception)
        {
            return;
        }
    }
}
