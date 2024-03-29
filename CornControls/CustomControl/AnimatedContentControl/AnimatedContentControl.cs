﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using PropertyChanged;

namespace CornControls.CustomControl
{
	[TemplatePart(Name = "PART_PaintArea", Type = typeof(Shape)),
	 TemplatePart(Name = "PART_MainContent", Type = typeof(ContentPresenter)), AddINotifyPropertyChangedInterface]
	public class AnimatedContentControl : ContentControl
	{
		#region Generated static constructor
		public readonly static DependencyProperty AnimationTimeProperty = DependencyProperty.Register(nameof(AnimationTime), typeof(double), typeof(AnimatedContentControl));
		public readonly static DependencyProperty OutDelayProperty = DependencyProperty.Register(nameof(OutDelay), typeof(double), typeof(AnimatedContentControl), new PropertyMetadata(0.0));

		static AnimatedContentControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedContentControl), new FrameworkPropertyMetadata(typeof(AnimatedContentControl)));
		}
		#endregion

		protected Shape m_paintArea;
		protected ContentPresenter m_mainContent;

		public double AnimationTime
        {
			get => (double)GetValue(AnimationTimeProperty);
			set => SetValue(AnimationTimeProperty, value);
        }
		public double OutDelay
        {
			get => (double)GetValue(OutDelayProperty);
			set => SetValue(OutDelayProperty, value);
        }

        public override void OnApplyTemplate()
		{
			m_paintArea = Template.FindName("PART_PaintArea", this) as Shape;
			m_mainContent = Template.FindName("PART_MainContent", this) as ContentPresenter;

			base.OnApplyTemplate();
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			if (m_paintArea != null && m_mainContent != null)
			{
				m_paintArea.Fill = CreateBrushFromVisual(m_mainContent);
				m_paintArea.Width = m_mainContent.ActualWidth;
				m_paintArea.Height = m_mainContent.ActualHeight;
				BeginAnimateContentReplacement();
			}
			base.OnContentChanged(oldContent, newContent);
		}

		protected virtual void BeginAnimateContentReplacement()
		{
			var newContentTransform = new TranslateTransform();
			var oldContentTransform = new TranslateTransform();
			m_paintArea.RenderTransform = oldContentTransform;
			m_mainContent.RenderTransform = newContentTransform;

			Dispatcher.Invoke(() =>
			{
				oldContentTransform.BeginAnimation(TranslateTransform.XProperty, CreateAnimation(0, this.ActualWidth, 0.3, EasingMode.EaseIn, (s, e) =>
				{
					m_paintArea.Visibility = Visibility.Visible;
					Task.Delay((int)(OutDelay * 1000)).ContinueWith(t =>
					{
						Dispatcher.Invoke(() =>
						{
							newContentTransform.BeginAnimation(TranslateTransform.XProperty, CreateAnimation(this.ActualWidth, 0, 0.3, EasingMode.EaseIn));
							m_paintArea.Visibility = Visibility.Hidden;
						}, DispatcherPriority.Background);
					});
				}));
			}, DispatcherPriority.Background);
		}

		/// <summary>
		/// Creates the animation that moves content in or out of view.
		/// </summary>
		/// <param name="from">The starting value of the animation.</param>
		/// <param name="to">The end value of the animation.</param>
		/// <param name="whenDone">(optional) A callback that will be called when the animation has completed.</param>
		protected virtual AnimationTimeline CreateAnimation(double from, double to, double amplitude, EasingMode mode, EventHandler whenDone = null)
		{
			IEasingFunction ease = new BackEase { Amplitude = amplitude, EasingMode = mode };
			var duration = new Duration(TimeSpan.FromSeconds(AnimationTime / 2));
			var anim = new DoubleAnimation(from, to, duration) { EasingFunction = ease };
			if (whenDone != null)
				anim.Completed += whenDone;
			anim.Freeze();
			return anim;
		}

		/// <summary>
		/// Creates a brush based on the current appearnace of a visual element. The brush is an ImageBrush and once created, won't update its look
		/// </summary>
		/// <param name="v">The visual element to take a snapshot of</param>
		protected virtual Brush CreateBrushFromVisual(Visual v)
		{
			if (v == null)
				throw new ArgumentNullException("v");
			var target = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			target.Render(v);
			var brush = new ImageBrush(target);
			brush.Freeze();
			return brush;
		}
	}
}
