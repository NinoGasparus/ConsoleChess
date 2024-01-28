using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using static referenc.referenc;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Data.Odbc;
using chess_2._0.LegalityChecks;
using System.CodeDom.Compiler;
using System.Xml;

namespace chess_2._0
{

    public class chessboard
    {
        string IpAddressx;
        int port;
        string username;
        string hostStartColor;
        string clientIP;
       public static string[,] board =
        {
            {" *R"," *N"," *B"," *K"," *Q"," *B"," *N"," *R" },
            {" *P"," *P"," *P"," *P"," *P"," *P"," *P"," *P" },
            {"   ","   ","  N","   ","   ","   ","   ","   " },
            {"   ","   ","   ","   ","   ","   ","   ","   " },
            {"   ","   ","   ","   ","   ","   ","   ","   " },
            {"   ","   ","   ","   ","   ","   ","   ","   " },
            {"  P","  P","  P","  P","  P","  P","  P","  P" },
            {"  R","  N","  B","  Q","  K","  B","  N","  R" }
        };


        

        public chessboard(string ip, int port, string username) //call this constructor when joing
        {
            this.IpAddressx = ip;
            this.port = port;
            this.username = username;
         //   drawboard();
            startgameClient();
        }
        public chessboard(int port) //call this constructor when hosting 
        {

            this.port = port;

            Console.Clear();

            startgameHost();
        }



        public static string[] Queue = new string[2];//stores 2 moves after being recieved from other guy
        public static string[] Innbox = { "", "" }; // stores 2 moves to be sent






