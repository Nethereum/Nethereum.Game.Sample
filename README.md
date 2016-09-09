#Nethereum Game Sample

This sample game demonstrates how to integrate Nethereum with the UrhoSharp SamplyGame (game sample) to create a cross-platform game that interacts with Ethereum.

A simple [smart contract deployed to Ethereum](https://github.com/Nethereum/Nethereum.Game.Sample/blob/master/Core/Ethereum/contracts/PlayerScore.sol) allows you to tracks the users high scores and the top scores. The sample uses private keys to sign "offline" transactions and use a public node for all the communication, this way avoids the local installation of an ethereum client.

More information on UrhoSharp can be found [on the Xamarin website](https://developer.xamarin.com/guides/cross-platform/urho/introduction/), and in github [the original sample](https://github.com/xamarin/urho-samples/tree/master/SamplyGame).

## Small video tutorial
This video gives you a quick introduction on the sample:

* Quick overview of UrhoSharp and cross-platform development
* Overview of public nodes like infura.io
* Overview of private keys
* Overview of the solution structure
* Overview of the smart contract, .net service, offline transaction signing.

[![Cross platform game development with Ethereum using UrhoSharp and .Net](http://img.youtube.com/vi/WtpmCmP11Iw/0.jpg)](https://www.youtube.com/watch?v=WtpmCmP11Iw "Cross platform game development with Ethereum using UrhoSharp and .Net")

![Screenshot](Screenshots/Video.gif)
