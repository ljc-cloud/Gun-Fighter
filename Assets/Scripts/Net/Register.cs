using System;
using System.Collections;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    public TMP_InputField Account;
    public TMP_InputField Pwd;
    public TMP_Text Error;

    public void RegisterStart()
    {
        StartCoroutine(RegisterProcess());
    }

    public IEnumerator RegisterProcess()
    {
        Debug.Log("Waiting......");
        WWWForm form = new WWWForm();
        form.AddField("userAccount", Account.text);
        form.AddField("userPwd", Pwd.text);

        UnityWebRequest request = UnityWebRequest.Post($"http://localhost:8949/api/register", form);
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
                Debug.Log(DateTime.Now.ToString());
                string path = Application.dataPath + "/Data/user.json";
                FileStream fileStream = File.Create(path);
                byte[] buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(user));
                fileStream.Write(buffer, 0, buffer.Length);
                Debug.Log("Register Success");
                SceneManager.LoadScene("Level01");
            }
            else
            {
                Error.text = response.message;
            }
        }
    }
}