        void startgameHost()
        {
            Menu.displaypage(4);
            clientIP = TC.initialsetupH(port);

            // Console.WriteLine(clientIP);
            Console.WriteLine();
            Console.WriteLine("Starting the game.");

            Thread.Sleep(1000);
         
            bool yourTurn = true;

         //  Thread x = new Thread(hostReceptionLoop); x.Start();
         //   Thread y = new Thread(hostSendLoop); y.Start();

            while (true)
            {
                gameOverSequence();
                cinterface.rd();

              

                if (yourTurn)
                {
                    cinterface.wplay();
                    while (true)
                    {
                        string move = Console.ReadLine();
                        if (!MoveLegalityChecker.isMoveLegal(move, yourTurn, true))
                        {
                           
                            cinterface.rd();
                            Console.WriteLine("\n\n   WHITE's turn");
                            cinterface.illegal();
                        }
                        else
                        {
                            //while (true)
                            //{
                         //   illcheck:
                                Console.Write("   Move selected piece to (e.g., H2, move to h2): ");
                                string move2 = Console.ReadLine();
                                char[] temp = move.ToCharArray();
                         //   Console.WriteLine(move2);
                                move2 = temp[0].ToString() + move2;
                         //   Console.WriteLine(move2);
                         //   Console.WriteLine("checkpoint before or after wait");
                        //    move2 = caseFormatMove(move2, yourTurn);
                          //  Console.WriteLine(yourTurn);
                           // var temporaryDebugX = Compose<object>(MoveLegalityChecker.isMoveLegal, () => !MoveLegalityChecker.isMoveLegal(move2, yourTurn, true));
                                if (!MoveLegalityChecker.isMoveLegal(move2, yourTurn, false))
                                {
                                    cinterface.rd();
                                    Console.WriteLine("\n\n   WHITE's turn");
                                    cinterface.illegal();
                               // goto illcheck;
                                }
                                else
                                {
                                    if (IndPiceCheck.piceLegal(move, move2, yourTurn))
                                    {
                                        movePice(move, move2);
                                        Queue[0] = move;
                                        Queue[1] = move2;
                                        //  Console.WriteLine("bout to send first message");
                                        TC.hostMoveSequence(move, move2, clientIP, port);
                                        yourTurn = false;
                                       
                                        break;


                                    }
                                }
                           // goto illcheck;
                           // }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("not your turn");
                    //Console.ReadKey();

                    // Console.WriteLine("entered listen loop");
                    string[] recievedMove = TC.hostRecieveMove(port);
                    //Console.WriteLine("x");
                    string x = caseFormatMove(recievedMove[0], yourTurn);
                   // Console.WriteLine("y");
                    string y = caseFormatMove(recievedMove[1], yourTurn);
                   // Console.WriteLine("z");

                    movePice(x, y);
                    Innbox[0] = "";
                    Innbox[1] = "";
                    yourTurn = true;
                }
            }
        }


        void startgameClient()
        {
            Console.CursorTop += 5;
            Console.WriteLine("Connecting...");
            TC.initialsetupC(IpAddressx, port, username);

            bool yourTurn = false;


            if (gameReady == true)
            {
                while (true)
                {
                    gameOverSequence();
                    Console.Clear();
                    drawBoard(true);

                    if (yourTurn)
                    {
                        cinterface.wplay();
                        while (true)
                        {
                            string move = Console.ReadLine();
                            if (!MoveLegalityChecker.isMoveLegal(move, false, true))
                            {
                                Console.Clear();
                                drawBoard(true);
                                Console.WriteLine("\n\n   WHITE's turn");
                                cinterface.illegal();
                            }
                            else
                            {
                                Console.Write("   Move selected piece to (e.g., H2, move to h2): ");
                                string move2 = Console.ReadLine();
                                char[] temp = move.ToCharArray();
                                move2 = temp[0] + move2;
                                if (!MoveLegalityChecker.isMoveLegal(move2, false, false))
                                {
                                    Console.Clear();
                                    drawBoard(true);
                                    Console.WriteLine("\n\n   WHITE's turn");
                                    cinterface.illegal();
                                }
                                else
                                {
                                    if (IndPiceCheck.piceLegal(move, move2, false))
                                    {



                                        TC.clientMoveSequence(move, move2, IpAddressx, port);
                                        yourTurn = false;
                                        break;

                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                        string[] RecievedMoves = TC.clientRecieveMove(port);
                        movePice(RecievedMoves[0], RecievedMoves[1]);
                        Innbox[0] = "";
                        Innbox[1] = "";
                        yourTurn = true;

                    }


                }
            }Menu.displaypage(2);
        }








      public static  void movePice(string a, string b)
        {
            //  Console.WriteLine("move pice was called");
            char[] move = a.ToCharArray();
            char[] move2 = b.ToCharArray();

            string[] Move = new string[move.Length];
            string[] Move2 = new string[move2.Length];

            for (int i = 0; i < move.Length; i++)
            {
                Move[i] = move[i].ToString();
            }
            for (int i = 0; i < move.Length; i++)
            {
                Move2[i] = move2[i].ToString();
            }

            int initialX = Convert.ToInt32(move[1] - 97);
            int finalX = Convert.ToInt32(move2[1] - 97);

            int initialY = (int)char.GetNumericValue(move[2]) - 1;
            int finalY = (int)char.GetNumericValue(move2[2]) - 1;
            //   Console.WriteLine("initialY " + initialY);
            //   Console.WriteLine("finalY " + finalY);
            //   Console.WriteLine("initialX " + initialX);
            //    Console.WriteLine("finalX   " + finalX);



            //    Console.WriteLine(board[initialY, initialX]);
            //    Console.WriteLine(board[finalY, finalX]);


            board[finalY, finalX] = "";
            board[finalY, finalX] = board[initialY, initialX];
            board[initialY, initialX] = "   ";
            //  Console.ReadKey();
        }

        public static void drawboard()
        {
            string leftOffSet = "   ";
            for (int i = 1; i <= 8; i++)
            {
                Console.Write(leftOffSet + " ");


                for (int j = 0; j < 8; j++)
                {
                    Console.Write("+---");
                }
                Console.WriteLine("+");
                Console.Write(leftOffSet);
                Console.Write(i);
                for (int j = 0; j < 8; j++)
                {
                    Console.Write("|" + board[i - 1, j]);
                }
                Console.WriteLine("|");
            }
            Console.Write(leftOffSet + " ");
            for (int j = 0; j < 8; j++)
            {
                Console.Write("+---");
            }
            Console.WriteLine("+");

            char[] columns = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            Console.Write(leftOffSet + "   ");
            foreach (char c in columns)
            {
                Console.Write(c + "   ");
            }
        }  //draws out the board and places the pices on their place according to board array
        public static void drawBoard(bool b)
        {
            string leftOffSet = "   ";
            for (int i = 8; i >= 1; i--)
            {
                Console.Write(leftOffSet + " ");


                for (int j = 0; j < 8; j++)
                {
                    Console.Write("+---");
                }
                Console.WriteLine("+");
                Console.Write(leftOffSet);
                Console.Write(i);
                for (int j = 0; j < 8; j++)
                {
                    Console.Write("|" + board[i - 1, j]);
                }
                Console.WriteLine("|");
            }
            Console.Write(leftOffSet + " ");
            for (int j = 0; j < 8; j++)
            {
                Console.Write("+---");
            }
            Console.WriteLine("+");

            char[] columns = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            Console.Write(leftOffSet + "   ");
            foreach (char c in columns)
            {
                Console.Write(c + "   ");
            }
        }  //draws out the board

        public static bool[] isGameOver()
        {
            bool[] kings = { false, false };
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if (board[i,j] == "  K")
                    {
                        kings[0] = true;
                    }
                    if (board[i,j]== " *K")
                    {
                        kings[1] = true;
                    }
                }
            }
            return kings;
            

        }

        public static string caseFormatMove(string move, bool player)
        {

            char[] c = move.ToCharArray();
            string[] Move = new string[c.Length];

            for (int i = 0; i < c.Length; i++) { Move[i] = c[i].ToString(); }


            if (player)
            {
                Move[0] = Move[0].ToUpper();
                Move[1] = Move[1].ToLower();


            }
            else
            {
                Move[0] = Move[0].ToUpper();
                Move[1] = Move[1].ToLower();
            }

            string ret = "";
            for (int i = 0; i < Move.Length; i++)
            {
                ret += Move[i].ToString();
            }
            return ret;
        }

        static void gameOverSequence()
        {
            bool[] temp = isGameOver();

            if (temp[0] == false)
            {
                Menu.displaypage(5);
            }else if (temp[1] == false)
            {
                Menu.displaypage(5);
            }
        }
    }
}
