﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Zork
{
    internal class Program
    {
        static Program() 
        {
            RoomMap = new Dictionary<string, Room>();
            foreach (Room room in Rooms) 
            {
                RoomMap[room.Name] = room;
            }
        }

        private static Room CurrentRoom 
        {
            get 
            {
                return Rooms[Location.Row, Location.Column];
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome To Zork!");

            const string defaultRoomsFilename = "Rooms.txt";
            string roomsFilename = (args.Length > 0 ? args[(int)CommandLineArguments.RoomsFilename] : defaultRoomsFilename);
            InitializeRoomDescriptions(roomsFilename);

            Room previousRoom = null;

            Commands command = Commands.UNKNOWN;
            while (command != Commands.QUIT) 
            {
                Console.WriteLine(CurrentRoom);
                if (previousRoom != CurrentRoom) 
                {
                    Console.WriteLine(CurrentRoom.Description);
                    previousRoom = CurrentRoom;
                }
                Console.Write("> ");
                command = ToCommand(Console.ReadLine().Trim());

                switch (command) 
                {
                    case Commands.QUIT:
                        Console.WriteLine("Thank you for playing!");
                        break;

                    case Commands.LOOK:
                        Console.WriteLine(CurrentRoom.Description);
                        break;

                    case Commands.NORTH:
                    case Commands.SOUTH:
                    case Commands.EAST:
                    case Commands.WEST:
                        if (Move(command) == false) 
                        {
                            Console.WriteLine("The way is shut!");
                        }
                        else 
                        {
                            Console.WriteLine($"You moved {command}.");
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
        }

        private enum CommandLineArguments 
        {
            RoomsFilename = 0
        }

        private static bool Move(Commands command)
        {
            Assert.IsTrue(IsDirection(command), "Invalid Direction");
            bool isValidMove = true;

            switch (command) 
            {
                case Commands.EAST when Location.Column < Rooms.GetLength(1) - 1:
                    Location.Column++;
                    break;

                case Commands.WEST when Location.Column > 0:
                    Location.Column--;
                    break;

                case Commands.NORTH when Location.Row > 0:
                    Location.Row--;
                    break;

                case Commands.SOUTH when Location.Row < Rooms.GetLength(0) - 1:
                    Location.Row++;
                    break;

                default:
                    isValidMove = false;
                    break;
            }

            return isValidMove;
        }

        private static Commands ToCommand(string commandString) => Enum.TryParse<Commands>(commandString, true, out Commands result) ? result : Commands.UNKNOWN;

        private static bool IsDirection(Commands command) => Directions.Contains(command);

        private static readonly Room[,] Rooms = {
            { new Room("Rocky Trail"), new Room("South of House"), new Room("Canyon View") },
            { new Room("Forest"), new Room("West of House"), new Room("Behind House") },
            { new Room("Dense Woods"), new Room("North of House"), new Room("Clearing") }
        };

        private static readonly Dictionary<string, Room> RoomMap;

        private static void InitializeRoomDescriptions(string roomsFilename) 
        {
            const string fieldDelimiter = "##";
            const int expectedFieldCount = 2;
            var roomQuery = from line in File.ReadLines(roomsFilename)
                            let fields = line.Split(fieldDelimiter)
                            where fields.Length == expectedFieldCount
                            select (Name: fields[(int)Fields.Name],
                            Description: fields[(int)Fields.Description]);

            foreach (var (Name, Description) in roomQuery) 
            {
                RoomMap[Name].Description = Description;
            }
            //string[] lines = File.ReadAllLines(roomsFilename);
            //foreach (string line in lines) 
            //{
            //    string[] fields = line.Split(fieldDelimiter);
            //    if (fields.Length != expectedFieldCount) 
            //    {
            //        throw new InvalidDataException("Invalid record.");
            //    }

            //    string name = fields[(int)Fields.Name];
            //    string description = fields[(int)Fields.Description];

            //    RoomMap[name].Description = description;
            //}
        }

        private enum Fields 
        { 
            Name = 0,
            Description
        }

        private static readonly List<Commands> Directions = new List<Commands>
        {
            Commands.NORTH,
            Commands.SOUTH,
            Commands.WEST,
            Commands.EAST
        };

        private static (int Row, int Column) Location = (1, 1);
    }
}
