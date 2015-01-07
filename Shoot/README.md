Shoot 'n' Share
===============
Author: Erik Jan de Wit (edewit)
Level: Beginner  
Technologies: C#, Windows Phone
Summary: A full example of Oauth2: Login and upload
Target Product: -   
Product Versions: -   
Source: https://github.com/edewit/aerogear-windows-cookbook/tree/master/Shoot

## What is it?

The `Shoot 'n' Share` project demostrates how to include OAuth2 functionality in a Windows Phone application. Before you can run and use this app you'll need to setup the providers `Google`, `Keycloak` and `Facebook`

### 1. Google credentials

First, you must authenticate with Google. You will need a Google Account for this. Now open the [Google Console](http://console.developer.google.com).

If you haven't created a project, click "Create Project" and give the new project a name

Go to APIs & auth menu, then select APIs and turn on Drive API

Always in APIs & auth menu, select Credentials and hit create new client id button Select iOS client and enter something for  bundle id can be anything as long as it contains at least one `.` (if you want to use facebook as well you'll have to use `fbXXX.` where xxx is the clientId (App ID), becuase for facebook you cannot set this)

Once completed you will have your information displayed as below:

![Google Cloud client registration](https://github.com/aerogear/aerogear-ios-cookbook/raw/master/Shoot/shoot_google_cloud_admin.png "Google Cloud client registration")

Open the Shoot project in Visual Studio and open the `Package.appxmanifest` file and set the `Protocol` to the bundle id you've just entered.

### 2. Keycloak credentials

You will need an instance of Keycloak running locally please refer to [aerogear-backend-cookbook shoot recipe](https://github.com/aerogear/aerogear-backend-cookbook/tree/master/Shoot).

Windows OAuth can only be used with ssl enabled for local testing you can use a self signed certificate. Setup [keycloak to use ssl.](http://docs.jboss.org/keycloak/docs/1.0-alpha-1/userguide/html/server-installation.html#d4e115)

When you use a self signed certificate you'll need to install it on the phone/emulator to be able to use it. You can use the keytool to export it or Chrome.

```
keytool -export -keystore keycloak.jks -alias localhost -file keycloak.cer
```

![Click on the lock symbol](img/certificate-export1.png)
![Copy to file](img/certificate-export2.png)

Then put in the `keycloak.cer` file into the war file and install it on the phone by visiting `http://<ip>:8080/shoot/keycloak.cer`

### 3. Facebook credentials
#### Step1: Setup facebook to be a facebook developer:

- Go to [Facebook dev console](https://developers.facebook.com/products/login/)
- Click Apps->Register as a Developer
- enter password
- accept policy
- send confirmation code to SMS
- once received enter code

#### Step2: Create a new app on facebook console

- Click apps-> Create a new app
- add display name: Shoot
- deal with difficult catcha
- configure Advanced setup:
	- Native or desktop app? NO
	- Client OAuth Login YES
	- Embedded browser OAuth Login YES
    
In the `MainPage.xaml.cs` replace XXX with 'App ID' and YYY with 'App Secret'. Then in the `Package.appxmanifest` file and set the `Protocol` to be fbYYY

## How it works

When you start the application you can take picture or select one from your library.

Once an image is selected, you can share it. Doing so, you trigger the OAuth2 authorization process. Once successfully authorized, your image will be uploaded.

More info on how the OAuth library works have a look at the [library location](https://github.com/edewit/aerogear-windows-oauth2)