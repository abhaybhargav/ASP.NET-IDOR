# IDOR Demo App

This application demonstrates Insecure Direct Object Reference (IDOR) vulnerabilities and how to prevent them. It's designed for educational purposes to help developers understand the importance of proper access controls in web applications.

## Features

- User registration and authentication
- Adding and viewing sensitive information
- Toggling between secure and insecure modes to demonstrate IDOR vulnerabilities
- Dockerized application for easy deployment

## Prerequisites

- Docker

## Getting Started

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/IDORDemoApp.git
   cd IDORDemoApp
   ```

2. Build the Docker image:
   ```
   docker build -t idor-demo-app .
   ```

3. Run the Docker container:
   ```
   docker run -p 88800:88800 idor-demo-app
   ```

4. Access the application in your web browser at `http://localhost:88800`

## Using the Application

1. **Sign Up**: Create a new account by clicking on the "Sign Up" link and filling out the form.

2. **Log In**: After signing up, log in using your credentials.

3. **Dashboard**: Once logged in, you'll be taken to your dashboard where you can:
   - Add new sensitive information
   - View your existing sensitive information
   - Toggle between secure and insecure modes

4. **Adding Sensitive Information**: Use the form on the dashboard to add new sensitive information.

5. **Viewing Sensitive Information**: Click on the links in your dashboard to view individual pieces of sensitive information.

6. **Toggling Security Mode**: Use the button on the dashboard to switch between secure and insecure modes.

7. **Demonstrating IDOR Vulnerability**:
   - In insecure mode, note the ID of your sensitive information.
   - Open an incognito window and create a second user account.
   - Log in as the second user and try to access the first user's sensitive information by manually entering the URL: `http://localhost:88800/User/ViewSensitiveInfo/{id}` (replace {id} with the noted ID).
   - In insecure mode, you'll be able to view the information. In secure mode, you'll get an access denied message.

## Security Modes

- **Insecure Mode**: Demonstrates the IDOR vulnerability by allowing access to any sensitive information just by knowing its ID.
- **Secure Mode**: Implements proper access controls, ensuring users can only access their own sensitive information.

## Secure vs Insecure Implementation

The key difference between the secure and insecure implementations lies in how sensitive information is retrieved from the DataStore. Here are the crucial lines of code:

1. In the DataStore.cs file:

```csharp
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
```

This method takes a `secure` parameter. When `true`, it only returns sensitive information belonging to the specified user. When `false`, it returns all sensitive information, regardless of the user.

2. In the UserController.cs file:

Insecure Dashboard:
```csharp
var sensitiveInfoList = _dataStore.GetSensitiveInfoForUser(userId.Value, false);
```

Secure Dashboard:
```csharp
var sensitiveInfoList = _dataStore.GetSensitiveInfoForUser(userId.Value, true);
```

The insecure version passes `false`, retrieving all sensitive information, while the secure version passes `true`, retrieving only the user's own information.

3. In the ViewSensitiveInfo action:

```csharp
public IActionResult ViewSensitiveInfo(int id)
{
    int? userId = HttpContext.Session.GetInt32("UserId");
    SensitiveInfo info;
    
    if (ViewBag.IsSecureMode)
    {
        info = _dataStore.GetSensitiveInfo(id, userId);
    }
    else
    {
        info = _dataStore.GetSensitiveInfo(id);
    }

    if (info == null)
    {
        return NotFound();
    }

    return View(info);
}
```

In secure mode, the `userId` is passed to `GetSensitiveInfo`, ensuring only the user's own information is accessible. In insecure mode, no user check is performed, allowing access to any sensitive information by ID.

These implementations demonstrate how proper access controls can prevent Insecure Direct Object Reference (IDOR) vulnerabilities.

## Warning

This application intentionally contains security vulnerabilities for educational purposes. Do not use any real sensitive information while testing this application.

## Contributing

Feel free to submit issues and pull requests to improve the application or add more security demonstrations.

## License

This project is open source and available under the [MIT License](LICENSE).