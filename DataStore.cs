using System;
using System.Collections.Generic;
using System.Linq;

public class DataStore
{
    private List<User> users = new List<User>();
    private List<SensitiveInfo> sensitiveInfos = new List<SensitiveInfo>();
    private bool secureMode = false;



    public void AddUser(string username, string password)
    {
        users.Add(new User { Id = users.Count + 1, Username = username, Password = password });
    }

    public User AuthenticateUser(string username, string password)
    {
        return users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    public int AddSensitiveInfo(int userId, string info)
    {
        var newInfo = new SensitiveInfo { Id = sensitiveInfos.Count + 1, UserId = userId, Info = info };
        sensitiveInfos.Add(newInfo);
        return newInfo.Id;
    }

    public SensitiveInfo GetSensitiveInfo(int infoId, int? userId = null)
    {
        if (secureMode)
        {
            return sensitiveInfos.FirstOrDefault(s => s.Id == infoId && s.UserId == userId);
        }
        else
        {
            return sensitiveInfos.FirstOrDefault(s => s.Id == infoId);
        }
    }

    public List<SensitiveInfo> GetSensitiveInfoForUser(int userId, bool secure)
    {
        if (secure)
        {
            return sensitiveInfos.Where(s => s.UserId == userId).ToList();
        }
        else
        {
            return sensitiveInfos.ToList();
        }
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class SensitiveInfo
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Info { get; set; }
}