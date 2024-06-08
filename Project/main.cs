using System;
using AlgoCoin;
class Program {
  public static void Main (string[] args) {
    // Инициализация кошельков
    CryptoCurrency.Wallets.AddOrUpdate("Alice", "1000");
    CryptoCurrency.Wallets.AddOrUpdate("Bob", "500");
    CryptoCurrency.Wallets.AddOrUpdate("Bob2", "0");

    // Перевод средств
    CryptoCurrency.Transfer(50, "Alice", "Bob");

    // Просмотр баланса
    CryptoCurrency.ViewBalance("Alice");
    CryptoCurrency.ViewBalance("Bob");


    CryptoCurrency.MineBlock("Bob2");
  }
}