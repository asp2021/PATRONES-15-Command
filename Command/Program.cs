using System;
using System.Collections.Generic;
using System.Linq;

namespace Command
{
    public class Product
    {
        public Product(string name, double price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; set; }
        public double Price { get; set; }

        public void IncreasedPrice(double amount)
        {
            Price += amount;
            Console.WriteLine($"El precio del producto {Name} se incremento por {amount}.");
        }

        public bool DecreasedPrice(double amount)
        {
            if (amount < Price)
            {
                Price -= amount;
                Console.WriteLine($"El precio del producto {Name} disminuyo {amount}.");
                return true;
            }
            return false;
        }

        public override string ToString() => $"El precio actual de {Name} es {Price}.";
    }

    public interface ICommand
    {
        void Executed();
        void Undo();
    }

    public enum PriceAction
    {
        Increase,
        Decrease
    }

    public class ProductCommand : ICommand
    {
        private Product _product;
        private PriceAction _priceAction;
        private double _amount;

        public bool IsCommandExecuted { get; private set; }
        public ProductCommand(Product product, PriceAction priceAction, double amount)
        {
            _product = product;
            _priceAction = priceAction;
            _amount = amount;
        }

        public void Executed()
        {
            if (_priceAction == PriceAction.Increase)
            {
                _product.IncreasedPrice(_amount);
                IsCommandExecuted = true;
            }
            else
            {
                IsCommandExecuted = _product.DecreasedPrice(_amount);
            }
        }

        public void Undo()
        {
            if (!IsCommandExecuted)
                return;
            if (_priceAction == PriceAction.Increase)
            {
                _product.DecreasedPrice(_amount);
            }
            else
            {
                _product.IncreasedPrice(_amount);
            }
        }
    }

    // Invoker
    public class ModifyPrice
    {
        private List<ICommand> _commands;
        private ICommand _command;

        public ModifyPrice()
        {
            _commands = new List<ICommand>();
        }

        public void SetCommand(ICommand command) => _command = command;

        public void Invoke()
        {
            _commands.Add(_command);
            _command.Executed();
        }

        public void Undo()
        {
            foreach(var command in Enumerable.Reverse(_commands))
            {
                command.Undo();
            }
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("COMMAND" + "\n");
            Console.WriteLine("Poder convertir una solicitud en un objeto. Este objeto va a tener toda la informacion de dicha solicitud.");
            Console.WriteLine("Uso: para solicitudes en cola o rastreo de las solicitudes que se envian al servidor." + "\n");


            var modifyPrice = new ModifyPrice();
            var product = new Product("Iphone", 5000);

            var productcommand = new ProductCommand(product, PriceAction.Increase, 200);
            modifyPrice.SetCommand(productcommand);
            modifyPrice.Invoke();

            var productcommand1 = new ProductCommand(product, PriceAction.Decrease, 100);
            modifyPrice.SetCommand(productcommand1);
            modifyPrice.Invoke();
            Console.WriteLine(product + Environment.NewLine);

            Console.WriteLine("UNDO");
            modifyPrice.Undo();
            Console.WriteLine(product);

            Console.ReadLine();
        }
    }
}
