# API Yoomoney - Unofficial C# Library
This is an unofficial [YooMoney](https://yoomoney.ru) API Python library.
## Summary
- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
  1. [Access Token](#access-token)
  2. [Account Information](#account-information)
  3. [Operation History](#operation-history)
  4. [Operation Details](#operation-details)
  5. [Quickpay Forms](#quickpay-forms)
## Introduction
This repository is based on the official documentation of [YooMoney](https://yoomoney.ru/docs/wallet).
## Features

Implemented methods:

- [Access Token](#access-token) - Getting an access token
- [Account Information](#account-information) - Getting information about the status of the user account.
- [Operation History](#operation-history) - This method allows viewing the full or partial history of operations in page mode. History records are displayed in reverse chronological order (from most recent to oldest).
- [Operation Details](#operation-details) - Provides detailed information about a particular operation from the history.
- [Quickpay](#quickpay) - The YooMoney form is a set of fields with information about a transfer. You can embed the payment form into your interface (e.g., a website or blog). When the sender pushes the button, the details from the form are sent to YooMoney, and an order for a transfer to your wallet is initiated.
## Installation

You can install it with:

```csharp
dotnet add package yoomoney-api --version 1.0.0
```

# Quick start

1. Log in to your YooMoney wallet with your username. If you do not have a wallet, [create it](https://yoomoney.ru/reg).
2. Go to the [App registration](https://yoomoney.ru/myservices/new) page.
   ![App registration2](https://i.imgur.com/jroGrUL.png)
3. Follow the example, enter all the required data and click the "all correct" button   
4. Set the application parameters. Save CLIENT_ID and YOUR_REDIRECT_URI for the next steps.
5. Click the Confirm button.
6. Paste CLIENT_ID and REDIRECT_URI instead of YOUR_CLIENT_ID and YOUR_REDIRECT_URI. Choose scopes and run the code.
7. Follow all steps from the program.

## Access token
Authorization proceeds as follows:

1. Paste this code and run it
```csharp
using yoomoney_api.authorize;

Authorize authorize = new("YOUR_CLIENT_ID", "YOUR_REDIRECT_URL", new []
{
    "account-info",
    "operation-history",
    "operation-details",
    "incoming-transfers",
    "payment-p2p",
});
```
2. You receive a link to follow

```csharp
Visit this website and confirm the application authorization request:
https://yoomoney.ru/oauth/authorize?client_id=XXXXXXXXXXXXXXXXXXXXXXXXXX
``` 
3. You follow the generated link, enter the code that you will receive, after which you will be redirected to YOUR_REDIRECT_URL.
4. The redirect completion field to YOUR_REDIRECT_URL, you copy the address bar of this page you are currently on, keep in mind that this address has a lifetime of a minute.
```csharp
https://example/k/?code=A54AB5755DFA80B0167532E413C87F90CBD8677C72758EAAD6E7F1AAD341FEBEBAD7B3754D2A6E42101029C134E55CB55A382412D953497D9CE5FCC7F96FE47B92615B0167BA727E49DC81F21A36312FDF440CAD5A1813E9849167C5B7307661504D134A432DDB727FDA302E040326425F82D41F3237FCFD6A9A6DE3C904D4A1
```
  and 
```csharp
Enter the full address of the page after the redirect=
=>  https://web.telegram.org/k/?code=EA9C91AA678ED7D4E151B83F51040B227EA57D60BC9D26965C684C111645A1B256877608FF624AC68AFE770D8236B8DF3C344D6AE9D0F74131B203E2D0FA9D2B31683F9E6778CC18505EBC9E5949710BC2CD079F3152101AD4AA3D78378C2E9AA1F96993AFB24526788D2CB178E0E22ADF3198FEFD38C95A3053780C73464D8F
```
5. Your access token
```csharp
Your access token:
4100118408605024.16F0ADB9BFE2156AF44828F2B7A7347A146B487DF8AF88343832A44F39691B888E3FFAEFE6087AD8F8C425809360F712E8A9BE9C1EC0B1906A967413A8FD66A132D786C4097D8EA4D60F086666FDABEF0FD89EFDCFB29CA4936A10E7F89463C337DED49799349B0D3A8581F7D7434A0938F3E0A9E75256752C4A78484630762A
```
You are done with the most difficult part!

## Account information
You can copy the received token or get it from the Token Url property of the Authorize class after authorization.
Paste YOUR_TOKEN and run this code:

```csharp
using yoomoney_api.account;
using yoomoney_api.authorize;

var client = new Client(authorize.TokenUrl);
var user = client.GetAccountInfo();
user.Print();
```
## Output:
``` csharp
Account number:                             410019014512803
Account balance:                            999999999999.99
Account currency code in ISO 4217 format:   643
Account status:                             identified
Account type:                               personal
Extended balance information:
        --> total:                   999999999999.99
        --> available:               999999999999.99
        --> deposition_pending:      Null
        --> blocked:                 Null
        --> debt:                    Null
        --> hold:                    Null
Information about linked bank cards:
        No card is linked to the account
```

## Operation history
Paste YOUR_TOKEN and run this code:

```csharp
var userHistory = client.GetOperationHistory();
userHistory.Print();
```

```csharp
List of operations:

        operation id:         --> 752413347835145104
        status                --> success
        datetime              --> 04.11.2023 11:42:27
        title                 --> Пополнение с карты ****5769
        pattern id            --> Null
        direction             --> in
        amount                --> 145,50
        label                 --> b5c57192-e7d0-4ecc-8d9d-623b8426890d
        type                  --> deposition
        amount_currency       --> RUB
        is_sbp_operations     --> false
```
