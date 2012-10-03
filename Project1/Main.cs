using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

//command to run in the terminal to run the program in unix if mono is not install
//sudo yum install mono
//yum list mono*

namespace Project1
{
	static class MainClass
	{
		static List<string> history = new List<string>();

		private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs args)
		{
			for (int i = (history.Count - 1), j = 0; i >= 0; i--, j++) 
			{
				Console.WriteLine(history[i]);
				
				if(j==9) break;
			}

			args.Cancel = true;
		}

		public static void Main (string[] args)
		{
			Console.CancelKeyPress += OnCancelKeyPress;
	
			while (true)
			{
				Console.Write("#ssh>");

				//This is the entire line
				string command = Console.ReadLine();

				//If the command comes out null continue
				if (command == null)
				{
					continue;
				}

				//Check if they typed exit
				if (command.ToLower() == "exit" )
				{
					break;
				}

				//Add to history
				if(command[0] != 'r')
				{
					history.Add(command);
				}

				//Split by the space character
				string [] split = command.Split(' ');

				//This is the process to run
				string exe = split[0];

				//These are the arguments to the program
				string arguments = command.Substring(exe.Length, command.Length - exe.Length).Trim ();

				//If their command was r x
				if (exe.ToLower() == "r")
				{
					for (int i = (history.Count - 1), j = 0; i >= 0; i--, j++) 
					{
						command = history[i];
						if (command.StartsWith(arguments))
						{
							split = command.Split (' ');
							exe = split[0];
							arguments = command.Substring(exe.Length, command.Length - exe.Length).Trim ();

							Fork(exe, arguments);
							break;
						}

						if(j==10)
						{
							Console.WriteLine("Command not found");
							break;
						}
					}
					continue;
				}

				//If their command was r
				if(command.ToLower() == "r")
				{
					command = history.Last();
					if (command.StartsWith(arguments))
					{
						split = command.Split (' ');
						exe = split[0];
						arguments = command.Substring(exe.Length, command.Length - exe.Length).Trim ();
						
						Fork(exe, arguments);
						break;
					}
				}

				Fork (exe, arguments);
			}
		}

		//Forks process to run
		private static void Fork(string exe, string arguments)
		{
			try
			{
				ProcessStartInfo info = new ProcessStartInfo(exe,arguments);
				info.UseShellExecute = false;
				info.RedirectStandardError = true;
				info.RedirectStandardOutput = true;
				
				using( var process = Process.Start(info))
				{
					//Waits for process to end
					process.WaitForExit();
					
					string error = process.StandardError.ReadToEnd().Trim();
					string output = process.StandardOutput.ReadToEnd().Trim();

					if(!string.IsNullOrEmpty(error))
					{
						Console.WriteLine(error);  
					}
					
					if(!string.IsNullOrEmpty(output))
					{
						Console.WriteLine(output);
					}
				}
			}

			catch(Exception exc)
			{
				Console.WriteLine("Error." + exc.Message);
			}
		}
	}
}
