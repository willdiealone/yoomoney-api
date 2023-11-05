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

```zsh
dotnet add package yoomoney-api --version 1.0.0
```

# Quick start

## Access token

First of all, we need to receive an access token.
1. Log in to your YooMoney wallet with your username. If you do not have a wallet, [create it](https://yoomoney.ru/reg).
2. Go to the [App registration](https://yoomoney.ru/myservices/new) page.
3. Set the application parameters. Save CLIENT_ID and YOUR_REDIRECT_URI for the next steps.
4. Click the Confirm button.
5. Paste CLIENT_ID and REDIRECT_URI instead of YOUR_CLIENT_ID and YOUR_REDIRECT_URI. Choose scopes and run the code.
6. Follow all steps from the program.
