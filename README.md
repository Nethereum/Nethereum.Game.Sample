# Nethereum Game Sample

This sample game demonstrates how to integrate Nethereum with the UrhoSharp SamplyGame (game sample) to create a cross-platform game that interacts with Ethereum.

A simple [smart contract deployed to Ethereum](https://github.com/Nethereum/Nethereum.Game.Sample/blob/master/Forms/Core/Ethereum/contracts/PlayerScore.sol) allows you to tracks the users high scores and the top scores. The sample uses private keys to sign "offline" transactions and use a public node for all the communication, this way avoids the local installation of an ethereum client.

More information on UrhoSharp can be found [on the Xamarin website](https://developer.xamarin.com/guides/cross-platform/urho/introduction/), and in github [the original sample](https://github.com/xamarin/urho-samples/tree/master/SamplyGame). Kudos and all the credit goes to the Xamarin guys, specially @EgorBo and @migueldeIcaza for the great engine and sample.

![Screenshot](Screenshots/Video.gif)

## Play along!!
There is an android package [here](https://github.com/Nethereum/Nethereum.Game.Sample/tree/master/Forms/Droid).

## Forms or Original
A newer version of the sample (which will be maintained if we do any changes) is the Xamarin.Forms version. This demonstrates decoupling the logic from the game, simplification of running other threads in the background, and support of copy and paste which was not provided in the Urho UI. The sample is still very simple, so not MVVM or dependecy injection has been added.

## Small video tutorial
This video gives you a quick introduction on the sample:

* Quick overview of UrhoSharp and cross-platform development
* Overview of public nodes like infura.io
* Overview of private keys
* Overview of the solution structure
* Overview of the smart contract, .net service, offline transaction signing.

[![Cross platform game development with Ethereum using UrhoSharp and .Net](http://img.youtube.com/vi/WtpmCmP11Iw/0.jpg)](https://www.youtube.com/watch?v=WtpmCmP11Iw "Cross platform game development with Ethereum using UrhoSharp and .Net")

Note: in this sample, a special INFURA API key is used: `7238211010344719ad14a89db874158c`. If you wish to use this sample in your own project you’ll need to [sign up on INFURA](https://infura.io/register) and use your own key.
