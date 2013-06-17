using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Timers;

namespace WpfCanvasTest
{
	public partial class Window1 : Window
	{
		const double gap = 15.0; // pixel gap between each TextBlock
		const int timer_interval = 1; // number of ms between timer ticks. 16 is near 1/60th second, for smoother updates on LCD displays
		const double move_amount = 2; // number of pixels to move each timer tick. 1 - 1.5 is ideal, any higher will introduce judders

		private LinkedList<TextBlock> textBlocks = new LinkedList<TextBlock>();
		private Timer timer = new Timer();

		public Window1()
		{
			InitializeComponent();

			// A snapshot of today's news (16th July 2009)

			AddTextBlock("Headlines:");
			AddTextBlock("Leons test new ticker");
			AddTextBlock("And another line of text");
			
            //AddTextBlock("England batsmen struggle in final session on first day of second test against Australia for Ashes");

			canvas1.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
			{
				var node = textBlocks.First;
				
				while (node != null)
				{
					double left = 0;

					if (node.Previous != null)
					{
						Canvas.SetLeft(node.Value, Canvas.GetLeft(node.Previous.Value) + node.Previous.Value.ActualWidth + gap);
					}
					else
					{
						Canvas.SetLeft(node.Value, canvas1.Width + gap);
					}
					
					node = node.Next;
				}

				return null;

			}), null);

			timer.Interval = timer_interval;
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			timer.Start();
		}

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			canvas1.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
			{
				var node = textBlocks.First;
				var lastNode = textBlocks.Last;

				while (node != null)
				{
					double newLeft = Canvas.GetLeft(node.Value) - move_amount;

					if (newLeft < (0 - (node.Value.ActualWidth + gap)))
					{
						textBlocks.Remove(node);

						var lastNodeLeftPos = Canvas.GetLeft(lastNode.Value);

						textBlocks.AddLast(node);

						if ((lastNodeLeftPos + lastNode.Value.ActualWidth + gap)> canvas1.Width) // Last element is offscreen
						{
							newLeft = lastNodeLeftPos + lastNode.Value.ActualWidth + gap;
						}
						else
						{
							newLeft = canvas1.Width + gap;
						}
					}
					
					Canvas.SetLeft(node.Value, newLeft);

					node = node == lastNode ? null : node.Next;
				}

				return null;

			}), null);
		}

		void AddTextBlock(string Text)
		{
			TextBlock tb = new TextBlock();
			tb.Text = Text;
			tb.FontSize = 72;
			tb.FontWeight = FontWeights.Bold;
			tb.Foreground = Brushes.Black;

			canvas1.Children.Add(tb);
           
			Canvas.SetTop(tb, 20);
			Canvas.SetLeft(tb, -999);

			textBlocks.AddLast(tb);
		}
	}
}
