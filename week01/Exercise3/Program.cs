using System;

class Program
{
    static void Main(string[] args)
    {

        string playAgain;
        Random randomGenerator = new();

        do
        {
            int magicNumber = randomGenerator.Next(1, 101);
            int guess;
            int guessCount = 0;

            Console.WriteLine("Guess the magic number between 1 and 100!");

            do
            {
                Console.Write("What is your guess? ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out guess))
                {
                    Console.WriteLine("Please enter a valid number.");
                    continue;
                }

                guessCount++;

                if (guess < magicNumber)
                {
                    Console.WriteLine("Higher");
                }
                else if (guess > magicNumber)
                {
                    Console.WriteLine("Lower");
                }
                else
                {
                    Console.WriteLine($"You guessed it in {guessCount} guesses!");
                }

            } while (guess != magicNumber);

            Console.Write("Do you want to play again? (yes/no): ");
            playAgain = Console.ReadLine()?.ToLower();

        } while (playAgain == "yes");

        Console.WriteLine("Thanks for playing!");
    }
}