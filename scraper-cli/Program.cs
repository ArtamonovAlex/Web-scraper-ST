﻿using System;
using System.Collections.Generic;
using System.IO;

namespace scraper_cli
{
    public class Program
    {
        public static string[] MenuOptions =
        {
            "Get raw page content",
            "Parse with active rules",
            "Create parsing rule",
            "Import rules",
            "Export rules",
            "Exit"
        };

        public static string[] RuleExportOptions =
        {
            "Export to JSON",
            "Cancel (or any other key)"
        };

        public static string[] RuleImportOptions =
        {
            "Import from JSON",
            "Cancel (or any other key)"
        };

        public static List<ParsingRule> ParsingRules = new List<ParsingRule>();

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to web scraper!");

            string url, response;

            bool isWorking = true;

            while (isWorking)
            {
                ShowOptions(MenuOptions);
                switch (Console.ReadLine())
                {
                    case "1":

                        Console.WriteLine("Input URL:");
                        url = Console.ReadLine();

                        response = RequestService.SendRequest(url) ?? "Couldn't get response";
                        Console.WriteLine("----------------Start of the page---------------");
                        Console.WriteLine(response);
                        Console.WriteLine("-----------------End of the page----------------");

                        break;

                    case "2":
                        Console.WriteLine("Input URL:");
                        url = Console.ReadLine();

                        response = RequestService.SendRequest(url);

                        if (response != null)
                        {
                            Dictionary<string, string> scrapedValues = ParsePage(response, ParsingRules);
                            Console.WriteLine("Scraped values:");
                            foreach (var item in scrapedValues)
                            {
                                Console.WriteLine($"{item.Key}: {item.Value}");
                            }
                        } else
                        {
                            Console.WriteLine("Couldn't get response");
                        }
                        Console.WriteLine();
                        break;

                    case "3":
                        Console.Write("Enter title: ");
                        string title = Console.ReadLine();

                        Console.Write("Enter prefix: ");
                        string prefix = Console.ReadLine();

                        Console.Write("Enter suffix: ");
                        string sufffix = Console.ReadLine();

                        Console.Write("Enter description: ");
                        string description = Console.ReadLine();

                        ParsingRules.Add(new ParsingRule(prefix, sufffix, title, description));
                        Console.WriteLine("===== Rule saved! =====");
                        break;

                    case "4":
                        ShowOptions(RuleImportOptions);
                        switch (Console.ReadLine())
                        {
                            case "1":
                                Console.Write("Please specify file path: ");
                                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Console.ReadLine());
                                ParsingRules = new List<ParsingRule>(FileService.ImportFromJson<ParsingRule[]>(path));
                                Console.WriteLine("===== Successfully loaded =====");
                                break;
                            default:
                                break;
                        }
                        break;

                    case "5":
                        ShowOptions(RuleExportOptions);
                        switch(Console.ReadLine())
                        {
                            case "1":
                                Console.Write("Please specify file path: ");
                                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Console.ReadLine());
                                FileService.ExportRulesToJson(ParsingRules.ToArray(), path);
                                Console.WriteLine("===== Successfully saved =====");
                                break;
                            default:
                                break;
                        }
                        break;
                    case "6":
                        isWorking = false;
                        break;

                    default:
                        Console.WriteLine("Wrong input");
                        break;
                }
            }
        }

        private static Dictionary<string, string> ParsePage(string pageContent, List<ParsingRule> parsingRules)
        {
            Dictionary<string, string> scrapedValues = new Dictionary<string, string>();
            foreach (var item in parsingRules)
            {
                scrapedValues.Add(item.title, ExtractSrtingBetween(pageContent, item.prefix, item.suffix));
            }
            return scrapedValues;
        }

        private static string ExtractSrtingBetween(string sourceString, string prefix, string suffix)
        {
            int pFrom = sourceString.IndexOf(prefix) + prefix.Length;
            string restString = sourceString.Substring(pFrom);
            int pTo = restString.IndexOf(suffix);
            return (pFrom - prefix.Length != -1 && pTo != -1) ? restString.Substring(0, pTo) : "";
        }

        private static void ShowOptions(string[] options)
        {
            Console.WriteLine("Available options:");
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }
        }
    }
}
