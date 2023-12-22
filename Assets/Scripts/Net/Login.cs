using System;
using System.Collections;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    private static readonly string url = "http://47.92.94.83:8949/api";
    public TMP_InputField Account;
    public TMP_InputField Pwd;
    public TMP_Text Error;

    public void LoginStart()
    {
        StartCoroutine(LoginProcess());
    }

    public IEnumerator LoginProcess()
    {
        Debug.Log("Waiting......");
        WWWForm form = new WWWForm();
        form.AddField("userAccount", Account.text);
        form.AddField("userPwd", Pwd.text);

        UnityWebRequest request = UnityWebRequest.Post($"{url}/login", form);
        yield return request.SendWebRequest();
        Debug.Log(Encoding.UTF8.GetString(request.downloadHandler.data));
        if (string.IsNullOrEmpty(request.error))
        {
            var res = request.downloadHandler.text;
            var response = JsonUtility.FromJson<ServerResponse<User>>(res);
            if (response.code == 200)
            {
                User user = response.data;
                user.timeStamp = DateTime.Now.ToString();
                string dirPath = Application.persistentDataPath + "/Data";
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                string path = $"{dirPath}/user.json";
                FileStream fileStream = File.Create(path);
                byte[] buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(user));
                fileStream.Write(buffer, 0, buffer.Length);
                Debug.Log("Login Success");
                // Load Game Scene
                SceneManager.LoadScene("Level01");
            }
            else
            {
                Error.text = response.message;
            }
        }
    }
}
