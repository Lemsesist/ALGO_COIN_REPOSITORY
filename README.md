Authors : Sarmat Shabdanov, Muratov Bektur, Toktorbekov Melis
AlgoCoin Repository README
Overview
AlgoCoin is a simple cryptocurrency simulation project that demonstrates basic blockchain concepts such as creating wallets, transferring funds between wallets, mining blocks, and viewing balances. This project uses a simplified version of blockchain technology to manage transactions and maintain the integrity of the system.

Features
Create Wallet: Allows users to create a new wallet with a given address and optional initial balance.
Transfer Funds: Enables transferring funds between existing wallets.
Mine Blocks: Simulates the process of mining a new block based on the current state of the blockchain.
View Balance: Displays the balance of a specified wallet.
Getting Started
To run this project, ensure you have.NET Core installed on your machine. Clone the repository and open the solution in your preferred IDE.

Prerequisites
-.NET Core SDK

Installation
1)Clone the repository:
git clone https://github.com/your_username/AlgoCoin.git
2)Open the solution in Visual Studio or another compatible IDE.
3)Run the application.
Usage
Creating a Wallet
CryptoCurrency.CreateWallet("wallet_address", initial_balance);
Transferring Funds
CryptoCurrency.Transfer(amount, "sender_wallet_address", "receiver_wallet_address");
Mining a Block
Viewing Balance
CryptoCurrency.ViewBalance("wallet_address");
Contributing
Contributions are welcome Please feel free to submit a pull request.
