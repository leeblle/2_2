using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;

namespace Lab2._2
{
	public partial class MainWindow : Window
	{
		private readonly List<LibTools> methods = new List<LibTools>();

		public MainWindow() => InitializeComponent();

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			string[] pathsDll = Directory.GetFiles("Plugins", "*.dll", SearchOption.TopDirectoryOnly);
			foreach (var path in pathsDll)
			{
				try
				{
					methods.Add(new LibTools(path));
				}
				catch { }
			}

			foreach (var method in methods)
				ComboBoxFuncs.Items.Add(method.FuncName());
			ComboBoxFuncs.Items.Add("Все");
		}

		private void ButtonApplyFunc_Click(object sender, RoutedEventArgs e)
		{
			ChartTheFunc.Series.Clear();
			ChartValues<ObservablePoint> points = new ChartValues<ObservablePoint>();
			Func<string> funcName = null;
			Func<double, double> theFunc = null;
			var nameMethod = ComboBoxFuncs.Text;
			if (nameMethod == "Все")
			{
				foreach (var method in methods)
				{
					points = new ChartValues<ObservablePoint>();

					funcName = method.FuncName;
					theFunc = method.TheFunc;

					for (double x = 0; x <= 10; x += 0.1)
					{
						//points.Add(new ObservablePoint(x, theFunc(x)));
						if (funcName() == "y=12x+sin(x/2)")
						{
							points.Add(new ObservablePoint(x, theFunc(x * 40)));
						}
						else if (funcName().Contains("cos"))
						{
							points.Add(new ObservablePoint(x, theFunc(x)));
						}
						else
						{
							points.Add(new ObservablePoint(x, theFunc(x)));
						}
					}

					ChartTheFunc.Series.Add(new LineSeries
					{
						Values = points,
						Title = funcName()
					});
				}
			}
			else
			{
				foreach (var method in from method in methods
									   where nameMethod == method.FuncName()
									   select method)
				{
					funcName = method.FuncName;
					theFunc = method.TheFunc;
				}

				for (double x = 0; x <= 10; x += 0.1)
					points.Add(new ObservablePoint(x, theFunc(x)));

				ChartTheFunc.Series.Clear();
				ChartTheFunc.Series.Add(new LineSeries
				{
					Values = points,
					Title = funcName()
				});
			}
		}
	}
}