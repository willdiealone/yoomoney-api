# API Yoomoney - Unofficial C# Library
![Logo](https://imgur.com/4tWrKD2.png)
## Summary
- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
  1. [Access Token](#access-token)
  3. [Account Information](#account-information)
  4. [Operation History](#operation-history)
  5. [Operation Details](#operation-details)
  6. [Quickpay](#quickpay)
## Introduction
**This library will allow you to:**
- **Generate a payment link and process the payment notification asynchronously**
- **Access account information**
- **Get access to payment history**
- **Access payment details**
## Features

Implemented methods:

- [Access Token](#access-token) - Getting an access token
- [Account Information](#account-information) - Getting information about the status of the user account.
- [Operation History](#operation-history) - This method allows viewing the full or partial history of operations in page mode. History records are displayed in reverse chronological order (from most recent to oldest).
- [Operation Details](#operation-details) - Provides detailed information about a particular operation from the history.
- [Quickpay](#quickpay) - The YooMoney form is a set of fields with information about a transfer. You can embed the payment form into your interface (e.g., a website or blog). When the sender pushes the button, the details from the form are sent to YooMoney, and an order for a transfer to your wallet is initiated.
## Installation

You can install it with .Net CLI command:

```csharp
dotnet add package yoomoney-api --version 1.4.0
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

Authorize authorize = new(clientId:"YOUR_CLIENT_ID",redirectUl:"YOUR_REDIRECT_URL",scope:new []
{
    "account-info",
    "operation-history",
    "operation-details",
    "incoming-transfers",
    "payment-p2p",
});
```
2. You get a link in the console to navigate to, but if you want to use it somewhere else, the Authorize class constructor initializes the AuthorizeUrl property with that link.
```csharp
authorize.AuthorizeUrl
```
3. Visit this website and confirm the application authorization request.
```csharp
https://yoomoney.ru/oauth/authorize?client_id=XXXXXXXXXXXXXXXXXXXXXXXXXX
``` 
4. You follow the generated link, enter the code that you will receive, after which you will be redirected to YOUR_REDIRECT_URL.
5. Once the redirect to YOUR_REDIRECT_URL is complete, you copy the address bar of that page you're currently on (but keep in mind that the lifetime of that address is a minute) and pass that address as a method parameter `GetAccessToken` and the access token we need will be returned to the `token` variable.

Example url
```csharp
https://example/k/?code=A54AB5755DFA80B0167532E413C87F90CBD8677C72758EAAD6E7F1AAD341FEBEBAD7B3754D2A6E42101029C134E55CB55A382412D953497D9CE5FCC7F96FE47B92615B0167BA727E49DC81F21A36312FDF440CAD5A1813E9849167C5B7307661504D134A432DDB727FDA302E040326425F82D41F3237FCFD6A9A6DE3C904D4A1
```
Copy and paste this code  
```csharp
var token = await authorize.GetAccessToken(code: "YOUR_СODE", clientId: "YOUR_CLIENT_ID", redirectUri: "YOUR_REDIRECT_URL");
```
6. Your access token
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

var client = new Client(token:authorize.TokenUrl);
var accountInfo = client.GetAccountInfo(token:YOUR_TOKEN);
accountInfo.Print();
```
## Output:
``` csharp
Account number:                             550019014512302
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
var operationHistory = client.GetOperationHistory(token:YOUR_TOKEN);
operationHistory.Print();
```
## Output:
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

## Operation details
Paste YOUR_TOKEN with an OPERATION_ID (example: 752413347835145104) from previous example output and run this code:
```csharp
var operationDetails = client.GetOperationDetails(token:YOUR_TOKEN,operationId:operationId);
operationDetails.Print();
```

## Output:
This way we can check whether the payment went through, generate a link for transfer using the card number, and then receive the transaction history and check (status, datetime, label).
```csharp
Operation details:
        operation_id           --> 752413347835145104
        status                 --> success
        pattern_id             --> p2p
        direction              --> in
        amount                 --> 145,50
        amount_due             --> Null
        fee                    --> Null
        datetime               --> 04.11.2023 11:42:27
        title                  --> Пополнение с карты ****5769
        sender                 --> Null
        recipient              --> Null
        recipient_type         --> Null
        message                --> Перевод по кнопке
        comment                --> Null
        coderpo                --> Null
        protection_code        --> Null
        expires                --> Null
        answer_datetime        --> Null
        label                  --> b5c57192-e7d0-4ecc-8d9d-623b8426890d
        details                --> Пополнение с банковской карты, операция №2cd841d2-0011-5000-a000-1c1da48c5f72.Банковская карта: ****5769.
        type                   --> deposition
        DigitalBonus is empty
        DigitalProduct is empty
```

## Quickpay
[![YouTube video about yoomoney-api]( https://img.shields.io/badge/watch%20on-YouTube-blue?logo=youtube&style=for-the-badge)](https://youtu.be/c9gv_IWgV4Y)
<p>	<a href="https://youtu.be/c9gv_IWgV4Y">
    <img src="https://imgur.com/5z1XqIl.png" width="600px">
</a>

</p>

The first step is to install our
[notifications](https://yoomoney.ru/transfer/myservices/http-notification)
in the YooMoney client.
Enter the address to which you will receive notifications, and also be sure to save the secret key, we will need it for the next step.
![dwdw](https://imgur.com/MpB25LF.png)
After this we need to generate a payment link, copy and paste this code

Label - make it unique for each payment.
Run this code:
```csharp
var label = Guid.NewGuid().ToString();
var quickpay = new Quickpay(receiver: "4100118408605024", quickpayForm: "shop", sum: 10, 
	label: label, paymentType: "AC"); //Payment method. Possible values: PC - payment from the YuMoney wallet; AC - from a bank card.
WriteLine(quickpay.LinkPayment);

//replace --> ("YOUR_IP_ADDRESS_OR_DNS_NAME","NOTIFICATION_SECRET",YOUR_PORT")
PaymentListenerToYooMoney paymentListenerToYooMoney = new(label,DateTime.Today,"NOTIFICATION_SECRET");
var resultPayment = await paymentListenerToYooMoney.Listen("YOUR_IP_ADDRESS_OR_DNS_NAME","YOUR_PORT");
WriteLine(resultPayment);
```
## Output:
```csharp
https://yoomoney.ru/transfer/quickpay?requestId=353432303336353332305f39366662343561383966646635393039633365396165366566656231366237383762333062346237

IP-appec: XXX.X.X.X // Доменное имя: https://XXXXXXXXXXX, IP: XX.XXX.XXX.XX
Сервер запущен. Ожидание подключений...
```
Follow the link, pay and wait for notifications from YooMooney,it will arrive immediately after a notification on your phone.

## Output:
```csharp
Подключен клиент.
HTTP Requests
-------------

POST /
Текущий платеж:
        NotificationType   --> card-incoming
        OperationId        --> 753525659460074104
        DateTime           --> 2023-11-17 12:40
        Amount             --> 9.70
        WithdrawAmount     --> 10.00
        Firstname          -->  Null
        Lastname           -->  Null
        Fathersname        -->  Null
        Email              -->  Null
        Phone              -->  Null
        City               -->  Null
        Street             -->  Null
        Building           -->  Null
        Suite              -->  Null
        Flat               -->  Null
        Zip                -->  Null
        Sender             -->  Null
        Unaccepted         --> false
        Codepro            --> false
        Currency           --> 643
        Label              --> eb0db09d-7354-48e9-abf8-bceef21c0b01

Успешно!
Сервер завершил работу
```
Contact information is available only by protocol HTTPS
```csharp
var quickpay = new Quickpay(
	receiver: "4100118408605024",
	quickpayForm: "shop",
	sum: 10, 
	label: label,
	paymentType: "AC",
	firstname:"Oleg",
	lastname:"Olegov",
	fathersname:"Olegovich",
	city:"Saint Petersburg",
	street:"Begovaya street",
	zip:"197374",
	building:"11",
	suite:"1",
	flat:"43",
	phone:"+7987674115",
	sender:"4100167987654");

//replace --> ("YOUR_IP_ADDRESS_OR_DNS_NAME","NOTIFICATION_SECRET",YOUR_PORT")
PaymentListenerToYooMoney paymentListenerToYooMoney = new(label,DateTime.Today,"NOTIFICATION_SECRET");
var resultPayment = await paymentListenerToYooMoney.Listen("YOUR_IP_ADDRESS_OR_DNS_NAME","YOUR_PORT");
WriteLine(resultPayment);
```
## Output:

```csharp
Подключен клиент.
HTTP Requests
-------------

POST /
Текущий платеж:
        NotificationType   --> card-incoming
        OperationId        --> 753525659460074104
        DateTime           --> 2020-01-12 01:22
        Amount             --> 9.70
        WithdrawAmount     --> 10.00
        Firstname          -->  Oleg
        Lastname           -->  Olegov
        Fathersname        -->  Olegovich
        Email              -->  Oleg@gmail.com
        Phone              -->  +79957773555
        City               -->  Saint Petersburg
        Street             -->  Begovaya street
        Building           -->  11
        Suite              -->  1
        Flat               -->  43
        Zip                -->  197374
        Sender             -->  4100167987654
        Unaccepted         --> false
        Codepro            --> false
        Currency           --> 643
        Label              --> 560bb09d-5986-38e9-abf8-cl59f21c0bh5

Успешно!
Сервер завершил работу
```
