[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.Facebook.Auth/master/Shared/Icon.png "Zebble.Facebook.Auth"


## Zebble.Facebook.Auth

![logo]

A Zebble plugin that allow you to Login with Facebook.


[![NuGet](https://img.shields.io/nuget/v/Zebble.Facebook.Auth.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.Facebook.Auth/)

> With this plugin you can register with Facebook and allow user to logon with Facebook account in Zebble applications.

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.Facebook.Auth/](https://www.nuget.org/packages/Zebble.Facebook.Auth/)
* Install in your platform client projects.
* Available for iOS, Android and UWP.
<br>


### Api Usage

#### Login with facebook
You should first go to https://developers.facebook.com/apps and register as a facebook developer.<br>
Create a new App for the application.<br>
Get the App ID assigned to you. for example: 124495023432023, and add to `config.xml`
```xml
<Facebook.App.Id>124495023432023</Facebook.App.Id>
```
In facebook Product Setup page, click "Get Started" for "Facebook Login". Ensure only the following are enabled:

- "Client OAuth Login"
- Embedded BRowser OAuth Login
- Register with Facebook button

### Platform Specific Notes
Some platforms require some setting to make you able to use this plugin.

#### Android
Create strings for your Facebook app ID and for those needed to enable Chrome Custom Tabs. Also, add FacebookActivity to your Android manifest.
Open your Android project Resources/values/strings.xml file and add the following:
```xml
<string name="facebook_app_id">[APP_ID]</string>
<string name="fb_login_protocol_scheme">fb[APP_ID]</string>
```

Open the Android project properties `AndroidManifest.xml` file and add the following uses-permission element after the application element:
   
```xml       
<uses-permission android:name="android.permission.INTERNET"/>
```
So your AndroidManifest file should be look like below:

```xml
<application ...>
    <meta-data android:name="com.facebook.sdk.ApplicationId"
            android:value="@string/facebook_app_id"/>

    <activity android:name="com.facebook.FacebookActivity"
        android:configChanges=
                "keyboard|keyboardHidden|screenLayout|screenSize|orientation"
        android:label="@string/app_name" />
    <activity
        android:name="com.facebook.CustomTabActivity"
        android:exported="true">
      <intent-filter>
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="@string/fb_login_protocol_scheme" />
      </intent-filter>
    </activity>
</application>
```
 
#### iOS
In your IOS project add these configuration to your `info.plist` file:

If your code does not have `CFBundleURLTypes` add the following just before the final `</dict>` element:

```xml
<key>CFBundleURLTypes</key>
<array>
  <dict>
  <key>CFBundleURLSchemes</key>
  <array>
    <string>fb[APP_ID]</string>
  </array>
  </dict>
</array>
<key>FacebookAppID</key>
<string>[APP_ID]</string>
<key>FacebookDisplayName</key>
<string>[APP_NAME]</string>
```
If your code already contains `CFBundleURLTypes` insert the following:

```xml
<array>
  <dict>
  <key>CFBundleURLSchemes</key>
  <array>
    <string>fb[APP_ID]</string>
  </array>
  </dict>
</array>
<key>FacebookAppID</key>
<string>[APP_ID]</string>
<key>FacebookDisplayName</key>
<string>[APP_NAME]</string>
```

In the XML snippet, replace the following:

* [APP_ID] with the App ID of your app.
* [APP_NAME] with the name of your app.

In the app if you want to read the user's details from facebook, you can then read the data using:

```csharp
Facebook.GetInfo(new Facebook.Field[] { Facebook.Field.Email , ... }, result =>
{
    //your application code.
    //result is type of Facebook.User
});
```
#### Log in with Facebook button
In the app if you want to allow logging in with facebook, then a secure approach is the following:

##### Button code:
```csharp
Facebook.OnSuccess.Handle(user =>
{
    if (user != null)
    {
        //application code
    }
});
await Facebook.Login(Facebook.Field.Email, ... );
```
##### API Code:

 ```csharp
[HttpPost, Route("login/facebook/{accessToken}")]
public IHttpActionResult LoginWithFacebook(string accessToken)
{
      var json = ("https://" + "graph.facebook.com/me?fields=email&access_token=" + accessToken).AsUri().Download();
      var email = JsonConvert.DeserializeObject<JObject>(json)?.Property("email")?.Value?.ToString();

      if (email.IsEmpty()) return BadRequest("Invalid email.");
      var member = Database.Find<Member>(m => m.Email == email);

      if (member == null) return BadRequest($"The email '{email}' is not registered in our system. Please register.");

      var result = JwtAuthentication.CreateTicket(member);
      return Ok(result);
}
```

### Methods
| Method       | Return Type  | Parameters                          | Android | iOS | Windows |
| :----------- | :----------- | :-----------                        | :------ | :-- | :------ |
| Login         | Task<string&gt;| fields -> Field[] | x       | x   | x       |
| GetInfo       | Task<User&gt;| accessToken -> string, fields -> Field[] | x       | x   | x       |