using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleBankingApp
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Account> Accounts { get; set; } = new List<Account>();
    }

    public class Account
    {
        public int AccountNumber { get; }
        public string AccountHolderName { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public DateTime LastInterestAdded { get; set; }

        public Account(int accountNumber, string accountHolderName, string accountType, decimal initialDeposit)
        {
            AccountNumber = accountNumber;
            AccountHolderName = accountHolderName;
            AccountType = accountType;
            Balance = initialDeposit;
            LastInterestAdded = DateTime.Now;
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
            Transactions.Add(new Transaction("Deposit", amount, Balance));
        }

        public bool Withdraw(decimal amount)
        {
            if (amount > Balance)
            {
                Console.WriteLine("Insufficient balance.");
                return false;
            }
            Balance -= amount;
            Transactions.Add(new Transaction("Withdrawal", amount, Balance));
            return true;
        }

        public void AddInterest(decimal rate)
        {
            if (AccountType.ToLower() == "savings" && (DateTime.Now - LastInterestAdded).Days >= 30)
            {
                decimal interest = Balance * rate;
                Deposit(interest);
                LastInterestAdded = DateTime.Now;
                Console.WriteLine($"Interest of {interest} added to the balance.");
            }
        }
    }

    public class Transaction
    {
        public DateTime Date { get; }
        public string Type { get; }
        public decimal Amount { get; }
        public decimal BalanceAfter { get; }

        public Transaction(string type, decimal amount, decimal balanceAfter)
        {
            Date = DateTime.Now;
            Type = type;
            Amount = amount;
            BalanceAfter = balanceAfter;
        }
    }

    class BankingApp
    {
        private static List<User> Users = new List<User>();
        private static User loggedInUser;
        private static int nextAccountNumber = 1000;
        private const decimal InterestRate = 0.03m; // 3% interest rate

        static void Main()
        {
            Console.WriteLine("Welcome to Console Banking Application!");

            while (true)
            {
                Console.WriteLine("\n1. Register\n2. Login\n3. Exit");
                Console.Write("Choose an option: ");
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1: Register(); break;
                    case 2: Login(); break;
                    case 3: return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }

        static void Register()
        {
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            if (Users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already exists. Try another.");
            }
            else
            {
                Users.Add(new User { Username = username, Password = password });
                Console.WriteLine("Registration successful! Please log in.");
            }
        }

        static void Login()
        {
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            loggedInUser = Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (loggedInUser == null)
            {
                Console.WriteLine("Invalid credentials. Try again.");
            }
            else
            {
                Console.WriteLine("Login successful!");
                UserMenu();
            }
        }

        static void UserMenu()
        {
            while (true)
            {
                Console.WriteLine("\n1. Open Account\n2. Deposit\n3. Withdraw\n4. View Statement\n5. Check Balance\n6. Calculate Interest\n7. Logout");
                Console.Write("Choose an option: ");
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1: OpenAccount(); break;
                    case 2: Deposit(); break;
                    case 3: Withdraw(); break;
                    case 4: ViewStatement(); break;
                    case 5: CheckBalance(); break;
                    case 6: CalculateInterest(); break;
                    case 7: loggedInUser = null; return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }

        static void OpenAccount()
        {
            Console.Write("Enter Account Holder Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Account Type (savings/checking): ");
            string type = Console.ReadLine();
            Console.Write("Enter Initial Deposit: ");
            decimal initialDeposit = decimal.Parse(Console.ReadLine());

            Account newAccount = new Account(nextAccountNumber++, name, type, initialDeposit);
            loggedInUser.Accounts.Add(newAccount);

            Console.WriteLine($"Account created successfully with Account Number: {newAccount.AccountNumber}");
        }

        static void Deposit()
        {
            Account account = SelectAccount();
            if (account != null)
            {
                Console.Write("Enter amount to deposit: ");
                decimal amount = decimal.Parse(Console.ReadLine());
                account.Deposit(amount);
                Console.WriteLine("Deposit successful!");
            }
        }

        static void Withdraw()
        {
            Account account = SelectAccount();
            if (account != null)
            {
                Console.Write("Enter amount to withdraw: ");
                decimal amount = decimal.Parse(Console.ReadLine());
                if (account.Withdraw(amount))
                {
                    Console.WriteLine("Withdrawal successful!");
                }
            }
        }

        static void ViewStatement()
        {
            Account account = SelectAccount();
            if (account != null)
            {
                Console.WriteLine("\nTransaction History:");
                foreach (var transaction in account.Transactions)
                {
                    Console.WriteLine($"{transaction.Date} - {transaction.Type} - Amount: {transaction.Amount} - Balance: {transaction.BalanceAfter}");
                }
            }
        }

        static void CheckBalance()
        {
            Account account = SelectAccount();
            if (account != null)
            {
                Console.WriteLine($"Current Balance: {account.Balance}");
            }
        }

        static void CalculateInterest()
        {
            Account account = SelectAccount();
            if (account != null)
            {
                account.AddInterest(InterestRate);
            }
        }

        static Account SelectAccount()
        {
            Console.Write("Enter Account Number: ");
            int accountNumber = int.Parse(Console.ReadLine());
            Account account = loggedInUser.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                Console.WriteLine("Invalid Account Number.");
            }
            return account;
        }
    }
}
