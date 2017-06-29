# gmail_oauth2

Transmission of Gmail corresponding to OAuth 2.0 authentication using C # and .Net framework

## Installation

Read `gmail_oauth2.sln` in Visual Studio and use it. If the Nuget package is wrong, try cleaning the solution. To run the sample, open `gmail_oauth2.sln` in Visual Studio and edit `Program.cs` in the sample project and build it.

## Features / How to use
- Get cilent ID, client secret

  First, open [Google Apis](https://console.developers.google.com/apis/). From the Api Manager tab on the left, select __[DashBoard](https://console.developers.google.com/apis/dashboard/)__ and select __ENABLE API__ in DashBoard to enable Api. Select __Google APIs > G Suites API > GmailAPI__.

  ![](https://raw.githubusercontent.com/okanon/gmail_oauth2/master/screenshots/enable_api.gif)

  Activate Gmail's API on the next screen. Since there is an item called __ENABLE__, please click on it. When you are done with that, let's select __Credentials__ from the left tab and add the authentication information. __Create Credentials > Oauth client ID > Other__. If you see `To create an Oauth client ID. You must frist set a product name on the consent screen` please specify product name.

  ![](https://raw.githubusercontent.com/okanon/gmail_oauth2/master/screenshots/create_credentials.gif)

  Select __Other__ and decide the name of client. This may be appropriate. Since creation is completed, download the client ID in json format. This is the end of work with the browser.

- Generate oauth2 access Token

  Import by referring to the built `gmail_oauth2.dll`. Then enter the email address of the Google Account that activated the API and the file path of the json that you just downloaded.

  ```
  using gmail_oauth2;

  .....

  oauth2.sync("yourmail@gmail.com", "path/your_client_id");
  ```

  If you do not have accessToken yet, you will need to authenticate with the browser when you run `oauth2.sync()`. This authentication will be displayed automatically. When authentication is completed, data is saved in xml format in the same directory as the exe being executed. Deleting this will cause the next error, so do not let it go.

- send gmail, other

  You can send gmail with `oauth2.sendGmail`. Here we are introducing text only email transmission. If you want to send an attached file, please enter the path etc. In the argument of sendGmail. Attached file transmission is described in sample.

  ```
  oauth2.sendGmail(
       new string[] { "to1@gmail.com", "to2@yahoo.co.jp" },
       "Subject",
       "Message",
       "your name"
       );

  oauth2.sendGmail(
       new string[] { "to1@gmail.com", "to2@yahoo.co.jp" },
       "Subject",
       "Message",
       "your name",
       "path/image.png",
       "image",
       "png"
       );
  ```

  It is also possible to refresh oauth2 accessToken using refreshToken. It will automatically be refreshed at the time of sign in, but you can refresh manually.

  ```
  oauth2.refresh_oauth2token();
  ```



## License

This library is under [the MIT License (MIT)](LICENSE).
