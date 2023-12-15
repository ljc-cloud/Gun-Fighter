using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PageManage : MonoBehaviour
{

    public static PageManage Instance;

    private Stack<Canvas> stack = new Stack<Canvas>();

    public Canvas Main;
    public Canvas Login;
    public Canvas Register;
    public Canvas Select;


    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        stack.Push(Main);
        Canvas canvas = stack.Peek();
        canvas.gameObject.SetActive(true);
    }

    public void ToSelect()
    {
        stack.Peek().gameObject.SetActive(false);
        stack.Push(Select);
        stack.Peek().gameObject.SetActive(true);
    }

    public void PlayClicked()
    {
        string path = $"{Application.dataPath}/Data/user.json";
        if (!File.Exists(path))
        {
            ToSelect();
            return;
        }
        FileStream fs = File.Open(path, FileMode.Open);
        byte[] buffer = new byte[200];
        fs.Read(buffer, 0, (int)fs.Length);
        string json = Encoding.UTF8.GetString(buffer);
        User user = JsonUtility.FromJson<User>(json);
        DateTime time = DateTime.Parse(user.timeStamp);
        TimeSpan span = DateTime.Now.Subtract(time);
        if (span.Minutes <= 120)
        {
            // TODO Load Game Scene
            SceneManager.LoadScene("Level01");
        }
    }

    public void LoginClicked()
    {
        ToLogin();
    }

    public void RegisterClicked()
    {
        ToRegister();
    }

    private void ToLogin()
    {
        stack.Peek().gameObject.SetActive(false);
        stack.Push(Login);
        stack.Peek().gameObject.SetActive(true);
    }

    private void ToRegister()
    {
        stack.Peek().gameObject.SetActive(false);
        stack.Push(Register);
        stack.Peek().gameObject.SetActive(true);
    }

    public void Back()
    {
        if (stack.Count <= 1)
            return;
        Canvas canvas = stack.Pop();
        canvas.gameObject.SetActive(false);
        stack.Peek().gameObject.SetActive(true);
    }
}
