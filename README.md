# The Remote Elevation of Privilege card game

Announcement: https://blog.logmeininc.com/logmein-releases-open-source-remote-elevation-of-privilege-eop/

Backend: .NET Core

Frontend: .NET Core - Blazor WASM PWA

Assets:
* [REOP cards/deck assets](https://github.com/adamshostack/eop) by Microsoft under [CC-BY-3.0](https://creativecommons.org/licenses/by/3.0/)
* [Cornucopia cards/deck assets](https://owasp.org/www-project-cornucopia/) by OWASP Foundation under [Creative Commons Attribution-ShareAlike 3.0](https://creativecommons.org/licenses/by-sa/3.0/)

"Supported" browser: Chrome

# How to setup and play

## Prerequisites
- [latest .NET Core 3.1 LTS SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- internet access (to download NuGet packages)

## Starting the server
From the `Server` folder, run the command:  
`dotnet run -c Release`

This command should `restore` and `build` implicitly.  
By default the application is listening on port 5000/http.  

In the Startup.cs file, commented out You can find Let's Encrypt and REDIS support - these are optional. If You want to use Let's Encrypt, don't forget to change the application url in Program.cs

## Starting a session

* Visit the self-hosted site. Please note if met with a warning on your browser (because you are using a self-signed cert, etc.), you may continue. In safari You MUST accept the cert on the first open of the site, else the websocket connection will mess up for some reason.

* Click 'Create Session' 

* You'll be prompted to enter your player name, and a checkbox to either join the game as a participant or only as a moderator. Select game mode: REOP or Cornucopia. Finish by clicking 'Create a Session'. 

* Once your session is ready, you'll be given the option to 'Copy Session ID' to share with 3-6 players on another channel. The content of the session, whiteboard and player names are transmitted with end-to-end encryption utilizing this ID - the encryption key never leaves the client side of the game, only its hash.  

* Players shall also visit the self-hosted site and click 'Join session'. They shall supply a name, and the pre-shared session ID - then click 'Join'.

* Wait in the lobby for everyone to join. When you're ready – start your session and have fun! The help menu explains some basic concepts of the game. 
