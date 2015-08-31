using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infrastructure
{
	public class TransitionContentControl : Control
	{
		private ContentControl currentWrappedContent = null;
		private Grid grid = null;

	    private const string DefaultTemplateMarkup = "<ControlTemplate xmlns=\"http://schemas.microsoft.com/client/2007\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><Grid x:Name=\"Grid\"/></ControlTemplate>";
		
		public TransitionContentControl()
		{
		    base.Template = System.Windows.Markup.XamlReader.Parse(DefaultTemplateMarkup) as ControlTemplate;
			this.Unloaded += new System.Windows.RoutedEventHandler(TransitionContentControl_Unloaded);
		}
		
		public static readonly DependencyProperty TransitionStyleProperty = DependencyProperty.Register("TransitionStyle", typeof(Style), typeof(TransitionContentControl), new PropertyMetadata(null, TransitionStylePropertyChanged));
		public Style TransitionStyle
		{
			get { return (Style)GetValue(TransitionStyleProperty); }
			set { SetValue(TransitionStyleProperty, value); }
		}

		public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(TransitionContentControl), new PropertyMetadata(null, ContentPropertyChanged));
		public object Content
		{
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		public static readonly DependencyProperty ReversedProperty = DependencyProperty.Register("Reversed", typeof(bool), typeof(TransitionContentControl), new PropertyMetadata(false));
		public bool Reversed
		{
			get { return (bool)GetValue(ReversedProperty); }
			set { SetValue(ReversedProperty, value); }
		}

		public override void OnApplyTemplate()
		{
			if (this.currentWrappedContent != null)
			{
				SetContent(this.currentWrappedContent, null);
			}
			
			base.OnApplyTemplate();
			this.grid = (Grid)this.GetTemplateChild("Grid");

			if (this.currentWrappedContent != null)
			{
				this.grid.Children.Add(this.currentWrappedContent);
			}
		}

		protected virtual void OnContentChanged(object oldContent, object newContent)
		{
			if (this.grid == null)
			{
				this.ApplyTemplate();
			}

			ContentControl oldWrapper = this.currentWrappedContent;
			
			if (oldWrapper != null)
			{
				VisualStateGroup layoutStatesGroup = FindNameInWrapper(oldWrapper, "LayoutStates") as VisualStateGroup;
				
				if (layoutStatesGroup == null)
				{
					this.grid.Children.Remove(oldWrapper);
					SetContent(oldWrapper, null);
				}
				else
				{
					layoutStatesGroup.CurrentStateChanged += delegate(object sender, VisualStateChangedEventArgs args)
					{
						this.grid.Children.Remove(oldWrapper);
						SetContent(oldWrapper, null);
					};
					VisualStateManager.GoToState(oldWrapper, this.Reversed ? "BeforeLoaded" : "BeforeUnloaded", true);
				}
			}

			ContentControl newWrapper = new ContentControl();
			newWrapper.Style = this.TransitionStyle;
			newWrapper.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			newWrapper.VerticalContentAlignment = VerticalAlignment.Stretch;

			this.grid.Children.Add(newWrapper);
			newWrapper.ApplyTemplate();
			if (this.TransitionStyle != null)
			{
				SetContent(newWrapper, newContent);
				if (oldWrapper != null)
				{
					VisualStateManager.GoToState(newWrapper, this.Reversed ? "BeforeUnloaded" : "BeforeLoaded", false);
					VisualStateManager.GoToState(newWrapper, "AfterLoaded", true);
				}
			}

			this.currentWrappedContent = newWrapper;
		}

		static void ContentPropertyChanged(object o, DependencyPropertyChangedEventArgs args)
		{
			((TransitionContentControl)o).OnContentChanged(args.OldValue, args.NewValue);
		}
		
		protected virtual void OnTransitionStyleChanged(Style oldContent, Style newContent)
		{
			if (this.currentWrappedContent != null)
			{
				SetContent(this.currentWrappedContent, null);
				
				this.currentWrappedContent.Style = newContent;
				this.currentWrappedContent.ApplyTemplate();
				
				SetContent(this.currentWrappedContent, this.Content);
			}
		}

		static void TransitionStylePropertyChanged(object o, DependencyPropertyChangedEventArgs args)
		{
			((TransitionContentControl)o).OnTransitionStyleChanged((Style)args.OldValue, (Style)args.NewValue);
		}
		
		private static void SetContent(ContentControl wrapper, object content)
		{
			ContentPresenter contentPresenter = FindNameInWrapper(wrapper, "contentPresenter") as ContentPresenter;
			if (contentPresenter != null)
			{
				contentPresenter.Content = content;
			}
		}
		
		private static object FindNameInWrapper(ContentControl wrapper, string childName)
		{
			if (VisualTreeHelper.GetChildrenCount(wrapper) != 0)
			{
				FrameworkElement wrapperRoot = VisualTreeHelper.GetChild(wrapper, 0) as FrameworkElement;
				if (wrapperRoot != null)
				{
					return wrapperRoot.FindName(childName);
				}			
			}
			
			return null;
		}

		private void TransitionContentControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (this.currentWrappedContent != null)
			{
				SetContent(this.currentWrappedContent, null);
			}
		}
	}
}