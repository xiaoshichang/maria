﻿using System;
using Maria.Server.Application.Server.GameServer;
using Maria.Server.Application.Server.GateServer;
using Maria.Server.Application.Server.GMServer;
using Maria.Server.Log;

#pragma warning disable CS8618

namespace Maria.Server.Application
{
	
	public static class Program
	{
		private static void _SetNativeDllSearchPath()
		{
			var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
			if (path != string.Empty)
			{
				path += ";";
			}
			var nativeDllSearchPath = Environment.GetEnvironmentVariable("NativeDllSearchPath");
			if (string.IsNullOrEmpty(nativeDllSearchPath))
			{
				throw new NullReferenceException("Native dll search path was not found.");
			}
			path += nativeDllSearchPath;
			Environment.SetEnvironmentVariable("PATH", path);
		}
		
		private static void _LoadConfig()
		{
			var configPath = Environment.GetEnvironmentVariable("ConfigPath");
			if (configPath == null)
			{
				throw new Exception("ConfigPath not found in EnvironmentVariable!");
			}
			var groupConfig = ServerGroupConfig.LoadConfig(configPath);
			if (groupConfig == null)
			{
				throw new Exception($"Load group config fail!");
			}
			var serverName = Environment.GetEnvironmentVariable("ServerName");
			if (serverName == null)
			{
				throw new Exception($"ServerName not found in EnvironmentVariable!");
			}
			var serverConfig = groupConfig.GetServerConfigByName(serverName);
			if (serverConfig == null)
			{
				throw new Exception($"{serverName} config not found in group config!");
			}

			ServerGroupConfig = groupConfig;
			ServerConfig = serverConfig;
		}
		
		private static void _InitLogger()
		{
			if (ServerGroupConfig == null || ServerConfig == null)
			{
				throw new Exception("ServerGroupConfig and ServerConfig not set!");
			}
			
			var path = ServerGroupConfig.Common.LogPath;
			var logFile = $"{ServerGroupConfig.Common.LogPath}/{ServerConfig.Name}.log";
			Logger.Init(path, logFile);
		}
		
		private static void _LogApplicationEnvironment()
		{
			Logger.Info($"Server: {ServerConfig?.Name}");
			Logger.Info($"Pid: {Environment.ProcessId}");
			Logger.Info($"CurrentDirectory: {Environment.CurrentDirectory}");
			Logger.Info($"OS: {Environment.OSVersion.Platform.ToString()}");
			Logger.Info($"ProcessorCount: {Environment.ProcessorCount}");
		}
		
		private static void _CreateServerByServerConfig()
		{
			if (ServerConfig is GMServerConfig)
			{
				Server = new GMServer();
			}
			else if (ServerConfig is GameServerConfig)
			{
				Server = new GameServer();
			}
			else if (ServerConfig is GateServerConfig)
			{
				Server = new GateServer();
			}
			else
			{
				throw new Exception("unknown server type");
			}
		}
		
		static void Main(string[] args)
		{
			try
			{
				_SetNativeDllSearchPath();
				_LoadConfig();
				_InitLogger();
				_LogApplicationEnvironment();
				_CreateServerByServerConfig();
				
				Server.Init();
				Server.Run();
				Server.UnInit();
			}
			catch (Exception e)
			{
				Logger.Error(e.Message);
				if (e.StackTrace != null)
				{
					Logger.Error(e.StackTrace);
				}
				Logger.Error("Unhandled exception!\n Exiting......");
				Environment.Exit(1);
			}
		}
		
		
		public static ServerGroupConfig ServerGroupConfig;
		public static ServerConfigBase ServerConfig;
		public static Server.ServerBase.ServerBase Server;
	}
}

