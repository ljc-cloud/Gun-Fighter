using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private static readonly string url = "http://localhost:8949/api";
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

        UnityWebRequest request = UnityWebRequest.Post($"http://localhost:8949/api/login", form);
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
                string path = Application.dataPath + "/Data/user.json";
                FileStream fileStream = File.Create(path);
                byte[] buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(user));
                fileStream.Write(buffer, 0, buffer.Length);
                Debug.Log("Login Success");
                // TODO Load Game Scene
                SceneManager.LoadScene("Level01");
            }
            else
            {
                Error.text = response.message;
            }
        }
    }
}
