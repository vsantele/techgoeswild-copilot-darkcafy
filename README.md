# Tech Goes Wild: Copilot Cafy

Project realised during the Tech Goes Wild hackathon, organised by [MIC belgique](https://mic-belgique.com). I was there as a coach but I manage to do this little project.

## The project

This project is a legacy of the DarkCafy project. A student project to automate a coffee machine. The last version used Facial Recognition to identify the user and the coffee he wanted.

This version used LLM to take action based on the user's request. The user can ask for a coffee, but also to play a song with his Spotify account.

It's a chatbot that makes coffee, literally. (And it can play music via Spotify)

It was an opportunity to discover [Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/overview/?wt.mc_id=studentamb_236461). A framework that allows you to create agent that can call code easily.

## Why a flutter app?

The flutter app is not an interface for the chatbot (but it will be in the future). It's the only way I found to establish a connection between the chatbot and the coffee machine. The chatbot running on my computer sends a http request to a webserver embedded inside the flutter that will send a signal to the coffee machine to brew.

## How to install it?

- Install [dotnet 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- Install [flutter](https://docs.flutter.dev/get-started)
- Get a compatible coffee machine (The model used is a [Delonghi Dinamica PLUS 370.95.T](https://www.amazon.fr/DeLonghi-370-95T-370-95-T-DINAMICA-Titane/dp/B07GGZBRZX?th=1))
- Get an android phone (not tested on iOS)
- Get a premium Spotify account (If you want to play music)
- Create a developper app on Spotify and get the client id
- Clone the repository

```bash
git clone https://github.com/vsantele/techgoeswild-copilot-darkcafy
```

### The chatbot

- Go to `semanticKernel` folder

```bash
cd semanticKernel
```

- Install the dependencies

```bash
dotnet restore
```

- Follow the instructions to setup secrets in the [README.md](semanticKernel/README.md#configuring-the-starter)

### The flutter app

- Go to `flutter` folder

```bash
cd flutter
```

- Update the device name in `lib/services/dark_cafy.dart`
- Connector your phone to your computer
- Install the dependencies

```bash
flutter pub get
```

## How to use it?

- Start the flutter app

```bash
cd flutter
flutter run
```

- Start the chatbot in another terminal

```bash
cd semanticKernel
dotnet run
```

- Ask for a coffee or a song

```
I would like a coffee.

I'm tired, I need energy to work.

Can you play me a song to wake me up?
```

## What's next?

- Add a real interface to the chatbot
- Improve the coffee machine connection
- Improve music playing with device auto detection
- More?
