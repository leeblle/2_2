using System;
using System.Runtime.InteropServices;

namespace Lab2._2
{
	public class LibTools
	{
		[Flags]
		private enum ErrorModes : uint
		{
			SYSTEM_DEFAULT = 0x0,
			SEM_FAILCRITICALERRORS = 0x0001,
			SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
			SEM_NOGPFAULTERRORBOX = 0x0002,
			SEM_NOOPENFILEERRORBOX = 0x8000
		}

		public double TheFunc(double x) => _theFunc(x);
		public string FuncName() => Marshal.PtrToStringAnsi(_funcName());

		private readonly _TheFuncDelegate _theFunc;
		private readonly _FuncNameDelegate _funcName;

		[DllImport("kernel32.dll")] private static extern IntPtr LoadLibrary(string path);
		[DllImport("kernel32.dll")] private static extern ErrorModes SetErrorMode(ErrorModes modes);
		[DllImport("kernel32.dll")] private static extern IntPtr GetProcAddress(IntPtr hLib, string name);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate double _TheFuncDelegate(double x);
		[UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate IntPtr _FuncNameDelegate();

		public LibTools(string path)
		{
			IntPtr hLib = GetLibrary(path);
			if (hLib != IntPtr.Zero)
			{
				_theFunc = GetFunc<_TheFuncDelegate>("TheFunc", hLib);
				_funcName = GetFunc<_FuncNameDelegate>("FuncName", hLib);
			}
			else
				throw new Exception();
		}

		private IntPtr GetLibrary(string path)
		{
			var oldMode = SetErrorMode(ErrorModes.SEM_FAILCRITICALERRORS);
			try
			{
				return LoadLibrary(path);
			}
			finally
			{
				SetErrorMode(oldMode);
			}
		}

		private T GetFunc<T>(string funcName, IntPtr hLib) where T : class
		{
			IntPtr address = GetProcAddress(hLib, funcName);
			if (address != IntPtr.Zero)
				return Marshal.GetDelegateForFunctionPointer(address, typeof(T)) as T;
			else
				throw new Exception();
		}
	}
}