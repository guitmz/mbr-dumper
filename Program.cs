// 2014 copyright Guilherme Thomazi (thomazi@linux.com)
// Released under the GPL v2 

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace MBR
{
	class MainClass
	{

		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern uint SetFilePointer(
			[In] SafeFileHandle hFile,
			[In] int lDistanceToMove,
			[Out] out int lpDistanceToMoveHigh,
			[In] EMoveMethod dwMoveMethod);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
			uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
			uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("kernel32", SetLastError = true)]
		internal extern static int ReadFile(SafeFileHandle handle, byte[] bytes,
			int numBytesToRead, out int numBytesRead, IntPtr overlapped_MustBeZero);


		public enum EMoveMethod : uint
		{
			Begin = 0,
			Current = 1,
			End = 2
		}

		public static void Main (string[] args)
		{
			Console.Title = "MBR Dumper";
			Console.WriteLine ("Dump MBR to raw.bin? (Y or N)");
			string ans = Console.ReadLine ();

			if (ans == "Y" || ans == "y") { 
				Console.WriteLine("\nDumping...");
				Dump ();
				Console.WriteLine("Done!");
				Console.ReadKey (true);
			} else {
				Environment.Exit (0);
			}

		}

		public static void Dump() {

			uint GENERIC_READ = 0x80000000;
			uint OPEN_EXISTING = 3;

			SafeFileHandle handleValue = CreateFile (@"\\.\PHYSICALDRIVE0", GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
			if (handleValue.IsInvalid) {
				Marshal.ThrowExceptionForHR (Marshal.GetHRForLastWin32Error ());
			}
			int offset = int.Parse ("0", System.Globalization.NumberStyles.HexNumber);
			int size = int.Parse ("200", System.Globalization.NumberStyles.HexNumber);
			byte[] buf = new byte[size];
			int read = 0;
			int moveToHigh;
			SetFilePointer (handleValue, offset, out moveToHigh, EMoveMethod.Begin);
			ReadFile (handleValue, buf, size, out read, IntPtr.Zero);
			FileStream myStream = File.OpenWrite ("raw.bin");
			myStream.Write (buf, 0, size);
			myStream.Flush ();
			myStream.Close ();
			handleValue.Close ();

		}
			

	}
}
	











