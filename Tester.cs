﻿using System;

namespace SimpleReactionMachine
{
    class Tester
    {
        private static IController controller;
        private static IGui gui;
        private static string displayText;
        private static int randomNumber;
        private static int passed = 0;

        static void Main(string[] args)
        {
            // run simple test
            SimpleTest();
            Console.WriteLine("\n=====================================\nSummary: {0} tests passed out of 38", passed);
            Console.ReadKey();
        }

        private static void SimpleTest()
        {
            //Construct a ReactionController
            controller = new SimpleReactionController();
            gui = new DummyGui();

            //Connect them to each other
            gui.Connect(controller);
            controller.Connect(gui, new RndGenerator());

            //Reset the components()
            gui.Init();

            //Test the SimpleReactionController
            //IDLE
            DoReset('A', controller, "Insert coin");
            DoGoStop('B', controller, "Please Insert Coin First");
            DoTicks('C', controller, 1, "Please Insert Coin First");

            //coinInserted
            DoInsertCoin('D', controller, "Press GO!");

            //READY
            DoTicks('E', controller, 1, "Press GO!");
            DoInsertCoin('F', controller, "Coin Already Inserted");

            //goStop
            randomNumber = 1170;
            DoGoStop('G', controller, "Wait...");

            //WAIT tick(s)
            // My implementation increased the timer by 10 ever tick, as the tick increments every 10 miliseconds in the simple reaction machine
            DoTicks('H', controller, randomNumber - 10, "Wait...");

            //RUN tick(s)
            DoTicks('I', controller, 10, "0.00");
            DoTicks('J', controller, 10, "0.01");
            DoTicks('K', controller, 110, "0.12");
            DoTicks('L', controller, 1110, "1.23");

            //goStop
            DoGoStop('M', controller, "Final Score is 1.23");

            //STOP tick(s)
            DoTicks('N', controller, 2990, "Final Score is 1.23");
            // *********************************new game?
            //tick
            DoTicks('O', controller, 10, "Insert coin");

            //IDLE coinInserted
            DoInsertCoin('P', controller, "Press GO!");

            //READY goStop
            randomNumber = 167;
            DoGoStop('Q', controller, "Wait...");
            // *********************************cheating?
            //WAIT tick(s) goStop
            DoTicks('R', controller, randomNumber - 10, "Wait...");
            DoGoStop('S', controller, "Insert coin");
            // *********************************new game?
            //IDLE init
            gui.Init();
            DoReset('T', controller, "Insert coin");

            //IDLE -> READY init
            randomNumber = 123;
            DoInsertCoin('U', controller, "Press GO!");
            // *********************************new game?	
            gui.Init();
            DoReset('V', controller, "Insert coin");

            //IDLE -> READY ->WAIT init
            randomNumber = 123;
            DoInsertCoin('W', controller, "Press GO!");
            DoGoStop('X', controller, "Wait...");
            // *********************************new game?
            gui.Init();
            DoReset('Y', controller, "Insert coin");

            //IDLE -> READY -> WAIT -> RUN init
            randomNumber = 1370;
            DoInsertCoin('Z', controller, "Press GO!");
            DoGoStop('a', controller, "Wait...");
            DoTicks('b', controller, randomNumber + 980, "0.98");
            // *********************************new game?
            gui.Init();
            DoReset('c', controller, "Insert coin");

            //IDLE -> READY -> WAIT -> RUN -> STOP init
            randomNumber = 119;
            DoInsertCoin('d', controller, "Press GO!");
            DoGoStop('e', controller, "Wait...");
            DoTicks('f', controller, randomNumber + 1350, "1.35");
            DoGoStop('g', controller, "Final Score is 1.35");
            // *********************************new game?
            gui.Init();
            DoReset('h', controller, "Insert coin");

            //IDLE -> READY -> WAIT -> RUN (timeout) -> STOP
            randomNumber = 120;
            DoInsertCoin('i', controller, "Press GO!");
            DoGoStop('j', controller, "Wait...");
            DoTicks('k', controller, randomNumber + 1990, "1.99");
            DoTicks('l', controller, 500, "Final Score is 2.00");
        }

        private static void DoReset(char ch, IController controller, string msg)
        {
            try
            {
                controller.Init();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoGoStop(char ch, IController controller, string msg)
        {
            try
            {
                controller.GoStopPressed();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoInsertCoin(char ch, IController controller, string msg)
        {
            try
            {
                controller.CoinInserted();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoTicks(char ch, IController controller, int n, string msg)
        {
            try
            {
                // My implementation increased the timer by 10 ever tick, as the tick increments every 10 miliseconds in the simple reaction machine
                for (int t = 0; t < n; t+=10) controller.Tick();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void GetMessage(char ch, string msg)
        {
            if (msg.ToLower() == displayText.ToLower())
            {
                Console.WriteLine("test {0}: passed successfully", ch);
                passed++;
            }
            else
                Console.WriteLine("test {0}: failed with message ( expected {1} | received {2})", ch, msg, displayText);
        }

        private class DummyGui : IGui
        {

            private IController controller;

            public void Connect(IController controller)
            {
                this.controller = controller;
            }

            public void Init()
            {
                displayText = "?reset?";
            }

            public void SetDisplay(string msg)
            {
                displayText = msg;
            }
        }

        private class RndGenerator : IRandom
        {
            public int GetRandom(int from, int to)
            {
                return randomNumber;
            }
        }

    }

}
