using CryptoTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AlgoCoin
{
    public class CryptoCurrency
    {
        // Счетчик блоков
        private static int BlockCount = 0;

        // Хэш-таблицы для блокчейна и кошельков
        private static CryptoHashTable Blockchain = new CryptoHashTable();
        public static CryptoHashTable Wallets = new CryptoHashTable();

        // Класс для представления транзакции
        public class Transaction
        {
            public string Sender { get; set; }
            public string Receiver { get; set; }
            public decimal Amount { get; set; }

            public override string ToString()
            {
                return $"Transaction: {Sender} -> {Receiver}, Amount: {Amount}";
            }
        }

        /*
        ______________________________________________________________________________
        Accepts: string address, decimal initialBalance (default 0)
        Returns: void
        Purpose: Создает новый кошелек с заданным адресом и начальными средствами.
        ______________________________________________________________________________
        */
        public static void CreateWallet(string address, decimal initialBalance = 0)
        {
            if (Wallets.ContainsKey(address))
            {
                Console.WriteLine("Wallet with this address already exists.");
                return;
            }
            Wallets.AddOrUpdate(address, initialBalance.ToString());
            Console.WriteLine($"Wallet created: {address}, Balance: {initialBalance}");
        }

        /*
        ______________________________________________________________________________
        Accepts: string minerAddress
        Returns: void
        Purpose: Выполняет майнинг блока, проверяя, не пустой ли баланс майнера.
        ______________________________________________________________________________
        */
        public static void MineBlock(string minerAddress)
        {
            if (!Blockchain.ContainsKey(minerAddress))
            {
                Console.WriteLine("This address has not mined any blocks yet.");
                return;
            }

            var transactionsJson = Blockchain.GetValue(minerAddress);
            var transactions = DeserializeTransactions(transactionsJson);
            var blockHash = CalculateHash(transactions);

            // Проверка на успешный майнинг блока
            if (decimal.Parse(Wallets.GetValue(minerAddress)) == 0)
            {
                Console.WriteLine($"New block mined Block hash: {blockHash}");
                BlockCount++;
                UpdateWallets(transactions);
            }
            else
            {
                Console.WriteLine("Failed to mine block.");
            }
        }

        /*
        ______________________________________________________________________________
        Accepts: List<Transaction> transactions
        Returns: string
        Purpose: Рассчитывает хеш блока, объединяя строки транзакций и применяя SHA256.
        ______________________________________________________________________________
        */
        private static string CalculateHash(List<Transaction> transactions)
        {
            var hashString = string.Join(",", transactions.Select(t => $"{t.Sender}-{t.Receiver}-{t.Amount}"));
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashString));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        /*
        ______________________________________________________________________________
        Accepts: decimal amount, string sender, string receiver
        Returns: void
        Purpose: Выполняет перевод средств между кошельками. Проверяет существование адресов отправителя и получателя, а также баланс отправителя.
        ______________________________________________________________________________
        */
        public static void Transfer(decimal amount, string sender, string receiver)
        {
            if (!Wallets.ContainsKey(sender) || !Wallets.ContainsKey(receiver))
            {
                Console.WriteLine("One of the addresses does not exist.");
                return;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Transfer amount should be greater than zero.");
                return;
            }

            var senderBalance = decimal.Parse(Wallets.GetValue(sender));
            if (senderBalance < amount)
            {
                Console.WriteLine("Insufficient balance.");
                return;
            }

            var receiverBalance = decimal.Parse(Wallets.GetValue(receiver));
            Wallets.AddOrUpdate(sender, (senderBalance - amount).ToString());
            Wallets.AddOrUpdate(receiver, (receiverBalance + amount).ToString());
            AddTransaction(new Transaction { Sender = sender, Receiver = receiver, Amount = amount });
        }

        /*
        ______________________________________________________________________________
        Accepts: Transaction transaction
        Returns: void
        Purpose: Добавляет новую транзакцию в блокчейн, обновляя данные для всех кошельков.
        ______________________________________________________________________________
        */
        private static void AddTransaction(Transaction transaction)
        {
            foreach (var wallet in Wallets.GetKeys())
            {
                if (!Blockchain.ContainsKey(wallet))
                {
                    Blockchain.AddOrUpdate(wallet, SerializeTransactions(new List<Transaction>()));
                }
                var transactions = DeserializeTransactions(Blockchain.GetValue(wallet));
                transactions.Add(transaction);
                Blockchain.AddOrUpdate(wallet, SerializeTransactions(transactions));
            }
        }

        /*
        ______________________________________________________________________________
        Accepts: List<Transaction> transactions
        Returns: void
        Purpose: Обновляет балансы кошельков на основе списка транзакций.
        ______________________________________________________________________________
        */
        private static void UpdateWallets(List<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                if (Wallets.ContainsKey(transaction.Sender))
                {
                    var senderBalance = decimal.Parse(Wallets.GetValue(transaction.Sender));
                    Wallets.AddOrUpdate(transaction.Sender, (senderBalance - transaction.Amount).ToString());
                }
                if (Wallets.ContainsKey(transaction.Receiver))
                {
                    var receiverBalance = decimal.Parse(Wallets.GetValue(transaction.Receiver));
                    Wallets.AddOrUpdate(transaction.Receiver, (receiverBalance + transaction.Amount).ToString());
                }
            }
        }

        /*
        ______________________________________________________________________________
        Accepts: string address
        Returns: void
        Purpose: Выводит баланс указанного кошелька.
        ______________________________________________________________________________
        */
        public static void ViewBalance(string address)
        {
            if (!Wallets.ContainsKey(address))
            {
                Console.WriteLine("Address not found.");
                return;
            }

            Console.WriteLine($"Balance for {address}: {Wallets.GetValue(address)}");
        }

        /*
        ______________________________________________________________________________
        Accepts: List<Transaction> transactions
        Returns: string
        Purpose: Сериализует список транзакций в строку для хранения и передачи данных.
        ______________________________________________________________________________
        */
        private static string SerializeTransactions(List<Transaction> transactions)
        {
            return string.Join(";", transactions.Select(t => $"{t.Sender},{t.Receiver},{t.Amount}"));
        }

        /*
        ______________________________________________________________________________
        Accepts: string transactionsJson
        Returns: List<Transaction>
        Purpose: Десериализует строку транзакций в список объектов транзакций.
        ______________________________________________________________________________
        */
        private static List<Transaction> DeserializeTransactions(string transactionsJson)
        {
            var transactions = new List<Transaction>();
            if (!string.IsNullOrEmpty(transactionsJson))
            {
                var transactionStrings = transactionsJson.Split(';');
                foreach (var t in transactionStrings)
                {
                    var parts = t.Split(',');
                    if (parts.Length == 3)
                    {
                        transactions.Add(new Transaction
                        {
                            Sender = parts[0],
                            Receiver = parts[1],
                            Amount = decimal.Parse(parts[2])
                        });
                    }
                }
            }
            return transactions;
        }
    }
}
